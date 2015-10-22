using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObject.ControlDeCombustible;
using Logictracker.Types.ValueObjects.Combustible;
using Logictracker.Web.Helpers.ExportHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class ReportesControlDeCombustiblePozosDespachosPozo : SecuredGridReportPage<ConsumosMotorVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "COMB_CONSUMOS"; } }

        protected override string GetRefference() { return "CONSUMOS"; }

        #endregion

        #region Protected Methods

        protected override List<ConsumosMotorVo> GetResults()
        {
            ToogleItems(lbMotores);

            var despachos = new List<ConsumosMotorVo>();
            double totalDespachado = 0;

            var consumos = ReportFactory.ConsumosPorMotorDAO.FindConsumosForMotores(lbMotores.SelectedValues,
                    dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault());

            if (consumos.Count <= 0) return despachos;

            foreach (var desp in consumos)
            {
                despachos.Add(new ConsumosMotorVo(desp));
                totalDespachado += desp.DifVolumen;
            }

            lblTotal.Text = string.Format(CultureManager.GetLabel("TOTAL_CONSUMIDO")+": {0} lt", totalDespachado);
            lblTitle.Visible = tblTotal.Visible = despachos.Count > 0;
        
            return despachos;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override void SelectedIndexChanged()
        {
            GetDespachos();
            ShowDespachos();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ConsumosMotorVo dataItem)
        {
            var volumen = dataItem.DifVolumen;
            var horas = dataItem.HsEnMarcha;

            GridUtils.GetCell(e.Row, ConsumosMotorVo.IndexDifVolumen).Text = String.Format("{0:00} l/h", horas.Equals(0) ? 0 : (volumen / horas));
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            base.BtnSearchClick(sender, e);

            lblTitle.Visible = ! LblInfo.Visible;

            lblMobiles.Visible = false;
            ifMobiles.Visible = false;
        }

        /// <summary>
        /// Inverse Motores selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnMotores_Click(object sender, EventArgs e) { lbMotores.ToogleItems(); }

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
            var motor = Convert.ToInt32(Grid.SelectedDataKey.Value);
            var desde = dpDesde.SelectedDate.GetValueOrDefault();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var consumos = DAOFactory.MovimientoDAO.FindConsumosByMotorAndDate(motor, desde, hasta);

            var consumosVo = consumos.Select(m => new ConsumoVo(m)).ToList();

            Session.Add("ConsumosDetail", consumosVo);
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

            builder.GenerateHeader(CultureManager.GetMenu("COMB_CONSUMOS"), GetFilterValues());
            GenerateCsvBody(builder);

            Session["CSV_EXPORT"] = builder.Build();
            Session["CSV_FILE_NAME"] = "report";

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }


        /// <summary>
        /// Generates the CSV Body adding the Grid and all the Details for the selected Motores.
        /// </summary>
        /// <param name="builder"></param>
        private void GenerateCsvBody(GridToCSVBuilder builder)
        {
            var separator = Usuario.CsvSeparator;
            builder.GenerateColumns(/*new List<string>{"Vehiculo"},*/ Grid);
            builder.GenerateFields(Grid);

            var desde = dpDesde.SelectedDate.GetValueOrDefault();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault();

            var consumos = DAOFactory.MovimientoDAO.FindConsumosBetweenDatesAndMotores(lbMotores.SelectedValues, desde,
                                                                                       hasta);
            var consumosVo = consumos.Select(d => new ConsumoVO(d)).ToList();

            foreach (var consumosPorMotor in lbMotores.SelectedValues.Select(motor => (from c in consumosVo where c.IDCaudalimetro == motor orderby c.Fecha descending select c)))
            {
                if (consumosPorMotor.Count().Equals(0)) return;

                builder.GenerateRow(String.Empty);
                builder.GenerateRow(String.Empty);

                /*Generates the Motor Description for its consumes datail.*/
                builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI39"), ":", consumosPorMotor.First().Motor));

                /*Genarates the columns headers*/
                builder.GenerateRow(String.Concat(CultureManager.GetEntity("PARENTI37"), separator,
                                                  CultureManager.GetLabel("FECHA"), separator,
                                                  CultureManager.GetLabel("INTERNO"), separator,
                                                  CultureManager.GetLabel("PATENTE"), separator,
                                                  CultureManager.GetLabel("VOLUMEN"), separator
                                                  , CultureManager.GetLabel("CHOFER")));

                /*Genearates all the datails for the Motor*/
                foreach (var c in consumosPorMotor) builder.GenerateRow(String.Concat(c.Fecha, separator, c.CentroDeCostos, separator, c.Volumen, separator, c.HsEnMarcha, separator, c.Caudal));
            }
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

            GetDespachos();
        }

        #endregion
    }
}
