namespace Logictracker.Web.Monitor.Base
{
    public class LatLon
    {
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public LatLon(double lat, double lon)
        {
            Latitud = lat;
            Longitud = lon;
        }
    }
}
