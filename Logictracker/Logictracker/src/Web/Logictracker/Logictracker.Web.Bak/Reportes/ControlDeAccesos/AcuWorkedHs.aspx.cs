using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.ControlDeAccesos
{
    public partial class Reportes_ControlDeAccesos_AcuWorkedHs : SecuredGraphReportPage<AcumulatedHours>
    {
        private const string DesdeHasta = "DESDE_HASTA";
        private const string Horas = "HORAS";
        private const string Parenti09 = "PARENTI09";

        protected override string VariableName { get { return "STAT_REP_AC_HORAS_ACUM"; } }
        protected override string GetRefference() { return "EMPLOYEES_TIME"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetEntity(Parenti09); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Horas); } }
        protected override bool ExcelButton { get { return true; } }

        #region Protected Methods

        protected override List<AcumulatedHours> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            ToogleItems(lbEmpleados);

            return ReportFactory.AcumulatedHoursDAO.GetAcumulatedHours(GetSelectedEmployees(), desde, hasta);
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(DesdeHasta), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString()));

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("numberSuffix", "hs");
                helper.AddConfigEntry("hoverCapSepChar", " - ");
                helper.AddConfigEntry("rotateNames", "1");

                if (ReportObjectsList.Select(r=>r.ElapsedTime).Sum() == 0)
                    helper.AddConfigEntry("yAxisMaxValue", "24");

                foreach (var time in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                        "n-{0}Reportes/ControlDeAccesos/WorkedHours.aspx?Emp={1}&InitialDate={2}&FinalDate={3}", ApplicationPath,
                        time.IdEmpleado, dpDesde.SelectedDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture),
                        dpHasta.SelectedDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture))));

                    item.AddPropertyValue("color", "AFD8F8");

                    item.AddPropertyValue("name", time.Empleado);

                    item.AddPropertyValue("value", time.ElapsedTime.ToString(CultureInfo.InvariantCulture));

                    var hours = TimeSpan.FromHours(time.ElapsedTime);

                    item.AddPropertyValue("hoverText", string.Concat(time.Empleado, string.Format(" - {0}dd {1}hh {2}mm {3}ss", hours.Days,
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
                categories.Add(t.Empleado);
                dataset.addValue(t.ElapsedTime.ToString(CultureInfo.InvariantCulture));
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.Empleado,
                                                  t => t.ElapsedTime.ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {"PARENTI01", ddlDistrito.SelectedItem.Text},
                           {"PARENTI02", ddlPlanta.SelectedItem.Text},
                           {"PARENTI43", ddlTipoEmpleado.SelectedItem.Text},
                           {"DESDE", dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {"HASTA", dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        #endregion

        private List<int> GetSelectedEmployees()
        {
            if (lbEmpleados.SelectedValues.Contains(0)) lbEmpleados.ToogleItems();

            return lbEmpleados.SelectedValues;
        }
    }
}
