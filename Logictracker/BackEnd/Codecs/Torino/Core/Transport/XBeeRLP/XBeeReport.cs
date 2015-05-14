#region Usings

using Urbetrack.Torino;

#endregion

namespace Urbetrack.Comm.Core.Transport.XBeeRLP
{
    public class XBeeReport
    {

        public enum QueryStates
        {
            QUERY_IDLE = 0,		// 0- la consulta no comenzo.
            QUERY_COUNTING = 1,	// 1- contabilizando los registros totales del indice.
            QUERY_FETCHING = 2,	// 2- consumiendo paginas con datos validos.
            QUERY_STEPBY = 3,	    // 3- consumiendo datos a medida que ingresan.
            QUERY_FINISHED = 4	// 4- la consulta ya termino.
        }

        /// <summary>
        /// Estados de la FSM del controlador XBEE en el dispositivo.
        /// </summary>
        public enum DeviceXbeeMachineStates
        {
            XBEE_UNKNOWN = 0xFF,
            XBEE_READY = 0x04,
            XBEE_ACTIVE = 0x05
        }

        public enum DeviceSessionMachineStates
        {
            SESSION_PERMANENT = 0x00, 
            SESSION_TRYING_CONNECT = 0x01, 
            SESSION_CONNECTED = 0x02
        }

        public enum DeviceSafaguardingMachineStates
        {
            NETWORK_FAULT = 0x00,
            NETWORK_ACTIVE = 0x01
        }

        private DeviceXbeeMachineStates radioLinkState;

        public DeviceXbeeMachineStates RadioLinkState
        {
            get { return radioLinkState; }
            set
            {
                var oldradioLinkState = radioLinkState;
                radioLinkState = value;
                if (Session == null) return;
                if (radioLinkState != oldradioLinkState || Session.State == XBeeSession.SessionStates.ACTIVE) Session.RadioLinkChanged();
            }
        }

        public byte NetworkConnections;
        
        public QueryStates QueryState
        {
            get { return queryState; }
            set
            {
                var oldqueryState = queryState;
                queryState = value;
                if (oldqueryState != queryState && Session != null)
                {
                    Session.QueryStateChanged();
                }
            }
        }

        public DeviceSessionMachineStates CommCoreState { get; set; }

        public DeviceSafaguardingMachineStates SafeguardingState { get; set; }

        public int CounterBlock { get; set; }

        public int OldestTrackingSample { get; set; }

        private QueryStates queryState;

        /// estado del consumidor
        public int QueryStartSample { get; set; }

        public int QueryEndSample;
        public int CursorSample;
        public int OldestSample;
        public int Processed;
        public int Empty;
        public int Tracking;
        public int Detailed;
        public int Sent;
        public int SessionSent;
        public int Pendings;
        public int MaxPendings;
        internal readonly XBeeSession Session;
        internal readonly Device Device;


        public XBeeReport (XBeeSession session, Device device)
        {
            Session = session;
            Device = device;
        }

        public int GetPendings()
        {
            var retrieve_flags = Session.ActivationRetrieveFlags;
            var result = 0;
            if ((retrieve_flags & 1) == 1) result += Tracking;
            if ((retrieve_flags & 2) == 2) result += Detailed;
            if ((retrieve_flags & 4) == 4) result += Sent;
            if (result > MaxPendings) MaxPendings = result;
            return result;
        }

    }
}