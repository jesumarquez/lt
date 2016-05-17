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
    public partial class ControlDeCombustible_Despachos : SecuredGridReportPage<CentroDeCostosDespachosVo>
    {

        #region Protected Properties

        protected override string VariableName { get { return "COMB_DESPACHOS"; } }

        private List<int> selectedVehicles
        {
            get { return ViewState["selectedVehicles"] != null ? (List<int>) ViewState["selectedVehicles"] : new List<int>(); }
            set { ViewState["selectedVehicles"] = value; }
        }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<CentroDeCostosDespachosVo> GetResults()
        {
            var despachos = new List<CentroDeCostosDespachosVo>();
            double totalDespachado = 0;
            GetSelectedMobiles(false);

            foreach (var desp in ReportFactory.CentroDeCostosDespachosDAO.FindByPlanta(ddlPlanta.Selected, selectedVehicles,
                                                                                       dpDesde.SelectedDate.GetValueOrDefault(),dpHasta.SelectedDate.GetValueOrDefault()))
            {
                despachos.Add(new CentroDeCostosDespachosVo(desp));
                totalDespachado += desp.TotalDespachado;
            }

            lblTotal.Text = string.Format(CultureManager.GetLabel("TOTAL_DESPACHADO")+": {0} lt", totalDespachado);
            lblTitle.Visible = tblTotal.Visible = despachos.Count > 0;
        
            return despachos;
        }

        protected override void SelectedIndexChanged()
        {
            GetDespachos();
            ShowDespachos();
        }


        /// <summary>
        /// Searches for results of the selected parameters and sets iframe visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            base.BtnSearchClick(sender, e);

            lblTitle.Visible = ! LblInfo.Visible;

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
        private void GetDespachos()
        {
            if (Grid.SelectedIndex < 0) return;
            var centro = GridUtils.GetCell(Grid.SelectedRow, CentroDeCostosDespachosVo.IndexCentroDeCostos).Text;
            var desde = dpDesde.SelectedDate.GetValueOrDefault();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var despachos = DAOFactory.MovimientoDAO.FindDespachosByCentroAndDate(selectedVehicles ,centro, desde, hasta);

            var despachosVO = despachos.Select(m => new DespachoVo(m)).ToList();

            Session.Add("DespachosDetail", despachosVO);
        }

        /// <summary>
        /// Get a string that represents the ids of all selected lines.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedMobiles(bool showTextValue)
        {
            if (lbMobiles.GetSelectedIndices().Length == 0) lbMobiles.ToogleItems();

            selectedVehicles = new List<int>(lbMobiles.SelectedValues);

            var ids = string.Empty;

            foreach (var index in lbMobiles.GetSelectedIndices())
                ids = string.Concat(ids, string.Format("{0},", showTextValue ? lbMobiles.Items[index].Text : lbMobiles.Items[index].Value));
            
            return ids.TrimEnd(',');
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "DESPACHOS"; }

        #endregion

        #region CSV and Print Methods

        /// <summary>
        /// Exports the Grid to CSV.
        /// Cuidado con esto porque rebindea la grilla sacandole el paging.
        /// </summary>
        protected override void ExportToCsv()
        {
            var builder = new GridToCSVBuilder(Usuario.CsvSeparator);

            builder.GenerateHeader(CultureManager.GetMenu("COMB_DESPACHOS"), GetFilterValues());
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
            builder.GenerateColumns(/*new List<string>(),*/ Grid);
            builder.GenerateFields(Grid);

            var despachos = DAOFactory.MovimientoDAO.FindDespachosBetweenDatesAndMobiles(selectedVehicles, dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault());
            var despachosVO = despachos.Select(d => new DespachoVO(d)).ToList();


            foreach (var c in (from d in despachosVO select d.CentroDeCarga).Distinct())
            {
                var center = c;

                builder.GenerateRow(String.Empty);
                builder.GenerateRow(String.Empty);

                /*Generates the Center Description for its dispatchs datail.*/
                builder.GenerateRow(String.Concat(CultureManager.GetLabel("CENTRO_CARGA"), ":", center));

                /*Genarates the columns headers*/
                builder.GenerateRow(String.Concat(CultureManager.GetLabel("FECHA"), separator,
                                                  CultureManager.GetEntity("PARENTI37"), separator,
                                                  CultureManager.GetLabel("INTERNO"), separator,
                                                  CultureManager.GetLabel("PATENTE"), separator,
                                                  CultureManager.GetLabel("VOLUMEN"), separator
                                                  , CultureManager.GetLabel("CHOFER")));

                foreach(var v in (from d in despachosVO select d.Vehiculo).Distinct())
                {
                    var vehicle = v;
                    builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI03"), vehicle));
                    var despachosPorCentroYVehiculo = (from d in despachosVO where d.CentroDeCarga.Equals(center) &&
                                                                                   d.Vehiculo.Equals(vehicle) orderby d.Fecha descending select d);

                    /*Genearates all the datails for the Motor*/
                    foreach (var d in despachosPorCentroYVehiculo)
                        builder.GenerateRow(String.Concat(d.Fecha, separator, d.CentroDeCostos, separator, d.InternoVehiculo, separator, d.Patente, separator,
                                                          d.Volumen, separator, d.Operador));
                }
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI03"), GetSelectedMobiles(true)},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        protected override void OnPrePrint()
        {
            Session["KeepInSes"] = true;

            lblTotalPrint.Text = lblTotal.Text;

            ifMobilesPrint.Visible = lblDetallePrint.Visible = true;

            GetDespachos();

        }

        #endregion
    }
}
