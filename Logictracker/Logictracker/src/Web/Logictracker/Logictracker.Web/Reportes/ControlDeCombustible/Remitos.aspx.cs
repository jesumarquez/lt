using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObject.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

namespace Logictracker.Reportes.ControlDeCombustible
{
    public partial class ControlDeCombustible_Remitos : SecuredGridReportPage<CentroDeCargaRemitosVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "COMB_REMITOS"; } }

        protected override string GetRefference() { return "REMITOS"; }

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
        protected override List<CentroDeCargaRemitosVo> GetResults()
        {
            var remitos = new List<CentroDeCargaRemitosVo>();
            double totalCargado = 0;
        
            foreach (var rem in ReportFactory.CentroDeCargaRemitosDAO.
                FindAllByDistritoPlantaAndFecha(ddlLocation.Selected, ddlPlanta.Selected,
                                                dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault()))
            {
                remitos.Add(new CentroDeCargaRemitosVo(rem));

                totalCargado += rem.TotalCargado;
            }

            lblTotal.Text = string.Format(CultureManager.GetLabel("TOTAL_CARGADO") + ": {0} lt", totalCargado);
            lblTitle.Visible = tblTotal.Visible = remitos.Count > 0;

            return remitos;
        }

        protected override void SelectedIndexChanged()
        {
            GetRemitos();
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
        /// Gets the Data for the CentroDeCostos detail. (data is used when printing)
        /// </summary>
        private void GetRemitos()
        {
            if (Grid.SelectedIndex < 0) return;
            var centro = GridUtils.GetCell(Grid.SelectedRow, CentroDeCargaRemitosVo.IndexCentroDeCostos).Text;
            var desde = dpDesde.SelectedDate.GetValueOrDefault();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var movimientos = DAOFactory.MovimientoDAO.FindRemitosByCentroAndDate(centro, desde, hasta);

            var remitos = movimientos.Select(m => new RemitoVo(m)).ToList();

            Session.Add("RemitosDetail", remitos);
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

            builder.GenerateHeader(CultureManager.GetMenu("COMB_REMITOS"), GetFilterValues());
            GenerateCSVBody(builder);

            SetCsvSessionVars(builder.Build());

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }


        /// <summary>
        /// Generates the CSV Body adding the Grid and all the Details for the selected Motores.
        /// </summary>
        /// <param name="builder"></param>
        private void GenerateCSVBody(GridToCSVBuilder builder)
        {
            var separator = Usuario.CsvSeparator;
            builder.GenerateColumns(/*new List<string>(), */Grid);
            builder.GenerateFields(Grid);

            var movimientos = DAOFactory.MovimientoDAO.FindRemitosByDate(dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault());

            var remitos = movimientos.Select(m => new RemitoVO(m)).ToList();

            foreach (var c in (from d in remitos select d.CentroDeCostos).Distinct())
            {
                var center = c;

                builder.GenerateRow(String.Empty);
                builder.GenerateRow(String.Empty);

                /*Generates the Center Description for its dispatchs datail.*/
                builder.GenerateRow(String.Concat(CultureManager.GetLabel("CENTRO_CARGA"), ":", center));

                /*Genarates the columns headers*/
                builder.GenerateRow(String.Concat(CultureManager.GetLabel("FECHA"), separator,
                                                  CultureManager.GetLabel("TIPO_INGRESO"), separator,
                                                  CultureManager.GetLabel("NRO_REMITO"), separator,
                                                  CultureManager.GetLabel("VOLUMEN")));

                foreach (var t in (from d in remitos select d.Tanque).Distinct())
                {
                    var tank = t;
                    builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI36"), tank));
                    var cargasPorCentroYTanque = (from d in remitos
                                                  where d.CentroDeCostos.Equals(center) && d.Tanque.Equals(tank)
                                                  orderby d.Fecha descending select d);

                    /*Genearates all the datails for the Remitos*/
                    foreach (var d in cargasPorCentroYTanque)
                        builder.GenerateRow(String.Concat(d.Fecha, separator, d.DescriTipo, separator, d.NroRemito, separator, d.Volumen));
                }
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocation.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        protected override void OnPrePrint()
        {
            Session["KeepInSes"] = true;

            lblTotalPrint.Text = lblTotal.Text;

            ifMobilesPrint.Visible = lblDetallePrint.Visible = true;

            GetRemitos();
        }

        #endregion
    }
}
