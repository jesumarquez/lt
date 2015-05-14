#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;

#endregion

namespace Logictracker.Web.Monitor
{
    public class MonitorDrawPolygonEventArgs: EventArgs
    {
        public MonitorDrawPolygonEventArgs(List<PointF> points)
        {
            Points = points;
        }

        public List<PointF> Points  { get; set; }

    }
}
