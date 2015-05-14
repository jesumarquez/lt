using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class Tickets_TicketTimeDifferences : SecuredGridReportPage<DataRowView>
    {
        #region Constants

        /// <summary>
        /// cantidad de columnas fijas que hay
        /// esto se usa porque antes de hacer el binding no se cuentan para el indice y durante y luego si lo hacen
        /// </summary>
        private const int DesplazamientoPorColumnasFijas = 1;

        /// <summary>
        /// how an empty column is represented while converting a DataTable into the source of a grid
        /// </summary>
        private const string EmptyString = "&nbsp;";

        private const int IndexMonitorLink = 0;
        private const int IndexFecha = 5;
        private const int IndexIdTicket = 6;
        private const int IndexIdTipoVehiculo = 7;
        private const int IndexIdVehiculo = 8;

        private const string InvalidDate = "01/01/1900 12:00:00 a.m.";
        private const string NotExists = "-N";
        //private const string RecorridoLargo = "ROUTE_TOO_LONG";
        //private const string SinRecorrido = "TICKET_ROUTE_NOT_FOUND";
        private const int VariableColumnsStartIndex = 9;
        private const string NoResultForCurrentFilters = "NO_RESULT_FOR_CURRENT_FILTERS";
        private int _variableColumnsLastIndex;

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "CLOG_REP_TICKET_TIEMPO"; } }

        protected override string GetRefference() { return "TIMEDIFF_TICKETS"; }

        /// <summary>
        /// Dictionary for saving the columns that represent an in-out of a geocerca
        /// </summary>
        private Dictionary<string,string> GeocercaColumns
        {
            get {return ViewState["geocercaColumns"] == null
                            ? new Dictionary<string, string>()
                            : (Dictionary<string, string>) ViewState["geocercaColumns"];
            }
            set { ViewState["geocercaColumns"] = value; }
        }

        /// <summary>
        /// Dictionary for saving the columns that represent an in-out of a geocerca
        /// </summary>
        private Dictionary<int, TimeSpan> Minimum
        {
            get
            {
                return ViewState["minimum"] == null
                           ? new Dictionary<int, TimeSpan>()
                           : (Dictionary<int, TimeSpan>)ViewState["minimum"];
            }
            set { ViewState["minimum"] = value; }
        }

        /// <summary>
        /// Dictionary for saving the columns that represent an in-out of a geocerca
        /// </summary>
        private Dictionary<int, TimeSpan> Maximum
        {
            get
            {
                return ViewState["maximum"] == null
                           ? new Dictionary<int, TimeSpan>()
                           : (Dictionary<int, TimeSpan>)ViewState["maximum"];
            }
            set { ViewState["maximum"] = value; }
        }

        /// <summary>
        /// Dictionary for saving the columns that represent an in-out of a geocerca
        /// </summary>
        private Dictionary<int, TimeSpan> Average
        {
            get
            {
                return ViewState["average"] == null
                           ? new Dictionary<int, TimeSpan>()
                           : (Dictionary<int, TimeSpan>)ViewState["average"];
            }
            set { ViewState["average"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Override for generating the grid using a DataSet (not the GetResult method) as DataSource.
        /// </summary>
        protected override void Bind()
        {
            if (lbMobiles.SelectedValues.Count.Equals(0))
            {
                LblInfo.Text = CultureManager.GetError("MUST_SELECT_MOBILE");
            
                return;
            }

            Grid.AllowColMoving = false;
            Grid.AllowGrouping = false;

            InitializeDictionaries();

            var src = ReportFactory.TicketReportDAO.GetTicketsTimeDifferencesByDateAndMobiles(lbMobiles.SelectedValues, dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(),
                                                                                              dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(), ddlPlanta.Selected);


            if(src.Tables.Count.Equals(0))
            {
                LblInfo.Text = CultureManager.GetSystemMessage(NoResultForCurrentFilters);
                LblInfo.Visible = true;

                return;
            }

            var source = src.Tables[0];

            GenerateGridWidth(source);    

            GenerateColumnsHeaders(source);

            Bind(source);

            HideIdColumns();

            FormatGridColumns();

            GenerateSubtotalsGrid();

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Grid.AllowSorting = false;
            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        /// <summary>
        /// Not used. Declared for compatibility with the base page.
        /// </summary>
        /// <returns></returns>
        protected override List<DataRowView> GetResults() { return new List<DataRowView>(); }

        /*/// <summary>
    /// Displays a popup with the givenn error message.
    /// </summary>
    /// <param name="error"></param>
    private void ShowErrorPopup(string error)
    {
        var errorMessage = string.Format("alert('{0}');", error);

        ScriptManager.RegisterStartupScript(this, typeof(string), "ErrorPopup", errorMessage, true);
    }//*/

        /// <summary>
        /// Adapts the values of the object for the need of the report.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="e"></param>
        /// <param name="dataItem"></param>
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, DataRowView dataItem)
        {
            e.Row.Cells[IndexFecha + DesplazamientoPorColumnasFijas].Text = String.Format("{0:dd/MM/yyyy hh:mm:ss}",
                                                                                          GetDateFromString(e.Row.Cells[IndexFecha + DesplazamientoPorColumnasFijas].Text));

            FormatGeocercaColumns(e);

            FormatHistoricalMonitorLink(e);

            FormatReportColumns(e);
        }

        protected override void CreateRowTemplate(C1GridViewRow row)
        {
            const int checkIndex = 0;

            var imgbtn = (row.Cells[checkIndex].FindControl("linkHistorico") as ImageButton);

            if (imgbtn == null) row.Cells[checkIndex].Controls.Add(new ImageButton { ID = "linkHistorico" });

            var lblId = (row.Cells[checkIndex].FindControl("lblLinkHistorico") as Label);

            if (lblId == null) row.Cells[checkIndex].Controls.Add(new Label { ID = "lblLinkHistorico", Visible = false });
        }

        /// <summary>
        /// Saves the link url into the AlternateText property of the ImageButton control in index[0] of the gridItem.
        /// </summary>
        /// <param name="e"></param>
        private void FormatHistoricalMonitorLink(C1GridViewRowEventArgs e)
        {
            var icon = e.Row.Cells[IndexMonitorLink].FindControl("linkHistorico") as ImageButton;
            var label = e.Row.Cells[IndexMonitorLink].FindControl("lblLinkHistorico") as Label;
        
            if (icon == null || label == null) return;

            icon.ImageUrl = String.Format("{0}images/centrar.bmp", ApplicationPath);

            var beginDate = GetFirstMobileMovementforTheTicket(e);
            var endDate = GetLastMobileMovementForTheTicket(e);

            if (beginDate.Equals(String.Empty) || endDate.Equals(String.Empty)) return;

            if(Convert.ToDateTime(endDate,CultureInfo.InvariantCulture).Subtract(Convert.ToDateTime(beginDate,CultureInfo.InvariantCulture)) > new TimeSpan(23,59,59))
            {
                icon.AlternateText = @"DAY_EXCEDED";
                return;
            }

            icon.AlternateText = string.Format(
                "../../Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={0}&TypeMobile={1}&Movil={2}&InitialDate={3}&FinalDate={4}&ShowMessages=0&ShowPOIS=0&Empresa={5}",
                ddlPlanta.SelectedValue,
                e.Row.Cells[IndexIdTipoVehiculo + DesplazamientoPorColumnasFijas].Text,
                e.Row.Cells[IndexIdVehiculo + DesplazamientoPorColumnasFijas].Text,
                beginDate,
                endDate,
                ddlLocacion.Selected
                );
        }

        private string GetFirstMobileMovementforTheTicket(C1GridViewRowEventArgs e)
        {
            for (var i = (VariableColumnsStartIndex + DesplazamientoPorColumnasFijas); i <= _variableColumnsLastIndex; i++ )
            {
                if(!(NoData(i,e) || NotExist(i,e) ||  NotValid(i,e))) return Convert.ToDateTime(e.Row.Cells[i].Text).ToString(CultureInfo.InvariantCulture);
            }

            return String.Empty;
        }

        private string GetLastMobileMovementForTheTicket(C1GridViewRowEventArgs e)
        {
            for (var i = _variableColumnsLastIndex; i > VariableColumnsStartIndex; i-- )
            {
                if(!(NoData(i,e) || NotExist(i,e) ||  NotValid(i,e)))
                {
                    return Convert.ToDateTime(e.Row.Cells[i].Text).
                        ToString(CultureInfo.InvariantCulture);
                }
            }
            return String.Empty;
        }

        protected override void SelectedIndexChanged()
        {
            Session["id"] = Grid.SelectedRow.Cells[IndexIdTicket + DesplazamientoPorColumnasFijas].Text;

            OpenWin("../../CicloLogistico/AltaTicket.aspx", CultureManager.GetMenu("ALTA_TICKET"));
        }

        /// <summary>
        /// Changes the visibility of the subtotals grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkSubtotals_chkChanged(object sender, EventArgs e) { ChangeSubtotalsVisibility(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the subtotals dictionaries.
        /// </summary>
        private void InitializeDictionaries()
        {
            Maximum = new Dictionary<int, TimeSpan>();
            Minimum = new Dictionary<int, TimeSpan>();
            Average = new Dictionary<int, TimeSpan>();
        }

        /// <summary>
        /// Generates the Headers of the grid grouping them by EstadoLogistico
        /// </summary>
        /// <param name="source"></param>
        private void GenerateColumnsHeaders(DataTable source)
        {
            var initialNumerOfColumns = source.Columns.Count;

            GenerateGeocercaColumns(source);

            GenerateReportColumns(source, initialNumerOfColumns);
        }

        /// <summary>
        /// Hides the Ticket ID Column.
        /// </summary>
        private void HideIdColumns()
        {
            if (Grid.Rows.Count == 0) return;

            Grid.Columns[IndexIdTicket + DesplazamientoPorColumnasFijas].Visible = false;
            Grid.Columns[IndexIdTipoVehiculo + DesplazamientoPorColumnasFijas].Visible = false;
            Grid.Columns[IndexIdVehiculo + DesplazamientoPorColumnasFijas].Visible = false;
        }

        /// <summary>
        /// Calculates and generates the columns of the in-out geocerca total time elapsed
        /// </summary>
        /// <param name="source"></param>
        private void GenerateGeocercaColumns(DataTable source)
        {
            GeocercaColumns = new Dictionary<string, string>();
            var numberOfQueryColumns = source.Columns.Count;

            for (var i = VariableColumnsStartIndex; i < numberOfQueryColumns; i++)/*searches for a geocercaColumn*/
                if (source.Columns[i].ColumnName.EndsWith("-G"))
                {
                    for (var j = i + 1; j < numberOfQueryColumns; j++)
                        if (source.Columns[j].ColumnName.EndsWith("-G")) /*searches for the next geocercaColumn and adds the combination to the dictionary*/
                        {
                            var columnName = String.Format("{0}-{1}", source.Columns[i].ColumnName.Replace("-G", ""),
                                                           source.Columns[j].ColumnName.Replace("-G", ""));                       
                            GeocercaColumns.Add(columnName, String.Format("{0}-{1}", j, i));                      
                            source.Columns.Add(columnName);
                            i = j;
                            break;
                        }
                }
        }

        /// <summary>
        /// Formats the columns returned by the sql query
        /// </summary>
        /// <param name="source"></param>
        /// <param name="initialNumerOfColumns"></param>
        private static void GenerateReportColumns(DataTable source, int initialNumerOfColumns)
        {
            for (var i = VariableColumnsStartIndex; i < initialNumerOfColumns - 1; i++)
                source.Columns[i].ColumnName = source.Columns[i].ColumnName.Replace("-G", "") + " - "
                                               + source.Columns[i + 1].ColumnName.Replace("-G", "");
            source.Columns[initialNumerOfColumns - 1].ColumnName = CultureManager.GetLabel("TOTAL");
        }

        /// <summary>
        /// Formats the data of the Geocerca Total Difference columns using the dictionary previously created
        /// </summary>
        /// <param name="e"></param>
        private void FormatGeocercaColumns(C1GridViewRowEventArgs e)
        {
            var initialIndex = Grid.Columns.Count - GeocercaColumns.Count;

            for (var i = initialIndex; i < Grid.Columns.Count; i++)
            {
                String str;
                int firstIndex;
                int secondIndex;

                GeocercaColumns.TryGetValue(Grid.Columns[i].HeaderText, out str);

                if (str.Equals(null)) return;

                int.TryParse(str.Remove(str.IndexOf('-')), out firstIndex); /*takes the first index of the string*/
                int.TryParse(str.Remove(0, str.IndexOf('-') + 1), out secondIndex); /*takes the second index*/

                var date1 = GetDateFromString(e.Row.Cells[firstIndex + DesplazamientoPorColumnasFijas].Text);
                var date2 = GetDateFromString(e.Row.Cells[secondIndex + DesplazamientoPorColumnasFijas].Text);

                if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
                {
                    e.Row.Cells[i].Text = String.Empty;
                    continue;
                }

                var dif = date1.Subtract(date2);

                e.Row.Cells[i].Text = FormatDifference(dif);

                CalculateSubtotals(dif,i,e);
            }
        }

        /// <summary>
        /// Formats a TimeSpan into a "Xhs Ymin" string.
        /// </summary>
        /// <param name="dif"></param>
        /// <returns></returns>
        private static string FormatDifference(TimeSpan dif)
        {
            if(dif.TotalHours >= 1)
            {
                return String.Format("{0}{1}h {2}min",(dif.Hours < 0 || dif.Minutes < 0) ? "-" : " ",dif.Hours.ToString().Replace("-", ""),dif.Minutes.ToString().Replace("-", ""));
            }
            return String.Format("{0}min",dif.Minutes);
        }

        /// <summary>
        /// Formats the C1Item with the data of the sql query.
        /// </summary>
        /// <param name="e"></param>
        private void FormatReportColumns(C1GridViewRowEventArgs e)
        {
            var totalTime = TimeSpan.Zero;
            var grdColumns = Grid.Columns.Count - GeocercaColumns.Count;

            for (var i = VariableColumnsStartIndex + DesplazamientoPorColumnasFijas; i < grdColumns - 1; i++)
            {
                if (NoData(i, e)) /*if there is no data for this Estado Logistico*/
                {
                    e.Row.Cells[i].Text = "";
                    continue;
                }

                if (NotExist(i, e)) /*if the ticket doesnt contain this EstadoLogistico*/
                {
                    e.Row.Cells[i].Text = "";
                    e.Row.Cells[i].BackColor = Color.LightGray;
                    continue;
                }

                if (NotValid(i, e)) /*if the data is not valid*/
                {
                    e.Row.Cells[i].Text = "";
                    continue;
                }

                if (NoData(i + 1, e) || NotExist(i + 1, e) || NotValid(i+1,e)) /*if next state is empty or invalid Date or it doesnt exist*/ totalTime += FindAndCalculateNextEstadoLogistico(i, e, grdColumns);
                else totalTime += CalculateDifference(i, i + 1, e);
            }

            e.Row.Cells[grdColumns - 1].Text = FormatDifference(totalTime);

            CalculateSubtotals(totalTime, grdColumns - 1,e);
        }

        /// <summary>
        /// Searches for the next Estado Available. If There is any returns the difference, else returns TimeSPan.Zero.
        /// </summary>
        /// <param name="begginingIndex"></param>
        /// <param name="e"></param>
        /// <param name="reportColumns"></param>
        /// <returns></returns>
        private TimeSpan FindAndCalculateNextEstadoLogistico(int begginingIndex, C1GridViewRowEventArgs e, int reportColumns)
        {
            for (var i = begginingIndex + 2; i < reportColumns; i++)
            {
                if ((NotExist(i, e) || NoData(i, e) || NotValid(i, e))) continue;

                e.Row.Cells[begginingIndex].BackColor = Color.LightCoral;

                return CalculateDifference(begginingIndex, i, e);
            }

            e.Row.Cells[begginingIndex].Text = "";

            return TimeSpan.Zero;
        }

        /// <summary>   
        /// Calculates And Formats a string with the difference between the 2 dates in the indexes.
        /// </summary>
        /// <param name="firstDateIndex"></param>
        /// <param name="secondDateIndex"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private TimeSpan CalculateDifference(int firstDateIndex, int secondDateIndex, C1GridViewRowEventArgs e)
        {
            var date1 = Convert.ToDateTime(e.Row.Cells[firstDateIndex].Text);
            var date2 = Convert.ToDateTime(e.Row.Cells[secondDateIndex].Text);
            var dif = date2.Subtract(date1);

            //Formats the data for the WebGrid
            e.Row.Cells[firstDateIndex].Text = FormatDifference(dif);

            CalculateSubtotals(dif, firstDateIndex,e);

            return dif;
        }

        /// <summary>
        /// Indicates if the EstadoLogistico Exists for that ticket.
        /// When a -N comes from the query it indicates not exists.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool NotExist(int i, C1GridViewRowEventArgs e) { return e.Row.Cells[i].Text.Contains(NotExists); }

        /// <summary>
        /// Indicates if the cell has data.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool NoData(int i, C1GridViewRowEventArgs e) { return e.Row.Cells[i].Text.Equals(EmptyString); }

        private static bool NotValid(int i, C1GridViewRowEventArgs e) { return e.Row.Cells[i].Text.Equals(InvalidDate); }

        /// <summary>
        /// Returns the string transformed in a Date time or DateTime.MinValue in case of error.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static DateTime GetDateFromString(String str)
        {
            return (str.Equals(EmptyString) || str.Equals("01/01/1900 12:00:00 a.m.")) ? DateTime.MinValue : Convert.ToDateTime(str).ToDisplayDateTime();
        }

        /// <summary>
        /// Formats the text to fit the columns for each EstadoLogistico
        /// </summary>
        private void FormatGridColumns()
        {
            for (var i = 0; i < Grid.Columns.Count; i++)
            {
                if (i >= VariableColumnsStartIndex && i < Grid.Columns.Count - 1)
                    Grid.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

                Grid.Columns[i].ItemStyle.BorderStyle = BorderStyle.Solid;
                Grid.Columns[i].ItemStyle.BorderWidth = 1;
                Grid.Columns[i].ItemStyle.BorderColor = Color.LightGray;
            }
        }

        /// <summary>
        /// Binds the grid with the source and sets the label or grid visibility according to the results set
        /// </summary>
        /// <param name="source"></param>
        private void Bind(DataTable source)
        {
            LblInfo.Text = CultureManager.GetSystemMessage(NoResultForCurrentFilters);
            LblInfo.Visible = source.Rows.Count.Equals(0);

            Grid.Columns.Clear();
            Grid.Columns.Add(new C1TemplateField());

            foreach (DataColumn column in source.Columns)
            {
                Grid.Columns.Add(new C1BoundField { HeaderText = column.ColumnName, DataField = column.ColumnName});
            }

            Grid.Visible = !source.Rows.Count.Equals(0);
            Grid.DataSource = source;
            Grid.DataBind();
        }

        /// <summary>
        /// Calculates and sets the Width needed for the grid
        /// </summary>
        /// <param name="source"></param>
        private void GenerateGridWidth(DataTable source)
        {
            Grid.Width = Unit.Pixel(850 + 240 * (source.Columns.Count - 3) % 3);

            grdSubtotals.Width = Unit.Pixel(850 + 240 * (source.Columns.Count - 3) % 3);

            _variableColumnsLastIndex = source.Columns.Count;
        }

        #endregion

        #region CSV Methods

        protected override void ExportToCsv()
        {
            var csv = new GridToCSVBuilder(Usuario.CsvSeparator);

            csv.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());

            var g = Grid;
            var allowPaging = Grid.AllowPaging;
            g.AllowPaging = false;

            Bind();

            csv.GenerateColumns(/*null,*/ Grid);

            csv.GenerateFields(Grid);

            if (chkSubtotals.Checked) GenerateCsvSubtotals(csv);

            SetCsvSessionVars(csv.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));

            g.AllowPaging = allowPaging;
        }

        /// <summary>
        /// Fills the report with the data of the subtotals grid.
        /// </summary>
        /// <param name="csv"></param>
        private void GenerateCsvSubtotals(BaseCsvBuilder csv)
        {
            var separator = Usuario.CsvSeparator;
            foreach(C1GridViewRow row in grdSubtotals.Rows)
            {
                var data = string.Concat(row.Cells[0].Text, separator, separator, separator, separator, separator, separator);

                for(var i = 1; i < grdSubtotals.Columns.Count; i++)
                    data = String.Concat(data, row.Cells[i].Text, separator);

                csv.GenerateRow(data.TrimEnd(','));
            }
            csv.GenerateRow("");

            csv.GenerateRow(String.Format(CultureManager.GetLabel("TICKETS_TOTALES") + ": {0}", Grid.Rows.Count));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        #endregion

        #region SubTotals Generation

        /// <summary>
        /// Generates the subtotals grid.
        /// </summary>
        private void GenerateSubtotalsGrid()
        {
            if (!Grid.Visible) return;

            var data = new DataSet();
            var table = new DataTable();
            data.Tables.Add(table);

            GenerateSubtotalsColumns(table);

            FormatSubtotalRow(table, Maximum, CultureManager.GetLabel("MAXIMO"));
            FormatSubtotalRow(table, Minimum, CultureManager.GetLabel("MINIMO"));
            FormatSubtotalRow(table, Average, CultureManager.GetLabel("PROMEDIO"));

            data.AcceptChanges();

            grdSubtotals.AutoGenerateColumns = true;
            grdSubtotals.DataSource = data.Tables[0];
            grdSubtotals.DataBind();

            ChangeSubtotalsVisibility();
        }

        /// <summary>
        /// Changes visibilitiy according to data and chkSubtotals. Also formats the total tickets label.
        /// </summary>
        private void ChangeSubtotalsVisibility()
        {
            lblTotales.Visible = grdSubtotals.Visible = Grid.Visible && chkSubtotals.Checked;
            lblTotales.Text = String.Format(CultureManager.GetLabel("TICKETS_TOTALES") + ": {0}", Grid.Rows.Count);
        }

        private void GenerateSubtotalsColumns(DataTable table)
        {
            table.Columns.Add("Categoria");
            //table.Columns.Add("Total Tickets");
            for (var i = VariableColumnsStartIndex; i < Grid.Columns.Count; i++)
                table.Columns.Add(Grid.Columns[i].HeaderText);
        }

        /// <summary>
        /// Creates the rows for each dictionary. TimeSpan.Zero is the null value so its replaced with an empty string*/
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dictionary"></param>
        /// <param name="rowName"></param>
        private static void FormatSubtotalRow(DataTable table, IDictionary<int, TimeSpan> dictionary, String rowName)
        {
            var r = table.NewRow();
            r.SetField(0, rowName);
            for (var j = 1 ; j < table.Columns.Count; j++)
            {
                TimeSpan Value;
                dictionary.TryGetValue(VariableColumnsStartIndex + j -1, out Value);
                r.SetField(j,FormatDifference(Value));
            }
            table.Rows.Add(r);
        }

        /// <summary>
        /// Calculates the subtotals to be used in the Subtotals grid.
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="index"></param>
        /// <param name="e"></param>
        private void CalculateSubtotals(TimeSpan dif, int index, C1GridViewRowEventArgs e)
        {
            RefreshMaximum(dif, index);
            RefreshMinimum(dif, index);
            RefreshAverage(dif, index,e);
        }

        /// <summary>
        /// Keeps always the maximum value of each column.
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="index"></param>
        private void RefreshMaximum(TimeSpan dif, int index)
        {
            TimeSpan max;
            Maximum.TryGetValue(index, out max);
            Maximum.Remove(index);
            Maximum.Add(index, dif.CompareTo(max) > 0 ? dif : max);
        }

        /// <summary>
        /// Keeps always the minimum value of each column.
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="index"></param>
        private void RefreshMinimum(TimeSpan dif, int index)
        {
            TimeSpan min;
            Minimum.TryGetValue(index, out min);
            Minimum.Remove(index);
            Minimum.Add(index, !min.Equals(TimeSpan.Zero) ? dif.CompareTo(min) < 0 ? dif : min
                                   : dif);
        }

        /// <summary>
        /// Accumulates the average of each column. 
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="index"></param>
        /// <param name="e"></param>
        private void RefreshAverage(TimeSpan dif, int index, C1GridViewRowEventArgs e)
        {
            TimeSpan avg;
            Average.TryGetValue(index, out avg);
            Average.Remove(index);
            Average.Add(index, TimeSpan.FromMinutes(CalculateAverage(avg, e, dif)));
        }

        /// <summary>
        /// Calculates the average of the column.
        /// </summary>
        /// <param name="avg"></param>
        /// <param name="e"></param>
        /// <param name="dif"></param>
        /// <returns></returns>
        private static double CalculateAverage(TimeSpan avg, C1GridViewRowEventArgs e, TimeSpan dif)
        {
            return (avg.TotalMinutes * e.Row.RowIndex + dif.TotalMinutes) / (e.Row.RowIndex + 1);
        }

        #endregion
    }
}

