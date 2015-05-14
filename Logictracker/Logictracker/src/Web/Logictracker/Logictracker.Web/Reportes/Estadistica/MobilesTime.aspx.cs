using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Estadistica
{
    public partial class EstadisticaMobilesTime : SecuredGraphReportPage<MobilesTime>
    {
        private const string DesdeHasta = "DESDE_HASTA";
        private const string Horas = "HORAS";
        private const string Parenti03 = "PARENTI03";

        protected override string VariableName { get { return "STAT_REP_HORAS_ACUM"; } }
        protected override string GetRefference() { return "MOBILES_TIME"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override bool ExcelButton { get { return true; } }
        protected override string XAxisLabel { get { return CultureManager.GetEntity(Parenti03); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Horas); } }

        protected override List<MobilesTime> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());
        
            ToogleItems(lbMobiles);

            return ReportFactory.MobilesTimeDAO.GetMobilesTime(desde, hasta, GetSelectedMobiles());
        }

        protected override string GetGraphXml()
        {
            using(var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(DesdeHasta), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString()));

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("numberSuffix", "hs");
                helper.AddConfigEntry("hoverCapSepChar", " - ");
                helper.AddConfigEntry("rotateNames", "1");

                foreach (var time in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                        "n-{0}Reportes/Estadistica/MonthlyTimes.aspx?Movil={1}&InitialDate={2}&FinalDate={3}", ApplicationPath,
                        time.Movil, dpDesde.SelectedDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture),
                        dpHasta.SelectedDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture))));

                    item.AddPropertyValue("color", "AFD8F8");

                    item.AddPropertyValue("name", time.Intern.Replace('&', 'y'));

                    item.AddPropertyValue("value", time.ElapsedTime.ToString(CultureInfo.InvariantCulture));

                    var hours = TimeSpan.FromHours(time.ElapsedTime);

                    item.AddPropertyValue("hoverText", string.Concat(time.Intern.Replace('&', 'y'), string.Format(" - {0}dd {1}hh {2}mm {3}ss", hours.Days,
                                                                                                hours.Hours, hours.Minutes, hours.Seconds)));
                    helper.AddItem(item);
                }
                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(t.Intern);
                dataset.addValue(t.ElapsedTime.ToString(CultureInfo.InvariantCulture));
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.Intern,
                                                  t => t.ElapsedTime.ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        private List<int> GetSelectedMobiles()
        {
            if (lbMobiles.SelectedValues.Contains(0)) lbMobiles.ToogleItems();

            return lbMobiles.SelectedValues;
        }
    }
}
