using System;
using System.Collections.Generic;
using System.Drawing;

namespace Logictracker.Web.Monitor
{
    public class MonitorDrawLineEventArgs:EventArgs
    {
        public MonitorDrawLineEventArgs(List<PointF> points)
        {
            Points = points;
        }

        public List<PointF> Points  { get; set; }
    }
}
