using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Helpers.ColorHelpers;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;
using NHibernate.Util;
using Point = Logictracker.Web.Monitor.Geometries.Point;

namespace Logictracker.Monitor.MonitorRecorrido
{
    public partial class MonitorRecorrido : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "OPE_MON_RECORRIDO"; }
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        private const string PopupEvent = "mousedown";

        protected static class Images
        {
            public static readonly string Arrow = string.Concat(ImagesDir, "arrow.png");
            public static readonly string GreenArrow = string.Concat(ImagesDir, "arrow_green.png");
            public static readonly string CurrentPosition = string.Concat(ApplicationPath, "OpenLayers/img/marker-blue.png");
            public static readonly string DefaultMessage = string.Concat(ApplicationPath, "OpenLayers/img/marker-gold.png");
            public static readonly string DefaultPois = string.Concat(ImagesDir, "pois.png");
            public static readonly string Start = string.Concat(ImagesDir, "salida.gif");
            public static readonly string End = string.Concat(ImagesDir, "llegada.gif");
            public static readonly string ImageHandler = string.Concat(ApplicationPath, "Common/EditImage.ashx?file={0}&angle={1}");
            public static readonly string Stopped = string.Concat(ImagesDir, "stopped.png");
        }
        protected static class Layers
        {
            public static readonly string Recorrido1 = CultureManager.GetLabel("RECORRIDO") + "1";
            public static readonly string Recorrido2 = CultureManager.GetLabel("RECORRIDO") + "2";
            public static readonly string Recorrido3 = CultureManager.GetLabel("RECORRIDO") + "3";
            public static readonly string Recorrido4 = CultureManager.GetLabel("RECORRIDO") + "4";
            public static readonly string Eventos = CultureManager.GetLabel("EVENTOS");
            public static readonly string MensajesDuracion = CultureManager.GetLabel("EXCESOS_VELOCIDAD");
            public static readonly string Mensajes = CultureManager.GetLabel("MENSAJES");
            public static readonly string PuntosDeInteres = CultureManager.GetLabel("LAYER_POI");
            public static readonly string Geocercas = CultureManager.GetLabel("LAYER_GEOCERCAS");    
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            this.RegisterCss(ResolveUrl("~/App_Styles/openlayers.css"));
            RegisterExtJsStyleSheet();

            SelectMessages();

            SelectPoisType();

            InitializeMap();
            
            SearchPositions();
        }
 
        private void InitializeMap()
        {
            Monitor.EnableTimer = false;
            Monitor.MultiplePopUps = true;

            var googleMapsEnabled = true;
            var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
            if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
            {
                var empresa = usuario.Empresas.First() as Empresa;
                if (empresa != null)
                    googleMapsEnabled = empresa.GoogleMapsEnabled;
            }

            Monitor.Initialize(googleMapsEnabled);
            Monitor.AddLayers(LayerFactory.GetVector(Layers.Recorrido1, true),
                              LayerFactory.GetVector(Layers.Recorrido2, true),
                              LayerFactory.GetVector(Layers.Recorrido3, true),
                              LayerFactory.GetVector(Layers.Recorrido4, true),
                              LayerFactory.GetVector(Layers.MensajesDuracion, true),
                              LayerFactory.GetVector(Layers.Geocercas, true),
                              LayerFactory.GetMarkers(Layers.PuntosDeInteres, true),
                              LayerFactory.GetMarkers(Layers.Mensajes, true),
                              LayerFactory.GetMarkers(Layers.Eventos, true));

            Monitor.AddControls(ControlFactory.GetToolbar(false, false, false, false, false, true, true));

            Monitor.ZoomTo(8);
            Monitor.Hide();

            this.RegisterStartupJScript("map", string.Format("var map = null;"));
        }

        private readonly List<DateTime> _fechasRecorridos = new List<DateTime>();
        private Double _x;
        private Double _y;
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
        private List<int> Mobiles
        {
            get
            {
                if (ViewState["Mobiles"] == null)
                {
                    ViewState["Mobiles"] = Session["Mobiles"];
                    Session["Mobiles"] = null;
                }
                return (ViewState["Mobiles"] != null) ? (List<int>)ViewState["Mobiles"] : new List<int>();
            }
            set { ViewState["Mobiles"] = value; }
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
                return (ViewState["InitialDate"] != null) ? Convert.ToDateTime(ViewState["InitialDate"]) : DateTime.UtcNow.Date.ToDataBaseDateTime();
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
                return (ViewState["FinalDate"] != null) ? Convert.ToDateTime(ViewState["FinalDate"]) : DateTime.UtcNow.Date.Add(new TimeSpan(23, 59, 59)).ToDataBaseDateTime();
            }
            set { ViewState["FinalDate"] = value; }
        }
        private TimeSpan Stopped
        {
            get { return (ViewState["Stopped"] != null) ? (TimeSpan)ViewState["Stopped"] : new TimeSpan(1,0,0); }
            set { ViewState["Stopped"] = value; }
        }
        private int Distance
        {
            get
            {
                if (ViewState["Distance"] == null)
                {
                    ViewState["Distance"] = Session["Distance"];
                    Session["Distance"] = null;
                }
                return (ViewState["Distance"] != null) ? Convert.ToInt32(ViewState["Distance"]) : 500;
            }
            set { ViewState["Distance"] = value; }
        }
        private int StoppedEvent
        {
            get
            {
                if (ViewState["StoppedEvent"] == null)
                {
                    ViewState["StoppedEvent"] = Session["StoppedEvent"];
                    Session["StoppedEvent"] = null;
                }
                return (ViewState["StoppedEvent"] != null) ? Convert.ToInt32(ViewState["StoppedEvent"]) : 5;
            }
            set { ViewState["StoppedEvent"] = value; }
        }
        private int? RouteCenterIndex
        {
            get
            {
                ViewState["CenterIndex"] = Session["CenterIndex"];
                Session["CenterIndex"] = null;
                return (ViewState["CenterIndex"] != null) ? (int?)Convert.ToInt32(ViewState["CenterIndex"]) : null;
            }
        }
        private int? MessageCenterIndex
        {
            get
            {
                if (Session["MessageCenterIndex"] != null)
                {
                    var index = Convert.ToInt32(Session["MessageCenterIndex"]);

                    Session["MessageCenterIndex"] = null;

                    return index;
                }

                return null;
            }
            set { Session["MessageCenterIndex"] = value; }
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
        private List<int> PoisTypesIds
        {
            get
            {
                if (ViewState["PoisTypesIds"] == null)
                {
                    ViewState["PoisTypesIds"] = Session["PoisTypesIds"];
                    Session["PoisTypesIds"] = null;
                }
                return ViewState["PoisTypesIds"] != null ? (List<int>)ViewState["PoisTypesIds"] : new List<int>(1){0};
            }
            set { ViewState["PoisTypesIds"] = value; }
        }
        private int? PosCenterIndex
        { 
            get
            {
                if (Session["PosCenterIndex"] != null)
                {
                    var index = Convert.ToInt32(Session["PosCenterIndex"]);

                    Session["PosCenterIndex"] = null;

                    return index;
                }

                return null;
            }
            set { Session["PosCenterIndex"] = value; }
        }
        private bool ShowMessages
        {
            get
            {
                if (Session["ShowMessages"] != null)
                {
                    var show = Convert.ToByte(Session["ShowMessages"]);

                    Session["ShowMessages"] = null;

                    return show.Equals(1);
                }

                return false;
            }
        }
        private bool ShowPois
        {
            get
            {
                if (Session["ShowPOIS"] != null)
                {
                    var show = Convert.ToByte(Session["ShowPOIS"]);

                    Session["ShowPOIS"] = null;

                    return show.Equals(1);
                }

                return false;
            }
        }

        #region Private Methods

        private bool GetResults()
        {
            var showResults = false;
            var colorGenerator = new ColorGenerator(new List<Color> { Color.Brown, Color.Purple, Color.RoyalBlue, Color.LawnGreen });

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;

            for (var i = 0; i < Mobiles.Count; i++)
            {
                var mobile = Mobiles[i];
                var routes = DAOFactory.RoutePositionsDAO.GetPositionsByRoute(mobile, InitialDate, FinalDate, Stopped, maxMonths);
                var layerName = GetLayerName(i);

                showResults = showResults || routes[0].Count > 0;

                if (showResults)
                {
                    AddPositions(routes, colorGenerator.GetNextColor(), layerName);

                    GetMessages(mobile);

                    GetInfracciones(mobile, routes);

                    GenerateMarkers();

                    AddFlags(routes);

                    GetReferenciasGeograficas();

                    GetCenter(routes);

                    Monitor.SetCenter(_x, _y);
                    Monitor.SetDefaultCenter(_x, _y);
                }
            }
            return showResults;
        }

        private static string GetLayerName(int i)
        {
            switch (i)
            {
                case 0: return Layers.Recorrido1;
                case 1: return Layers.Recorrido2;
                case 2: return Layers.Recorrido3;
                default: return Layers.Recorrido4;
            }
        }

        private void DrawGeocerca(ReferenciaGeografica geocerca)
        {
            var color = geocerca.Color != null ? geocerca.Color.Color : Color.Blue;
            if (geocerca.Poligono.Radio > 0) DrawCircle(geocerca, color);
            else DrawPolygon(geocerca, color);
        }

        private void DrawCircle(ReferenciaGeografica geocerca, Color color)
        {
            var point = geocerca.Poligono.FirstPoint;
            var poligono = new Point(string.Format("POINT_{0}", geocerca.Id), point.X, point.Y, geocerca.Poligono.Radio, StyleFactory.GetPointFromColor(color));

            Monitor.AddGeometries(Layers.Geocercas, poligono);
        }

        private void DrawPolygon(ReferenciaGeografica geocerca, Color color)
        {
            var poligono = new Polygon(string.Format("POLYGON_{0}", geocerca.Id), StyleFactory.GetPointFromColor(color));

            var points = geocerca.Poligono.ToPointFList();

            for (var i = 0; i < points.Count; i++) poligono.AddPoint(new Point(string.Format("{0}_{1}", geocerca.Id, i), points[i].X, points[i].Y));

            Monitor.AddGeometries(Layers.Geocercas, poligono);
        }

        private void GetCenter(IEnumerable<List<RoutePosition>> routes)
        {
            var centerPosition = PosCenterIndex;

            if (centerPosition == null) return;

            foreach (var position in routes.SelectMany(route => route.Where(position => position.Id.Equals(centerPosition.Value))))
            {
                Monitor.AddMarkers(Layers.Eventos, new Marker(position.Date.ToDisplayDateTime().ToString(), Images.CurrentPosition, position.Latitude, position.Longitude,
                    string.Format("javascript:gCP('{0}', '{1}', '{2}')", GeocoderHelper.GetDescripcionEsquinaMasCercana(position.Latitude, position.Longitude),
                    position.Date.ToDisplayDateTime(), position.Speed),
                    DrawingFactory.GetSize(21, 25), DrawingFactory.GetOffset(-10.5, -25)));

                _x = position.Latitude;
                _y = position.Longitude;
                
                Monitor.TriggerEvent(position.Date.ToDisplayDateTime().ToString(), Layers.Eventos, PopupEvent);
            }
        }

        private void AddPositions(IList<List<RoutePosition>> routes, Color color, string layerName)
        {
            RoutePosition lastPosition = null;
            for (var i = 0; i < routes.Count; i++)
            {
                if (routes[i].Count > 0) _fechasRecorridos.Add(routes[i][routes[i].Count -1].Date);

                var lineId = i.ToString(CultureInfo.InvariantCulture);
                var routeLine = new Line(lineId, StyleFactory.GetLineFromColor(color));
                
                for (var j = 0; j < routes[i].Count; j++)
                {
                    var currentPosition = routes[i][j];
                    if (lastPosition != null)
                    {
                        if(currentPosition.EqualsPosition(lastPosition))
                        {
                            lastPosition = currentPosition;
                            continue;
                        }
                    }

                    routeLine.AddPoint(new Point(j.ToString(CultureInfo.InvariantCulture), currentPosition.Longitude, currentPosition.Latitude));
                    lastPosition = currentPosition;
                }

                Monitor.AddGeometries(layerName, routeLine);
            }

            _x = routes[0][0].Latitude;
            _y = routes[0][0].Longitude;

            Monitor.SetDefaultCenter(_x, _y);
        }

        private void AddFlags(IList<List<RoutePosition>> routes)
        {
            var start = routes[0][0];
            var end = routes[routes.Count - 1][routes[routes.Count - 1].Count - 1];

            var size = DrawingFactory.GetSize(40, 40);
            var offset = DrawingFactory.GetOffset(-20, -32);

            Monitor.AddMarkers(Layers.Eventos, new Marker(start.Date.ToDisplayDateTime().ToString(), Images.Start, start.Latitude, start.Longitude,
                        string.Format("javascript:gFP('{0}')", string.Concat(start.Date.ToDisplayDateTime().ToShortDateString(), " ", start.Date.ToDisplayDateTime().TimeOfDay.ToString())), size, offset),
                        new Marker(end.Date.ToDisplayDateTime().ToString(), Images.End, end.Latitude, end.Longitude, string.Format("javascript:gFP('{0}')",
                        string.Concat(end.Date.ToDisplayDateTime().ToShortDateString(), " ", end.Date.ToDisplayDateTime().TimeOfDay.ToString())), size, offset));
        }

        private void SearchPositions()
        {
            try
            {
                if (Mobiles.Count > 0)
                {
                    Monitor.Clear();

                    var showPositions = GetResults();

                    if (showPositions) 
                        Monitor.Show();
                    else 
                        Monitor.Hide();

                    infoLabel1.Text = !showPositions ? "No se encontraron posiciones para los parámetros de búsqueda ingresados!" : null;
                }
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void GetReferenciasGeograficas()
        {
            var pois = !PoisTypesIds.Contains(0)
                       ? DAOFactory.ReferenciaGeograficaDAO.GetList(new[] { Distrito }, new[] { Location }, PoisTypesIds).Where(x => x.Vigencia == null || x.Vigencia.Vigente(InitialDate, FinalDate)).ToList()
                       : new List<ReferenciaGeografica>();

            foreach (var punto in pois)
            {
                if (punto.Poligono != null) DrawGeocerca(punto);

                if (punto.Direccion != null) Monitor.AddMarkers(Layers.PuntosDeInteres, new Marker(punto.Id.ToString("#0"), GetPoiIcon(punto), punto.Direccion.Latitud, punto.Direccion.Longitud,
                    string.Format("javascript:gPOIP('{0}')", punto.Descripcion), DrawingFactory.GetSize(24, 24), DrawingFactory.GetOffset(-12, -12)));
            }
        }

        private string GetPoiIcon(ReferenciaGeografica punto)
        {
            string iconPath;

            if (punto.Icono != null && File.Exists(Server.MapPath((iconPath = string.Concat(IconDir, punto.Icono.PathIcono))))) return iconPath;

            if (punto.TipoReferenciaGeografica.Icono != null && File.Exists(Server.MapPath((iconPath = string.Concat(IconDir, punto.TipoReferenciaGeografica.Icono.PathIcono)))))
                return iconPath;
            
            return Images.DefaultPois;
        }

        private List<RouteEvent> FilterEvents()
        {
            var events = ReportFactory.RouteEventDAO.GetRouteEvents(Mobiles, InitialDate, FinalDate, 5);

            if (events == null || events.Count.Equals(0)) return null;

            var time = TimeSpan.FromMinutes(StoppedEvent);

            return (from marker in events where marker.Distance >= Distance/1000.0 || (marker.MaximumSpeed == 0 && marker.ElapsedTime >= time) select marker).ToList();
        }

        private void GetMessages(int mobile)
        {
            var messages = lbMessages.SelectedStringValues;

            if (MessagesIds.Count == 0 && messages.Count == 0) return;

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;
            var events = DAOFactory.LogMensajeDAO.GetByMobilesAndTypes(new[] {mobile}, GetSelectedMessagesCodes(messages), InitialDate, FinalDate, maxMonths);

            for (var i = 0; i < events.Count(); i++)
            {
                var el = events.ElementAt(i);
                if (!el.HasValidLatitudes()) continue;

                var messageIconUrl = el.GetIconUrl();
                var iconUrl = string.IsNullOrEmpty(messageIconUrl) ? Images.DefaultMessage : string.Concat(IconDir, messageIconUrl);

                Monitor.AddMarkers(Layers.Mensajes, new Marker(i.ToString("#0"), iconUrl, el.Latitud, el.Longitud,
                    string.Format("javascript:gMSP({0})", el.Id), DrawingFactory.GetSize(24, 24), DrawingFactory.GetOffset(-12, -12)));

                //if (el.HasDuration()) AddMessageWithElapsedTime(el);
            }

            SetMessagesCenterIndex(events);
        }

        private void GetInfracciones(int mobile, List<List<RoutePosition>> routes)
        {
            var infracciones = DAOFactory.InfraccionDAO.GetByVehiculo(mobile, Infraccion.Codigos.ExcesoVelocidad, InitialDate, FinalDate);

            foreach (var infraccion in infracciones)
            {
                if (!infraccion.HasValidLatitudes) continue;
                if (infraccion.HasDuration) AddMessageWithElapsedTime(infraccion, routes);
            }
        }
        
        private IEnumerable<string> GetSelectedMessagesCodes(IEnumerable<string> messages) { return MessagesIds.Count > 0 ? MessagesIds.Select(m => m) : messages; }

        private void SetMessagesCenterIndex(IEnumerable<LogMensaje> events)
        {
            var centerIndex = MessageCenterIndex;

            if (centerIndex == null) return;

            for (var i = 0; i < events.Count(); i++)
            {
                var el = events.ElementAt(i);
                if (el.Id.Equals(centerIndex))
                {
                    _x = el.Latitud;
                    _y = el.Longitud;

                    Monitor.TriggerEvent(i.ToString("#0"), Layers.Mensajes, PopupEvent);
                }
            }
        }

        private void AddMessageWithElapsedTime(Infraccion mobileEvent, IEnumerable<List<RoutePosition>> routes)
        {
            var positions = GetMessagePositions(mobileEvent, routes);

            var line = new Line(mobileEvent.Fecha.ToString(), StyleFactory.GetRedLine());

            line.AddPoint(new Point(mobileEvent.Fecha.ToString(), mobileEvent.Longitud, mobileEvent.Latitud));

            for (var i = 0; i < positions.Count; i++) line.AddPoint(new Point(i.ToString("#0"), positions[i].Longitude, positions[i].Latitude));

            line.AddPoint(new Point(mobileEvent.FechaFin.Value.ToString(), mobileEvent.LongitudFin, mobileEvent.LatitudFin));

            Monitor.AddGeometries(Layers.MensajesDuracion, line);
        }

        private static List<RoutePosition> GetMessagePositions(Infraccion mobileEvent, IEnumerable<List<RoutePosition>> routes)
        {
            var positions = new List<RoutePosition>();

            foreach (var route in routes) positions.AddRange(route.Where(position => position.Date >= mobileEvent.Fecha && position.Date <= mobileEvent.FechaFin.Value));

            return positions;
        }

        private void GenerateMarkers()
        {
            var events = FilterEvents();

            if (events == null) return;

            for (var i = 0; i < events.Count; i++)
            {
                Monitor.AddMarkers(Layers.Eventos, new Marker(i.ToString("#0"), GetIconUrl(events[i]), events[i].InitialLatitude, events[i].InitialLongitude,
                    string.Format("javascript:gMP('{0}','{1}','{2}','{3}','{4:0.00}','{5}','{6}','{7}')", events[i].InitialLatitude.ToString(CultureInfo.InvariantCulture),
                        events[i].InitialLongitude.ToString(CultureInfo.InvariantCulture), string.Concat(events[i].InitialDate.ToDisplayDateTime().ToShortDateString(), " ",
                        events[i].InitialDate.ToDisplayDateTime().TimeOfDay.ToString()), events[i].ElapsedTime, events[i].Distance, events[i].MinimumSpeed, events[i].MaximumSpeed, 
                        Convert.ToInt32(events[i].AverageSpeed)),
                    DrawingFactory.GetSize(24, 24), GetMarkerOffset(events[i])));
            }

            var centerIndex = RouteCenterIndex;

            if (centerIndex == null) return;

            _x = events[centerIndex.Value].InitialLatitude;
            _y = events[centerIndex.Value].InitialLongitude;

            Monitor.TriggerEvent(centerIndex.ToString(), Layers.Eventos, PopupEvent);
        }

        private static string GetMarkerOffset(RouteEvent routeEvent)
        {
            if (routeEvent.MaximumSpeed == 0) return DrawingFactory.GetOffset(-12, -12);

            const int desplazamiento = 10;
            var angulo = (-(routeEvent.Direction - 90) * 2 * Math.PI) / 360;
            var dx = Math.Cos(angulo) * desplazamiento;
            var dy = Math.Sin(angulo) * desplazamiento;

            return DrawingFactory.GetOffset(-12 + dx, -12 - dy);
        }

        private string GetIconUrl(RouteEvent aEvent)
        {
            return aEvent.MaximumSpeed == 0 ? Images.Stopped : string.Format(Images.ImageHandler, GetRouteImage(aEvent.FinalDate), aEvent.Direction);
        }

        private string GetRouteImage(DateTime dateTime)
        {
            for (var i = 0; i < _fechasRecorridos.Count; i++) if (dateTime <= _fechasRecorridos[i]) return i == 0 || i%2 == 0 ? Images.GreenArrow : Images.Arrow;

            return string.Empty;
        }

        private void SetInitialFilterValues()
        {
            GetQueryStringParameters();

            dtDesde.SelectedDate = InitialDate.ToDisplayDateTime();
            dtHasta.SelectedDate = FinalDate.ToDisplayDateTime();
            tpStopped.SelectedTime = Stopped;
            npDistance.Number = Distance;
            npStoppedEvent.Number = StoppedEvent;
        }

        private void GetQueryStringParameters()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Empresa"])) Distrito = Convert.ToInt32(Request.QueryString["Empresa"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Planta"])) Location = Convert.ToInt32(Request.QueryString["Planta"]);

            if (!string.IsNullOrEmpty(Request.QueryString["InitialDate"]))
                InitialDate = Convert.ToDateTime(Request.QueryString["InitialDate"], CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(Request.QueryString["FinalDate"]))
                FinalDate = Convert.ToDateTime(Request.QueryString["FinalDate"], CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(Request.QueryString["PosCenterIndex"])) PosCenterIndex = Convert.ToInt32(Request.QueryString["PosCenterIndex"]);

            if (!string.IsNullOrEmpty(Request.QueryString["MessageCenterIndex"])) MessageCenterIndex = Convert.ToInt32(Request.QueryString["MessageCenterIndex"]);
        }

        private void SelectAllMessages() { foreach (ListItem item in lbMessages.Items) item.Selected = true; }

        private void SelectMessages()
        {
            var showAll = false;

            if (!string.IsNullOrEmpty(Request.QueryString["ShowMessages"])) showAll = Convert.ToByte(Request.QueryString["ShowMessages"]).Equals(1);

            if (showAll || ShowMessages) 
                SelectAllMessages();
            else 
                foreach (var id in MessagesIds.Where(id => lbMessages.Items.FindByValue(id) != null)) 
                    lbMessages.Items.FindByValue(id).Selected = true;
        }

        private void SelectPoisType()
        {
            var showAll = false;

            if (!string.IsNullOrEmpty(Request.QueryString["ShowPOIS"])) showAll = Convert.ToByte(Request.QueryString["ShowPOIS"]).Equals(1);

            if (showAll || ShowPois) SelectAllPois();
            else foreach (var id in PoisTypesIds) if (lbPuntosDeInteres.Items.FindByValue(id.ToString("#0")) != null) lbPuntosDeInteres.Items.FindByValue(id.ToString("#0")).Selected = true;

            PoisTypesIds = lbPuntosDeInteres.SelectedValues;
        }

        private void SelectAllPois() { foreach (ListItem poi in lbPuntosDeInteres.Items) poi.Selected = true; }

        private void AddSessionParameters()
        {
            Session.Add("Distrito", Distrito);
            Session.Add("Location", Location);
            Session.Add("Mobiles", Mobiles);
            Session.Add("InitialDate", InitialDate);
            Session.Add("FinalDate", FinalDate);
            Session.Add("PoisTypesIds", PoisTypesIds);
        }

        #endregion

        #region Protected Methods

        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack) SetInitialFilterValues();

            base.OnPreLoad(e);
        }

        protected void DdlDistritoInitialBinding(object sender, EventArgs e) { if (Distrito > 0) ddlDistrito.EditValue = Distrito; }
        protected void DdlPlantaPreBind(object sender, EventArgs e) { if (Location > 0) ddlPlanta.EditValue = Location; }
        protected void DdlTipoPreBind(object sender, EventArgs e) { if (MessageType > 0) ddlTipo.EditValue = MessageType; }

        protected void BtnSearchClick(object sender, EventArgs e)
        {
            Distrito = ddlDistrito.Selected;
            Location = ddlPlanta.Selected;
            var moviles = new List<int>();
            if (ddlMovil1.Selected > 0) moviles.Add(ddlMovil1.Selected);
            if (ddlMovil2.Selected > 0 && !moviles.Contains(ddlMovil2.Selected)) moviles.Add(ddlMovil2.Selected);
            if (ddlMovil3.Selected > 0 && !moviles.Contains(ddlMovil3.Selected)) moviles.Add(ddlMovil3.Selected);
            if (ddlMovil4.Selected > 0 && !moviles.Contains(ddlMovil4.Selected)) moviles.Add(ddlMovil4.Selected);
            Mobiles = moviles;
            InitialDate = SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.Value);
            FinalDate = SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value);
            Stopped = tpStopped.SelectedTime;
            Distance = npDistance.Number;
            StoppedEvent = npStoppedEvent.Number;
            MessageType = ddlTipo.Selected;
            MessagesIds = lbMessages.SelectedStringValues;
            PoisTypesIds = lbPuntosDeInteres.SelectedValues;

            var deltaTime = FinalDate.Subtract(InitialDate);
            if(deltaTime > dtvalidator.MaxRange)
            {
                ShowError("El rango de tiempo debe ser menor o igual a " + dtvalidator.MaxRange.ToString());
                return;
            }
            SearchPositions();
        }

        protected void LnkSimulatorClick(object sender, EventArgs e)
        {
            AddSessionParameters();

            OpenWin(ResolveUrl("~/Operacion/Simulador/Default.aspx"), "Simulador de Recorridos");
        }

        #endregion
    }
}