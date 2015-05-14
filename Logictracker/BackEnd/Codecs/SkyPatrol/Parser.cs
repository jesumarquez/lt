using System;
using System.Globalization;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.SkyPatrol
{
	[FrameworkElement(XName = "SkyPatrolParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Skypatrol; } }

        #region Attributes

	    [ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2006)]
		public override int Port { get; set; }

		[ElementAttribute(XName = "Password", IsRequired = false, DefaultValue = "TT8850")]
		public String Password { get; set; }

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
			if (frame.Payload[0] != 0) return null;
			if (frame.Payload[1] == 5)
			{
				var imei = new StringBuilder();
				for (var i = 0; i < 15; i++)
					imei.Append((char)frame.Payload[9 + i]);
				return DataProvider.FindByIMEI(imei.ToString(), this);
			}
			if (frame.Payload[1] == 4)
			{
				var s = Encoding.ASCII.GetString(frame.Payload, 2, frame.Payload.Length - 2).Split(',');
				return DataProvider.FindByIMEI(s[5], this);
			}
			return null;
		}

		public override IMessage Decode(IFrame frame)
		{
			if (frame.Payload[0] != 0) return null;
			if (ParserUtils.IsInvalidDeviceId(Id)) return null;
			if (frame.Payload[1] == 5)
			{
				var mid = frame.Payload[frame.Payload.Length - 2] << 8 + frame.Payload[frame.Payload.Length - 1];
				return new UserMessage(Id, (ulong)mid).AddStringToSend(String.Format("+SACK:{0:X4}$", mid));
			}
			if (frame.Payload[1] == 4)
			{
				var s = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length).Split(',');

				var midS = s[s.Length - 1].TrimEnd('$');
				var mid = Convert.ToUInt64(midS, 16) + ((ulong)s[3].GetHashCode()) << 32;

				var res = Decode2(mid, s);
				if (res != null)
				{
					res.Tiempo = DateTimeUtils.SafeParseFormat(s[s.Length - 2], "yyyyMMddHHmmss");
					if (s[2] == "1") res.AddStringToSend(String.Format("+SACK:{0}$", midS));
				}

				return res;
			}
			return null;
		}

		#endregion

		#region Private Members

		private IMessage Decode2(ulong mid, String[] s)
		{
			switch (s[3])
			{
				case Report.DeviceStateChanged:
					//var ReportType = Convert.ToInt32(s[8]); //it means the current state of the device,
					//21: The device attached vehicle is ignition on and motionless.
					//22: The device attached vehicle is ignition on and moving.
					//41: The device is motionless without ignition on.
					//42: The device is moving without ignition on.
				case Report.GpsRequest:
				case Report.DeviceInformation:
				case Report.Heartbeat:
				case Report.LocationCenterOfGeofence:
				case Report.SwitchingGeofence0:
					return FactoryK(Id, mid);

				case Report.BatteryPercentageTimer:
					return ParseBateryInfo(Id, mid, MessageIdentifier.BateryInfo, s);

				case Report.Fixed:
				case Report.RealTimeLocation:
				case Report.LocationByCallRequest:
					return ParsePosition(Id, mid, s);

				case Report.CrossBorderEvent:
					{
						var reportId = Convert.ToInt32(s[7]); //ID of Geo‐Fence
						var reportType = Convert.ToInt32(s[8]) == 1; //enter the corresponding Geo‐Fence flag
						var ev = (Event)ParseEventWithPosition(Id, mid, s, reportType ? MessageIdentifier.InsideGeoRefference : MessageIdentifier.OutsideGeoRefference);
						ev.SensorsDataString = "Geofence:" + reportId;
						return ev;
					}
				case Report.SpeedAlarm:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.SpeedingTicketEnd);
				case Report.SosEvent:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PanicButtonOn);
				case Report.PowerOnLocation:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.GpsSignalOn);
				case Report.NonMovementEvent:
					{
						var reportType = Convert.ToInt32(s[8]) == 1; //The state of the device changed from rest to motion flag
						return ParseEventWithPosition(Id, mid, s, reportType ? MessageIdentifier.StartMovementEvent : MessageIdentifier.StoppedEvent);
					}
				case Report.PowerOnReport:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.DeviceTurnedOn);
				case Report.PowerOffReport:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.DeviceShutdown);
				case Report.ExternalPowerSupplyConnectedReport:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PowerReconnected);
				case Report.ExternalPowerSupplyDisconnectedReport:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.PowerDisconnected);
				case Report.LowBatteryVoltage:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.BateryLow);
				case Report.ChargingStarted:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.BateryReConected);
				case Report.ChargingStopped:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.BateryDisconected);
				case Report.GpsAntennaIndication:
					{
						var reportId = Convert.ToInt32(s[7]) == 1; //the device is using the external GPS antenna flag
						return ParseEventWithPosition(Id, mid, s, reportId ? MessageIdentifier.UsingExternalAntenna : MessageIdentifier.UsingInternalAntenna);
					}
				case Report.PdpConnectionReset:
					return ParseEventWithPosition(Id, mid, s, MessageIdentifier.DeviceOnLine);
			}
			return null;
		}

		private static IMessage FactoryK(int id, ulong mid)
		{
			return new UserMessage(id, mid)
				.SetUserSetting("user_message_code", "KEEPALIVE");
		}

		private static IMessage ParseBateryInfo(int id, ulong mid, MessageIdentifier mi, String[] s)
		{
			var ev = mi.FactoryEvent(id, mid, null, ((GPSPoint) null).GetDate(), null, null);
			ev.SensorsDataString = String.Format(CultureInfo.InvariantCulture,
				"ExternalPowerSupplyFlag:{0},BatteryPercentage:{1},BatteryVoltage:{2},ChargingFlag:{3},LedOnFlag:{4}",
				s[7], //External power supply, Boolean
				s[9], //Battery percentage, 0..100
				s[10], //Battery voltage, 0..450 (0.0V .. 4.50V)
				s[11], //Charging, Boolean
				s[12]); //LED on, Boolean
			return ev;
		}

		private static IMessage ParsePosition(int id, ulong mid, String[] s)
		{
			return ParseGpsPoint(s).ToPosition(id, mid) ?? FactoryK(id, mid);
		}

		private static IMessage ParseEventWithPosition(int id, ulong mid, String[] s, MessageIdentifier mi)
		{
			var pos = ParseGpsPoint(s);
			return mi.FactoryEvent(id, mid, pos, pos.GetDate(), null, null);
		}

		private static GPSPoint ParseGpsPoint(String[] s)
		{
			if (String.IsNullOrEmpty(s[16])) return null;

		    var fechaHora = DateTimeUtils.SafeParseFormat(s[16], "yyyyMMddHHmmss");
		    var latitude = Convert.ToSingle(s[15], CultureInfo.InvariantCulture);
		    var longitude = Convert.ToSingle(s[14], CultureInfo.InvariantCulture);
		    var speed = Convert.ToSingle(s[11], CultureInfo.InvariantCulture);
            var azimuth = Convert.ToSingle(s[12], CultureInfo.InvariantCulture);
            var hdop = Convert.ToSingle(s[10], CultureInfo.InvariantCulture);
            var altitude = Convert.ToSingle(s[13], CultureInfo.InvariantCulture);
		    
			var pos = GPSPoint.Factory(fechaHora, latitude, longitude, speed, azimuth, altitude, hdop);

	        return pos;

		    //var Speed = new Speed(Convert.ToSingle(s[11], CultureInfo.InvariantCulture));
		    //var Longitude = Convert.ToSingle(s[14], CultureInfo.InvariantCulture);
		    //var Latitude = Convert.ToSingle(s[15], CultureInfo.InvariantCulture);
		    //var dt = FechasUtm.SafeParseFormat(s[16], "yyyyMMddHHmmss");
		    //var MCC = s[17]; //Mobile country code of the service cell
		    //var MNC = s[18]; //Mobile network code of the service cell.
		    //var LAC = s[19]; //Location area code in hex format of the service cell.
		    //var CELL ID = s[20]; //CELL ID in hex format of the service cell.
		    //var battery_percentage = s[21];
		}

		private static class Report
		{
			public const String Heartbeat = "GTHBD";
			public const String GpsRequest = "GTGPS"; //este se pide desde el server
			public const String DeviceInformation = "GTINF";

			public const String BatteryPercentageTimer = "GTBAT";

			public const String Fixed = "GTFRI";
			public const String CrossBorderEvent = "GTGEO";
			public const String SpeedAlarm = "GTSPD";
			public const String SosEvent = "GTSOS";
			public const String RealTimeLocation = "GTRTL";
			public const String PowerOnLocation = "GTPNL";
			public const String NonMovementEvent = "GTNMR";
			public const String LocationByCallRequest = "GTLBC";
			public const String LocationCenterOfGeofence = "GTGCR";
			public const String PowerOnReport = "GTPNA";
			public const String PowerOffReport = "GTPFA";
			public const String ExternalPowerSupplyConnectedReport = "GTEPN";
			public const String ExternalPowerSupplyDisconnectedReport = "GTEPF";
			public const String LowBatteryVoltage = "GTBPL";
			public const String ChargingStarted = "GTBTC";
			public const String ChargingStopped = "GTSTC";
			public const String DeviceStateChanged = "GTSTT";
			public const String GpsAntennaIndication = "GTANT";
			public const String PdpConnectionReset = "GTPDP";
			public const String SwitchingGeofence0 = "GTSWG";
		}

		#endregion
	}
}
