#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    public class DetentionTimes
    {
        public DateTime Fecha { get; set; }
        public double HsTurnoOn { get; set; }
        public double HsTurnoOff { get; set; }
        public double HsOn { get; set; }
        public double HsOff { get; set; }
    }
}
