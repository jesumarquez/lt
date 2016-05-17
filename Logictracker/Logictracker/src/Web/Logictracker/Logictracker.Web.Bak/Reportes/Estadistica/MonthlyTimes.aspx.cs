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
    public partial class Reportes_MontlyTimes : SecuredGraphReportPage<MobileTime>
    {
        private const string Dia = "DIA";
        private const string Horas = "HORAS";
        private const string MovilDesdeHasta = "MOVIL_DESDE_HASTA";
    
        protected override string VariableName { get { return "STAT_REP_HORAS"; } }
        protected override string GetRefference() { return "MONTH_TIMERS"; }
        protected override string XAxisLabel { get { return CultureManager.GetLabel(Dia); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Horas); } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override bool ExcelButton { get { return true; } }

        #region Private Properties

        private int Mobile
        {
            get
            {
                if (ViewState["Mobile"] == null)
                {
                    ViewState["Mobile"] = Session["Mobile"];
                    Session["Mobile"] = null;
                }
                return (ViewState["Mobile"] != null) ? Convert.ToInt32(ViewState["Mobile"]) : 0;
            }
            set { ViewState["Mobile"] = value; }
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

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the list of report objects.
        /// </summary>
        /// <returns></returns>
        protected override List<MobileTime> GetResults()
        {
            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            return ReportFactory.MonthlyTimesDAO.GetMobileTimes(ddlMovil.Selected, desde, hasta);
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

        /// <summary>
        /// Gets the graph XML file.
        /// </summary>
        /// <returns></returns>
        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                var hs = ReportObjectsList.Sum(d => d.ElapsedTime);
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(MovilDesdeHasta),
                                                               ddlMovil.SelectedItem.Text.Replace('&', 'y'), 
                                                               dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(),
                                                               dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString())
                                                 + TimeSpan.FromSeconds(hs).TotalHours.ToString(" - 0.00 hs")
                    );

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("numberSuffix", "hs");
                helper.AddConfigEntry("limitsDecimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", "-");
                helper.AddConfigEntry("rotateNames", "1");

                foreach (var timer in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    if (!timer.Fecha.DayOfWeek.Equals(DayOfWeek.Saturday) & !timer.Fecha.DayOfWeek.Equals(DayOfWeek.Sunday))
                        item.AddPropertyValue("color", "008ED6");

                    item.AddPropertyValue("name", String.Format("{0:dd/MM}",timer.Fecha));
                    item.AddPropertyValue("value", (timer.ElapsedTime / 3600.0).ToString(CultureInfo.InvariantCulture));
                    item.AddPropertyValue("hoverText", string.Concat(TimeSpan.FromSeconds(timer.ElapsedTime).ToString(), " "));

                    item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                        "n-{0}Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={1}&TypeMobile={2}&Movil={3}&InitialDate={4}&FinalDate={5}&Empresa={6}", ApplicationPath,
                        ddlPlanta.Selected, ddlTipoVehiculo.Selected, ddlMovil.Selected, timer.Fecha.ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture),
                        timer.Fecha.Add(new TimeSpan(23, 59, 59)).ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture), ddlDistrito.Selected)));

                    helper.AddItem(item);
                }
                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = ddlMovil.SelectedItem.Text };
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(String.Format("{0:dd/MM}", t.Fecha));
                dataset.addValue((t.ElapsedTime / 3600.0).ToString(CultureInfo.InvariantCulture));
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => string.Format("{0:dd/MM}", t.Fecha),
                                                  t => (t.ElapsedTime/3600).ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI03"), ddlMovil.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Size.Tick = OnTick;

            if (!IsPostBack) Size.EnableTick = true;
        }

        #endregion

        #region Private Methods

        private void OnTick(object sender, EventArgs e)
        {
            if (Mobile <= 0) return;

            BtnSearchClick(sender, e);

            UpdatePanelGraph.Update();
        }

        /// <summary>
        /// Sets up initial filter values according to how the page was called.
        /// </summary>
        private void SetInitialFilterValues()
        {
            if (IsPostBack) return;

            GetQueryStringParameters();

            if (Mobile <= 0) return;
        
            var coche = DAOFactory.CocheDAO.FindById(Mobile);

            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
            ddlDistrito.SetSelectedValue(coche.Empresa != null ? coche.Empresa.Id : -1);
            ddlPlanta.SetSelectedValue(coche.Linea != null ? coche.Linea.Id : -1);
            ddlTipoVehiculo.SetSelectedValue(coche.TipoCoche != null ? coche.TipoCoche.Id : -1);
            ddlMovil.SetSelectedValue(Mobile);
        }

        /// <summary>
        /// Get filter initial values from query string.
        /// </summary>
        private void GetQueryStringParameters()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Movil"])) Mobile = Convert.ToInt32(Request.QueryString["Movil"]);

            if (!string.IsNullOrEmpty(Request.QueryString["InitialDate"]))
                InitialDate = Convert.ToDateTime(Request.QueryString["InitialDate"], CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(Request.QueryString["FinalDate"]))
                FinalDate = Convert.ToDateTime(Request.QueryString["FinalDate"], CultureInfo.InvariantCulture);
        }

        #endregion

    }
}
