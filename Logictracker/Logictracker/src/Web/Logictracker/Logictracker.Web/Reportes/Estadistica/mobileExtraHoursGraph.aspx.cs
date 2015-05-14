#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesEstadisticaMobileExtraHoursGraph : SecuredGraphReportPage<MobileExtraHours>
    {
        #region Protected Properties

        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }

        protected override string VariableName { get { return "EST_REP_HORAS_EXTRA_MOVIL_GRAPH"; } }

        protected override string XAxisLabel { get { return CultureManager.GetEntity("PARENTI03"); } }

        protected override string YAxisLabel { get { return CultureManager.GetLabel("HORAS"); } }

        protected override string GetRefference() { return "MOV_EXTRA_HOURS_GRAPH"; }

        #endregion

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dtpDesde.SetDate();
            dtpHasta.SetDate();
        }

        protected override List<MobileExtraHours> GetResults()
        {
            ToogleItems(lbMovil);

            var iniDate = dtpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var finDate = dtpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            return ReportFactory.MobileExtraHoursDAO.GetMobilesExtraHours(lbMovil.SelectedValues, iniDate, finDate);
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                InitializeGraph(helper);

                for (var i = 0; i < ReportObjectsList.Count; i++)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("name", ReportObjectsList[i].Interno.Replace('&', 'y'));
                    item.AddPropertyValue("hoverText", ReportObjectsList[i].Interno.Replace('&', 'y'));
                    item.AddPropertyValue("value", HoursValue(i));

                    helper.AddItem(item);
                }
                return helper.BuildXml();
            }
        }
    
        #endregion

        #region Private Methods

        /// <summary>
        /// Formats the value to show it on the graph
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private string HoursValue(int i) { return ReportObjectsList[i].ExtraHours.ToString().Replace(',', '.'); }

        /// <summary>
        /// Graph initialization.
        /// </summary>
        /// <param name="helper"></param>
        private void InitializeGraph(FusionChartsHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("REP_HORAS_EXTRA_GRAPH"), dtpDesde.SelectedDate, dtpHasta.SelectedDate));

            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("numberSuffix", "Hs");
            helper.AddConfigEntry("hoverCapSepChar", "-");
            helper.AddConfigEntry("rotateNames", "1");
        }

        #endregion

        #region CSV and Print Methods

        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var data = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            for (var i = 0; i < ReportObjectsList.Count; i++)
            {
                categories.Add(ReportObjectsList[i].Interno);
                data.addValue(HoursValue(i));
            }

            datasets.Add(data);
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
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                                {CultureManager.GetLabel("DESDE"), dtpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dtpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                                {CultureManager.GetLabel("HASTA"), dtpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dtpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                            };
        }
        #endregion
    }
}