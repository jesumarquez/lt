#region Usings

using System;
using Logictracker.AVL.Messages;
using Logictracker.Model;
using Logictracker.Utils;
using System.Globalization;

#endregion

namespace Logictracker.MiniMT.v1
{
	public static class PositionReport
	{
		public static bool IsPositionReport(String buffer)
		{
			return buffer.Contains("$GPRMC");
		}

		public static IMessage Factory(String buffer, int node)
		{
			var partes = buffer.Split(',');
			if (partes[3] != "A") return null;
			var time = DateTimeUtils.SafeParseFormat(partes[10] + partes[2].Substring(0, 6), "ddMMyyHHmmss");
			var latitud = GPSPoint.ResampleAxis(partes[4])*((partes[5] == "N") ? 1 : -1);
			var longitud = GPSPoint.ResampleAxis(partes[6])*((partes[7] == "E") ? 1 : -1);
			var velocidad = Speed.KnotToKm(Convert.ToSingle(partes[8], CultureInfo.InvariantCulture));
			var direccion = Convert.ToSingle(partes[9]);
			return GPSPoint.Factory(time, latitud, longitud, velocidad, direccion, 0, 0).ToPosition(node, 0);
		}
	}
}