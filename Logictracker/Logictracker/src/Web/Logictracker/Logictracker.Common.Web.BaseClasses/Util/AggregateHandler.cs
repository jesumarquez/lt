using System;
using Logictracker.Types.ValueObjects;

namespace Logictracker.Web.BaseClasses.Util
{
    [Serializable]
    public class AggregateHandler
    {
        public string PropertyName { get; set; }
        public int OriginalColumnIndex { get; set; }
        public string AggregateTextFormat { get; set; }
        public GridAggregateType AggregateType { get; set; }
    }
}
