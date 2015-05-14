#region Usings

using System;
using Logictracker.Utils;

#endregion

namespace Logictracker.FulMar
{
	public static class Posicion
	{
		public static GPSPoint Parse(byte[] data)
		{
			//var time = Utils.FromUnixTimeStamp(BitConverter.ToUInt32(data, 0));
			var time = FromFulmarTime(data, 0);

			var lat = Convert.ToSingle(BitConverter_BigEndian.ToInt32(data, 4)) / 100000;
			var lon = Convert.ToSingle(BitConverter_BigEndian.ToInt32(data, 8)) / 100000;

			//var edad = data[12];
			var vel = Convert.ToSingle(data[13]);
			//var velPico = data[14];
			var dir = Convert.ToSingle(data[15]) * 2; //puaj, viene dividida por 2 para q entre en un byte

			return GPSPoint.Factory(time, lat, lon, vel, dir, 0, 0);
		}

		private static DateTime FromFulmarTime(byte[] t, int i)
        {
            //fulmar usa un formato binario propietario empaquetado
            //45,		16,			0xFB,	0xE1
            //00101101, 00010000, 11111011, 11100001

            // 001011.., ........, ........, ........	año 6 bits FC .. .. ..	10  --  2010
            // ......01, 00......, ........, ........	mes 4 bits .3 C. .. ..	04  --  abril
            // ........, ..01000., ........, ........	dia 5 bits .. 3E .. ..	8   --  dia 8
            // ........, .......0, 1111...., ........	hor 5 bits .. .1 F. ..	15  --  15 utm -> 12 horas
            // ........, ........, ....1011, 11......	min 6 bits .. .. .F C.	47  --  y 47 minutos
            // ........, ........, ........, ..100001	seg 6 bits .. .. .. 3F	33  --  y 33 segundos

            var yy = 2000 + t[i].Bbti(0xFC, 2);
            var mm = t[i].Bbti(0x03, -2) + t[1 + i].Bbti(0xC0, 6);
            var dd = t[1 + i].Bbti(0x3E, 1);
            var hh = t[1 + i].Bbti(0x01, -4) + t[2 + i].Bbti(0xF0, 4);
            var m2 = t[2 + i].Bbti(0x0F, -2) + t[3 + i].Bbti(0xC0, 6);
            var ss = t[3 + i].Bbti(0x3F, 0);

			return new DateTime(yy, mm, dd, hh, m2, ss, DateTimeKind.Utc);
        }

        /// <summary>
        /// ByteBitsToInt
        /// obtiene un valor tomando los bits de un byte
        /// </summary>
        /// <param name="bits">el byte original</param>
        /// <param name="mask1">primer mascara que desecha los bits que no deseo</param>
        /// <param name="shift">corrimiento a la derecha, puede ser negativo y se corre a la izquierda, o cero y no pasa nada</param>
        /// <returns>el valor de los bits seleccionados convertidos a un entero de 32 bits</returns>
        private static Int32 Bbti(this byte bits, int mask1, int shift)
        {
            var b1 = Convert.ToUInt32(bits & mask1);
            if (shift < 0)
            {
                shift = shift * -1;
                b1 = b1 << shift;
            }
            else
            {
                b1 = b1 >> shift;
            }

            return Convert.ToInt32(b1);
        }
    }
}