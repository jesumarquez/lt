#region Usings

using System;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Gateway.Joint.MessageQueue
{
    internal static class Aux
    {
        public static double UniqueGPSPoint(this GPSPoint punto)
        {
            return (punto.Date - DateTime.MinValue).TotalSeconds + Math.Round(punto.Lat, 6) + Math.Round(punto.Lon, 6);
        }
    }
}
