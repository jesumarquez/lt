#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using Logictracker.AVL.Messages;
using Logictracker.Model;
using Logictracker.Utils;

#endregion

namespace Logictracker.Unetel.v2
{
    public class Posicion
    {
        public static IMessage Parse(String[] partes, INode node)
        {
        	var pl = new List<GPSPoint>();
        	for (var index = 4; index < partes.Length; index++)
        	{
        		var pos = ParsearPosicion(partes[index]);
        		if (pos == null) continue;
        		pl.Add(pos);
        	}
			return pl.ToPosition(node.GetDeviceId(), 0).AddStringToSend(String.Format("RQ2;{0}", partes[1]));
        }

		public static GPSPoint ParsearPosicionCorta(String src)
        {
            var partes = src.Split(',');
            var date = partes[0];
            if (partes.GetLength(0) < 3) return null;

			DateTime t;
			try
			{
				t = new DateTime(Convert.ToInt32(date.Substring(0, 2)) + 2000,
				                 Convert.ToInt32(date.Substring(4, 2)),
				                 Convert.ToInt32(date.Substring(2, 2)),
				                 Convert.ToInt32(date.Substring(6, 2)),
				                 Convert.ToInt32(date.Substring(8, 2)),
								 Convert.ToInt32(date.Substring(10, 2)), DateTimeKind.Utc);
			}
			catch (FormatException)
			{
				return null;
			}
			
			return GPSPoint.Factory(t, GPSPoint.ResampleAxis(partes[1]), GPSPoint.ResampleAxis(partes[2]));
        }

		private static GPSPoint ParsearPosicion(String src)
        {
            var partes = src.Split(",".ToCharArray());
            var date = partes[0];
            if ((date.Length < 12) || (partes.GetLength(0) < 5)) return null;
        	DateTime t;
			try
			{
				t = new DateTime(Convert.ToInt32(date.Substring(0, 2)) + 2000,
									 Convert.ToInt32(date.Substring(4, 2)),
									 Convert.ToInt32(date.Substring(2, 2)),
									 Convert.ToInt32(date.Substring(6, 2)),
									 Convert.ToInt32(date.Substring(8, 2)),
									 Convert.ToInt32(date.Substring(10, 2)), DateTimeKind.Utc);
			}
			catch (FormatException)
			{
				return null;
			}
            return GPSPoint.Factory(
				t,
				GPSPoint.ResampleAxis(partes[1]),
				GPSPoint.ResampleAxis(partes[2]),
				Speed.KnotToKm(Convert.ToSingle(String.IsNullOrEmpty(partes[3]) ? "0" : partes[3], CultureInfo.InvariantCulture)),
				Convert.ToSingle(String.IsNullOrEmpty(partes[4]) ? "0" : partes[4], CultureInfo.InvariantCulture),
				0,
				0);
        }
    }
}