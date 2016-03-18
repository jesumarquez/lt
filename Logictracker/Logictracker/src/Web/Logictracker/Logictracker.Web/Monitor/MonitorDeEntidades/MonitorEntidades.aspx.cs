using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Messaging;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.ContextMenu;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;
using NHibernate.Util;

namespace Logictracker.Monitor.MonitorDeEntidades
{
    public partial class MonitorEntidades : OnLineSecuredPage
    {
        protected override string GetRefference() { return "OPE_MON_ENTIDAD"; }
        protected override InfoLabel LblInfo { get { return null; } }

        protected const int MinZoomLevel = 4;
        protected const int SearchZoomLevel = 13;
        protected static string LayerEntidades { get { return CultureManager.GetLabel("LAYER_ENTIDADES"); } }
        protected static string LayerPoi { get { return CultureManager.GetLabel("LAYER_POI"); } }
        protected static string LayerDir { get { return CultureManager.GetLabel("LAYER_ADDRESS"); } }
        protected static string LayerClientPoi { get { return CultureManager.GetLabel("LAYER_CLIENT_POI"); } }
        protected static string LayerClientArea { get { return CultureManager.GetLabel("LAYER_CLIENT_AREA"); } }
        protected static string LayerAreasPoi { get { return CultureManager.GetLabel("LAYER_POI_AREA"); } }

        private bool _clearLayerEntidades;
        private bool _clearLayerPoi;
        private bool _clearLayerClient;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                ParsearQueryString();
                Monitor.AddCallbackScript(string.Format("$get('clock').innerHTML = '{0}<blink>:</blink>{1}';", DateTime.UtcNow.ToDisplayDateTime().ToString("HH"), DateTime.UtcNow.ToDisplayDateTime().ToString("mm")));
                RegisterStatusCheck();
                RegisterEvents();
                Monitor.AddCallbackScript(string.Format("startClock(new Date({0},{1},{2},{3},{4},{5}));", DateTime.UtcNow.ToDisplayDateTime().Year, DateTime.UtcNow.ToDisplayDateTime().Month, DateTime.UtcNow.ToDisplayDateTime().Day, DateTime.UtcNow.ToDisplayDateTime().Hour, DateTime.UtcNow.ToDisplayDateTime().Minute, DateTime.UtcNow.ToDisplayDateTime().Second));
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                SelectedEntidades = null;
                SelectedMessages = null;
                SelectedLineas = new List<int> { -1 };
                RegisterExtJsStyleSheet();
                Monitor.ImgPath = Config.Monitor.GetMonitorImagesFolder(this);
                Monitor.GoogleMapsScript = GoogleMapsKey;
                Monitor.TimerInterval = 1;
                Monitor.GeocoderMarkersLayer = LayerDir;
                Monitor.DefaultMarkerIcon = "salida.gif";

                var googleMapsEnabled = true;
                var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
                if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
                {
                    var empresa = usuario.Empresas.First() as Empresa;
                    if (empresa != null)
                        googleMapsEnabled = empresa.GoogleMapsEnabled;
                }

                if (googleMapsEnabled)
                {
                    Monitor.AddLayers(
                        LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), MinZoomLevel),
                        //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, MinZoomLevel),
                        LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                        LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), MinZoomLevel),
                        LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), MinZoomLevel),
                        LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), MinZoomLevel),
                        LayerFactory.GetVector(LayerAreasPoi, true),
                        LayerFactory.GetVector(LayerClientArea, true),
                        LayerFactory.GetMarkers(LayerEntidades, true),
                        LayerFactory.GetMarkers(LayerPoi, true),
                        LayerFactory.GetMarkers(LayerClientPoi, true),
                        LayerFactory.GetMarkers(LayerDir, true));
                }
                else
                {
                    Monitor.AddLayers(LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                                        LayerFactory.GetVector(LayerAreasPoi, true),
                                        LayerFactory.GetVector(LayerClientArea, true),
                                        LayerFactory.GetMarkers(LayerEntidades, true),
                                        LayerFactory.GetMarkers(LayerPoi, true),
                                        LayerFactory.GetMarkers(LayerClientPoi, true),
                                        LayerFactory.GetMarkers(LayerDir, true));
                }


                var ctx = ControlFactory.GetContextMenu();
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.None, CultureManager.GetLabel("MAP_OPTIONS"), "", "olContextMenuTitle", true));
                ctx.AddItem(ContextMenuItem.Separator);
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.Postback, CultureManager.GetLabel("MAP_MARK_DIR"), "Consulta", "olContextMenuItem", true));
                ctx.AddItem(ContextMenuItem.Separator);
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.Center, CultureManager.GetLabel("MAP_CENTER"), "", "olContextMenuItem", true));
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.ZoomIn, CultureManager.GetLabel("MAP_ZOOM_IN"), "", "olContextMenuItem", true));
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.ZoomOut, CultureManager.GetLabel("MAP_ZOOM_OUT"), "", "olContextMenuItem", true));

                Monitor.AddControls(ControlFactory.GetLayerSwitcher(),
                                    ControlFactory.GetNavigation(),
                                    ControlFactory.GetPanZoomBar(),
                                    ctx,
                                    ControlFactory.GetToolbar(false, false, false, false, false, false, true, true, LayerAreasPoi));

                if (SelectedLineas.Count == 1)
                {
                    var linea = DAOFactory.LineaDAO.FindById(SelectedLineas.First());
                    SetCenterLinea(linea);
                }
                else
                {
                    Monitor.SetDefaultCenter(-34.6134981326759, -58.4255323559046);
                    Monitor.SetCenter(-34.6134981326759, -58.4255323559046, 10);
                }
            }

            Monitor.Tick += MonitorTick;
            Monitor.ContextMenuPostback += MonitorContextMenuPostback;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack || _selectAllTipoMensaje) SelectAllTipoMensaje();
            if (!IsPostBack || _selectAllMensajes) SelectAllMensajes();

            if (!IsCallback) SetSelectedMensajes();

            if (_clearLayerEntidades)
            {
                Monitor.ClearLayer(LayerEntidades);
            }
            if (_clearLayerPoi)
            {
                Monitor.ClearLayer(LayerPoi);
                Monitor.ClearLayer(LayerAreasPoi);
            }
            if (_clearLayerClient)
            {
                Monitor.ClearLayer(LayerClientArea);
                Monitor.ClearLayer(LayerClientPoi);
            }

            base.OnPreRender(e);
        }

        protected void MonitorContextMenuPostback(object sender, PostbackEventArgs e) { }

        void MonitorTick(object sender, MonitorEventArgs e)
        {
            if (Usuario == null) return;

            try
            {
                Monitor.TimerInterval = 20;
                Monitor.CallbackArgument = DateTime.UtcNow.ToString();

                var count = 0;

                if (SelectedEntidades.Count > 0)
                {
                    foreach (var entidad in SelectedEntidades)
                    {
                        var style = GetMarkerStyle(entidad);

                        AddMarker(entidad, style);
                        count++;
                        if (SelectedEntidades.Count == 1) Monitor.SetCenter(entidad.ReferenciaGeografica.Latitude, entidad.ReferenciaGeografica.Longitude);
                    }

                    if (SelectedMessages.Count > 0)
                    {
                        var selectedDispositivos = SelectedEntidades.Select(entidad => entidad.Dispositivo).ToList();

                        var popups = SharedPositions.GetNewPopupsM2M(selectedDispositivos, DAOFactory)
                                                    .Where(p => SelectedMessages.Contains(p.CodigoMensaje));

                        var sonidos = new List<string>();
                        var lastMsg = LastMessage;
                        var newPopups = 0;
                        foreach (var popup in popups)
                        {
                            if (popup.Id > lastMsg)
                            {
                                Monitor.AddCallbackScript("AddEvent('" + GetMessageM2M(popup) + "');");
                                if (!string.IsNullOrEmpty(popup.Sound) &&
                                    !sonidos.Contains(Config.Directory.SoundsDir + popup.Sound))
                                    sonidos.Add(Config.Directory.SoundsDir + popup.Sound);
                                LastMessage = Math.Max(popup.Id, LastMessage);
                                newPopups++;
                            }
                        }
                        if (sonidos.Count > 0)
                            Monitor.AddCallbackScript("enqueueSounds(['" + string.Join("','", sonidos.ToArray()) + "']);");

                        if (newPopups > 0)
                            Monitor.AddCallbackScript("if(!PopupPanelOpen)ShowEvents();");
                    }
                }

                // Inserto un script personalizado que muestra un mensaje en pantalla
                Monitor.AddCallbackScript(string.Format("$get('{0}').innerHTML = '{1}';",
                                                        lblInfo.ClientID,
                                                        string.Format(CultureManager.GetSystemMessage("ONLINE_UPDATED_POSITIONS"),
                                                                      count,
                                                                      DateTime.UtcNow.ToDisplayDateTime().ToString("HH:mm:ss"))));
            }
            catch (Exception ex)
            {
                // Inserto un script personalizado que muestra el error en pantalla
                Monitor.AddCallbackScript(string.Format("$get('{0}').innerHTML = '{1}';", lblInfo.ClientID, ex.Message));
            }

            Monitor.AddCallbackScript("lastUpdate = new Date();");
        }

        #region Events

        protected void CbLocacionSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            SelectedEmpresa = IdEmpresa;

            _clearLayerEntidades = true;
            _clearLayerPoi = true;
            _clearLayerClient = true;
        }

        protected void CbPlantaSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            foreach (ListItem poi in cbPoi.Items) { poi.Selected = false; }
            foreach (ListItem cli in cbCliente.Items) { cli.Selected = false; }
            //updPoi.Update();
            //updCliente.Update();
            UpdatePanel1.Update();

            _clearLayerEntidades = true;
            _clearLayerPoi = true;
            _clearLayerClient = true;

            SelectedLineas = IdLinea;

            _selectAllTipoMensaje = true;
            _selectAllMensajes = true;

            if (IsPostBack)
            {
                if (SelectedLineas.Count == 1)
                {
                    var linea = DAOFactory.LineaDAO.FindById(SelectedLineas.First());
                    SetCenterLinea(linea);
                }
                else
                {
                    Monitor.SetDefaultCenter(-34.6134981326759, -58.4255323559046);
                    Monitor.SetCenter(-34.6134981326759, -58.4255323559046, 10);
                }
            }

            Monitor.ExecuteScript("ClearEvents(); HideEvents();");
            LastMessage = 0;
        }

        protected void CbTipoEntidadSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerEntidades = true;
            SelectedEntidades = null;
        }

        protected void CbEntidadSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            SetSelectedEntidades();
        }

        protected void CbPoiSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerPoi = false;
            Monitor.ClearLayer(LayerPoi);
            Monitor.ClearLayer(LayerAreasPoi);

            var tipos = Enumerable.ToList<int>((from ListItem cli in cbPoi.Items where cli.Selected select Convert.ToInt32((string) cli.Value)));

            if (tipos.Count == 0) return;
            var list = DAOFactory.ReferenciaGeograficaDAO.GetList(new[] { cbLocacion.Selected }, SelectedLineas, tipos)
                                                         .Where(d => d.Vigencia == null || d.Vigencia.Vigente(DateTime.UtcNow))
                                                         .ToList();

            foreach (var dom in list)
            {
                AddReferenciaGeografica(LayerPoi, LayerAreasPoi, dom, GetPoiPopupContent(dom));
            }
        }

        protected void CbClienteSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerClient = false;
            Monitor.ClearLayer(LayerClientPoi);
            Monitor.ClearLayer(LayerClientArea);

            var clientes = Enumerable.ToList<int>((from ListItem cli in cbCliente.Items where cli.Selected select Convert.ToInt32((string) cli.Value)));

            if (clientes.Count == 0) return;

            foreach (var clientId in clientes)
            {
                var cliente = DAOFactory.ClienteDAO.FindById(clientId);

                if (chkPuntoEntrega.Checked)
                {
                    var puntos = DAOFactory.PuntoEntregaDAO.GetList(new[] { -1 }, new[] { -1 }, new[] { cliente.Id });
                    foreach (var punto in puntos)
                    {
                        AddReferenciaGeografica(LayerClientPoi, LayerClientArea, punto.ReferenciaGeografica,
                                                GetPuntoEntregaPopupContent(punto));
                    }
                }

                if (chkCliente.Checked)
                    AddReferenciaGeografica(LayerClientPoi, LayerClientArea, cliente.ReferenciaGeografica, GetClientPopupContent(cliente));
            }
        }

        private void AddReferenciaGeografica(string layerPoi, string layerArea, ReferenciaGeografica dom, string popupContent)
        {
            if (dom.Poligono != null)
            {
                var center = dom.Poligono.FirstPoint;
                var col = StyleFactory.GetPointFromColor(dom.Color.Color);
                var id = dom.Id + "_GEO";

                if (dom.Poligono.Radio > 0)
                {
                    Monitor.AddGeometries(layerArea, new Point(id, center.X, center.Y, dom.Poligono.Radio, col));
                }
                else
                {
                    var points = dom.Poligono.ToPointFList();
                    var poly = new Polygon(id, col);
                    for (var i = 0; i < points.Count; i++)
                        poly.AddPoint(new Point(id + "_" + i, points[i].X, points[i].Y));

                    Monitor.AddGeometries(layerArea, poly);
                }
            }

            if (dom.Direccion == null) return;

            var icono = dom.Icono != null ? IconDir + dom.Icono.PathIcono : string.Empty;
            var marker = new Marker(dom.Id.ToString("#0"),
                icono,
                dom.Direccion.Latitud,
                dom.Direccion.Longitud,
                popupContent,
                DrawingFactory.GetSize(dom.Icono != null ? dom.Icono.Width : 0, dom.Icono != null ? dom.Icono.Height : 0), DrawingFactory.GetOffset(dom.Icono != null ? dom.Icono.OffsetX : 0, dom.Icono != null ? dom.Icono.OffsetY : 0));

            Monitor.AddMarkers(layerPoi, marker);
        }

        protected void ConnectionStatusClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            SelectedLineas = IdLinea;

            SetSelectedEntidades();

            SetSelectedMensajes();

            Monitor.CallbackArgument = DateTime.UtcNow.ToString();

            Monitor.ResetTimer();
        }

        #endregion

        #region Busqueda

        protected void BtBuscarClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            var entidades = DAOFactory.EntidadDAO.GetByDescripcion(new[] { cbLocacion.Selected },
                                                                   new[] { cbPlanta.Selected },
                                                                   new[] { -1 },
                                                                   new[] { cbTipoEntidad.Selected },
                                                                   txtEntidad.Text.Trim());

            if (!entidades.Any())
            {
                JsAlert(string.Format(CultureManager.GetError("ENTIDAD_NOT_FOUND"), (object) txtEntidad.Text.Trim()));

                SetSelectedEntidades();

                return;
            }

            foreach (var entidad in entidades)
            {
                var style = GetMarkerStyle(entidad);
                AddMarker(entidad, style);
                SelectEntidad(entidad.Id);
            }

            Monitor.SetCenter(entidades.First().ReferenciaGeografica.Latitude, entidades.First().ReferenciaGeografica.Longitude, SearchZoomLevel);

            UpdatePanel1.Update();
        }

        protected void BtBuscarPoiClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            lblPoiResult.Text = string.Empty;

            var empresas = (from l in SelectedLineas
                            select DAOFactory.LineaDAO.FindById(l)
                                into line
                                where line != null && line.Empresa != null
                                select line.Empresa.Id).ToList();

            var pois = DAOFactory.ReferenciaGeograficaDAO.GetByDescripcion(empresas, SelectedLineas, txtPoi.Text.Trim());

            switch (pois.Count())
            {
                case 0: JsAlert(string.Format(CultureManager.GetError("POI_NOT_FOUND"), "\\n\\n\\t" + txtPoi.Text)); break;
                case 1: SelectPoi(pois.First()); break;
                default:
                    MultiViewPoi.SetActiveView(ViewPoiResult);
                    cbPoiResult.DataSource = pois;
                    cbPoiResult.DataBind();
                    cbPoiResult.SelectedIndex = 0;
                    break;
            }

            txtPoi.Text = string.Empty;
        }

        protected void BtCancelPoiClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            cbPoiResult.Items.Clear();
            MultiViewPoi.ActiveViewIndex = 0;
        }

        protected void BtSelectPoiClick(object sender, EventArgs e)
        {
            if (Usuario == null) return;

            SelectPoi(DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32((string) cbPoiResult.SelectedValue)));
            MultiViewPoi.ActiveViewIndex = 0;
            cbPoiResult.Items.Clear();
        }

        #endregion

        #region Helper Functions

        protected void SetCenterLinea(Linea l)
        {
            if (l.ReferenciaGeografica == null || (l.ReferenciaGeografica.Direccion == null && l.ReferenciaGeografica.Poligono == null)) return;

            var lat = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Latitud : l.ReferenciaGeografica.Poligono.Centro.Y;
            var lon = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Longitud : l.ReferenciaGeografica.Poligono.Centro.X;

            Monitor.SetDefaultCenter(lat, lon);
            Monitor.SetCenter(lat, lon, 10);
        }

        protected void AddMarker(EntidadPadre entidad, string style)
        {
            if (entidad != null)
            {
                var icono = IconDir;
                if (entidad.ReferenciaGeografica != null && entidad.ReferenciaGeografica.Icono != null)
                    icono += entidad.ReferenciaGeografica.Icono.PathIcono;
                
                var desc = entidad.Descripcion;
                var refGeo = entidad.ReferenciaGeografica;
                var vehiculoAsociado = DAOFactory.CocheDAO.FindMobileByDevice(entidad.Dispositivo.Id);
                var ultimaPosicion = vehiculoAsociado != null ? DAOFactory.LogPosicionDAO.GetLastVehiclesPositions(new List<Coche>{vehiculoAsociado})[vehiculoAsociado.Id] : null;
                var latitud = ultimaPosicion != null ? ultimaPosicion.Latitud : refGeo != null ? refGeo.Latitude : 0;
                var longitud = ultimaPosicion != null ? ultimaPosicion.Longitud : refGeo != null ? refGeo.Longitude : 0;
                var marker = MarkerFactory.CreateLabeledMarker(entidad.Id.ToString("#0"), icono, latitud, longitud, desc, style, GetMovilPopupContent(entidad));

                if (refGeo != null && refGeo.Icono != null)
                {
                    marker.Size = DrawingFactory.GetSize(refGeo.Icono.Width, refGeo.Icono.Height);
                    marker.Offset = DrawingFactory.GetOffset(refGeo.Icono.OffsetX, refGeo.Icono.OffsetY);
                }

                Monitor.AddMarkers(LayerEntidades, marker);
            }
        }

        private string GetMovilPopupContent(EntidadPadre entidad)
        {
            var empresa = IdEmpresa;

            var linea = IdLinea.Count == 1 ? IdLinea.FirstOrDefault() : -1;

            return "javascript:getEntP(" + entidad.Id + ",'" + empresa + "','" + linea + "')";
        }

        private string GetPoiPopupContent(ReferenciaGeografica dom)
        {
            var lineas = SelectedLineas.Aggregate("", (current, linea) => current + (linea.ToString("#0") + ","));

            lineas = lineas.TrimEnd(',');

            return "javascript:getPOIP('" + dom.Id + "','" + lineas + "')";
        }

        private static string GetClientPopupContent(Cliente cliente)
        {
            const string html = @"<table><tr><td><img src=""{0}"" /></td><td>"
                                + @"<div style=""font-size: 9px; color: #CCCCCC"">{1}</div>"
                                + @"<div style=""font-size: 12px;""><b>{2}</b> ({3})</div>"
                                + @"</td></tr></table><hr />"
                                + "{4}<br/>{5}<br/>{6}<br/>{7}";

            var icon = cliente.ReferenciaGeografica.Icono != null
                            ? cliente.ReferenciaGeografica.Icono.PathIcono
                            : cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono != null
                                    ? cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono.PathIcono
                                    : string.Empty;
            return string.Format(html,
                                 IconDir + icon,
                                 CultureManager.GetEntity("CLIENT"),
                                 cliente.Descripcion,
                                 cliente.Codigo,
                                 cliente.ReferenciaGeografica.Descripcion,
                                 cliente.Comentario1,
                                 cliente.Comentario2,
                                 cliente.Comentario3);
        }

        private static string GetPuntoEntregaPopupContent(PuntoEntrega punto)
        {
            const string html = @"<table><tr><td><img src=""{0}"" /></td><td>"
                                + @"<div style=""font-size: 9px; color: #CCCCCC"">{1}</div>"
                                + @"<div style=""font-size: 12px;""><b>{2}</b> ({3})</div>"
                                + @"</td></tr></table><hr />"
                                + "{4}: {5}<br/>{6}<br/>{7}<br/>{8}<br/>{9}";

            var icon = punto.ReferenciaGeografica.Icono != null
                            ? punto.ReferenciaGeografica.Icono.PathIcono
                            : punto.ReferenciaGeografica.TipoReferenciaGeografica.Icono != null
                                ? punto.ReferenciaGeografica.TipoReferenciaGeografica.Icono.PathIcono
                                : string.Empty;

            return string.Format(html,
                                 IconDir + icon,
                                 CultureManager.GetEntity("PARENTI44"),
                                 punto.Descripcion,
                                 punto.Codigo,
                                 CultureManager.GetEntity("CLIENT"),
                                 punto.Cliente.Descripcion,
                                 punto.ReferenciaGeografica.Descripcion,
                                 punto.Comentario1,
                                 punto.Comentario2,
                                 punto.Comentario3);
        }

        public void JsAlert(string msg)
        {
            Monitor.ExecuteScript("alert(\'" + msg + "\');");
        }

        protected string GetShowMessage(string msg)
        {
            return string.Format("$get('{0}').innerHTML = '{1}';", lblInfo.ClientID, msg.Replace('\n', ' '));
        }

        protected void RegisterStatusCheck()
        {
            var script = @"var lastUpdate = new Date(); setInterval(check_status, 10000);
        function check_status(){
            var secs = Math.floor(new Date().getTime() - lastUpdate.getTime())/1000;
            if (secs > 20){
                $get('" + connection_status.ClientID + @"').src = 'disconnected.png';
                $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_DISCONNECTED") + @"';
                if(secs < 60)"
                         + ClientScript.GetPostBackEventReference(connection_status, "") +
                         @";return false;}
            else {
                $get('" + connection_status.ClientID + @"').src = 'connected.png';
                $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_CONNECTED") + @"'; 
                return true;}}";
            ClientScript.RegisterStartupScript(typeof(string), "check_status", script, true);
            connection_status.OnClientClick = "return !check_status();";
        }

        protected void RegisterEvents()
        {
            var script =
                string.Format(
                    @" var PopupPanelOpen = false;
                function ClearEvents(){{ 
                    var cont = $get('{1}');                           
                    cont.innerHTML = ''; 
                }}
                function ShowEvents(){{ HideDetail(); $get('{0}').style.display = '';PopupPanelOpen = true; }} 
                function HideEvents(){{ $get('{0}').style.display = 'none';PopupPanelOpen = false; }} 
                function ToggleEvents(){{ if($get('{0}').style.display == 'none') ShowEvents(); else HideEvents(); }} 
                function AddEvent(txt){{ 
                    var cont = $get('{1}');
                    if(cont.childNodes.length > 20)
                        cont.removeChild(cont.lastChild);
                            
                    cont.innerHTML = txt + cont.innerHTML; 
                }}
                function ShowDetail(){{  $get('{1}').style.display = 'none'; $get('{2}').style.display = '';PopupPanelOpen = true; }}
                function HideDetail(){{ $get('{1}').style.display = ''; $get('{2}').style.display = 'none'; PopupPanelOpen = false;}}
                function LoadDetail(id){{ $get('ifrPopupDetail').src = 'InfoEventM2M.aspx?evt='+id; ShowDetail(); }}
                ",
                    panelPopup.ClientID,
                    panelPopupEvents.ClientID,
                    panelPopupDetail.ClientID
                    );
            ClientScript.RegisterStartupScript(typeof(string), "popup_events", script, true);

            cbEntidad.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(btCenter, ""));
        }

        protected void BtCenterClick(object sender, EventArgs e)
        {
            if (SelectedEntidades.Count == 1)
            {
                var entidad = SelectedEntidades[0];
                var style = GetMarkerStyle(entidad);
                AddMarker(entidad, style);

                var refGeo = entidad.ReferenciaGeografica;
                var vehiculoAsociado = DAOFactory.CocheDAO.FindMobileByDevice(entidad.Dispositivo.Id);
                var ultimaPosicion = vehiculoAsociado != null ? DAOFactory.LogPosicionDAO.GetLastVehiclesPositions(new List<Coche> { vehiculoAsociado })[vehiculoAsociado.Id] : null;
                var latitud = ultimaPosicion != null ? ultimaPosicion.Latitud : refGeo != null ? refGeo.Latitude : 0;
                var longitud = ultimaPosicion != null ? ultimaPosicion.Longitud : refGeo != null ? refGeo.Longitude : 0;
                Monitor.SetCenter(latitud, longitud);
            }
        }

        protected static string GetMessage(Logictracker.SharedPopup popup)
        {
            return "<div style=\"cursor: pointer; background-color: " + popup.Color + "; border: solid 1px White;color: White; padding: 3px;\" onclick=\"LoadDetail(" + popup.Id + "); this.parentNode.removeChild(this);\">" + popup.DateTime.ToDisplayDateTime().ToString("HH:mm") + ": <b>" + popup.Interno + "</b> - " + popup.Text + "</div>";
        }

        protected static string GetMessageM2M(SharedPopupM2M popup)
        {
            return "<div style=\"cursor: pointer; background-color: " + popup.Color + "; border: solid 1px White;color: White; padding: 3px;\" onclick=\"LoadDetail(" + popup.Id + "); this.parentNode.removeChild(this);\">" + popup.DateTime.ToDisplayDateTime().ToString("HH:mm") + ": <b>" + popup.Sensor + "</b> - " + popup.Text + "</div>";
        }

        private void ParsearQueryString()
        {
            if (Request.QueryString.AllKeys.Contains("ID_ENTIDAD") && Request.QueryString["ID_ENTIDAD"] != string.Empty) 
            {
                var entidad = DAOFactory.EntidadDAO.FindById(Convert.ToInt32(Request.QueryString["ID_ENTIDAD"]));
                cbLocacion.SetSelectedValue(entidad.Empresa != null
                                            ? entidad.Empresa.Id
                                            : entidad.Linea != null
                                              ? entidad.Linea.Empresa.Id
                                              : 0);
                cbPlanta.SetSelectedValue(entidad.Linea != null
                                          ? entidad.Linea.Empresa.Id
                                          : 0);
                cbTipoEntidad.SetSelectedValue(entidad.TipoEntidad.Id);
                cbEntidad.SelectedValue = entidad.Id.ToString("#0");

                SetSelectedEntidades();
            }
        }

        #endregion

        #region Properties

        private List<int> IdLinea
        {
            get
            {
                if (cbPlanta.SelectedIndex <= 0) return Enumerable.ToList<int>((from ListItem item in cbPlanta.Items where item.Value != @"-1" select Convert.ToInt32((string) item.Value)));
                return new List<int> { Convert.ToInt32((string) cbPlanta.SelectedValue) };
            }
        }

        private int IdEmpresa
        {
            get { if (cbLocacion.SelectedIndex < 0) return -1; return Convert.ToInt32((string) cbLocacion.SelectedValue); }
        }

        protected List<int> SelectedLineas
        {
            get
            {
                var v = (List<int>)Session["MONITOR_SelectedLinea"];
                return v ?? IdLinea;
            }
            set
            {
                if (value.Contains(-1))
                    Session["MONITOR_SelectedLinea"] = null;
                else
                    Session["MONITOR_SelectedLinea"] = value;
            }
        }

        protected int SelectedEmpresa
        {
            get
            {
                var v = Session["MONITOR_SelectedEmpresa"];
                return v != null ? (int)v : IdEmpresa;
            }
            set
            {
                if (value == -1)
                    Session["MONITOR_SelectedEmpresa"] = null;
                else
                    Session["MONITOR_SelectedEmpresa"] = value;
            }
        }

        protected List<EntidadPadre> SelectedEntidades
        {
            get { return Session["MONITOR_SelectedEntidades"] != null ? Session["MONITOR_SelectedEntidades"] as List<EntidadPadre> : new List<EntidadPadre>(); }
            set { Session["MONITOR_SelectedEntidades"] = value; }
        }

        protected List<string> SelectedMessages
        {
            get
            {
                return Session["MONITOR_SelectedMessages"] != null ? Session["MONITOR_SelectedMessages"] as List<string> : new List<string>();
            }
            set { Session["MONITOR_SelectedMessages"] = value; }
        }

        protected int LastMessage
        {
            get
            {
                return (int)(Session["MONITOR_LastMessage"] ?? 0);
            }
            set { Session["MONITOR_LastMessage"] = value; }
        }

        #endregion

        #region Selection

        private void SetSelectedEntidades()
        {
            SelectedEntidades = null;

            foreach (var id in from ListItem li in cbEntidad.Items where li.Selected select Convert.ToInt32((string) li.Value))
            {
                SelectEntidad(id);
            }

            _clearLayerEntidades = false;
            Monitor.ClearLayer(LayerEntidades);

            if (SelectedEntidades == null) return;

            foreach (var entidad in SelectedEntidades)
            {
                var style = GetMarkerStyle(entidad);
                AddMarker(entidad, style);
            }
        }

        protected void SelectEntidad(int id)
        {
            var entidad = DAOFactory.EntidadDAO.FindById(id);
            var sv = SelectedEntidades;
            if (!sv.Contains(entidad)) sv.Add(entidad);
            SelectedEntidades = sv;
            var li = cbEntidad.Items.FindByValue(id.ToString("#0"));
            if (li != null) li.Selected = true;
        }

        protected void UnselectEntidad(int id)
        {
            var sv = SelectedEntidades;

            sv.Remove(SelectedEntidades.Where(c => c.Id == id).Select(c => c).FirstOrDefault());

            SelectedEntidades = sv;

            var li = cbEntidad.Items.FindByValue(id.ToString("#0"));
            if (li != null) li.Selected = false;
        }

        private void SelectPoi(ReferenciaGeografica dom)
        {
            var li = cbPoi.Items.FindByValue(dom.TipoReferenciaGeografica.Id.ToString("#0"));
            if (li != null)
            {
                li.Selected = true;
                CbPoiSelectedIndexChanged(cbPoi, EventArgs.Empty);
            }

            var lat = dom.Direccion != null ? dom.Direccion.Latitud : dom.Poligono.Centro.Y;
            var lon = dom.Direccion != null ? dom.Direccion.Longitud : dom.Poligono.Centro.X;
            Monitor.SetCenter(lat, lon, SearchZoomLevel);
        }

        private string GetMarkerStyle(EntidadPadre entidad)
        {
            var subEntidades = DAOFactory.SubEntidadDAO.GetList(new[] { entidad.Empresa.Id },
                                                                new[] { entidad.Linea != null ? entidad.Linea.Id : -1 },
                                                                new[] { entidad.TipoEntidad.Id },
                                                                new[] { entidad.Id },
                                                                new[] { entidad.Dispositivo != null ? entidad.Dispositivo.Id : -1 },
                                                                new[] { -1 });
            var style = "";

            foreach (var subEntidad in subEntidades)
            {
                var ultimaMedicion = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, DateTime.UtcNow);
                var ultimoEventoDescongelamiento = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                                     new[] { Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString("#0"),
                                                                                                             Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonUnpressed).ToString("#0") });
                var ultimoEventoConexion = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                             new[] { Convert.ToInt32(MessageIdentifier.TemperaturePowerReconected).ToString("#0"),
                                                                                                     Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString("#0") });
                var ultimoEventoBotonera = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                             new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton1).ToString("#0"),
                                                                                                     Convert.ToInt32(MessageIdentifier.KeyboardButton2).ToString("#0"),
                                                                                                     Convert.ToInt32(MessageIdentifier.KeyboardButton3).ToString("#0") });

                var enDescogelamiento = ultimoEventoDescongelamiento != null
                                     && ultimoEventoDescongelamiento.Mensaje != null
                                     && ultimoEventoDescongelamiento.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString("#0");

                var energiaDesconectada = ultimoEventoConexion != null
                                       && ultimoEventoConexion.Mensaje != null
                                       && ultimoEventoConexion.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString("#0");

                var enEmergencia = ultimoEventoBotonera != null && ultimoEventoBotonera.Fecha > DateTime.UtcNow.AddMinutes(-10);

                if (ultimaMedicion != null)
                {
                    switch (ultimaMedicion.TipoMedicion.Codigo)
                    {
                        case "TEMP":
                            if (enDescogelamiento)
                                style = "ol_marker_labeled_blue";
                            else
                            {
                                if (energiaDesconectada 
                                    || (ultimaMedicion.ValorDouble > subEntidad.Maximo && subEntidad.ControlaMaximo)
                                    || (ultimaMedicion.ValorDouble < subEntidad.Minimo && subEntidad.ControlaMinimo))
                                    return "ol_marker_labeled_red";

                                if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-3))
                                {
                                    if (style != "ol_marker_labeled_blue")
                                        style = "ol_marker_labeled";
                                }
                                else
                                {
                                    if (style == "")
                                        style = "ol_marker_labeled_green";
                                }
                            }
                            break;
                        case "EST":
                            if (enEmergencia)
                                return "ol_marker_labeled_red";

                            if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-3))
                            {
                                if (style != "ol_marker_labeled_blue")
                                    style = "ol_marker_labeled";
                            }
                            else
                            {
                                if (style == "")
                                    style = "ol_marker_labeled_green";
                            }
                            break;
                        case "NU":
                            if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-2))
                            {
                                if (style != "ol_marker_labeled_blue")
                                    style = "ol_marker_labeled";
                            }
                            else
                            {
                                if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-1))
                                {
                                    if (style == "ol_marker_labeled_green" || style == "")
                                        style = "ol_marker_labeled_yellow";
                                }
                                else
                                {
                                    if (style == "")
                                        style = "ol_marker_labeled_green";
                                }
                            }
                            break;
                        default:
                            if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-2))
                            {
                                if (style != "ol_marker_labeled_blue")
                                    style = "ol_marker_labeled";
                            }
                            else
                            {
                                if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-1))
                                {
                                    if (style == "ol_marker_labeled_green" || style == "")
                                        style = "ol_marker_labeled_yellow";
                                }
                                else
                                {
                                    if (style == "")
                                        style = "ol_marker_labeled_green";
                                }
                            }
                            break;
                    }
                }
            }

            return style != "" ? style : "ol_marker_labeled";
        }

        #endregion

        #region Messages

        private bool _selectAllMensajes;
        private bool _selectAllTipoMensaje;
        protected void CbTipoMensajeSelectedIndexChanged(object sender, EventArgs e)
        {
            _selectAllMensajes = true;
            SelectedMessages = null;
        }

        protected void CbMensajeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            SetSelectedMensajes();
        }

        private void SetSelectedMensajes()
        {
            SelectedMessages = Enumerable.OfType<ListItem>(cbMensajes.Items).Where(li => li.Selected).Select(li => li.Value).ToList();
        }
        private void SelectAllTipoMensaje()
        {
            cbTipoMensaje.SetSelectedValue(-1);
        }
        private void SelectAllMensajes()
        {
            foreach (ListItem li in cbMensajes.Items) li.Selected = true;
            SetSelectedMensajes();
            //updMensajes.Update();
        }

        #endregion
    }
}