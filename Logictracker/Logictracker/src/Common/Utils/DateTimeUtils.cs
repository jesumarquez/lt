#region Usings

using System;
using System.Globalization;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Utils
{
    public static class DateTimeUtils
    {
    	public static DateTime SafeParseFormat(String entrada, String format)
    	{
    		try
    		{
    			return DateTime.ParseExact(entrada, format, CultureInfo.InvariantCulture);
    		}
    		catch (Exception e)
    		{
				STrace.Debug(typeof(DateTimeUtils).FullName, String.Format("SafeParseFormat: entrada={0} format={1} Error={2}", entrada, format, e.Message));
				return new DateTime(2000, 1, 1);
    		}
    	}

    	public static DateTime SafeParseFormatWithTraxFix(String entrada, String format)
    	{
    		try
    		{
    			var res = DateTime.ParseExact(entrada, format, CultureInfo.InvariantCulture);
    	        return res;
    		}
    	    catch (Exception) { }

			try
			{
				var dd = int.Parse(entrada.Substring(0, 2));
				var mo = int.Parse(entrada.Substring(2, 2));
				var yy = int.Parse(entrada.Substring(4, 2));
				var HH = int.Parse(entrada.Substring(6, 2));
				var mi = int.Parse(entrada.Substring(8, 2));
				var ss = int.Parse(entrada.Substring(10, 2));

				return new DateTime(yy, mo, dd, HH, mi, ss);
			}
			catch (Exception)
			{
				return new DateTime(2000, 1, 1);
			}
    	}

	    public static double ToUnixTimeStamp(this DateTime date)
		{
			var origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			var diff = date - origin;
			return Math.Floor(diff.TotalSeconds);
		}

		public static DateTime FromUnixTimeStamp(int seconds)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
		}
    }
}
