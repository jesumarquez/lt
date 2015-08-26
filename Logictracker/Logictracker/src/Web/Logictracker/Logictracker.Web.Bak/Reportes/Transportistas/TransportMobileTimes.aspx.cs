using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Transportistas
{
    public partial class TransportMobileTimes : SecuredGraphReportPage<MobilesTime>
    {
        private const string DesdeHasta = "DESDE_HASTA";
        private const string Horas = "HORAS";
        private const string Parenti03 = "PARENTI03";

        protected override string GetRefference() { return "TRANSPORT_MOBILE_TIMES"; }
        protected override string VariableName { get { return "TRAN_HORAS_ACUMULADAS"; } }
        protected override bool ExcelButton { get { return true; } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetEntity(Parenti03); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Horas); } }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(DesdeHasta),
                    dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString()));

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("numberSuffix", "hs");
                helper.AddConfigEntry("hoverCapSepChar", " - ");
                helper.AddConfigEntry("rotateNames", "1");

                foreach (var time in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("color", "AFD8F8");

                    item.AddPropertyValue("name", time.Intern.Replace('&','y'));

                    item.AddPropertyValue("value", time.ElapsedTime.ToString(CultureInfo.InvariantCulture));

                    var hours = TimeSpan.FromHours(time.ElapsedTime);

                    item.AddPropertyValue("hoverText", string.Concat(time.Intern.Replace('&', 'y'), string.Format(" - {0}dd {1}hh {2}mm {3}ss", hours.Days, hours.Hours, hours.Minutes, hours.Seconds)));

                    helper.AddItem(item);
                }
                return helper.BuildXml();
            }
        }

        protected override List<MobilesTime> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            return ReportFactory.MobilesTimeDAO.GetMobilesTime(desde, hasta, GetMobilesIds());
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(time => time.Intern, 
                                                  time => time.ElapsedTime.ToString(CultureInfo.InvariantCulture));
        }

        private List<Int32> GetMobilesIds()
        {
            var ids = new List<int>();

            var mobiles = DAOFactory.CocheDAO.GetList(ddlDistrito.SelectedValues, ddlBase.SelectedValues, new []{-1}, ddlTransportista.SelectedValues);

            if (mobiles == null) return ids;

            ids.AddRange(from Coche mobile in mobiles select mobile.Id);

            return ids;
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var data = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            foreach (var time in ReportObjectsList)
            {
                categories.Add(time.Intern);
                data.addValue(time.ElapsedTime.ToString(CultureInfo.InvariantCulture));
            }

            datasets.Add(data);
            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI07"), ddlTransportista.SelectedItem.Text},
                                {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                                {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                            };
        }
    }
}
