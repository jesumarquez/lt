using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Cache;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.FulMar
{
    [FrameworkElement(XName = "FulMarParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Fulmar; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 3030)]
		public override int Port { get; set; }

		#endregion

		#region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

		public override INode Factory(IFrame frame, int formerId)
        {
			var buffer = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
			if (String.IsNullOrEmpty(buffer)) return null;
            var dev = ParserUtils.GetDeviceIdTaip(buffer);
            if (ParserUtils.IsInvalidDeviceId(dev))
            {
				return buffer.StartsWith(Reporte.IdReq) ? DataProvider.FindByIMEI(buffer.Substring(5, 15), this) : null;
            }
            return DataProvider.Get(dev, this);
        }

		public override IMessage Decode(IFrame frame)
        {
			var s = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
            var buffer = GetEntrada(s);
            if (buffer == null)
            {
                return null;
            }

            IMessage salida;

            var msgId = ParserUtils.GetMsgIdTaip(buffer);
            var data = buffer.Split(';')[0].Split(',');
            var tipoReporte = GetTipoReporte(data);

            switch (tipoReporte)
            {
				case Reporte.Nada:
            		return null;
                case Reporte.EventoFg:
                    if (ParserUtils.IsInvalidDeviceId(Id)) goto case Reporte.SinNodo;
                    var bytes = new byte[33];
                    Array.Copy(frame.Payload, 4, bytes, 0, 33);
                    salida = GetSalida(bytes, this, msgId);
                    break;
                case Reporte.EventoGp:
                    if (ParserUtils.IsInvalidDeviceId(Id)) goto case Reporte.SinNodo;
					salida = GetSalida(buffer, this, msgId, null);
                    break;
                case Reporte.SinNodo:
                    msgId = ParserUtils.MsgIdNotSet; //para no enviar ack sino un query
					salida = new UserMessage(Id, 0).AddStringToSend(">QIME<");
                    break;
                case Reporte.IdReq:
                    msgId = ParserUtils.MsgIdNotSet; //para no enviar ack sino un query
					salida = Mensaje.FactorySetId(msgId, Id, ParserUtils.GetDeviceIdTaip(buffer));
                    break;
                default: //es un ack o un mensaje no reconocido, no se responde, solo paso la info arriba
                    salida = new UserMessage(Id, msgId);
					//CheckFota(msgId);
            		break;
            }
            if ((msgId != ParserUtils.MsgIdNotSet) && (!salida.IsPending()))
            	salida.AddStringToSend(BuildAck(buffer));

			return salida;
        }

		protected override int MessageSequenceMin { get { return 0x8000; } }
		protected override int MessageSequenceMax { get { return 0xFFFF; } }

        #endregion
        
        #region Members

		private static String BuildAck(String msg)
		{
			var fin = msg.LastIndexOf(";*", StringComparison.Ordinal);
			var ini = msg.LastIndexOf(";#", fin, StringComparison.Ordinal);

			var dgram = String.Format(">ACK{0};*", msg.Substring(ini, fin - ini));
			return String.Format("{0}{1:X2}<", dgram, Mensaje.GetCheckSumFg(dgram));
		}

		private static IMessage GetSalida(String buffer, INode device, ulong msgId, byte[] data)
		{
			byte entradas;
			int codEv;

			var gpsPoint = GPSPoint.ParseGp(buffer, out entradas, out codEv, false);

            return GetSalidaReal(gpsPoint, "00000000", codEv, device, msgId, data);
		}

		private static IMessage GetSalida(byte[] data, INode device, ulong msgId)
		{
			var gpsPoint = Posicion.Parse(data);
			var codEv = data[17]; // codigo del Evento generador del reporte

			var rfidT = BitConverter.ToUInt32(data, 23);
			var rfid = rfidT == 0xFFFFFFFF ? "00000000" : rfidT.ToString(CultureInfo.InvariantCulture);

            return GetSalidaReal(gpsPoint, rfid, codEv, device, msgId, data);
		}

		private static IMessage GetSalidaReal(GPSPoint gpsPoint, String rfid, int codEv, INode device, ulong msgId, byte[] data)
		{
			MessageIdentifier codigo;
			switch (codEv) // codigo del Evento generador del reporte
			{
				case Evento.MotorOn:             codigo = MessageIdentifier.EngineOnInternal;		break;
				case Evento.MotorOff:            codigo = MessageIdentifier.EngineOffInternal;		break;
				case Evento.GarminOn:            codigo = MessageIdentifier.GarminOn;				break;
				case Evento.GarminOff:           codigo = MessageIdentifier.GarminOff;				break;
				case Evento.PanicButtonOn:       codigo = MessageIdentifier.PanicButtonOn;			break;
				case Evento.PanicButtonOff:      codigo = MessageIdentifier.PanicButtonOff;			break;
				case Evento.AmbulanciaSirenaOn:  codigo = MessageIdentifier.SirenOn;				break;
				case Evento.AmbulanciaSirenaOff: codigo = MessageIdentifier.SirenOff;				break;
				case Evento.AmbulanciaBalizaOn:  codigo = MessageIdentifier.BeaconOn;				break;
				case Evento.AmbulanciaBalizaOff: codigo = MessageIdentifier.BeaconOff;				break;
				case Evento.GsmOn:               codigo = MessageIdentifier.GsmSignalOn;			break;
				case Evento.GsmOff:              codigo = MessageIdentifier.GsmSignalOff;			break;
				case Evento.GpsSignalOn:         codigo = MessageIdentifier.GpsSignalOn;			break;
				case Evento.GpsSignalOff:        codigo = MessageIdentifier.GpsSignalOff;			break;
				case Evento.Gps3DSignalOff:      codigo = MessageIdentifier.GpsSignal3DOff;			break;
				case Evento.Gps2DSignalOn:       codigo = MessageIdentifier.GpsSignal2DOn;			break;
				case Evento.PistonOff:           codigo = MessageIdentifier.PistonOff;				break;
				case Evento.PistonOn:            codigo = MessageIdentifier.PistonOn;				break;
				case Evento.DistanciaSinChofer:  codigo = MessageIdentifier.NoDriverMovement;		break;
				case Evento.DistanciaMotorOff:   codigo = MessageIdentifier.NoEngineMovement;		break;

				case Evento.ChoferLoggedOn:  return MessageIdentifierX.FactoryRfid(device.Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, 0);
				case Evento.ChoferLoggedOff: return MessageIdentifierX.FactoryRfid(device.Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, 1);

				case Evento.Infraccion:
					var maxVel = data[14];
					var maxPermitida = Speed.KmToKnot(80); //volvemos al hardcoded ya q no hay qtree
					var posIni = device.Retrieve<GPSPoint>("IniInfraccion") ?? gpsPoint;
					return new SpeedingTicket(device.Id, msgId, posIni, gpsPoint, maxVel, maxPermitida, rfid);
				case Evento.InfraccionInicio:
					device.Store("IniInfraccion", gpsPoint);
					goto default;
				default:
					return gpsPoint.ToPosition(device.Id, msgId);
			}
			return codigo.FactoryEvent(device.Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, null);
		}

		private static String GetEntrada(String tmp)
		{
            const int fgTrashLenght = 37;
			try
			{
				var ini = tmp.IndexOf('>');
				var trash = (tmp.StartsWith(Reporte.EventoFg)) ? fgTrashLenght : 0;
				var len = (tmp.IndexOf('<', ini + trash) - ini) + 1;
				var entrada = tmp.Substring(ini, len);
                //ParserUtils.CheckChecksumOk(entrada, ParserUtils.GetCheckSumFg);
				return entrada;
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(Parser).FullName, e, tmp);
				return null;
			}
		}

		private static String GetTipoReporte(IList<String> cade)
    	{
    		return cade[0].Length < 4 ? Reporte.Nada : cade[0].Substring(0, 4);
    	}

		private abstract class Reporte
		{
			public const String Nada = "Nada";
			public const String SinNodo = "SinNodo";
			public const String EventoFg = ">RFG";
			public const String EventoGp = ">RGP";
			public const String IdReq = ">RIM";
		}

		//si no es uno de estos es una posicion
		private static class Evento
		{
			public const int PistonOn = 31;
			public const int PistonOff = 32;
			public const int PanicButtonOn = 39;
			public const int PanicButtonOff = 40;
			public const int AmbulanciaSirenaOn = 41;
			public const int AmbulanciaSirenaOff = 42;
			public const int AmbulanciaBalizaOn = 43;
			public const int AmbulanciaBalizaOff = 44;

			public const int GarminOn = 47;
			public const int GarminOff = 48;

			//public const int Trompo = 55;
			//public const int Tolva = 56;

			public const int MotorOn = 61;
			public const int MotorOff = 62;
			public const int GsmOn = 63;
			public const int GsmOff = 64;
			public const int GpsSignalOn = 65;
			public const int Gps3DSignalOff = 66;
			public const int Gps2DSignalOn = 67;
			public const int GpsSignalOff = 68;
			public const int ChoferLoggedOn = 69;
			public const int ChoferLoggedOff = 70;

			public const int InfraccionInicio = 97;
			public const int Infraccion = 99;
			public const int DistanciaMotorOff = 0xCC;
			public const int DistanciaSinChofer = 0xCF;
		}

		#endregion
	}
}