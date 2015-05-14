#region Usings

using System;
using System.ComponentModel;
using System.Threading;
using Urbetrack.Backbone;
using Urbetrack.Configuration;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Fleet
{
    public class SpineClientWrap
    {
        public enum States
        {
            [Description("Conectado;Green")] CONNECTED,
            [Description("Conectando;Blue")] CONNECTING,
            [Description("Desconectado;Red")] DISCONNECTED
        };

        public States State;

        private readonly int disconnected_comfort_timeout;
        private readonly int connected_comfort_timeout;

        private readonly string ServerUri;
        private readonly string Description;
        public bool Running;
        private SpineClient SpineClient;

        public event SpineCommandHandler SpineCommand;
        public event SpineStateChangedHandler SpineStateChanged;

        public SpineClientWrap(string serverUri, string description)
        {
			disconnected_comfort_timeout = Config.Spine.DisconnectedComfortTimeout;
        	connected_comfort_timeout = Config.Spine.ConnectedComfortTimeout;
            ServerUri = serverUri;
            Description = description;
            State = States.DISCONNECTED;
            Running = !serverUri.Equals(String.Empty);
        }

        public void MainLoop()
        {
            while (Running)
            {
                try
                {
                    if (State == States.DISCONNECTED)
                    {
                        Thread.Sleep(disconnected_comfort_timeout);
                        STrace.Debug(GetType().FullName, "SPINE_CLIENT_WRAP: Conectando al SPINE.");
                        SpineClient = new SpineClient(ServerUri, Description);
                        State = States.CONNECTING;
                        if (SpineClient.Connect())
                        {
                            STrace.Debug(GetType().FullName, "SPINE_CLIENT_WRAP: CONECTADO.");
                            State = States.CONNECTED;
                            SpineClient.SpineCommand += FireSpineCommand;
                            SpineClient.SpineStateChanged += FireSpineStateChange;
                        } else
                        {
                            State = States.DISCONNECTED;
                            STrace.Debug(GetType().FullName,"SPINE_CLIENT_WRAP: FALLO LA CONEXION.");
                        }
                    }
                    else
                    {
                        Thread.Sleep(connected_comfort_timeout);
                        STrace.Debug(GetType().FullName, "SPINE_CLIENT_WRAP: Disparo HEARTBEAT.");
                    	if (SpineClient != null) SpineClient.Spine.GatewayHeartbeat(Description);
                    }
                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName, e);
                    STrace.Debug(GetType().FullName,"SPINE_CLIENT_WRAP: ERROR INICIANDO EL CLIENTE DE SPINE.");
                    State = States.DISCONNECTED;
                    SpineClient = null;
                }
            }
        }

        private void FireSpineStateChange(SpineStates old_state)
        {
            if (SpineStateChanged != null)
            {
                SpineStateChanged(old_state);
            }
        }

        public DeviceState GetDeviceState(short device_id)
        {
            try
            {
                if (State == States.DISCONNECTED)
                {
                    return null;
                }
                return SpineClient.Spine.GetDeviceState(device_id);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e);
                STrace.Debug(GetType().FullName, String.Format("SPINE_CLIENT_WRAP/ERROR: No se pudo leer el estado del dispositivo {0}", device_id));
                State = States.DISCONNECTED;
                SpineClient = null;
            }
            return null;
        }

        public bool SetDeviceStateProperties(short device_id, DeviceStatePropertiesQueue properties)
        {
            try
            {
            	return State != States.DISCONNECTED && SpineClient.Spine.SetDeviceStateProperties(device_id, properties);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e);
                STrace.Debug(GetType().FullName, String.Format("SPINE_CLIENT_WRAP/ERROR: No se pudo actualizar las propiedades del dispositivo {0}", device_id));
                State = States.DISCONNECTED;
                SpineClient = null;
            }
            return false;
        }

        private void FireSpineCommand(Commands sig, object data)
        {
            if (SpineCommand != null)
            {
                SpineCommand(sig, data);
            }
        }


        public bool UpdateDeviceStateCounters(short device_id, DeviceStateCountersQueue counters)
        {
            try
            {
                return State == States.CONNECTED && SpineClient.Spine.UpdateDeviceStateCounters(device_id, counters);
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e);
                STrace.Debug(GetType().FullName, String.Format("SPINE_CLIENT_WRAP/ERROR: No se pudo actualizar las propiedades del dispositivo {0}", device_id));
                State = States.DISCONNECTED;
                SpineClient = null;
            }
            return false;
        }
    }
}