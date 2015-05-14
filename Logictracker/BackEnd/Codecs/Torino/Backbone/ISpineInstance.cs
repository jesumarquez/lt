#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Backbone
{
    public enum Commands
    {
        HEARTBEAT,
        FOTA_DEVICE,
        FULL_QTREE_DEVICE,
        QTREE_DEVICE,
        REBOOT_DEVICE,
        CONFIGURE_DEVICE,
        GATEWAY_REFRESH,
        TEMPORARILY_DISABLE_DEVICE,
        RESET_DEVICE_COUNTERS,
        RESET_ALL
    };

    public enum SpineStates
    {
        WAITFOR_START,
        STARTING_UP,
        RUNNING,
        SHUTING_DOWN,
        OVERLOADED,
        PERMANENT
    };

    public enum GatewayStates
    {
        UNKNOWN,
        DISCONNECTED,
        CONNECTED
    };

    // Delegados utilizados desde el server al cliente.
    public delegate void SpineStateChangedHandler(SpineStates old_state);
    public delegate void GatewayStateChangedHandler(GatewayStates old_state);
    public delegate void SpineCommandHandler(Commands signal, object data);
    public delegate void DeviceStateChangedHandler(List<short> devices);

    // Interface del servidor que se despliega en los clientes
    // se entiende por cliente tanto los gateways como la consola o el dispatcher.
    public interface ISpineInstance
    {
        // Inicializa el Spine
        void Start();

        // Espera que el Spine este operativo
        bool WaitForRunning();

        // Registra un cliente
        void AttachClient(ClientCallbackSink _client, string description);

        // Estado del Spine
        SpineStates State { get; set; }

        // Estado de la capa de comunicaciones
        GatewayStates GatewayState { get; set; }

        // Lista de los dispositivos conocidos por el Spine
        List<short> GetDevices();

        // Retorna una copia al estado del dispositivo
        DeviceState GetDeviceState(short id);

        // Retorna una lista de discos monitoreados por el spine.
        //List<HardDiskState> GetHardDisksState();

        // Retorna una lista de procesadores monitoreados por el spine
        //List<ProcessorState> GetProcessorsState();

        // Setea un parametro de estado de dispositivo.
        bool SetDeviceStateProperties(short device_id, DeviceStatePropertiesQueue properties); 

        // keepalive del gateway.
        void GatewayHeartbeat(string description);

        // keepalive del cliente
        void ConsoleHeartbeat(string description);

        // distribucion de comandos
        void Command(Commands cmd, object data);

        bool UpdateDeviceStateCounters(short device_id, DeviceStateCountersQueue operations_queue);
    }

    /// <summary>
    /// Clase utilizada por los cliente para proveer los delegados que el server
    /// utilizara para disparar y encaminar los eventos desde el server a los clientes.
    /// </summary>
    public abstract class ClientCallbackSink : MarshalByRefObject
    {
        // manejo del SpineState
        public void FireSpineStateChanged(SpineStates old_state)
        {
            OnSpineStateChanged(old_state);
        }

        protected abstract void OnSpineStateChanged(SpineStates old_state);

        // manejo de GatewayState
        public void FireGatewayStateChanged(GatewayStates old_state)
        {
            OnGatewayStateChanged(old_state);
        }

        protected abstract void OnGatewayStateChanged(GatewayStates old_state);

        // manejo de Comandos 
        public void FireSpineCommand(Commands signal, object data)
        {
            OnSpineCommand(signal, data);
        }

        protected abstract void OnSpineCommand(Commands signal, object data);

        // manejo de Cambios en un DeviceState
        public void FireDeviceStateChanged(List<short> devices)
        {
            OnDeviceStateChanged(devices);
        }

        protected abstract void OnDeviceStateChanged(List<short> devices);
    }
}
