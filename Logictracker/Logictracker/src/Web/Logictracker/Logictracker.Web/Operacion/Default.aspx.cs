using Geocoder.Core.VO;
using Logictracker.App_Controls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.Controls;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.ContextMenu;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Logictracker.Operacion
{
    public partial class MonitorOnline : OnLineSecuredPage
    {
        protected override string GetRefference() { return "MONITOR"; }
        protected override InfoLabel LblInfo { get { return null; } }
        
        #region const

        protected const double LatitudCenter = -34.6134981326759;
        protected const double LongitudCenter = -58.4255323559046;

        protected const int MinZoomLevel = 4;
        protected const int SearchZoomLevel = 13;
        protected static string LayerVehiculos { get { return CultureManager.GetLabel("LAYER_VEHICULOS"); } }
        protected static string LayerVehiculosInactivos { get { return CultureManager.GetLabel("LAYER_VEHICULOS_INACTIVOS"); } }
        protected static string LayerPoi { get { return CultureManager.GetLabel("LAYER_POI"); } }
        protected static string LayerDir { get { return CultureManager.GetLabel("LAYER_ADDRESS"); } }
        protected static string LayerClientPoi { get { return CultureManager.GetLabel("LAYER_CLIENT_POI"); } }
        protected static string LayerClientArea { get { return CultureManager.GetLabel("LAYER_CLIENT_AREA"); } }
        protected static string LayerTicketPoi { get { return CultureManager.GetLabel("LAYER_TICKET_POI"); } }
        protected static string LayerTicketArea { get { return CultureManager.GetLabel("LAYER_TICKET_AREA"); } }
        protected static string LayerAreasPoi { get { return CultureManager.GetLabel("LAYER_POI_AREA"); } }
        protected static string LayerTalleresPoi { get { return CultureManager.GetLabel("LAYER_TALLER_POI"); } }
        protected static string LayerTalleresArea { get { return CultureManager.GetLabel("LAYER_TALLER_AREA"); } }
        protected static string LayerCamaras { get { return "Camaras"; } }

        #endregion

        #region Page Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            chkTalleres.Visible = WebSecurity.IsSecuredAllowed(Securables.ViewTaller);

            if (!IsPostBack)
            {
                EmbedSoundPlayer();
                Monitor.AddCallbackScript(string.Format("$get('clock').innerHTML = '{0}:{1}';", DateTime.UtcNow.ToDisplayDateTime().ToString("HH"), DateTime.UtcNow.ToDisplayDateTime().ToString("mm")));
                RegisterStatusCheck();
                RegisterEvents();
                Monitor.AddCallbackScript(string.Format("startClock(new Date({0},{1},{2},{3},{4},{5}));", DateTime.UtcNow.ToDisplayDateTime().Year, DateTime.UtcNow.ToDisplayDateTime().Month, DateTime.UtcNow.ToDisplayDateTime().Day, DateTime.UtcNow.ToDisplayDateTime().Hour, DateTime.UtcNow.ToDisplayDateTime().Minute, DateTime.UtcNow.ToDisplayDateTime().Second));
                


                var autUser = WebSecurity.AuthenticatedUser;
                if (autUser != null)
                {
                    var usuario =  DAOFactory.UsuarioDAO.FindById(autUser.Id);    
                    if (usuario != null && usuario.PorEmpresa)
                    {
                        var totalizadores = new List<short>();
                        foreach (Empresa empresa in usuario.Empresas)
                        {
                            totalizadores.AddRange(empresa.Totalizadores);
                        }
                        pnlTotalizador.Visible = totalizadores.Distinct().Any();
                    }
                }

                SetContextKey();
            }
        }

        #region Init (Map Setup)
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            if (WebSecurity.ShowDriver) cbVehiculo.ShowDriverName = true;

            if (!IsPostBack)
            {
                SelectedVehiculos = null;
                SelectedMessages = null;
                SelectedLineas = new List<int> { -1 };
                //BindPois();

                RegisterExtJsStyleSheet();

                Monitor.ImgPath = Config.Monitor.GetMonitorImagesFolder(this);
                Monitor.GoogleMapsScript = GoogleMapsKey;
                Monitor.TimerInterval = 18;
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
                    Monitor.AddLayers(LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), MinZoomLevel, "poioff", "[{{\"featureType\": \"poi.business\",\"stylers\": [{{ \"visibility\": \"off\" }}]}},{{\"featureType\": \"road\", \"stylers\": [{{ \"visibility\": \"on\" }}] }}]"),
                                      //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, MinZoomLevel),
                                      LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                                      LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), MinZoomLevel),
                                      LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), MinZoomLevel),
                                      LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), MinZoomLevel),
                                      //LayerFactory.GetVector(LAYER_GEOCERCAS, true),
                                      LayerFactory.GetVector(LayerAreasPoi, true),
                                      LayerFactory.GetVector(LayerClientArea, true),
                                      LayerFactory.GetVector(LayerTicketArea, true),
                                      LayerFactory.GetVector(LayerTalleresArea, true),
                                      LayerFactory.GetMarkers(LayerVehiculos, true),
                                      LayerFactory.GetMarkers(LayerVehiculosInactivos, true),
                                      LayerFactory.GetMarkers(LayerPoi, true),
                                      LayerFactory.GetMarkers(LayerClientPoi, true),
                                      LayerFactory.GetMarkers(LayerTicketPoi, true),
                                      LayerFactory.GetMarkers(LayerTalleresPoi, true),
                                      LayerFactory.GetMarkers(LayerDir, true),
                                      LayerFactory.GetMarkers(LayerCamaras, true));
                }
                else
                {
                    Monitor.AddLayers(LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                                      LayerFactory.GetVector(LayerAreasPoi, true),
                                      LayerFactory.GetVector(LayerClientArea, true),
                                      LayerFactory.GetVector(LayerTicketArea, true),
                                      LayerFactory.GetVector(LayerTalleresArea, true),
                                      LayerFactory.GetMarkers(LayerVehiculos, true),
                                      LayerFactory.GetMarkers(LayerVehiculosInactivos, true),
                                      LayerFactory.GetMarkers(LayerPoi, true),
                                      LayerFactory.GetMarkers(LayerClientPoi, true),
                                      LayerFactory.GetMarkers(LayerTicketPoi, true),
                                      LayerFactory.GetMarkers(LayerTalleresPoi, true),
                                      LayerFactory.GetMarkers(LayerDir, true),
                                      LayerFactory.GetMarkers(LayerCamaras, true));
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
                    Monitor.SetDefaultCenter(LatitudCenter, LongitudCenter);
                    Monitor.SetCenter(LatitudCenter, LongitudCenter, 10);
                }
                AddCamaras();                
            }

            Monitor.Tick += MonitorTick;
            Monitor.ContextMenuPostback += MonitorContextMenuPostback;
        }
        #endregion

        private bool _clearLayerVehiculos;
        private bool _clearLayerPoi;
        private bool _clearLayerClient;
        private bool _clearLayerTicket;

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack || _selectAllTipoMensaje) SelectAllTipoMensaje();
            if (!IsPostBack || _selectAllMensajes) SelectAllMensajes();

            if (!IsCallback) SetSelectedMensajes();

            if (_clearLayerVehiculos)
            {
                Monitor.ClearLayer(LayerVehiculos);
                Monitor.ClearLayer(LayerVehiculosInactivos);
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
            if (_clearLayerTicket)
            {
                Monitor.ClearLayer(LayerTicketArea);
                Monitor.ClearLayer(LayerTicketPoi);
            }

            base.OnPreRender(e);
        } 

        #endregion        

        protected void MonitorContextMenuPostback(object sender, PostbackEventArgs e) { if (e.CommandArguments == "Consulta") Consultas1.AddFromLatLon(e.Latitud, e.Longitud); }

        #region Camaras
        
        private void AddCamaras()
        {
            try
            {
                var cameraMarkerList = new List<Marker>();
                var cameras = Config.Camera.Config.Camera;
                foreach (var camera in cameras)
                {
                    var icono = ResolveUrl("~/images/camara.png");
                    var popup = "javascript:getCamP('" + camera.Id + "')";
                    var marker = MarkerFactory.CreateMarker(camera.Id, icono, camera.Latitud, camera.Longitud, popup);
                    marker.Size = DrawingFactory.GetSize(32, 32);
                    marker.Offset = DrawingFactory.GetOffset(-16, -16);                    
                    cameraMarkerList.Add(marker);
                }
                Monitor.AddMarkers(LayerCamaras, cameraMarkerList.ToArray());
            }
            catch(Exception ex)
            {
                STrace.Exception("MonitorOnline.Camaras", ex);
            }
        }
        
        #endregion

        #region Monitor Tick

        void MonitorTick(object sender, MonitorEventArgs e)
        {
            if (Usuario == null) return;

            CalcularTotalizadores(); 
            pnlTotalizador.Update();
            
            try
            {
                Monitor.TimerInterval = 18;
                // Fecha del ultimo Request
                DateTime lastTimestamp;
                if (!DateTime.TryParse(e.EventArgument, out lastTimestamp))
                    lastTimestamp = DateTime.MinValue;

                var count = 0;

                var selected = SelectedVehiculos.Count;
                if (selected > 0)
                {
                    Monitor.CallbackArgument = DateTime.UtcNow.ToString();

                    var nuevas = SharedPositions.GetNewPositions(lastTimestamp, SelectedVehiculos).ToList();
                    if (nuevas.Count > 0)
                    {
                        var coches = GetCochesIn(nuevas.Select(z => z.IdCoche)).ToList();
                        AddMarkers(coches, nuevas);
                        count += nuevas.Count;

                        if (SelectedVehiculos.Count == 1)
                        {
                            var fe = nuevas.First();
                            Monitor.SetCenter(fe.Latitud, fe.Longitud);
                        }
                    }
                    
                    if (!IsPostBack && !IsCallback)
                        LastMessage = 0;

                    if (SelectedMessages.Count > 0)
                    {
                        var popups = DAOFactory.LogMensajeDAO.GetMensajesToPopup(SelectedVehiculos.Select(d => d.Id).ToArray(), 5, LastMessage).Select(popup => new SharedPopup(popup));
                        var perfiles = WebSecurity.AuthenticatedUser.IdPerfiles;
                        popups = popups.Where(p => p.IdPerfil == -1 || perfiles.Contains(-1) || perfiles.Contains(p.IdPerfil));
                        popups = popups.Where(p => SelectedMessages.Contains(p.CodigoMensaje));
                        popups = popups.OrderBy(p => p.Id);

                        var sonidos = new List<string>();
                        var lastMsg = LastMessage;
                        var newPopups = 0;

                        foreach (var popup in popups.Where(p => p.Id > lastMsg))
                        {
                            var requiereatencion = popup.RequiereAtencion ? "true" : "false";
                            Monitor.AddCallbackScript("AddEvent('" + GetMessage(popup) + "'," + requiereatencion + ");");
                            if (!string.IsNullOrEmpty(popup.Sound) && !sonidos.Contains(Config.Directory.SoundsDir + popup.Sound))
                                sonidos.Add(Config.Directory.SoundsDir + popup.Sound);
                            LastMessage = Math.Max(popup.Id, LastMessage);
                            newPopups++;
                        }
                        
                        if (sonidos.Count > 0)
                            Monitor.AddCallbackScript("enqueueSounds(['" + string.Join("','", sonidos.ToArray()) + "']);");
                        
                        if (newPopups > 0)
                            Monitor.AddCallbackScript("if (!PopupPanelOpen) ShowEvents();");
                    }
                }

                var msg2Display = SelectedVehiculos.Count > 0 
                                    ? string.Format(CultureManager.GetSystemMessage("ONLINE_UPDATED_POSITIONS"), count, DateTime.UtcNow.ToDisplayDateTime().ToString("HH:mm:ss")) 
                                    : CultureManager.GetSystemMessage("SELECCIONE_VEHICULO");

                Monitor.AddCallbackScript(string.Format("$get('{0}').innerHTML = '{1}';", lblInfo.ClientID, msg2Display));
            }
            catch (Exception ex)
            {
                // Inserto un script personalizado que muestra el error en pantalla
                Monitor.AddCallbackScript(string.Format("$get('{0}').innerHTML = '{1}';", lblInfo.ClientID, ex.Message));
                STrace.Exception(GetType().FullName, ex);
            }
        }

        #endregion

        //#region DataBindings
        ///// <summary>
        ///// Bindeo de tipos de puntos de interes
        ///// </summary>
        //protected void BindPois()
        //{
        //    var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

        //    var lin = SelectedLineas.Count > 1 ? -1 : SelectedLineas.FirstOrDefault();

        //    var pois = DAOFactory.TipoReferenciaGeograficaDAO.FindByEmpresaLineaYUsuario(cbLocacion.Selected, lin, user);

        //    cbPoi.Items.Clear();

        //    foreach (var poi in pois)
        //    {
        //        var cli = new ListItem(poi.Descripcion, poi.Id.ToString());
        //        cbPoi.Items.Add(cli);
        //    }
        //}

        //#endregion

        #region Events

        protected void CbLocacionSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            SelectedEmpresa = IdEmpresa;
            Mensajeria1.IdEmpresa = IdEmpresa;

            _clearLayerVehiculos = true;
            _clearLayerPoi = true;
            _clearLayerClient = true;

            ChkTalleresCheckedChanged(sender, e);

            lstTicket.Items.Clear();
            lstTicket.DataBind();
            updTickets.Update();
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

            _clearLayerVehiculos = true;
            _clearLayerPoi = true;
            _clearLayerClient = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            //Monitor.ClearLayer(LAYER_POI);
            //Monitor.ClearLayer(LAYER_AREAS_POI);
            //Monitor.ClearLayer(LAYER_CLIENT_AREA);
            //Monitor.ClearLayer(LAYER_CLIENT_POI);
            //Monitor.ClearLayer(LAYER_GEOCERCAS);

            SelectedLineas = IdLinea;

            _selectAllTipoMensaje = true;
            _selectAllMensajes = true;

            //if (SelectedLinea <= 0)
            //{
            //    Monitor.AddCallbackScript(GetShowMessage("El distrito seleccionado no posee ninguna base"));
            //    return;
            //}

            Mensajeria1.IdLinea = IdLinea.Count == 1 ? IdLinea.FirstOrDefault() : -1;
            //BindPois();

            if (IsPostBack)
            {
                if (SelectedLineas.Count == 1)
                {
                    var linea = DAOFactory.LineaDAO.FindById(SelectedLineas.First());
                    SetCenterLinea(linea);
                }
                else
                {
                    Monitor.SetDefaultCenter(LatitudCenter, LongitudCenter);
                    Monitor.SetCenter(LatitudCenter, LongitudCenter, 10);
                }
            }

            Monitor.ExecuteScript("ClearEvents(); HideEvents();");
            LastMessage = 0;

            ChkTalleresCheckedChanged(sender, e);

            lstTicket.Items.Clear();
            lstTicket.DataBind();
            updTickets.Update();
        }

        protected void CbTransportistaSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerVehiculos = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            SelectedVehiculos = null;
            Mensajeria1.UpdateVehicles();
            lstTicket.Items.Clear();
            lstTicket.DataBind();
            updTickets.Update();
        }

        protected void CbCentroDeCostosSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerVehiculos = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            SelectedVehiculos = null;
            Mensajeria1.UpdateVehicles();
        }

        protected void CbSubCentroDeCostosSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerVehiculos = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            SelectedVehiculos = null;
            Mensajeria1.UpdateVehicles();
        }

        protected void CbDepartamentoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerVehiculos = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            SelectedVehiculos = null;
            Mensajeria1.UpdateVehicles();
        }

        protected void CbTipoVehiculoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerVehiculos = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            SelectedVehiculos = null;
            Mensajeria1.UpdateVehicles();
        }

        protected void CbTipoEmpleadoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerVehiculos = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            SelectedVehiculos = null;
            Mensajeria1.UpdateVehicles();
        }

        protected void CbEmpleadoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerVehiculos = true;
            //Monitor.ClearLayer(LAYER_VEHICULOS);
            SelectedVehiculos = null;
            Mensajeria1.UpdateVehicles();
        }

        protected void CbVehiculoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");
            if (IsPostBack)
            {
                    Monitor.AddCallbackScript(
                        string.Format("$get('{0}').innerHTML = '{1}';",
                                      lblInfo.ClientID,
                                      string.Format(CultureManager.GetSystemMessage("PLEASE_WAIT"))));

                Monitor.CallbackArgument = DateTime.UtcNow.ToString();                

                if (!IsCallback)
                    Monitor.ExecuteCallServerTick(true);
                else
                {
                    Monitor.ResetTimer();
                    return;                   
                }                
            }

            var cocheIds = (from ListItem li in cbVehiculo.Items where li.Selected select Convert.ToInt32(li.Value)).ToList();
            var coches = GetCochesIn(cocheIds).ToList();
            SetSelectedVehiculos(true, coches);            
            ChkTicketsCheckChanged(sender, e);
        }

        protected void CbPoiSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerPoi = false;            
            Monitor.ClearLayer(LayerPoi);
            Monitor.ClearLayer(LayerAreasPoi);

            var tipos = (from ListItem cli in cbPoi.Items where cli.Selected select Convert.ToInt32(cli.Value)).ToList();

            if (tipos.Count == 0) return;
            var list = DAOFactory.ReferenciaGeograficaDAO.GetList(new[] {cbLocacion.Selected}, SelectedLineas, tipos)
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

            var clientes = (from ListItem cli in cbCliente.Items where cli.Selected select Convert.ToInt32(cli.Value)).ToList();

            if (clientes.Count == 0) return;

            foreach (var clientId in clientes)
            {
                var cliente = DAOFactory.ClienteDAO.FindById(clientId);

                if (chkPuntoEntrega.Checked)
                {
                    var puntos = DAOFactory.PuntoEntregaDAO.GetList(new[]{-1}, new[]{-1}, new[]{cliente.Id});
                    foreach (PuntoEntrega punto in puntos)
                    {
                        AddReferenciaGeografica(LayerClientPoi, LayerClientArea, punto.ReferenciaGeografica,
                                                GetPuntoEntregaPopupContent(punto));
                    }
                }

                if(chkCliente.Checked)
                    AddReferenciaGeografica(LayerClientPoi, LayerClientArea, cliente.ReferenciaGeografica, GetClientPopupContent(cliente));
            }
        }

        protected void ChkTicketsCheckChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerTicket = false;
            Monitor.ClearLayer(LayerTicketPoi);
            Monitor.ClearLayer(LayerTicketArea);

            if (!chkTickets.Checked) return;

            var coches = (from ListItem c in cbVehiculo.Items where c.Selected select Convert.ToInt32(c.Value)).ToList();

            if (coches.Count == 0) return;

            foreach (var cocheId in coches)
            {
                var coche = DAOFactory.CocheDAO.FindById(cocheId);
                var ticket = coche.Dispositivo != null ? DAOFactory.TicketDAO.FindEnCurso(coche.Dispositivo) : null;
                var distribucion = DAOFactory.ViajeDistribucionDAO.FindEnCurso(coche);
                var puntos = new List<PuntoEntrega>();
                var refGeo = new List<ReferenciaGeografica>();
                
                if (ticket != null)
                {
                    if (ticket.Linea != null && ticket.Linea.ReferenciaGeografica != null)
                        AddReferenciaGeografica(LayerTicketPoi, LayerTicketArea, ticket.Linea.ReferenciaGeografica, GetPoiPopupContent(ticket.Linea.ReferenciaGeografica));

                    if (ticket.PuntoEntrega != null && ticket.PuntoEntrega.ReferenciaGeografica != null)
                        AddReferenciaGeografica(LayerTicketPoi, LayerTicketArea, ticket.PuntoEntrega.ReferenciaGeografica, GetPuntoEntregaPopupContent(ticket.PuntoEntrega));
                }
                else
                {
                    if (distribucion != null)
                    {
                        if (distribucion.Linea != null && distribucion.Linea.ReferenciaGeografica != null)
                            AddReferenciaGeografica(LayerTicketPoi, LayerTicketArea, distribucion.Linea.ReferenciaGeografica, GetPoiPopupContent(distribucion.Linea.ReferenciaGeografica));

                        if (distribucion.Detalles != null)
                        {
                            var puntosEntrega = distribucion.Detalles.OfType<EntregaDistribucion>().Where(en => en.PuntoEntrega != null).Select(en => en.PuntoEntrega);
                            puntos.AddRange(puntosEntrega);
                            refGeo.AddRange(puntosEntrega.Select(en => en.ReferenciaGeografica));
                        }

                        for (var i = 0; i < puntos.Count; i++)
                        {
                            AddReferenciaGeografica(LayerTicketPoi, LayerTicketArea, refGeo[i], GetPuntoEntregaPopupContent(puntos[i]));
                        }
                    }
                }
            }
        }

        protected void BtnCochesConTickets_OnClick(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");
            
            var cocheIds = (from ListItem c in cbVehiculo.Items select Convert.ToInt32(c.Value)).ToList();                       
            if (cocheIds.Count == 0) return;

            var coches = GetCochesIn(cocheIds);
            var cochesToSelect = new List<Coche>();

            foreach (var coche in coches)
            {
                var ticket = coche.Dispositivo != null ? DAOFactory.TicketDAO.FindEnCurso(coche.Dispositivo) : null;
                var distribucion = DAOFactory.ViajeDistribucionDAO.FindEnCurso(coche);

                if (ticket != null || distribucion != null)
                    cochesToSelect.Add(coche);
            }
            cbVehiculo.ClearSelection();
            SetSelectedVehiculos(false, cochesToSelect);            
            ChkTicketsCheckChanged(sender, e);
        }

        protected void LstTicketsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            var tickets = GetSelectedStringValues();

            if (tickets.Count == 0) return;

            cbVehiculo.ClearSelection(); 
            cbPoi.ClearSelection();
            Monitor.ClearLayer(LayerPoi);
            Monitor.ClearLayer(LayerAreasPoi);

            var coches = new List<Coche>();

            foreach (var ticketId in tickets)
            {
                var id = Convert.ToInt32(ticketId);
                Coche coche = null;

                if (id > 0)
                {
                    var distribucion = DAOFactory.ViajeDistribucionDAO.FindById(id);
                    if (distribucion != null)
                    {
                        coche = distribucion.Vehiculo;
                        var georefs = distribucion.Detalles.Select(d => d.ReferenciaGeografica).ToList();
                        foreach (var georef in georefs)
                        {
                            AddReferenciaGeografica(LayerPoi, LayerAreasPoi, georef, GetPoiPopupContent(georef));
                        }
                    }
                    
                    if (coche != null)
                    {
                        coches.Add(coche);
                        cbCentroDeCostos.SetSelectedValue(0);
                        cbSubCentroDeCostos.SetSelectedValue(0);
                        cbDepartamento.SetSelectedValue(0);
                        //cbTransportista.SetSelectedValue(0);
                        cbTipoVehiculo.SetSelectedValue(0);                        
                    }
                }
            }

            SetSelectedVehiculos(false, coches);
        }

        private List<string> GetSelectedStringValues()
        {
            return lstTicket.SelectedIndex < 0 
                        ? new List<string>(0) 
                        : (from index in lstTicket.GetSelectedIndices() select lstTicket.Items[index].Value).ToList();
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
            if (Usuario == null) Response.Redirect("~/");

            SelectedLineas = IdLinea;

            var cocheIds = (from ListItem c in cbVehiculo.Items where c.Selected select Convert.ToInt32(c.Value)).ToArray();
            var coches = GetCochesIn(cocheIds).ToList();

            SetSelectedVehiculos(false, coches);

            SetSelectedMensajes();

            Monitor.CallbackArgument = DateTime.UtcNow.ToString();
            Monitor.AddCallbackScript(
                    string.Format(  "$get('{0}').innerHTML = '{1}';",
                                    lblInfo.ClientID, 
                                    string.Format(CultureManager.GetSystemMessage("PLEASE_WAIT"))));
            Monitor.ExecuteCallServerTick(true);
        }

        protected void ChkTalleresCheckedChanged(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            _clearLayerPoi = false;
            Monitor.ClearLayer(LayerTalleresArea);
            Monitor.ClearLayer(LayerTalleresPoi);

            if (!chkTalleres.Checked) return;

            var talleres = DAOFactory.TallerDAO.FindAll().Where(t => !t.Baja);
            foreach (var taller in talleres)
            {
                if (taller.ReferenciaGeografica != null)
                    AddReferenciaGeografica(LayerTalleresPoi, LayerTalleresArea, taller.ReferenciaGeografica, GetPoiPopupContent(taller.ReferenciaGeografica));
            }
        }

        protected void CbTotalizadorSelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularTotalizadores();
        }

        private void CalcularTotalizadores()
        {
            if (!pnlTotalizador.Visible) return;

            var coches = new Coche[] {};
            var cochesIds = (from ListItem item in cbVehiculo.Items select Convert.ToInt32(item.Value));
            
            var lastEvents = new List<LogMensaje>();
            IEnumerable<Coche> conectados = new List<Coche>();
            IEnumerable<Coche> desconectados = new List<Coche>();

            if (cbTotalizador.Selected != Coche.Totalizador.Ninguno)
                coches = DAOFactory.CocheDAO.GetByIds(cochesIds).ToArray();

            var on = 0;
            var off = 0;
            var na = 0;
            var total = cochesIds.Count();

            lblTotal.Text = "Total: " + total;
            
            var strOn = new StringBuilder();
            var strOff = new StringBuilder();
            var strNa = new StringBuilder();

            switch (cbTotalizador.Selected)
            {
                case Coche.Totalizador.EstadoMotor:
                    lastEvents = DAOFactory.LastVehicleEventDAO.GetEventsByVehiclesAndType(coches, Coche.Totalizador.EstadoMotor).ToList();
                    conectados = lastEvents.Where(ev => ev.Mensaje.Codigo == MessageCode.EngineOn.GetMessageCode()).Select(ev => ev.Coche);
                    desconectados = lastEvents.Where(ev => ev.Mensaje.Codigo == MessageCode.EngineOff.GetMessageCode()).Select(ev => ev.Coche);

                    on = conectados.Count();
                    off = desconectados.Count();
                    na = total - on - off;

                    foreach (var coche in conectados)
                    {
                        if (strOn.Length > 0) strOn.Append(',');
                        strOn.Append(coche.Id);
                    }
                    foreach (var coche in desconectados)
                    {
                        if (strOff.Length > 0) strOff.Append(',');
                        strOff.Append(coche.Id);
                    }
                    foreach (ListItem item in cbVehiculo.Items)
                    {
                        var idCoche = Convert.ToInt32(item.Value);
                        if (!conectados.Select(c => c.Id).Contains(idCoche)
                         && !desconectados.Select(c => c.Id).Contains(idCoche))
                        {
                            if (strNa.Length > 0) strNa.Append(',');
                            strNa.Append(idCoche);
                        }
                    }


                    break;
                case Coche.Totalizador.EstadoGps:
                    lastEvents = DAOFactory.LastVehicleEventDAO.GetEventsByVehiclesAndType(coches, Coche.Totalizador.EstadoGps).ToList();
                    conectados = lastEvents.Where(ev => ev.Mensaje.Codigo == MessageCode.PrivacyOff.GetMessageCode()).Select(ev => ev.Coche);
                    desconectados = lastEvents.Where(ev => ev.Mensaje.Codigo == MessageCode.PrivacyOn.GetMessageCode()).Select(ev => ev.Coche);

                    on = conectados.Count();
                    off = desconectados.Count();
                    na = total - on - off;

                    foreach (var coche in conectados)
                    {
                        if (strOn.Length > 0) strOn.Append(',');
                        strOn.Append(coche.Id);
                    }
                    foreach (var coche in desconectados)
                    {
                        if (strOff.Length > 0) strOff.Append(',');
                        strOff.Append(coche.Id);
                    }
                    foreach (ListItem item in cbVehiculo.Items)
                    {
                        var idCoche = Convert.ToInt32(item.Value);
                        if (!conectados.Select(c => c.Id).Contains(idCoche)
                         && !desconectados.Select(c => c.Id).Contains(idCoche))
                        {
                            if (strNa.Length > 0) strNa.Append(',');
                            strNa.Append(idCoche);
                        }
                    }
                    break;
                case Coche.Totalizador.EstadoGarmin:
                    lastEvents = DAOFactory.LastVehicleEventDAO.GetEventsByVehiclesAndType(coches, Coche.Totalizador.EstadoGarmin).ToList();
                    conectados = lastEvents.Where(ev => ev.Mensaje.Codigo == MessageCode.GarminOn.GetMessageCode()).Select(ev => ev.Coche);
                    desconectados = lastEvents.Where(ev => ev.Mensaje.Codigo == MessageCode.GarminOff.GetMessageCode()).Select(ev => ev.Coche);

                    on = conectados.Count();
                    off = desconectados.Count();
                    na = total - on - off;

                    foreach (var coche in conectados)
                    {
                        if (strOn.Length > 0) strOn.Append(',');
                        strOn.Append(coche.Id);
                    }
                    foreach (var coche in desconectados)
                    {
                        if (strOff.Length > 0) strOff.Append(',');
                        strOff.Append(coche.Id);
                    }
                    foreach (ListItem item in cbVehiculo.Items)
                    {
                        var idCoche = Convert.ToInt32(item.Value);
                        if (!conectados.Select(c => c.Id).Contains(idCoche)
                         && !desconectados.Select(c => c.Id).Contains(idCoche))
                        {
                            if (strNa.Length > 0) strNa.Append(',');
                            strNa.Append(idCoche);
                        }
                    }
                    break;
                case Coche.Totalizador.TicketEnCurso:
                    foreach (var coche in coches)
                    {
                        var distribucion = DAOFactory.ViajeDistribucionDAO.FindEnCurso(coche);
                        
                        if (distribucion != null)
                        {
                            on++;
                            if (strOn.Length > 0) strOn.Append(',');
                            strOn.Append(coche.Id);
                        }
                        else
                        {
                            off++;
                            if (strOff.Length > 0) strOff.Append(',');
                            strOff.Append(coche.Id);
                        }
                    }
                    break;
            }
            lnkOn.Text = String.Format("{0}: {1}", cbTotalizador.Selected == Coche.Totalizador.TicketEnCurso ? CultureManager.GetLabel("SI") : CultureManager.GetSystemMessage("COUNTERS_ON"), on);
            lnkOff.Text = String.Format("{0}: {1}", cbTotalizador.Selected == Coche.Totalizador.TicketEnCurso ? CultureManager.GetLabel("NO") : CultureManager.GetSystemMessage("COUNTERS_OFF"), off);
            lnkNA.Text = String.Format("{0}: {1}", CultureManager.GetSystemMessage("COUNTERS_NOT_AVAILABLE"), na);
            lnkOn.Attributes.Add("coches", strOn.ToString());
            lnkOff.Attributes.Add("coches", strOff.ToString());
            lnkNA.Attributes.Add("coches", strNa.ToString());
        }

        protected void LnkOnClick(object sender, EventArgs e)
        {
            var lnk = sender as LinkButton;
            if (lnk == null) return;

            var coches = new List<Coche>();
            
            if (lnk.Attributes["coches"] != null && lnk.Attributes["coches"] != string.Empty)
            {
                var cochesIds = lnk.Attributes["coches"].Split(',').Select(id => Convert.ToInt32(id));
                coches = DAOFactory.CocheDAO.GetByIds(cochesIds).ToList();
            }

            cbVehiculo.ClearSelection();
            SetSelectedVehiculos(true, coches);
        }

        protected void BtnBuscarTicketsOnClick(object sender, EventArgs e)
        {
            lstTicket.Items.Clear();

            var viajes = DAOFactory.ViajeDistribucionDAO.GetList(cbLocacion.SelectedValues,
                                                                 cbPlanta.SelectedValues,
                                                                 cbTransportista.SelectedValues,
                                                                 new[] { -1 }, // DEPTOS
                                                                 new[] { -1 }, // CENTROS DE COSTO
                                                                 new[] { -1 }, // SUB CENTROS DE COSTO
                                                                 new[] { -1 }, // COCHES
                                                                 new int[] { ViajeDistribucion.Estados.EnCurso },
                                                                 DateTime.Today.ToDataBaseDateTime().AddMonths(-3),
                                                                 DateTime.Now.ToDataBaseDateTime())
                                                        .OrderBy(t => GetDescripcionViaje(t))
                                                        .ToList();

            var items = new List<ListItem>();
            items.AddRange(viajes.Select(viaje => new ListItem(GetDescripcionViaje(viaje), viaje.Id.ToString("#0"))));

            if (!string.IsNullOrEmpty(txtTickets.Text.Trim()))
                items = items.Where(i => i.Text.ToLower().Contains(txtTickets.Text.ToLower())).ToList();

            lstTicket.Items.AddRange(items.ToArray());
            lstTicket.DataBind();
        }

        private static string GetDescripcionViaje(ViajeDistribucion viaje)
        {
            var str = viaje.Codigo + " - " + viaje.NumeroViaje;

            if (viaje.Vehiculo != null) str = str + " (" + viaje.Vehiculo.Interno + ")";

            return str;
        }

        #endregion

        #region Busqueda

        #region Vehiculo
        /// <summary>
        /// Busqueda por Interno
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtBuscarClick(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            IEnumerable<LogUltimaPosicionVo> upms = new List<LogUltimaPosicionVo>();
            var coche = autoVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(autoVehiculo.Selected) : null;
            var coches = new List<Coche>();

            if (coche != null)
                coches.Add(coche);
            else
                coches = DAOFactory.CocheDAO.GetList(cbLocacion.SelectedValues, cbPlanta.SelectedValues, new[] {-1},
                                                     new[] {-1}, new[] {-1}, new[] {-1}, new[] {-1}, true)
                                            .Where(c => c.CompleteDescripcion().ToLower().Contains(autoVehiculo.Text))
                                            .ToList();
            
            if (coches.Any(c => c != null))
            {
                upms = SharedPositions.GetLastPositions(coches);
            }

            cbVehiculo.ClearSelection();
            SetSelectedVehiculos(false, coches);

            if (!upms.Any())
            {
                JsAlert(string.Format(CultureManager.GetError("INTERNO_NOT_FOUND"), autoVehiculo.Text.Trim()));
                return;
            }

            AddMarkers(coches, upms);

            //updVehiculos.Update();
            var fe = upms.FirstOrDefault();
            if (fe != null)
                Monitor.SetCenter(fe.Latitud, fe.Longitud, SearchZoomLevel);

            UpdatePanel1.Update();
        }

        protected void BtnBuscarEntregaClick(object sender, EventArgs e)
        {
            if (IsCallback) return;
            if (Usuario == null) Response.Redirect("~/");

            cbVehiculo.ClearSelection();
            cbPoi.ClearSelection();
            Monitor.ClearLayer(LayerPoi);
            Monitor.ClearLayer(LayerAreasPoi);

            IList<Coche> coches = new List<Coche>();

            var entregas = DAOFactory.EntregaDistribucionDAO.GetList(cbLocacion.SelectedValues,
                                                                     cbPlanta.SelectedValues,
                                                                     cbTransportista.SelectedValues,
                                                                     cbDepartamento.SelectedValues,
                                                                     cbCentroDeCostos.SelectedValues,
                                                                     cbSubCentroDeCostos.SelectedValues,
                                                                     new[] { -1 }, // VEHICULOS
                                                                     new[] { -1 }, // VIAJES
                                                                     new[] { -2 }, // ESTADOS
                                                                     DateTime.Today.ToDataBaseDateTime(),
                                                                     DateTime.UtcNow.ToDataBaseDateTime())
                                                            .Where(ent => ent.Descripcion.Trim().ToLowerInvariant().Contains(txtEntrega.Text.Trim().ToLowerInvariant()));
                
            foreach (var entrega in entregas)
            {
                var coche = entrega.Viaje.Vehiculo;
                AddReferenciaGeografica(LayerPoi, LayerAreasPoi, entrega.ReferenciaGeografica, GetPoiPopupContent(entrega.ReferenciaGeografica));

                if (coche != null)
                {
                    coches.Add(coche);
                    cbCentroDeCostos.SetSelectedValue(0);
                    cbSubCentroDeCostos.SetSelectedValue(0);
                    cbDepartamento.SetSelectedValue(0);
                    //cbTransportista.SetSelectedValue(0);
                    cbTipoVehiculo.SetSelectedValue(0);
                }
            }

            SetSelectedVehiculos(false, coches);
        }

        protected void SetContextKey()
        {   
            autoVehiculo.ContextKey = AutoCompleteTextBox.CreateContextKey(new[] { cbLocacion.Selected},
                                                                           new[] { cbPlanta.Selected});
        }

        #endregion

        #region Punto de Interes
        /// <summary>
        /// Busqueda de POI por palabra clave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtBuscarPoiClick(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

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
                    
                    cbPoiResult.Items.Clear();
                    foreach (var poi in pois)
                    {
                        cbPoiResult.Items.Add(new ListItem(poi.Codigo + " - " + poi.Descripcion, poi.Id.ToString("#0")));
                    }

                    cbPoiResult.SelectedIndex = 0;
                    break;
            }

            txtPoi.Text = string.Empty;
        }

        /// <summary>
        /// Cancelar seleccion de POI cuando la busqueda devuelve multiples resultados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtCancelPoiClick(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            cbPoiResult.Items.Clear();
            MultiViewPoi.ActiveViewIndex = 0;
        }

        /// <summary>
        /// Aceptar seleccion de POI cuando la busqueda devuelve multiples resultados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtSelectPoiClick(object sender, EventArgs e)
        {
            if (Usuario == null) Response.Redirect("~/");

            SelectPoi(DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32(cbPoiResult.SelectedValue)));
            MultiViewPoi.ActiveViewIndex = 0;
            cbPoiResult.Items.Clear();
        }
        #endregion

        #endregion

        #region Helper Functions

        #region Center On Planta/Linea
        /// <summary>
        /// Setea el DefaultCenter y centra el mapa en la posicion de la Linea
        /// </summary>
        /// <param name="l">La linea</param>
        protected void SetCenterLinea(Linea l)
        {
            if (l.ReferenciaGeografica == null || (l.ReferenciaGeografica.Direccion == null && l.ReferenciaGeografica.Poligono == null)) return;

            var lat = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Latitud : l.ReferenciaGeografica.Poligono.Centro.Y;
            var lon = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Longitud : l.ReferenciaGeografica.Poligono.Centro.X;

            Monitor.SetDefaultCenter(lat, lon);
            Monitor.SetCenter(lat, lon, 10);
        }
        #endregion

        #region Add Marker

        private LabeledMarker makeMarker(LogUltimaPosicionVo upm, Coche coche)
        {
            if (upm == null) return null;

            var icono = IconDir + upm.IconoNormal;

            string style;

            if (upm.FechaMensaje >= DateTime.UtcNow.AddMinutes(-coche.Empresa.MinutosEnAmarillo))
            {
                style = upm.Velocidad == 0 ? "ol_marker_labeled" : "ol_marker_labeled_green";
            }
            else
            {
                style = upm.FechaMensaje >= DateTime.UtcNow.AddHours(-coche.Empresa.HorasEnRojo)
                            ? "ol_marker_labeled_yellow"
                            : "ol_marker_labeled_red";
            }

            var desc = upm.Coche;
            if (WebSecurity.ShowDriver)
            {
                desc = coche.Chofer != null
                           ? coche.Chofer.Entidad.Descripcion
                           : CultureManager.GetControl("DDL_NO_EMPLOYEE");
            }
            var icon = coche.TipoCoche.IconoNormal;
            var marker = MarkerFactory.CreateLabeledMarker(upm.IdCoche.ToString("#0"), icono, upm.Latitud, upm.Longitud, desc, style, GetMovilPopupContent(upm));
            marker.Size = DrawingFactory.GetSize(icon.Width, icon.Height);
            marker.Offset = DrawingFactory.GetOffset(icon.OffsetX, icon.OffsetY);
            return marker;
        }

        private IEnumerable<Coche> GetCochesIn(IEnumerable<int> ids)
        {
            return DAOFactory.CocheDAO.GetByIds(ids);
        }

        /// <summary>
        /// Agrega markers al mapa para los moviles
        /// </summary>
        /// <param name="coches">Lista de coches precacheados de las upms</param>
        /// <param name="upms">La <see cref="SharedLastPosition"/> del movil</param>
        protected void AddMarkers(IEnumerable<Coche> coches, IEnumerable<LogUltimaPosicionVo> upms)
        {
            var upmDic = upms.ToDictionary(z => z.IdCoche, z => z);
            var markers = upmDic.Select(z => makeMarker(z.Value, coches.First(c => z.Value != null && c.Id == z.Value.IdCoche)));
//            var markers = coches.Select(z => makeMarker(upmDic[z.Id], z));
            var activos = markers.Where(m => m.LabelStyle != "ol_marker_labeled_red").ToArray();
            var inactivos = markers.Where(m => m.LabelStyle == "ol_marker_labeled_red").ToArray();
            Monitor.AddMarkers(LayerVehiculos, activos);
            Monitor.AddMarkers(LayerVehiculosInactivos, inactivos);
        }        

        /// <summary>
        /// Agrega un marker al mapa para un movil
        /// </summary>
        /// <param name="upm">La <see cref="SharedLastPosition"/> del movil</param>
        protected void AddMarker(LogUltimaPosicionVo upm)
        {
            if (upm == null) return;

            var coche = DAOFactory.CocheDAO.FindById(upm.IdCoche);
            var icono = IconDir + upm.IconoNormal;

            string style;

            if (upm.FechaMensaje >= DateTime.UtcNow.AddMinutes(-coche.Empresa.MinutosEnAmarillo))
            {
                style = upm.Velocidad == 0 ? "ol_marker_labeled" : "ol_marker_labeled_green";
            }
            else
            {
                style = upm.FechaMensaje >= DateTime.UtcNow.AddHours(-coche.Empresa.HorasEnRojo) 
                            ? "ol_marker_labeled_yellow"
                            : "ol_marker_labeled_red";
            }
            
            var desc = upm.Coche;
            if (WebSecurity.ShowDriver)
            {
                desc = coche.Chofer != null
                           ? coche.Chofer.Entidad.Descripcion
                           : CultureManager.GetControl("DDL_NO_EMPLOYEE");
            }
            var icon = coche.TipoCoche.IconoNormal;
            var marker = MarkerFactory.CreateLabeledMarker(upm.IdCoche.ToString("#0"), icono, upm.Latitud, upm.Longitud, desc, style, GetMovilPopupContent(upm));
            marker.Size = DrawingFactory.GetSize(icon.Width, icon.Height);
            marker.Offset = DrawingFactory.GetOffset(icon.OffsetX, icon.OffsetY);

            Monitor.AddMarkers(style == "ol_marker_labeled_red" ? LayerVehiculosInactivos : LayerVehiculos, marker);
        }
        #endregion

        #region Popup Content
        /// <summary>
        /// Devuelve al contenido del popup para un coche
        /// </summary>
        /// <param name="upm">La <see cref="SharedLastPosition"/> del movil</param>
        /// <returns>Un string HTML</returns>
        private string GetMovilPopupContent(LogUltimaPosicionVo upm)
        {
            var empresa = IdEmpresa;

            var linea = IdLinea.Count == 1 ? IdLinea.FirstOrDefault() : -1;

            return "javascript:getMovP(" + upm.IdCoche + ",'" + empresa + "','" + linea + "')";
        }

        /// <summary>
        /// Devuelve el contenido del popup para un punto de interes
        /// </summary>
        /// <param name="dom">El punto de interes</param>
        /// <returns>Un string HTML</returns>
        private string GetPoiPopupContent(ReferenciaGeografica dom)
        {
            var lineas = SelectedLineas.Aggregate("", (current, linea) => current + (linea + ","));

            lineas = lineas.TrimEnd(',');

            return "javascript:getPOIP('" + dom.Id + "','" + lineas + "','" + SelectedEmpresa + "')";
        }
        private string GetClientPopupContent(Cliente cliente)
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
                 cliente.Comentario3
                 );
        }
        private string GetPuntoEntregaPopupContent(PuntoEntrega punto)
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
        #endregion

        #region UI Logs & Alerts
        /// <summary>
        /// Muestra un alert
        /// </summary>
        /// <param name="msg">El mensaje</param>
        public void JsAlert(string msg)
        {
            Monitor.ExecuteScript("alert(\'" + msg + "\');");
        }

        /// <summary>
        /// Devulve el codigo javascript para mostrar un mensaje en la barra de estado
        /// </summary>
        /// <param name="msg">El mensaje a mostrar</param>
        /// <returns>Un string Javascript</returns>
        protected string GetShowMessage(string msg)
        {
            return string.Format("$get('{0}').innerHTML = '{1}';", lblInfo.ClientID, msg.Replace('\n', ' '));
        }
        #endregion

        #region Status Check
        protected void RegisterStatusCheck()
        {
            var script = @"                
        function check_status(){
            var secs = Math.floor(new Date().getTime() - lastUpdate.getTime())/1000;            
            if (onPause) {
                    $get('" + connection_status.ClientID + @"').src = 'paused.gif';
                    $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_PAUSED") + @"';                     
            } else
            if (onCall) {
                console.log('check_status: onCall -> %s', secs);
                if (secs <= " + (Monitor.TimerInterval * 2) + @"){                    
                    $get('" + connection_status.ClientID + @"').src = 'connectedw.gif';
                    $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_WAITING") + @"'; 
                } else { 
                    $get('" + connection_status.ClientID + @"').src = 'waitingconn2.gif';
                    $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_DISCONNECTED") + @"';
                    if(secs > " + (Monitor.TimerInterval * 3) + @") {
                        lastUpdate = new Date(); " + ClientScript.GetPostBackEventReference(connection_status, "") + @";
                        return false;
                    }
                }
            } else {                
                console.log('check_status: !onCall -> %s', secs);
                if (secs > " + (Monitor.TimerInterval * 1.5) + @"){
                    $get('" + connection_status.ClientID + @"').src = 'disconnected.png';
                    $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_DISCONNECTED") + @"';
                    if(secs > " + (Monitor.TimerInterval * 2) + @") {
                        lastUpdate = new Date(); " + ClientScript.GetPostBackEventReference(connection_status, "") + @";
                        return false;
                    }
                } else {
                    $get('" + connection_status.ClientID + @"').src = 'connected.png';
                    $get('" + connection_status.ClientID + @"').title = '" + CultureManager.GetLabel("ONLINE_CONNECTED") + @"'; 
                }
            }
            return true;
        }; setInterval(check_status, 2500);";
            ClientScript.RegisterStartupScript(typeof(string), "check_status", script, true);
            connection_status.OnClientClick = "if (!check_status()) return true;";
        }
        #endregion

        #region Popup Events
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
                function AddEvent(txt, reqAtencion)
                {{ 
                    var cont = $get('{1}');
                    if(cont.childNodes.length > 19 && !reqAtencion)
                    {{
                        var sinatencion = 0;
                        for (i = 0; i < cont.childNodes.length; i++)
                        {{
                            if (cont.childNodes[i].getAttribute('requiereatencion') != 'True')
                                sinatencion++;

                            if (sinatencion > 19)
                            {{
                                for (j = cont.childNodes.length-1; j >= 0; j--)
                                {{
                                    if (cont.childNodes[j].getAttribute('requiereatencion') != 'True')
                                    {{
                                        cont.removeChild(cont.childNodes[j]);
                                        break;
                                    }}
                                }}
                                break;
                            }}
                        }}
                    }}                        
                            
                    cont.innerHTML = txt + cont.innerHTML; 
                }}
                function ShowDetail(){{  $get('{1}').style.display = 'none'; $get('{2}').style.display = '';PopupPanelOpen = true; }}
                function HideDetail(){{ $get('{1}').style.display = ''; $get('{2}').style.display = 'none'; PopupPanelOpen = false;}}
                function LoadDetail(id){{ $get('ifrPopupDetail').src = 'InfoEvent.aspx?evt='+id; ShowDetail(); }}
                ",
                    panelPopup.ClientID,
                    panelPopupEvents.ClientID,
                    panelPopupDetail.ClientID
                    );
            ClientScript.RegisterStartupScript(typeof(string), "popup_events", script, true);

            cbVehiculo.Attributes.Add("ondblclick", ClientScript.GetPostBackEventReference(btCenter, ""));
        }

        protected void BtCenterClick(object sender, EventArgs e)
        {
            if (SelectedVehiculos.Count != 1) return;
            var nuevas = SharedPositions.GetLastPositions(SelectedVehiculos);

            foreach (var upm in nuevas)
            {
                AddMarker(upm);
                Monitor.SetCenter(upm.Latitud, upm.Longitud);
            }
        }

        protected static string GetMessage(Logictracker.SharedPopup popup)
        {
            var posTime = popup.DateTime.ToDisplayDateTime().ToString("HH:mm");
            var posTimeAlta = popup.DateTimeAlta.ToDisplayDateTime().ToString("HH:mm");

            var html = new StringBuilder();
            html.Append(String.Format("<div requiereatencion=\"{3}\" style=\"cursor: pointer; background-color: {0}; border: solid 1px White;color: White; padding: 3px;\" onclick=\"LoadDetail({1}); this.parentNode.removeChild(this);\">{2}", popup.Color, popup.Id, posTime, popup.RequiereAtencion));
            html.Append(String.Format(" - ({0})", posTimeAlta));

            html.Append(String.Format(": <b>{0}</b> - {1}</div>", popup.Interno, popup.Text));
            return html.ToString();
        }
        #endregion

        #endregion

        #region Properties

        private List<int> IdLinea
        {
            get
            {
                if (cbPlanta.SelectedIndex <= 0) return (from ListItem item in cbPlanta.Items where item.Value != "-1" select Convert.ToInt32(item.Value)).ToList();
                return new List<int> { Convert.ToInt32(cbPlanta.SelectedValue) };
            }
        }
        private int IdEmpresa
        {
            get { if (cbLocacion.SelectedIndex < 0) return -1; return Convert.ToInt32(cbLocacion.SelectedValue); }
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
        protected IList<Coche> SelectedVehiculos
        {
            get
            {
                var result = Session["MONITOR_SelectedVehiculos"] != null ? Session["MONITOR_SelectedVehiculos"] as IList<Coche> : new List<Coche>();

                var ids = result.Select(c => c.Id);

                return ids.Any() ? DAOFactory.CocheDAO.GetByIds(ids).ToList() : new List<Coche>();
            }
            set { Session["MONITOR_SelectedVehiculos"] = value; }
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

        #region Vehiculo

        private void SetSelectedVehiculos(bool isNewSelection, IEnumerable<Coche> coches)
        {
            SelectedVehiculos = null;
            SelectVehiculos(coches);

            _clearLayerVehiculos = false;
            Monitor.ClearLayer(LayerVehiculos);
            Monitor.ClearLayer(LayerVehiculosInactivos);

            if (coches == null || !coches.Any()) return;

            Monitor.CallbackArgument = DateTime.UtcNow.ToString();

            var vehs = DAOFactory.LogPosicionDAO.GetLastVehiclesPositions(SelectedVehiculos);

            if (vehs != null && vehs.Count > 0)
            {
                var upms = vehs.Where(z=> z.Value != null).Select(z => z.Value).ToArray();
                AddMarkers(coches, upms);

                if (isNewSelection)
                {
                    Monitor.SetZoomOn(LayerVehiculos);
                }
                else if (vehs.Count == 1)
                {
                    var veh = vehs.FirstOrDefault().Value;
                    if (veh != null)
                        Monitor.SetCenter(veh.Latitud, veh.Longitud);
                }
            }
        }

        protected void SelectVehiculos(IEnumerable<Coche> cocheList)
        {
            SelectedVehiculos = SelectedVehiculos.Union(cocheList).ToList();

            foreach (Coche c in cocheList)
            {
                var li = cbVehiculo.Items.FindByValue(c.Id.ToString("#0"));
                if (li != null &&
                    !li.Selected) li.Selected = true;
            }
        }

        protected void SelectVehiculo(Coche coche)
        {
            var sv = SelectedVehiculos;
            if (!sv.Contains(coche)) sv.Add(coche);
            SelectedVehiculos = sv;
            var li = cbVehiculo.Items.FindByValue(coche.Id.ToString("#0"));
            if (li != null && !li.Selected) li.Selected = true;
        }


        protected void SelectVehiculo(int id)
        {
            var coche = DAOFactory.CocheDAO.FindById(id);
            SelectVehiculo(coche);
        }

        protected void UnselectVehiculo(int id)
        {
            var sv = SelectedVehiculos;

            sv.Remove(SelectedVehiculos.Where(c => c.Id == id).Select(c => c).FirstOrDefault());

            SelectedVehiculos = sv;

            var li = cbVehiculo.Items.FindByValue(id.ToString("#0"));
            if (li != null && li.Selected) li.Selected = false;
        }

        #endregion

        #region Punto de Interes

        private void SelectPoi(ReferenciaGeografica dom)
        {
            var li = cbPoi.Items.FindByValue(dom.TipoReferenciaGeografica.Id.ToString("#0"));            
            if (li != null)
            {
                li.Selected = true;
                CbPoiSelectedIndexChanged(cbPoi, EventArgs.Empty);
                updPoi.Update();
            }

            var lat = dom.Direccion != null ? dom.Direccion.Latitud : dom.Poligono.Centro.Y;
            var lon = dom.Direccion != null ? dom.Direccion.Longitud : dom.Poligono.Centro.X;
            Monitor.SetCenter(lat, lon, SearchZoomLevel);
        }

        #endregion

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
            SelectedMessages = cbMensajes.Items.OfType<ListItem>().Where(li=>li.Selected).Select(li => li.Value).ToList();
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

        #region Consultas

        protected void Consultas1DireccionAdded(object sender, DireccionEventArgs e)
        {   
            DisplayDirection(e.Direccion, e.Index);
            if (e.Posicionar)
            {
                cbPlanta.SetSelectedValue(cbPlanta.AllValue);
                cbCentroDeCostos.SetSelectedValue(cbCentroDeCostos.AllValue);
                cbSubCentroDeCostos.SetSelectedValue(cbSubCentroDeCostos.AllValue);
                cbDepartamento.SetSelectedValue(cbDepartamento.AllValue);
                cbTransportista.SetSelectedValue(cbTransportista.AllValue);
                cbTipoVehiculo.SetSelectedValue(cbTipoVehiculo.AllValue);
                cbVehiculo.ClearSelection();
                
                var vehicles = DAOFactory.CocheDAO.GetList(new[] {cbLocacion.Selected}, new[] {cbPlanta.Selected});
                var pois = ReportFactory.MobilePoiDAO.GetMobilesNearPoint(vehicles, e.Direccion.Latitud, e.Direccion.Longitud, 1000);
                if (pois.Count > 10) pois.RemoveRange(10, pois.Count - 10);

                var ids = pois.Select(p => p.IdVehiculo);
                var coches = vehicles.Where(v => ids.Contains(v.Id)).ToList();
                SetSelectedVehiculos(false, coches);
            }
            CenterDirection(e.Direccion);
        }
        protected void Consultas1DireccionRemoved(object sender, DireccionEventArgs e)
        {
            RemoveDirection(e.Index);
        }
        protected void Consultas1DireccionSelected(object sender, DireccionEventArgs e)
        {
            CenterDirection(e.Direccion);
        }
        protected void Consultas1Clear(object sender, EventArgs e)
        {
            Monitor.ClearLayer(LayerDir);
        }
        protected void Consultas1DireccionSaved(object sender, DireccionSavedEventArgs e)
        {
            SaveDirection(e.Direccion, e.IdTipoReferenciaGeografica, e.Codigo);
        }
        private void DisplayDirection(DireccionVO direction, int id)
        {
            var lineas = DAOFactory.LineaDAO.FindList(new[] { SelectedEmpresa }).Aggregate("", (current, linea) => current + (linea.Id + " "));
            //var lineas = SelectedLineas.Aggregate("", (current, linea) => current + (linea + " "));

            lineas = lineas.TrimEnd(' ');

            var popupHtml = string.Format("javascript:getDirP('{0}','{1}',{2},{3})", lineas, direction.Direccion,
                                          direction.Latitud.ToString(CultureInfo.InvariantCulture), direction.Longitud.ToString(CultureInfo.InvariantCulture));

            Monitor.AddMarkers(LayerDir, MarkerFactory.CreateMarker("DIR_" + id, "salida.gif", direction.Latitud, direction.Longitud, popupHtml));
        }
        private void CenterDirection(DireccionVO direction)
        {
            Monitor.SetCenter(direction.Latitud, direction.Longitud, SearchZoomLevel);
        }
        private void RemoveDirection(int id)
        {
            Monitor.ClearLayer(LayerDir);
            for (var i = 0; i < Consultas1.Selected.Count; i++)
            {
                if (Consultas1.SelectedId[i] == id) continue;
                DisplayDirection(Consultas1.Selected[i], i);
            }
        }
        
        private void SaveDirection(DireccionVO direction, int idTipoReferenciaGrografica, string codigo)
        {
            var linea = DAOFactory.LineaDAO.FindById(cbPlanta.Selected);
            var directionType = DAOFactory.TipoReferenciaGeograficaDAO.FindById(idTipoReferenciaGrografica);

            var georef = DAOFactory.ReferenciaGeograficaDAO.FindByCodigo(new []{ linea.Empresa.Id }, new [] { linea.Id },new[]{-1}, codigo);
            if (georef != null)
            {
                JsAlert("Ya existe una Referencia Geografica con ese codigo");
                return;
            }
            var inicio = DateTime.UtcNow;
            var fin = directionType.Vigencia != null ? directionType.Vigencia.Fin.HasValue ? directionType.Vigencia.Fin : null : null;

            //Modificar para tomar el pais, provincia y localidad del Geocoder.
            var poi = new ReferenciaGeografica
            {
                Baja = false,
                Descripcion = direction.Direccion,
                Codigo = codigo,
                Vigencia = new Vigencia { Inicio = inicio, Fin = fin },
                Icono = directionType.Icono,
                InhibeAlarma = directionType.InhibeAlarma,
                EsInicio = directionType.EsInicio,
                EsIntermedio = directionType.EsIntermedio,
                EsFin = directionType.EsFin,
                Empresa = linea.Empresa, 
                Linea = linea,
                TipoReferenciaGeografica = directionType
            };

            poi.Historia.Add(new HistoriaGeoRef
                             {
                                ReferenciaGeografica = poi,
                                Direccion = new Direccion
                                            {
                                                Pais = string.Empty,
                                                Provincia = direction.Provincia,
                                                Partido = direction.Partido,
                                                Altura = direction.Altura,
                                                IdMapa = (short)direction.IdMapaUrbano,
                                                IdCalle = direction.IdPoligonal,
                                                IdEsquina = direction.IdEsquina,
                                                Latitud = direction.Latitud,
                                                Longitud = direction.Longitud,
                                                Calle = direction.Calle,
                                                Descripcion = direction.Direccion,
                                                Vigencia = new Vigencia { Inicio = inicio, Fin = fin }
                                            },
                                Vigencia = new Vigencia { Inicio = inicio, Fin = fin }
                             });

            DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(poi);
            STrace.Trace("QtreeReset", "Default");
            JsAlert(CultureManager.GetSystemMessage("POI_CREATE_OK"));
        }
        
        #endregion
    }
}