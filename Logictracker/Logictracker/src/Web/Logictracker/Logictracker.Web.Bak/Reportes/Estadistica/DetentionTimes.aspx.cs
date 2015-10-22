using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesEstadisticaDetentionTimes : SecuredGraphReportPage<DetentionTimes>
    {
        protected override string VariableName { get { return "HORAS_DENTENCION"; } }
        protected override string GetRefference() { return "HORAS_DENTENCION"; }
        protected override GraphTypes GraphType { get { return GraphTypes.StackedColumn; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("DATE"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("HORAS"); } }
        protected override bool PrintButton{ get { return false; } }
        protected override bool ExcelButton { get { return true; } }
        
        protected override List<DetentionTimes> GetResults()
        {
            return ReportFactory.DetentionTimesDAO.GetDetentionTimes(Convert.ToInt32((string) ddlMovil.SelectedValue), SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()),
                                                                     SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()));
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                if (ReportObjectsList.Count <= 0)
                    throw new Exception("No se encontraron datos asociados a los filtros seleccionados!");

                if (!ReportObjectsList.Any(r => r.HsOff > 0 || r.HsOn > 0 || r.HsTurnoOff > 0 || r.HsTurnoOn > 0))
                    return helper.BuildXml();

                SetGraphProperties(helper);

                var hsTurnoOn = new FusionChartsDataset();
                hsTurnoOn.SetPropertyValue("color", "2E8B57");
                hsTurnoOn.SetPropertyValue("seriesName", CultureManager.GetLabel("HS_TURNO_ON"));

                var hsTurnoOff = new FusionChartsDataset();
                hsTurnoOff.SetPropertyValue("color", "FFD700");
                hsTurnoOff.SetPropertyValue("seriesName", CultureManager.GetLabel("HS_TURNO_OFF"));

                var hsOn = new FusionChartsDataset();
                hsOn.SetPropertyValue("color", "4876FF");
                hsOn.SetPropertyValue("seriesName", CultureManager.GetLabel("HS_ON"));

                var hsOff = new FusionChartsDataset();
                hsOff.SetPropertyValue("color", "B22222");
                hsOff.SetPropertyValue("seriesName", CultureManager.GetLabel("HS_OFF"));

                foreach (var result in ReportObjectsList)
                {
                    helper.AddCategory(String.Format("{0}", result.Fecha.ToShortDateString()));
                    hsTurnoOn.addValue(result.HsTurnoOn.ToString(CultureInfo.InvariantCulture));
                    hsTurnoOff.addValue(result.HsTurnoOff.ToString(CultureInfo.InvariantCulture));
                    hsOn.addValue(result.HsOn.ToString(CultureInfo.InvariantCulture));
                    hsOff.addValue(result.HsOff.ToString(CultureInfo.InvariantCulture));
                }
                helper.AddDataSet(hsTurnoOn);
                helper.AddDataSet(hsTurnoOff);
                helper.AddDataSet(hsOn);
                helper.AddDataSet(hsOff);

                return helper.BuildXml();
            }
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.Fecha.ToShortDateString(),
                                                  t => (t.HsOff + t.HsOn + t.HsTurnoOff + t.HsTurnoOn).ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI03"), ddlMovil.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var hsTurnoOn = new FusionChartsDataset { Name = CultureManager.GetLabel("HS_TURNO_ON") };
            var hsTurnoOff = new FusionChartsDataset { Name = CultureManager.GetLabel("HS_TURNO_OFF") };
            var hsOn = new FusionChartsDataset { Name = CultureManager.GetLabel("HS_ON") };
            var hsOff = new FusionChartsDataset { Name = CultureManager.GetLabel("HS_OFF") };
            var categories = new List<string>();

            foreach (var r in ReportObjectsList)
            {
                categories.Add(r.Fecha.ToShortDateString());
                hsTurnoOn.addValue(r.HsTurnoOn.ToString(CultureInfo.InvariantCulture));
                hsTurnoOff.addValue(r.HsTurnoOff.ToString(CultureInfo.InvariantCulture));
                hsOn.addValue(r.HsOn.ToString(CultureInfo.InvariantCulture));
                hsOff.addValue(r.HsOff.ToString(CultureInfo.InvariantCulture));
            }
            datasets.Add(hsTurnoOn);
            datasets.Add(hsTurnoOff);
            datasets.Add(hsOn);
            datasets.Add(hsOff);
            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        private void SetGraphProperties(FusionChartsMultiSeriesHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format("Horas en detención del Vehículo {2}, Desde: {0} Hasta: {1}",
                                                           dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), ddlMovil.SelectedItem.Text));

            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "2");
            helper.AddConfigEntry("thousandSeparator", ",");
            helper.AddConfigEntry("formatNumberScale", "0");
            helper.AddConfigEntry("numberSuffix", "hs");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("showNames", "1");
            helper.AddConfigEntry("rotateNames", "1");
        }
    }
}
