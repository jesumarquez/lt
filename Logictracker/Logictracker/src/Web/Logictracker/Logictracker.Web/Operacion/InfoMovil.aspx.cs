using System.Drawing;
using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Operacion
{
    public partial class InfoMovil : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string GetRefference() { return "MONITOR"; }

        protected override string PageTitle { get { return string.Format("{0} - InfoMovil", ApplicationTitle); } }

        protected Coche pageCoche;

        protected int IdDispositivo
        {
            get{ return (int)(ViewState["IdDispositivo"]??0);}
            set { ViewState["IdDispositivo"] = value; }
        }
        protected int IdVehiculo
        {
            get { return (int)(ViewState["IdVehiculo"] ?? 0); }
            set { ViewState["IdVehiculo"] = value; }
        }
        protected List<int> IdsLineas
        {
            get { return (List<int>)(ViewState["IdsLineas"] ?? new List<int>()); }
            set { ViewState["IdsLineas"] = value; }
        }
        protected List<int> IdsEmpresas
        {
            get { return (List<int>)(ViewState["IdsEmpresas"] ?? new List<int>()); }
            set { ViewState["IdsEmpresas"] = value; }
        }
        protected int LoadStep
        {
            get { return (int)(ViewState["LoadStep"] ?? 0); }
            set { ViewState["LoadStep"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            IdVehiculo = Convert.ToInt32(Request.QueryString["c"]);
            IdsLineas = Request.QueryString["l"].Split(' ').Select(l => Convert.ToInt32(l)).ToList();
            IdsEmpresas = !string.IsNullOrEmpty(Request.QueryString["e"])
                              ? Request.QueryString["e"].Split(' ').Select(l => Convert.ToInt32(l)).ToList()
                              : new List<int> { -1 };


            pageCoche = DAOFactory.CocheDAO.FindById(IdVehiculo);
        }
                
        protected void Page_Load(object sender, EventArgs e)
        {            
            if (IsPostBack) return;
   
            if (Request.QueryString["c"] == null)
            {
                lblInterno.Text = CultureManager.GetError("NO_VEHICLE_INFO");
                return;
            }

            lblTitVelocidad.Visible = lblVelocidad.Visible = lblKmH.Visible = lblPatente.Visible = lblOdometro.Visible = !pageCoche.TipoCoche.SeguimientoPersona;

            IdDispositivo = pageCoche.Dispositivo != null ? pageCoche.Dispositivo.Id : -1;

            lblInterno.Text = pageCoche.Interno;
            lblPatente.Text = string.Concat(CultureManager.GetLabel("PATENTE"), ": ", pageCoche.Patente);
            if (pageCoche.Transportista != null)
            {
                lblTransportista.Text = pageCoche.Transportista.Descripcion;    
            }            
            lblOdometro.Text = string.Concat(CultureManager.GetLabel("ODOMETRO"), ": ", (pageCoche.InitialOdometer + pageCoche.ApplicationOdometer).ToString("0.00"), "km");
            lblTipo.Text = pageCoche.TipoCoche.Descripcion;
            imgTipo.ImageUrl = IconDir + pageCoche.TipoCoche.IconoDefault.PathIcono;

            var pos = DAOFactory.LogPosicionDAO.GetLastVehiclePosition(pageCoche);
            if (pos == null) return;

            lblFechaPosicion.Text = pos.FechaMensaje.ToDisplayDateTime().ToString("dddd dd \"de\" MMMM \"de\" yyyy HH:mm:ss");
            lblVelocidad.Text = pos.Velocidad.ToString("#0");        
            lblPosicion.Text = string.Format(CultureInfo.InvariantCulture, "({0}, {1})", pos.Latitud, pos.Longitud);

            var empleado = DAOFactory.EmpleadoDAO.GetLoggedInDriver(pageCoche);
            lblChofer.Text = empleado != null ? empleado.Entidad.Descripcion : CultureManager.GetLabel("NINGUNO");

            imgDirection.ImageUrl = "~/Common/EditImage.ashx?file=~/images/arrow_green.png&angle=" + Convert.ToInt32(pos.Curso);

            var desde = pageCoche.Empresa.MonitorHistoricoDiaEntero ? pos.FechaMensaje.Date.ToDataBaseDateTime().ToString(CultureInfo.InvariantCulture) : pos.FechaMensaje.Subtract(TimeSpan.FromMinutes(15)).ToString(CultureInfo.InvariantCulture);

            
            var link = string.Format("../Monitor/MonitorHistorico/monitorHistorico.aspx?Planta={0}&TypeMobile={1}&Movil={2}&InitialDate={3}&FinalDate={4}&ShowMessages=0&ShowPOIS=0&Empresa={5}&Chofer={6}",
                                    pageCoche.Linea != null ? pageCoche.Linea.Id : -1,
                                    pageCoche.TipoCoche.Id,
                                    pageCoche.Id,
                                    desde,
                                    pos.FechaMensaje.Add(TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                                    pageCoche.Empresa != null ? pageCoche.Empresa.Id : pageCoche.Linea != null ? pageCoche.Linea.Empresa.Id : -1,
                                    pageCoche.Chofer != null ? pageCoche.Chofer.Id : -1);

            lnkHistorico.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("OPE_MON_HISTORICO") + "')", link);

            var linkCalidad = string.Format("../Monitor/MonitorDeCalidad/monitorDeCalidad.aspx?Location={0}&TypeMobile={1}&Mobile={2}&InitialDate={3}&FinalDate={4}&Distrito={5}&Chofer={6}",
                                            pageCoche.Linea != null ? pageCoche.Linea.Id : -1, 
                                            pageCoche.TipoCoche.Id,
                                            pageCoche.Id, 
                                            pos.FechaMensaje.Subtract(TimeSpan.FromMinutes(15)).ToString("yyyy/MM/dd HH:mm:ss"),
                                            pos.FechaMensaje.Add(TimeSpan.FromMinutes(1)).ToString("yyyy/MM/dd HH:mm:ss"), 
                                            pageCoche.Empresa != null ? pageCoche.Empresa.Id : pageCoche.Linea != null ? pageCoche.Linea.Empresa.Id : -1,
                                            pageCoche.Chofer != null ? pageCoche.Chofer.Id : -1);
        
            lnkCalidad.OnClientClick = string.Format("window.open('{0}', '" + CultureManager.GetMenu("OPE_MON_CALIDAD") + "')", linkCalidad);
        }

        protected void BtVerCercanosClick(object sender, EventArgs e)
        {
            var pos = SharedPositions.GetLastPositions(new List<Coche> { pageCoche }).FirstOrDefault();
            if (pos != null) VehiculosCercanos(pageCoche, pos);
            divCercanos.Visible = true;
            spanVerCercanos.Visible = false;
        }

        protected void TimerGeocoderTick(object sender, EventArgs e)
        {
            if (LoadStep == 0)
            {
                // Paso 0: Direccion - Geocoder
                try
                {
                    var latlon = lblPosicion.Text.TrimStart('(').TrimEnd(')').Split(',');
                    var lat = Convert.ToDouble(latlon[0].Trim(), CultureInfo.InvariantCulture);
                    var lon = Convert.ToDouble(latlon[1].Trim(), CultureInfo.InvariantCulture);

                    lblPosicion.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(lat, lon);
                }
                catch
                {
                    lblPosicion.Text = "No se encontró posición.";
                }
                LoadStep = 1;
            }
            else if(LoadStep == 1)
            {
                // Paso 1: Ultimo Login
                var lastFrid = DAOFactory.LogUltimoLoginDAO.GetLastVehicleRfidEvent(pageCoche);
                if (lastFrid != null)
                {
                    var chofer = lastFrid.Chofer ?? CultureManager.GetLabel("REVISAR_TARJETA");
                    lblLastRfid.Text = string.Format("{0} ({1})", chofer, lastFrid.Fecha.Value.ToDisplayDateTime());
                }
                divUltimoLogin.Visible = lastFrid != null;
                LoadStep = 2;
            }
            else if (LoadStep == 2)
            {
                // Paso 2: Solapa Ticket
                LoadTicket(pageCoche);
                LoadStep = 3;
            }
            else if (LoadStep == 3)
            {
                // Paso 3: Trabajando para... Clientes
                lblClientes.Text = pageCoche.Clientes.Cast<Cliente>().Aggregate(String.Empty, (current, c) => String.Concat(current, c.DescripcionCorta));
                divClientes.Visible = !string.IsNullOrEmpty(lblClientes.Text);
                LoadStep = 4;
            }
            else if (LoadStep == 4)
            {
                // Paso 4: Vehiculos Cercanos
                var pos = SharedPositions.GetLastPositions(new List<Coche> { pageCoche }).FirstOrDefault();
                if (pos != null) VehiculosCercanos(pageCoche, pos);
                LoadStep = 5;
            }
            else if (LoadStep == 5)
            {
                // Paso 5: Solapa Rutaa
                LoadRuta(pageCoche);
                timerGeocoder.Enabled = false;
            }
        }
    
        protected void BtInfoClick(object sender, EventArgs e)
        {
            btInfo.Enabled = false;
            btEstado.Enabled = true;
            btRuta.Enabled = true;
            MultiView1.ActiveViewIndex = 0;
        }
    
        protected void BtEstadoClick(object sender, EventArgs e)
        {
            btEstado.Enabled = false;
            btInfo.Enabled = true;
            btRuta.Enabled = true;
            MultiView1.ActiveViewIndex = 1;
            var dispositivo = DAOFactory.DispositivoDAO.FindById(IdDispositivo);
            var ticket = DAOFactory.TicketDAO.FindEnCurso(dispositivo);
            grid.Visible = ticket != null;
            if (ticket == null) return;

            lblTicket.Text = ticket.Codigo;
            lblCliente.Text = ticket.Cliente != null ? ticket.Cliente.Descripcion : CultureManager.GetLabel("NINGUNO");
            lblFecha.Text = ticket.FechaTicket.Value.ToDisplayDateTime().ToString("dddd dd \"de\" MMMM \"de\" yyyy");
            lblDetalles.Text = string.Empty;
            if (!string.IsNullOrEmpty(ticket.CodigoProducto) || !string.IsNullOrEmpty(ticket.DescripcionProducto))
                lblDetalles.Text += @"<div class='infoMovil_label'>" + CultureManager.GetLabel("PRODUCTO") + @"<span class='infoMovil_info'>" + ticket.CodigoProducto + @" - " + ticket.DescripcionProducto + @"</span></div>";

            var hasCantidad = !string.IsNullOrEmpty(ticket.CantidadPedido) || !string.IsNullOrEmpty(ticket.CantidadCarga) ||
                              !string.IsNullOrEmpty(ticket.CumulativeQty);

            if (hasCantidad) lblDetalles.Text += @"<div class='infoMovil_label'>";

            if (!string.IsNullOrEmpty(ticket.CantidadPedido))
                lblDetalles.Text += CultureManager.GetLabel("CANTIDAD_PEDIDO") + @": <span class='infoMovil_info'>" + ticket.CantidadPedido + @" " + ticket.Unidad + @"</span>";
            if (!string.IsNullOrEmpty(ticket.CantidadCarga))
                lblDetalles.Text += @" | " + CultureManager.GetLabel("CANTIDAD_CARGA") + @": <span class='infoMovil_info'>" + ticket.CantidadCarga + @" " + ticket.Unidad + @"</span>";
            if (!string.IsNullOrEmpty(ticket.CumulativeQty))
                lblDetalles.Text += @" | " + CultureManager.GetLabel("CANTIDAD_ACUMULADA") + @": <span class='infoMovil_info'>" + ticket.CumulativeQty + @" " + ticket.Unidad + @"</span>";

            if (hasCantidad) lblDetalles.Text += @"</div>";

            if (!string.IsNullOrEmpty(ticket.UserField1))
                lblDetalles.Text += @"<div class='infoMovil_label'>" + CultureManager.GetLabel("COMMENT_1") + @": <span class='infoMovil_info'>" + ticket.UserField1 + @"</span></div>";
            if (!string.IsNullOrEmpty(ticket.UserField2))
                lblDetalles.Text += @"<div class='infoMovil_label'>" + CultureManager.GetLabel("COMMENT_2") + @": <span class='infoMovil_info'>" + ticket.UserField2 + @"</span></div>";
            if (!string.IsNullOrEmpty(ticket.UserField3))
                lblDetalles.Text += @"<div class='infoMovil_label'>" + CultureManager.GetLabel("COMMENT_3") + @": <span class='infoMovil_info'>" + ticket.UserField3 + @"</span></div>";

            grid.DataSource = ticket.Detalles;
            grid.DataBind();
        }

        protected void BtRutaClick(object sender, EventArgs e)
        {
            btRuta.Enabled = false;
            btEstado.Enabled = true;
            btInfo.Enabled = true;
            MultiView1.ActiveViewIndex = 2;

            var viaje = DAOFactory.ViajeDistribucionDAO.FindEnCurso(pageCoche);
            if (viaje == null) return;

            lblViaje.Text = viaje.Codigo;
            lblAlta.Text = viaje.Alta.HasValue ? viaje.Alta.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : " - ";
            lblInicio.Text = viaje.InicioReal.HasValue ? viaje.InicioReal.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : " - ";
            var entregas = viaje.Detalles.Where(d => d.PuntoEntrega != null).ToList();
            var total = entregas.Count();
            var realizadas = entregas.Count(ent => ent.Estado == EntregaDistribucion.Estados.Completado
                                                || ent.Estado == EntregaDistribucion.Estados.Visitado);
            var porc = total > 0 ? (double)realizadas / (double)total * 100.0 : 0.00;
            lblEntregas.Text = total.ToString("#0");
            lblRealizadas.Text = realizadas.ToString("#0");
            lblPorc.Text = porc.ToString("#0.00");
            lnkMonitorCiclo.Attributes.Add("id", viaje.Id.ToString("#0"));

            gridRuta.DataSource = viaje.GetEntregasPorOrdenReal();
            gridRuta.DataBind();
        }

        protected void LnkMonitorCicloOnClick(object sender, EventArgs e)
        {
            var lnk = sender as ResourceLinkButton;
            if (lnk == null) return;
            int id;
            if (int.TryParse(lnk.Attributes["id"], out id))
                OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlDistribucion(id)), "_blank");
        }

        protected void GridRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;
            var estado = e.Row.DataItem as DetalleTicket;
            if (estado == null) return;

            var hasProg = estado.Programado.HasValue;
            var hasAuto = estado.Automatico.HasValue;
            var hasManu = estado.Manual.HasValue;

            var programado = hasProg ? estado.Programado.Value.ToDisplayDateTime() : DateTime.MinValue;
            var manual = hasManu ? estado.Manual.Value.ToDisplayDateTime() : DateTime.MinValue;
            var automatico = hasAuto ? estado.Automatico.Value.ToDisplayDateTime() : DateTime.MinValue;

            var difm = hasProg && hasManu ? "(" + (int)programado.Subtract(manual).TotalMinutes + ")" : string.Empty;
            var difa = hasProg && hasAuto ? "(" + (int)programado.Subtract(automatico).TotalMinutes + ")" : string.Empty;

            e.Row.Cells[0].Text = estado.EstadoLogistico.Descripcion;
            e.Row.Cells[1].Text = hasProg ? programado.ToString("HH:mm") : string.Empty;
            e.Row.Cells[2].Text = hasManu ? manual.ToString("HH:mm") + difm : string.Empty;
            e.Row.Cells[3].Text = hasAuto ? automatico.ToString("HH:mm") + difa : string.Empty;
        }

        protected void GridRutaRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;
            var entrega = e.Row.DataItem as EntregaDistribucion;
            if (entrega == null) return;

            var entrada = entrega.Entrada.HasValue ? entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : " - ";
            var manual = entrega.Manual.HasValue ? entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : " - ";
            var salida = entrega.Salida.HasValue ? entrega.Salida.Value.ToDisplayDateTime().ToString("HH:mm") : "  - ";
            
            e.Row.Cells[0].Text = entrega.Cliente != null ? entrega.Cliente.Descripcion : " - ";
            e.Row.Cells[1].Text = entrega.PuntoEntrega != null ? entrega.PuntoEntrega.Descripcion : " - ";
            e.Row.Cells[2].Text = entrega.Descripcion;
            e.Row.Cells[3].Text = entrada;
            e.Row.Cells[4].Text = manual;
            e.Row.Cells[5].Text = salida;
            e.Row.Cells[6].Text = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado));

            switch (entrega.Estado)
            {
                case EntregaDistribucion.Estados.Completado: e.Row.BackColor = Color.GreenYellow; break;
                case EntregaDistribucion.Estados.NoCompletado: e.Row.BackColor = Color.Red; break;
                case EntregaDistribucion.Estados.Visitado: e.Row.BackColor = Color.Yellow; break;
                case EntregaDistribucion.Estados.EnSitio: e.Row.BackColor = Color.CornflowerBlue; break;
                case EntregaDistribucion.Estados.Pendiente: e.Row.BackColor = Color.Orange; break;
            }
        }

        protected void LoadTicket(Coche coche)
        {
            var ticket = pageCoche.Dispositivo != null ? DAOFactory.TicketDAO.FindEnCurso(pageCoche.Dispositivo) : null;

            panelTab.Visible = ticket != null;
            btEstado.Visible = true;
            btRuta.Visible = false;

            if (ticket != null && ticket.Empleado != null)
            {
                lblChofer.Text = ticket.Empleado.Entidad.Descripcion;
            }
        }

        protected void LoadRuta(Coche coche)
        {
            var distribucion = pageCoche.Dispositivo != null ? DAOFactory.ViajeDistribucionDAO.FindEnCurso(coche) : null;

            panelTab.Visible = distribucion != null;
            btEstado.Visible = false;
            btRuta.Visible = true;
        }

        protected void VehiculosCercanos(Coche vehiculo, LogUltimaPosicionVo posicion)
        {
            var vehiculos = DAOFactory.CocheDAO.GetList(IdsEmpresas, IdsLineas)
                                               .Where(v => v.Dispositivo != null)
                                               .ToList();
            var positions = SharedPositions.GetLastPositions(vehiculos);

            var distancias = from pos in positions
                             where pos.IdCoche != vehiculo.Id
                             select new VehiculoDistancia
                                        {Distancia = Distancias.Loxodromica(posicion.Latitud, posicion.Longitud, pos.Latitud, pos.Longitud),
                                         Id = pos.IdCoche, Interno = pos.Coche};
            var cercanas = (from pos in distancias orderby pos.Distancia select pos).Take(5);

            gridMov.DataSource = cercanas;
            gridMov.DataBind();
        }

        #region Nested T: VehiculoDistancia

        protected class VehiculoDistancia
        {
            public int Id { get; set;}
            public string Interno { get; set; }
            public double Distancia { get; set; }
            public string DistanciaKm { get { return FormatKm(Distancia); } }
            private static string FormatKm(double metros)
            {
                if (metros < 1000) return metros.ToString("0.00m");
                return (metros / 1000).ToString("0.00km");
            }
        }

        #endregion
    }
}
