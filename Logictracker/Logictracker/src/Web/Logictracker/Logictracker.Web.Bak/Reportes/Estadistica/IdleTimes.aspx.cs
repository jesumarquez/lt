using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Utils;
using Logictracker.Web.Helpers.ColorHelpers;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesEstadisticaIdleTimes : SecuredGraphReportPage<IdleTimes>
    {
        protected override GraphTypes GraphType { get { return GraphTypes.MultiColumn; } }
        protected override string VariableName { get { return "EST_IDLE_TIME"; } }
        protected override string GetRefference() { return "EST_IDLE_TIMES"; }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("FECHA"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("CANTIDAD_VEHICULOS"); } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dtpDate.SetDate();
        }

        protected override List<IdleTimes> GetResults() { return new List<IdleTimes> {new IdleTimes()}; }

        protected override string GetGraphXml()
        {
            ToogleItems(lbBase);

            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                SetGraphConfiguration(helper);

                var iniDate = SecurityExtensions.ToDataBaseDateTime(dtpDate.SelectedDate.GetValueOrDefault());
                var finDate = SecurityExtensions.ToDataBaseDateTime(dtpDate.SelectedDate.GetValueOrDefault()).AddHours(23).AddMinutes(59).AddSeconds(59);
                var colorGen = new ColorGenerator();

                var noCategoriesAdded = true;
                var hasValue = false;
                if(lbBase.GetSelectedIndices().Length == 0) lbBase.ToogleItems();

                foreach(var index in lbBase.GetSelectedIndices())
                {
                    var data = ReportFactory.IdleTimesDAO.GetAllMovilesStoppedInPlanta(Convert.ToInt32((string) lbBase.Items[index].Value),
                                                                                       iniDate, finDate, chkUndefined.Checked);

                    var dataset = new FusionChartsDataset { Name = lbBase.Items[index].Text };
                    dataset.SetPropertyValue("SeriesName", lbBase.Items[index].Text);
                    dataset.SetPropertyValue("color", HexColorUtil.ColorToHex(colorGen.GetNextColor()));
                
                    foreach (var item in data)
                    {
                        if (noCategoriesAdded) helper.AddCategory(item.Date.ToShortTimeString());
                        dataset.addValue(item.TotalVehicles.ToString(CultureInfo.InvariantCulture));
                        if (!item.TotalVehicles.Equals(0)) hasValue = true; 
                    }

                    helper.AddDataSet(dataset);
                    noCategoriesAdded = false;
                }

                if (!hasValue) throw new Exception(CultureManager.GetError("NO_MOBILE_IN_BASE"));

                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var categories = new List<string>();

            var iniDate = SecurityExtensions.ToDataBaseDateTime(dtpDate.SelectedDate.GetValueOrDefault());
            var finDate = SecurityExtensions.ToDataBaseDateTime(dtpDate.SelectedDate.GetValueOrDefault()).AddHours(23).AddMinutes(59).AddSeconds(59);
            var colorGen = new ColorGenerator();

            var noCategoriesAdded = true;
            if (lbBase.GetSelectedIndices().Length == 0) lbBase.ToogleItems();

            foreach (var index in lbBase.GetSelectedIndices())
            {
                var data = ReportFactory.IdleTimesDAO.GetAllMovilesStoppedInPlanta(Convert.ToInt32((string) lbBase.Items[index].Value),
                                                                                   iniDate, finDate ,chkUndefined.Checked);

                var dataset = new FusionChartsDataset { Name = lbBase.Items[index].Text };
                dataset.SetPropertyValue("color", HexColorUtil.ColorToHex(colorGen.GetNextColor()));

                foreach (var item in data)
                {
                    if (noCategoriesAdded) categories.Add(item.Date.ToShortTimeString());
                    dataset.addValue(item.TotalVehicles.ToString(CultureInfo.InvariantCulture));
                }

                datasets.Add(dataset);
                noCategoriesAdded = false;
            }

            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            var hasta =
                dtpDate.SelectedDate.GetValueOrDefault().AddHours(23).AddMinutes(59).AddSeconds(59);

            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), GetSelectedBasesDescription()},
                           {CultureManager.GetLabel("DESDE"),  dtpDate.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dtpDate.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), hasta.ToShortDateString() + " " + hasta.ToShortTimeString()}
                       };
        }

        private string GetSelectedBasesDescription()
        {
            if (lbBase.GetSelectedIndices().Length == 0) lbBase.ToogleItems();
            return Enumerable.Aggregate<int, string>(lbBase.GetSelectedIndices(), string.Empty, (current, i) => String.Concat(current, lbBase.Items[i].Text, ",")).TrimEnd(','); 
        }

        private void SetGraphConfiguration(FusionChartsMultiSeriesHelper helper)
        {
            var hasta = dtpDate.SelectedDate.GetValueOrDefault().AddHours(23).AddMinutes(59).AddSeconds(59);
            helper.AddConfigEntry("caption", string.Format("{0}: {1}, Desde: {2} {3}, Hasta: {4} {5}",CultureManager.GetEntity("PARENTI02"), GetSelectedBasesDescription(),
                                                           dtpDate.SelectedDate.GetValueOrDefault().ToShortDateString(),
                                                           dtpDate.SelectedDate.GetValueOrDefault().ToShortTimeString(),
                                                           hasta.ToShortDateString(), hasta.ToShortTimeString()));
            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "0");
            helper.AddConfigEntry("numberSuffix", " vehiculos");
            helper.AddConfigEntry("limitsDecimalPrecision", "0");
            helper.AddConfigEntry("hoverCapSepChar", "-");
            helper.AddConfigEntry("showNames", "1");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("rotateNames", "1");
        }
    }
}
