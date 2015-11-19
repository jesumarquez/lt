namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class LocationSos
    {
        public LocationSos(string lat, string lon, string dir, string loc)
        {
            Latitud = double.Parse(lat);
            Longitud = double.Parse(lon);
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