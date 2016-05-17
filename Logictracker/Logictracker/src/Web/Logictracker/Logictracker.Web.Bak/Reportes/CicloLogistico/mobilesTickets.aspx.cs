using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class Documentos_mobilesTickets : SecuredGridReportPage<DataRowView>
    {
        #region Constants

        private const string _EMPTY_STRING = "&nbsp;"; /*how an empty column is represented while converting a DataTable into the source of a grid*/
        private const int _FECHA = 3;
        private const int _PRIMER_ESTADO = 4;

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "CLOG_REP_TICKET_MOVIL"; } }

        protected override string GetRefference() { return "MOBILE_TICKETS"; }

        public override bool SelectableRows { get { return false; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Override for generating the grid using a DataSet (not the GetResult method) as DataSource.
        /// </summary>
        protected override void Bind()
        {
            Grid.AutoGenerateColumns = false;
            Grid.AllowColMoving = false;
            Grid.AllowGrouping = false;

            if (lbMobiles.SelectedValues.Count.Equals(0))
            {
                ShowResourceError("MUST_SELECT_MOBILE");
                return;
            }
            if (!chkProgramado.Checked && !chkManual.Checked && !chkAuto.Checked)
            {
                ShowResourceError("MUST_SELECT_CLOG_TIME");
                return;
            }

            var source = ReportFactory.TicketReportDAO.GetTicketsByDateAndMobiles(lbMobiles.SelectedValues, dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(), 
                                                                                  dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime(), ddlPlanta.Selected).Tables[0];

            GenerateGridWidth(source);

            GenerateColumnsHeaders(source);

            Bind(source);

            FormatGridColumns();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        /// <summary>
        /// Not used. Declared for compatibility with the base page.
        /// </summary>
        /// <returns></returns>
        protected override List<DataRowView> GetResults() { return new List<DataRowView>(); }

        /// <summary>
        /// Adapts the values of the object for the need of the report.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="e"></param>
        /// <param name="dataItem"></param>
        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, DataRowView dataItem)
        {
            var separator = Usuario.CsvSeparator;
            e.Row.Cells[_FECHA].Text = String.Format("{0:dd/MM/yyyy}", GetDateFromString(e.Row.Cells[_FECHA].Text));

            for (var i = _PRIMER_ESTADO; i < Grid.Columns.Count; i += 3)
            {
                var Column1Date = GetDateFromString(e.Row.Cells[i].Text);
                var Column2Date = GetDateFromString(e.Row.Cells[i + 1].Text);
                var Column3Date = GetDateFromString(e.Row.Cells[i + 2].Text);

                var ColumnManual = "";
                var ColumnAuto = "";

                if (Column1Date.Equals(DateTime.MinValue))
                {
                    e.Row.Cells[i].BackColor = Color.LightGray;
                    e.Row.Cells[i].Text = "";
                    e.Row.Cells[i + 1].Text = string.Concat(separator, separator, separator, separator, separator);
                    continue;
                }
                var ColumnProg = String.Format("{0}", Column1Date.TimeOfDay.ToString().Remove(5, 3));
                var CSVDataRow = String.Concat(ColumnProg, separator);

                FormatColumnAndCSV(Column1Date, Column2Date, ref CSVDataRow, ref ColumnManual);

                FormatColumnAndCSV(Column1Date, Column3Date, ref CSVDataRow, ref ColumnAuto);

                const string column = "<td class='column'>{0}</td>";

                var data = @"<table class='datatable'><tr>";
            
                if (chkProgramado.Checked) data += string.Format(column, ColumnProg);
                if (chkManual.Checked) data += string.Format(column, ColumnManual);
                if (chkAuto.Checked) data += string.Format(column, ColumnAuto);
                data += @"</tr></table>";

                e.Row.Cells[i].Text = data;
           
                //Formats the data for the CSV
                e.Row.Cells[i + 1].Text = CSVDataRow;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the string transformed in a Date time or DateTime.MinValue in case of error.
        /// IT ALSO TRANSFORMS THE DATE TO DISPLAY_DATE
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static DateTime GetDateFromString(String str)
        {
            return str.Equals(_EMPTY_STRING) ? DateTime.MinValue : Convert.ToDateTime(str).ToDisplayDateTime();
        }

        /// <summary>
        /// Formats the grid data and the CSV data column.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="CSVDataRow"></param>
        /// <param name="ColumnAuto"></param>
        private static void FormatColumnAndCSV(DateTime date1, DateTime date2, ref string CSVDataRow, ref string ColumnAuto)
        {
            TimeSpan dif;
            var separator = Usuario.CsvSeparator;
            if (!date2.Equals(DateTime.MinValue))
            {
                dif = date1.Subtract(date2);
                ColumnAuto = String.Format("{0}({1}m)", date2.TimeOfDay.ToString().Remove(5, 3),(int) dif.TotalMinutes);
                CSVDataRow = String.Concat(CSVDataRow, string.Concat(date2.TimeOfDay.ToString().Remove(5, 3), separator, (int)dif.TotalMinutes, separator));
            }
            else CSVDataRow = String.Concat(CSVDataRow, separator, separator);
        }

        /// <summary>
        /// Formats the text to fit the columns for each EstadoLogistico
        /// </summary>
        private void FormatGridColumns()
        {
            for (var i = _PRIMER_ESTADO; i < Grid.Columns.Count; i += 3)
            {
                Grid.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                Grid.Columns[i + 1].Visible = Grid.Columns[i + 2].Visible = false;
            }
        }

        /// <summary>
        /// Generates the Headers of the grid grouping them by EstadoLogistico
        /// </summary>
        /// <param name="source"></param>
        private void GenerateColumnsHeaders(DataTable source)
        {
            var prog = FormatHeader(CultureManager.GetLabel("TIME_PROGRAMMED"));
            var man = FormatHeader(CultureManager.GetLabel("TIME_MANUAL"));
            var auto = FormatHeader(CultureManager.GetLabel("TIME_AUTOMATIC"));

            const string column = "<td class='column'>{0}</td>";

            var data = @"<div style='text-align: center;'>{0}</div><table class='datatable header'><tr>";
            if (chkProgramado.Checked) data += string.Format(column, prog);
            if (chkManual.Checked) data += string.Format(column, man);
            if (chkAuto.Checked) data += string.Format(column, auto);
            data += @"</tr></table>";

            for (var i = _PRIMER_ESTADO; i < source.Columns.Count; i += 3)
            {
                source.Columns[i].ColumnName = string.Format(data, source.Columns[i].ColumnName);
            }
        }

        private static string FormatHeader(string name)
        {
            if(name.Length < 8) return name;
            return name.Substring(0, 6) + ".";
        }

        /// <summary>
        /// Binds the grid with the source and sets the label or grid visibility according to the results set
        /// </summary>
        /// <param name="source"></param>
        private void Bind(DataTable source)
        {
            if (source.Rows.Count.Equals(0))
            {
                ShowInfo(CultureManager.GetError("NO_MATCHES_FOUND"));
            }

            Grid.Visible = !source.Rows.Count.Equals(0);

            Grid.AllowSorting = false;

            Grid.Columns.Clear();

            foreach (DataColumn column in source.Columns)
            {
                Grid.Columns.Add(new C1BoundField { HeaderText = column.ColumnName, DataField = column.ColumnName });
            }

            Grid.DataSource = source;
    
            Grid.DataBind();
        }

        /// <summary>
        /// Calculates and sets the Width needed for the grid
        /// </summary>
        /// <param name="source"></param>
        private void GenerateGridWidth(DataTable source)
        {
            Grid.Width = Unit.Pixel(851 + 350 * (source.Columns.Count - 3) % 3);
        }

        #endregion

        #region CSV Methods

        /// <summary>
        /// Fills the report with the Items of the grid*/
        /// </summary>
        private void GenerateCSVFields(BaseCsvBuilder csv)
        {
            var separator = Usuario.CsvSeparator;
            foreach (C1GridViewRow row in Grid.Rows)
            {
                var data = string.Concat(row.Cells[0].Text, separator, row.Cells[1].Text, separator, row.Cells[2].Text, separator, row.Cells[3].Text, separator);
                for (var i = _PRIMER_ESTADO; i < Grid.Columns.Count; i += 3)
                    data = String.Concat(data, String.Format("{0}",
                                                             row.Cells[i + 1].Text.Equals(_EMPTY_STRING) ? string.Concat(separator, separator, separator, separator, separator)
                                                                 : row.Cells[i + 1].Text));
                csv.GenerateRow(data.TrimEnd(separator));
            }
            csv.GenerateRow("");
        }

        /// <summary>
        /// Generates the sub-columns Programado,Manual y Automatico
        /// </summary>
        /// <param name="csv"></param>
        private void GenerateCSVSubColumns(BaseCsvBuilder csv)
        {
            var separator = Usuario.CsvSeparator;
            var subheaders = string.Concat(separator, separator, separator, separator);
            for (var i = _PRIMER_ESTADO; i < Grid.Columns.Count; i += 3)
                subheaders = String.Concat(subheaders, "Programado", separator, "Manual", separator, "Dif.Manual", separator, "Automatico", separator, "Dif.Automatico", separator);
            csv.GenerateRow(subheaders);
        }

        /// <summary>
        /// Generates the columns of the report*/
        /// </summary>
        private void GenerateCSVColumns(BaseCsvBuilder csv)
        {
            var separator = Usuario.CsvSeparator;
            var headers = string.Concat("Ticket",separator,"Interno",separator,"Transportista",separator,"Fecha");
            for (var i = _PRIMER_ESTADO; i < Grid.Columns.Count; i += 3)
            {
                var str = Grid.Columns[i].HeaderText;
                var columName = str.Remove(str.IndexOf('<')); /*removes the html used in the grid*/
                headers = String.Concat(headers, string.Concat(separator, columName, separator, separator, separator, separator));
            }
            csv.GenerateRow(headers);
        }

        protected override void ExportToCsv()
        {
            var csv = new GridToCSVBuilder(Usuario.CsvSeparator);
            csv.GenerateHeader(CultureManager.GetMenu(VariableName), GetFilterValues());

            var g = Grid;
            var allowPaging = Grid.AllowPaging;
            g.AllowPaging = false;

            Bind();

            GenerateCSVColumns(csv);

            GenerateCSVSubColumns(csv);

            GenerateCSVFields(csv);

            SetCsvSessionVars(csv.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));

            g.AllowPaging = allowPaging;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string> { 
                                                      { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                                                      { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                                                      {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                                                      {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                                                  };
        }

        #endregion
    }
}

