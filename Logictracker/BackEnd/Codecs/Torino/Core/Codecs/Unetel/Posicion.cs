#region Usings

using System;
using System.Globalization;
using System.Text;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Common.Utils;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Comm.Core.Codecs.Unetel
{
    public class Posicion
    {
        public static Urbetrack.Comm.Core.Mensajeria.Posicion Parse(byte[] ins)
        {
            var partes = Encoding.ASCII.GetString(ins).Split(";".ToCharArray());
            var result = new Urbetrack.Comm.Core.Mensajeria.Posicion
                             {
                                 Saliente = false,
                                 Seq = Convert.ToByte(partes[1]),
                                 IdDispositivo = Convert.ToInt16(partes[2]),
                                 Posiciones =
                                     (partes[3][0] == '*'
                                          ? Convert.ToByte(partes[3].Substring(1))
                                          : Convert.ToByte(partes[3]))
                             };
            var c = 0;
            foreach(var parte in partes)
            {
                c++;
                if (c < 5) 
                    continue;
                var pos = ParsearPosicion(parte);
                if (pos == null) continue;
                result.Puntos.Add(pos);
            }
            return result;
        }

        public byte[] Ack(PDU source)
        {
            var rta = String.Format("RQ2;{0:D4};{1:D5}", source.Seq, source.IdDispositivo);
            return Encoding.ASCII.GetBytes(rta);
        }

        public static GPSPoint ParsearPosicionCorta(string src)
        {
            try
            {
                var partes = src.Split(",".ToCharArray());
                var date = partes[0];
                if (partes.GetLength(0) < 3)
                    return null;
                var point = new GPSPoint
                                {
                                    Date = new DateTime(Convert.ToInt32(date.Substring(0, 2)) + 2000,
                                                        Convert.ToInt32(date.Substring(4, 2)),
                                                        Convert.ToInt32(date.Substring(2, 2)),
                                                        Convert.ToInt32(date.Substring(6, 2)),
                                                        Convert.ToInt32(date.Substring(8, 2)),
                                                        Convert.ToInt32(date.Substring(10, 2))),
                                    Lat = ResampleAxis(partes[1]),
                                    Lon = ResampleAxis(partes[2]),
                                    Speed = new Speed(0),
                                    Course = new Course(0),
                                    Height = new Altitude(0),
                                    HDOP = 0,
                                    LcyStatus = 0
                                };
                return point;
            }
            catch (Exception e)
            {
                T.EXCEPTION(e);
            }
            return null;
        }

        public static GPSPoint ParsearPosicion(string src)
        {
            try
            {
                var partes = src.Split(",".ToCharArray());
                var nfi = new CultureInfo("en-US", false).NumberFormat;
                var date = partes[0];
                if (partes.GetLength(0) < 5) 
                    return null;
                var point = new GPSPoint
                                {
                                    Date = new DateTime(Convert.ToInt32(date.Substring(0, 2)) + 2000,
                                                        Convert.ToInt32(date.Substring(4, 2)),
                                                        Convert.ToInt32(date.Substring(2, 2)),
                                                        Convert.ToInt32(date.Substring(6, 2)),
                                                        Convert.ToInt32(date.Substring(8, 2)),
                                                        Convert.ToInt32(date.Substring(10, 2))),
                                    Lat = ResampleAxis(partes[1]),
                                    Lon = ResampleAxis(partes[2]),
                                    Speed = new Speed(Convert.ToSingle(String.IsNullOrEmpty(partes[3]) ? "0" : partes[3], nfi)),
                                    Course = new Course(Convert.ToSingle(String.IsNullOrEmpty(partes[4]) ? "0" : partes[4], nfi)),
                                    Height = new Altitude(0),
                                    HDOP = 0,
                                    LcyStatus = 0
                                };
                return point;
            }
            catch(Exception e)
            {
                T.EXCEPTION(e);
            }
            return null;
        }

        internal static float ResampleAxis(string s)
        {
            var nfi = new CultureInfo("en-US", false).NumberFormat;
            var t = Convert.ToSingle(s, nfi);
            var grados = (int) (t/100);
            var fracciongrado = ((t/100) - grados)/60*100;
            return grados + fracciongrado;
        }

    }
}