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

#endregion

namespace Logictracker.Reportes.ControlDeCombustible
{
    public partial class ControlDeCombustibleConsistenciaStock : SecuredGridReportPage<ConsistenciaStockVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "COMB_CONSISTENCIA_STOCK"; } }

        protected override string GetRefference() { return "CONSISTENCIA"; }

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

        protected override List<ConsistenciaStockVo> GetResults()
        {
            Desde = dpDesde.SelectedDate.GetValueOrDefault();
            Hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var tanque = ddlTanque.Selected;

            var result = ReportFactory.ConsistenciaStockDAO.GetByTanqueAndDateGroupedByDay(tanque, Desde, Hasta);

            return (from ConsistenciaStock o in result select new ConsistenciaStockVo(o)).ToList();
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

        /// <summary>
        /// Subtotal items data binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridSubtotales_ItemDataBound(object sender, C1GridViewRowEventArgs e) { e.Row.BackColor = Color.Yellow; }

        #region CSV And Print Methods


        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI36"), ddlTanque.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        protected override void ExportToCsv()
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            var g = Grid;

            g.AllowPaging = false;
            g.DataSource = GetResults();
            g.DataBind();

            builder.GenerateHeader(CultureManager.GetMenu("COMB_CONSISTENCIA_STOCK"), GetFilterValues());

            builder.GenerateColumns(/*new List<string>(),*/ g);

            builder.GenerateFields(g);

            builder.GenerateColumns(/*null, */gridSubTotales);

            builder.GenerateFields(gridSubTotales);

            SetCsvSessionVars(builder.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));

            g.AllowPaging = true;
        }

        #endregion

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

            gridSubTotales.DataSource = new List<ConsistenciaStock> {subtotals};

            gridSubTotales.DataBind();
        }

        /// <summary>
        /// Gets subtotals for the current period.
        /// </summary>
        /// <returns></returns>
        private ConsistenciaStock GetSubtotals()
        {
            if (ReportObjectsList == null || ReportObjectsList.Count.Equals(0)) return null;

            var first = ReportObjectsList[0];
            var last = ReportObjectsList[ReportObjectsList.Count - 1];

            return new ConsistenciaStock
                                {
                                    Fecha = last.Fecha,
                                    Egresos = (from consistencia in ReportObjectsList select consistencia.Egresos).Sum(),
                                    Ingresos = (from consistencia in ReportObjectsList select consistencia.Ingresos).Sum(),
                                    StockFinal = last.StockFinal,
                                    StockInicial = first.StockInicial,
                                    Diferencia = (from consistencia in ReportObjectsList select consistencia.Diferencia).Sum(),
                                    StockSonda = last.StockSonda - first.StockSonda
                                };
        }

        #endregion
    }
}