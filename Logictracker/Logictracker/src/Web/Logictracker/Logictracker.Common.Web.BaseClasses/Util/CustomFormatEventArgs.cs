using System;

namespace Logictracker.Web.BaseClasses.Util
{
    public class CustomFormatEventArgs: EventArgs
    {
        public Type Type { get; set; }
        public double Value { get; set; }
        public string FormattedString { get; set; }
        public AggregateHandler AggregateHandler { get; set; }
    }
}
