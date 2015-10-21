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
    public partial class Reportes_CombustibleEnPozos_ConsumoCaudalMotores : SecuredGraphReportPage<ConsumoCaudalMotor>
    {
        #region Constants

        /// <summary>
        /// Defines the maximun number of labels to be displayed.
        /// </summary>
        private const int MaxLabels = 50;

        #endregion

        #region Protected Properties

        protected override GraphTypes GraphType{ get { return GraphTypes.MultiLine; } }

        protected override string XAxisLabel { get { return CultureManager.GetLabel("DATE"); } }

        protected override string YAxisLabel { get { return CultureManager.GetLabel("VOLUMEN"); } }

        protected override string VariableName { get { return "CONSUMO_CAUDAL_MOTORES"; } }

        protected override string GetRefference() { return "CONSUMO_CAUDAL_MOTOR"; }

        #endregion

        #region Protected Methods

        protected override List<ConsumoCaudalMotor> GetResults()
        {
            ToogleItems(lbMotor);

            var motores = ReportFactory.ConsumoCaudalMotorDAO.FindConsumoCaudalMotores(lbMotor.SelectedValues,
                                                                                       dpDesde.SelectedDate.GetValueOrDefault(), dpHasta.SelectedDate.GetValueOrDefault(), Convert.ToInt32(npIntervalo.Value));
            return motores ?? new List<ConsumoCaudalMotor>();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                SetGraphProperties(helper);
            
                AddDates(helper);

                if (chkCaudal.Checked)
                {
                    AddEnginesCaudal(helper);

                    AddEnginesCaudalTrendline(helper);
                }
            
                if (chkConsumo.Checked)
                {
                    AddEnginesConsumption(helper);

                    AddEnginesConsumptionTrendline(helper);
                }

                if (!chkCaudal.Checked && !chkConsumo.Checked)
                {
                    AddEnginesCaudal(helper);
                    AddEnginesCaudalTrendline(helper);

                    AddEnginesConsumption(helper);
                    AddEnginesConsumptionTrendline(helper);
                }

                return helper.BuildXml();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var avgConsumption = (from nivel in ReportObjectsList select nivel.Consumo).Average();
            var consumos = new FusionChartsDataset
                               { Name = string.Format("{0}: {1}lit", CultureManager.GetLabel("CONSUMO"), (int) avgConsumption) };

            var avgCaudal = (from nivel in ReportObjectsList select nivel.Caudal).Average();
            var caudal = new FusionChartsDataset
                             { Name = string.Format("{0}: {1}lit", CultureManager.GetLabel("CAUDAL"), (int) avgCaudal) };

            int resto;
            Math.DivRem(Convert.ToInt32(npIntervalo.Value), 1440, out resto);
            var printOnlyDate = resto.Equals(0);

            var categories = ReportObjectsList.Select(nivel => printOnlyDate 
                                                                   ? String.Format("{0}", nivel.Fecha.ToShortDateString()) 
                                                                   : String.Format("{0} - {1}", nivel.Fecha.ToShortDateString(), nivel.Fecha.ToShortTimeString())).ToList();

            foreach (var nivel in ReportObjectsList) caudal.addValue(nivel.Caudal.ToString(CultureInfo.InvariantCulture));
            foreach (var dia in ReportObjectsList) consumos.addValue(dia.Consumo.ToString(CultureInfo.InvariantCulture));

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { caudal, consumos };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocation.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI19"), ddlEquipo.SelectedItem.Text},
                           {CultureManager.GetLabel("INTERVALO"), npIntervalo.Value.ToString()},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the graph configuration
        /// </summary>
        /// <param name="helper"></param>
        private void SetGraphProperties(FusionChartsMultiSeriesHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format("Volumen Consumido Desde: {0} - {1}  Hasta: {2} - {3}",
                                                           dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString(),
                                                           dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()));

            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
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
        private void AddEnginesCaudal(FusionChartsMultiSeriesHelper helper)
        {
            var niveles = new FusionChartsDataset();

            var avgCaudal = (from nivel in ReportObjectsList select nivel.Caudal).Average();

            niveles.SetPropertyValue("seriesName", string.Format("{0}: {1}lit", CultureManager.GetLabel("CAUDAL"), (int) avgCaudal));
            niveles.SetPropertyValue("color", HexColorUtil.ColorToHex(Color.SteelBlue));

            foreach (var nivel in ReportObjectsList)
            {
                niveles.addValue(nivel.Caudal.ToString(CultureInfo.InvariantCulture));
            }
            helper.AddDataSet(niveles);
        }

        /// <summary>
        /// Adds engine average caudal refference line.
        /// </summary>
        /// <param name="helper"></param>
        private void AddEnginesCaudalTrendline(FusionChartsMultiSeriesHelper helper)
        {
            var trendline = new FusionChartsTrendline();

            var avgCaudal = (from nivel in ReportObjectsList select nivel.Caudal).Average();

            trendline.AddPropertyValue("startValue", string.Format("{0}", (int)avgCaudal));
            trendline.AddPropertyValue("displayValue", string.Format("{0}: {1}lit", CultureManager.GetLabel("CAUDAL"), (int)avgCaudal));
            trendline.AddPropertyValue("color", HexColorUtil.ColorToHex(Color.LightSteelBlue));
            trendline.AddPropertyValue("thickness", "3");

            helper.AddTrendLine(trendline);
        }

        /// <summary>
        /// Adds engine average consumption refference line.
        /// </summary>
        /// <param name="helper"></param>
        private void AddEnginesConsumptionTrendline(FusionChartsMultiSeriesHelper helper)
        {
            var trendline = new FusionChartsTrendline();

            var avgConsumption = (from nivel in ReportObjectsList select nivel.Consumo).Average();

            trendline.AddPropertyValue("startValue", string.Format("{0}", (int)avgConsumption));
            trendline.AddPropertyValue("displayValue", string.Format("{0}: {1}lit", CultureManager.GetLabel("CONSUMO"), (int)avgConsumption));
            trendline.AddPropertyValue("color", HexColorUtil.ColorToHex(Color.LightCoral));
            trendline.AddPropertyValue("thickness", "3");

            helper.AddTrendLine(trendline);
        }

        /// <summary>
        /// Adda dates to graph
        /// </summary>
        /// <param name="helper"></param>
        private void AddDates (FusionChartsMultiSeriesHelper helper)
        {
            var labelInterval = (ReportObjectsList.Count / MaxLabels) + 1;
            var i = labelInterval;
            int resto;
            Math.DivRem(Convert.ToInt32(npIntervalo.Value), 1440,out resto);
            var printOnlyDate = resto.Equals(0);
      
            foreach (var nivel in ReportObjectsList)
            {
                var str = string.Empty;

                if (i.Equals(labelInterval))
                {
                    str = printOnlyDate ? String.Format("{0}",nivel.Fecha.ToShortDateString())
                              : String.Format("{0} - {1}", nivel.Fecha.ToShortDateString(), nivel.Fecha.ToShortTimeString());

                    i = 0;
                }

                i++;

                helper.AddCategory(str);

            }
        }

        /// <summary>
        /// adds the consumptions made by all engines associated to the tank.
        /// </summary>
        /// <param name="helper"></param>
        private void AddEnginesConsumption(FusionChartsMultiSeriesHelper helper)
        {
            var niveles = new FusionChartsDataset();

            var avgConsumption = (from nivel in ReportObjectsList select nivel.Consumo).Average();

            niveles.SetPropertyValue("seriesName", string.Format("{0}: {1}lit", CultureManager.GetLabel("CONSUMO"), (int)avgConsumption));

            foreach (var dia in ReportObjectsList) niveles.addValue(dia.Consumo.ToString(CultureInfo.InvariantCulture));

            helper.AddDataSet(niveles);
        }

        #endregion
    }
}
