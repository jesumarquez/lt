#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Types.ReportObjects.ControlDeCombustible;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Reportes.ControlDeCombustible
{
    public partial class Reportes_ControlDeCombustible_ConsistenciaStockGrafico : SecuredGraphReportPage<ConsistenciaStockGraph>
    {
        #region Constants

        private const int MaxLabels = 50;

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "COMB_CONSIST_STOCK_GRAFICO"; } }

        protected override GraphTypes GraphType { get { return GraphTypes.MultiLine; } }

        protected override string XAxisLabel { get { return CultureManager.GetLabel("FECHA"); } }

        protected override string YAxisLabel { get { return CultureManager.GetLabel("VOLUMEN"); } }

        protected override string GetRefference() { return "CONSISTENCIA_GRAPH,CONSISTENCIA_GRAPH_POZO"; }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<ConsistenciaStockGraph> GetResults()
        {   
            var desde = dpDesde.SelectedDate.GetValueOrDefault();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault();
            var tanque = ddlTanque.Selected;

            var res = ReportFactory.ConsistenciaStockGraphDAO.GetByTanqueAndDate(tanque, desde, hasta, Convert.ToInt32(npInterval.Value));
            return res.Cast<ConsistenciaStockGraph>().ToList();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                SetGraphProperties(helper);

                var teoricDataset = new FusionChartsDataset();
                var realDataset = new FusionChartsDataset();
                var labelInterval =(ReportObjectsList.Count/MaxLabels)+1;
                var i = labelInterval;

                int resto;
                Math.DivRem( Convert.ToInt32(npInterval.Value), 1440, out resto);
                var printOnlyDate = resto.Equals(0);

                teoricDataset.SetPropertyValue("seriesName","Te&oacute;rico");

                realDataset.SetPropertyValue("seriesName", "Real");
                realDataset.SetPropertyValue("color", "008ED6");

                if( ReportObjectsList.Any(o => o.StockReal >0 || o.StockTeorico > 0))
                    foreach (var consist in ReportObjectsList)
                    {
                        var str = String.Empty;
                        if (i == labelInterval)
                        {
                            str = printOnlyDate ? String.Format("{0}",consist.Fecha.ToShortDateString()) 
                                      : String.Format("{0} - {1}", consist.Fecha.ToShortDateString(), consist.Fecha.ToShortTimeString());
                            i = 0;
                        }
                        i++;
                        helper.AddCategory(str);
                        teoricDataset.addValue(consist.StockTeorico.ToString(CultureInfo.InvariantCulture));
                        realDataset.addValue(consist.StockReal.ToString(CultureInfo.InvariantCulture));
                    }
                if(chkTeorico.Checked)helper.AddDataSet(teoricDataset);
                if(chkReal.Checked)helper.AddDataSet(realDataset);

                var s = helper.BuildXml();
                return s;
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
            helper.AddConfigEntry("caption", string.Format("Tanque: {0} - Desde: {1} - {2}  Hasta: {3} - {4}",
                                                           ddlTanque.Selected != 0 ? DAOFactory.TanqueDAO.FindById(ddlTanque.Selected).Descripcion : "",
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


        #endregion

        #region CSV Methods

        /// <summary>
        /// Generates the categories and Datasets for the 
        /// </summary>
        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var teoricDataset = new FusionChartsDataset { Name = CultureManager.GetLabel("TEORICO") };
            var realDataset = new FusionChartsDataset { Name = CultureManager.GetLabel("REAL") };
            var categories = new List<string>();

            foreach (var consist in ReportObjectsList)
            {
                categories.Add(String.Format("{0} - {1}", consist.Fecha.ToShortDateString(), consist.Fecha.ToShortTimeString()));
                teoricDataset.addValue(consist.StockTeorico.ToString(CultureInfo.InvariantCulture));
                realDataset.addValue(consist.StockReal.ToString(CultureInfo.InvariantCulture));
            }

            if (chkTeorico.Checked) datasets.Add(teoricDataset);
            if (chkReal.Checked) datasets.Add(realDataset);

            GraphCategories = categories;
            GraphDataSet = datasets;
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
                           {CultureManager.GetEntity("PARENTI36"), ddlTanque.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString() },
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString() }
                       };
        }

        #endregion

    }
}
