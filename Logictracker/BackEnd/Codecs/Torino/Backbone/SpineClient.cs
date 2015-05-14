#region Usings

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;

#endregion

namespace Urbetrack.Backbone
{
    public class SpineClient
    {
        public event SpineStateChangedHandler SpineStateChanged;
        public event GatewayStateChangedHandler GatewayStateChanged;
        public event SpineCommandHandler SpineCommand;
        public event DeviceStateChangedHandler DeviceStateChanged;

        // manejo del SpineState
        public void FireSpineStateChanged(SpineStates old_state)
        {
            if (SpineStateChanged != null) SpineStateChanged(old_state);
        }

        // manejo de GatewayState
        public void FireGatewayStateChanged(GatewayStates old_state)
        {
            if (GatewayStateChanged != null) GatewayStateChanged(old_state);
        }

        // manejo de Comandos 
        public void FireSpineCommand(Commands cmd, object data)
        {
            if (SpineCommand != null) SpineCommand(cmd, data);
        }

        // manejo de Cambios en un DeviceState
        public void FireDeviceStateChanged(List<short> devices)
        {
            if (DeviceStateChanged != null) DeviceStateChanged(devices);
        }

        private readonly string ServerUri;
        private readonly string Description;

        public SpineClient(string serverUri, string description)
        {
            ServerUri = serverUri;
            Description = description;
            Sink = new ClientSink(this);
        }

        public List<short> GetDevicesList()
        {
            return new List<short>(DevicesList);
        }

        public bool Connect()
        {
            try
            {
                Spine = (ISpineInstance)Activator.GetObject(typeof(ISpineInstance), ServerUri);
                if (!RemotingServices.IsTransparentProxy(Spine))
                {
                    throw new Exception(
                        "El modulo Spine esta configurado de forma inaporpiada, debe ser accesible por Remoting.");
                }
                Spine.Start();
                Spine.WaitForRunning();
                DevicesList = Spine.GetDevices();
                Spine.AttachClient(Sink, Description);
                return true;
            }
            catch
            {
                // STrace.Exception(GetType().FullName,e);
                Spine = null;
                return false;
            }
        }

        public ISpineInstance Spine { get; private set; }
        public ClientSink Sink { get; private set; }

        public List<short> DevicesList { get; private set; }
    }

    public class ClientSink : ClientCallbackSink
    {
        private readonly SpineClient client;

        public ClientSink(SpineClient _client)
        {
            client = _client;
        }

        public override object InitializeLifetimeService()
        {
            // HACK: hace del cliente una clase eterna.
            return null;
        }

        protected override void OnSpineStateChanged(SpineStates old_state)
        {
            client.FireSpineStateChanged(old_state);
        }

        protected override void OnGatewayStateChanged(GatewayStates old_state)
        {
            client.FireGatewayStateChanged(old_state);
        }

        protected override void OnSpineCommand(Commands cmd, object data)
        {
            client.FireSpineCommand(cmd, data);
        }

        protected override void OnDeviceStateChanged(List<short> devices)
        {
            client.FireDeviceStateChanged(devices);
        }
    }
}