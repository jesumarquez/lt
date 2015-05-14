using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.SimComLite
{
	[FrameworkElement(XName = "SimComLiteParser", IsContainer = false)]
	public class Parser : BaseCodec, IFoteable, IPowerBoot
	{
        public override NodeTypes NodeType { get { return NodeTypes.Simcomlite; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2012)]
		public override int Port { get; set; }

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

			var s = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length).Split(',');

			if (!s[0].StartsWith("+")) return null;

			if (s[0] == Report.Exception || s[0] == Report.Exception2)
			{
				//deberia detectar si es un error en el foteo y archivar lo que se quiera fotear?
				return new TextEvent(Id, 0, DateTime.UtcNow) { Text = String.Format("Error en dispositivo: Tipo={0} Comando={1} Parameter={2} Extra={3}", s[1], s[2], s[3], s[4]) };
			}
			
			var mid = GetMessageId(s.Last());

			var tiempo = DateTimeUtils.SafeParseFormat(s[s.Length - 2], "yyyyMMddHHmmss");
			String peeked;
			var res = Decode2(mid, s, tiempo, out peeked);
			if (res != null)
			{
				res.Tiempo = tiempo;

				if (!s[0].StartsWith("+ACK:") || s[0] == Report.Heartbeat)
				{
					res.AddStringToSend(String.Format("+SACK:{0}", s.Last()));
					peeked = Fota.Peek(this);
				}
				res.AddStringToPostSend(peeked);
			}

			return res;
		}

		#endregion

		#region Private Members

		private IMessage Decode2(ulong mid, String[] s, DateTime dtsent, out String peeked)
		{
			peeked = null;
			var ss = s[0].Replace("+BUFF", "+RESP");
			switch (ss)
			{
				case Report.Fixed: return ParsePositions(Id, mid, s);
				case Report.CrossBorderEvent: return ParseEventWithPosition(Id, mid, s, s[5] == "1" ? MessageIdentifier.InsideGeoRefference : MessageIdentifier.OutsideGeoRefference, dtsent, 7, null);
				case Report.SpeedAlarm: return ParsePosition(Id, mid, s);
				case Report.SosEvent: return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PanicButtonOn, dtsent, 7, null);
				case Report.RtoRtl: return ParsePosition(Id, mid, s);
				case Report.DevicePowerUpFirstPosition: return ParseEventWithPosition(Id, mid, s, MessageIdentifier.GpsSignalOn, dtsent, 7, null);
				case Report.NonMovementEvent:
					{
						var ev =
							(s[5] == "0") ?  MessageIdentifier.StoppedEvent :
							((s[5] == "1") ? MessageIdentifier.StartMovementEvent :
							/*s[5] == "2"*/  MessageIdentifier.Freefall);
						return ParseEventWithPosition(Id, mid, s, ev, dtsent, 7, null);
					}

				case Report.LocationByCall:
					{
						var res = (Event)ParseEventWithPosition(Id, mid, s, MessageIdentifier.BlackcallIncoming, dtsent, 5, null);
						res.SensorsDataString = s[4];
						return res;
					}
				case Report.LocationAsCentreOfGeofence: return ParsePosition(Id, mid, s);
				case Report.DeviceInformation: return new UserMessage(Id, mid);
				case Report.GpsRequest: return new UserMessage(Id, mid);
				case Report.GetAllConfiguration: return new UserMessage(Id, mid);
				case Report.RealTimeOperationCid: return new UserMessage(Id, mid);
				case Report.RealTimeOperationCsq: return new UserMessage(Id, mid);
				case Report.RealTimeOperationVer: return new UserMessage(Id, mid);
				case Report.RealTimeOperationBat: return FactoryEventWithoutPosition(Id, mid, MessageIdentifier.BateryInfo, String.Format("BatteryPercentage:{0},BatteryVoltage:{1},ExternalPowerSupply:{2},Charging:{3},LedOn:{4}", s[6], s[7], s[4], s[8], s[9]));
				case Report.RealTimeOperationTmz: return new UserMessage(Id, mid);

				case Report.PowerOn: return FactoryEventWithoutPosition(Id, mid, MessageIdentifier.DeviceTurnedOn, null);
				case Report.PowerOff: return FactoryEventWithoutPosition(Id, mid, MessageIdentifier.DeviceShutdown, null);
				case Report.ConnectingExternalPowerSupply: return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PowerReconnected, dtsent, 4, null);
				case Report.DisconnectingExternalPowerSupply: return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PowerDisconnected, dtsent, 4, null);
				case Report.BatteryLow: return ParseEventWithPosition(Id, mid, s, MessageIdentifier.BateryLow, dtsent, 5, String.Format("BatteryVoltage:{0}", s[4]));
				case Report.StartCharging: return ParseEventWithPosition(Id, mid, s, MessageIdentifier.BatteryChargingStart, dtsent, 4, null);
				case Report.StopCharging: return ParseEventWithPosition(Id, mid, s, MessageIdentifier.BatteryChargingStop, dtsent, 5, null);
				case Report.DeviceMotionStateIndication: return new UserMessage(Id, mid);
				case Report.GpsAntennaStatusIndication: return new UserMessage(Id, mid);
				case Report.SwitchOnOffGeofence0ViaFunctionKey: return new UserMessage(Id, mid);

				case Report.Heartbeat: return ParseHeartbeat(Id, mid, s);

				default:
					if (s[0].StartsWith("+ACK:"))
					{
						var midR = Convert.ToUInt64(s[s.Length-3], 16);
						peeked = Fota.Peek(this);
						var midF = Convert.ToUInt64(peeked.Split(',').Last().TrimEnd('$'), 16);
						if (midR == midF)
						{
							Fota.Dequeue(this, null);
						}
					}
					return null;
			}
		}

		private static IMessage ParsePositions(int id, ulong mid, String[] s)
		{
			var count = Convert.ToInt32(s[6]);
			var pl = new List<GPSPoint>();
			for (var i = 0; i < count; i++)
			{
				pl.Add(ParseGpsPoint(s, 7 + i * 12));
			}
			//var batteryPercentage = s[s.Length-3];
			return pl.ToPosition(id, mid);
		}

		private static IMessage ParseEventWithPosition(int id, ulong mid, String[] s, MessageIdentifier ev, DateTime dtsent, int offset, String datastring)
		{
			var pos = ParseGpsPoint(s, offset);
			if (pos.GetDate().AddHours(12) < dtsent) pos = null;
			var res = ev.FactoryEvent(id, mid, pos, dtsent, null, null);
			res.SensorsDataString = datastring;
			return res;
		}

		private static IMessage ParsePosition(int id, ulong mid, String[] s)
		{
			return ParseGpsPoint(s, 7).ToPosition(id, mid);
		}

		private static IMessage FactoryEventWithoutPosition(int id, ulong mid, MessageIdentifier mi, String datastring)
		{
			var res = mi.FactoryEvent(id, mid, null, ((GPSPoint) null).GetDate(), null, null);
			res.SensorsDataString = datastring;
			return res;
		}

		private static IMessage ParseHeartbeat(int id, ulong mid, String[] s)
		{
			return new UserMessage(id, mid)
				.AddStringToSend(String.Format("+SACK:GTHBD,{0},{1}", s[1], s[5]))
				.SetUserSetting("user_message_code", "KEEPALIVE");
		}

		private static GPSPoint ParseGpsPoint(String[] s, int offset)
		{
			var gpsAccuracy = Convert.ToInt32(s[offset]); //(hdop)
			if (gpsAccuracy == 0) return null;

			return GPSPoint.Factory(
				DateTimeUtils.SafeParseFormat(s[offset + 6], "yyyyMMddHHmmss"),
				Convert.ToSingle(s[offset + 5], CultureInfo.InvariantCulture),
				Convert.ToSingle(s[offset + 4], CultureInfo.InvariantCulture),
				Convert.ToSingle(s[offset + 1], CultureInfo.InvariantCulture),
				0,
				Convert.ToSingle(s[offset + 3], CultureInfo.InvariantCulture),
				gpsAccuracy);

			//var GPSaccuracy = Convert.ToByte(s[offset]); //(hdop)
			//var Speed = new Speed(Convert.ToSingle(s[offset + 1], CultureInfo.InvariantCulture));
			//var Azimuth = Convert.ToInt32(s[offset + 2], CultureInfo.InvariantCulture);
			//var Altitude = new Altitude(Convert.ToSingle(s[offset + 3], CultureInfo.InvariantCulture));
			//var Longitude = Convert.ToSingle(s[offset + 4], CultureInfo.InvariantCulture);
			//var Latitude = Convert.ToSingle(s[offset + 5], CultureInfo.InvariantCulture);
			//var dt = FechasUtm.SafeParseFormat(s[offset + 6], "yyyyMMddHHmmss");
			//var MCC = s[offset + 7]; //Mobile country code of the service cell, it is 3 digits in length and ranges from 000 to 999.
			//var MNC = s[offset + 8]; //Mobile network code of the service cell, it is 3 digits in length and ranges from 000 to 999.
			//var LAC = s[offset + 9]; //Location area code in hex format of the service cell.
			//var CELL ID = s[offset + 10]; //CELL ID in hex format of the service cell.
			//reserved
		}

		private static class Report
		{
			public const String Fixed = "+RESP:GTFRI";
			public const String CrossBorderEvent = "+RESP:GTGEO";
			public const String SpeedAlarm = "+RESP:GTSPD";
			public const String SosEvent = "+RESP:GTSOS";
			public const String RtoRtl = "+RESP:GTRTL"; //este se pide desde el server
			public const String DevicePowerUpFirstPosition = "+RESP:GTPNL";
			public const String NonMovementEvent = "+RESP:GTNMR";

			public const String LocationByCall = "+RESP:GTLBC";
			public const String LocationAsCentreOfGeofence = "+RESP:GTGCR";
			public const String DeviceInformation = "+RESP:GTINF";
			public const String GpsRequest = "+RESP:GTGPS"; //este se pide desde el server
			public const String GetAllConfiguration = "+RESP:GTALL";
			public const String RealTimeOperationCid = "+RESP:GTCID";
			public const String RealTimeOperationCsq = "+RESP:GTCSQ";
			public const String RealTimeOperationVer = "+RESP:GTVER";
			public const String RealTimeOperationBat = "+RESP:GTBAT";
			public const String RealTimeOperationTmz = "+RESP:GTTMZ";

			public const String PowerOn = "+RESP:GTPNA"; //puede no tener gps?
			public const String PowerOff = "+RESP:GTPFA";
			public const String ConnectingExternalPowerSupply = "+RESP:GTEPN";
			public const String DisconnectingExternalPowerSupply = "+RESP:GTEPF";
			public const String BatteryLow = "+RESP:GTBPL";
			public const String StartCharging = "+RESP:GTBTC";
			public const String StopCharging = "+RESP:GTSTC";
			public const String DeviceMotionStateIndication = "+RESP:GTSTT"; 
			public const String GpsAntennaStatusIndication = "+RESP:GTANT"; 
			public const String SwitchOnOffGeofence0ViaFunctionKey = "+RESP:GTSWG";

			public const String Exception = "+RESP:EXCEPTION";
			public const String Exception2 = "+BUFF:EXCEPTION"; 
			public const String Heartbeat = "+ACK:GTHBD"; 
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

        public bool ResetFMIOnGarmin(ulong messageId)
        {
            throw new NotImplementedException();
        }

        public Boolean? IsGarminConnected { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		public bool ReloadConfiguration(ulong messageId)
		{
			return true;
		}

		public bool ContainsMessage(String line)
		{
			return String.IsNullOrEmpty(line);
		}

		public ulong GetMessageId(String line)
		{
			var midS = line.TrimEnd('$');
			return Convert.ToUInt64(midS, 16) + ((ulong)line[0].GetHashCode()) << 32;
		}
	
		public INodeMessage LastSent { get; set; }

		#endregion

        #region SendMessages

        private void SendMessages(String config, ulong messageId)
        {
            Fota.Enqueue(this, messageId, config);
        }

        #endregion

		#region IPowerBoot

		public bool Reboot(ulong messageId)
		{
			var password = DataProvider.GetDetalleDispositivo(Id, "Password").As("AIR11");
			SendMessages(String.Format("AT+GTRTO={0},3,,,,,,{1:D4}$", password, messageId), messageId);
			return true;
		}

		#endregion
	}
}
