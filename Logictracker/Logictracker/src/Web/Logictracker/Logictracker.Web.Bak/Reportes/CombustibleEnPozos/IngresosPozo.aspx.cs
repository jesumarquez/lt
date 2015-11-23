#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Types.ValueObject.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class Reportes_CombustibleEnPozos_IngresosPozo : SecuredGridReportPage<IngresosPorEquipoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "COMB_INGRESOS_POZO"; } }

        protected override string GetRefference() { return "INGRESOS_POZO"; }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        /// <summary>
        /// Gets the report results for the selected parameters.
        /// </summary>
        /// <returns></returns>
        protected override List<IngresosPorEquipoVo> GetResults()
        {
            var ingresos = new List<IngresosPorEquipoVo>();
            double totalCargado = 0;
        
            foreach (IngresosPorEquipo ing in ReportFactory.IngresosPorTanqueDAO.FindAllTanquesBetweenDatesByEquipo(ddlEquipo.Selected,
                                                                                                                    dpDesde.SelectedDate.GetValueOrDefault(),dpHasta.SelectedDate.GetValueOrDefault()))
            {
                ingresos.Add(new IngresosPorEquipoVo(ing));
                totalCargado += ing.TotalCargado;
            }

            lblTotal.Text = string.Format(CultureManager.GetLabel("TOTAL_CARGADO") + ": {0} lt", totalCargado); /*porq daba negativo magicamente*/
            lblTitle.Visible = tblTotal.Visible = ingresos.Count > 0;

            return ingresos;
        }

        protected override void SelectedIndexChanged()
        {
            GetIngresos();

            ShowDespachos();
        }

        /// <summary>
        /// Search Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            base.BtnSearchClick(sender, e);

            lblTitle.Visible = !  LblInfo.Visible;

            lblMobiles.Visible = false;
            ifMobiles.Visible = false;
        }

        /// <summary>
        /// Shows the detail of a specific CentroDeCostos
        /// </summary>
        private void ShowDespachos()
        {
            ifMobiles.Visible = true;
            lblMobiles.Visible = true;
        }

        /// <summary>
        /// Gets the Data for the CentroDeCostos detail.
        /// </summary>
        private void GetIngresos()
        {
            if (Grid.SelectedIndex < 0) return;
            var idequipo = Convert.ToInt32(Grid.SelectedDataKey.Value);
            var desde = dpDesde.SelectedDate.GetValueOrDefault();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var movimientos = DAOFactory.MovimientoDAO.FindIngresosByEquipoAndDate(idequipo, desde, hasta);

            var remitos = movimientos.Select(m => new IngresoVo(m)).ToList();

            Session.Add("IngresosDetail", remitos);
        }

        #endregion

        #region CSV and Print Methods

        /// <summary>
        /// Exports the Grid to CSV.
        /// Cuidado con esto porque rebindea la grilla sacandole el paging.
        /// </summary>
        protected override void ExportToCsv()
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            builder.GenerateHeader(CultureManager.GetMenu("COMB_INGRESOS_POZO"), GetFilterValues());
            GenerateCSVBody(builder);

            Session["CSV_EXPORT"] = builder.Build();
            Session["CSV_FILE_NAME"] = "report";

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }


        /// <summary>
        /// Generates the CSV Body adding the Grid and all the Details for the selected Motores.
        /// </summary>
        /// <param name="builder"></param>
        private void GenerateCSVBody(GridToCSVBuilder builder)
        {
            var separator = Usuario.CsvSeparator;
            builder.GenerateColumns(/*null,*/ Grid);
            builder.GenerateFields(Grid);

            var movimientos = DAOFactory.MovimientoDAO.FindIngresosByDate(dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault());

            var remitos = movimientos.Select(m => new IngresoVO(m)).ToList();

            var e = ddlEquipo.Selected;

            builder.GenerateRow(String.Empty);
            builder.GenerateRow(String.Empty);

            var ingresosPorEquipo = (from i in remitos where i.IDEquipo == e orderby i.Fecha descending select i);
            if (ingresosPorEquipo.Count().Equals(0)) return;

            /*Generates the Motor Description for its consumes datail.*/
            builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI19"), ":", ingresosPorEquipo.First().NombreEquipo));

            /*Genarates the columns headers*/
            builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI36"), separator,
                                              CultureManager.GetLabel("FECHA"), separator,
                                              CultureManager.GetEntity("OPECOMB01"), separator,
                                              CultureManager.GetLabel("VOLUMEN")));

            /*Genearates all the datails for the Motor*/
            foreach (var i in ingresosPorEquipo)
                builder.GenerateRow(String.Concat(i.Tanque, separator, i.Fecha, separator, i.DescriTipo, separator, i.Volumen));

        }
        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI19"), ddlEquipo.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        protected override void OnPrePrint()
        {
            Session["KeepInSes"] = true;

            lblTotalPrint.Text = lblTotal.Text;

            ifMobilesPrint.Visible = lblDetallePrint.Visible = true;

            GetIngresos();
        }

        #endregion
    }
}

