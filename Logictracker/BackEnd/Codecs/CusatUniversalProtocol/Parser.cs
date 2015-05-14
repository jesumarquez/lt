using System;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.Backend.Codec.CusatUniversal
{
	[FrameworkElement(XName = "CusatUniversalParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Cusat; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 5053)]
		public override int Port { get; set; }

		[ElementAttribute(XName = "Empresa", IsRequired = false, DefaultValue = "")]
		public String Empresa { get; set; }

		[ElementAttribute(XName = "UseIntern", IsRequired = false, DefaultValue = false)]
		public bool UseIntern { get; set; }

		[ElementAttribute(XName = "SwapInternAndPatent", IsRequired = false, DefaultValue = false)]
		public bool SwapInternAndPatent { get; set; }

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

			if (String.IsNullOrEmpty(buffer) || !buffer.Contains(">RU")) return null;

			var matricula = buffer.Substring(4, 8).Trim();
			var interno = buffer.Substring(12, 8).Trim();
			var emp = buffer.Substring(20, 8).Trim();
			String empresa;
			String linea;
			if (String.IsNullOrEmpty(Empresa))
			{
				empresa = emp;
				linea = null;
			}
			else
			{
				empresa = Empresa; //seguramente es "Cusat"
				linea = emp;
			}

			return ((SQLDataProvider)DataProvider).Find(empresa, linea, SwapInternAndPatent ? interno : matricula, SwapInternAndPatent ? matricula : interno, UseIntern, this);
        }

		public override IMessage Decode(IFrame frame)
        {
			if (ParserUtils.IsInvalidDeviceId(Id)) return null;

			var buffer = AsString(frame);
            if (buffer == null || !buffer.Contains(">RU")) return null;

			var dt = DateTimeUtils.SafeParseFormat(buffer.Substring(28, 12), "ddMMyyHHmmss");
			var codEv = GetEventCode(buffer);

			//var msgId = GetMsgId(buffer); //siempre es 0001!!!
			var msgId = (ulong)((dt.Ticks << 8) + codEv);


			var gpsPoint = ParseRu2(buffer, dt);
			return GetSalida(gpsPoint, dt, "00000000", codEv, this, msgId);
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

						/*if (frame.PayloadAsString.Contains(">RU1"))
						{
							frame.PayloadAsString = ">RU2" + frame.PayloadAsString.Substring(4, 8) + "                " + frame.PayloadAsString.Substring(12);
						}//*/
					}
				}
			}
			return frame.PayloadAsString;
		}

        #endregion
        
        #region Members

		/*private static ulong GetMsgId(String buffer)
		{
			if (String.IsNullOrEmpty(buffer) || !buffer.Contains("#")) return ParserUtils.MsgIdNotSet;

			var ini = buffer.IndexOf("#") + 1;

			var id = Convert.ToUInt64(buffer.Substring(ini, 4), 16);

			return id == 0 ? ParserUtils.CeroMsgId : id;
		}//*/

		private static int GetEventCode(String buffer)
		{
			int codEv;
			try
			{
				codEv = Convert.ToInt32(buffer.Substring(73, 2));
			}
			catch (FormatException)
			{
				codEv = Convert.ToInt32(buffer.Substring(73, 2), 16);
			}
			return codEv;
		}

		private static GPSPoint ParseRu2(String buffer, DateTime dt)
		{
			var lat = Convert.ToSingle(buffer.Substring(40, 8)) * (float)0.00001;
			var lon = Convert.ToSingle(buffer.Substring(48, 9)) * (float)0.00001;
			var fix = Convert.ToInt32(buffer.Substring(57, 1)); //FIX 0=Err, 1=No fix, 2=2D, 3=3D y 4=OK
			if (fix < 2) return null;
			var age = Convert.ToInt32(buffer.Substring(58, 3));
			if ((age / 60) > 4) return null;
			var vel = Convert.ToSingle(buffer.Substring(61, 3));
			var dir = Convert.ToSingle(buffer.Substring(64, 3));
			//var cantSat = Convert.ToInt32(buffer.Substring(67, 2));
			//var pdop = Convert.ToInt32(buffer.Substring(69, 2));
			var entradas = Convert.ToByte(buffer.Substring(71, 2), 16);

			return new GPSPoint(dt, lat, lon, vel, GPSPoint.SourceProviders.Unespecified, 0)
			{
				Course = new Course(dir),
				IgnitionStatus = ((entradas & 0x80) == 0x80) ? IgnitionStatus.On : IgnitionStatus.Off,
			};
		}

		private static IMessage GetSalida(GPSPoint gpsPoint, DateTime dt, String rfid, int codEv, INode device, ulong msgId)
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
					var pos = gpsPoint.ToPosition(device.Id, msgId);
					pos.Tiempo = dt;
					return pos;
			}
			
			return codigo.FactoryEvent(device.Id, msgId, gpsPoint, dt, rfid, null);
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