using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.Qtree;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.ContextMenu;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using NHibernate.Util;
using Logictracker.QuadTree.Data;

namespace Logictracker.Monitor.MonitorDeCalidad
{
    public partial class MonitorCalidad : ApplicationSecuredPage
    {
        protected override InfoLabel LblInfo { get { return infoLabel1; } }
        protected override string GetRefference() { return "MONITOR_CALIDAD,PARTE_REPORT"; }

        private FullVsProperty<List<int>> PoisTypesIds { get { return this.CreateFullVsProperty("PoisTypesIds", new List<int>()); } }
        private FullVsProperty<int> Distrito { get { return this.CreateFullVsProperty("Distrito",-1); } }
        private FullVsProperty<int> Location { get { return this.CreateFullVsProperty("Location", -1); } }
        private FullVsProperty<int> Chofer { get { return this.CreateFullVsProperty<int>("Chofer"); } }
        private FullVsProperty<int> TypeMobile { get { return this.CreateFullVsProperty<int>("TypeMobile"); } }
        private FullVsProperty<int> Mobile { get { return this.CreateFullVsProperty<int>("Mobile"); } }
        private FullVsProperty<DateTime> InitialDate { get { return this.CreateFullVsProperty("InitialDate", DateTime.UtcNow.Date.ToDataBaseDateTime()); } }
        private FullVsProperty<DateTime> FinalDate { get { return this.CreateFullVsProperty("FinalDate", DateTime.UtcNow.Date.ToDataBaseDateTime().AddHours(23).AddMinutes(59)); } }
        private FullVsProperty<bool> LockFilters { get { return this.CreateFullVsProperty<bool>("LockFilters"); } }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        protected void LnkReporteDeEventosClick(object sender, EventArgs e)
        {         
            Session.Add("EventsLocation", ddlDistrito.Selected);
            Session.Add("EventsCompany", ddlPlanta.Selected);
            Session.Add("EventsMobileType", ddlTipoVehiculo.Selected);
            Session.Add("EventsMobile", ddlMovil.Selected);
            Session.Add("EventsFrom", dtDesde.SelectedDate );
            Session.Add("EventsTo", dtHasta.SelectedDate);

            OpenWin(ResolveUrl("~/Reportes/DatosOperativos/eventos.aspx"), "Reporte de Eventos");
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Request.QueryString["qs"] != null)
            {
                Response.Clear();
                Response.ContentType = "text/plain";
                var param = Parameters.Deserialize(Request.QueryString["qs"]);
                var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
                var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;

                if (Request.QueryString["op"] == "p")
                {
                    var positions = DAOFactory.RoutePositionsDAO.GetPositions(param.Vehiculo, param.Desde, param.Hasta, maxMonths);
                    var posicionesDescartadas = DAOFactory.RoutePositionsDAO.GetPositionsDescartadas(param.Vehiculo, param.Desde, param.Hasta, maxMonths);
                    var posParam = string.Join(",", positions.Select(x => SerializePosition(x)).ToArray());
                    var posDesParam = string.Join(",", posicionesDescartadas.Select(x => SerializePosition(x)).ToArray());
                    Response.Write("{ \"positions\": [" + posParam + "], \"descartadas\": [" + posDesParam + "] }");
                }
                else if (Request.QueryString["op"] == "q")
                {
                    var qualityMessages = DAOFactory.LogMensajeDAO.GetQualityMessagesByMobilesAndTypes(param.Vehiculo, param.Desde, param.Hasta, maxMonths);
                    var infractions = DAOFactory.InfraccionDAO.GetByVehiculo(param.Vehiculo, param.Desde, param.Hasta);
                    var qualityMessagesDesc = DAOFactory.LogMensajeDescartadoDAO.GetQualityMessagesByMobilesAndTypes(param.Vehiculo, param.Desde, param.Hasta);
                    var qualityMessParam = string.Join(",", qualityMessages.Select(x => SerializeMessage(x)).ToArray());
                    var infractionsParam = string.Join(",", infractions.Select(x => SerializeInfraccion(x)).ToArray());
                    var qualityMessDescParam = string.Join(",", qualityMessagesDesc.Select(x => SerializeMessage(x)).ToArray());
                    Response.Write("{ \"quality\": [" + qualityMessParam + "], \"infractions\": [" + infractionsParam + "],  \"quality_descartados\": [" + qualityMessDescParam + "] }");
                }
                else if(Request.QueryString["op"] == "g")
                {
                    if (param.TiposPoi.Length > 0 && !param.TiposPoi.Contains(0) && !param.TiposPoi.Contains(-1))
                    {
                        var pois = DAOFactory.ReferenciaGeograficaDAO.GetList(new[] { param.Empresa }, new[] { param.Linea }, param.TiposPoi);
                        var poisParam = string.Join(",", pois.Select(x => SerializeReferenciaGeografica(x)).ToArray());
                        Response.Write("{ \"geocercas\": [" + poisParam + "] }");
                    }
                    else
                    {
                        Response.Write("{ \"geocercas\": [] }");
                    }
                }
                else if (Request.QueryString["op"] == "t")
                {
                    Response.ContentType = "application/json";
                    if (WebSecurity.IsSecuredAllowed(Securables.ViewQtree))
                    {
                        var positions = DAOFactory.RoutePositionsDAO.GetPositions(param.Vehiculo, param.Desde, param.Hasta, maxMonths);
                        double hres, vres;
                        var leafs = GetQtree(param.Vehiculo, positions, out hres, out vres);
                        var leafsParam = leafs != null 
                            ? string.Join(",", leafs.Select(x => SerializeQtree(x, hres, vres)).ToArray())
                            : string.Empty;
                        Response.Write("{ \"qtree\": [" + leafsParam + "] }");
                    }
                    else
                    {
                        Response.Write("{ \"qtree\": [] }");
                    }
                }
                Response.End();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Monitor.ContextMenuPostback += Monitor_ContextMenuPostback;
            
            LoadQtreeInfo();

            base.OnLoad(e);

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected);
            var maxHours = empresa != null && empresa.Id > 0 ? empresa.MaxHorasMonitor : 24;
            dtvalidator.MaxRange = new TimeSpan(maxHours, 0, 0);

            if (!IsPostBack)
            {
                this.RegisterCss(ResolveUrl("~/App_Styles/openlayers.css"));
                RegisterExtJsStyleSheet();

                dtDesde.SelectedDate = InitialDate.Get().ToDisplayDateTime();
                dtHasta.SelectedDate = FinalDate.Get().ToDisplayDateTime();
                WestPanel.Enabled = !LockFilters.Get();
                dtDesde.Enabled = !LockFilters.Get();
                dtHasta.Enabled = !LockFilters.Get();
                if (Distrito.Get() > 0) ddlDistrito.SetSelectedValue(Distrito.Get());
                if (Location.Get() > 0) ddlPlanta.SetSelectedValue(Location.Get());
                if (Chofer.Get() > 0) ddlEmpleado.SetSelectedValue(Chofer.Get());
                if (TypeMobile.Get() > 0) ddlTipoVehiculo.SetSelectedValue(TypeMobile.Get());
                if (Mobile.Get() > 0) ddlMovil.SetSelectedValue(Mobile.Get());

                foreach (var id in PoisTypesIds.Get())
                {
                    var it = lbPuntosDeInteres.Items.FindByValue(id.ToString());
                    if (it != null) it.Selected = true;
                }
                PoisTypesIds.Set(lbPuntosDeInteres.SelectedValues);

                InitializeMap();
                if (Mobile.Get() > 0) LoadPositions(true);
            }
        }

        #region Serialization
        private string SerializePosition(RoutePosition x)
        {
            return string.Format("{{ \"lat\": {0},\"lon\": {1}, \"date\": {2}, \"speed\": {3}, \"course\": {4}, \"id\": {5}, \"historical\": {6}, \"received\": {7}, \"motivo\": {8} }}",
                        x.Latitude.ToString(CultureInfo.InvariantCulture),
                        x.Longitude.ToString(CultureInfo.InvariantCulture),
                        SerializeDate(x.Date.ToDisplayDateTime()),
                        x.Speed,
                        x.Course.ToString(CultureInfo.InvariantCulture),
                        x.Id,
                        x.Historical ? "true" : "false",
                        SerializeDate(x.Recieved.ToDisplayDateTime()),
                        x.MotivoDescarte
                       );
        }
        private string SerializeMessage(LogMensajeBase x)
        {
            return string.Format("{{ \"lat\": {0},\"lon\": {1}, \"date\": {2}, \"enddate\": {3}, \"icon\": \"{4}\", \"id\": {5}, \"endlat\": {6}, \"endlon\": {7} }}",
                        x.Latitud.ToString(CultureInfo.InvariantCulture),
                        x.Longitud.ToString(CultureInfo.InvariantCulture),
                        SerializeDate(x.Fecha.ToDisplayDateTime()),
                        x.FechaFin.HasValue ? SerializeDate(x.FechaFin.Value.ToDisplayDateTime()).ToString() : "null",
                        string.IsNullOrEmpty(x.GetIconUrl()) ? string.Concat(ApplicationPath, "OpenLayers/img/marker-gold.png") : string.Concat(IconDir, x.GetIconUrl()),
                        x.Id,
                        x.LatitudFin.HasValue ? x.LatitudFin.Value.ToString(CultureInfo.InvariantCulture) : "null",
                        x.LongitudFin.HasValue ? x.LongitudFin.Value.ToString(CultureInfo.InvariantCulture) : "null"
                );
        }
        private string SerializeInfraccion(Infraccion x)
        {
            return string.Format("{{ \"lat\": {0},\"lon\": {1}, \"date\": {2}, \"enddate\": {3}, \"id\": {4}, \"endlat\": {5}, \"endlon\": {6} }}",
                        x.Latitud.ToString(CultureInfo.InvariantCulture),
                        x.Longitud.ToString(CultureInfo.InvariantCulture),
                        SerializeDate(x.Fecha.ToDisplayDateTime()),
                        x.FechaFin.HasValue ? SerializeDate(x.FechaFin.Value.ToDisplayDateTime()).ToString() : "null",
                        x.Id,
                        x.LatitudFin.ToString(CultureInfo.InvariantCulture),
                        x.LongitudFin.ToString(CultureInfo.InvariantCulture)
                );
        }
        private string SerializeReferenciaGeografica(ReferenciaGeografica x)
        {
            var icono = x.Icono != null ? string.Concat(IconDir, x.Icono.PathIcono)
                        : x.TipoReferenciaGeografica.Icono != null
                        ? string.Concat(IconDir, x.TipoReferenciaGeografica.Icono.PathIcono)
                        : string.Concat(ImagesDir, "pois.png");

            return string.Format("{{ \"id\": {0}, \"lat\": {1},\"lon\": {2}, \"icon\": \"{3}\", \"color\": \"#{4}\", \"radio\": {5}, \"points\": [{6}], \"name\": \"{7}\" }}",
                        x.Id,
                        x.Direccion != null ? x.Direccion.Latitud.ToString(CultureInfo.InvariantCulture) : "null",
                        x.Direccion != null ? x.Direccion.Longitud.ToString(CultureInfo.InvariantCulture) : "null",
                        icono,
                        (x.Color != null ? x.Color.HexValue : "0000FF"),
                        x.Poligono != null ? x.Poligono.Radio : 0,
                        x.Poligono != null
                            ? string.Join(",", x.Poligono.ToPointFList().Select(y => string.Format("{{ \"lat\": {0}, \"lon\": {1} }}", y.Y.ToString(CultureInfo.InvariantCulture), y.X.ToString(CultureInfo.InvariantCulture))).ToArray())
                            : string.Empty,
                        x.Descripcion.Replace("\"", "\\\"")
                );
        }
        private string SerializeQtree(QLeaf x, double hres, double vres)
        {

         return 
             string.Format("{{'id':'{0}','lon':{1},'lat':{2},'hres':{3},'vres':{4},'color':'{5}'}}",
                x.Index.Y.ToString()+"-"+x.Index.X.ToString(),
                x.Posicion.Longitud.ToString(CultureInfo.InvariantCulture),
                x.Posicion.Latitud.ToString(CultureInfo.InvariantCulture),
                hres.ToString(CultureInfo.InvariantCulture),
                vres.ToString(CultureInfo.InvariantCulture),
                System.Web.HttpUtility.JavaScriptStringEncode(HexColorUtil.ColorToHex(BaseQtree.GetColorForLevel(x.Valor)))
                ).Replace("'","\"");
            //return string.Format("{{ \"id\": '{0}{1}', \"lon\": {2}, \"lat\": {3}, \"hres\": {4}, \"vres\": {5}, \"color\": \"{6}\" }}",
            //    x.Index.Y,
            //    x.Index.X,
            //    x.Posicion.Longitud.ToString(CultureInfo.InvariantCulture),
            //    x.Posicion.Latitud.ToString(CultureInfo.InvariantCulture),
            //    hres.ToString(CultureInfo.InvariantCulture),
            //    vres.ToString(CultureInfo.InvariantCulture),
            //    HexColorUtil.ColorToHex(BaseQtree.GetColorForLevel(x.Valor))
            //    );
        }
        private long SerializeDate(DateTime date)
        {
            return (long)new TimeSpan(date.Ticks - Epoch.ToDisplayDateTime().Ticks).TotalMilliseconds;
        } 
        #endregion

        private void Monitor_ContextMenuPostback(object sender, PostbackEventArgs e)
        {
            if (Mobile.Get() <= 0) return;
            var vehicle = DAOFactory.CocheDAO.FindById(Mobile.Get());
            if (vehicle.Dispositivo == null) return;

            var qtreeDir = DAOFactory.DetalleDispositivoDAO.GetQtreeFileNameValue(vehicle.Dispositivo.Id);
            var qtreeType = DAOFactory.DetalleDispositivoDAO.GetQtreeTypeValue(vehicle.Dispositivo.Id);

            if (string.IsNullOrEmpty(qtreeType) || !Enum.IsDefined(typeof(QtreeFormats), qtreeType))
                return;

            var qtreeFormat = (QtreeFormats)Enum.Parse(typeof(QtreeFormats), qtreeType);
            qtreeDir = Path.Combine(qtreeFormat.Equals(QtreeFormats.Gte)
                                        ? Config.Qtree.QtreeGteDirectory
                                        : Config.Qtree.QtreeTorinoDirectory, qtreeDir);

            if (String.IsNullOrEmpty(qtreeDir) || !Directory.Exists(qtreeDir)) return;

            var leafs = new List<QLeaf>();
            double hres = 0, vres = 0;

            using (var qtree = BaseQtree.Open(qtreeDir, qtreeFormat))
            {
                var index = qtree.GetIndex(e.Latitud, e.Longitud);
                hres = qtree.HorizontalResolution;
                vres = qtree.VerticalResolution;
                for (var i = index.Y - 10; i < index.Y + 10; i++)
                    for (var j = index.X - 10; j < index.X + 10; j++)
                    {
                        var latlon = qtree.GetCenterLatLon(new QIndex { Y = i, X = j });
                        var leaf = qtree.GetQLeaf(latlon.Latitud, latlon.Longitud);

                        if (leaf == null) continue;


                        leafs.Add(leaf);
                    }
            }
            var leafsParam = leafs.Count > 0
                            ? string.Join(",", leafs.Select(x => SerializeQtree(x, hres, vres)).ToArray())
                            : string.Empty;
            Monitor.ExecuteScript("processGeocercas( { \"qtree\": [" + leafsParam + "] });");
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Distrito.Set(ddlDistrito.Selected);
            Location.Set(ddlPlanta.Selected);
            Chofer.Set(ddlEmpleado.Selected);
            Mobile.Set(ddlMovil.Selected);
            InitialDate.Set(SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.Value));
            FinalDate.Set(SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value));
            PoisTypesIds.Set(lbPuntosDeInteres.SelectedValues);

            var deltaTime = FinalDate.Get().Subtract(InitialDate.Get());
            if (deltaTime > dtvalidator.MaxRange)
            {
                ShowError("El rango de tiempo debe ser menor o igual a " + dtvalidator.MaxRange.ToString());
                return;
            }
            LoadPositions(true);
        }

        private void LoadPositions(bool center)
        {
            var par = new Parameters
            {
                Empresa = Distrito.Get(),
                Linea = Location.Get(),
                Chofer = Chofer.Get(),
                Vehiculo = Mobile.Get(),
                Desde = InitialDate.Get(),
                Hasta = FinalDate.Get(),
                TiposPoi = PoisTypesIds.Get().ToArray()
            };

            Monitor.ExecuteScript(string.Format("CallForData('{0}', {1});", par.Serialize(), center ? "true" : "false"));
        }
      

        #region Eliminar Posiciones / Eventos
        protected void botonEliminar_Click(object sender, EventArgs e)
        {
            var historica = hiddenEliminar.Value.Split(':')[0] == "H";
            var id = Convert.ToInt32(hiddenEliminar.Value.Split(':')[1]);

            if (historica)
            {
                //var posicion = DAOFactory.LogPosicionHistoricaDAO.FindById(id);
                //if (posicion == null) return;
                //var posicionDescartada = new LogPosicionDescartada(posicion, DiscardReason.Manual);
                //DAOFactory.LogPosicionDescartadaDAO.SaveOrUpdate(posicionDescartada);
                //DAOFactory.LogPosicionHistoricaDAO.Delete(posicion);
            }
            else
            {
                var posicion = DAOFactory.LogPosicionDAO.FindById(id);
                if (posicion == null) return;
                var posicionDescartada = new LogPosicionDescartada(posicion, DiscardReason.Manual);
                DAOFactory.LogPosicionDescartadaDAO.SaveOrUpdate(posicionDescartada);
                DAOFactory.LogPosicionDAO.Delete(posicion);
            }

            LoadPositions(false);
        }

        protected void botonEliminarEvento_Click(object sender, EventArgs e)
        {
            var id = Convert.ToInt32((string) hiddenEliminarEvento.Value.Split(':')[0]);
            var evento = DAOFactory.LogMensajeDAO.FindById(id);
            if (evento == null) return;
            var eventoDescartado = new LogMensajeDescartado(evento, DiscardReason.Manual);

            DAOFactory.LogMensajeDescartadoDAO.SaveOrUpdate(eventoDescartado);
            DAOFactory.LogMensajeDAO.Delete(evento);

            Monitor.ExecuteScript("CallForQualityMessages();");
        } 
        #endregion

        #region Mapa
        private void InitializeMap()
        {
            var googleMapsEnabled = true;
            var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
            if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
            {
                var empresa = usuario.Empresas.First() as Empresa;
                if (empresa != null)
                    googleMapsEnabled = empresa.GoogleMapsEnabled;
            }

            Monitor.Initialize(googleMapsEnabled);
            Monitor.ImgPath = Config.Monitor.GetMonitorImagesFolder(this);
            Monitor.EnableTimer = false;
            Monitor.MultiplePopUps = true;
            Monitor.PostbackOnMoveZoom = 99;

            var showQtree = WebSecurity.IsSecuredAllowed(Securables.ViewQtree);
            if (showQtree) Monitor.AddLayers(LayerFactory.GetVector("Qtree", true));

            Monitor.AddLayers(
                LayerFactory.GetVector(CultureManager.GetLabel("RECORRIDO"), true),
                LayerFactory.GetVector(CultureManager.GetLabel("REC_DESCARTADO"), false),
                LayerFactory.GetVector(CultureManager.GetLabel("EXCESOS_VELOCIDAD"), true),
                LayerFactory.GetVector(CultureManager.GetLabel("EXCESOS_VELOCIDAD_DESCARTADOS"), false),
                LayerFactory.GetVector(CultureManager.GetLabel("LAYER_GEOCERCAS"), true),
                LayerFactory.GetMarkers(CultureManager.GetLabel("LAYER_POI"), true),
                LayerFactory.GetMarkers(CultureManager.GetLabel("POSICIONES_REPORTADAS"), true),
                LayerFactory.GetMarkers(CultureManager.GetLabel("POS_DESCARTADAS"), false),
                LayerFactory.GetMarkers(CultureManager.GetLabel("EVENTOS"), true),
                LayerFactory.GetMarkers(CultureManager.GetLabel("EVENTOS_DESCARTADOS"), false)
                );

            Monitor.AddControls(ControlFactory.GetToolbar(false, false, false, false, false, true, true));

            if (showQtree)
            {
                var ctx = ControlFactory.GetContextMenu();
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.None, "Opciones", "", "olContextMenuTitle", true));
                ctx.AddItem(ContextMenuItem.Separator);
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.Postback, "Mostrar Qtree", "Qtree", "olContextMenuItem", true));
                Monitor.AddControls(ctx);
            }

            Monitor.ZoomTo(8);
            GenerateScriptBase();
        }

        private List<QLeaf> GetQtree(int vehiculo, List<RoutePosition> positions, out double hres, out double vres)
        {
            hres = 0;
            vres = 0;
            if (positions.Count <= 1) return null;

            var vehicle = DAOFactory.CocheDAO.FindById(vehiculo);
            if (vehicle.Dispositivo == null) return null;

            var qtreeDir = DAOFactory.DetalleDispositivoDAO.GetQtreeFileNameValue(vehicle.Dispositivo.Id);
            var qtreeType = DAOFactory.DetalleDispositivoDAO.GetQtreeTypeValue(vehicle.Dispositivo.Id);

            if (string.IsNullOrEmpty(qtreeType) || !Enum.IsDefined(typeof(QtreeFormats), qtreeType))
                return null;

            var qtreeFormat = (QtreeFormats)Enum.Parse(typeof(QtreeFormats), qtreeType);
            qtreeDir = Path.Combine(qtreeFormat.Equals(QtreeFormats.Gte)
                                        ? Config.Qtree.QtreeGteDirectory
                                        : Config.Qtree.QtreeTorinoDirectory, qtreeDir);

            if (String.IsNullOrEmpty(qtreeDir)) return null;

            if (!Directory.Exists(qtreeDir)) return null;

            var toAdd = new List<QLeaf>();
            var added = new Dictionary<string, bool>();

            using (var qtree = BaseQtree.Open(qtreeDir, qtreeFormat))
            {
                hres = qtree.HorizontalResolution;
                vres = qtree.VerticalResolution;
                for (var i = 1; i < positions.Count; i++)
                {
                    var ini = positions[i - 1];
                    var fin = positions[i];
                    var qs = qtree.MakeLeafLine(ini.Longitude, ini.Latitude, fin.Longitude, fin.Latitude, 2);

                    foreach (var leaf in qs)
                    {
                        var key = leaf.Index.X + "," + leaf.Index.Y;

                        if (added.ContainsKey(key)) continue;

                        toAdd.Add(leaf);
                        added.Add(key, true);
                    }
                }
            }
            return toAdd;
        }

        private void LoadQtreeInfo()
        {
            chkQtree.Visible = WebSecurity.IsSecuredAllowed(Securables.ViewQtree);
            tblVersion.Visible = chkQtree.Checked;
            tblEditar.Visible = chkQtree.Checked && WebSecurity.IsSecuredAllowed(Securables.EditQtree);

            if (chkQtree.Checked)
            {
                var vehicle = DAOFactory.CocheDAO.FindById(ddlMovil.Selected);
                if (vehicle.Dispositivo == null) return;
                var qtreeDir = DAOFactory.DetalleDispositivoDAO.GetQtreeFileNameValue(vehicle.Dispositivo.Id);
                var qtreeVersion = DAOFactory.DetalleDispositivoDAO.GetQtreeRevisionNumberValue(vehicle.Dispositivo.Id);
                lblArchivo.Text = qtreeDir;
                lblVersionEquipo.Text = qtreeVersion;

                var gg = new GeoGrillas { Repository = new Repository { BaseFolderPath = Path.Combine(Config.Qtree.QtreeGteDirectory, qtreeDir) } };
                var revision = 0;
                var base_revision = 0;
                int.TryParse(qtreeVersion, out base_revision);
                var changedSectorsList = new TransactionLog(gg.Repository, vehicle.Dispositivo.Id).GetChangedSectorsAndRevision(base_revision, out revision);

                lblVersionServer.Text = revision.ToString();
                pnlQtree.Update();
            }
        }

        protected void btnGenerarOnClick(object sender, EventArgs e)
        {
            var vehicle = DAOFactory.CocheDAO.FindById(ddlMovil.Selected);
            if (vehicle.Dispositivo == null) return;

            var qtreeDir = DAOFactory.DetalleDispositivoDAO.GetQtreeFileNameValue(vehicle.Dispositivo.Id);
            var qtreeType = DAOFactory.DetalleDispositivoDAO.GetQtreeTypeValue(vehicle.Dispositivo.Id);

            if (string.IsNullOrEmpty(qtreeType) || !Enum.IsDefined(typeof(QtreeFormats), qtreeType))
                return;

            var qtreeFormat = (QtreeFormats)Enum.Parse(typeof(QtreeFormats), qtreeType);
            qtreeDir = Path.Combine(qtreeFormat.Equals(QtreeFormats.Gte)
                                        ? Config.Qtree.QtreeGteDirectory
                                        : Config.Qtree.QtreeTorinoDirectory, qtreeDir);

            var maxMonths = vehicle.Empresa.MesesConsultaPosiciones;
            var positions = DAOFactory.RoutePositionsDAO.GetPositions(vehicle.Id, dtDesde.SelectedDate.Value.ToDataBaseDateTime(), dtHasta.SelectedDate.Value.ToDataBaseDateTime(), maxMonths);

            using (var qtree = BaseQtree.Open(qtreeDir, qtreeFormat))
            {
                for (var i = 1; i < positions.Count; i++)
                {
                    var ini = positions[i - 1];
                    var fin = positions[i];

                    var qs = qtree.MakeLeafLine(ini.Longitude, ini.Latitude, fin.Longitude, fin.Latitude, 2);
                    foreach (var item in qs)
                    {
                        var latlon = qtree.GetCenterLatLon(item.Posicion);
                        qtree.SetValue(latlon.Latitud, latlon.Longitud, lvlSel.SelectedLevel);
                        qtree.Commit();
                    }
                }
                qtree.Close();
            }
            
            btnSearch_Click(sender, e);
        }

        protected void GenerateScriptBase()
        {
            string script = string.Format(@"if(!Logictracker) Logictracker = {{}};
if(!Logictracker.Monitor) Logictracker.Monitor = {{}};
if(!Logictracker.Monitor.Layers) Logictracker.Monitor.Layers = {{
GoogleHybrid: '{0}',
GoogleSatellite: '{1}',
GoogleStreet: '{2}',
GooglePhysical: '{3}',
Compumap: '{4}',
Recorrido: '{5}',
PosicionesReportadas: '{6}',
Eventos: '{7}',
EventosDescartados: '{8}',
EventosDuracion: '{9}',
EventosDuracionDescartados: '{10}',
PuntosDeInteres: '{11}',
Geocercas: '{12}',
PosicionesDescartadas: '{13}',
RecorridoDescartado: '{14}',
Qtree: '{15}'
}};
if(!Logictracker.Monitor.Mapa) Logictracker.Monitor.Mapa = {16};
if(!Logictracker.Monitor.Images) Logictracker.Monitor.Images = {{}};
Logictracker.Monitor.Images.Posicion = '{17}';
Logictracker.Monitor.Images.Salida = '{18}';
Logictracker.Monitor.Images.Llegada = '{19}';
Logictracker.Monitor.Progress = '{20}';
Logictracker.Monitor.Info = '{21}';
Logictracker.Monitor.QtreeCheck = '{22}';

",
            CultureManager.GetLabel("LAYER_GHIBRIDO"),
            CultureManager.GetLabel("LAYER_GSAT"),
            CultureManager.GetLabel("LAYER_GSTREET"),
            CultureManager.GetLabel("LAYER_GFISICO"),
            CultureManager.GetLabel("LAYER_COMPUMAP"),
            CultureManager.GetLabel("RECORRIDO"),
            CultureManager.GetLabel("POSICIONES_REPORTADAS"),
            CultureManager.GetLabel("EVENTOS"),
            CultureManager.GetLabel("EVENTOS_DESCARTADOS"),
            CultureManager.GetLabel("EXCESOS_VELOCIDAD"),
            CultureManager.GetLabel("EXCESOS_VELOCIDAD_DESCARTADOS"),
            CultureManager.GetLabel("LAYER_POI"),
            CultureManager.GetLabel("LAYER_GEOCERCAS"),
            CultureManager.GetLabel("POS_DESCARTADAS"),
            CultureManager.GetLabel("REC_DESCARTADO"),
            "Qtree",
            Monitor.Map,
            string.Concat(ImagesDir, "point.png"),
            string.Concat(ImagesDir, "salida.png"),
            string.Concat(ImagesDir, "llegada.png"),
            ProgressLabel1.ClientID,
            infoLabel1.ClientID,
            chkQtree.ClientID
            );
            Monitor.ExecuteScript(script);
        } 
        #endregion

        #region Class Parameters
        public class Parameters
        {
            public int Empresa { get; set; }
            public int Linea { get; set; }
            public int Chofer { get; set; }
            public int Vehiculo { get; set; }
            public DateTime Desde { get; set; }
            public DateTime Hasta { get; set; }
            public int[] TiposPoi { get; set; }

            public string Serialize()
            {
                return string.Format("{0};{1};{2};{3};{4};{5};{6}",
                                     Empresa,
                                     Linea,
                                     Chofer,
                                     Vehiculo,
                                     Desde.ToString(CultureInfo.InvariantCulture),
                                     Hasta.ToString(CultureInfo.InvariantCulture),
                                     string.Join(",", TiposPoi.Select(x => x.ToString()).ToArray()));
            }
            public static Parameters Deserialize(string serialized)
            {
                var par = serialized.Split(';');
                var p = new Parameters
                {
                    Empresa = Convert.ToInt32(par[0]),
                    Linea = Convert.ToInt32(par[1]),
                    Chofer = Convert.ToInt32(par[2]),
                    Vehiculo = Convert.ToInt32(par[3]),
                    Desde = Convert.ToDateTime(par[4], CultureInfo.InvariantCulture),
                    Hasta = Convert.ToDateTime(par[5], CultureInfo.InvariantCulture),
                    TiposPoi = par[6].Split(',').Select(x => Convert.ToInt32(x)).ToArray()
                };
                return p;
            }
        }
        #endregion
    }
}