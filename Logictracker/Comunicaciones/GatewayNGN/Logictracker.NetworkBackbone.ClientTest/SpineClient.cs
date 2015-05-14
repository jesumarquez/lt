using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Urbetrack.Backbone;
using Urbetrack.Toolkit;

namespace Urbetrack.Backbone.Client
{
    public class SpineClient : MarshalByRefObject
    {
        public delegate void SpineStateChangedHandler(Spine source, Spine.States old_state);
        public delegate void GatewayStateChangedHandler(Spine source, Spine.GatewayStates old_state);
        public delegate void DeviceMappingHandler(DeviceState ds);
        public delegate void SpineSignalHandler(Spine.Signals signal, int data);

        public event SpineStateChangedHandler SpineStateChanged;
        public event GatewayStateChangedHandler GatewayStateChanged;
        public event DeviceMappingHandler DeviceAdded;
        public event DeviceMappingHandler DeviceRemoved;
        public event SpineSignalHandler SpineSignal;

        public SpineClient()
        {
            Spine = new Spine();
            Spine.SpineStateChanged += Spine_SpineStateChanged;
            Spine.GatewayStateChanged += Spine_GatewayStateChanged;
            Spine.DeviceAdded += Spine_DeviceAdded;
            Spine.DeviceRemoved += Spine_DeviceRemoved;
            Spine.SpineSignal += Spine_SpineSignal;


            Devices = new Dictionary<short, DeviceState>();
            if (!RemotingServices.IsTransparentProxy(Spine))
            {
                throw new Exception(
                    "El modulo Spine esta configurado de forma inaporpiada, debe ser accesible por Remoting."); 
            }
        }

        public bool Connect()
        {
            try
            {
                Spine.Start();
                Spine.WaitForRunning();
                DevicesList = Spine.GetDevices();
                foreach (var device_id in DevicesList)
                {
                    var ds = Spine.LoadDeviceState(device_id);
                    Devices.Add(device_id, ds);
                }
                return true;
            }
            catch (Exception e)
            {
                T.EXCEPTION(e);
                return false;
            }
        }

        public Spine Spine { get; private set; }

        public List<short> DevicesList { get; private set; }

        public Dictionary<short, DeviceState> Devices { get; private set; }

        public void Spine_SpineStateChanged(Spine source, Spine.States old_state)
        {
            if (SpineStateChanged != null) SpineStateChanged(source, old_state);
        }

        public void Spine_GatewayStateChanged(Spine source, Spine.GatewayStates old_state)
        {
            if (GatewayStateChanged != null) GatewayStateChanged(source, old_state);
        }

        public void Spine_DeviceAdded(DeviceState ds)
        {
            if (DeviceAdded != null) DeviceAdded(ds);
        }

        public void Spine_DeviceRemoved(DeviceState ds)
        {
            if (DeviceRemoved != null) DeviceRemoved(ds);
        }

        public void Spine_SpineSignal(Spine.Signals signal, int data)
        {
            if (SpineSignal != null) SpineSignal(signal, data);
        }
    }
}