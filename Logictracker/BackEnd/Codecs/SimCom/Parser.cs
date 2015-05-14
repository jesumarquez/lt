using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace Logictracker.SimCom
{
	[FrameworkElement(XName = "SimComParser", IsContainer = false)]
	public class Parser : BaseCodec, IFoteable, IPowerBoot
	{
        public override NodeTypes NodeType { get { return NodeTypes.Simcom; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2004)]
		public override int Port { get; set; }

		[ElementAttribute(XName = "Password", IsRequired = false, DefaultValue = "vl2000")]
		public String Password { get; set; }

        private String _fotaFolder;

        public String FotaFolder
        {
            get
            {
                if (_fotaFolder == null)
                    _fotaFolder = Process.GetApplicationFolder("FOTA");
                return _fotaFolder;
            }
        }

		#endregion

		#region IFoteable

		public bool ReloadFirmware(ulong messageId)
		{
			return true;
		}

        public bool ReloadMessages(ulong messageId)
        {
            return true;
        }

		public bool ReloadConfiguration(ulong messageId)
		{
			String dummyhash;
			var config = GetConfig(out dummyhash);
			if (!String.IsNullOrEmpty(config)) SendMessages(config, messageId);
			return true;
		}

        public bool ResetFMIOnGarmin(ulong messageId)
        {
            throw new NotImplementedException();
        }

        public Boolean? IsGarminConnected { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		public bool ContainsMessage(String line)
		{
			return line.StartsWith("AT");
		}

		public INodeMessage LastSent { get; set; }

		private String GetConfig(out String hash)
		{
			var config = new StringBuilder(DataProvider.GetConfiguration(Id).Replace("\r", "\n").Replace("\n", Environment.NewLine));
			if (String.IsNullOrEmpty(config.ToString()))
			{
				hash = null;
				return null;
			}

			//reemplazar cada "$ParameterName" con "ParameterValue"
			var configParametersTable = DataProvider.GetDetallesDispositivo(Id);
			if (configParametersTable != null)
			{
				var configParameters = configParametersTable.Where(detalle => detalle.TipoParametro.Nombre.StartsWith("CONFIG_PARAM_"));
				foreach (var s in configParameters) config.Replace("$" + s.TipoParametro.Nombre, s.As("--- PARAMETRO SIN VALOR! ---"));
			}

			if (!String.IsNullOrEmpty(config.ToString()))
			{
				config.AppendFormat("{0}{0}Configuracion Generada Exitosamente{0}", Environment.NewLine);
				config.Insert(0, Fota.VirtualMessageFactory(MessageIdentifier.ConfigStart, 0));
			}
			else
			{
				config.AppendLine("Configuracion vacia.");
			}

			config.Append(Fota.VirtualMessageFactory(MessageIdentifier.ConfigSuccess, 0));

			var conf = config.ToString();
			hash = conf.GetHashCode().ToString(CultureInfo.InvariantCulture);
			return conf.Replace("$revision", hash);
		}

		//private DateTime reconfiglimit;

		#region SendMessages

		private void SendMessages(String config, ulong messageId)
		{
			Fota.Enqueue(this, messageId, config);
		}

		#endregion

		#endregion

		#region IPowerBoot

		public bool Reboot(ulong messageId)
		{
			var model = DataProvider.GetDetalleDispositivo(Id, "Model").As("vl2000");
			var password = DataProvider.GetDetalleDispositivo(Id, "Password").As("vl2000");
			switch (model)
			{
				case "vl1000":
					SendMessages(String.Format("AT+GTIGN={0},0,{1:D4}$", password, messageId), messageId);
					break;
				case "vl2000":
					SendMessages(String.Format("AT+GTRTO={0},3,,,,,,,{1:D4}$", password, messageId), messageId);
					break;
			}
			
			return true;
		}

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
			var s = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length).Split(',');
			return !s[0].StartsWith("+") ? null : DataProvider.FindByIMEI(s[2], this);
		}

		public override IMessage Decode(IFrame frame)
		{
			if (ParserUtils.IsInvalidDeviceId(Id)) return null;

			/*if (reconfiglimit < DateTime.UtcNow)
			{
				reconfiglimit = DateTime.UtcNow.AddDays(10);
				String dummyhash;
				var config = GetConfig(out dummyhash);
				if (!String.IsNullOrEmpty(config)) SendMessages(config, 0);
			}//*/

			var buffer = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
			var s = buffer.Split(',');

			if (!s[0].StartsWith("+")) return null;

			var mid = GetMessageId(buffer) + ((ulong)s[0].GetHashCode()) << 32;

			var tiempo = DateTimeUtils.SafeParseFormat(s[s.Length - 2], "yyyyMMddHHmmss");
			if (tiempo.AddHours(72) < DateTime.UtcNow) tiempo = DateTime.UtcNow;
			var res = Decode2(mid, s, tiempo);
			if (res != null) res.Tiempo = tiempo;
			return res;
		}

		/*protected override void  OnMemberwiseClone()
		{
			//reconfiglimit = DateTime.MinValue;
		}//*/

		#endregion

		#region Private Members

		private IMessage Decode2(ulong mid, String[] s, DateTime dtsent)
		{
			var msg = Decode3(mid, s, dtsent);
			if (msg == null) return null;
			var pending = Fota.Peek(this);
			if (!String.IsNullOrEmpty(pending))
			{
				Lastsentmessageid = GetMessageId(pending);
				if (msg.IsPending())
					msg.AddStringToPostSend(pending);
				else
					msg.AddStringToSend(pending);
			}
			return msg;
		}

		private IMessage Decode3(ulong mid, String[] s, DateTime dtsent)
		{
			switch (s[0])
			{
				case Report.NonMovementEvent:
					return ParsePosition(Id, mid, s);
				case Report.MovementEvent:
					return ParsePosition(Id, mid, s);
				case Report.GpsRequest:
					return ParsePosition(Id, mid, s);

				case Report.Fixed:
					return ParsePositionsFixed(Id, mid, s);
				case Report.GpsHistoryFixRecords:
					return ParseGpsHistoryFixRecords(Id, mid, s);

				case Report.DevicePowerDown:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.DeviceShutdown, dtsent);
				case Report.CrossBorderEvent:
					return ParseEventWithPosition(Id, mid, s, s[5] == "1" ? MessageIdentifier.InsideGeoRefference : MessageIdentifier.OutsideGeoRefference, dtsent);
				case Report.SpeedAlarm:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.SpeedingTicketEnd, dtsent);
				case Report.PowerKeyShortPressEvent:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PowerKeyShortPress, dtsent);
				case Report.FreefallEvent:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.Freefall, dtsent);

				case Report.DeviceInformation:
					return ParseEventWithoutPosition(Id, mid, MessageIdentifier.DeviceOnLine);
				case Report.BlackCallIncoming:
					{
						var res = (Event) ParseEventWithoutPosition(Id, mid, MessageIdentifier.BlackcallIncoming);
						res.SensorsDataString = s[4];
						return res;
					}

				case Report.SosEvent:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PanicButtonOn, dtsent).AddStringToSend(String.Format("AT+GTRTO={1},C,,,,,,,{0:X4}$", NextSequence, DataProvider.GetDetalleDispositivo(Id, "Password").As(Password)));
				case Report.Heartbeat:
					return ParseHeartbeat(Id, mid, s);

				case Report.BatteryTimer:
					return ParseBatteryTimer(Id, mid, s);

				default:
					if (s[0].StartsWith("+ACK"))
					{
						var mid2 = GetMessageId2(s, Id);
						if ((Lastsentmessageid == mid2))
						{
							Fota.Dequeue(this, null);
						}
					}
					return new UserMessage(Id, mid);
			}
		}

		public ulong GetMessageId(String line)
		{
			return Convert.ToUInt64(line.Split(',').Last().TrimEnd('$'), 16);
		}

		private static ulong GetMessageId2(String[] pending, int deviceId)
		{
			try
			{
				return pending.Length < 3 ? 0 : Convert.ToUInt64(pending[pending.Length - 3], 16);
			}
			catch(Exception)
			{
				STrace.Debug(typeof(Parser).FullName, deviceId, String.Format("GetMessageId2 bug: {0}", String.Join(",", pending)));
				return 0;
			}
		}

		private static IMessage ParseEventWithoutPosition(int id, ulong mid, MessageIdentifier mi)
		{
			return mi.FactoryEvent(id, mid, null, DateTime.UtcNow, null, null);
		}

		private static IMessage ParseHeartbeat(int id, ulong mid, String[] s)
		{
			return new UserMessage(id, mid)
				.AddStringToSend(String.Format("+SACK:GTHBD,{0},{1}", s[1], s[5])) //s[5] contiene "$" al final
				.SetUserSetting("user_message_code", "KEEPALIVE");
		}

		private IMessage ParseBatteryTimer(int id, ulong mid, string[] s)
		{
            var ev = MessageIdentifier.BateryInfo.FactoryEvent(id, mid, null, DateTime.UtcNow, null, new []{Convert.ToInt64(s[6])});
			ev.SensorsDataString = String.Format(CultureInfo.InvariantCulture,
				"BatteryPercentage:{0},BatteryVoltage:{1}",
				s[6], //BatteryPercentage 0-100 %
				s[7]); //Battery voltage 0-4500 mV
			return ev;
		}

		private static IMessage ParsePosition(int id, ulong mid, String[] s)
		{
			return ParseGpsPoint(s, 6).ToPosition(id, mid);
		}

		private static IMessage ParseEventWithPosition(int id, ulong mid, String[] s, MessageIdentifier ev, DateTime dtsent)
		{
			var pos = ParseGpsPoint(s, 6);
			if (pos.GetDate().AddHours(12) < dtsent) pos = null;
			return ev.FactoryEvent(id, mid, pos, dtsent, null, null);
		}

		private static IMessage ParsePositionsFixed(int id, ulong mid, String[] s)
		{
			var count = Convert.ToInt32(s[6]) + 1;
			var pl = new List<GPSPoint>();
			for (var i = 0; i < count; i++)
			{
				pl.Add(ParseGpsPoint(s, 7 + i*14));
			}
			return pl.ToPosition(id, mid);
		}

		private static GPSPoint ParseGpsPoint(String[] s, int offset)
		{
			return GPSPoint.Factory(
				DateTimeUtils.SafeParseFormat(s[offset + 7], "yyyyMMddHHmmss"),
				Convert.ToSingle(s[offset + 6], CultureInfo.InvariantCulture),
				Convert.ToSingle(s[offset + 5], CultureInfo.InvariantCulture),
				Convert.ToSingle(s[offset + 2], CultureInfo.InvariantCulture));
			//var dop = s[offset];
			//var GPSaccuracy = s[offset + 1];
			//var Speed = new Speed(Convert.ToSingle(s[offset + 2], CultureInfo.InvariantCulture));
			//var Azimuth = Convert.ToInt32(s[offset + 3], CultureInfo.InvariantCulture);
			//var Altitude = new Altitude(Convert.ToSingle(s[offset + 4], CultureInfo.InvariantCulture));
			//var Longitude = Convert.ToSingle(s[offset + 5], CultureInfo.InvariantCulture);
			//var Latitude = Convert.ToSingle(s[offset + 6], CultureInfo.InvariantCulture);
			//var dt = FechasUtm.SafeParseFormat(s[offset + 7], "yyyyMMddHHmmss");
			//var MCC = s[offset + 8]; //Mobile country code of the service cell
			//var MNC = s[offset + 9]; //Mobile network code of the service cell.
			//var LAC = s[offset + 10]; //Location area code in hex format of the service cell.
			//var CELL ID = s[offset + 11]; //CELL ID in hex format of the service cell.
			//var TA = s[offset + 12]; //Timing advance. Blank when it cannot be obtained.
			//var CSQ RSSI = s[offset + 13]; //Receiving signal strength indication: 0=less than or equal -115dBm; 1=-111dBm; 2...30=-110...-54dBm; 31=greater than or equal to -52dBm; 99=unknown or undetectable
		}

		private static IMessage ParseGpsHistoryFixRecords(int id, ulong mid, String[] s)
		{
			return GPSPoint.Factory(
				DateTimeUtils.SafeParseFormat(s[9], "yyyyMMddHHmmss"),
				Convert.ToSingle(s[4], CultureInfo.InvariantCulture),
				Convert.ToSingle(s[5], CultureInfo.InvariantCulture),
				Convert.ToSingle(s[7], CultureInfo.InvariantCulture))
				.ToPosition(id, mid);
		}

		private static class Report
		{
			public const String DeviceInformation = "+RESP:GTINF";
			public const String DevicePowerDown = "+RESP:GTPWD";
			public const String SosEvent = "+RESP:GTSOS";
			public const String NonMovementEvent = "+RESP:GTNMR";
			public const String MovementEvent = "+RESP:GTMOV";
			public const String CrossBorderEvent = "+RESP:GTGEO";
			public const String SpeedAlarm = "+RESP:GTSPD";
			public const String PowerKeyShortPressEvent = "+RESP:GTPKS";
			public const String BlackCallIncoming = "+RESP:GTBCI";
			public const String GpsRequest = "+RESP:GTGPS"; //este se pide desde el server
			public const String Fixed = "+RESP:GTFRI";
			public const String GpsHistoryFixRecords = "+RESP:GTGHR";
			public const String FreefallEvent = "+RESP:GTFFA";
			public const String Heartbeat = "+RESP:GTHBD";
			public const String BatteryTimer = "+RESP:GTBAT";
		}

		#endregion
	}
}
