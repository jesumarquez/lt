#region Usings

using System;
using System.ComponentModel;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Statistics;
using Urbetrack.Toolkit;
using Urbetrack.Torino;

#endregion

namespace Urbetrack.Comm.Core.Transport.XBeeRLP
{
    public class XBeeSession
    {
        /// <summary>
        /// Estados de la Session XBEE
        /// </summary>
        public enum SessionStates
        {
            [Description("Sin Señal;Gray")]NOSESSION,
            [Description("Iniciando;DarkBlue")]STARTING_UP,
            [Description("Listo;Blue")]READY_ENQUEUED,
            [Description("Preparando;DarkGreen")]RESTARTING,
            [Description("Esperando;DarkBlue")]CONNECTING,
            [Description("Recibiendo;Green")] ACTIVE,
            [Description("Terminado;Green")] FINISHED,
        }

        private readonly Device Device;
        
        private XBeeRadioLinkProtocol RLP;
        private TransporteXBEE Transporte;

        public XBeeSession(Device d)
        {
            Device = d;
        }

        public int ExpectedTrackingPositions { get; private set; }
        public int ExpectedDetailedPositions { get; private set; }
        public int ExpectedAlreadySentPositions { get; private set; }

        private int receivedPositions;
        private readonly Trend positionsTrend = new Trend(120, 30);
        public int ReceivedPositions {
            get 
            { 
                return receivedPositions; 
            }

            set 
            {
                positionsTrend.NewSample(value);
                receivedPositions = value; 
            }
        }

        public int ReceivedEvents  { get; set; }

        public string StateDescription
        {
            get
            {
               //if (State == SessionStates.XBEE_ACTIVE && Report.NetworkConnections == 0x01)
               //{
               //    return SessionStates.CONNECTING.Description();
               // }
               if (State == SessionStates.ACTIVE && ReceivedPositions == 0)
               {
                    return SessionStates.CONNECTING.ToString("G");
               }
			   return State.ToString("G");
            }
        }

        public DateTime ActivateTimestamp;
        public DateTime FinishTimestamp;

        private SessionStates state;
        public SessionStates State
        {
            get { return state; }
            private set
            {
            	state = value;
                if (Device.HackBugXBEEv99 && state == SessionStates.STARTING_UP)
                {
                    hack_v99_wait_for_restart = false;
                }
                if (Transporte != null)
                    Transporte.DoReceiveReport(Device);
            }
        }

        public int CurrentProgress
        {
            get
            {
                switch (State)
                {
                    case SessionStates.NOSESSION:
                        return 100;
                    case SessionStates.STARTING_UP:
                        { 
                            var samples = Report.QueryEndSample - Report.QueryStartSample;
                            var sample = samples - (Report.QueryEndSample - Report.CursorSample);
                            if (sample < 0) sample = 0;
                            if (samples != 0)
                            {
                                var x = sample * 100 / samples;
                                return x <= 100 ? x : 100;
                            }
                            return 0;
                        }
                    case SessionStates.READY_ENQUEUED:
                        return 0;
                    case SessionStates.ACTIVE:
                        var expected = Report.SessionSent + Report.Pendings;
                        var result = ReceivedPositions*100/expected;
                        if (result < 0) result = 0;
                        if (result > 100) result = 100;
                        return result;
                    default:
                        return 0;
                }
            }
        }

        private XBeeReport report;
        public bool hack_v99_wait_for_restart;
        public bool hack_v99_wait_for_events;
        public byte ActivationRetrieveFlags = 1;

        public XBeeReport Report
        {
            get { return report ?? (report = new XBeeReport(this, Device)); }
        	protected set { report = value; }
        }


        /// <summary>
        /// Se produjo un cambio en el estado del Link de Radio en el 
        /// Dispositivo.
        /// </summary>
        public void RadioLinkChanged()
        {
			STrace.Debug(GetType().FullName, String.Format("XBEESESSION: RadioLinkChanged State={0} Qry={1} RLP={2}", State.ToString("G"), Report.QueryState.ToString("G"), Report.RadioLinkState.ToString("G")));
            switch (State)
            {
                case SessionStates.ACTIVE:
                    if (Report.RadioLinkState == XBeeReport.DeviceXbeeMachineStates.XBEE_READY)
                    {
                        RLP.ActivateNode(Device, 1);
                    }
                    break;
            }   
        }

        public int PendingBytes
        {
            get
            {
                var result = State == SessionStates.ACTIVE ? (Report.Pendings)*20 : Math.Max(0,State == SessionStates.FINISHED ? 0 : Report.Tracking * 20);
                return result < 0 ? 0 : result;
            }
        }

        public void Activate(byte retrieve_flags)
        {
            ActivateTimestamp = DateTime.Now;
            RLP.ActivateNode(Device, retrieve_flags);
        }

        public void Deactivate()
        {
            if (State == SessionStates.ACTIVE)
            {
                FinishTimestamp = DateTime.Now;
                RLP.DeactivateNode(Device);
            }
        }

        /// <summary>
        /// Se produjo un cambio en el estado del indice del dispositivo.
        /// </summary>
        public void QueryStateChanged()
        {
			STrace.Debug(GetType().FullName, String.Format("XBEESESSION: QueryStateChanged State={0} Qry={1} RLP={2}", State.ToString("G"), Report.QueryState.ToString("G"), Report.RadioLinkState.ToString("G")));
            switch(State)
            {
                case  SessionStates.STARTING_UP:
                    if (Report.QueryState == XBeeReport.QueryStates.QUERY_FETCHING ||
                        Report.QueryState == XBeeReport.QueryStates.QUERY_STEPBY)
                    {
                        State = SessionStates.READY_ENQUEUED;
                    }
                    break;
                case  SessionStates.READY_ENQUEUED:
                    if (Report.QueryState == XBeeReport.QueryStates.QUERY_IDLE ||
                        Report.QueryState == XBeeReport.QueryStates.QUERY_COUNTING)
                    {
                        State = SessionStates.STARTING_UP;
                    }
                    break;
            }
        }

        /// <summary>
        /// El dispositivo aparecio visible.
        /// </summary>
        public void DeviceComesUp(XBeeRadioLinkProtocol rLP, TransporteXBEE transporte)
        {
            RLP = rLP;
            Transporte = transporte;
            State = SessionStates.STARTING_UP;
        }

        /// <summary>
        /// El dispositivo desaparecio.
        /// </summary>
        public void DeviceComesDown()
        {
            State = SessionStates.NOSESSION;
        }
        
        public void GoesActive()
        {
            State = SessionStates.ACTIVE;
            STrace.Debug(GetType().FullName,"SETEEANDO VALORES ESPERADOS..");
            ExpectedTrackingPositions = Report.Tracking;
            ExpectedDetailedPositions = Report.Detailed;
            ExpectedAlreadySentPositions = Report.Sent;
        }

        public void GoesInactive()
        {
            State = SessionStates.FINISHED;    
        }
    }
}