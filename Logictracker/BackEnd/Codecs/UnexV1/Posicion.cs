#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Logictracker.AVL.Messages;
using Logictracker.Model;
using Logictracker.Utils;

#endregion

namespace Logictracker.Unetel.v1
{
	internal class Posicion
	{
		private enum GIRO_TROMPO
		{
			Derecha,
			Izquierda,
			Reposo
		} ;

		internal static IMessage Parse(String asString, INode node)
		{
			Debug.Assert(node != null);

			// [8-12] : sentido de giro del trompo (5)
			var girotrompo = new GIRO_TROMPO[5];
			for (var i = 0; i < 5; i++)
			{
				var sg = asString[8 + i];

				switch (sg)
				{
					case 'D': girotrompo[i] = GIRO_TROMPO.Derecha; break;
					case 'I': girotrompo[i] = GIRO_TROMPO.Izquierda; break;
					case 'N': girotrompo[i] = GIRO_TROMPO.Reposo; break;
					default: girotrompo[i] = GIRO_TROMPO.Reposo; break;
				}
			}

			var pl = new List<GPSPoint>();
			// [7] : Cantidad de posiciones en el paquete (0 a 5)
			//[13-47][48-82][83-117][118-152][153-187] : posiciones
			// hasta 5 la cantidad esta dada por CantPos
			// hhmmss,ddmm.mmmm,[N|S|I],0ddmm.mmmm,[W|E|I],vvv.
			var cantPos = Convert.ToInt32(asString.Substring(7, 1));
			for (var i = 0; i < cantPos; i++)
			{
				var subMsg = asString.Substring(13 + (36 * i), 36);

				var ordLat = subMsg[17];
				var ordLon = subMsg[30];

				float lat, lon;

				//parseo la latitud
				switch (ordLat)
				{
					case 'N': lat = GPSPoint.ResampleAxis(subMsg.Substring(7, 9)); break;
					case 'S': lat = -GPSPoint.ResampleAxis(subMsg.Substring(7, 9)); break;
					default: continue;
				}

				//parseo la longitud
				switch (ordLon)
				{
					case 'E': lon = GPSPoint.ResampleAxis(subMsg.Substring(19, 9)); break;
					case 'W': lon = -GPSPoint.ResampleAxis(subMsg.Substring(19, 9)); break;
					default: continue;
				}

				pl.Add(GPSPoint.Factory(ExtraeHhmmss(subMsg.Substring(0, 6)), lat, lon, Convert.ToInt32(subMsg.Substring(32, 3))));
			}

			return pl.ToPosition(node.Id, 0);
		}

		private static DateTime ExtraeHhmmss(string hora)
		{
			var hh = Convert.ToInt32(hora.Substring(0, 2));
			var mm = Convert.ToInt32(hora.Substring(2, 2));
			var ss = Convert.ToInt32(hora.Substring(4, 2));

			return HackSampeDatetime(hh, mm, ss);
		}

		private static DateTime HackSampeDatetime(int hh, int mm, int ss)
		{
			var fechora = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, mm, ss, DateTimeKind.Utc);

			// Tomando un margen de 1 hora para evitar diferencias de reloj, 
			// si la fecha es mayor a la actual le resto un dia. 
			// Es imposible que llegue una posicion futura,
			// entonces asumo que es del dia anterior.
			return fechora > DateTime.UtcNow.AddHours(1) ? fechora.Subtract(TimeSpan.FromDays(1)) : fechora;
		}
	}
}