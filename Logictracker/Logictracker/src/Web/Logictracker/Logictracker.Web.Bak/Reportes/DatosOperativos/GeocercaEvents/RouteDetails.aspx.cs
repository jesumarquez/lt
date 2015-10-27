#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Reportes.DatosOperativos.GeocercaEvents
{
    public partial class EstadisticaGeocercaEventsRouteDetails : ApplicationSecuredPage
    {
        #region Private Properties

        /// <summary>
        /// No route info message variable name.
        /// </summary>
        private const string NoRouteInfo = "NO_ROUTE_INFO";

        /// <summary>
        /// Details title format variable name.
        /// </summary>
        private const string RepGeocercasTitle = "REP_GEOCERCAS_TITLE";

        /// <summary>
        /// Infractions time and events format variable name.
        /// </summary>
        private const string TimeAndEventsInfractions = "TIME_AND_EVENTS_INFRACTIONS";

        /// <summary>
        /// Route details associated mobile.
        /// </summary>
        private int Mobile
        {
            get
            {
                if (ViewState["RouteDetailsMobile"] == null)
                {
                    ViewState["RouteDetailsMobile"] = Session["RouteDetailsMobile"];

                    if (Session["KeepInSes"] == null) { Session.Remove("RouteDetailsMobile"); }

                    Session["KeepInSes"] = null;
                }

                return ViewState["RouteDetailsMobile"] != null ? Convert.ToInt32(ViewState["RouteDetailsMobile"]) : 0;
            }
        }

        /// <summary>
        /// Route details from date time.
        /// </summary>
        private DateTime From
        {
            get
            {
                if (ViewState["RouteDetailsFrom"] == null)
                {
                    ViewState["RouteDetailsFrom"] = Session["RouteDetailsFrom"];

                    if (Session["KeepInSes"] == null) { Session.Remove("RouteDetailsFrom"); }

                    Session["KeepInSes"] = null;
                }

                return ViewState["RouteDetailsFrom"] != null ? Convert.ToDateTime(ViewState["RouteDetailsFrom"]) : DateTime.MinValue;
            }
        }

        /// <summary>
        /// Route details to date time.
        /// </summary>
        private DateTime To
        {
            get
            {
                if (ViewState["RouteDetailsTo"] == null)
                {
                    ViewState["RouteDetailsTo"] = Session["RouteDetailsTo"];

                    if (Session["KeepInSes"] == null) { Session.Remove("RouteDetailsTo"); }

                    Session["KeepInSes"] = null;
                }

                return ViewState["RouteDetailsTo"] != null ? Convert.ToDateTime(ViewState["RouteDetailsTo"]) : DateTime.MinValue;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Info messages label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return null; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Report data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var showResults = false;
            MobileActivity activity = null;

            if (!To.ToShortDateString().Equals(DateTime.UtcNow.ToShortDateString()))
            {
                activity = GetResults();

                showResults = activity != null;
            }

            if (showResults) DisplayResults(activity);
            else
            {
                infoLabel1.Mode = InfoLabelMode.INFO;

                infoLabel1.Text = CultureManager.GetSystemMessage(NoRouteInfo);
            }

            var mobile = activity != null ? activity.Movil : DAOFactory.CocheDAO.FindById(Mobile).Interno;

            lblTitle.Text = string.Format(CultureManager.GetLabel(RepGeocercasTitle), mobile);

            tblResults.Visible = showResults;

            lnkResumenDeRuta.Enabled = showResults;
        }

        /// <summary>
        /// Displays the selected route fragment in the historic monitor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LnkMonitorHistoricoClick(object sender, EventArgs e)
        {
            var mobile = DAOFactory.CocheDAO.FindById(Mobile);

            Session.Add("Distrito", mobile.Empresa != null ? mobile.Empresa.Id : mobile.Linea != null ? mobile.Linea.Empresa.Id : -1);
            Session.Add("Location", mobile.Linea != null ? mobile.Linea.Id : -1);
            Session.Add("Chofer", mobile.Chofer != null ? mobile.Chofer.Id : -1);
            Session.Add("TypeMobile", mobile.TipoCoche.Id);
            Session.Add("Mobile", Mobile);
            Session.Add("InitialDate", From.AddMinutes(-1));
            Session.Add("FinalDate", To.AddMinutes(1));
            Session.Add("ShowMessages", 0);
            Session.Add("ShowPOIS", 0);

            OpenWin("../../../Monitor/MonitorHistorico/monitorHistorico.aspx", "Monitor Historico");
        }

        /// <summary>
        /// Displays a detailed information about the route.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LnkResumenDeRutaClick(object sender, EventArgs e)
        {
            var mobile = DAOFactory.CocheDAO.FindById(Mobile);

            Session.Add("RouteLocation", mobile.Empresa != null ? mobile.Empresa.Id : -1);
            Session.Add("RouteCompany", mobile.Linea != null ? mobile.Linea.Id : -1);
            Session.Add("RouteMobileType", mobile.TipoCoche != null ? mobile.TipoCoche.Id : -1);
            Session.Add("RouteMobile", Mobile);
            Session.Add("RouteInitialDate", From.AddMinutes(-1));
            Session.Add("RouteFinalDate", From.ToShortDateString().Equals(To.ToShortDateString()) ? To.AddMinutes(1) : To.Add(new TimeSpan(23, 59, 59)));

            OpenWin("../../Estadistica/MobileRoutes.aspx", "Resumen de Ruta");
        }

        /// <summary>
        /// Displays a detailed list of events reported within the route.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LnkReporteDeEventosClick(object sender, EventArgs e)
        {
            var mobile = DAOFactory.CocheDAO.FindById(Mobile);

            Session.Add("EventsLocation", mobile.Empresa != null ? mobile.Empresa.Id : -1);
            Session.Add("EventsCompany", mobile.Linea != null ? mobile.Linea.Id : -1);
            Session.Add("EventsMobileType", mobile.TipoCoche != null ? mobile.TipoCoche.Id : -1);
            Session.Add("EventsMobile", Mobile);
            Session.Add("EventsFrom", From.AddMinutes(-1));
            Session.Add("EventsTo", To.AddMinutes(1));

            OpenWin("../eventos.aspx", "Reporte de Eventos");
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "REP_GEOCERCAS"; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets report data.
        /// </summary>
        /// <returns></returns>
        private MobileActivity GetResults()
        {
            var vehicle = DAOFactory.CocheDAO.FindById(Mobile);

            var empresa = vehicle.Empresa != null ? vehicle.Empresa.Id : -1;
            var linea = vehicle.Linea != null ? vehicle.Linea.Id : -1;

            var ids = new List<Int32> {vehicle.Id};

            var activities = ReportFactory.MobileActivityDAO.GetMobileActivitys(From.ToDataBaseDateTime(), To.ToDataBaseDateTime(), empresa, linea, ids, 0);

            if (activities == null || activities.Count.Equals(0)) return null;

            return activities[0];
        }

        /// <summary>
        /// Displays info aboute the specified route fragment.
        /// </summary>
        /// <param name="activity"></param>
        private void DisplayResults(MobileActivity activity)
        {
            lblDetencion.Text = activity.HorasDetenido.ToString();
            lblInfraccion.Text = string.Format(CultureManager.GetLabel(TimeAndEventsInfractions), activity.HorasInfraccion, activity.Infracciones);
            lblKilometros.Text = string.Format("{0:0.00}km", activity.Recorrido);
            lblMovimiento.Text = activity.HorasActivo.ToString();
            lblSinReportar.Text = activity.HorasSinReportar.ToString();
            lblVelocidadMaxima.Text = string.Format("{0}km/h", activity.VelocidadMaxima);
            lblVelocidadPromedio.Text = string.Format("{0}km/h", activity.VelocidadPromedio);
            lblDuracion.Text = To.Subtract(From).ToString();
            lblFin.Text = string.Format("{0} {1}", To.ToShortDateString(), To.TimeOfDay);
            lblInicio.Text = string.Format("{0} {1}", From.ToShortDateString(), From.TimeOfDay);
        }

        #endregion
    }
}