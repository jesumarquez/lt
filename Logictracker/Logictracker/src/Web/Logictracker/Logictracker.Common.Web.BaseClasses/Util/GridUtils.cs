using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Types.ValueObjects;
using Logictracker.Utils;
using Logictracker.Web.Helpers.C1Helpers;

namespace Logictracker.Web.BaseClasses.Util
{
    public class GridUtils<T>
    {
        public event EventHandler GenerateCustomColumns;
        public event EventHandler<C1GridViewRowEventArgs> CreateRowTemplate;
        public event EventHandler<RowEventArgs<T>> RowDataBound;
        public event EventHandler SelectedIndexChanged;
        public event EventHandler Binding;         
        public event EventHandler<C1GridViewCommandEventArgs> RowCommand;
        public event EventHandler<CustomFormatEventArgs> AggregateCustomFormat;

        private readonly C1GridView Grid;
        private readonly IGridded<T> Page;
        public bool AnyIncludedInSearch { get; private set; }
        private bool RealizarBusqueda { get; set; }
        public bool CustomPagination = false;
        private bool CalculateAggragateSums { get { return Grid.GroupedColumns.Count > 0 && RealizarBusqueda; } }
        
        public GridUtils(C1GridView grid, IGridded<T> page)
        {
            Grid = grid;
            Page = page;

            Grid.Sorting += Grid_Sorting;
            Grid.ColumnMoving += Grid_ColumnMoving;
            Grid.ColumnMoved += Grid_ColumnMoved;
            Grid.PageIndexChanging += Grid_PageIndexChanging;
            Grid.GroupColumnMoving += Grid_GroupColumnMoving;
            Grid.GroupColumnMoved += Grid_GroupColumnMoved;
            Grid.GroupAggregate += Grid_GroupAggregate;
            Grid.RowDataBound += Grid_RowDataBound;
            Grid.SelectedIndexChanging += Grid_SelectedIndexChanging;
            Grid.RowCommand += Grid_RowCommand;

            GenerateDataKeyNames();

            Grid.PageSize = Page.PageSize;
            Grid.EmptyDataText = CultureManager.GetError("NO_MATCHES_FOUND");
            Grid.GroupByCaption = CultureManager.GetControl("GRID_GROUPBY_CAPTION");
        }

        public void Reset()
        {
            Grid.Columns.Clear();
            Grid.GroupedColumns.Clear();
            Aggregates = null;
            Subtotals = null;
        }

        #region Events

        void Grid_RowCommand(object sender, C1GridViewCommandEventArgs e)
        {
            if (RowCommand != null) RowCommand(Grid, e);
        }

        void Grid_SelectedIndexChanging(object sender, C1GridViewSelectEventArgs e)
        {
            Grid.SelectedIndex = e.NewSelectedIndex;
            if (SelectedIndexChanged != null) SelectedIndexChanged(Grid, EventArgs.Empty);
        }

        internal void Grid_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;
            var page = (Page as System.Web.UI.Control).Page;
            
            if (Page.SelectableRows) e.Row.Attributes.Add("onclick", page.ClientScript.GetPostBackEventReference(Grid, string.Format("Select:{0}", e.Row.RowIndex)));

            if (Page.MouseOverRowEffect)
            {
                e.Row.Attributes.Add("onmouseover", "this.className = 'Grid_MouseOver_Item';");
                e.Row.Attributes.Add("onmouseout", string.Format("this.className = '{0}';", e.Row.RowIndex % 2 == 0 ? "Grid_Item" : "Grid_Alternating_Item"));
            }

            if(CreateRowTemplate != null) CreateRowTemplate(Grid, e);
            if (RowDataBound != null) RowDataBound(Grid, new RowEventArgs<T>(Grid, (T)e.Row.DataItem, e));
        }

        void Grid_Sorting(object sender, C1GridViewSortEventArgs e)
        {
            SortExpression = e.SortExpression;
            SortDirection = e.SortDirection;

            Bind();
        }

        void Grid_ColumnMoving(object sender, C1GridViewColumnMoveEventArgs e)
        {
            UpdateColumnIndex(e.Column, e.DestIndex);
        }

        void Grid_ColumnMoved(object sender, C1GridViewColumnMovedEventArgs e)
        {
            Bind();
        }
        
        void Grid_PageIndexChanging(object sender, C1GridViewPageEventArgs e)
        {
            Grid.PageIndex = e.NewPageIndex;
            Bind();
        }

        void Grid_GroupColumnMoved(object sender, C1GridViewGroupColumnMovedEventArgs e)
        {
            /*Subtotals = null;
            var newIndex = GetColumnIndex(e.Column);
            if (_tempGroupedColumns < Grid.GroupedColumns.Count)
            {
                newIndex = e.DestIndex;
                var col = Grid.Columns[newIndex];
                Grid.Columns[newIndex] = Grid.Columns[_tempColumnIndex];

                for (var i = newIndex + 1; i <= _tempColumnIndex; i++)
                {
                    var colAux = Grid.Columns[i];
                    Grid.Columns[i] = col;
                    col = colAux;
                }
            }
            UpdateColumnIndex(_tempColumnIndex, newIndex);
            */
            GroupColumn(e.Column, _tempColumnIndex, e.DestIndex, _tempGroupedColumns);
            Bind();
        }

        private int _tempColumnIndex = -1;
        private int _tempGroupedColumns;
        
        void Grid_GroupColumnMoving(object sender, C1GridViewGroupColumnMoveEventArgs e)
        {
            _tempColumnIndex = GetColumnIndex(e.Column);
            _tempGroupedColumns = Grid.GroupedColumns.Count;
        }

        void Grid_GroupAggregate(object sender, C1GroupTextEventArgs e)
        {
            GroupAggregate(e);
        } 

        private void GroupColumn(C1BaseField column, int fromIndex, int destIndex, int groupedColumnsCount)
        {
            Subtotals = null;
            //if(column as C1Field != null) (column as C1Field).GroupInfo.OutlineMode = Page.GridOutlineMode;
            var newIndex = GetColumnIndex(column);
            if (groupedColumnsCount < Grid.GroupedColumns.Count)
            {
                newIndex = destIndex;
                var col = Grid.Columns[newIndex];
                Grid.Columns[newIndex] = Grid.Columns[fromIndex];

                for (var i = newIndex + 1; i <= fromIndex; i++)
                {
                    var colAux = Grid.Columns[i];
                    Grid.Columns[i] = col;
                    col = colAux;
                }
            }
            UpdateColumnIndex(fromIndex, newIndex);
        }
        
        #endregion

        #region Sorting

        /// <summary>
        /// Dictionary for column specific IComparer sorting.
        /// </summary>
        private readonly IDictionary<string, ICustomComparer<T>> _sortDictionary = new Dictionary<string, ICustomComparer<T>>();

        /// <summary>
        /// The current sort expression.
        /// </summary>
        private string SortExpression
        {
            get { return Page.StateBag["SortExpression"] != null ? Page.StateBag["SortExpression"].ToString() : InitialSortExpression; }
            set { Page.StateBag["SortExpression"] = value; }
        }

        /// <summary>
        /// Initial sort expression.
        /// </summary>
        private string InitialSortExpression { get; set; }

        /// <summary>
        /// The current sort direction.
        /// </summary>
        private C1SortDirection SortDirection
        {
            get { return Page.StateBag["SortDirection"] != null ? (C1SortDirection)Page.StateBag["SortDirection"] : C1SortDirection.Ascending; }
            set { Page.StateBag["SortDirection"] = value; }
        }

        /// <summary>
        /// Adds a specific comparer for the givenn sort expression.
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="comparer"></param>
        public void AddCustomSort(string sortExpression, ICustomComparer<T> comparer)
        {
            if (_sortDictionary.ContainsKey(sortExpression)) _sortDictionary[sortExpression] = comparer;
            else _sortDictionary.Add(sortExpression, comparer);
        }

        private void Sort(List<T> data)
        {
            if (!string.IsNullOrEmpty(SortExpression) || Grid.GroupedColumns.Count > 0)
                data.Sort(GetSortComparer());
        }
        
        /// <summary>
        /// Gets the sort comparer to use for the current sort expression.
        /// </summary>
        /// <returns></returns>
        private IComparer<T> GetSortComparer()
        {
            if (Grid.GroupedColumns.Count > 0)
            {
                var comp = new MultiCriteriaObjectComparer<T>();
                foreach (C1Field field in Grid.GroupedColumns)
                    comp.AddSortExpression(field.SortExpression, field.SortDirection == C1SortDirection.Descending);
                
                if (!string.IsNullOrEmpty(SortExpression))
                    comp.AddSortExpression(SortExpression, SortDirection == C1SortDirection.Descending);
                return comp;
            }

            if (_sortDictionary.ContainsKey(SortExpression))
            {
                var comp = _sortDictionary[SortExpression];

                comp.Descending = SortDirection == C1SortDirection.Descending;

                return comp;
            }

            return new ObjectComparer<T>(SortExpression, SortDirection == C1SortDirection.Descending);
        }

        #endregion

        #region Aggregates

        /// <summary>
        /// SubTotals collection
        /// </summary>
        private Dictionary<string, ArrayList> Subtotals
        {
            get { return (Page.StateBag["Subtotals"] as Dictionary<string, ArrayList>) ?? new Dictionary<string, ArrayList>(); }
            set { Page.StateBag["Subtotals"] = value; }
        }

        /// <summary>
        /// The current sort expression.
        /// </summary>
        private List<AggregateHandler> Aggregates
        {
            get { return (Page.StateBag["Aggregates"]  as List<AggregateHandler>) ?? new List<AggregateHandler>(); }
            set { Page.StateBag["Aggregates"] = value; }
        }

        /// <summary>
        /// Adds and Aggregate field that will be processed.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="originalColumnIndex"></param>
        /// <param name="aggregateTextFormat"></param>
        /// <param name="aggregateType"></param>
        public void AddAggregate(string propertyName, int originalColumnIndex, string aggregateTextFormat, GridAggregateType aggregateType)
        {
            var a = Aggregates;
            a.Add(new AggregateHandler
            {
                PropertyName = propertyName,
                AggregateTextFormat = aggregateTextFormat,
                AggregateType = aggregateType,
                OriginalColumnIndex = originalColumnIndex
            });
            Aggregates = a;
        }

        private ArrayList CalcularSubTotales(List<T> list, string index)
        {
            var key = index;
            var totals = Subtotals;

            if (!Subtotals.ContainsKey(key))
            {
                var sums = new double[Aggregates.Count];

                MakeAggregate(sums, list);

                totals.Add(key, new ArrayList(sums));
            }

            Subtotals = totals;
            return Subtotals[key];
        }

        /// <summary>
        /// Makes the aggregate of the property. Defauls is SUM.
        /// </summary>
        /// <returns></returns>
        private void MakeAggregate(double[] array, List<T> list)
        {
            for (var j = 0; j < Aggregates.Count; j++)
            {
                var aggregate = Aggregates.ElementAt(j);
                var aggregateType = aggregate.AggregateType;

                if (aggregateType.Equals(GridAggregateType.Count))
                {
                    array[j] = list.ToList().Count;
                    continue;
                }

                if (list.Count > 0 && (aggregateType.Equals(GridAggregateType.Sum) || aggregateType.Equals(GridAggregateType.Avg) || aggregateType.Equals(GridAggregateType.AvgNonZero)))
                {
                    var firstItem = list.Select(l=>l.GetReflectedValue(aggregate.PropertyName)).Where(l=>l!= null).FirstOrDefault();

                    if (firstItem.IsNumeric())
                    {
                        var group = list.Where(o => o.GetReflectedValue(aggregate.PropertyName).IsNumeric());

                        if (aggregateType.Equals(GridAggregateType.Sum))
                        {
                            array[j] = group.Sum(o => Convert.ToDouble(o.GetReflectedValue(aggregate.PropertyName)));
                        }
                        else if (aggregateType.Equals(GridAggregateType.Avg))
                        {
                            array[j] = group.Average(o => Convert.ToDouble(o.GetReflectedValue(aggregate.PropertyName)));
                        }
                        else if (aggregateType.Equals(GridAggregateType.AvgNonZero))
                        {
                            array[j] = group
                                .Where(x => Convert.ToDouble(x.GetReflectedValue(aggregate.PropertyName)) > 0)
                                .Average(o => Convert.ToDouble(o.GetReflectedValue(aggregate.PropertyName)));
                        }
                    }
                    else if (firstItem != null && firstItem.GetType().Equals(typeof(TimeSpan)))
                    {
                        //La sumatoria es de time spans

                        var group = from o in list
                                    let val = o.GetReflectedValue(aggregate.PropertyName)
                                    where val != null && (TimeSpan)val != default(TimeSpan)
                                    select (TimeSpan)val;

                        var timeSpan = TimeSpan.Zero;
                        timeSpan = group.Aggregate(timeSpan, (current, item) => current.Add(item));

                        if (aggregateType.Equals(GridAggregateType.Sum))
                        {
                            array[j] = timeSpan.TotalMinutes;
                        }
                        else if (aggregateType.Equals(GridAggregateType.Avg))
                        {
                            array[j] = timeSpan.TotalMinutes != 0 ? timeSpan.TotalMinutes / group.Count() : timeSpan.TotalMinutes;
                        }
                        else if (aggregateType.Equals(GridAggregateType.AvgNonZero))
                        {
                            var count = group.Count(x => x.TotalMinutes > 0);
                            array[j] = timeSpan.TotalMinutes != 0 && count > 0 ? timeSpan.TotalMinutes / count : 0;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Formats group header information.
        /// </summary>
        /// <param name="e"></param>
        private void GroupAggregate(C1GroupTextEventArgs e)
        {
            if (CalculateAggragateSums)
            {
                var groupColumnIndex = Grid.GroupedColumns.IndexOf(e.GroupCol);
                var listIndex = (Grid.PageIndex*Grid.PageSize) + e.StartItemIndex;
                var refValue = Page.Data[listIndex];
                var list = Page.Data.ToList();
                //var key = groupColumnIndex + listIndex;
                var key = string.Empty;

                for (var j = 0; j <= groupColumnIndex; j++)
                {
                    var col = Grid.GroupedColumns[j] as C1BoundField;
                    string dataField;
                    if (col != null)
                    {
                        dataField = col.DataField;
                    }
                    else
                    {
                        var temp = Grid.GroupedColumns[j] as C1TemplateField;
                        if (temp == null) continue;
                        var originalIndex = GetOriginalColumnIndex(Grid.Columns.IndexOf(Grid.GroupedColumns[j]));

                        var properties = from p in typeof (T).GetProperties()
                                         let atts = p.GetGridMappingAttributes()
                                         where atts.Count() > 0
                                               && atts.First().IsTemplate
                                               && atts.First().Index == originalIndex
                                         select p.Name;

                        if (properties.Count() == 0) continue;

                        dataField = properties.First();
                    }

                    key += string.Concat("[",refValue.GetReflectedValue(dataField).ToString(),"]");

                    list = (from o in list
                            let data = o.GetReflectedValue(dataField)
                            where data != null && data.Equals(refValue.GetReflectedValue(dataField))
                            select o).ToList();
                }

                var subtotales = CalcularSubTotales(list, key);

                if (subtotales.Count >= Aggregates.Count)
                {
                    for (var i = 0; i < Aggregates.Count; i++)
                    {
                        var aggregate = Aggregates[i];
                        var aggregateIndex = aggregate.OriginalColumnIndex;
                        var columnIndex = GetOriginalColumnIndex(GetColumnIndex(e.Col));

                        if (aggregateIndex.Equals(columnIndex))
                            e.Text = GetFormatedTotalString((double) subtotales[i], aggregate, list);
                    }
                }
            }
        }

        public void Totalizar()
        {
            if (Page.HasTotalRow)
            {
                var list = Page.Data.ToList();
                var sums = new double[Aggregates.Count];
                MakeAggregate(sums, list);
                
                Grid.Columns[0].FooterText = "TOTAL GENERAL";
                Grid.FooterStyle.CssClass = "C1GroupFooter";

                for (var i = 0; i < Aggregates.Count; i++)
                {
                    var aggregate = Aggregates[i];
                    var aggregateIndex = GetColumnIndex(aggregate.OriginalColumnIndex);

                    Grid.Columns[aggregateIndex].FooterText = GetFormatedTotalString(sums[i], aggregate, list);
                }
            }
        }

        private string GetFormatedTotalString(double value, AggregateHandler aggregate, IEnumerable<T> list)
        {
            var type = (from t in list
                        where t.GetReflectedValue(aggregate.PropertyName) != null
                        select t.GetReflectedValue(aggregate.PropertyName).GetType()).FirstOrDefault();

            if (aggregate.AggregateTextFormat.ToLower() == "custom")
            {
                if (AggregateCustomFormat != null)
                {
                    var eventArgs = new CustomFormatEventArgs { Type = type, Value = value, AggregateHandler = aggregate };
                    AggregateCustomFormat(Grid, eventArgs);
                    return eventArgs.FormattedString;
                }
            }
            if (type != null && type == typeof(TimeSpan))
            {
                try
                {
                    return String.Format(aggregate.AggregateTextFormat, TimeSpan.FromMinutes(value)).Substring(0,8);
                }
                catch
                {
                    return string.Empty;
                }
            }
            
            return String.Format(aggregate.AggregateTextFormat, value);
        }


        #endregion

        #region Columns

        private void GenerateDataKeyNames()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => (p.GetGridMappingAttributes().Count() > 0
                    && p.GetGridMappingAttributes().First().IsDataKey)
                    || p.Name == "Id")
                    .OrderBy(p => p.Name == "Id" ? -1 : p.GetGridMappingAttributes().First().Index);

            Grid.DataKeyNames = properties.Select(property => property.Name).ToArray();
        }

        public void GenerateColumns()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetGridMappingAttributes().Count() > 0)
                .OrderBy(p => (p.GetGridMappingAttributes().First()).Index);

            var showGrouping = false;
            var showSearch = false;
            var groupedColumns = new List<C1Field>();

            foreach (var property in properties)
            {
                var attribute = property.GetGridMappingAttributes().First();

                if (attribute.InitialSortExpression)
                {
                    SortExpression = InitialSortExpression = property.Name;
                    SortDirection = attribute.SortDirection.Equals(GridSortDirection.Descending) ? C1SortDirection.Descending : C1SortDirection.Ascending;
                }

                if (attribute.IsAggregate) AddAggregate(property.Name, attribute.Index, attribute.AggregateTextFormat, attribute.AggregateType);

                if (attribute.IsDataKey) continue;

                C1Field field;
                if (attribute.IsTemplate)
                {
                    field = new C1ResourceTemplateColumn
                    {
                        SortExpression = property.Name,
                        SortDirection = attribute.InitialSortExpression ? attribute.SortDirection.Equals(GridSortDirection.Descending) ? C1SortDirection.Descending : C1SortDirection.Ascending : C1SortDirection.NotSet,
                        AllowGroup = attribute.AllowGroup,
                        AllowMove = attribute.AllowMove,
                        AllowSizing = attribute.AllowSizing,
                        HeaderText = attribute.HeaderText,
                        ResourceName = attribute.ResourceName,
                        VariableName = attribute.VariableName,
                        Visible = attribute.Visible
                    };
                }
                else
                {
                    field = new C1ResourceBoundColumn
                    {
                        DataField = property.Name,
                        ResourceName = attribute.ResourceName,
                        VariableName = attribute.VariableName,
                        SortExpression = property.Name,
                        SortDirection = attribute.InitialSortExpression ? attribute.SortDirection.Equals(GridSortDirection.Descending) ? C1SortDirection.Descending : C1SortDirection.Ascending : C1SortDirection.NotSet,
                        AllowGroup = attribute.AllowGroup,
                        AllowMove = attribute.AllowMove,
                        AllowSizing = attribute.AllowSizing,
                        HeaderText = attribute.HeaderText,
                        DataFormatString = attribute.DataFormatString,
                        Aggregate = attribute.IsAggregate ? AggregateEnum.Custom : AggregateEnum.None,
                        Visible = attribute.Visible
                    };
                    if (!String.IsNullOrEmpty(attribute.DataFormatString)) (field as C1ResourceBoundColumn).DataFormatString = attribute.DataFormatString;
                }              

                if (!string.IsNullOrEmpty(attribute.Width))
                    field.ItemStyle.Width = new Unit(attribute.Width);

                Grid.Columns.Add(field);

                if (attribute.AllowGroup) showGrouping = true;
                if (attribute.IncludeInSearch) showSearch = true;
                if (attribute.IsInitialGroup)
                {
                    groupedColumns.Add(field);
                }
            }

            if(GenerateCustomColumns != null) GenerateCustomColumns(Grid, EventArgs.Empty);

            GenerateColumnIndices();

            AnyIncludedInSearch = showSearch;

            if (!showGrouping) Grid.AllowGrouping = false;

            Grid.ShowFooter = Page.HasTotalRow;

            foreach (var field in groupedColumns)
            {
                GroupColumn(field, GetColumnIndex(field), Grid.GroupedColumns.Count, Grid.GroupedColumns.Count);
                Grid.GroupedColumns.Add(field);
            }
        }

        public void GenerateColumnIndices()
        {
            ColumnIndices = Grid.Columns.Cast<object>().Select((t, i) => i).ToList();
        }

        public void CopyColumns(C1GridView from)
        {
            foreach (var column in from.Columns) Grid.Columns.Add(column as C1BaseField);

            foreach (var column in from.GroupedColumns) Grid.GroupedColumns.Add(column as C1Field);
        }
        
        #region Index Handling

        private List<int> ColumnIndices
        {
            get { return Page.StateBag["ColumnIndices"] as List<int> ?? new List<int>(); }
            set { Page.StateBag["ColumnIndices"] = value; }
        }
        
        /// <summary>
        /// Dictionary that contains the actual column index in the [originalIndex] position.
        /// </summary>
        private Dictionary<int, int> ActualIndexes
        {
            get { return Page.StateBag["ActualIndexes"] as Dictionary<int, int> ?? new Dictionary<int, int>(); }
            set { Page.StateBag["ActualIndexes"] = value; }
        }

        private void UpdateColumnIndex(int lastIndex, int newIndex)
        {
            var colIndices = ColumnIndices;

            var movedVal = colIndices[lastIndex];
            colIndices.RemoveAt(lastIndex);
            colIndices.Insert(newIndex, movedVal);
            ColumnIndices = colIndices;
        }

        private void UpdateColumnIndex(C1BaseField column, int index)
        {
            var colIndices = ColumnIndices;
            var lastIndex = GetColumnIndex(column);

            var movedVal = colIndices[lastIndex];
            colIndices.RemoveAt(lastIndex);
            colIndices.Insert(index, movedVal);
            ColumnIndices = colIndices;
        }

        /// <summary>
        /// Updates the Columns indexes of the grid.
        /// </summary>
        private void UpdateActualIndexes()
        {
            var list = new Dictionary<int, int>(ColumnIndices.Count);

            for (var i = 0; i < ColumnIndices.Count; i++) list.Add(ColumnIndices[i], i);

            ActualIndexes = list;
        }

        /// <summary>
        /// Gets the new Index of the original column index.
        /// </summary>
        /// <param name="originalIndex"></param>
        /// <returns></returns>
        public int GetColumnIndex(int originalIndex)
        {
            return ActualIndexes[originalIndex];
        }

        private int GetColumnIndex(C1BaseField column)
        {
            for (var i = 0; i < Grid.Columns.Count; i++)
                if (Grid.Columns[i] == column) return i;
            return -1;
        }
       
        /// <summary>
        /// Gets the original column index of the grid.
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <returns></returns>
        private int GetOriginalColumnIndex(int currentIndex)
        {
            return ColumnIndices[currentIndex];
        }

        public TableCell GetCell(C1GridViewRow row, int originalIndex)
        {
            return row.Cells[GetColumnIndex(originalIndex)];
        }
        
        public C1BaseField GetColumn(int originalIndex)
        {
            return Grid.Columns[GetColumnIndex(originalIndex)];
        }
        
        #endregion

        #endregion

        #region Bind
        
        public void Bind()
        {
            if(Binding != null) Binding(Grid, EventArgs.Empty);

            //Generates columns if the grid has been hidden.
            if (Grid.Columns.Count.Equals(0)) GenerateColumns();
            
            RealizarBusqueda = true;

            //Updates the original index dictionary.
            UpdateActualIndexes();

            var list = Search(Page.Data, Page.SearchString);

            if (!CustomPagination)
            { 
                if (Grid.PageIndex > Math.Floor((double)list.Count / Grid.PageSize))
                    Grid.PageIndex = 0;
            }

            Sort(list);

            Totalizar();

            if (Page.GridOutlineMode == OutlineMode.StartCollapsed)
            {
                foreach (C1Field field in Grid.Columns)
                {
                    field.GroupInfo.OutlineMode = OutlineMode.StartExpanded;
                }
                foreach (C1Field field in Grid.GroupedColumns)
                {
                    field.GroupInfo.OutlineMode = Page.GridOutlineMode;
                }
            }

            Grid.DataSource = list;
            Grid.DataBind();

            ShowTotalCount(list.Count);
        }

        private void ShowTotalCount(int count)
        {
            var text = string.Format(CultureManager.GetControl("GRID_CANT_RESULTADOS"), count);
            if (Grid.AllowCustomPaging && Grid.VirtualItemCount > 0)
            {
                text = text + " de un total de " + Grid.VirtualItemCount;
            }
            var litTop = new Label { Text = text };
            litTop.Style.Add("float", "left");
            var litBottom = new Label { Text = text };
            litBottom.Style.Add("float", "left");
            if(Grid.TopPagerRow != null) Grid.TopPagerRow.Cells[0].Controls.AddAt(0, litTop);
            if (Grid.BottomPagerRow != null) Grid.BottomPagerRow.Cells[0].Controls.AddAt(0, litBottom);
        }

        #endregion

        #region Search
        
        public List<T> Search(List<T> data, string search)
        {
            if (string.IsNullOrEmpty(search)) return data;
			search = StringUtils.RemoverAcentos(search);

            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(GridMappingAttribute), false).Count() > 0)
                .OrderBy(p => (p.GetCustomAttributes(typeof(GridMappingAttribute), false)[0] as GridMappingAttribute).Index);

            var searchProperties = (from property in properties
                                    let attribute = property.GetCustomAttributes(typeof(GridMappingAttribute), false).OfType<GridMappingAttribute>().First()
                                    where attribute.IncludeInSearch
                                    select property).ToList();

            var filtered = new List<T>();
            foreach (var searchProperty in searchProperties)
            {
                filtered.AddRange(
                    (from f in data
                     let valor = searchProperty.GetValue(f, null)
					 where valor != null && StringUtils.RemoverAcentos(valor.ToString()).Contains(search) && !filtered.Contains(f)
                     select f).ToList());
            }
            return Page.Data = filtered;
        } 
        
        #endregion
    }
    public static class GridUtils
    {
        public static void MakeRowSelectable(this C1GridViewRow row)
        {
            var grid = row.BindingContainer as C1GridView;
            if (grid == null) return;
            var page = grid.Page;
            row.Attributes.Add("onclick", page.ClientScript.GetPostBackEventReference(grid, string.Format("Select:{0}", row.RowIndex)));
        }
        public static void AddHoverEffect(this C1GridViewRow row, string cssClass, string altCssClass, string hoverCssClass)
        {
            row.Attributes.Add("onmouseover", string.Format("this.className = '{0}';", hoverCssClass));
            row.Attributes.Add("onmouseout", string.Format("this.className = '{0}';", row.RowIndex % 2 == 0 ? cssClass : altCssClass));
        }
        public static T GetControl<T>(this C1GridViewRow row, string controlId) where T:Control
        {
            return row.FindControl(controlId) as T;
        }
    }
}
