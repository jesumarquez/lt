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

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesMonthKilometers : SecuredGraphReportPage<MobileKilometer>
    {
        private const string Dia = "DIA";
        private const string Kilometros = "KILOMETROS";
        private const string MovilDesdeHasta = "MOVIL_DESDE_HASTA";
        private const string Refference = "REFFERENCE";
        
        protected override string VariableName { get { return "STAT_REP_KM"; } }
        protected override string GetRefference() { return "MONTH_KILOMETERS"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override bool ExcelButton { get { return true; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel(Dia); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Kilometros); } }

        private int Mobile
        {
            get
            {
                if (ViewState["Movil"] == null)
                {
                    ViewState["Movil"] = Session["Movil"];
                    Session["Movil"] = null;
                }
                return (ViewState["Movil"] != null) ? Convert.ToInt32(ViewState["Movil"]) : 0;
            }
            set { ViewState["Movil"] = value; }
        }
        private DateTime InitialDate
        {
            get
            {
                if (ViewState["InitialDate"] == null)
                {
                    ViewState["InitialDate"] = Session["InitialDate"];
                    Session["InitialDate"] = null;
                }
                return (ViewState["InitialDate"] != null) ? Convert.ToDateTime(ViewState["InitialDate"]) : DateTime.Today;
            }
            set { ViewState["InitialDate"] = value; }
        }
        private DateTime FinalDate
        {
            get
            {
                if (ViewState["FinalDate"] == null)
                {
                    ViewState["FinalDate"] = Session["FinalDate"];
                    Session["FinalDate"] = null;
                }
                return (ViewState["FinalDate"] != null) ? Convert.ToDateTime(ViewState["FinalDate"]) : DateTime.Today.Add(new TimeSpan(23, 59, 59));
            }
            set { ViewState["FinalDate"] = value; }
        }
        private bool SoloEnRuta
        {
            get
            {
                if (ViewState["SoloEnRuta"] == null)
                {
                    ViewState["SoloEnRuta"] = Session["SoloEnRuta"];
                    Session["SoloEnRuta"] = null;
                }
                return (ViewState["SoloEnRuta"] != null) && Convert.ToBoolean(ViewState["SoloEnRuta"]);
            }
            set { ViewState["SoloEnRuta"] = value; }
        }

        protected override List<MobileKilometer> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            return ReportFactory.MonthlyKilometersDAO.GetMobileKilometers(ddlMovil.Selected, desde, hasta, chkSoloEnRuta.Checked);
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                var vehicle = DAOFactory.CocheDAO.FindById(ddlMovil.Selected);

                AddConfiguration(helper, vehicle);

                foreach (var kilometer in ReportObjectsList) AddItem(helper, kilometer);

                AddTrendline(helper, vehicle);

                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset {Name = ddlMovil.SelectedItem.Text };
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(String.Format("{0:dd/MM}", t.Fecha));
                dataset.addValue(t.Kilometers.ToString(CultureInfo.InvariantCulture));
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => String.Format("{0:dd/MM}", t.Fecha),
                                                  t => t.Kilometers.ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<String, String> GetFilterValues()
        {
            return new Dictionary<String, String>
                       { 
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI03"), ddlMovil.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), string.Concat(dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), " ", dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString())},
                           {CultureManager.GetLabel("HASTA"), string.Concat(dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString(), " ", dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString())}
                       };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Size.Tick = OnTick;

            GetQueryStringParameters();

            if (!IsPostBack && Mobile > 0) Size.EnableTick = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }

            SetInitialFilterValues();
        }

        #region Private Methods

        /// <summary>
        /// Updates the graph when the init timer expires. Used when the report is called from a link.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            BtnSearchClick(sender, e);

            UpdatePanelGraph.Update();
        }

        /// <summary>
        /// Sets up initial filter values according to how the page was called.
        /// </summary>
        private void SetInitialFilterValues()
        {
            if (IsPostBack) return;

            if (Mobile <= 0) return;

            var coche = DAOFactory.CocheDAO.FindById(Mobile);

            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
            ddlDistrito.SetSelectedValue(coche.Empresa != null ? coche.Empresa.Id : -1);
            ddlPlanta.SetSelectedValue(coche.Linea != null ? coche.Linea.Id : -1);
            ddlTipoVehiculo.SetSelectedValue(coche.TipoCoche != null ? coche.TipoCoche.Id : -1);
            ddlMovil.SetSelectedValue(Mobile);
            chkSoloEnRuta.Checked = SoloEnRuta;
        }

        /// <summary>
        /// Get filter initial values from query string.
        /// </summary>
        private void GetQueryStringParameters()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Movil"])) Mobile = Convert.ToInt32(Request.QueryString["Movil"]);

            if (!string.IsNullOrEmpty(Request.QueryString["InitialDate"])) InitialDate = Convert.ToDateTime(Request.QueryString["InitialDate"], CultureInfo.InvariantCulture);
            
            if (!string.IsNullOrEmpty(Request.QueryString["FinalDate"])) FinalDate = Convert.ToDateTime(Request.QueryString["FinalDate"], CultureInfo.InvariantCulture);
            
            if (!string.IsNullOrEmpty(Request.QueryString["SoloEnRuta"])) SoloEnRuta = Convert.ToBoolean(Request.QueryString["SoloEnRuta"], CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Adds a daily kilometers refference trendline.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="vehicle"></param>
        private static void AddTrendline(FusionChartsHelper helper, Coche vehicle)
        {
            if (vehicle.KilometrosDiarios.Equals(0)) return;

            var trendline = new FusionChartsTrendline();

            trendline.AddPropertyValue("startValue", Convert.ToInt32(vehicle.KilometrosDiarios).ToString("#0"));
            trendline.AddPropertyValue("displayValue", string.Format("{0}: {1}km", CultureManager.GetLabel(Refference), vehicle.KilometrosDiarios));
            trendline.AddPropertyValue("color", "91C728");
            trendline.AddPropertyValue("showOnTop", "1");

            helper.AddTrendLine(trendline);
        }

        /// <summary>
        /// Adds information about daily kilometers.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="kilometer"></param>
        private void AddItem(FusionChartsHelper helper, MobileKilometer kilometer)
        {
            var item = new FusionChartsItem();

            if (!kilometer.Fecha.DayOfWeek.Equals(DayOfWeek.Saturday) & !kilometer.Fecha.DayOfWeek.Equals(DayOfWeek.Sunday)) item.AddPropertyValue("color", "008ED6");

            item.AddPropertyValue("name", String.Format("{0:dd/MM}", kilometer.Fecha));
            item.AddPropertyValue("value", kilometer.Kilometers.ToString(CultureInfo.InvariantCulture));
            item.AddPropertyValue("hoverText", string.Format("{0:dd/MM/yyyy} ", kilometer.Fecha));

            item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                "n-{0}Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={1}&TypeMobile={2}&Movil={3}&InitialDate={4}&FinalDate={5}&Empresa={6}", ApplicationPath, ddlPlanta.Selected,
                ddlTipoVehiculo.Selected, ddlMovil.Selected, kilometer.Fecha.ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture),
                kilometer.Fecha.Add(new TimeSpan(23, 59, 59)).ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture), ddlDistrito.Selected)));

            helper.AddItem(item);
        }

        /// <summary>
        /// Adds configuration for the current report.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="vehicle"></param>
        private void AddConfiguration(FusionChartsHelper helper, Coche vehicle)
        {
            var km = ReportObjectsList.Sum(d => d.Kilometers);
            helper.AddConfigEntry("caption", String.Format(CultureManager.GetLabel(MovilDesdeHasta),
                vehicle.Interno.Replace('&', 'y'), 
                dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(),
                dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString())
                + km.ToString(" - 0.00 km")
                );

            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "2");
            helper.AddConfigEntry("numberSuffix", "km");
            helper.AddConfigEntry("hoverCapSepChar", "-");
            helper.AddConfigEntry("rotateNames", "1");
        }

        #endregion
    }
}
