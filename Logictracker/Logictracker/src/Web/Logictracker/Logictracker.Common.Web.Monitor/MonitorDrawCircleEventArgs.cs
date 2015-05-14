#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor
{
    public class MonitorDrawCircleEventArgs:EventArgs
    {
        public MonitorDrawCircleEventArgs(double lat, double lon, int radio)
        {
            Latitud = lat;
            Longitud = lon;
            Radio = radio;
        }

        public double Latitud { get; set; }

        public double Longitud { get; set; }

        public int Radio { get; set; }
    }
}
