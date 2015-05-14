using System;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.Virloc
{
    [FrameworkElement(XName = "VirlocParser", IsContainer = false)]
	public partial class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Virloc; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 4040)]
		public override int Port { get; set; }

		#endregion

        private String _askedIMEI;

		#region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x8000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

        private string ExtractIMEI(VirlocDeviceCommand dc)
        {
            String result = null;
            var cmd = dc.getCommand();
            if (cmd != null && cmd.StartsWith(Reporte.IMEIReq)) {
                result = cmd.Replace(Reporte.IMEIReq, "").Trim();
            }
            return result;
        }

        public override INode Factory(IFrame frame, int formerId)
		{
		    var dc = new VirlocDeviceCommand(frame.Payload);
            if (!dc.hasIdNum())
            {                
                var imei = ExtractIMEI(dc);
                if (imei != null)
                    return DataProvider.FindByIMEI(imei, this);
                return null;
            }

		    return DataProvider.FindByIdNum(dc.IdNum.Value, this);
        }

		public override IMessage Decode(IFrame frame)
        {
            IMessage salida = null;
            var tipoReporte = Reporte.SinNodo;
            var msgId = ParserUtils.MsgIdNotSet;

            var dc = new VirlocDeviceCommand(frame.Payload, this);
            if (dc.isValid()) // && _askedIMEI != null)
                tipoReporte = GetTipoReporte(dc);

            if (dc.hasMessasgeId())
                msgId = dc.MessageId ?? 0;

            switch (tipoReporte)
            {
				case Reporte.Nada:
            		return null;
                case Reporte.EventoGL:
                case Reporte.EventoGP:
                    salida = GetSalida(dc);
                    break;
                case Reporte.SinNodo:
                    if (!GetTipoReporte(dc).StartsWith(Reporte.IMEIReq))
                    {
                        var askIMEIcmd = new VirlocDeviceCommand(Mensajes.AskIMEI, this, null);
                        askIMEIcmd.IdNum = new VirlocDeviceCommand(frame.Payload, this).IdNum;
                        var askIMEIcmdStr = askIMEIcmd.ToString(true);
                        
                        salida = new UserMessage(Id, dc.MessageId ?? 0);
                        salida.AddStringToSend(askIMEIcmdStr);
                    }
                    else
                    {
                        salida = new ConfigRequest(Id, dc.MessageId ?? 0);
                        var imei = ExtractIMEI(dc);
                        if (imei != null)
                        {
                            _askedIMEI = imei;
                        }
                    }
                    break;
            }

            if (!Reporte.SinNodo.Equals(tipoReporte))
            {
                // Replying with ACK if needed
                if (dc.hasIdNum() && dc.hasMessasgeId() && !salida.IsPending())
                {
                    if (salida == null)
                        salida = new UserMessage(Id, msgId);

                    var ackStr = dc.BuildAck().ToString(true);
                    salida.AddStringToSend(ackStr);
                }

                CheckLastSentAndDequeueIt(dc);

                if (LastSent == null)  //&& !(new String[] {Reporte.IdReq}.Any(r=> tipoReporte == r)))
                    SendPendingFota(ref salida);

            }
		    return salida;
        }

        #endregion
        
        #region Members

		private IMessage GetSalida(VirlocDeviceCommand dc)
		{
            var msgid = dc.MessageId ?? 0;
            
            DeviceStatus devStatus = dc.ParsePosition();
            Console.WriteLine(devStatus.ToString());

            /*
             * // cachear devstatus en de gpspoint
                        var oldpos = NewestPositionReceived;
                        if (oldpos == null ||
                            (oldpos.Date < pos4.Date))
                            NewestPositionReceived = pos4;

                        // If the Message Origin is no the LOG, then doesnt process it, just store on CACHE.
                        if (dc.MessageOrigin != GTEDeviceCommand.Attributes.MessageOrigin_LOG)
                            break;
            */

            var gpoint = devStatus.Position;

            if (gpoint == null)
                return null;

            MessageIdentifier codigo;
			switch (devStatus.FiredEventNumber) // codigo del Evento generador del reporte
			{
                case Evento.PanicoConductor:    codigo = MessageIdentifier.DigitalInput01Closed;    break;
                case Evento.PanicoJefe:         codigo = MessageIdentifier.DigitalInput00Closed;    break;
				case Evento.IgnitionOn:         codigo = MessageIdentifier.EngineOnInternal;	    break;
				case Evento.IgnitionOff:        codigo = MessageIdentifier.EngineOffInternal;   	break;
                case Evento.PowerOn:            codigo = MessageIdentifier.PowerReconnected;        break;
                case Evento.SleepModeOn:        codigo = MessageIdentifier.SleepModeOn;             break;
                case Evento.GPSAntennaInShort:  codigo = MessageIdentifier.GPSAntennaShort;         break;
                case Evento.GPSAntennaDisconnected: codigo = MessageIdentifier.GPSAntennaDisconnected;  break;
                case Evento.CustomerArrivedTo:  codigo = MessageIdentifier.CustomEvent0001;         break;
                case Evento.CustomerLeaved:     codigo = MessageIdentifier.CustomEvent0002;         break;
                case Evento.Parked15Min:        codigo = MessageIdentifier.CustomEvent0003;         break;
                case Evento.NonPlannedParked2Min:   codigo = MessageIdentifier.CustomEvent0004;       break;
                case Evento.StopNotCompleted:   codigo = MessageIdentifier.CustomEvent0005;         break;
                case Evento.CustomerCodeCorrect:    codigo = MessageIdentifier.CustomEvent0006; break;
                case Evento.DoorOpenedOutsideOfStop:    codigo = MessageIdentifier.CustomEvent0007; break;
                case Evento.OperativeCodeEntered:   codigo = MessageIdentifier.CustomEvent0008; break;
                case Evento.MovementDetectedWithoutStopEnd: codigo = MessageIdentifier.CustomEvent0009; break;
                case Evento.EventualCodeEntered:    codigo = MessageIdentifier.CustomEvent0010; break;
                case Evento.StopNotice:         codigo = MessageIdentifier.CustomEvent0011; break;
                case Evento.TreasureDoorOpenWithDoorOpenIgnitionOffNotFixed:    codigo = MessageIdentifier.CustomEvent0012; break;
                case Evento.TreasureDoorOpenInPanicMode:    codigo = MessageIdentifier.CustomEvent0013; break;
                case Evento.TreasureDoorOpenOutOfCustomer:  codigo = MessageIdentifier.CustomEvent0014; break;
                case Evento.TreasureDoorOpenOutOfSequence:  codigo = MessageIdentifier.CustomEvent0015; break;
                case Evento.TreasureDoorOpenWithCorrectCode: codigo = MessageIdentifier.CustomEvent0016; break;
                case Evento.TreasureDoorOpenOutsideStop:    codigo = MessageIdentifier.CustomEvent0017; break;
                case Evento.TreasureDoorReadyButNotOpened:  codigo = MessageIdentifier.CustomEvent0018; break;
                case Evento.ForceCodeEnteredForTreasureDoorOpening: codigo = MessageIdentifier.CustomEvent0019; break;
                case Evento.TreasureDoorOpen:   codigo = MessageIdentifier.CustomEvent0020;         break;
                case Evento.AlarmDeactivatedFromVehicle:    codigo = MessageIdentifier.CustomEvent0021; break;
                case Evento.AITOffByKey:        codigo = MessageIdentifier.CustomEvent0022;         break;
                case Evento.AITOffByCode:       codigo = MessageIdentifier.CustomEvent0023;         break;
                case Evento.GabineteApertura:   codigo = MessageIdentifier.DigitalInput03Open;      break;
                case Evento.GabineteCierre:     codigo = MessageIdentifier.DigitalInput03Closed;    break;
                    
				default:
                    return gpoint.ToPosition(Id, msgid);
			}
            return codigo.FactoryEvent(Id, msgid, gpoint, gpoint.GetDate(), null, null);
		}

		private static String GetTipoReporte(BaseDeviceCommand c)
		{
		    var cmd = c.getCommand();
            if (cmd.StartsWith(Reporte.IMEIReq)) return Reporte.IMEIReq;
            if (cmd.StartsWith(Reporte.IdReq)) return Reporte.IdReq;
            if (cmd.StartsWith(Reporte.EventoGP)) return Reporte.EventoGP;
            if (cmd.StartsWith(Reporte.EventoGL)) return Reporte.EventoGL;
    		return cmd.Length < 3 ? Reporte.Nada : cmd.Substring(0, 3);
    	}

		private abstract class Reporte
		{
			public const String Nada = "Nada";
			public const String SinNodo = "SinNodo";
			public const String EventoGP = "RGP";
            public const String EventoGL = "RGL";
			public const String IdReq = "RID";
		    public const String IMEIReq = "REN11";
		}

        private static class Mensajes
        {
            public const String SetId = ">SID{0}<";
            public const String AskIMEI = ">QEN11<";
        }

		//si no es uno de estos es una posicion
		private static class Evento
		{
		    public const int PanicoConductor = 4;
            public const int PanicoJefe = 5;

            public const int IgnitionOn = 13;
            public const int GabineteApertura = 7;
            public const int GabineteCierre = 8;
            public const int PowerOn = 11;
//		    public const int SleepModeOn = 6;

		    public const int GPSAntennaInShort = 14;
            public const int GPSAntennaDisconnected = 15;

            public const int CustomerLeaved = 18;
		    public const int IgnitionOff = 21;

		    public const int CustomerArrivedTo = 23;
		    public const int Parked15Min = 25;

		    public const int NonPlannedParked2Min = 27;

		    public const int StopNotCompleted = 28;
		    public const int CustomerCodeCorrect = 29;
		    public const int DoorOpenedOutsideOfStop = 36;
		    public const int OperativeCodeEntered = 40;
		    public const int MovementDetectedWithoutStopEnd = 46;
		    public const int EventualCodeEntered = 59;
		    public const int StopNotice = 61;
		    public const int TreasureDoorOpenWithDoorOpenIgnitionOffNotFixed = 79;
		    public const int TreasureDoorOpenInPanicMode = 80;
            public const int TreasureDoorOpenOutOfCustomer = 81;
            public const int TreasureDoorOpenOutOfSequence = 82;
            public const int TreasureDoorOpenWithCorrectCode = 83;
		    public const int TreasureDoorOpenOutsideStop = 84;
            public const int TreasureDoorReadyButNotOpened = 85;
		    public const int ForceCodeEnteredForTreasureDoorOpening = 89;
		    public const int TreasureDoorOpen = 92;
		    public const int AlarmDeactivatedFromVehicle = 103;
		    public const int AITOffByKey = 106;
		    public const int AITOffByCode = 109;
		    public const int SleepModeOn = 117;
		}

		#endregion

        public ulong GetMessageId(string line)
        {
            throw new NotImplementedException();
        }
	}
}