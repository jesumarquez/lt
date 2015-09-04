using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using C1.Web.UI.Controls.C1Gauge;

namespace Logictracker.Reportes.Estadistica
{
    public partial class EstadisticaMobileRoutes : SecuredGridReportPage<MobileRoutesVo>
    {
        public override int PageSize { get { return 15; } }
        protected override string VariableName { get { return "STAT_RESUMEN_RUTA"; } }
        protected override string GetRefference() { return "MOBILE_ROUTES"; }
        protected override bool ExcelButton { get { return true; } }

        protected void DdlDistritoInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Location > 0) ddlDistrito.EditValue = Location; }
        protected void DdlBaseInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Company > 0) ddlBase.EditValue = Company; }
        protected void DdlTipoVehiculoInitialBinding(object sender, EventArgs e) { if (!IsPostBack && MobileType > 0) ddlTipoVehiculo.EditValue = MobileType; }
        protected void DdlVehiculoInitialBinding(object sender, EventArgs e) { if (!IsPostBack && Mobile > 0) ddlVehiculo.EditValue = Mobile; }

        #region Private Properties

        private int Location
        {
            get
            {
                if (ViewState["RouteLocation"] == null)
                {
                    ViewState["RouteLocation"] = Session["RouteLocation"];

                    Session["RouteLocation"] = null;
                }

                return ViewState["RouteLocation"] != null ? Convert.ToInt32(ViewState["RouteLocation"]) : 0;
            }
        }

        private int Company
        {
            get
            {
                if (ViewState["RouteCompany"] == null)
                {
                    ViewState["RouteCompany"] = Session["RouteCompany"];

                    Session["RouteCompany"] = null;
                }

                return ViewState["RouteCompany"] != null ? Convert.ToInt32(ViewState["RouteCompany"]) : 0;
            }
        }

        private int MobileType
        {
            get
            {
                if (ViewState["RouteMobileType"] == null)
                {
                    ViewState["RouteMobileType"] = Session["RouteMobileType"];

                    Session["RouteMobileType"] = null;
                }

                return ViewState["RouteMobileType"] != null ? Convert.ToInt32(ViewState["RouteMobileType"]) : 0;
            }
        }

        private int Mobile
        {
            get
            {
                if (ViewState["RouteMobile"] == null)
                {
                    ViewState["RouteMobile"] = Session["RouteMobile"];

                    Session["RouteMobile"] = null;
                }

                return ViewState["RouteMobile"] != null ? Convert.ToInt32(ViewState["RouteMobile"]) : 0;
            }
        }

        private DateTime InitialDate
        {
            get
            {
                if (ViewState["RouteInitialDate"] == null)
                {
                    ViewState["RouteInitialDate"] = Session["RouteInitialDate"];

                    Session["RouteInitialDate"] = null;
                }

                return (DateTime)ViewState["RouteInitialDate"];
            }
        }

        private DateTime FinalDate
        {
            get
            {
                if (ViewState["RouteFinalDate"] == null)
                {
                    ViewState["RouteFinalDate"] = Session["RouteFinalDate"];

                    Session["RouteFinalDate"] = null;
                }

                return (DateTime)ViewState["RouteFinalDate"];
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            gaugeAverageSpeed.AbsoluteExpiration = gaugeMaxSpeed.AbsoluteExpiration = gaugeMinSpeed.AbsoluteExpiration =
                                                                                      gaugeTotalKm.AbsoluteExpiration =
                                                                                      DateTime.Today.AddDays(-1);
            gaugeAverageSpeed.SlidingExpiration = gaugeMaxSpeed.SlidingExpiration = gaugeMinSpeed.SlidingExpiration =
                                                                                    gaugeTotalKm.SlidingExpiration =
                                                                                    new TimeSpan(0, 0, 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();

                if (!Mobile.Equals(0))
                {
                    SetInitialFilterValues();
                    Bind();
                }
            }
        }

        protected override void OnPrePrint()
        {
            if (Grid.SelectedRow != null)
            {
                ShowEvents();
                lblEventosPrint.Visible = true;
                DisplayEventsTitle();
                ifEventsPrint.Visible = true;
            }

            Session["KeepInSession"] = true;
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            base.BtnSearchClick(sender, e);

            var showResults = ReportObjectsList.Count > 0;

            SetPageLayout(showResults);
        }

        protected override List<MobileRoutesVo> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            var routes = ReportFactory.MobileRoutesDAO.GetMobileRoutes(ddlVehiculo.Selected, desde, hasta);

            var showResults = routes != null && routes.Count > 0;

            if (showResults) DisplayTotalizers(routes);

            return MergeResults(routes).Select(r => new MobileRoutesVo(r, chkDirecciones.Checked)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileRoutesVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileRoutesVo.IndexKilometers).Text = string.Format("{0:0.00}km", dataItem.Kilometers);

            e.Row.BackColor = GetBackColor(dataItem);

            grid.Columns[MobileRoutesVo.IndexInfractions].Visible = grid.Columns[MobileRoutesVo.IndexInfractionsDuration].Visible = !chkCombustible.Checked;
            grid.Columns[MobileRoutesVo.IndexConsumo].Visible = grid.Columns[MobileRoutesVo.IndexHsMarcha].Visible = chkCombustible.Checked;
        }

        protected override void SelectedIndexChanged()
        {
            ShowEvents();
            DisplayEventsTitle();
        }

        protected override void AddToolBarIcons()
        {
            base.AddToolBarIcons();

            ToolBar.AddMapToolbarButton();
        }

        protected override void ToolbarItemCommand(object sender, CommandEventArgs e)
        {
            base.ToolbarItemCommand(sender, e);

            if (e.CommandName.Equals("View")) ShowRoute();
        }

        protected void LnkEventosClick(object sender, EventArgs e)
        {
            var desde = DateTime.Parse(lnkEventos.Attributes["fechaDesde"]);
            var hasta = DateTime.Parse(lnkEventos.Attributes["fechaHasta"]);

            Session.Remove("Distrito");
            Session.Remove("Location");
            Session.Remove("TypeMobile");
            Session.Remove("Mobile");
            Session.Remove("InitialDate");
            Session.Remove("FinalDate");
            Session.Remove("ShowMessages");
            Session.Remove("ShowPOIS");

            Session.Add("Distrito", ddlDistrito.Selected);
            Session.Add("Location", ddlBase.Selected);
            Session.Add("TypeMobile", ddlTipoVehiculo.Selected);
            Session.Add("Mobile", ddlVehiculo.Selected);
            Session.Add("InitialDate", desde);
            Session.Add("FinalDate", hasta);
            Session.Add("ShowMessages", 0);
            Session.Add("ShowPOIS", 0);            

            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }
        
        protected void LnkStopClick(object sender, EventArgs e)
        {
            ShowEvents("100");
            DisplayEventsTitle("100");
        }

        protected void LnkInfractionClick(object sender, EventArgs e)
        {
            ShowEvents("92");
            DisplayEventsTitle("92");
        }
        
        #endregion

        #region Private Methods

        private void SetInitialFilterValues()
        {
            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
        }

        private static Color GetBackColor(MobileRoutesVo mobileRoutes)
        {
            var color = Color.LightGray;

            if (mobileRoutes != null)
            {
                switch (mobileRoutes.VehicleStatus)
                {
                    case "Detenido": 
                        color = Color.LightCoral; 
                        break;
                    case "En Movimiento": 
                        color = Color.LightGreen; 
                        break;
                }
            }

            return color;
        }

        private IEnumerable<Logictracker.Types.ReportObjects.MobileRoutes> MergeResults(ICollection<Logictracker.Types.ReportObjects.MobileRoutes> routes)
        {
            var filteredRoutes = FilterResults(routes);

            for (var i = 1; i < filteredRoutes.Count; i++)
            {
                if (filteredRoutes[i - 1].EqualState(filteredRoutes[i]))
                {
                    MergeRouteFragments(filteredRoutes[i - 1], filteredRoutes[i]);
                    filteredRoutes.RemoveAt(i);
                    i--;
                }
            }

            return filteredRoutes;
        }

        private static void MergeRouteFragments(Logictracker.Types.ReportObjects.MobileRoutes pastFragment, Logictracker.Types.ReportObjects.MobileRoutes currentFragment)
        {
            pastFragment.AverageSpeed = pastFragment.AverageSpeed >= currentFragment.AverageSpeed ? pastFragment.AverageSpeed : currentFragment.AverageSpeed;
            pastFragment.Duration += currentFragment.Duration;
            pastFragment.FinalTime = currentFragment.FinalTime;
            pastFragment.InfractionsDuration += currentFragment.InfractionsDuration;
            pastFragment.Infractions += currentFragment.Infractions;
            pastFragment.Kilometers += currentFragment.Kilometers;
            pastFragment.MaxSpeed = pastFragment.MaxSpeed >= currentFragment.MaxSpeed ? pastFragment.MaxSpeed : currentFragment.MaxSpeed;
            pastFragment.MinSpeed = pastFragment.MinSpeed <= currentFragment.MinSpeed ? pastFragment.MinSpeed : currentFragment.MinSpeed;
            pastFragment.Consumo += currentFragment.Consumo;
            pastFragment.HsMarcha += currentFragment.HsMarcha;
        }

        private List<Logictracker.Types.ReportObjects.MobileRoutes> FilterResults(ICollection<Logictracker.Types.ReportObjects.MobileRoutes> routes)
        {
            if (routes == null || routes.Count.Equals(0))
                return new List<Logictracker.Types.ReportObjects.MobileRoutes>();

            var distance = npDistancia.Number / 1000.0;

            return (from route in routes
                    where (route.VehicleStatus.Equals("En Movimiento") && route.Kilometers > distance)
                          || (route.VehicleStatus.Equals("Detenido") && route.Duration > tpDetencion.SelectedTime.TotalHours)
                          || (route.VehicleStatus.Equals("Sin Reportar") && route.Duration > tpSinReportar.SelectedTime.TotalHours)
                    select route).ToList();
        }

        private void DisplayTotalizers(IList<Logictracker.Types.ReportObjects.MobileRoutes> routes)
        {
            var totalMovementTime = 0.0;
            var totalMovementEvents = 0;

            var totalStoppedTime = 0.0;
            
            var totalNoReportTime = 0.0;
            var totalNoReportEvents = 0;

            var totalInfractionTime = 0.0;
            var totalInfractionEvents = 0;

            var totalKilometers = 0.0;

            var minSpeed = routes[0].MinSpeed;
            var maxSpeed = routes[0].MaxSpeed;

            foreach (var route in routes)
            {
                totalKilometers += route.Kilometers;

                totalInfractionTime += route.InfractionsDuration;
                totalInfractionEvents += route.Infractions;

                if ((route.MinSpeed < minSpeed && route.MinSpeed > 0) || minSpeed == 0) minSpeed = route.MinSpeed;
                if (route.MaxSpeed > maxSpeed) maxSpeed = route.MaxSpeed;

                switch (route.VehicleStatus)
                {
                    case "En Movimiento":
                        totalMovementTime += route.Duration;
                        totalMovementEvents += 1;
                        break;
                    case "Detenido":
                        totalStoppedTime += route.Duration;
                        break;
                    default:
                        totalNoReportTime += route.Duration;
                        totalNoReportEvents += 1;
                        break;
                }
            }

            var totalStoppedEvents = GetEvents(Convert.ToDateTime((object) dpDesde.SelectedDate).ToDataBaseDateTime(), Convert.ToDateTime((object) dpHasta.SelectedDate).ToDataBaseDateTime(), "100").Count;

            var averageSpeed = totalKilometers > 0 && totalMovementTime > 0 ? totalKilometers/totalMovementTime : 0;

            lblTotalMovementTimePrint.Text = lblTotalMovementTime.Text = string.Format(CultureManager.GetLabel("TIME_AND_EVENTS"), String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(totalMovementTime).Hours, TimeSpan.FromHours(totalMovementTime).Minutes, TimeSpan.FromHours(totalMovementTime).Seconds)), totalMovementEvents);
            lblTotalStooppedTimePrint.Text = lblTotalStooppedTime.Text = String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(totalStoppedTime).Hours, TimeSpan.FromHours(totalStoppedTime).Minutes, TimeSpan.FromHours(totalStoppedTime).Seconds));
            lblTotalNoReportTimePrint.Text = lblTotalNoReportTime.Text = string.Format(CultureManager.GetLabel("TIME_AND_EVENTS"), String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(totalNoReportTime).Hours, TimeSpan.FromHours(totalNoReportTime).Minutes, TimeSpan.FromHours(totalNoReportTime).Seconds)), totalNoReportEvents);
            lblTotalInfractionTimePrint.Text = lblTotalInfractionTime.Text = String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromMinutes(totalInfractionTime).Hours, TimeSpan.FromMinutes(totalInfractionTime).Minutes, TimeSpan.FromMinutes(totalInfractionTime).Seconds));
            lblTotalKilometersPrint.Text = lblTotalKilometers.Text = string.Format("{0:0.00}km", totalKilometers);
            lblVelocidadMinimaPrint.Text = lblVelocidadMinima.Text = string.Format("{0}km/h", minSpeed);
            lblVelocidadPromedioPrint.Text = lblVelocidadPromedio.Text = string.Format("{0:0.00}km/h", averageSpeed);
            lblVelocidadMaximaPrint.Text = lblVelocidadMaxima.Text = string.Format("{0}km/h", maxSpeed);
            
            lnkStopEvents.Text = string.Format(CultureManager.GetLabel("EVENTS"), totalStoppedEvents);
            lnkInfractionEvents.Text = string.Format(CultureManager.GetLabel("INFRACTIONS"), totalInfractionEvents);
            
            var maxKm = (Convert.ToInt32(totalKilometers / 100) + 1) * 100;
            gaugeTotalKm.Gauges[gaugeTotalKm.Gauges.IndexOf("linearGaugeTotalKm")].Maximum = maxKm;
            gaugeTotalKm.Gauges[gaugeTotalKm.Gauges.IndexOf("linearGaugeTotalKm")].Pointer.Value = totalKilometers;
            
            var maxVel = (Convert.ToInt32(minSpeed / 20) + 1) * 20;
            gaugeMinSpeed.Gauges[gaugeMinSpeed.Gauges.IndexOf(("radialGaugeMinSpeed"))].Maximum = maxVel > 120 ? maxVel : 120;
            ((C1GaugeLabels)gaugeMinSpeed.Gauges[gaugeMinSpeed.Gauges.IndexOf(("radialGaugeMinSpeed"))].Decorators[3]).Interval = maxVel > 120 ? 20 : 10;
            gaugeMinSpeed.Gauges[gaugeMinSpeed.Gauges.IndexOf(("radialGaugeMinSpeed"))].Pointer.Value = minSpeed;
            
            maxVel = (Convert.ToInt32(averageSpeed / 20) + 1) * 20;
            gaugeAverageSpeed.Gauges[gaugeAverageSpeed.Gauges.IndexOf(("radialGaugeAverageSpeed"))].Maximum = maxVel > 120 ? maxVel : 120;
            ((C1GaugeLabels)gaugeAverageSpeed.Gauges[gaugeAverageSpeed.Gauges.IndexOf(("radialGaugeAverageSpeed"))].Decorators[3]).Interval = maxVel > 120 ? 20 : 10;
            gaugeAverageSpeed.Gauges[gaugeAverageSpeed.Gauges.IndexOf(("radialGaugeAverageSpeed"))].Pointer.Value = averageSpeed;

            maxVel = (Convert.ToInt32(maxSpeed / 20) + 1) * 20;
            gaugeMaxSpeed.Gauges[gaugeMaxSpeed.Gauges.IndexOf(("radialGaugeMaxSpeed"))].Maximum = maxVel > 120 ? maxVel : 120;
            ((C1GaugeLabels)gaugeMaxSpeed.Gauges[gaugeMaxSpeed.Gauges.IndexOf(("radialGaugeMaxSpeed"))].Decorators[3]).Interval = maxVel > 120 ? 20 : 10;
            gaugeMaxSpeed.Gauges[gaugeMaxSpeed.Gauges.IndexOf(("radialGaugeMaxSpeed"))].Pointer.Value = maxSpeed;
        }

        private void SetPageLayout(bool showResults)
        {
            tblSubtotalsPrint.Visible = tblSubtotals.Visible = showResults;

            lblTitle.Visible = showResults;
            ifEvents.Visible = ifEvents.Visible = false;

            lblEventosPrint.Visible = lblEventos.Visible = false;

            if (showResults)
            {
                var mobile = ddlVehiculo.SelectedItem.Text;
                var from = dpDesde.SelectedDate;
                var to = dpHasta.SelectedDate;

                lblTitle.Text = string.Format(CultureManager.GetLabel("REP_RUTA_TITLE"), mobile, from, to);
            }
        }

        private void DisplayEventsTitle()
        {
            DisplayEventsTitle(string.Empty);
        }

        private void DisplayEventsTitle(string evento)
        {
            ifEvents.Visible = true;
            
            lblEventosPrint.Visible = lblEventos.Visible = lnkEventos.Visible = true;

            var mobile = ddlVehiculo.SelectedItem.Text;

            DateTime from;
            DateTime to;

            if (evento == string.Empty)
            {   
                from = Convert.ToDateTime(GridUtils.GetCell(Grid.SelectedRow, MobileRoutesVo.IndexInitialTime).Text);
                to = Convert.ToDateTime(GridUtils.GetCell(Grid.SelectedRow, MobileRoutesVo.IndexFinalTime).Text);
            }
            else
            {
                from = Convert.ToDateTime((object) dpDesde.SelectedDate);
                to = Convert.ToDateTime((object) dpDesde.SelectedDate);
            }

            lblEventosPrint.Text = lblEventos.Text = string.Format(CultureManager.GetLabel("REP_EVENTOS_TITLE"), mobile, from, to);
            lnkEventos.Text = CultureManager.GetControl("BUTTON_MAP");
            lnkEventos.Attributes.Add("fechaDesde", from.ToString());
            lnkEventos.Attributes.Add("fechaHasta", to.ToString());
        }

        private void ShowRoute()
        {
            Session.Remove("Distrito");
            Session.Remove("Location");
            Session.Remove("TypeMobile");
            Session.Remove("Mobile");
            Session.Remove("InitialDate");
            Session.Remove("FinalDate");
            Session.Remove("ShowMessages");
            Session.Remove("ShowPOIS");

            Session.Add("Distrito", ddlDistrito.Selected);
            Session.Add("Location", ddlBase.Selected);
            Session.Add("TypeMobile", ddlTipoVehiculo.Selected);
            Session.Add("Mobile", ddlVehiculo.Selected);
            Session.Add("InitialDate", dpDesde.SelectedDate);
            Session.Add("FinalDate", dpHasta.SelectedDate);
            Session.Add("ShowMessages", 0);
            Session.Add("ShowPOIS", 0);

            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        private void ShowEvents()
        {
            ShowEvents("0");
        }

        private void ShowEvents(string evento)
        {
            DateTime from;
            DateTime to;
            
            if (evento.Equals("0"))
            {
                from = Convert.ToDateTime(GridUtils.GetCell(Grid.SelectedRow, MobileRoutesVo.IndexInitialTime).Text).ToDataBaseDateTime();
                to = Convert.ToDateTime(GridUtils.GetCell(Grid.SelectedRow, MobileRoutesVo.IndexFinalTime).Text).ToDataBaseDateTime();
            }
            else
            {
                from = Convert.ToDateTime((object) dpDesde.SelectedDate).ToDataBaseDateTime();
                to = Convert.ToDateTime((object) dpHasta.SelectedDate).ToDataBaseDateTime();
            }

            var events = GetEvents(from, to, evento);

            Session.Add("RouteFragmentEvents", events);
            Session.Add("RouteFragmentDistrict", ddlDistrito.Selected);
            Session.Add("RouteFragmentLocation", ddlBase.Selected);
            Session.Add("RouteFragmentTypeMobile", ddlTipoVehiculo.Selected);
            Session.Add("RouteFragmentMobile", ddlVehiculo.Selected);
            Session.Add("RouteFragmentDate", dpDesde.SelectedDate);
            Session.Add("RouteFragmentInitialTime", from.TimeOfDay);
            Session.Add("RouteFragmentFinalTime", to.TimeOfDay);
            upEvents.Update();
        }

        private List<MobileEvent> GetEvents(DateTime from, DateTime to, string evento)
        {   
            var mobiles = new List<int> { ddlVehiculo.Selected };

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;
            
            return ReportFactory.MobileEventDAO.GetMobilesEvents(mobiles, new List<int> { int.Parse(evento) }, new List<int> { 0 }, from, to, maxMonths);
        }
        
        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetLabel("INTERNO"), ddlVehiculo.SelectedItem.Text},
                           {CultureManager.GetLabel("DISTANCIA"), npDistancia.Number.ToString()},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()} 
                       };
        }

        #endregion
    }
}
