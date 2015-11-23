using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Helpers.ColorHelpers;
using Logictracker.Security;

namespace Logictracker.Operacion.Simulador
{
    public partial class OperacionSimuladorDefault : ApplicationSecuredPage
    {
        #region Private Properties

        private static readonly string DefaultMessageImgUrl = string.Concat(ApplicationPath, "OpenLayers/img/marker-gold.png");
        private static string GoogleEarthScript { get { return string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", Config.Map.GoogleEarthKey); } }
        private static string GoogleMapsScript { get { return string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>", Config.Map.GoogleMapsKey); } }

        private int Distrito
        {
            get
            {
                if (ViewState["Distrito"] == null)
                {
                    ViewState["Distrito"] = Session["Distrito"];
                    Session["Distrito"] = null;
                }
                return (ViewState["Distrito"] != null) ? Convert.ToInt32(ViewState["Distrito"]) : 0;
            }
            set { ViewState["Distrito"] = value; }
        }

        private int Location
        {
            get
            {
                if (ViewState["Location"] == null)
                {
                    ViewState["Location"] = Session["Location"];
                    Session["Location"] = null;
                }
                return (ViewState["Location"] != null) ? Convert.ToInt32(ViewState["Location"]) : 0;
            }
            set { ViewState["Location"] = value; }
        }

        private int TypeMobile
        {
            get
            {
                if (ViewState["TypeMobile"] == null)
                {
                    ViewState["TypeMobile"] = Session["TypeMobile"];
                    Session["TypeMobile"] = null;
                }
                return (ViewState["TypeMobile"] != null) ? Convert.ToInt32(ViewState["TypeMobile"]) : 0;
            }
        }

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
                return (ViewState["InitialDate"] != null) ? Convert.ToDateTime(ViewState["InitialDate"]).ToDisplayDateTime() : DateTime.Today;
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
                return (ViewState["FinalDate"] != null) ? Convert.ToDateTime(ViewState["FinalDate"]).ToDisplayDateTime()
                           : DateTime.Today.Add(new TimeSpan(23, 59, 59));
            }
            set { ViewState["FinalDate"] = value; }
        }

        /// <summary>
        /// Determines wither to enable or disable filter controls.
        /// </summary>
        private bool LockFilters
        {
            get
            {
                if (ViewState["LockFilters"] == null)
                {
                    ViewState["LockFilters"] = Session["LockFilters"];
                    Session["LockFilters"] = null;
                }
                return ViewState["LockFilters"] != null && Convert.ToBoolean(ViewState["LockFilters"]);
            }
        }

        private int MessageType
        {
            get
            {
                if (ViewState["MessageType"] == null)
                {
                    ViewState["MessageType"] = Session["MessageType"];
                    Session["MessageType"] = null;
                }
                return ViewState["MessageType"] != null ? Convert.ToInt32(ViewState["MessageType"]) : 0;
            }
            set { ViewState["MessageType"] = value; }
        }


        private List<string> MessagesIds
        {
            get
            {
                if (ViewState["MessagesIds"] == null)
                {
                    ViewState["MessagesIds"] = Session["MessagesIds"];
                    Session["MessagesIds"] = null;
                }
                return ViewState["MessagesIds"] != null ? (List<string>)ViewState["MessagesIds"] : new List<string>(1);
            }
            set { ViewState["MessagesIds"] = value; }
        }
        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the info label associated to the page.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets up the filters initial values.
        /// </summary>
        private void SetInitialFilterValues()
        {
            dtDesde.SelectedDate = InitialDate;
            dtHasta.SelectedDate = FinalDate;

            WestPanel.Enabled = !LockFilters;
            dtDesde.Enabled = !LockFilters;
            dtHasta.Enabled = !LockFilters;
        }

        /// <summary>
        /// Searchs result positions to be displayed.
        /// </summary>
        /*private void SearchPositions()
        {
            var route = DAOFactory.RoutePositionDAO.GetPositions(Mobile, InitialDate.ToDataBaseDateTime(), FinalDate.ToDataBaseDateTime());

            if (route.Count == 0)
            {
                ShowInfo("No se encontraron posiciones para los parametros de busqueda ingresados!");

                return;
            }

            var pos = "[";

            for (var i = 0; i < route.Count; i++)
            {
                var dist = i == route.Count - 1 ? 0 : Distancias.Loxodromica(route[i].Latitude, route[i].Longitude, route[i + 1].Latitude, route[i + 1].Longitude);

                var duration = i == route.Count - 1 ? 0 : route[i + 1].Date.Subtract(route[i].Date).TotalSeconds;

                if (i > 0) pos = string.Concat(pos, ',');

                pos = string.Concat(pos, string.Format("{{lon: {0}, lat: {1}, speed: {2}, distance: {3}, duration: {4}, time: new Date{5}}}",
                    route[i].Longitude.ToString(CultureInfo.InvariantCulture), route[i].Latitude.ToString(CultureInfo.InvariantCulture), route[i].Speed,
                    dist.ToString(CultureInfo.InvariantCulture), duration.ToString(CultureInfo.InvariantCulture), route[i].Date.ToString("(yyyy, MM, dd, HH, mm, ss)")));
            }

            pos = string.Concat(pos, "]");

            System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "route", string.Format("simulador.createRoute({0});", pos), true);
        }
        */
        private void SearchPositions()
        {
            var colorGenerator = new ColorGenerator(new List<Color> { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Violet, Color.Cyan, Color.Purple });
            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;

            var route = DAOFactory.RoutePositionsDAO.GetPositionsByRoute(Mobile, InitialDate.ToDataBaseDateTime(), FinalDate.ToDataBaseDateTime(), TimeSpan.Zero, maxMonths);

            if (route.Count == 0)
            {
                ShowInfo("No se encontraron posiciones para los parametros de busqueda ingresados!");

                return;
            }

            var pos = "[";

            for (var j = 0; j < route.Count; j++)
            {
                var tramo = route[j];
                var color = HexColorUtil.ColorToHex(colorGenerator.GetNextColor(j)).Substring(1);
                color = color.Substring(4, 2) + color.Substring(2, 2) + color.Substring(0, 2);
                for (var i = 0; i < tramo.Count; i++)
                {
                    var posicion = tramo[i];
                    var next = i == tramo.Count - 1
                                   ? j == route.Count - 1 ? null : route[j + 1][0]
                                   : tramo[i + 1];
                    var dist = next == null ? 0 : Distancias.Loxodromica(posicion.Latitude, posicion.Longitude, next.Latitude, next.Longitude);

                    var duration = next == null ? 0 : next.Date.Subtract(posicion.Date).TotalSeconds;

                    if (j > 0 || i > 0) pos = string.Concat(pos, ',');

                    pos = string.Concat(pos, string.Format("{{lon: {0}, lat: {1}, speed: {2}, distance: {3}, duration: {4}, time: new Date{5}, 'color': '{6}' }}",
                        posicion.Longitude.ToString(CultureInfo.InvariantCulture), posicion.Latitude.ToString(CultureInfo.InvariantCulture), posicion.Speed,
                        dist.ToString(CultureInfo.InvariantCulture), duration.ToString(CultureInfo.InvariantCulture), posicion.Date.ToDisplayDateTime().ToString("(yyyy, MM, dd, HH, mm, ss)"), color));
                }
            }
            pos = string.Concat(pos, "]");

            var startflag = CreateAbsolutePath("~/images/salida.png");
            var endflag = CreateAbsolutePath("~/images/llegada.png");

            System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "route", string.Format("simulador.createRoute({0},'{1}','{2}');", pos, startflag, endflag), true);
        }

        /// <summary>
        /// Transforms the mobile event into a javascript object.
        /// </summary>
        /// <returns></returns>
        private void GetMessages()
        {
            var messages = lbMessages.SelectedStringValues;

            if (MessagesIds.Count == 0 && messages.Count == 0) return;

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;
            var events = (from ev in DAOFactory.LogMensajeDAO.GetByMobilesAndTypes(ddlMovil.SelectedValues, GetSelectedMessagesCodes(messages), InitialDate.ToDataBaseDateTime(), FinalDate.ToDataBaseDateTime(), maxMonths) orderby ev.Fecha select ev).ToList();

            var eventarray = "[";
            for (var i = 0; i < events.Count; i++)
            {
                var evento = events[i];
                if (!evento.HasValidLatitudes()) continue;

                var messageIconUrl = evento.GetIconUrl();
                var iconUrl = CreateAbsolutePath(string.IsNullOrEmpty(messageIconUrl) ? DefaultMessageImgUrl : string.Concat(IconDir, messageIconUrl));

                if (i > 0) eventarray += ",";

                eventarray += string.Format("{{ id: 'ev_{0}', name: '{1}', lat: {2}, lon:{3}, icon:'{4}', time: new Date{5} }}",
                    evento.Id, evento.Texto, evento.Latitud.ToString(CultureInfo.InvariantCulture), evento.Longitud.ToString(CultureInfo.InvariantCulture),
                    iconUrl, evento.Fecha.ToDisplayDateTime().ToString("(yyyy, MM, dd, HH, mm, ss)"));

                //Monitor.AddMarkers(MENSAJES, new Marker(i.ToString(), iconUrl, events[i].Latitud, events[i].Longitud,
                //    string.Format("javascript:gMSP({0})", events[i].Id), DrawingFactory.GetSize(24, 24), DrawingFactory.GetOffset(-12, -12)));

                //if (events[i].HasDuration()) AddMessageWithElapsedTime(events[i]);
            }
            eventarray += "]";

            System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "events", string.Format("simulador.addEvents({0});", eventarray), true);
            //SetMessagesCenterIndex(events);
        }
        #endregion

        #region Protected Methods

        protected string CreateAbsolutePath(string virtualUrl)
        {
            return string.Concat(Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Length - Request.Url.LocalPath.Length),
                                 ResolveUrl(virtualUrl));
        }

        private IEnumerable<string> GetSelectedMessagesCodes(List<string> messages) { return MessagesIds.Count > 0 ? MessagesIds : messages; }

        /// <summary>
        /// Initial filter values binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack) SetInitialFilterValues();

            base.OnPreLoad(e);
        }

        /// <summary>
        /// Sets the initial search filters.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            Header.Controls.AddAt(0, new Literal { Text = string.Concat(GoogleEarthScript, GoogleMapsScript) });

            RegisterExtJsStyleSheet();

            var callServer = string.Format("function callServer(){{{0}}}", Mobile > 0 ? string.Concat(ClientScript.GetPostBackEventReference(btnSearch, string.Empty), ";") : string.Empty);

            ClientScript.RegisterStartupScript(typeof(string), "callServer", callServer, true);
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "SIMULADOR"; }

        /// <summary>
        /// Company data binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlDistrito_InitialBinding(object sender, EventArgs e) { if (Distrito > 0) ddlDistrito.EditValue = Distrito; }

        /// <summary>
        /// Location initial data binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPlanta_PreBind(object sender, EventArgs e) { if (Location > 0) ddlPlanta.EditValue = Location; }

        /// <summary>
        /// Vehicle type initial data binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTipoVehiculo_PreBind(object sender, EventArgs e) { if (TypeMobile > 0)  ddlTipoVehiculo.EditValue = TypeMobile; }

        /// <summary>
        /// Vehicle initial data binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlMovil_PreBind(object sender, EventArgs e) { if (Mobile > 0) ddlMovil.EditValue = Mobile; }

        /// <summary>
        /// Message T initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTipo_PreBind(object sender, EventArgs e) { if (MessageType > 0) ddlTipo.EditValue = MessageType; }

        /// <summary>
        /// Selects all the messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkMessages_Click(object sender, EventArgs e) { lbMessages.ToogleItems(); }

        /// <summary>
        /// Searchs for positions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Distrito = ddlDistrito.Selected;
            Location = ddlPlanta.Selected;
            Mobile = ddlMovil.Selected;
            InitialDate = dtDesde.SelectedDate.HasValue ? dtDesde.SelectedDate.Value.ToDataBaseDateTime(): DateTime.UtcNow.ToDataBaseDateTime();
            FinalDate = dtHasta.SelectedDate.HasValue ? dtHasta.SelectedDate.Value.ToDataBaseDateTime() : InitialDate.AddDays(1);
            MessageType = ddlTipo.Selected;
            MessagesIds = lbMessages.SelectedStringValues;

            SearchPositions();
            GetMessages();
        }

        #endregion
    }
}
