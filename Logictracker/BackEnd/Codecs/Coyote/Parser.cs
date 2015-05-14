using System;
using System.Collections.Generic;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.Coyote
{
    [FrameworkElement(XName = "CoyoteParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Coyote; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 5052)]
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
			var buffer = AsString(frame);
			return String.IsNullOrEmpty(buffer) ? null : DataProvider.FindByIMEI(GetDeviceId(buffer), this);
        }

		public override IMessage Decode(IFrame frame)
        {
            var buffer = AsString(frame);
            if (buffer == null) return null;

            IMessage salida;

            var msgId = ParserUtils.GetMsgIdTaip(buffer);
            var data = buffer.Split(';')[0].Split(',');
            var tipoReporte = Reporte.GetTipoReporte(data);

            switch (tipoReporte)
            {
				case Reporte.Nada:
            		return null;
                case Reporte.EventoCq:
					{
						if (ParserUtils.IsInvalidDeviceId(Id)) return null;

						byte entradas;
						int codEv;
						var gpsPoint = ParseCq(buffer, out entradas, out codEv, false); //TODO chequear que sea en kilometros o knots
						salida = GetSalida(gpsPoint, "00000000", codEv, this, msgId);
						break;
					}
                default: //es un ack o un mensaje no reconocido, no se responde, solo paso la info arriba
                    salida = new UserMessage(Id, msgId);
					//CheckFota(msgId);
            		break;
            }
            if ((msgId != ParserUtils.MsgIdNotSet) && (!salida.IsPending()))
            	salida.AddStringToSend(BuildAck(buffer));

			return salida;
        }

		public override String AsString(IFrame frame)
		{
			if (frame.PayloadAsString == null)
			{
				frame.PayloadAsString = base.AsString(frame);
				var ini = frame.PayloadAsString.IndexOf('>');
				if (ini >= 0)
				{
					var len = (frame.PayloadAsString.LastIndexOf('<') - ini) + 1;
					if (len > 0)
					{
						frame.PayloadAsString = frame.PayloadAsString.Substring(ini, len);
						//ParserUtils.CheckChecksumOk(frame.PayloadAsString, ";*", "<", GetCheckSum);
					}
				}
			}
			return frame.PayloadAsString;
		}

        #endregion
        
        #region Members

		private static String GetDeviceId(String buffer)
		{
			var ini = buffer.IndexOf(";ID=");

			if (ini == -1) return null;

			ini += 4;

			var len = buffer.IndexOf(';', ini) - ini;

			return buffer.Substring(ini, len);
		}

		private static IMessage GetSalida(GPSPoint gpsPoint, String rfid, int codEv, INode device, ulong msgId)
		{
			MessageIdentifier codigo;
			switch (codEv) // codigo del Evento generador del reporte
			{
				case Evento.Panico: codigo = MessageIdentifier.PanicButtonOn; break;
				case Evento.DoorOpenned: codigo = MessageIdentifier.DoorOpenned; break;
				case Evento.SemiTrailerUnhook: codigo = MessageIdentifier.TrailerUnHooked; break;
				case Evento.Substitute: codigo = MessageIdentifier.SubstituteViolation; break;
				case Evento.Generic01: codigo = MessageIdentifier.CustomMsg1On; break;
				case Evento.Generic02: codigo = MessageIdentifier.CustomMsg2On; break;
				case Evento.EngineOn: codigo = MessageIdentifier.EngineOn; break;
				case Evento.EngineOff: codigo = MessageIdentifier.EngineOff; break;
				//case Evento.Periodic01: goto default;
				//case Evento.Periodic02: goto default;
				case Evento.DeviceTurnedOn: codigo = MessageIdentifier.DeviceTurnedOn; break;
				default:
					return gpsPoint.ToPosition(device.Id, msgId);
			}
			
			return codigo.FactoryEvent(device.Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, null);
		}

		private static GPSPoint ParseCq(String data, out byte entradas, out int evento, bool knotsFlag)
		{
			try
			{
				evento = Convert.ToInt32(data.Substring(4, 2));
			}
			catch (FormatException)
			{
				evento = Convert.ToInt32(data.Substring(4, 2), 16);
			}
			var time = DateTimeUtils.SafeParseFormat(data.Substring(6, 12), "ddMMyyHHmmss");
			var lat = Convert.ToSingle(data.Substring(18, 8)) * (float)0.00001;
			var lon = Convert.ToSingle(data.Substring(26, 9)) * (float)0.00001;
			var vel = Convert.ToSingle(data.Substring(35, 3));
			if (knotsFlag) vel = Speed.KnotToKm(vel);
			var dir = Convert.ToSingle(data.Substring(38, 3));
			entradas = Convert.ToByte(data.Substring(41, 2), 16);
			//var salidas = Convert.ToByte(data.Substring(43, 2));
			//var batteryVoltage = Convert.ToInt32(data.Substring(45, 3));
			//var odometerMeters = Convert.ToInt32(data.Substring(48, 8), 16);
			//var gpsOnFlag = data.Substring(56, 1) == '1';
			//var pdop = Convert.ToInt32(data.Substring(57, 2));
			//var satcount = Convert.ToInt32(data.Substring(59, 2));
			//var edad = Convert.ToInt32(data.Substring(61, 4), 16);
			//var gprsOnFlag = data.Substring(65, 1) == '1';
			//var gsmState = data.Substring(66, 1); //0=Not registered, ME is not currently searching a new operador to register to; 1=Registered, home network; 2=Not registered, but ME is currently searching a new operator to register to; 3=Registration denied; 4=Unknown; 5=Registered, roaming
			//var csq = Convert.ToInt32(data.Substring(67, 2)); //Nivel de señal CEL (CSQ); 99= Open / No Signal; 0-30= igual Level

			return new GPSPoint(time, lat, lon, vel, GPSPoint.SourceProviders.Unespecified, 0)
			{
				Course = new Course(dir),
				IgnitionStatus = ((entradas & 0x80) == 0x80) ? IgnitionStatus.On : IgnitionStatus.Off,
			};
		}

		private static String BuildAck(String msg)
		{
			var fin = msg.LastIndexOf(";*", StringComparison.Ordinal);
			var ini = msg.LastIndexOf(";#", fin, StringComparison.Ordinal);

			var dgram = String.Format(">ACK{0};*", msg.Substring(ini, fin - ini));
			return String.Format("{0}{1:X2}<", dgram, GetCheckSum(dgram));
		}

		private static int GetCheckSum(String message)
		{
			var lon = message.LastIndexOf(";*", StringComparison.Ordinal);
			if (lon == -1)
			{
				lon = message.Length;
			}
			else
			{
				lon += 2;
			}

			var chksum = 0;
			for (var i = 0; i < lon; i++)
				chksum ^= message[i];

			return chksum;
		}

		private abstract class Reporte
		{
			public const String Nada = "Nada";
			public const String EventoCq = ">RCQ";

			internal static String GetTipoReporte(IList<String> cade)
			{
				return cade[0].Length < 4 ? Nada : cade[0].Substring(0, 4);
			}
		}

		//si no es uno de estos es una posicion
		private abstract class Evento
		{
			public const int Panico = 1;
			public const int DoorOpenned = 2;
			public const int SemiTrailerUnhook = 3;
			public const int Substitute = 4;
			public const int Generic01 = 5;
			public const int Generic02 = 6;
			public const int EngineOn = 8;
			public const int EngineOff = 16;
			//public const int Periodic01 = 17; //posicion
			//public const int Periodic02 = 18; //posicion
			public const int DeviceTurnedOn = 23;
		}

		#endregion
	}
}