using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Process.Geofences;
using Logictracker.Process.Geofences.Classes;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Operacion.Mimico
{
    public partial class Default : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "OPE_MIMICO"; }
        protected override InfoLabel LblInfo { get { return null; } }

        protected void CbVehiculoSelectedIndexChanged(object sender, EventArgs e)
        {
            ShowInfo();
        }

        protected override void OnInit(EventArgs e)
        {
            if (WebSecurity.ShowDriver) cbVehiculo.ShowDriverName = true;

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RegisterExtJsStyleSheet();

            ShowInfo();

            Status("Iniciando...", 1000);
        }

        protected void TimerTick(object sender, EventArgs e)
        {
            ShowInfo();
            Status("Actualizando...", 1000);
        }

        protected void ChkReload(object sender, EventArgs e)
        {
            ShowInfo();
            Status("Actualizando...", 1000);
        }
        
        public void ShowTotales()
        {

        }

        public void ShowInfo()
        {
            ClearMimico();
            var coches = Enumerable.Where<int>(cbVehiculo.SelectedValues, id=>id > 0).Select(id => DAOFactory.CocheDAO.FindById(id)).Where(c=>c.Dispositivo != null);
            var totalCoches = coches.Count();
            var conServicio = 0;
            var sinServicio = 0;
            var enMantenimiento = 0;

            var enHora = 0;
            var adelantados = 0;
            var demorados = 0;

            var enBase = 0;
            var enCliente = 0;
            
            var ciclos = new List<Ciclo>();
            foreach(var coche in coches)
            {
                Ciclo ciclo = null;
                var idBase = 0;
                var idCoche = coche.Id;
                var idClientes = new List<int>();

                var ticket = DAOFactory.TicketDAO.FindEnCurso(coche.Dispositivo);
                if (ticket != null)
                {
                    ciclo = new Ciclo(ticket, DAOFactory);
                    if (ticket.Linea != null && ticket.Linea.ReferenciaGeografica != null)
                        idBase = ticket.Linea.ReferenciaGeografica.Id;
                    idClientes.Add(ticket.PuntoEntrega.ReferenciaGeografica.Id);
                }

                if (ciclo == null)
                {
                    var distribucion = DAOFactory.ViajeDistribucionDAO.FindEnCurso(coche);
                    if (distribucion != null)
                    {
                        ciclo = new Ciclo(distribucion, DAOFactory);
                        if (distribucion.Linea != null && distribucion.Linea.ReferenciaGeografica != null)
                            idBase = distribucion.Linea.ReferenciaGeografica.Id;
                        idClientes = distribucion.Detalles.Select(d => d.ReferenciaGeografica.Id).ToList();
                    }
                }

                if (ciclo != null)
                {
                    conServicio++;
                    var dif = ciclo.Diferencia;

                    if (Math.Abs(dif) <= 5)
                    {
                        enHora++;
                        ciclo.Demorado = false;
                        ciclo.Adelantado = false;
                    }
                    else if (dif > 0)
                    {
                        demorados++;
                        ciclo.Demorado = true;
                        ciclo.Adelantado = false;
                    }
                    else
                    {
                        adelantados++;
                        ciclo.Demorado = false;
                        ciclo.Adelantado = true;
                    }

                    ciclos.Add(ciclo);
                }
                else
                {
                    if (coche.Estado == Coche.Estados.EnMantenimiento) enMantenimiento++;
                    else sinServicio++;
                }
                               
                if (idBase > 0)
                {
                    var state = GeocercaManager.GetEstadoGeocerca(coche, idBase, DAOFactory);
                    if (state != null && state.Estado == EstadosGeocerca.Dentro)
                    {
                        enBase++;
                        if (ciclo != null)
                        {
                            ciclo.EnPlanta = true;
                            ciclo.EnObra = false;
                        }
                    }
                }
                
                if (idClientes.Count > 0)
                {
                    var withGeo = idClientes.Select(c => GeocercaManager.GetEstadoGeocerca(coche, c, DAOFactory))
                                            .Where(s => s != null);

                    if (withGeo.Any(st => st != null && st.Estado == EstadosGeocerca.Dentro))
                    {
                        enCliente++;
                        if (ciclo != null)
                        {
                            ciclo.EnObra = true;
                            ciclo.EnPlanta = false;
                        }
                    }
                }
            }
            var sh = new ScriptHelper(this);
            foreach (var ciclo in ciclos.OrderByDescending(c => c.Completed))
            {
                if (!chkEnCliente.Checked && ciclo.EnObra) continue;
                if (!chkEnPlanta.Checked && ciclo.EnPlanta) continue;
                if (!chkEnViaje.Checked && (!ciclo.EnObra && !ciclo.EnPlanta)) continue;
                if (!chkDemorados.Checked && ciclo.Demorado) continue;
                if (!chkAdelantados.Checked && ciclo.Adelantado) continue;
                if (!chkEnHora.Checked && (!ciclo.Demorado && !ciclo.Adelantado)) continue;
                
                sh.RegisterStartupScript(string.Format("init_{0}_{1}", ciclo.Tipo, ciclo.Id), ciclo.Render(), true);    
            }

            lblTotal.Text = totalCoches.ToString();

            lblEnServicio.Text = conServicio.ToString();
            lblSinServicio.Text = sinServicio.ToString();
            lblEnMantenimiento.Text = enMantenimiento.ToString();

            lblEnHora.Text = enHora.ToString();
            lblAdelantados.Text = adelantados.ToString();
            lblDemorados.Text = demorados.ToString();

            lblEnPlanta.Text = enBase.ToString();
            lblEnCliente.Text = enCliente.ToString();
            lblEnViaje.Text = (conServicio - enBase - enCliente).ToString();
        }

        private void ClearMimico()
        {
            var sh = new ScriptHelper(this);
            sh.RegisterStartupScript("clear", "clearMimicos();", true);
        }
        
        private void Status(string message, int time)
        {
            var sh = new ScriptHelper(this);
            sh.RegisterStartupScript("show_status",string.Format("status('{0}', {1});", message, time), true);
        }
        
        private class Ciclo
        {
            private const string Template = "{{ 'id': '{0}' , 'type': '{5}', 'interno': '{1}', 'style': '{2}', 'icon': '{3}', completed: {4} }}";
            public int Id { get; set; }
            public string Tipo { get; set;}
            public string Icono { get; set; }
            public string Interno { get; set; }
            public string LabelStyle { get; set; }
            public int Completed { get; set; }
            public List<Detalle> Detalles { get; set; }
            public bool EnPlanta { get; set; }
            public bool EnObra { get; set; }
            public bool Demorado { get; set; }
            public bool Adelantado { get; set; }
            
            public Ciclo(Ticket ticket, DAOFactory daoFactory)
            {
                Id = ticket.Id;
                Tipo = "Ticket";
                Icono = Path.Combine(IconDir, ticket.Vehiculo.TipoCoche.IconoDefault.PathIcono);
                Interno = ticket.Vehiculo.Interno;
                LabelStyle = GetLabelStyle(ticket.Vehiculo, daoFactory);
                Detalles = ticket.Detalles.Cast<DetalleTicket>().Select(d => new Detalle(d)).ToList();
                Completed = GetCompleted();
            }
            public Ciclo(ViajeDistribucion distribucion, DAOFactory daoFactory)
            {
                Id = distribucion.Id;
                Tipo = "Distribucion";
                Icono = Path.Combine(IconDir, distribucion.Vehiculo.TipoCoche.IconoDefault.PathIcono);
                Interno = distribucion.Vehiculo.Interno;
                LabelStyle = GetLabelStyle(distribucion.Vehiculo, daoFactory);
                Detalles = distribucion.Detalles.Select((d, i) => new Detalle(d) {Descripcion = (i + 1).ToString()}).ToList();
                RecalculateTimes(distribucion);
                Completed = GetCompleted(distribucion);
            }

            private void RecalculateTimes(ViajeDistribucion distribucion)
            {
                var firstDetalle = Detalles.First();
                var lastDetalle = Detalles.Last();

                var totalTime = lastDetalle.Programado.Subtract(firstDetalle.Programado).TotalMinutes;

                if (totalTime == 0)
                {
                    lastDetalle = null;
                    foreach (var detalle in Detalles)
                    {
                        if (lastDetalle != null)
                        {
                            var distancia = Distancias.Loxodromica(lastDetalle.Latitud, lastDetalle.Longitud, detalle.Latitud, detalle.Longitud)/1000;
                            var velocidad = distribucion.Vehiculo.VelocidadPromedio > 0 ? distribucion.Vehiculo.VelocidadPromedio: distribucion.Vehiculo.TipoCoche.VelocidadPromedio > 0?distribucion.Vehiculo.TipoCoche.VelocidadPromedio:40;
                            var tiempo = TimeSpan.FromHours(distancia/velocidad);
                            detalle.Programado = lastDetalle.Programado.Add(tiempo) + TimeSpan.FromMinutes(10);
                        }
                        lastDetalle = detalle;
                    }
                }
            }

            private int GetCompleted(ViajeDistribucion distribucion)
            {
                if (distribucion.Tipo == ViajeDistribucion.Tipos.Desordenado)
                {
                    var detalles = distribucion.Detalles.Where(d => d.Linea == null);
                    var cantDetalles = detalles.Count();
                    var entregados = detalles.Count(detalle => detalle.Entrada.HasValue || detalle.Manual.HasValue || detalle.Salida.HasValue);

                    return Convert.ToInt32(entregados * 100 / cantDetalles);
                }
                
                return GetCompleted();
            }

            private int GetCompleted()
            {
                var firstDetalle = Detalles.First();
                var lastDetalle = Detalles.Last();
                if (lastDetalle.Automatico.HasValue) return 100;

                var totalTime = lastDetalle.Programado.Subtract(firstDetalle.Programado).TotalMinutes;

                lastDetalle = null;

                foreach (var detalle in Detalles)
                {
                    if (lastDetalle != null)
                    {
                        var duracion = detalle.Programado.Subtract(lastDetalle.Programado).TotalMinutes;
                        var parcial = !detalle.Automatico.HasValue && lastDetalle.Automatico.HasValue
                                          ? (DateTime.UtcNow.Subtract(lastDetalle.Automatico.Value).TotalMinutes * 100) / duracion
                                          : 0;
                        if (parcial > 100) parcial = 100;
                        detalle.Duracion = Convert.ToInt32(duracion);
                        detalle.PorcentajeDelTotal = totalTime == 0 ? 0 : Convert.ToInt32((duracion / totalTime) * 100);
                        detalle.Completado = detalle.Automatico.HasValue;
                        detalle.PorcentajeParcial = detalle.Automatico.HasValue
                                                        ? 100
                                                        : Math.Min(90, Convert.ToInt32(parcial));
                    }
                    lastDetalle = detalle;
                }

                return Convert.ToInt32(Detalles.Select(d => (d.PorcentajeParcial / 100) * d.PorcentajeDelTotal).Sum());
            }

            private string GetLabelStyle(Coche coche, DAOFactory daoFactory)
            {
                var upm = SharedPositions.GetLastPositions(new List<Coche> { coche }).FirstOrDefault();
                if (upm == null) return "ol_marker_labeled_red";

                string style;

                if (upm.FechaMensaje >= DateTime.UtcNow.AddMinutes(-5)) style = upm.Velocidad == 0 ? "ol_marker_labeled" : "ol_marker_labeled_green";
                else style = upm.FechaMensaje >= DateTime.UtcNow.AddHours(-48) ? "ol_marker_labeled_yellow" : "ol_marker_labeled_red";

                return style;
            }

            public string Render()
            {
                var m = string.Format(Template, Id, Interno, LabelStyle, Icono, Completed, Tipo);
                var st = string.Concat("[", string.Join(",", Detalles.Select(d=>d.Render()).ToArray()), "]");

                return string.Format("addMimico('{0}', {1}, {2});", Id, m, st);
            }

            public int Diferencia
            {
                get
                {
                    var last = Detalles.LastOrDefault(d => d.Automatico.HasValue);
                    return last != null ? Convert.ToInt32(last.Automatico.Value.Subtract(last.Programado).TotalMinutes):0;
                }
            }
        }

        private class Detalle
        {
            public int Id { get; set; }
            public string Descripcion { get; set; }
            public string PopupText { get; set; }
            public int Duracion { get; set; }
            public int PorcentajeDelTotal { get; set;}
            public int PorcentajeParcial { get; set; }
            public bool Completado { get; set; }
            public DateTime Programado { get; set; }
            public DateTime? Automatico { get; set; }
            public double Latitud { get; set;}
            public double Longitud { get; set; }

            public Detalle(DetalleTicket detalle)
            {
                Id = detalle.Id;
                Descripcion = detalle.EstadoLogistico.Descripcion;
                PopupText = string.Format("<div>({2}) <span>{3}</span></div><div>Cliente: <span>{4}</span></div><div>Punto de Entrega: <span>{5}</span></div><div>Telefono: <span>{7}</span></div><div>Programado: <span>{0}</span><br/>Real: <span>{1}</span></div><div>Chofer: <span>{6}</span></div>",
                                  detalle.Programado.Value.ToDisplayDateTime().ToString("HH:mm"),
                                  (detalle.Automatico.HasValue ? detalle.Automatico.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  detalle.Ticket.OrdenDiario,
                                  detalle.Ticket.Codigo,
                                  detalle.Ticket.Cliente.Descripcion,
                                  detalle.Ticket.PuntoEntrega.Descripcion,
                                  detalle.Ticket.Empleado != null ? detalle.Ticket.Empleado.Entidad.Descripcion : string.Empty,
                                  detalle.Ticket.PuntoEntrega.Telefono);
                Programado = detalle.Programado.Value;
                Automatico = detalle.Automatico;
            }

            public Detalle(EntregaDistribucion entrega)
            {
                Id = entrega.Id;
                Descripcion = entrega.Orden.ToString();
                PopupText = string.Format("<div>Punto de Entrega: <span>{5}</span></div><div>Telefono: <span>{6}</span></div><div>Cliente: <span>{4}</span></div><div>Programado: <span>{0}</span><br/>Entrada: <span>{1}</span><br/>Salida: <span>{2}</span><br/>Manual: <span>{3}</span></div>",
                                  entrega.Programado.ToDisplayDateTime().ToString("HH:mm"),
                                  (entrega.Entrada.HasValue ? entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  (entrega.Salida.HasValue ? entrega.Salida.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  (entrega.Manual.HasValue ? entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  entrega.Cliente != null ? entrega.Cliente.Descripcion: string.Empty,
                                  entrega.PuntoEntrega != null ? entrega.PuntoEntrega.Descripcion : entrega.Linea.Descripcion,
                                  entrega.PuntoEntrega != null ? entrega.PuntoEntrega.Telefono : string.Empty);

                Programado = entrega.Programado;
                Automatico = entrega.Entrada;
                Latitud = entrega.ReferenciaGeografica.Latitude;
                Longitud = entrega.ReferenciaGeografica.Longitude;
            }

            public string Render()
            {
                return string.Format("{{ 'id': '{0}', 'name': '{1}', 'programmed': {2}, 'real': {3}, 'details': '{4}' }}",
                                Id,
                                Descripcion,
                                GetJsDate(Programado),
                                GetJsDate(Automatico),
                                PopupInfo().Replace("'", "\\'"));
            }
            private string PopupInfo()
            {
                return string.Format("<div class='popup_title'>{0}</div><div class='popup_detail'>{1}</div>", Descripcion, PopupText);
            }
            private string GetJsDate(DateTime? date)
            {
                return date.HasValue ? string.Concat("new Date", date.Value.ToDisplayDateTime().ToString("(yyyy,M,d,H,m)")) : "null";
            }
        }       
    }
}