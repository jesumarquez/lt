#region Usings

using System;

#endregion

namespace Logictracker.Web.Monitor.Base
{
    [Serializable]
    public class Bounds
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public Bounds()
        {}
        public Bounds(double top, double bottom, double left, double right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }
    }
}
