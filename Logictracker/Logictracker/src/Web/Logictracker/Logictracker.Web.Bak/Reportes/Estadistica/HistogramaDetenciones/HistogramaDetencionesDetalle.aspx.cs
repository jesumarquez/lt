using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.Estadistica.HistogramaDetenciones
{
    public partial class ReportesEstadisticaHistogramaDetencionesHistogramaDetencionesDetalle : SecuredGridReportPage<HistogramaDetencionesDetailVo>
    {
        #region Protected Properties

        public override C1GridView Grid { get{return gridDetenciones;} }
   
        protected override InfoLabel LblInfo { get{return infoLabel1;} }

        protected override InfoLabel NotFound {get { return infoLabel1; }}

        protected override UpdatePanel UpdatePanelGrid { get { return upGrid;} }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override ToolBar ToolBar { get { return ToolBar1; } }

        protected override string VariableName { get { return "DETENCIONES_DETALLE"; } }

        protected override string GetRefference() { return "EST_REP_HISTOGRAMA_DETENCIONES"; }

        protected override C1GridView GridPrint { get { return gridPrint; } }

        protected override UpdatePanel UpdatePanelPrint { get { return upPrint; } }

        protected override Repeater PrintFilters { get { return FiltrosPrint; } }

        #endregion

        #region Private Properties

        /// <summary>
        /// The list of vehicle ids to use as filter value.
        /// </summary>
        private List<int> Vehiculos
        {
            get { return (ViewState["Vehiculos"] ?? new List<int>()) as List<int>; }
            set { ViewState["Vehiculos"] = value; }
        }

        /// <summary>
        /// Initial time for filtering messages.
        /// </summary>
        private DateTime Desde
        {
            get { return (DateTime)(ViewState["Desde"] ?? DateTime.MinValue); }
            set { ViewState["Desde"] = value; }
        }

        /// <summary>
        /// Final time for filtering messages.
        /// </summary>
        private DateTime Hasta
        {
            get { return (DateTime)(ViewState["Hasta"] ?? DateTime.MinValue); }
            set { ViewState["Hasta"] = value; }
        }

        /// <summary>
        /// Determines the minimum duration for the messages.
        /// </summary>
        private int Duracion
        {
            get { return (int)(ViewState["Duracion"] ?? 0); }
            set { ViewState["Duracion"] = value; }
        }

        /// <summary>
        /// Defines the radius that is going to be used to gruop events.
        /// </summary>
        private int Radio
        {
            get { return (int)(ViewState["Radio"] ?? 0); }
            set { ViewState["Radio"] = value; }
        }

        /// <summary>
        /// Initial refference latitude.
        /// </summary>
        private double Latitud
        {
            get { return (double)(ViewState["Latitud"] ?? 0); }
            set { ViewState["Latitud"] = value; }
        }

        /// <summary>
        /// Initial refference longitude.
        /// </summary>
        private double Longitud
        {
            get { return (double)(ViewState["Longitud"] ?? 0); }
            set { ViewState["Longitud"] = value; }
        }

        #endregion

        #region Protecthed Methods

        protected override List<HistogramaDetencionesDetailVo> GetResults()
        {
            const int maxMonths = 3;

            return (from o in ReportFactory.MobileEventDAO.GetDetenciones(Vehiculos, Desde, Hasta, Duracion, Radio, Latitud, Longitud, maxMonths)
                        select new HistogramaDetencionesDetailVo(o)).ToList();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
             return new Dictionary<string, string>
                       {  
                           {CultureManager.GetLabel("DESDE"), Desde.ToShortDateString() + " " + Desde.ToShortTimeString() }, 
                           {CultureManager.GetLabel("HASTA"), Hasta.ToShortDateString() + " " + Hasta.ToShortTimeString() }
                       };
        }

        /// <summary>
        /// Initial filter values setup and data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            SetInitialFilterValues();

            Bind();
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the initial filter values based on query string parameters.
        /// </summary>
        private void SetInitialFilterValues()
        {
            Vehiculos = Request.QueryString["Vehiculos"].Split(',').Select(coche => Convert.ToInt32(coche)).ToList();
            Desde = Convert.ToDateTime(Request.QueryString["Desde"], CultureInfo.InvariantCulture);
            Hasta = Convert.ToDateTime(Request.QueryString["Hasta"], CultureInfo.InvariantCulture);
            Duracion = Convert.ToInt32(Request.QueryString["Duracion"]);
            Radio = Convert.ToInt32(Request.QueryString["Radio"]);
            Latitud = Convert.ToDouble(Request.QueryString["Latitud"]);
            Longitud = Convert.ToDouble(Request.QueryString["Longitud"]);
        }

        /// <summary>
        /// Adds info about the current selected event to session in order to display it at the historic monitor.
        /// </summary>
        private void AddSessionParameters()
        {
            var stoppedEvent = DAOFactory.LogMensajeDAO.FindById(Convert.ToInt32(Grid.SelectedDataKey.Value));

            var eventTime = stoppedEvent.Fecha.ToDisplayDateTime();
            var finalTime = eventTime.Add(TimeSpan.FromSeconds(stoppedEvent.Duracion));

            Session.Add("Distrito", stoppedEvent.Coche.Empresa != null ? stoppedEvent.Coche.Empresa.Id : stoppedEvent.Coche.Linea != null ? stoppedEvent.Coche.Linea.Empresa.Id : -1);
            Session.Add("Location", stoppedEvent.Coche.Linea != null ? stoppedEvent.Coche.Linea.Id : -1);
            Session.Add("TypeMobile", stoppedEvent.Coche.TipoCoche.Id);
            Session.Add("Mobile", stoppedEvent.Coche.Id);
            Session.Add("InitialDate", new DateTime(eventTime.Year, eventTime.Month, eventTime.Day, eventTime.Hour, eventTime.Minute, 0));
            Session.Add("FinalDate", new DateTime(finalTime.Year, finalTime.Month, finalTime.Day, finalTime.Hour, finalTime.Minute, 59));
            Session.Add("MessageType", stoppedEvent.Mensaje.TipoMensaje.Id);
            Session.Add("MessagesIds", new List<string> {stoppedEvent.Mensaje.Codigo});
            Session.Add("MessageCenterIndex", stoppedEvent.Id);
            Session.Add("ShowPOIS", 1);
        }

        #endregion
    }
}