#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Utils;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Reportes.CombustibleEnPozos
{
    public partial class ReportesCombustibleEnPozosVariacionDeNivel : SecuredGraphReportPage<ConsumoDiario>
    {
        #region Constants

        /// <summary>
        /// Defines the maximun number of labels to be displayed.
        /// </summary>
        private const int _MAX_LABELS = 50;

        #endregion

        #region Private Properties

        /// <summary>
        /// Date variable name.
        /// </summary>
        private const string DATE = "DATE";

        /// <summary>
        /// List for containing engines consumed volume data.
        /// </summary>
        private List<ConsumoDiario> _motores = new List<ConsumoDiario>();

        #endregion

        #region Protected Properties

        protected override GraphTypes GraphType { get { return GraphTypes.MultiLine; } }

        protected override string XAxisLabel { get { return CultureManager.GetLabel(DATE); } }

        protected override string YAxisLabel { get { return CultureManager.GetLabel("VOLUMEN"); } }

        protected override string VariableName { get { return "COMB_VARIACION_TANQUE"; } }

        protected override string GetRefference() { return "COMB_VARIACION_TANQUE"; }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<ConsumoDiario> GetResults()
        {
            var niveles = ReportFactory.ConsumoDiarioDAO.FindVariacionDeNivelTanque(ddlTanque.Selected, dpDesde.SelectedDate.GetValueOrDefault(),
                        dpHasta.SelectedDate.GetValueOrDefault(), Convert.ToInt32(npIntervalo.Value));

            _motores = ReportFactory.ConsumoDiarioDAO.FindVariacionDeNivelMotores(ddlTanque.Selected, dpDesde.SelectedDate.GetValueOrDefault(),
                        dpHasta.SelectedDate.GetValueOrDefault(), Convert.ToInt32(npIntervalo.Value));

            return niveles;
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                SetGraphProperties(helper);

                AddTankLevelVariation(helper);

                AddEnginesConsumption(helper);

                return helper.BuildXml();
            }
        }

       
        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the graph configuration
        /// </summary>
        /// <param name="helper"></param>
        private void SetGraphProperties(FusionChartsMultiSeriesHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format("Volumen Consumido en: {0} - Desde: {1} - {2}  Hasta: {3} - {4}", ddlTanque.SelectedItem.Text,
                dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString(),
                dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()));

            helper.AddConfigEntry("xAxisName", XAxisLabel );
            helper.AddConfigEntry("yAxisName", YAxisLabel );
            helper.AddConfigEntry("decimalPrecision", "0");
            helper.AddConfigEntry("thousandSeparator", ".");
            helper.AddConfigEntry("formatNumberScale", "0");
            helper.AddConfigEntry("numberSuffix", "lt");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("showNames", "1");
            helper.AddConfigEntry("rotateNames", "1");
        }

        /// <summary>
        /// Adds tank level variation.
        /// </summary>
        /// <param name="helper"></param>
        private void AddTankLevelVariation(FusionChartsMultiSeriesHelper helper)
        {
            var niveles = new FusionChartsDataset();

            var labelInterval = (ReportObjectsList.Count / _MAX_LABELS) + 1;
            var i = labelInterval;

            int resto;
            Math.DivRem(Convert.ToInt32(npIntervalo.Value), 1440, out resto);
            var printOnlyDate = resto.Equals(0);

            niveles.SetPropertyValue("seriesName", ddlTanque.SelectedItem.Text);
            niveles.SetPropertyValue("color", HexColorUtil.ColorToHex(Color.SteelBlue));

            if(ReportObjectsList.Any(o => o.VolumenConsumido > 0.01))
            {
                foreach (var nivel in ReportObjectsList)
                {
                    var str = string.Empty;

                    if (i.Equals(labelInterval))
                    {
                        str = printOnlyDate ? String.Format("{0}", nivel.Fecha.ToShortDateString()) :
                                String.Format("{0} - {1}", nivel.Fecha.ToShortDateString(), nivel.Fecha.ToShortTimeString());

                        i = 0;
                    }

                    i++;

                    helper.AddCategory(str);

                    niveles.addValue(nivel.VolumenConsumido.ToString(CultureInfo.InvariantCulture));
                }
            }
            helper.AddDataSet(niveles);
        }

        /// <summary>
        /// adds the consumptions made by all engines associated to the tank.
        /// </summary>
        /// <param name="helper"></param>
        private void AddEnginesConsumption(FusionChartsMultiSeriesHelper helper)
        {
            var niveles = new FusionChartsDataset();

            niveles.SetPropertyValue("seriesName", CultureManager.GetLabel("MOTORES"));

            if (_motores.Where(o => o.VolumenConsumido > 0.01).Count() > 0)
                foreach (var consumo in _motores) niveles.addValue(consumo.VolumenConsumido.ToString(CultureInfo.InvariantCulture));

            helper.AddDataSet(niveles);
        }

        #endregion

        #region CSV Methods

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlLocation.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI19"), ddlEquipo.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI36"), ddlTanque.SelectedItem.Text},
                                {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                                {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                            };
        }

        /// <summary>
        /// Generates the Datasets and Categories for the CSV.
        /// </summary>
        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();

            var consumos = new FusionChartsDataset { Name = CultureManager.GetLabel("MOTORES") };
            foreach (var consumo in _motores) consumos.addValue(consumo.VolumenConsumido.ToString(CultureInfo.InvariantCulture));
            datasets.Add(consumos);

            var niveles = new FusionChartsDataset { Name = ddlTanque.SelectedItem.Text };
            foreach (var r in ReportObjectsList) niveles.addValue(r.VolumenConsumido.ToString(CultureInfo.InvariantCulture));
            datasets.Add(niveles);

            var categories = ReportObjectsList.Select(nivel => String.Format("{0} - {1}", nivel.Fecha.ToShortDateString(), nivel.Fecha.ToShortTimeString())).ToList();

            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}