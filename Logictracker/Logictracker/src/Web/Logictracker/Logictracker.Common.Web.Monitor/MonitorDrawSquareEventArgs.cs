#region Usings

using System;
using Logictracker.Web.Monitor.Base;

#endregion

namespace Logictracker.Web.Monitor
{
    public class MonitorDrawSquareEventArgs: EventArgs
    {

        public MonitorDrawSquareEventArgs(Bounds bounds)
        {
            Bounds = bounds;
        }

        public Bounds Bounds { get; set; }

    }
}
