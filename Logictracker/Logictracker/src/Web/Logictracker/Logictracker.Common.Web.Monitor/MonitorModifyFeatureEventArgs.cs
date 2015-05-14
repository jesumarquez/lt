using System;
using System.Collections.Generic;
using System.Drawing;

namespace Logictracker.Web.Monitor
{
    public class MonitorModifyFeatureEventArgs:EventArgs
    {
        public static class Tipos
        {
            public const string Line = "LINE";
            public const string Polygon = "POLYGON";
        }
        public MonitorModifyFeatureEventArgs(string tipos, List<PointF> points)
        {
            Points = points;
            Tipo = Tipo;
        }

        public string Tipo { get; set; }
        public List<PointF> Points  { get; set; }
    }
}
