#region Usings

using System;

#endregion

namespace Logictracker.Types.ValueObjects
{
    public enum GridAggregateType { Sum = 0, Count = 1, Avg = 2, AvgNonZero = 3 } ;
    public enum GridSortDirection { Ascending = 0, Descending = 1 } ;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class GridMappingAttribute : Attribute
    {
        public int Index { get; set; }
        public GridSortDirection SortDirection { get; set; }
        public String DataFormatString { get; set; }
        public bool AllowGroup { get; set; }
        public bool AllowSizing { get; set; }
        public bool AllowMove { get; set; }
        public string HeaderText { get; set; }
        public string ResourceName { get; set; }
        public string VariableName { get; set; }
        public bool IsTemplate { get; set; }
        public string Width { get; set; }
        public bool InitialSortExpression { get; set; }
        public bool IncludeInSearch { get; set; }
        public bool IsDataKey { get; set; }

        public bool IsAggregate { get; set; }
        public string AggregateTextFormat { get; set; }
        public int InitialGroupIndex { get; set; }
        public bool IsInitialGroup { get; set; }
        public GridAggregateType AggregateType { get; set; }
        public bool Visible { get; set; }
        public string ExcelTextFormat { get; set; }
        public string ExcelTemplateName { get; set; }

        public GridMappingAttribute()
        {
            AllowMove = true;
            AllowGroup = true;
            AllowSizing = true;
            HeaderText = "";
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
            DataFormatString = "{0}";
            ExcelTextFormat = "{0}";
            ExcelTemplateName = null;
        }
    }
}
