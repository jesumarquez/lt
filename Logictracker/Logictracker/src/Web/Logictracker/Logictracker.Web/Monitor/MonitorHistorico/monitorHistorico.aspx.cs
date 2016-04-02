using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Messaging;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
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
using Point = Logictracker.Web.Monitor.Geometries.Point;
using NHibernate.Util;

namespace Logictracker.Monitor.MonitorHistorico
{
    public partial class MonitorHistorico : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "MONITOR_HISTORICO"; }
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
            public static readonly string Recorrido = CultureManager.GetLabel("RECORRIDO");
            public static readonly string Eventos = CultureManager.GetLabel("EVENTOS");
            public static readonly string MensajesDuracion = CultureManager.GetLabel("EXCESOS_VELOCIDAD");
            public static readonly string Mensajes = CultureManager.GetLabel("MENSAJES");
            public static readonly string PuntosDeInteres = CultureManager.GetLabel("LAYER_POI");
            public static readonly string Geocercas = CultureManager.GetLabel("LAYER_GEOCERCAS");    
        }
        
        #region Private Properties

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
        private int Chofer
        {
            get
            {
                if (ViewState["Chofer"] == null)
                {
                    ViewState["Chofer"] = Session["Chofer"];
                    Session["Chofer"] = null;
                }
                return (ViewState["Chofer"] != null) ? Convert.ToInt32(ViewState["Chofer"]) : 0;
            }
            set { ViewState["Chofer"] = value; }
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
            set { ViewState["TypeMobile"] = value; }
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
                return (ViewState["Distance"] != null) ? Convert.ToInt32(ViewState["Distance"]) : 100;
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
                return (ViewState["StoppedEvent"] != null) ? Convert.ToInt32(ViewState["StoppedEvent"]) : 1;
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
        private int Ticket
        {
            get
            {
                if (ViewState["Ticket"] == null)
                {
                    ViewState["Ticket"] = Session["Ticket"];
                    Session["Ticket"] = null;
                }
                return (ViewState["Ticket"] != null) ? Convert.ToInt32(ViewState["Ticket"]) : 0;
            }
            set { ViewState["Ticket"] = value; }
        }
        private int Distribucion
        {
            get
            {
                if (ViewState["Distribucion"] == null)
                {
                    ViewState["Distribucion"] = Session["Distribucion"];
                    Session["Distribucion"] = null;
                }
                return (ViewState["Distribucion"] != null) ? Convert.ToInt32(ViewState["Distribucion"]) : 0;
            }
            set { ViewState["Distribucion"] = value; }
        }

        #endregion

        #region Private Methods

        private bool GetResults()
        {
            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;

            var routes = DAOFactory.RoutePositionsDAO.GetPositionsByRoute(Mobile, InitialDate, FinalDate, Stopped, maxMonths);

            var showResults = routes[0].Count > 0;

            if (showResults)
            {
                var coche = DAOFactory.CocheDAO.FindById(Mobile);
                if (coche.IsNewestPositionReceivedInCache())
                {
                    var pos = coche.RetrieveNewestReceivedPosition();
                    if (pos.FechaMensaje > InitialDate && pos.FechaMensaje < FinalDate)
                    {
                        var route = new RoutePosition(pos, false, -1);
                        var last = routes.Last();
                        routes.Remove(last);
                        last.Add(route);
                        routes.Add(last);
                    }
                }

                AddPositions(routes);

                GetMessages();

                GetInfracciones(routes);

                GenerateMarkers();

                AddFlags(routes);

                GetReferenciasGeograficas();

                GetCenter(routes);

                Monitor.SetCenter(_x, _y);
                Monitor.SetDefaultCenter(_x, _y);
            }

            return showResults;
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
            Monitor.AddLayers(LayerFactory.GetVector(Layers.Recorrido, true),
                              LayerFactory.GetVector(Layers.MensajesDuracion, true),
                              LayerFactory.GetVector(Layers.Geocercas, true),
                              LayerFactory.GetMarkers(Layers.PuntosDeInteres, true),
                              LayerFactory.GetMarkers(Layers.Mensajes, true),
                              LayerFactory.GetMarkers(Layers.Eventos, true));

            Monitor.AddControls(ControlFactory.GetToolbar(false, false, false, false, false, true, true));

            Monitor.ZoomTo(8);
            Monitor.Hide();
            
            //this.RegisterStartupJScript("map", string.Format("var map = $M('{0}');", (object) Monitor.MapDivId));
            this.RegisterStartupJScript("map", string.Format("var map = null;", (object)Monitor.MapDivId));
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

        private void AddPositions(IList<List<RoutePosition>> routes)
        {
            //var maxTimeBetweenPositions = TimeSpan.FromMinutes(15);
            //var maxDistanceBetweenPositions = 500;


            var colorGenerator = new ColorGenerator(new List<Color> {Color.Yellow, Color.Orange});
            //var subid = 1;
            RoutePosition lastPosition = null;
            for (var i = 0; i < routes.Count; i++)
            {
                if (routes[i].Count > 0) _fechasRecorridos.Add(routes[i][routes[i].Count -1].Date);

                var color = colorGenerator.GetNextColor();
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
                        //var tiempo = currentPosition.Date.Subtract(lastPosition.Date);
                        //if (tiempo > maxTimeBetweenPositions ||
                        //    Distancias.Loxodromica(lastPosition.Latitude, lastPosition.Longitude,
                        //    currentPosition.Latitude, currentPosition.Longitude) > maxDistanceBetweenPositions)
                        //{
                        //    Monitor.AddGeometries(_recorrido, routeLine);

                        //    routeLine = new Line(string.Format("{0}_{1}",lineId, (subid++)), StyleFactory.GetDottedLineFromColor(color));
                        //    routeLine.AddPoint(new Point(string.Format("{0}_{1}", lineId, (subid++)), lastPosition.Longitude, lastPosition.Latitude));
                        //    routeLine.AddPoint(new Point(string.Format("{0}_{1}", lineId, (subid++)), currentPosition.Longitude, currentPosition.Latitude));
                        //    Monitor.AddGeometries(_recorrido, routeLine);

                        //    routeLine = new Line(string.Format("{0}_{1}", lineId, (subid++)), StyleFactory.GetLineFromColor(color));
                        //}
                    }

                    routeLine.AddPoint(new Point(j.ToString(CultureInfo.InvariantCulture), currentPosition.Longitude, currentPosition.Latitude));
                    lastPosition = currentPosition;
                }

                Monitor.AddGeometries(Layers.Recorrido, routeLine);
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
                if (Mobile > 0)
                {
                    Monitor.Clear();

                    var showPositions = GetResults();

                    if (showPositions) 
                        Monitor.Show();
                    else 
                        Monitor.Hide();

                    lnkSimulator.Visible = lnkQualityMonitor.Visible = showPositions;
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

            if (pois.Count == 0 && lstTicket.SelectedValue != "")
            {
                var value = lstTicket.SelectedValue.Contains("T-") || lstTicket.SelectedValue.Contains("V-")
                            ? Convert.ToInt32((string) lstTicket.SelectedValue.Split('-')[1])
                            : Convert.ToInt32((string) lstTicket.SelectedValue);

                if (value > 0 && lstTicket.SelectedValue.Contains("T-"))
                {
                    var ticket = DAOFactory.TicketDAO.FindById(value);

                    pois.Add(ticket.Linea.ReferenciaGeografica);
                    pois.Add(ticket.PuntoEntrega.ReferenciaGeografica);
                }

                if (value > 0 && lstTicket.SelectedValue.Contains("V-"))
                {
                    var viaje = DAOFactory.ViajeDistribucionDAO.FindById(value);

                    pois.Add(viaje.Linea.ReferenciaGeografica);
                    pois.AddRange(viaje.Detalles.Select(detalle => detalle.ReferenciaGeografica));
                }
            }

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
            var events = ReportFactory.RouteEventDAO.GetRouteEvents(Mobile, InitialDate, FinalDate, 5);

            if (events == null || events.Count.Equals(0)) return null;

            var time = TimeSpan.FromMinutes(StoppedEvent);

            return (from marker in events where marker.Distance >= Distance/1000.0 || (marker.MaximumSpeed == 0 && marker.ElapsedTime >= time) select marker).ToList();
        }

        private void GetMessages()
        {
            var messages = lbMessages.SelectedStringValues;

            if (MessagesIds.Count == 0 && messages.Count == 0) return;

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;
            var events = DAOFactory.LogMensajeDAO.GetByMobilesAndTypes(ddlMovil.SelectedValues, GetSelectedMessagesCodes(messages), InitialDate, FinalDate, maxMonths);

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

        private void GetInfracciones(List<List<RoutePosition>> routes)
        {
            var infracciones = DAOFactory.InfraccionDAO.GetByVehiculo(Mobile, Infraccion.Codigos.ExcesoVelocidad, InitialDate, FinalDate);

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
                Monitor.AddMarkers(Layers.Eventos, new Marker(i.ToString("#0"), GetIconUrl(events[i]), events[i].InitialLatitude, events[i].InitialLongitude,
                    string.Format("javascript:gMP('{0}','{1}','{2}','{3}','{4:0.00}','{5}','{6}','{7}')", events[i].InitialLatitude.ToString(CultureInfo.InvariantCulture),
                        events[i].InitialLongitude.ToString(CultureInfo.InvariantCulture), string.Concat(events[i].InitialDate.ToDisplayDateTime().ToShortDateString(), " ",
                        events[i].InitialDate.ToDisplayDateTime().TimeOfDay.ToString()), events[i].ElapsedTime, events[i].Distance, events[i].MinimumSpeed, events[i].MaximumSpeed, 
                        Convert.ToInt32(events[i].AverageSpeed)),
                    DrawingFactory.GetSize(24, 24), GetMarkerOffset(events[i])));

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
            dtDia.SelectedDate = InitialDate.ToDisplayDateTime();
        }

        private void GetQueryStringParameters()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Empresa"])) Distrito = Convert.ToInt32(Request.QueryString["Empresa"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Planta"])) Location = Convert.ToInt32(Request.QueryString["Planta"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Chofer"])) Chofer = Convert.ToInt32(Request.QueryString["Chofer"]);

            if (!string.IsNullOrEmpty(Request.QueryString["TypeMobile"])) TypeMobile = Convert.ToInt32(Request.QueryString["TypeMobile"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Movil"])) Mobile = Convert.ToInt32(Request.QueryString["Movil"]);

            if (!string.IsNullOrEmpty(Request.QueryString["InitialDate"]))
                InitialDate = Convert.ToDateTime(Request.QueryString["InitialDate"], CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(Request.QueryString["FinalDate"]))
                FinalDate = Convert.ToDateTime(Request.QueryString["FinalDate"], CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(Request.QueryString["PosCenterIndex"])) PosCenterIndex = Convert.ToInt32(Request.QueryString["PosCenterIndex"]);

            if (!string.IsNullOrEmpty(Request.QueryString["MessageCenterIndex"])) MessageCenterIndex = Convert.ToInt32(Request.QueryString["MessageCenterIndex"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Ticket"])) Ticket = Convert.ToInt32(Request.QueryString["Ticket"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Distribucion"])) Distribucion = Convert.ToInt32(Request.QueryString["Distribucion"]);
            
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
            Session.Add("Chofer", Chofer);
            Session.Add("TypeMobile", TypeMobile);
            Session.Add("Mobile", Mobile);
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

        protected void DdlDistritoInitialBinding(object sender, EventArgs e)
        {
            if (Distrito > 0) ddlDistrito.EditValue = Distrito;
            else
            {
                if (Mobile > 0)
                {
                    var coche = DAOFactory.CocheDAO.FindById(Mobile);
                    ddlDistrito.EditValue = coche != null && coche.Empresa != null ? coche.Empresa.Id : -1;
                }
            }
        }
        protected void DdlPlantaPreBind(object sender, EventArgs e)
        {
            if (Location > 0) ddlPlanta.EditValue = Location;
            else
            {
                if (Mobile > 0)
                {
                    var coche = DAOFactory.CocheDAO.FindById(Mobile);
                    ddlPlanta.EditValue = coche != null && coche.Linea != null ? coche.Linea.Id : -1;
                }
            }
        }
        protected void DdlEmpleadoPreBind(object sender, EventArgs e)
        {
            if (Chofer > 0) ddlEmpleado.EditValue = Chofer;
            else
            {
                if (Mobile > 0)
                {
                    var coche = DAOFactory.CocheDAO.FindById(Mobile);
                    ddlEmpleado.EditValue = coche != null && coche.Chofer != null ? coche.Chofer.Id : -1;
                }
            }
        }
        protected void DdlTipoVehiculoPreBind(object sender, EventArgs e)
        {
            if (TypeMobile > 0) ddlTipoVehiculo.EditValue = TypeMobile;
            else
            {
                if (Mobile > 0)
                {
                    var coche = DAOFactory.CocheDAO.FindById(Mobile);
                    ddlTipoVehiculo.EditValue = coche != null && coche.TipoCoche != null ? coche.TipoCoche.Id : -1;
                }
            }
        }
        protected void DdlMovilPreBind(object sender, EventArgs e) { if (Mobile > 0) ddlMovil.EditValue = Mobile; }
        protected void DdlTipoPreBind(object sender, EventArgs e) { if (MessageType > 0) ddlTipo.EditValue = MessageType; }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxHours = empresa != null && empresa.Id > 0 ? empresa.MaxHorasMonitor : 24;
            dtvalidator.MaxRange = new TimeSpan(maxHours, 0, 0);

            if (IsPostBack) return;

            this.RegisterCss(ResolveUrl("~/App_Styles/openlayers.css"));
            RegisterExtJsStyleSheet();

            SelectMessages();

            SelectPoisType();

            InitializeMap();

            if (Ticket != 0)
            {
                BtnSearchTicketsClick(null, null);
                lstTicket.SelectedValue = "T-" + Ticket;
                BtnPosicionarTicketClick(null, null);
            }
            else if (Distribucion != 0)
            {
                BtnSearchTicketsClick(null, null);
                lstTicket.SelectedValue = "V-" + Distribucion;
                BtnPosicionarTicketClick(null, null);
            }
            else
            {
                SearchPositions();
            }
        }

        protected void BtnSearchClick(object sender, EventArgs e)
        {
            Distrito = ddlDistrito.Selected;
            Location = ddlPlanta.Selected;
            Chofer = ddlEmpleado.Selected;
            Mobile = ddlMovil.Selected;
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

        protected void BtnSearchTicketsClick(object sender, EventArgs e)
        {
            var tickets = DAOFactory.TicketDAO.GetByTexto(ddlDistrito.SelectedValues, 
                                                          ddlPlanta.SelectedValues, 
                                                          ddlMovil.SelectedValues,
                                                          SecurityExtensions.ToDataBaseDateTime(dtDia.SelectedDate.Date),
                                                          SecurityExtensions.ToDataBaseDateTime(dtDia.SelectedDate.Date.AddDays(1)),
                                                          txtBuscar.Text.Trim());

            var viajes = DAOFactory.ViajeDistribucionDAO.GetByTexto(ddlDistrito.SelectedValues,
                                                                    ddlPlanta.SelectedValues,
                                                                    new[] {-1}, // TRANSPORTISTAS
                                                                    ddlMovil.SelectedValues,
                                                                    SecurityExtensions.ToDataBaseDateTime(dtDia.SelectedDate.Date),
                                                                    SecurityExtensions.ToDataBaseDateTime(dtDia.SelectedDate.Date.AddDays(1)),
                                                                    txtBuscar.Text.Trim());

            lstTicket.Items.Clear();
            foreach (var ticket in tickets)
            {
                lstTicket.Items.Add(new ListItem(ticket.PuntoEntrega.Descripcion, "T-" + ticket.Id));
            }
            foreach (var viaje in viajes)
            {
                lstTicket.Items.Add(new ListItem(viaje.Codigo + " - " + viaje.NumeroViaje, "V-" + viaje.Id));
            }

            TabContainer2.ActiveTabIndex = 3;
        }

        protected void BtnPosicionarTicketClick(object sender, EventArgs e)
        {
            if (lstTicket.SelectedValue.Equals(""))
            {
                infoLabel1.Text = "No se ha seleccionado ningún Ticket.";
                return;
            }
            
            var split = lstTicket.SelectedValue.Split('-');
            var prefijo = split[0];
            var id = Convert.ToInt32((string) split[1]);
            
            switch (prefijo)
            {
                case "T":
                    var ticket = DAOFactory.TicketDAO.FindById(id);

                    var detalles = ticket.Detalles.Cast<DetalleTicket>()
                                                  .Where(d => d.Automatico.HasValue)
                                                  .OrderBy(t => t.Automatico.Value);

                    var primerDetalle = detalles.FirstOrDefault();
                    var ultimoDetalle = detalles.LastOrDefault();

                    InitialDate = primerDetalle != null ? primerDetalle.Automatico.Value : DateTime.UtcNow.Date.ToDataBaseDateTime();

                    if (ticket.Estado == Logictracker.Types.BusinessObjects.Tickets.Ticket.Estados.EnCurso)
                        FinalDate = DateTime.UtcNow;
                    else
                        FinalDate = ultimoDetalle != null ? ultimoDetalle.Automatico.Value : DateTime.UtcNow.Date.AddHours(23).AddMinutes(59).ToDataBaseDateTime();

                    dtDesde.SelectedDate = InitialDate.ToDisplayDateTime();
                    dtHasta.SelectedDate = FinalDate.ToDisplayDateTime();
                    
                    if (ticket.Vehiculo != null)
                        Mobile = ticket.Vehiculo.Id;
                    break;
                case "V":
                    var viaje = DAOFactory.ViajeDistribucionDAO.FindById(id);
                    InitialDate = viaje.InicioReal.HasValue 
                                     ? viaje.InicioReal.Value
                                     : viaje.Inicio;
                    FinalDate = viaje.Fin;
                
                    dtDesde.SelectedDate = InitialDate.ToDisplayDateTime();
                    dtHasta.SelectedDate = FinalDate.ToDisplayDateTime();

                    if (viaje.Vehiculo != null)
                        Mobile = viaje.Vehiculo.Id;
                    break;
            }

            var mensajes = DAOFactory.MensajeDAO.FindAll().Where(m => m.TipoMensaje != null && m.TipoMensaje.DeEstadoLogistico).ToList();
            var msj = DAOFactory.MensajeDAO.FindAll().Where(m => m.Codigo == MessageCode.EstadoLogisticoCumplido.GetMessageCode()
                                                              || m.Codigo == MessageCode.EstadoLogisticoCumplidoEntrada.GetMessageCode()
                                                              || m.Codigo == MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode()
                                                              || m.Codigo == MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode()
                                                              || m.Codigo == MessageCode.EstadoLogisticoCumplidoManualNoRealizado.GetMessageCode()
                                                              || m.Codigo == MessageCode.EstadoLogisticoCumplidoSalida.GetMessageCode()
                                                              || m.Codigo == MessageCode.CicloLogisticoIniciado.GetMessageCode()
                                                              || m.Codigo == MessageCode.CicloLogisticoCerrado.GetMessageCode()).ToList();
            mensajes.AddRange(msj);

            lbMessages.SetSelectedValues(mensajes.Select(m => m.Codigo));
            
            Distrito = ddlDistrito.Selected;
            Location = ddlPlanta.Selected;
            Stopped = tpStopped.SelectedTime;
            Distance = npDistance.Number;
            StoppedEvent = npStoppedEvent.Number;
            MessageType = ddlTipo.Selected;
            PoisTypesIds = lbPuntosDeInteres.SelectedValues;
            MessagesIds = lbMessages.SelectedStringValues;
                        
            SearchPositions();
        }

        protected void LnkQualityMonitorClick(object sender, EventArgs e)
        {
            AddSessionParameters();

            OpenWin("../MonitorDeCalidad/monitorDeCalidad.aspx", "Monitor de Calidad");
        }

        protected void LnkSimulatorClick(object sender, EventArgs e)
        {
            AddSessionParameters();

            OpenWin(ResolveUrl("~/Operacion/Simulador/Default.aspx"), "Simulador de Recorridos");
        }

        #endregion
    }
}