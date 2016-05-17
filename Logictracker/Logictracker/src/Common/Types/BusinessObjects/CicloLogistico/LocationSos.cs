using System;
using System.Globalization;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    [Serializable]
    public class LocationSos
    {
        public LocationSos(string lat, string lon, string dir, string loc)
        {
            Latitud = double.Parse(lat, CultureInfo.InvariantCulture);
            Longitud = double.Parse(lon, CultureInfo.InvariantCulture);
            Direccion = dir;
            Localidad = loc;
        }

        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Referencia { get; set; }
    }
}