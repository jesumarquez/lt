#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    public class StoppedHours
    {
        public double HoursInShift { get; set; }
        public double HoursOutOfShift { get; set; }
        public DateTime Date { get; set; }
    }
}
