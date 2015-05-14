#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Urbetrack.Utils;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Statistics;

#endregion

namespace Urbetrack.Backbone
{
    public static class DeviceStateExtensionMethods
    {
        public static bool IsConnectedState(this DeviceState.States state)
        {
            switch(state)
            {
                case DeviceState.States.CONNECTED:
                case DeviceState.States.ONLINE:
                case DeviceState.States.ONNET:
                case DeviceState.States.SYNCING:
                    return true;
            }
            return false;
        }
    }

    [Serializable]
    public partial class DeviceState
    {
        [Serializable]
        public enum States
        {
            UNLOADED,     // la informacion no se cargo todavia desde las base de datos de objetos.
            PERMANENT,    // solo provisionado, nunca hubo datos del gtw.
            MAINT,        // el dispositivo o una capa intermedia esta transientemente en mantenimiento, deteccion automatica.
            OFFLINE,      // el dispositivo esta desconectado de la plataforma.
            CONNECTED,    // el dispositivo esta conectado pero no esta listo para enviar la informacion esperada.
            ONLINE,       // el dispositivo esta conectado y reportando por internet.
            ONNET,        // el dispositivo esta conectado a una base inalambrica y la base esta ONLINE.
            SHUTDOWN,     // el dispositivo esta apagado y desconectado de la plataforma por desicion del gateway.
            SYNCING,      // el dispositivo esta conectado y enviando la cola de posiciones.
            OUTOFSERVICE,        // el dispositivo esta fuera de servicio
        }

        [Serializable]
        public enum AdminStates
        {
            UNLOADED, // Este estado indica que el estado es incierto. Ningun TSP permanece mucho tiempo en este estado, se especifica para absorber condiciones de carrera (Race Condition). Solo se trata de una condición de implementación como por ejemplo el caso de un TSP que fue instanciado en la base pero que aun los datos no fueron deserializados. Debe tratarse como si el TSP no existiera.
            PLANNED, //  El TSP esta planificado pero todavía no fue completamente instalado.
            TESTINGFORSERVICE, //  El TSP paso a servicio activo recientemente y todavía esta bajo supervisión preventiva
            INSERVICE, //  El TSP esta en servicio activo.
            WATINGFORMAINTENANCE, //  El TSP esta en servicio activo, pero presenta problemas técnicos y esta en espera de mantenimiento. En este estado es posible que sea inoperable o que presente una operación limitada.
            INMAINTENANCE, //  El TSP esta fuera de servicio temporalmente
            INLEGACYMAINTENANCE, //  El TSP esta fuera de servicio temporalmente no por una falla sino por causas de fuerza mayor externas. Por ejemplo: un TSP de seguimiento GPS instalado en un vehiculo el cual se encuentra en un taller mecánico.
            RECYCLING, //  El TSP fue dado de baja, pero su correspondinte TSPC seguira bloqueado.        
        }

        public int ObjectVersion { get; set; }
        public const int MinimumCompatibleVersion = 6;

        // indicacion para storage de que se debe persistir pues los datos han cambiado.
        private bool requireCommit;
        public bool RequireCommit
        {
            get { return requireCommit; }
            set
            {
                FirstActivity = LastCommit = DateTime.Now;
                if (requireCommit == value) return;
                requireCommit = value;
                STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE: id={0} fue {1}", Id, (requireCommit ? "modificado" : "persistido")));
            }
        }

        public DeviceTypes.Types Type { get; set; }

        public void LeaveOutOfService(GatewayStates gtw)
        {
            if (serviceState != States.OUTOFSERVICE) return;
            
            // esto es por que asi evito que el test de lifetime de siempre false.-
            serviceState = States.UNLOADED;
            if (!TestLifetime(LifeTime))
            {
                // si al salir de stock
                serviceState = gtw == GatewayStates.CONNECTED ? States.OFFLINE : States.MAINT;
            }
            // verifico si el estado en verdad cambio.
            STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE[{0}]: LEAVE OUTOFSERVICE TO {1}", Id, serviceState));
            RequireCommit = true;
        }

        private AdminStates adminState;
        public AdminStates AdminState
        {
            get { return adminState; }
        }

        private States serviceState;
        public States ServiceState
        {
            get { return serviceState; }

            set
            {
                if (serviceState == value) return;
                if (serviceState == States.OUTOFSERVICE) 
                        return;
                // proceso el cambio de estado.
                var _old_state = serviceState;
                serviceState = value;
                if (StatesChanges != null) StatesChanges.Inc(1);
                // si es verdadero, se desconecto o fue desconectado.
                if (_old_state.IsConnectedState() && !serviceState.IsConnectedState() && LastLogin != DateTime.MinValue)
                {
                    var time = DateTime.Now - LastLogin;
                    if (AverageConnectedTime != null) AverageConnectedTime.NewSample((ulong)time.TotalSeconds);
                }
                switch (serviceState)
                {
                    case States.PERMANENT:
                        if (PERMANENTCounter != null) PERMANENTCounter.Inc(1);
                        break;
                    case States.MAINT:
                        if (MAINTCounter != null) MAINTCounter.Inc(1);
                        break;
                    case States.OFFLINE:
                        if (OFFLINECounter != null) OFFLINECounter.Inc(1);
                        break;
                    case States.CONNECTED:
                        if (CONNECTEDCounter != null) CONNECTEDCounter.Inc(1);
                        break;
                    case States.ONLINE:
                        if (ONLINECounter != null) ONLINECounter.Inc(1);
                        break;
                    case States.ONNET:
                        if (ONNETCounter != null) ONNETCounter.Inc(1);
                        break;
                    case States.SHUTDOWN:
                        if (SHUTDOWNCounter != null) SHUTDOWNCounter.Inc(1);
                        break;
                    case States.SYNCING:
                        if (SYNCINGCounter != null) SYNCINGCounter.Inc(1);
                        break;
                    case States.OUTOFSERVICE:
                        if (OUTOFSERVICECounter != null) OUTOFSERVICECounter.Inc(1);
                        break;
                }
                if (serviceState == States.CONNECTED)
                {
                    // si esta CONNECTED, significa que acaba de loguearse.
                    LastLogin = DateTime.Now;
                }
                // verifico si el estado en verdad cambio.
                STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE[{0}]:old={1} new={2}", LogId, _old_state, serviceState));
                RequireCommit = true;
            }
        }

        private DateTime firstActivity;
        public DateTime FirstActivity
        {
            get { return firstActivity; }
            set
            {
                // no se puede actualizar este campo si fue seteado.
                if (firstActivity != DateTime.MinValue) 
                    return;
                firstActivity = value;
            }
        }

        public int LifeTime { get; set; }

        public ArithmeticMean AverageConnectedTime { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime LastCommit { get; set; }

        public DateTime LastReceivedTrackingData { get; set; }

        public DateTime LastReceivedEventData { get; set; }

        public GPSPoint LastLoginGPSPoint { get; set; }

        private GPSPoint lastKnownGPSPoint;

        public GPSPoint LastKnownGPSPoint
        {
            get { return lastKnownGPSPoint; }
            set
            {
                if (value == null) return;
                // si la posicion que se intenta setear es mas vieja, se ignora.
                if (lastKnownGPSPoint != null && lastKnownGPSPoint.Date > value.Date)
                {
                    STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE[{0}]: Descartando posicion por ser antigua.", LogId));
                    return;
                }
                LastCommit = DateTime.Now;
                lastKnownGPSPoint = value;
                RequireCommit = true;
            }
        }

        /// <summary>
        /// Esta propiedad provee una cadena de texto con toda la informacion necesaria
        /// para 
        /// </summary>
        public string LogId
        {
            get {
                //return T.ActiveContext.DetailScope(GetType().FullName) ? string.Format("DS{{devid={0}/code='{1}'/vehicle='{2}'/base='{3}'}}", Id, Code, Vehicle, OrganizationBase) : string.Format("{0}/{1}", Id, Code);
                return string.Format("{0}/{1}", Id, Code);
            }
        }

        public bool AutomaticUpdateQtree { get; set; }

        public bool AutomaticUpdateFirmware { get; set; }

        public bool XBeeEnabled { get; set; }

        public DeviceTypes.QTreeStates QTreeState { get; set; }

        public short Id { get; private set; }

        public string FirmwareVersion { get; set; }

        public string TransientDeviceNetworkPath { get; set; }

        public bool HardwareEmergencyMode { get; set; }

        public int QTreeRevision { get; set; }

        public string Code { get; set; }

        public string DeviceIdentifier { get; set; }

        public string Vehicle { get; set; }

        public string Organization { get; set; } // v2
        
        public string OrganizationBase { get; set; }

        public string OrganizationUnit { get; internal set; }

        public string VehicleType { get; internal set; }

        public string Carrier { get; internal set; }

        public string XBeeFirmware { get; set; }

        public bool HaveDisplay { get; set; }

        // desplazamiento del reloj del dispositivo con el server

        public TimeSpan ClockSlice { get; set; }

        // contadores

        public Gauge64 FlashCounter { get; internal set; } // v1
        public Gauge64 CrapReceivedCounter { get; internal set; } // v1
        public Gauge64 StatesChanges { get; internal set; } // v2
        public Gauge64 PERMANENTCounter { get; internal set; } // v2
        public Gauge64 MAINTCounter { get; internal set; } // v2
        public Gauge64 OFFLINECounter { get; internal set; } // v2
        public Gauge64 CONNECTEDCounter { get; internal set; } // v2
        public Gauge64 ONLINECounter { get; internal set; } // v2
        public Gauge64 ONNETCounter { get; internal set; } // v2
        public Gauge64 SHUTDOWNCounter { get; internal set; } // v2
        public Gauge64 SYNCINGCounter { get; internal set; } // v2
        public Gauge64 OUTOFSERVICECounter { get; internal set; } // v2


        //public Toolkit.TransitionTrend<ServiceStates> TransitionTrend { get; private set; }
        
        public void ResetCounters()
        {
            FlashCounter.Reset();       
            CrapReceivedCounter.Reset();
            StatesChanges.Reset();
            PERMANENTCounter.Reset();   
            MAINTCounter.Reset();       
            OFFLINECounter.Reset();     
            CONNECTEDCounter.Reset();   
            ONLINECounter.Reset();      
            ONNETCounter.Reset();       
            SHUTDOWNCounter.Reset();    
            SYNCINGCounter.Reset();     
            OUTOFSERVICECounter.Reset();
            //TransitionTrend.Reset();
            STrace.Debug(GetType().FullName, String.Format("DEVICE['{0}'/{1}]: Reseteando contadores.", Code, Id));
            AverageConnectedTime.Reset();
            RequireCommit = true;
        }

        // constructor utilizado al crear la base de datos
        public DeviceState(short id, States initial_state)
        {
            STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE: constructor de ALTA id={0}.", id));
            Id = id;
            ServiceState = initial_state;
            QTreeState = DeviceTypes.QTreeStates.UNKNOWN;
            firstActivity = DateTime.MinValue;
            LastCommit = DateTime.MinValue;
            FlashCounter        = new Gauge64();
            CrapReceivedCounter = new Gauge64();
            StatesChanges       = new Gauge64();
            PERMANENTCounter    = new Gauge64();
            MAINTCounter        = new Gauge64();
            OFFLINECounter      = new Gauge64();
            CONNECTEDCounter    = new Gauge64();
            ONLINECounter       = new Gauge64();
            ONNETCounter        = new Gauge64();
            SHUTDOWNCounter     = new Gauge64();
            SYNCINGCounter      = new Gauge64();
            OUTOFSERVICECounter        = new Gauge64();
            //TransitionTrend     = new Toolkit.TransitionTrend<ServiceStates>();
            AverageConnectedTime = new ArithmeticMean(32);
        }

        public void Trace(int level)
        {
            STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE: id={0} imei={1} type={2} state={3}", Id, DeviceIdentifier, Type, ServiceState));
        }

        public bool TestLifetime(int device_lifetime)
        {
            if (ServiceState == States.PERMANENT || ServiceState == States.OUTOFSERVICE || LastLogin.Year < 2009 ||
                DateTime.Now.Subtract(LastLogin).TotalDays <= device_lifetime) return false;

            STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE: id={0} code={1}, pasa a PERMANENTE, no reporto por {2} dias.", Id, Code, DateTime.Now.Subtract(LastLogin).TotalDays));
            serviceState = States.PERMANENT;
            return requireCommit = true;
        }

        public enum OperationalState
        {
            [Description("INAPLICABLE")] UNDEFINED,
            [Description("OPERATIVO")] OPERATIVE,
            [Description("FALLA INDICADA")] INDICATED_FAILURE,
            [Description("FALLA SOSPECHADA")] SUSPECTED_FAILURE
        }

        public void SetAdminState(AdminStates adm, GatewayStates gtw)
        {
            var oldState = adminState;
            adminState = adm;
            if (oldState == adminState) return;
            if (adminState == AdminStates.INSERVICE)
            {
                LeaveOutOfService(gtw);
                return;
            }
            STrace.Debug(GetType().FullName, String.Format("DEVICE_STATE[{0}]: ENTER OUTOFSERVICE FROM {1}", Id, serviceState));
            ServiceState = States.OUTOFSERVICE;
            RequireCommit = true;
        }
    }

    [Serializable]
    public class DeviceStateCounter
    {
        public enum Actions
        {
            Increase,
            ResetPartial
        }

        public Actions Action;
        public string Counter;
        public ulong PartialValue;
        public int PendingCommits;
    }

    [Serializable]
    public class DeviceStateCountersQueue : Queue<DeviceStateCounter>
    { }


    [Serializable]
    public class DeviceStateProperty
    {
        public string Property;
        public object Value;
        public int PendingCommits;
        public bool IsCounter;
    }

    [Serializable]
    public class DeviceStatePropertiesQueue : Queue<DeviceStateProperty>
    {}

}