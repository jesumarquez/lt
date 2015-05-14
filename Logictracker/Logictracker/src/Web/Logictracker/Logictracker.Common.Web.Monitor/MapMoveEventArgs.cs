#region Usings

using System;
using Logictracker.Web.Monitor.Base;

#endregion

namespace Logictracker.Web.Monitor
{
    public class MapMoveEventArgs: EventArgs
    {
        public MapMoveEventArgs(int zoom, Bounds bounds)
        {
            Zoom = zoom;
            Bounds = bounds;
        }

        public int Zoom { get; set; }
        public Bounds Bounds { get; set; }
    }
}
