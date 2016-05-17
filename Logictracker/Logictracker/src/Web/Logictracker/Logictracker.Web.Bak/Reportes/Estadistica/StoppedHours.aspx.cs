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
    public partial class ReportesEstadisticaStoppedHours : SecuredGraphReportPage<StoppedHours>
    {
        protected override string VariableName { get { return "EST_STOPPED_HS"; } }
        protected override string GetRefference() { return "EST_STOPPED_HS"; }
        protected override GraphTypes GraphType { get { return GraphTypes.StackedColumn; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("FECHA"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("HORA"); } }
        protected override bool ExcelButton { get { return true; } }

        protected override List<StoppedHours> GetResults()
        {
            var iniDate = SecurityExtensions.ToDataBaseDateTime(dtpDesde.SelectedDate.Value);
            var finDate = SecurityExtensions.ToDataBaseDateTime(dtpHasta.SelectedDate.Value);

            return ReportFactory.StoppedHoursDAO.GetVehicleStoppedTimeGroupedByInterval(ddlMovil.Selected, iniDate, finDate, chkUndefined.Checked);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dtpDesde.SetDate();
            dtpHasta.SetDate();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                SetGraphConfiguration(helper);

                var hoursInShift = new FusionChartsDataset{Name = CultureManager.GetLabel("HS_EN_TURNO")};
                hoursInShift.SetPropertyValue("seriesName", CultureManager.GetLabel("HS_EN_TURNO"));

                var hoursOutOfShift = new FusionChartsDataset{Name = CultureManager.GetLabel("HS_FUERA_DE_TURNO")};
                hoursOutOfShift.SetPropertyValue("seriesName", CultureManager.GetLabel("HS_FUERA_DE_TURNO"));
                hoursOutOfShift.SetPropertyValue("color", "1E90FF");

                if (!ReportObjectsList.Any(d => !d.HoursInShift.Equals(0) || !d.HoursOutOfShift.Equals(0))) return helper.BuildXml();

                foreach(var item in ReportObjectsList)
                {
                    helper.AddCategory(item.Date.ToShortTimeString());
                    hoursInShift.addValue(item.HoursInShift.ToString(CultureInfo.InvariantCulture));
                    hoursOutOfShift.addValue(item.HoursOutOfShift.ToString(CultureInfo.InvariantCulture));
                }

                helper.AddDataSet(hoursInShift);
                helper.AddDataSet(hoursOutOfShift);
                return helper.BuildXml();
            }
        }
        
        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();

            var hoursInShift = new FusionChartsDataset { Name = CultureManager.GetLabel("HS_EN_TURNO") };
            var hoursOutOfShift = new FusionChartsDataset { Name = CultureManager.GetLabel("HS_FUERA_DE_TURNO") };

            foreach (var item in ReportObjectsList)
            {
                hoursInShift.addValue(item.HoursInShift.ToString(CultureInfo.InvariantCulture));
                hoursOutOfShift.addValue(item.HoursOutOfShift.ToString(CultureInfo.InvariantCulture));
            }

            var categories = ReportObjectsList.Select(nivel => String.Format("{0} - {1}", nivel.Date.ToShortDateString(), nivel.Date.ToShortTimeString())).ToList();
            datasets.Add(hoursOutOfShift);
            datasets.Add(hoursInShift);

            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.Date.ToString("dd/MM/yyyy HH:mm"),
                                                  t => (t.HoursInShift + t.HoursOutOfShift).ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI03"), ddlMovil.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dtpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dtpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dtpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dtpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        private void SetGraphConfiguration(FusionChartsMultiSeriesHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format(String.Format("Vehículo - {0} , Desde: {1} {2} , Hasta: {3} {4}", ddlMovil.SelectedItem.Text,
                                                                         dtpDesde.SelectedDate.Value.ToShortDateString(),dtpDesde.SelectedDate.Value.ToShortTimeString(),
                                                                         dtpHasta.SelectedDate.Value.ToShortDateString(), dtpHasta.SelectedDate.Value.ToShortTimeString())));
            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "2");
            helper.AddConfigEntry("numberSuffix", "hs");
            helper.AddConfigEntry("limitsDecimalPrecision", "2");
            helper.AddConfigEntry("hoverCapSepChar", "-");
            helper.AddConfigEntry("showNames", "1");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("rotateNames", "1");
        }
    }
}
