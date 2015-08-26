#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class ReportesCombustibleEnPozosConsistenciaStockPozo : SecuredGridReportPage<ConsistenciaStockPozoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "COMB_CONSISTENCIA_STOCK"; } }

        protected override string GetRefference() { return "CONSISTENCIA_POZO"; }

        protected override bool ScheduleButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            return ddlLocation.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Beggining Report Date
        /// </summary>
        private DateTime Desde
        {
            get { return ViewState["Desde"] != null ? Convert.ToDateTime(ViewState["Desde"]) : DateTime.MinValue; }
            set { ViewState["Desde"] = value; }
        }

        /// <summary>
        /// End Report Date
        /// </summary>
        private DateTime Hasta
        {
            get { return ViewState["Hasta"] != null ? Convert.ToDateTime(ViewState["Hasta"]) : DateTime.MinValue; }
            set { ViewState["Hasta"] = value; }
        }

        /// <summary>
        /// Current dispatch center.
        /// </summary>
        private string CentroDeCarga
        {
            get { return ViewState["CentroDeCarga"] != null ? ViewState["CentroDeCarga"].ToString() : String.Empty; }
            set { ViewState["CentroDeCarga"] = value; }
        }

        #endregion

        #region Protected Methods

        public override int PageSize { get { return 31; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        /// <summary>
        /// Gets the results to fill the grid
        /// </summary>
        /// <returns></returns>
        protected override List<ConsistenciaStockPozoVo> GetResults()
        {
            Desde = dpDesde.SelectedDate.GetValueOrDefault();
            Hasta = dpHasta.SelectedDate.GetValueOrDefault();

            CentroDeCarga = ddlTanque.Selected > 0 ? DAOFactory.TanqueDAO.FindById(ddlTanque.Selected).Descripcion : "";

            var result = ddlTanque.Selected > 0 ? ReportFactory.ConsistenciaStockPozoDAO.FindConsistenciaBetweenDates(ddlTanque.Selected, Desde, Hasta)
                                                : new List<ConsistenciaStockPozoVo>();
            return (from ConsistenciaStockPozo consistencia in result select new ConsistenciaStockPozoVo(consistencia)). ToList();
        }


        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ConsistenciaStockPozoVo dataItem)
        {
            FormatDataItem(e, dataItem);
        }

        /// <summary>
        /// Subtotals data binding and formating.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridSubtotales_ItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var stock = e.Row.DataItem as ConsistenciaStockPozoVo;

            if (stock == null) return;

            FormatDataItem(e, stock);

            e.Row.BackColor = Color.Yellow;
        }

        

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetLabel("CENTRO_DE_CARGA"),CentroDeCarga },
                           { CultureManager.GetEntity("PARENTI19"), ddlEquipo.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI36"), (ddlTanque.SelectedItem != null) ? ddlTanque.SelectedItem.Text : "" },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           { CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            return new Dictionary<string, string> { { "TANQUE", (ddlTanque.Selected > 0) ? ddlTanque.Selected.ToString() : "0" } };
        }

        /// <summary>
        /// Displays report data and subtotals.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            base.BtnSearchClick(sender, e);

            DisplaySubtotals();
        }

        protected override void ExportToCsv()
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            var g = Grid;

            g.AllowPaging = false;
            g.DataSource = GetResults();
            g.DataBind();

            builder.GenerateHeader(CultureManager.GetMenu("COMB_CONSISTENCIA_STOCK"), GetFilterValues());

            builder.GenerateColumns(/*null,*/ g);

            builder.GenerateFields(g);

            builder.GenerateColumns(/*null,*/ gridSubTotales);

            builder.GenerateFields(gridSubTotales);

            Session["CSV_EXPORT"] = builder.Build();
            Session["CSV_FILE_NAME"] = "report";
             g.AllowPaging = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Displays subtotals for the current period.
        /// </summary>
        private void DisplaySubtotals()
        {
            var subtotals = GetSubtotals();

            gridSubTotales.Visible = subtotals != null;

            if (!gridSubTotales.Visible) return;

            gridSubTotales.DataSource = new List<ConsistenciaStockPozo> {subtotals};

            gridSubTotales.DataBind();
        }

        /// <summary>
        /// Calculates subtotals for the current period.
        /// </summary>
        /// <returns></returns>
        private ConsistenciaStockPozo GetSubtotals()
        {
            if (ReportObjectsList == null || ReportObjectsList.Count.Equals(0)) return null;

            var subtotales = new ConsistenciaStockPozo();

            var first = ReportObjectsList[0];
            var last = ReportObjectsList[ReportObjectsList.Count - 1];

            subtotales.Fecha = last.Fecha;
            subtotales.StockInicial = first.StockInicial;
            subtotales.Ingresos = (from stock in ReportObjectsList select stock.Ingresos).Sum();
            subtotales.IngresosPorConciliacion = (from stock in ReportObjectsList select stock.IngresosPorConciliacion).Sum();
            subtotales.Egresos = (from stock in ReportObjectsList select stock.Egresos).Sum();
            subtotales.EgresosPorConciliacion = (from stock in ReportObjectsList select stock.EgresosPorConciliacion).Sum();
            subtotales.StockFinal = last.StockFinal;
            subtotales.StockSonda = last.StockSonda - first.StockSonda;
            subtotales.DiferenciaDeStock = (from stock in ReportObjectsList select stock.DiferenciaDeStock).Sum();

            return subtotales;
        }

        /// <summary>
        /// Formats display data.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="stock"></param>
        private void FormatDataItem(C1GridViewRowEventArgs e, ConsistenciaStockPozoVo stock)
        {
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexFecha).Text = stock.Fecha.ToString("dd/MM/yyyy");
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexStockInicial).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexStockInicial).Text));
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexIngresos).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexIngresos).Text));
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexEgresosPorConciliacion).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexIngresosPorConciliacion).Text));
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexEgresos).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexEgresos).Text));
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexEgresosPorConciliacion).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexEgresosPorConciliacion).Text));
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexStockFinal).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexStockFinal).Text));
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexStockSonda).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexStockSonda).Text));
            GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexDiferenciaDeStock).Text = string.Format("{0:00} lit", Convert.ToDouble(GridUtils.GetCell(e.Row, ConsistenciaStockPozoVo.IndexDiferenciaDeStock).Text));
        }

        #endregion
    }
}