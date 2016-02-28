#region Usings

using System;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Utils;

#endregion

namespace Alas
{
	public static class Posicion
	{
		public static GPSPoint Parse(String[] data)
		{
            //check edad
            if ((data[3].Length > 20) && (Convert.ToInt32(data[3].Substring(20, 4)) > 300)) return null;

            DateTime time;
			
			try
			{
				time = DateTimeUtils.SafeParseFormatWithTraxFix(data[1], "ddMMyyHHmmss");
			}
        	catch (Exception)
        	{
        		var ss = new StringBuilder();
				foreach (var s in data) ss.Append(s);
				STrace.Debug(typeof(Posicion).FullName, String.Format(@"DateTime invalido ""{0}""", ss));
				time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        	}
			var lat = Convert.ToSingle(data[3].Substring(0, 8))*(float) 0.00001;
			var lon = Convert.ToSingle(data[3].Substring(8, 9))*(float) 0.00001;
			var dir = Convert.ToSingle(data[3].Substring(17, 3));
			var hdop = Convert.ToByte(Convert.ToSingle(data[4].Substring(0, 3))/10);
			var vel = Speed.KnotToKm(Convert.ToSingle(data[5]));

			//STrace.Trace(GetType().FullName,"TraxDecodePoint {0};{1};vel={2};dir={3};hdop={4};{5:u}", lat, lon, vel, dir, hdop, time);

			return GPSPoint.Factory(time, lat, lon, vel, dir, 0, hdop);
		}

		public static GPSPoint ParseOld(String[] data)
		{
			var time = DateTimeUtils.SafeParseFormatWithTraxFix(data[1], "ddMMyyHHmmss");
			var lat = Convert.ToSingle(data[4])*(float) 0.00001;
			var lon = Convert.ToSingle(data[5])*(float) 0.00001;
			var dir = Convert.ToSingle(data[6]);
			var hdop = Convert.ToByte(Math.Round(Convert.ToSingle(data[7])/10));
			var vel = Speed.KnotToKm(Convert.ToSingle(data[9]));

			//STrace.Trace(GetType().FullName,"TraxDecodePoint {0};{1};{2};{3};{4:u}", lat, lon, vel, dir, time);

			return GPSPoint.Factory(time, lat, lon, vel, dir, 0, hdop);
		}

		public static GPSPoint ParseCompact(String data, bool hdopF)
		{
            var time = DateTimeUtils.SafeParseFormatWithTraxFix(data.Substring(0, 12), "ddMMyyHHmmss");
			var lat = Convert.ToSingle(data.Substring(12, 8))*(float) 0.00001;
			var lon = Convert.ToSingle(data.Substring(20, 9))*(float) 0.00001;
			var dir = Convert.ToSingle(data.Substring(29, 3));
			var vel = Convert.ToSingle(data.Substring(32, 3));
			var hdop = hdopF ? Convert.ToByte(Convert.ToSingle(data.Substring(35, 3))/10) : (byte) 0;

			return GPSPoint.Factory(time, lat, lon, vel, dir, 0, hdop);
		}
	}
}