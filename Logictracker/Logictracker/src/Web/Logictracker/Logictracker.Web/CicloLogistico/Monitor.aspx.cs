using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.BaseClasses.Util;
using Logictracker.Web.CustomWebControls.DropDownLists;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using NHibernate.Util;
using Point = Logictracker.Web.Monitor.Geometries.Point;

namespace Logictracker.CicloLogistico
{
    public partial class Monitor : OnLineSecuredPage
    {
        public static class Modos
        {
            public const short Ninguno = 0;
            public const short OrdenProgramado = 1;
            public const short OrdenReal = 2;
            public const short PuntoDeEntrega = 3;
            public const short Entrega = 4;
            public const short UltimaVisita = 5;
            public const short ProximaVisita = 6;
        }

        protected override string GetRefference() { return "CLOG_MONITOR"; }
        protected override InfoLabel LblInfo { get { return null; } }
        public PlantaDropDownList ComboLinea { get { return cbLinea; } }

        public static class Layers
        {
            public static string Positions { get { return CultureManager.GetLabel("POSICIONES_REPORTADAS"); } }
            public static string Recorrido { get { return CultureManager.GetLabel("RECORRIDO"); } }
            public static string Puntos { get { return CultureManager.GetLabel("LAYER_POI"); } }
            public static string Geocercas { get { return CultureManager.GetLabel("LAYER_GEOCERCAS"); } }
            public static string Eventos { get { return CultureManager.GetLabel("EVENTOS"); } }
            public static string Mensajes { get { return CultureManager.GetLabel("MENSAJES"); } }
            public static string Detenciones { get { return CultureManager.GetLabel("DETENCIONES"); } }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var googleMapsEnabled = true;
                var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
                if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
                {
                    var empresa = usuario.Empresas.First() as Empresa;
                    if (empresa != null)
                        googleMapsEnabled = empresa.GoogleMapsEnabled;
                }

                RegisterExtJsStyleSheet();
                monitor.Initialize(googleMapsEnabled);
                monitor.EnableTimer = false;
                monitor.AddLayers(LayerFactory.GetVector(Layers.Positions, true, StyleFactory.GetLineFromColor(Color.Blue, 4, 0.5)),
                                  LayerFactory.GetVector(Layers.Recorrido, true, StyleFactory.GetDashedLineFromColor(Color.Red, 4, 0.5)),
                                  LayerFactory.GetVector(Layers.Geocercas, true),
                                  LayerFactory.GetMarkers(Layers.Puntos, true),
                                  LayerFactory.GetMarkers(Layers.Eventos, true),
                                  LayerFactory.GetMarkers(Layers.Mensajes, true),
                                  LayerFactory.GetMarkers(Layers.Detenciones, true));
                monitor.AddControls(ControlFactory.GetToolbar(false, false, false, false, false, true, true));
                
                dtDesde.SelectedDate = DateTime.UtcNow.Date;
                dtHasta.SelectedDate = DateTime.UtcNow.AddDays(1).Date.AddSeconds(-1);

                ApplyParameters();
            }
        }

        private void ApplyParameters()
        {
            var t = Request.QueryString["t"];
            var i = Request.QueryString["i"];
            if (t == null || i == null) return;

            var id = Convert.ToInt32(i);
            chkRecorridoCalculado.Checked = Request.QueryString["c"] == "1";

            if (t == "T")
            {
                var ciclo = DAOFactory.TicketDAO.FindById(id);
                cbEmpresa.SetSelectedValue(ciclo.Empresa != null ? ciclo.Empresa.Id : cbEmpresa.AllValue);
                cbLinea.SetSelectedValue(ciclo.Linea != null ? ciclo.Linea.Id : cbLinea.AllValue);
                cbVehiculo.SetSelectedValue(ciclo.Vehiculo != null ? ciclo.Vehiculo.Id : cbVehiculo.AllValue);
                dtDesde.SelectedDate = ciclo.FechaTicket.Value;
                dtHasta.SelectedDate = ciclo.FechaTicket.Value.AddMinutes(1);

                BtnSearchClick(null, null);
                var row = gridTickets.Rows.Cast<C1GridViewRow>().FirstOrDefault(r => (r.FindControl("hidId") as HiddenField).Value == string.Format("{0}:{1}", t, id));
                if (row != null) gridTickets.SelectedIndex = row.RowIndex;

                SelectCiclo(ciclo);
            }
            else if (t == "D")
            {
                var ciclo = DAOFactory.ViajeDistribucionDAO.FindById(id);
                cbEmpresa.SetSelectedValue(ciclo.Empresa != null ? ciclo.Empresa.Id : cbEmpresa.AllValue);
                cbLinea.SetSelectedValue(ciclo.Linea != null ? ciclo.Linea.Id : cbLinea.AllValue);
                cbEmpleado.SetSelectedValue(ciclo.Empleado != null ? ciclo.Empleado.Id : cbEmpleado.AllValue);
                cbVehiculo.SetSelectedValue(ciclo.Vehiculo != null ? ciclo.Vehiculo.Id : cbVehiculo.AllValue);
                dtDesde.SelectedDate = ciclo.Inicio.ToDisplayDateTime();
                dtHasta.SelectedDate = ciclo.Inicio.ToDisplayDateTime().AddMinutes(1);

                var list = new List<Ciclo>();
                list.Add(new Ciclo(ciclo));                

                gridTickets.DataSource = list;
                gridTickets.DataBind();
                gridTickets.SelectedIndex = 0;

                tab.ActiveTab = tabTickets;
                Clear();

                updTickets.Update();
                updTabCompleto.Update();
                updEast.Update();
                
                if (ciclo.InicioReal.HasValue)
                {
                    SelectCiclo(ciclo, true, Modos.Ninguno);
                    UpdateLinks(ciclo);
                    dtDesde.SelectedDate = ciclo.InicioReal.Value.ToDisplayDateTime();
                    if (ciclo.Estado == ViajeDistribucion.Estados.Cerrado)
                        dtHasta.SelectedDate = ciclo.Fin.ToDisplayDateTime();
                    else
                        dtHasta.SelectedDate = DateTime.UtcNow.ToDisplayDateTime();


                    if (ciclo.EntregasTotalCountConBases > 0)
                    {
                        gridEntregas.DataSource = ciclo.Detalles.Where(d => d.PuntoEntrega != null);
                        gridEntregas.DataBind();
                    }                    
                }
            }
        }

        private void UpdateLinks(ViajeDistribucion distribucion)
        {
            var fin = distribucion.Estado == ViajeDistribucion.Estados.Cerrado ? distribucion.Fin : DateTime.UtcNow;
            var link = string.Format("../Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={0}&TypeMobile={1}&Movil={2}&InitialDate={3}&FinalDate={4}&ShowMessages=0&ShowPOIS=0&Empresa={5}",
                                    distribucion.Vehiculo.Linea != null ? distribucion.Vehiculo.Linea.Id : -1,
                                    distribucion.Vehiculo.TipoCoche.Id,
                                    distribucion.Vehiculo.Id,
                                    distribucion.InicioReal.Value.ToString(CultureInfo.InvariantCulture),
                                    fin.ToString(CultureInfo.InvariantCulture),
                                    distribucion.Vehiculo.Empresa != null ? distribucion.Vehiculo.Empresa.Id : distribucion.Vehiculo.Linea != null ? distribucion.Vehiculo.Linea.Empresa.Id : -1);
            lnkHistorico.Visible = true;
            lnkHistorico.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("OPE_MON_HISTORICO") + "')", link);
            var linkEntregas = string.Format("../Reportes/Estadistica/ReporteDistribucion.aspx?Movil={0}&Fecha={1}",
                                            distribucion.Vehiculo.Id,
                                            distribucion.Inicio.ToString("yyyyMMdd"));
            lnkEstadoEntregas.Visible = true;
            lnkEstadoEntregas.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("REP_DISTRIBUCION") + "')", linkEntregas);
        }

        protected void CbLineaSelectedIndexChanged(object sender, EventArgs e)
        {
            updMensajes.Update();
        }
        
        protected void BtnSearchClick(object sender, EventArgs e)
        {
            var tickets = DAOFactory.TicketDAO.GetList(cbEmpresa.SelectedValues, 
                                                       cbLinea.SelectedValues,
                                                       cbTransportista.SelectedValues,
                                                       new[] { -1 }, // DEPTOS
                                                       cbCentroDeCostos.SelectedValues,
                                                       new[] { -1 }, // TIPOS VEHICULO
                                                       cbVehiculo.SelectedValues,
                                                       new[] { chkActivas.Checked ? Ticket.Estados.EnCurso : -1 }, // ESTADOS
                                                       new[] { -1 }, // CLIENTES
                                                       new[] { -1 }, // PUNTOS DE ENTREGA
                                                       new[] { -1 }, // BOCAS DE CARGA
                                                       dtDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                       dtHasta.SelectedDate.Value.ToDataBaseDateTime());
            var distribuciones = DAOFactory.ViajeDistribucionDAO.GetList(cbEmpresa.SelectedValues,
                                                                         cbLinea.SelectedValues, 
                                                                         cbTransportista.SelectedValues,
                                                                         new[] {-1}, // DEPTOS
                                                                         new[] {-1}, // CENTROS DE COSTO
                                                                         new[] {-1}, // SUB CENTROS DE COSTO
                                                                         cbVehiculo.SelectedValues,
                                                                         new[] { chkActivas.Checked ? ViajeDistribucion.Estados.EnCurso : -1},
                                                                         dtDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                                         dtHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                                .Where(v => v.InicioReal.HasValue);

            var todos = tickets.Select(t => new Ciclo(t))
                .Union(distribuciones.Select(d => new Ciclo(d)))
                .OrderBy(t => t.Fecha);

            gridTickets.DataSource = todos;
            gridTickets.DataBind();

            tab.ActiveTab = tabTickets;
            Clear();

            updTickets.Update();
            updTabCompleto.Update();
            updEast.Update();
        }
        
        protected void BtMensajesClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;
            monitor.ClearLayer(Layers.Mensajes);

            var ciclo = gridTickets.Rows[gridTickets.SelectedIndex].FindControl("hidId") as HiddenField;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            if (ciclo.Value[0] == 'T')
            {
                var ticket = DAOFactory.TicketDAO.FindById(id);
                var inicio = Ciclo.GetFechaInicio(ticket);
                var fin = Ciclo.GetFechaFin(ticket);
                ShowMensajes(ticket.Vehiculo, inicio, fin);
            }
            else if (ciclo.Value[0] == 'D')
            {
                var distribucion = DAOFactory.ViajeDistribucionDAO.FindById(id);
                var inicio = Ciclo.GetFechaInicio(distribucion);
                var fin = Ciclo.GetFechaFin(distribucion);
                ShowMensajes(distribucion.Vehiculo, inicio, fin);
            }
        }
        
        protected void GridTicketsRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var ciclo = e.Row.DataItem as Ciclo;
            if (ciclo == null) return;
            e.Row.MakeRowSelectable();
            var hidId = e.Row.FindControl("hidId") as HiddenField;
            var lblDate = e.Row.FindControl("lblDate") as Label;
            var lblVehiculo = e.Row.FindControl("lblVehiculo") as Label;
            var lblTipo = e.Row.FindControl("lblTipo") as Label;
            var lblCodigo = e.Row.FindControl("lblCodigo") as Label;
            
            hidId.Value = ciclo.TipoId;
            lblDate.Text = ciclo.Inicio.HasValue 
                ? ciclo.Inicio.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm")
                : ciclo.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            lblVehiculo.Text = ciclo.Vehiculo;
            lblTipo.Text = ciclo.Tipo;
            lblCodigo.Text = ciclo.Codigo;
        }

        protected void GridTicketsSelectedIndexChanging(object sender, C1GridViewSelectEventArgs e)
        {
            gridTickets.SelectedIndex = e.NewSelectedIndex;

            Clear();

            var ciclo = gridTickets.Rows[e.NewSelectedIndex].FindControl("hidId") as HiddenField;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));
            
            if (id > 0)
            {
                var viaje = DAOFactory.ViajeDistribucionDAO.FindById(id);

                if (viaje.EntregasTotalCountConBases > 0)
                {
                    gridEntregas.DataSource = viaje.Detalles.Where(d => d.PuntoEntrega != null);
                    gridEntregas.DataBind();
                }
            }

            MostarCiclo(ciclo, id, false, Modos.Ninguno);
        }

        private void MostarCiclo(HiddenField ciclo, int id, bool posicionar, short modo)
        {
            if (ciclo.Value[0] == 'T')
            {
                var ticket = DAOFactory.TicketDAO.FindById(id);
                SelectCiclo(ticket);
            }
            else if (ciclo.Value[0] == 'D')
            {
                lblDistancia.Visible = false;
                var distribucion = DAOFactory.ViajeDistribucionDAO.FindById(id);
                SelectCiclo(distribucion, posicionar, modo);
                UpdateLinks(distribucion);
            }

            updEast.Update();
        }

        protected void BtnOrdenProgOnClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;

            var ciclo = gridTickets.SelectedRow.FindControl("hidId") as HiddenField;
            if (ciclo == null) return;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            MostarCiclo(ciclo, id, true, Modos.OrdenProgramado);
        }

        protected void BtnOrdenRealOnClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;

            var ciclo = gridTickets.SelectedRow.FindControl("hidId") as HiddenField;
            if (ciclo == null) return;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            MostarCiclo(ciclo, id, true, Modos.OrdenReal);
        }

        protected void BtnPuntoEntregaOnClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;

            var ciclo = gridTickets.SelectedRow.FindControl("hidId") as HiddenField;
            if (ciclo == null) return;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            MostarCiclo(ciclo, id, true, Modos.PuntoDeEntrega);
        }

        protected void BtnEntregaOnClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;

            var ciclo = gridTickets.SelectedRow.FindControl("hidId") as HiddenField;
            if (ciclo == null) return;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            MostarCiclo(ciclo, id, true, Modos.Entrega);
        }

        protected void BtnUltimaEntregaOnClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;

            var ciclo = gridTickets.SelectedRow.FindControl("hidId") as HiddenField;
            if (ciclo == null) return;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            MostarCiclo(ciclo, id, true, Modos.UltimaVisita);
        }

        protected void BtnSiguienteEntregaOnClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;

            var ciclo = gridTickets.SelectedRow.FindControl("hidId") as HiddenField;
            if (ciclo == null) return;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            MostarCiclo(ciclo, id, true, Modos.ProximaVisita);
        }

        protected void BtnVehiculoOnClick(object sender, EventArgs e)
        {
            if (gridTickets.SelectedIndex < 0) return;

            var ciclo = gridTickets.SelectedRow.FindControl("hidId") as HiddenField;
            if (ciclo == null) return;
            var id = Convert.ToInt32(ciclo.Value.Substring(2));

            MostarCiclo(ciclo, id, false, Modos.Ninguno);
        }

        protected void GridEntregasRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;
            var entrega = e.Row.DataItem as EntregaDistribucion;
            if (entrega == null) return;

            e.Row.Cells[0].Text = entrega.Orden.ToString("#0");
            e.Row.Cells[1].Text = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado));
            e.Row.Cells[2].Text = entrega.Descripcion;
            e.Row.Cells[3].Text = entrega.PuntoEntrega.Descripcion;
            
            switch (entrega.Estado)
            {
                case EntregaDistribucion.Estados.Completado: e.Row.BackColor = Color.GreenYellow; break;
                case EntregaDistribucion.Estados.NoCompletado: e.Row.BackColor = Color.Red; break;
                case EntregaDistribucion.Estados.Visitado: e.Row.BackColor = Color.Yellow; break;
                case EntregaDistribucion.Estados.EnSitio: e.Row.BackColor = Color.CornflowerBlue; break;
                case EntregaDistribucion.Estados.EnZona: e.Row.BackColor = Color.Gray; break;
                case EntregaDistribucion.Estados.SinVisitar:
                case EntregaDistribucion.Estados.Pendiente: e.Row.BackColor = Color.Orange; break;
            }
        }

        protected void Clear()
        {
            monitor.ClearLayers();
            panelReferenciaSimple.Visible = false;
            panelReferencia.Visible = false;
        }

        protected void SelectCiclo(Ticket ciclo)
        {
            monitor.ClearLayers();

            var inicio = Ciclo.GetFechaInicio(ciclo);
            var fin = Ciclo.GetFechaFin(ciclo);

            var ptBase = new LatLon(ciclo.Linea.ReferenciaGeografica.Latitude, ciclo.Linea.ReferenciaGeografica.Longitude);
            var ptObra = new LatLon(ciclo.PuntoEntrega.ReferenciaGeografica.Latitude, ciclo.PuntoEntrega.ReferenciaGeografica.Longitude);

            var obra = ciclo.Detalles.Cast<DetalleTicket>().FirstOrDefault(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.LlegaAObra);
            if(obra != null)
            {
                var tobra = obra.Manual.HasValue ? obra.Manual.Value : obra.Automatico.HasValue ? obra.Automatico.Value : obra.Programado.Value;

                ShowPosicionesReportadas(ciclo.Vehiculo, inicio, tobra, Color.ForestGreen);
                ShowPosicionesReportadas(ciclo.Vehiculo, tobra.AddSeconds(-1), fin, Color.Blue);

                if (chkRecorridoCalculado.Checked)
                {
                    ShowRecorridoCalculado(Color.Red, ptBase, ptObra);
                    ShowRecorridoCalculado(Color.DarkViolet, ptObra, ptBase);
                }
                panelReferenciaSimple.Visible = false;
                panelReferencia.Visible = true;
                trCalculadoIda.Visible = chkRecorridoCalculado.Checked;
                trCalculadoVuelta.Visible = chkRecorridoCalculado.Checked;
            }
            else
            {
                ShowPosicionesReportadas(ciclo.Vehiculo, inicio, fin, Color.ForestGreen);
                if (chkRecorridoCalculado.Checked)
                    ShowRecorridoCalculado(Color.Red, ptBase, ptObra, ptBase);

                panelReferenciaSimple.Visible = true;
                panelReferencia.Visible = false;
                trCalculado.Visible = chkRecorridoCalculado.Checked;                
            }
            ShowEventos(ciclo.Vehiculo, inicio, fin);
            ShowMensajes(ciclo.Vehiculo, inicio, fin);

            ShowPuntos(ciclo);

            monitor.SetCenter(ptBase.Latitud, ptBase.Longitud);
            monitor.SetDefaultCenter(ptBase.Latitud, ptBase.Longitud);
        }

        protected void SelectCiclo(ViajeDistribucion ciclo, bool posicionar, short modo)
        {
            monitor.ClearLayers();
            var lat = 0.0;
            var lon = 0.0;
            if (ciclo.Linea != null && ciclo.Linea.ReferenciaGeografica != null)
            {
                lat = ciclo.Linea.ReferenciaGeografica.Latitude;
                lon = ciclo.Linea.ReferenciaGeografica.Longitude;
            }
            else if (ciclo.Detalles.Any())
            {
                lat = ciclo.Detalles[0].ReferenciaGeografica.Latitude;
                lon = ciclo.Detalles[0].ReferenciaGeografica.Longitude;
            }
            monitor.SetCenter(lat, lon);
            monitor.SetDefaultCenter(lat, lon);

            var inicio = Ciclo.GetFechaInicio(ciclo);
            var fin = Ciclo.GetFechaFin(ciclo);

            ShowPosicionesReportadas(ciclo.Vehiculo, inicio, fin.AddSeconds(1), Color.ForestGreen);

            var puntos = ciclo.Detalles.Where(e => e.PuntoEntrega != null).Select(e => new LatLon(e.ReferenciaGeografica.Latitude, e.ReferenciaGeografica.Longitude)).ToList();
            if (ciclo.Linea != null)
            {
                puntos.Insert(0, new LatLon(ciclo.Linea.ReferenciaGeografica.Latitude, ciclo.Linea.ReferenciaGeografica.Longitude));
            }

            if (chkRecorridoCalculado.Checked)
            {
                ShowRecorridoCalculado(Color.Red, puntos.ToArray());
                ReferenciaGeografica anterior = null;
                var kmCalculados = 0.0;
                var duracion = new TimeSpan();
                var dets = ciclo.Detalles.OrderBy(d => d.Orden);
                foreach (var detalle in dets)
                {
                    var actual = detalle.PuntoEntrega != null 
                                    ? detalle.PuntoEntrega.ReferenciaGeografica
                                    : detalle.Linea.ReferenciaGeografica;
                    if (anterior != null)
                    {
                        var origen = new LatLon(anterior.Latitude, anterior.Longitude);
                        var destino = new LatLon(actual.Latitude, actual.Longitude);
                        var dir = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty);
                        if (dir != null)
                        {
                            kmCalculados += (dir.Distance/1000.0);
                            duracion = duracion.Add(dir.Duration);
                        }
                    }

                    anterior = actual;
                }
                lblKmCalculados.Text = kmCalculados.ToString("#0.00") + " Km - " + string.Format("{0}:{1}:{2} Hs", ((int)duracion.TotalHours).ToString("00"), duracion.Minutes.ToString("00"), duracion.Seconds.ToString("00"));
            }
            
            ShowEventos(ciclo.Vehiculo, inicio, fin, ciclo.Id);
            ShowMensajes(ciclo.Vehiculo, inicio, fin);
            if (ciclo.Linea != null) ShowPuntos(new[] {ciclo.Linea.ReferenciaGeografica}, 0);
            ShowPuntos(posicionar, modo, ciclo.Detalles.Where(e => e.PuntoEntrega != null).ToArray());

            panelReferenciaSimple.Visible = true;
            panelReferencia.Visible = false; 
            trCalculado.Visible = chkRecorridoCalculado.Checked;

            var kmReales = 0.0;
            var duracionReal = new TimeSpan();
            if (ciclo.InicioReal.HasValue)
            {
                if (ciclo.InicioReal.Value < DateTime.Today)
                {
                    duracionReal = ciclo.Fin.Subtract(ciclo.InicioReal.Value);
                    var dmViaje = DAOFactory.DatamartViajeDAO.GetRecords(ciclo.Id).FirstOrDefault();
                    if (dmViaje != null) kmReales = dmViaje.KmTotales;
                }
                else 
                    kmReales = DAOFactory.CocheDAO.GetDistance(ciclo.Vehiculo.Id, inicio, fin);
            }

            lblKmReales.Text = kmReales.ToString("#0.00") + " Km - " + string.Format("{0}:{1}:{2} Hs", ((int)duracionReal.TotalHours).ToString("00"), duracionReal.Minutes.ToString("00"), duracionReal.Seconds.ToString("00"));
            var inicioProgramado = ciclo.Detalles.Min(d => d.Programado);
            var finProgramado = ciclo.Detalles.Max(d => d.Programado);
            var duracionProgramada = finProgramado.Subtract(inicioProgramado);
            lblKmProgramados.Text = ciclo.Detalles.Sum(d => d.KmCalculado).Value.ToString("#0.00") + " Km - " + string.Format("{0}:{1}:{2} Hs", ((int)duracionProgramada.TotalHours).ToString("00"), duracionProgramada.Minutes.ToString("00"), duracionProgramada.Seconds.ToString("00"));
        }

        protected void ShowPosicionesReportadas(Coche vehiculo, DateTime desde, DateTime hasta, Color color)
        {
            if (vehiculo == null) return;
            var maxMonths = vehiculo.Empresa != null ? vehiculo.Empresa.MesesConsultaPosiciones : 3;
            var posiciones = DAOFactory.LogPosicionDAO.GetPositionsBetweenDates(vehiculo.Id, desde, hasta, maxMonths);
            var line = new Line("V:" + vehiculo.Id + ":" + color.ToArgb(), StyleFactory.GetLineFromColor(color, 4, 0.5));
            line.AddPoints(posiciones.Select(p => new Point("", p.Longitud, p.Latitud)));
            monitor.AddGeometries(Layers.Positions, line);
            
            if (posiciones.Count > 0)
            {
                var lastPos = posiciones.Last();
                monitor.SetCenter(lastPos.Latitud, lastPos.Longitud);
                var icon = vehiculo.TipoCoche.IconoNormal;
                var url = IconDir + vehiculo.TipoCoche.IconoNormal.PathIcono;
                var marker = new Marker(vehiculo.Id.ToString("#0"), url, lastPos.Latitud, lastPos.Longitud)
                                 {
                                     Size = DrawingFactory.GetSize(icon.Width, icon.Height),
                                     Offset = DrawingFactory.GetOffset(icon.OffsetX, icon.OffsetY)
                                 };
                monitor.AddMarkers(Layers.Puntos, marker);
            }
        }

        protected void ShowEventos(Coche vehiculo, DateTime desde, DateTime hasta)
        {
            ShowEventos(vehiculo, desde, hasta, 0);
        }
        protected void ShowEventos(Coche vehiculo, DateTime desde, DateTime hasta, int viajeId)
        {
            if (vehiculo == null) return;
            var codigos = new[] { MessageCode.CicloLogisticoIniciado.GetMessageCode(), 
                                  MessageCode.EstadoLogisticoCumplido.GetMessageCode(), 
                                  MessageCode.EstadoLogisticoCumplidoEntrada.GetMessageCode(), 
                                  MessageCode.EstadoLogisticoCumplidoSalida.GetMessageCode(),
                                  MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode(), 
                                  MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(), 
                                  MessageCode.EstadoLogisticoCumplidoManualNoRealizado.GetMessageCode(), 
                                  MessageCode.CicloLogisticoCerrado.GetMessageCode(), 
                                  MessageCode.StoppedEvent.GetMessageCode() };
            var maxMonths = vehiculo.Empresa != null ? vehiculo.Empresa.MesesConsultaPosiciones : 3;
            var events = DAOFactory.LogMensajeDAO.GetEventos(new[] { vehiculo.Id }, codigos, desde, hasta, maxMonths);
            
            for (var i = 0; i < events.Count(); i++)
            {
                var el = events.ElementAt(i);
                if (!el.HasValidLatitudes()) continue;
                
                var messageIconUrl = el.GetIconUrl();
                if (el.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoEntrada.GetMessageCode())
                    messageIconUrl = "flag_1_right_green_32.png";
                else if (el.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoSalida.GetMessageCode())
                    messageIconUrl = "flag_2_left_red_32.png";
                else if (el.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoManual.GetMessageCode()
                    || el.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode()
                    || el.Mensaje.Codigo == MessageCode.EstadoLogisticoCumplidoManualNoRealizado.GetMessageCode())
                    messageIconUrl = "flag_3_right_blue_32.png";
                                
                var iconUrl = string.IsNullOrEmpty(messageIconUrl) ? ResolveUrl("~/point.png") : Path.Combine(IconDir, messageIconUrl);
                var popupText = string.Format("{0}<br/><b>{1}</b>",el.Fecha.ToDisplayDateTime().ToString("dd-MM-yyyy HH:mm"), el.Texto);
                
                if (el.Mensaje.Codigo == MessageCode.StoppedEvent.GetMessageCode() && viajeId != 0) 
                    popupText = GetDetencionPopupContent(el.Id, viajeId);

                var marker = MarkerFactory.CreateMarker("E:" + el.Id, iconUrl, el.Latitud, el.Longitud, popupText);

                monitor.AddMarkers(el.Mensaje.Codigo == MessageCode.StoppedEvent.GetMessageCode() 
                                        ? Layers.Detenciones 
                                        : Layers.Eventos,
                                   marker);
            }
        }

        protected void ShowRecorridoCalculado(Color color, params LatLon[] points)
        {
            var origen = new LatLon(points[0].Latitud, points[0].Longitud);
            var destino = new LatLon(points[points.Length - 1].Latitud, points[points.Length - 1].Longitud);
            var waypoints = points.Skip(1).Take(points.Length - 2).ToList();
            if (waypoints.Count > 8) waypoints = waypoints.Take(8).ToList();
            var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, waypoints.ToArray());
            var posiciones = directions.Legs.SelectMany(l => l.Steps.SelectMany(s => s.Points));
            var line = new Line("D:" + color.ToArgb(), StyleFactory.GetDashedLineFromColor(color, 4, 0.5));
            line.AddPoints(posiciones.Select(p => new Point("", p.Longitud, p.Latitud)));
            monitor.AddGeometries(Layers.Recorrido, line);
        }
        protected void ShowPuntos(Ticket ciclo)
        {
            var detalles = ciclo.Detalles.Cast<DetalleTicket>();

            var eBase = detalles.FirstOrDefault(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.LlegaAPlanta);
            var sBase = detalles.FirstOrDefault(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.SaleDePlanta);
            var iconoBase = ciclo.Linea.ReferenciaGeografica.Icono != null ? Path.Combine(IconDir, ciclo.Linea.ReferenciaGeografica.Icono.PathIcono) : ResolveUrl("~/point.png");

            var popupBase = string.Format("javascript:ticket('{0}','{1}','{2}','{3}','{4}','{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                ciclo.Linea.Descripcion.Replace("'", "\\'"),
                ciclo.Linea.DescripcionCorta.Replace("'", "\\'"),
                iconoBase,
                sBase != null ? sBase.Programado.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                sBase != null && sBase.Automatico.HasValue ? sBase.Automatico.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                sBase != null && sBase.Manual.HasValue ? sBase.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                eBase != null ? eBase.Programado.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                eBase != null && eBase.Automatico.HasValue ? eBase.Automatico.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                eBase != null && eBase.Manual.HasValue ? eBase.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                "Salida", "Entrada");

            ciclo.Linea.ReferenciaGeografica.Observaciones = popupBase;

            var eObra = detalles.FirstOrDefault(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.LlegaAObra);
            var sObra = detalles.FirstOrDefault(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.SaleDeObra);
            var iconoObra = ciclo.PuntoEntrega.ReferenciaGeografica.Icono != null ? Path.Combine(IconDir, ciclo.PuntoEntrega.ReferenciaGeografica.Icono.PathIcono) : ResolveUrl("~/point.png");

            var popupObra = string.Format("javascript:ticket('{0}','{1}','{2}','{3}','{4}','{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                ciclo.PuntoEntrega.Descripcion.Replace("'", "\\'"),
                ciclo.PuntoEntrega.Codigo.Replace("'", "\\'"),
                iconoObra,
                eObra != null ? eObra.Programado.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                eObra != null && eObra.Automatico.HasValue ? eObra.Automatico.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                eObra != null && eObra.Manual.HasValue ? eObra.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                sObra != null ? sObra.Programado.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                sObra != null && sObra.Automatico.HasValue ? sObra.Automatico.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                sObra != null && sObra.Manual.HasValue ? sObra.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty,
                "Entrada", "Salida");

            ciclo.PuntoEntrega.ReferenciaGeografica.Observaciones = popupObra;

            ShowPuntos(new[] {ciclo.Linea.ReferenciaGeografica, ciclo.PuntoEntrega.ReferenciaGeografica}, 0);
        }
        protected void ShowPuntos(bool posicionar, int modo, params EntregaDistribucion[] entregas)
        {
            foreach(var e in entregas)
            {
                e.PuntoEntrega.ReferenciaGeografica.Observaciones = GetPuntoEntregaPopupContent(e.Id);
            }
            ShowPuntos(entregas.OrderBy(e => e.Orden), 1, posicionar, modo);
        }
        protected void ShowPuntos(IEnumerable<EntregaDistribucion> entregas, int index, bool posicionar, int modo)
        {
            var orden = index;
            ReferenciaGeografica toPosition = null;
            var viaje = entregas.First().Viaje;
            var vehiculo = viaje.Vehiculo;
            var viajeEnCurso = viaje.Estado == ViajeDistribucion.Estados.EnCurso;
            var entregaActiva = vehiculo.GetActiveStop();
            STrace.Trace("Monitor de ciclo", "Vehiculo: " + vehiculo.Id + " entregaActiva: " + entregaActiva);

            var visitadas = entregas.Where(e => e.Entrada.HasValue);
            var lastVisited = visitadas.Any() 
                                ? visitadas.OrderByDescending(ent => ent.Entrada.Value).Select(e => e.Id).First()
                                : 0;

            foreach (var entrega in entregas)
            {
                var punto = entrega.ReferenciaGeografica;
                if (punto == null) continue;
                var icono = punto.Icono != null ? Path.Combine(IconDir, punto.Icono.PathIcono) : ResolveUrl("~/point.png");

                var horario = entrega.Entrada.HasValue
                                ? " (" + entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") + ")"
                                : entrega.Manual.HasValue
                                    ? " (" + entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") + ")"
                                    : string.Empty;
                string style;
                Color color;
                switch (entrega.Estado)
                {
                    case EntregaDistribucion.Estados.Completado: 
                        style = "ol_marker_labeled_green";
                        color = Color.Green;
                        break;
                    case EntregaDistribucion.Estados.Visitado: 
                        style = "ol_marker_labeled_yellow";
                        color = Color.Gold;
                        break;
                    case EntregaDistribucion.Estados.EnSitio: 
                        style = "ol_marker_labeled_blue";
                        color = Color.RoyalBlue;
                        break;
                    case EntregaDistribucion.Estados.EnZona: 
                        style = "ol_marker_labeled_gray";
                        color = Color.DarkGray;
                        break;
                    case EntregaDistribucion.Estados.NoCompletado:
                        style = "ol_marker_labeled_red";
                        color = Color.Red;
                        break;
                    default:
                        style = "ol_marker_labeled_orange";
                        color = Color.DarkOrange;
                        break;
                }

                if (lastVisited == entrega.Id)
                    color = Color.Purple;

                var eta = string.Empty;
                if (viajeEnCurso && entregaActiva == entrega.Id)
                {
                    color = Color.Cyan;
                    var cacheEta = vehiculo.EtaEstimated();
                    
                    if (cacheEta.HasValue)
                    {
                        eta = " - ETA: " + cacheEta.Value.ToDisplayDateTime().ToString("HH:mm");
                        STrace.Trace("Monitor de ciclo", "Vehiculo: " + vehiculo.Id + " ETA: " + cacheEta.Value.ToDisplayDateTime().ToString("HH:mm"));
                    }
                    else
                    {
                        STrace.Trace("Monitor de ciclo", "Vehiculo: " + vehiculo.Id + " ETA: NULL");
                    }
                }

                var text = orden.ToString("#0") + horario + eta;
                var marker = MarkerFactory.CreateLabeledMarker("P:" + punto.Id, icono, punto.Latitude, punto.Longitude, text, style, punto.Observaciones);
                monitor.AddMarkers(Layers.Puntos, marker);
                if (posicionar)
                {
                    switch (modo)
                    {
                        case Modos.OrdenProgramado:
                            if (txtOrdenProg.Text.Trim() == orden.ToString("#0"))
                                toPosition = punto;
                            break;
                        case Modos.OrdenReal:
                            var entr = entrega.Viaje.GetEntregasPorOrdenReal();
                            var ordenReal = entr.IndexOf(entrega);

                            if (txtOrdenReal.Text.Trim() == ordenReal.ToString("#0"))
                                toPosition = punto;
                            break;
                        case Modos.PuntoDeEntrega:
                            if (txtPuntoEntrega.Text.Trim() == entrega.PuntoEntrega.Codigo.Trim())
                                toPosition = punto;
                            break;
                        case Modos.Entrega:
                            if (txtEntrega.Text.Trim() == entrega.Descripcion.Trim())
                                toPosition = punto;
                            break;
                        case Modos.UltimaVisita:
                            if (entrega.Id == lastVisited) 
                                toPosition = punto;
                            break;
                        case Modos.ProximaVisita:
                            if (viajeEnCurso && entregaActiva == entrega.Id)
                                toPosition = punto;
                            break;
                    }
                }

                orden++;

                if (punto.Poligono != null)
                {
                    Geometry geocerca;
                    if (punto.Poligono.Radio > 0)
                    {
                        var point = punto.Poligono.FirstPoint;
                        geocerca = new Point("G:" + punto.Id, point.X, point.Y, punto.Poligono.Radio, StyleFactory.GetPointFromColor(color));
                    }
                    else
                    {
                        var poligono = new Polygon("G:" + punto.Id, StyleFactory.GetPointFromColor(color));
                        var points = punto.Poligono.ToPointFList();
                        for (var i = 0; i < points.Count; i++) poligono.AddPoint(new Point("", points[i].X, points[i].Y));
                        geocerca = poligono;
                    }
                    monitor.AddGeometries(Layers.Geocercas, geocerca);
                }
            }

            if (toPosition != null)
            {
                PosicionarPunto(toPosition);
                if (viajeEnCurso)
                {
                    var lastPosition = DAOFactory.LogPosicionDAO.GetLastVehiclePosition(vehiculo);
                    var distancia = GeocoderHelper.CalcularDistacia(toPosition.Latitude, toPosition.Longitude, lastPosition.Latitud, lastPosition.Longitud);
                    lblDistancia.Text = "Distancia: " + distancia.ToString("#0.00") + "km";
                    lblDistancia.Visible = true;
                }
                if (toPosition.Poligono != null)
                {
                    var color = Color.Black;
                    Geometry geocerca;
                    if (toPosition.Poligono.Radio > 0)
                    {
                        var point = toPosition.Poligono.FirstPoint;
                        geocerca = new Point("G:" + toPosition.Id, point.X, point.Y, toPosition.Poligono.Radio, StyleFactory.GetPointFromColor(color));
                    }
                    else
                    {
                        var poligono = new Polygon("G:" + toPosition.Id, StyleFactory.GetPointFromColor(color));
                        var points = toPosition.Poligono.ToPointFList();
                        for (var i = 0; i < points.Count; i++) poligono.AddPoint(new Point("", points[i].X, points[i].Y));
                        geocerca = poligono;
                    }
                    monitor.AddGeometries(Layers.Geocercas, geocerca);
                }
            }
        }
        protected void ShowPuntos(ReferenciaGeografica[] puntos, int index)
        {
            var orden = index;
            foreach (var punto in puntos)
            {
                if (punto == null) continue;
                var icono = punto.Icono != null ? Path.Combine(IconDir, punto.Icono.PathIcono) : ResolveUrl("~/point.png");
                
                var marker = MarkerFactory.CreateLabeledMarker("P:" + punto.Id, icono, punto.Latitude, punto.Longitude, orden.ToString("#0"), string.Empty, punto.Observaciones);
                monitor.AddMarkers(Layers.Puntos, marker);
                orden++;

                if (punto.Poligono != null)
                {
                    var color = punto.Color != null ? punto.Color.Color : Color.Blue;
                    Geometry geocerca;
                    if (punto.Poligono.Radio > 0)
                    {
                        var point = punto.Poligono.FirstPoint;
                        geocerca = new Point("G:" + punto.Id, point.X, point.Y, punto.Poligono.Radio, StyleFactory.GetPointFromColor(color));
                    }
                    else
                    {
                        var poligono = new Polygon("G:" + punto.Id, StyleFactory.GetPointFromColor(color));
                        var points = punto.Poligono.ToPointFList();
                        for (var i = 0; i < points.Count; i++) poligono.AddPoint(new Point("", points[i].X, points[i].Y));
                        geocerca = poligono;
                    }
                    monitor.AddGeometries(Layers.Geocercas, geocerca);
                }
            }
        }
        protected void ShowMensajes(Coche vehiculo, DateTime desde, DateTime hasta)
        {
            if (vehiculo == null) return;
            var maxMonths = vehiculo.Empresa != null ? vehiculo.Empresa.MesesConsultaPosiciones : 3;
            var events = DAOFactory.LogMensajeDAO.GetEventos(new[] { vehiculo.Id }, cbMensajes.SelectedValues.Select(m => m.ToString()), desde, hasta, maxMonths);

            for (var i = 0; i < events.Count(); i++)
            {
                var el = events.ElementAt(i);
                if (!el.HasValidLatitudes()) continue;

                var messageIconUrl = el.GetIconUrl();
                var iconUrl = string.IsNullOrEmpty(messageIconUrl) ? ResolveUrl("~/point.png") : Path.Combine(IconDir, messageIconUrl);
                var popupText = string.Format("{0}<br/><b>{1}</b>", el.Fecha.ToDisplayDateTime().ToString("dd-MM-yyyy HH:mm"), el.Texto);
                monitor.AddMarkers(Layers.Mensajes, MarkerFactory.CreateMarker("M:" + el.Id, iconUrl, el.Latitud, el.Longitud, popupText));
            }
        }
        private static string GetPuntoEntregaPopupContent(int id)
        {
            return "javascript:getPuntoEntrega('" + id + "')";
        }
        private static string GetDetencionPopupContent(int eventoId, int viajeId)
        {
            return "javascript:getDetencion('" + eventoId + "', '" + viajeId + "')";
        }

        protected void BtnBuscarRutaClick(object sender, EventArgs e)
        {
            var code = txtBuscarRuta.Text.Trim();
            var rutas = DAOFactory.ViajeDistribucionDAO.GetByTexto(cbEmpresa.SelectedValues, 
                                                                   cbLinea.SelectedValues, 
                                                                   cbTransportista.SelectedValues,
                                                                   new[]{-1}, // VEHICULOS
                                                                   dtDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                                   dtHasta.SelectedDate.Value.ToDataBaseDateTime(),
                                                                   code);
            
            var viajes = rutas.Select(v => new Ciclo(v));
            gridTickets.DataSource = viajes;
            gridTickets.DataBind();

            tab.ActiveTab = tabTickets;
            Clear();

            updTickets.Update();
            updTabCompleto.Update();
            updEast.Update();

            if (rutas.Count() == 1 && rutas.First().InicioReal.HasValue) 
                SelectCiclo(rutas.First(), false, Modos.Ninguno);
        }

        protected void BtnBuscarClienteClick(object sender, EventArgs e)
        {
            var code = txtBuscarCliente.Text.Trim();
            var cliente = DAOFactory.ClienteDAO.GetByCode(cbEmpresa.SelectedValues, cbLinea.SelectedValues, code);

            if (cliente != null && cliente.Id != 0)
            {
                var entregas = DAOFactory.EntregaDistribucionDAO.GetList(cbEmpresa.SelectedValues,
                                                                         cbLinea.SelectedValues,
                                                                         cbTransportista.SelectedValues,
                                                                         new[] {-1}, // DEPTOS
                                                                         new[] {-1}, // CENTROS DE COSTO
                                                                         new[] {-1}, // SUB CENTROS DE COSTO
                                                                         new[] { -1 }, // VEHICULOS
                                                                         new[] {-1}, // VIAJES
                                                                         new[] {-2}, // ESTADOS
                                                                         dtDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                                         dtHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                                .Where(ent => ent.Cliente != null && ent.Cliente.Id == cliente.Id);

                var viajes = entregas.Select(ent => ent.Viaje)
                                     .Where(v => v.Estado != ViajeDistribucion.Estados.Eliminado)
                                     .Distinct()
                                     .Select(v => new Ciclo(v));

                gridTickets.DataSource = viajes;
                gridTickets.DataBind();

                if (viajes.Count() == 1 && entregas.First().Viaje.InicioReal.HasValue)
                    SelectCiclo(entregas.First().Viaje, false, Modos.Ninguno);
            }

            tab.ActiveTab = tabTickets;
            Clear();

            updTickets.Update();
            updTabCompleto.Update();
            updEast.Update();
        }

        protected void BtnBuscarPuntoEntregaClick(object sender, EventArgs e)
        {
            var code = txtBuscarPuntoEntrega.Text.Trim();
            var punto = DAOFactory.PuntoEntregaDAO.FindByCode(cbEmpresa.SelectedValues, cbLinea.SelectedValues, new[]{-1}, code);

            var entregas = new List<EntregaDistribucion>();
            var viajes = new List<Ciclo>();

            if (punto != null && punto.Id != 0)
            {
                entregas = DAOFactory.EntregaDistribucionDAO.GetList(cbEmpresa.SelectedValues,
                                                                     cbLinea.SelectedValues,
                                                                     cbTransportista.SelectedValues,
                                                                     new[] { -1 }, // DEPTOS
                                                                     new[] { -1 }, // CENTROS DE COSTO
                                                                     new[] { -1 }, // SUB CENTROS DE COSTO
                                                                     new[] { -1 }, // VEHICULOS
                                                                     new[] { -1 }, // VIAJES
                                                                     new[] { -2 }, // ESTADOS
                                                                     dtDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                                     dtHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                            .Where(ent => ent.PuntoEntrega != null && ent.PuntoEntrega.Id == punto.Id)
                                                            .ToList();

                viajes = entregas.Select(ent => ent.Viaje)
                                 .Where(v => v.Estado != ViajeDistribucion.Estados.Eliminado)
                                 .Distinct()
                                 .Select(v => new Ciclo(v))
                                 .ToList();

                gridTickets.DataSource = viajes;
                gridTickets.DataBind();
            }

            tab.ActiveTab = tabTickets;
            Clear();

            updTickets.Update();
            updTabCompleto.Update();
            updEast.Update();

            if (entregas.Count() == 1 && entregas.First().Viaje.InicioReal.HasValue)
            {
                txtPuntoEntrega.Text = entregas.First().PuntoEntrega.Codigo;
                SelectCiclo(entregas.First().Viaje, true, Modos.PuntoDeEntrega);
            }
        }

        protected void BtnBuscarEntregaClick(object sender, EventArgs e)
        {
            var code = txtBuscarEntrega.Text.Trim();

            var entregas = DAOFactory.EntregaDistribucionDAO.GetList(cbEmpresa.SelectedValues,
                                                                     cbLinea.SelectedValues,
                                                                     cbTransportista.SelectedValues,
                                                                     new[] {-1}, // DEPTOS
                                                                     new[] {-1}, // CENTROS DE COSTO
                                                                     new[] {-1}, // SUB CENTROS DE COSTO
                                                                     new[] { -1 }, // VEHICULOS
                                                                     new[] {-1}, // VIAJES
                                                                     new[] {-2}, // ESTADOS
                                                                     dtDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                                     dtHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                            .Where(ent => ent.Descripcion.Trim() == code);

            var viajes = entregas.Select(ent => ent.Viaje)
                                 .Where(v => v.Estado != ViajeDistribucion.Estados.Eliminado)
                                 .Distinct()
                                 .Select(v => new Ciclo(v));

            gridTickets.DataSource = viajes;
            gridTickets.DataBind();

            if (viajes.Count() == 1 && entregas.First().Viaje.InicioReal.HasValue)
                SelectCiclo(entregas.First().Viaje, false, Modos.Ninguno);

            tab.ActiveTab = tabTickets;
            Clear();

            updTickets.Update();
            updTabCompleto.Update();
            updEast.Update();

            if (entregas.Count() == 1 && entregas.First().Viaje.InicioReal.HasValue)
            {
                txtEntrega.Text = entregas.First().Descripcion;
                SelectCiclo(entregas.First().Viaje, true, Modos.Entrega);
            }
        }

        private void PosicionarPunto(ReferenciaGeografica punto)
        {
            monitor.SetCenter(punto.Latitude, punto.Longitude);
            monitor.ZoomTo(13);
        }
    }

    public class Ciclo
    {
        public string Tipo { get; set; }
        public int Id { get; set; }
        public string TipoId { get; set; }
        public string Vehiculo { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? Inicio { get; set; }
        public string Codigo { get; set; }

        public Ciclo(Ticket ticket)
        {
            Tipo = CultureManager.GetEntity("OPETICK01");
            Id = ticket.Id;
            TipoId = "T:" + ticket.Id;
            Vehiculo = ticket.Vehiculo != null ? ticket.Vehiculo.Interno : string.Empty;
            Fecha = ticket.FechaTicket.Value;
            Codigo = ticket.Codigo;
        }

        public Ciclo(ViajeDistribucion distribucion)
        {
            Tipo = CultureManager.GetEntity("DISTRIBUCION");
            Id = distribucion.Id;
            TipoId = "D:" + distribucion.Id;
            Vehiculo = distribucion.Vehiculo != null ? distribucion.Vehiculo.Interno : string.Empty;
            Fecha = distribucion.Inicio;
            Codigo = distribucion.Codigo;
            Inicio = distribucion.InicioReal;
        }

        public static DateTime GetFechaFin(Ticket ciclo)
        {
            var ultimo = ciclo.Detalles.Cast<DetalleTicket>().Last();
            var fin = ultimo.Programado.Value;
            if (ultimo.Automatico.HasValue && ultimo.Automatico.Value > fin) fin = ultimo.Automatico.Value;
            if (ultimo.Manual.HasValue && ultimo.Manual.Value > fin) fin = ultimo.Manual.Value;
            return fin;
        }
        public static DateTime GetFechaFin(ViajeDistribucion ciclo)
        {
            if (ciclo.Estado == ViajeDistribucion.Estados.Cerrado) return ciclo.Fin;

            return DateTime.UtcNow;
            //var times = ciclo.Detalles.Select(x => x.Programado);
            //times = times.Union(ciclo.Detalles.Where(x => x.Manual.HasValue).Select(x => x.Manual.Value));
            //times = times.Union(ciclo.Detalles.Where(x => x.Entrada.HasValue).Select(x => x.Entrada.Value));
            //times = times.Union(ciclo.Detalles.Where(x => x.Salida.HasValue).Select(x => x.Salida.Value));
            //return times.Max();
        }
        public static DateTime GetFechaInicio(Ticket ciclo)
        {
            return ciclo.FechaTicket.Value;
        }
        public static DateTime GetFechaInicio(ViajeDistribucion ciclo)
        {
            if (ciclo.InicioReal.HasValue) return ciclo.InicioReal.Value;

            var first = ciclo.Detalles.First();
            var times = new List<DateTime>{first.Programado};
            if (first.Manual.HasValue) times.Add(first.Manual.Value);
            if (first.Entrada.HasValue) times.Add(first.Entrada.Value);
            if (first.Salida.HasValue) times.Add(first.Salida.Value);
            return times.Min();
        }
    }
}