#region Usings

using System;

#endregion

namespace Urbetrack.Types.ValueObjects
{
    public enum GridAggregateType { Sum = 0, Count = 1, Avg = 2 } ;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ReportMappingAttribute:GridMappingAttribute
    {
        public bool IsAggregate { get; set; }
        public string AggregateTextFormat { get; set; }
        public int InitialGroupIndex { get; set; }
        public bool IsInitialGroup { get; set; }
        public GridAggregateType AggregateType { get; set; }
        public bool Visible { get; set; }

        public ReportMappingAttribute()
        {
            Visible = true;
            AggregateType = GridAggregateType.Sum;
            AllowMove = true;
            AllowGroup = true;
            AllowSizing = true;
            IsDataKey = false;
            IsAggregate = false;
            IsInitialGroup = false;
            InitialSortExpression = false;
            AggregateTextFormat = "{0:00}";
            InitialGroupIndex = -1;
            HeaderText = "";
        }
    }
}
