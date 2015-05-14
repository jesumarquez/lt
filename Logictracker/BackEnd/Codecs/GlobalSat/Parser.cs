using System;
using System.Globalization;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.GsTraq
{
	[FrameworkElement(XName = "GsTraqParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Gstraq; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2001)]
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
			var datos = buffer.Split(',');
			return DataProvider.FindByIMEI(datos[(datos[1] == Reporte.DevInfAck) ? 0 : 1], this);
		}

		public override IMessage Decode(IFrame frame)
		{
			/*
			Format 0 Default=SORPZAB27GHKLMN*U!

			S	IMEI
			O	Operation mode: 1=Sleeping, 2=Periodic, 3=On-line, 4=Motion
			R	Report type
			P	Alarm status
			Z	Geo-fence statu
			A	1=not fix, 2=2D fix, 3=3D fix
			B	UTC Date, Time  ddmmyy,hhmmss
			2	Longitude  (E or W)dddmm.mmm
			7	Latitude  (N or S)ddmm.mmmm
			G	Altitude  xxxxx.x Unit: meter
			H	Speed  xxx.xx unit: knots (1.852km/hr)
			K	Heading  xxx unit: degree
			L	Number of satellite in use xx 
			M	HDOP  xx.x 
			N	Battery capacity  xx unit: percent capacity
			*U!	Checksum

			GSr,IMEI,[T,S,]Device_Mode,Report_Type,Alarm_Status,Geofence_status,GPS_Fix,UTC_Date,UTC_Time,Longitude,Latitude,Altitude,Speed,Heading,Number_of_Satellites,HDOP,Battery_capacity*checksum!
			GSr,011412000845531,2,2,00,,1,020109,022556,+0,+0,135,0.00,0,0,0.0,94#68!
			*/

			var buffer = AsString(frame);
			ParserUtils.CheckChecksumOk(buffer, "*", null, ParserUtils.GetCheckSumNmea);
			IMessage result;

			var data = buffer.Split(',');
			switch (data[0])
			{
				case Reporte.DevInfPositionAndStatusReportFormat0:
					result = PositionParse(data).ToPosition(Id, 0).AddStringToSend(Reporte.SrvCmdAck);
					break;
				default:
					result = null;
					if (data[1] != Reporte.DevInfAck)
					{
						result = new UserMessage(Id, Convert.ToUInt64(data[3]))
							.AddStringToSend(Reporte.SrvCmdAck);
					}
					break;
			}
			return result;
		}

		#endregion

		#region Properties

		private static GPSPoint PositionParse(String[] src)
		{
			//GSr,IMEI,[T,S,]Device_Mode,Report_Type,Alarm_Status,Geofence_status,GPS_Fix,UTC_Date,UTC_Time,Longitude,Latitude,Altitude,Speed,Heading,Number_of_Satellites,HDOP,Battery_capacity*checksum!
			//GSr,011412000845531,2,2,00,,1,020109,022556,+0,+0,135,0.00,0,0,0.0,94#68!
			//GSr,011412000845531,2,2,00,,3,050112,185436,-58425608,-34614105,65,0.04,350,7,1.4,100#64!

			if ((src[6] != "2") && (src[6] != "3")) return null;
			var time = DateTimeUtils.SafeParseFormat(src[7] + src[8], "ddMMyyHHmmss");
			var lat = GetDegrees(src[10]);
			var lon = GetDegrees(src[9]);

			var dir = Convert.ToSingle(src[13], CultureInfo.InvariantCulture);
			var vel = Speed.KnotToKm(Convert.ToSingle(src[12], CultureInfo.InvariantCulture));
			var hdop = Convert.ToByte(Math.Round(Convert.ToSingle(src[15], CultureInfo.InvariantCulture)));
			return GPSPoint.Factory(time, lat, lon, vel, dir, 0, hdop);
		}

		private static float GetDegrees(String src)
		{
			if (src.Contains("."))
			{
				src = src.Replace("W", "-").Replace("S", "-").Replace("N", "+").Replace("E", "+");
				return GPSPoint.ResampleAxis(src);
			}
			return Convert.ToSingle(src.Insert(3, "."), CultureInfo.InvariantCulture);
		}

		private abstract class Reporte
		{
			//public const String SrvCmd_WriteSetting = "GSS";
			//public const String DevInf_ReportSetting = "GSs";
			//public const String SrvCmd_WriteGeofenceParameter = "GSG";
			//public const String DevInf_GeofenceParameter = "GSg";
			//public const String SrvCmd_ActionCommand = "GSC";
			public const String DevInfPositionAndStatusReportFormat0 = "GSr";
			//public const String DevInf_PositionAndStatusReportFormat1 = "GSh";
			public const String DevInfAck = "ACK\r\n";
			public const String SrvCmdAck = "ACK\r";
		}

		#endregion
	}
}
