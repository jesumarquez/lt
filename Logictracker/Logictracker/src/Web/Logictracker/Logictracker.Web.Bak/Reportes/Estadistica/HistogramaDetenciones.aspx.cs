using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesEstadisticaHistogramaDetenciones : SecuredGraphReportPage<MobileStoppedEvent>
    {
        protected override string VariableName { get { return "EST_REP_HISTOGRAMA_DETENCIONES"; } }
        protected override string GetRefference() { return "EST_REP_HISTOGRAMA_DETENCIONES"; }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("LOCATION"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("CANTIDAD_EVENTOS"); } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override int? DefaultHeight { get {  return 800; } }
        protected override bool ExcelButton { get { return true; } }

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override List<MobileStoppedEvent> GetResults()
        {
            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            const int maxMonths = 3;

            ToogleItems(lbVehiculos);

            return ReportFactory.MobileStoppedEventDAO.GetMultipleStoppedEvents(lbVehiculos.SelectedValues,
                                                                                desde,
                                                                                hasta,
                                                                                Convert.ToInt32(npDuracion.Value),
                                                                                Convert.ToInt32(npRadio.Value),
                                                                                Convert.ToInt32(npResultados.Value),
                                                                                maxMonths);
        }

        protected override string GetGraphXml()
        {            
            using (var helper = new FusionChartsHelper())
            {
                AddConfiguration(helper);

                foreach (var stoppedEvent in ReportObjectsList) AddItem(helper, stoppedEvent);

                return helper.BuildXml();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds information about daily kilometers.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="stoppedEvent"></param>
        private void AddItem(FusionChartsHelper helper, MobileStoppedEvent stoppedEvent)
        {
            var item = new FusionChartsItem();

            item.AddPropertyValue("name", stoppedEvent.Corner);
            item.AddPropertyValue("value", stoppedEvent.StoppedEvents.ToString("#0"));
            item.AddPropertyValue("hoverText", stoppedEvent.Corner);

            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture);
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture);

            item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                "n-{0}Reportes/Estadistica/HistogramaDetenciones/HistogramaDetencionesDetalle.aspx?Vehiculos={1}&Desde={2}&Hasta={3}&Duracion={4}&Radio={5}&Latitud={6}&Longitud={7}",
                    ApplicationPath, GetMobilesIdString(), desde, hasta, Convert.ToInt32(npDuracion.Value)
                        , Convert.ToInt32(npRadio.Value), stoppedEvent.Latitude, stoppedEvent.Longitude)));

            helper.AddItem(item);
        }

        /// <summary>
        /// Adds configuration for the current report.
        /// </summary>
        /// <param name="helper"></param>
        private void AddConfiguration(FusionChartsHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("HISTOGRAMA_DETENCIONES_CAPTION"), dpDesde.SelectedDate, dpHasta.SelectedDate));
            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("hoverCapSepChar", "-");
            helper.AddConfigEntry("rotateNames", "1");
            helper.AddConfigEntry("numberSuffix", " detenciones");
            helper.AddConfigEntry("decimalPrecision", "0");
        }
        
        private IEnumerable<int> GetMobilesIds()
        {
            if (lbVehiculos.GetSelectedIndices().Length == 0) lbVehiculos.ToogleItems();

            return lbVehiculos.SelectedValues;
        }

        /// <summary>
        /// Returns a comma separated string containing the ids of the selected mobiles.
        /// </summary>
        /// <returns></returns>
        private string GetMobilesIdString()
        {
            var ids = GetMobilesIds().Aggregate("", (current, id) => String.Concat(current, id.ToString("#0"), ","));

            return ids.TrimEnd(',');
        }

        #endregion

        #region CSV and Priunt Methods

        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var data = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            foreach (var stoppedEvent in ReportObjectsList)
            {
                categories.Add(stoppedEvent.Corner);
                data.addValue(stoppedEvent.StoppedEvents.ToString("#0"));
            }

            datasets.Add(data);
            GraphCategories = categories;
            GraphDataSet = datasets;

        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(st => st.Corner,
                                                  st => st.StoppedEvents.ToString("#0"));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                                {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                                {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                            };
        }

        #endregion
    }
}