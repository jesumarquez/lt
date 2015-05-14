#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor
{
    public class MonitorClickEventArgs: EventArgs
    {
        public MonitorClickEventArgs(double lat, double lon)
        {
            Latitud = lat;
            Longitud = lon;
        }

        public double Latitud { get; set; }

        public double Longitud { get; set; }
    }
}