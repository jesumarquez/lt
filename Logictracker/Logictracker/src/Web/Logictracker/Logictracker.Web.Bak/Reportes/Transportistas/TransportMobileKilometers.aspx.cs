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
    public partial class ReportesTransportistasTransportMobileKilometers : SecuredGraphReportPage<MobilesKilometers>
    {
        #region Private Const Properties

        /// <summary>
        /// Report title variable name.
        /// </summary>
        private const string DesdeHasta = "DESDE_HASTA";

        /// <summary>
        /// Kilometers variable name.
        /// </summary>
        private const string Kilometros = "KILOMETROS";

        /// <summary>
        /// Mobile variable name.
        /// </summary>
        private const string Parenti03 = "PARENTI03";

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "TRAN_KILOMETROS_ACUMULADOS"; } }
        protected override string GetRefference() { return "TRANSPORT_MOBILE_KILOMETERS"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override bool ExcelButton { get { return true; } }
        protected override string XAxisLabel { get { return CultureManager.GetEntity(Parenti03); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Kilometros); } }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<MobilesKilometers> GetResults()
        {
            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            return ReportFactory.MobilesKilometersDAO.GetMobilesKilometers(desde, hasta, GetMobilesIds(), false);
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
                helper.AddConfigEntry("numberSuffix", "km");
                helper.AddConfigEntry("hoverCapSepChar", " - ");
                helper.AddConfigEntry("rotateNames", "1");

                foreach (var kilometer in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("color", "AFD8F8");

                    item.AddPropertyValue("name", kilometer.Interno.Replace('&', 'y'));
                    item.AddPropertyValue("value", kilometer.Kilometers.ToString(CultureInfo.InvariantCulture));
                    item.AddPropertyValue("hoverText", kilometer.Interno.Replace('&', 'y'));

                    helper.AddItem(item);
                }
                return helper.BuildXml();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets all the ids of the selected transport company.
        /// </summary>
        /// <returns></returns>
        private List<Int32> GetMobilesIds()
        {
            var ids = new List<int>();

            var mobiles = DAOFactory.CocheDAO.GetList(ddlDistrito.SelectedValues, ddlBase.SelectedValues, new[]{-1}, ddlTransportista.SelectedValues);

            if (mobiles == null) return ids;

            ids.AddRange(from Coche mobile in mobiles select mobile.Id);

            return ids;
        }

        #endregion

        #region CSV and Print Methods

        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var data = new FusionChartsDataset();
            var categories = new List<string>();

            foreach (var kilometer in ReportObjectsList)
            {
                categories.Add(kilometer.Interno);
                data.addValue(kilometer.Kilometers.ToString(CultureInfo.InvariantCulture));
            }
            datasets.Add(data);
            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(kilometer => kilometer.Interno,
                                                  kilometer => kilometer.Kilometers.ToString(CultureInfo.InvariantCulture));
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

        #endregion
    }
}
