using System;
using System.Globalization;

namespace Logictracker.Services.Helpers
{
    [Serializable]
    public class LatLon
    {
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public LatLon(double latitud, double longitud)
        {
            Latitud = latitud;
            Longitud = longitud;
        }

        public override string ToString()
        {
            return string.Concat(Latitud.ToString(CultureInfo.InvariantCulture), ",",
                                 Longitud.ToString(CultureInfo.InvariantCulture));
        }
    }
}
