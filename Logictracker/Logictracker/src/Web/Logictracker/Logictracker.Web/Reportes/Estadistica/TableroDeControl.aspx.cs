using C1.Web.UI.Controls.C1Gauge;
using Logictracker.Messaging;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.Geofences;
using Logictracker.Process.Geofences.Classes;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Logictracker.Reportes.Estadistica
{
    public partial class TableroDeControl : ApplicationSecuredPage
    {   
        protected override string GetRefference() { return "EST_TABLERO_CONTROL"; }
        protected override InfoLabel LblInfo { get { return null; } }

        #region Private Properties

        private readonly int _codigoEntrada = Convert.ToInt32(MessageCode.InsideGeoRefference.GetMessageCode());
        private readonly int _codigoSalida = Convert.ToInt32(MessageCode.OutsideGeoRefference.GetMessageCode());
        private readonly DateTime _desdeHoy = DateTime.Today.ToUniversalTime();
        private readonly DateTime _hastaHoy = DateTime.UtcNow;
        private readonly DateTime _desdeAyer = DateTime.Today.ToUniversalTime().AddDays(-1);
        private readonly DateTime _hastaAyer = DateTime.Today.ToUniversalTime().AddSeconds(-1);
        private readonly DateTime _desdeMes = DateTime.Today.ToUniversalTime().AddDays(-DateTime.Today.ToUniversalTime().Day).AddDays(1);
        private readonly DateTime _hastaMes = DateTime.Today.ToUniversalTime().AddDays(-DateTime.Today.ToUniversalTime().Day).AddDays(1).AddMonths(1).AddSeconds(-1);
        private double _totalKilometers;
        private double _totalMovementTime;
        private double _totalStoppedTime;
        private double _detenidoMotorOn;
        private double _detenidoMotorOff;
        private double _maxSpeed;
        private int _stopsHigher1;
        private int _stopsHigher15;
        private List<int> _mobiles = new List<int>();
        private double _tiempoGeoCerca;
        private bool _tieneInfraccion;
        private int _infracciones;
        private int _vehiculosConInfraccion;
        private int _movilesEnMovimiento;
        private int _movilesDetenidos;
        private int _movilesEnBase;
        private int _movilesEnGeocerca;
        private int _movilesEnBaseMas1Hora;
        private int _movilesEnGeocercaMas1Hora;
        private int _movilesActivos;
        private int _movilesInactivosBase;
        
        private int _movilesEnPlanta;
        private int _movilesEnCliente;
        private int _movilesEnViaje;

        private int _movilesEnHoraHoy;
        private int _movilesDemoradosHoy;
        private int _movilesAdelantadosHoy;
        private int _movilesEnHoraAyer;
        private int _movilesDemoradosAyer;
        private int _movilesAdelantadosAyer;
        private int _movilesEnHoraMes;
        private int _movilesDemoradosMes;
        private int _movilesAdelantadosMes;
                
        private Dictionary<int, int> _detalleBases = new Dictionary<int,int>();
        
        #endregion

        protected void BtnActualizar_OnClick(object sender, EventArgs e) { }
        protected void Timer_OnTick(object sender, EventArgs e) { }
        protected void LnkInfracciones_OnClick(object sender, EventArgs e) 
        { 
            Response.Redirect("~/Reportes/Accidentologia/infractionsDetails.aspx");
        }
        protected void LnkTickets_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Reportes/CicloLogistico/DuracionEstadosTicket.aspx");
        }
        protected void LnkCercas_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Reportes/DatosOperativos/GeocercasEvents.aspx");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SetExpiration();

            var lineas = DAOFactory.LineaDAO.GetList(new[] { ddlEmpresa.Selected });
            var idPoiBase = lineas.Where(l => l.ReferenciaGeografica != null).Select(l => l.ReferenciaGeografica.Id);
            
            foreach (var idBase in idPoiBase)
            {
                if (!_detalleBases.ContainsKey(idBase))
                    _detalleBases.Add(idBase, 0);
            }

            var coches = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected });
            
            foreach (var coche in coches)
            {
                _mobiles.Add(coche.Id);
                CalcularEstadisticasHoy(coche, idPoiBase);
                CalcularEstadisticasAyer(coche);
                CalcularEstadisticasMes(coche);
            }

            var empresa = DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected);
            var maxMonths = empresa != null && empresa.Id > 0 ? empresa.MesesConsultaPosiciones : 3;
            
            var entradasGeocerca = ReportFactory.MobileEventDAO.GetMobilesEvents(_mobiles, new List<int> { _codigoEntrada }, new List<int> { 0 }, _desdeAyer, _hastaAyer, maxMonths);
            var salidasGeocerca = ReportFactory.MobileEventDAO.GetMobilesEvents(_mobiles, new List<int> { _codigoSalida }, new List<int> { 0 }, _desdeAyer, _hastaAyer, maxMonths);
            var entradasBase = entradasGeocerca.Where(g => g.IdPuntoInteres.HasValue && idPoiBase.Contains(g.IdPuntoInteres.Value)).ToList();

            CalcularTiempoEnGeoCerca(entradasGeocerca, salidasGeocerca);

            var tickets = DAOFactory.TicketDAO.GetList(new[] { ddlEmpresa.Selected }, 
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       new[] { -1 }, 
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       coches.Select(t => t.Id), 
                                                       new int[] { Ticket.Estados.Cerrado },
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       new[] { -1 },
                                                       _desdeAyer, 
                                                       _hastaAyer).Count();
            var averageSpeed = _totalKilometers > 0 && _totalMovementTime > 0 ? _totalKilometers / _totalMovementTime : 0;
            var averageTimeGeofence = _tiempoGeoCerca > 0 && entradasGeocerca.Count > 0 ? _tiempoGeoCerca / entradasGeocerca.Count : 0;

            LoadLabelsHoy();
            LoadGaugesHoy();
            
            LoadLabelsAyer(coches.Count, entradasBase.Count, entradasGeocerca.Count, averageSpeed, averageTimeGeofence, tickets);
            LoadGaugesAyer(coches.Count, entradasBase.Count, entradasGeocerca.Count, averageSpeed, tickets);

            LoadLabelsMes();
            LoadGaugesMes();
        }

        private void CalcularTiempoEnGeoCerca(List<MobileEvent> entradasGeocerca, List<MobileEvent> salidasGeocerca)
        {
            foreach (var entrada in entradasGeocerca)
            {
                for (var i = 0; i < salidasGeocerca.Count; i++)
                {
                    if (entrada.IdPuntoInteres == salidasGeocerca[i].IdPuntoInteres &&
                        entrada.Intern == salidasGeocerca[i].Intern &&
                        entrada.EventTime < salidasGeocerca[i].EventTime)
                    {
                        _tiempoGeoCerca += (salidasGeocerca[i].EventTime - entrada.EventTime).TotalMinutes;
                        break;
                    }
                }
            }
        }

        private void SetExpiration()
        {
            gaugeActivos.AbsoluteExpiration = gaugeAverageSpeed.AbsoluteExpiration = gaugeDet1.AbsoluteExpiration =
            gaugeEntradas.AbsoluteExpiration = gaugeInfracciones.AbsoluteExpiration = gaugeKm.AbsoluteExpiration =
            gaugeMaxSpeed.AbsoluteExpiration = gaugeMovDet.AbsoluteExpiration = gaugeBaseGeocerca.AbsoluteExpiration =
            gaugeMas1Hora.AbsoluteExpiration = gaugeTickets.AbsoluteExpiration = gaugeInactivosBase.AbsoluteExpiration = DateTime.Today.AddDays(-1);

            gaugeActivos.SlidingExpiration = gaugeAverageSpeed.SlidingExpiration = gaugeDet1.SlidingExpiration =
            gaugeEntradas.SlidingExpiration = gaugeInfracciones.SlidingExpiration = gaugeKm.SlidingExpiration =
            gaugeMaxSpeed.SlidingExpiration = gaugeMovDet.SlidingExpiration = gaugeBaseGeocerca.SlidingExpiration =
            gaugeMas1Hora.SlidingExpiration = gaugeTickets.SlidingExpiration = gaugeInactivosBase.SlidingExpiration = new TimeSpan(0, 0, 1);
        }

        private void CalcularEstadisticasHoy(Coche coche, IEnumerable<int> idPoiBase)
        {
            if ((coche.Dispositivo != null && DAOFactory.TicketDAO.FindEnCurso(coche.Dispositivo) != null) || DAOFactory.ViajeDistribucionDAO.FindEnCurso(coche) != null)
                _movilesActivos++;

            var ultimaPos = coche.RetrieveLastPosition();

            if (ultimaPos != null && ultimaPos.Velocidad > 0)
            {
                _movilesEnMovimiento++;
            }
            else
            {
                _movilesDetenidos++;

                //var cercas = GeofenceManager.GetGeocercas(coche);
                var cercasDentro = GeocercaManager.GetEstadoVehiculo(coche, DAOFactory);
                //var cercasDentro = GeofenceManager.GetGeocercasInside(coche);

                var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;

                var entradaEnCerca = ReportFactory.MobileEventDAO.GetMobilesEvents(new List<int> { coche.Id }, new List<int> { _codigoEntrada }, new List<int> { 0 }, _desdeHoy, _hastaHoy, maxMonths);

                foreach (var cerca in cercasDentro.GeocercasDentro)
                {
                    if (idPoiBase.Contains(cerca.Geocerca.Id))
                    {
                        _movilesEnBase++;
                        _detalleBases[cerca.Geocerca.Id] += 1;

                        var ciclo = CicloLogisticoFactory.GetCiclo(coche, null);
                        if(ciclo != null) _movilesInactivosBase++;
                        if (entradaEnCerca.Count > 0 && entradaEnCerca[entradaEnCerca.Count - 1].EventTime < DateTime.UtcNow.AddHours(-1))
                            _movilesEnBaseMas1Hora++;
                    }
                    else
                    {
                        _movilesEnGeocerca++;
                        if (entradaEnCerca.Count > 0 && entradaEnCerca[entradaEnCerca.Count - 1].EventTime < DateTime.UtcNow.AddHours(-1))
                            _movilesEnGeocercaMas1Hora++;
                    }
                }
            }

            ///// TICKETS /////

            var enServicio = 0;
            var tickets = DAOFactory.TicketDAO.FindByCocheYFecha(new[] { coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1 },
                                                                 new[] { coche.Linea != null ? coche.Linea.Id : -1 },
                                                                 new[] { coche.Id },
                                                                 _desdeHoy,
                                                                 _hastaHoy);

            foreach (var ticket in tickets)
            {
                var diferencia = 0;
                var ultimoEstado = ticket.Detalles.Cast<DetalleTicket>()
                                                  .Where(d => d.Automatico.HasValue)
                                                  .LastOrDefault();

                if (ultimoEstado != null)
                    diferencia = Convert.ToInt32(ultimoEstado.Automatico.Value.Subtract(ultimoEstado.Programado.Value).TotalMinutes);

                if (Math.Abs(diferencia) <= 5)
                    _movilesEnHoraHoy++;
                else
                    if (diferencia > 0)
                        _movilesDemoradosHoy++;
                    else
                        _movilesAdelantadosHoy++;

                if (ticket.Estado == Ticket.Estados.EnCurso && ticket.FechaTicket < DateTime.UtcNow)
                {
                    enServicio++;
                    var idBase = 0;
                    var idClientes = new List<int>();

                    if (ticket.Linea != null && ticket.Linea.ReferenciaGeografica != null)
                        idBase = ticket.Linea.ReferenciaGeografica.Id;
                    idClientes.Add(ticket.PuntoEntrega.ReferenciaGeografica.Id);

                    if (idBase > 0)
                    {
                        var state = GeocercaManager.GetEstadoGeocerca(coche, idBase, DAOFactory);
                        if (state != null && state.Estado == EstadosGeocerca.Dentro) 
                            _movilesEnPlanta++;
                    }

                    if (idClientes.Count > 0)
                    {
                        var withGeo = idClientes.Select(c => GeocercaManager.GetEstadoGeocerca(coche, c, DAOFactory))
                                                .Where(s => s != null);

                        if (withGeo.Any(st => st != null && st.Estado == EstadosGeocerca.Dentro))
                            _movilesEnCliente++;
                    }
                }
            }

            _movilesEnViaje = enServicio - _movilesEnPlanta - _movilesEnCliente;
        }

        private void CalcularEstadisticasAyer(Coche coche)
        {
            foreach (var route in ReportFactory.MobileRoutesDAO.GetMobileRoutes(coche.Id, _desdeAyer, _hastaAyer))
            {
                _totalKilometers += route.Kilometers;
                if (route.MaxSpeed > _maxSpeed) _maxSpeed = route.MaxSpeed;

                switch (route.VehicleStatus)
                {
                    case "En Movimiento":
                        _totalMovementTime += route.Duration;
                        break;
                    case "Detenido":
                        _totalStoppedTime += route.Duration;
                        if (route.Duration > 0.25) // 1/4 de hora = 15 minutos
                        {
                            _stopsHigher1++;
                            _stopsHigher15++;
                        }
                        else
                        {
                            if (route.Duration > 0.0167)// 1/60 de hora = 1 hora
                                _stopsHigher1++;
                        }

                        switch (route.EngineStatus)
                        {
                            case "Encendido":
                                _detenidoMotorOn += route.Duration;
                                break;
                            case "Apagado":
                                _detenidoMotorOff += route.Duration;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                if (route.Infractions > 0)
                {
                    _tieneInfraccion = true;
                    _infracciones += route.Infractions;
                }
            }

            if (_tieneInfraccion)
            {
                _vehiculosConInfraccion++;
                _tieneInfraccion = false;
            }

            ///// TICKETS /////

            var tickets = DAOFactory.TicketDAO.FindByCocheYFecha(new[] { coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1 },
                                                                 new[] { coche.Linea != null ? coche.Linea.Id : -1 },
                                                                 new[] { coche.Id },
                                                                 _desdeAyer,
                                                                 _hastaAyer);

            foreach (var ticket in tickets)
            {
                var diferencia = 0;
                var ultimoEstado = ticket.Detalles.Cast<DetalleTicket>()
                                                  .Where(d => d.Automatico.HasValue)
                                                  .LastOrDefault();

                if (ultimoEstado != null)
                    diferencia = Convert.ToInt32(ultimoEstado.Automatico.Value.Subtract(ultimoEstado.Programado.Value).TotalMinutes);

                if (Math.Abs(diferencia) <= 5)
                    _movilesEnHoraAyer++;
                else
                    if (diferencia > 0)
                        _movilesDemoradosAyer++;
                    else
                        _movilesAdelantadosAyer++;
            }
        }

        private void CalcularEstadisticasMes(Coche coche)
        {
            var tickets = DAOFactory.TicketDAO.FindByCocheYFecha(new[] { coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1 },
                                                                 new[] { coche.Linea != null ? coche.Linea.Id : -1 },
                                                                 new[] { coche.Id },
                                                                 _desdeMes,
                                                                 _hastaMes);

            foreach (var ticket in tickets)
            {
                var diferencia = 0;
                var ultimoEstado = ticket.Detalles.Cast<DetalleTicket>()
                                                  .Where(d => d.Automatico.HasValue)
                                                  .LastOrDefault();
                
                if (ultimoEstado != null) 
                    diferencia = Convert.ToInt32(ultimoEstado.Automatico.Value.Subtract(ultimoEstado.Programado.Value).TotalMinutes);

                if (Math.Abs(diferencia) <= 5) 
                    _movilesEnHoraMes++;
                else 
                    if (diferencia > 0)
                        _movilesDemoradosMes++;
                    else 
                        _movilesAdelantadosMes++;
            }
        }

        private void LoadLabelsHoy()
        {
            lblActiva.Text = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected }, new short[] { 0 }).Count.ToString();
            lblInactiva.Text = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected }, new short[] { 2 }).Count.ToString();
            lblMantenimiento.Text = DAOFactory.CocheDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected }, new short[] { 1 }).Count.ToString();
            lblHora.Text = DateTime.Now.ToString("HH:mm:ss");
            lblEnMovimiento.Text = _movilesEnMovimiento.ToString();
            lblDetenidos.Text = _movilesDetenidos.ToString();
            lblEnBase.Text = _movilesEnBase.ToString();
            lblEnGeocerca.Text = _movilesEnGeocerca.ToString();
            lblEnBaseMas1Hora.Text = _movilesEnBaseMas1Hora.ToString();
            lblEnGeocercaMas1Hora.Text = _movilesEnGeocercaMas1Hora.ToString();
            lblActivosAhora.Text = _movilesActivos.ToString();
            lblInactivosBase.Text = _movilesInactivosBase.ToString();
            lblEnHoraHoy.Text = _movilesEnHoraHoy.ToString();
            lblDemoradosHoy.Text = _movilesDemoradosHoy.ToString();
            lblAdelantadosHoy.Text = _movilesAdelantadosHoy.ToString();
            lblEnPlanta.Text = _movilesEnPlanta.ToString();
            lblEnCliente.Text = _movilesEnCliente.ToString();
            lblEnViaje.Text = _movilesEnViaje.ToString();

            HtmlTableRow row;
            HtmlTableCell cell;
            Label lblDescripcion;
            Label lblCantidad;

            foreach (var key in _detalleBases.Keys)
            {
                var descripcion = DAOFactory.ReferenciaGeograficaDAO.FindById(key).Descripcion;

                lblDescripcion = new Label {Text = descripcion + ": "};
                lblDescripcion.Font.Bold = true;

                lblCantidad = new Label {Text = _detalleBases[key].ToString(), CssClass = "labelGaugeNaranja"};

                cell = new HtmlTableCell();
                cell.Controls.Add(lblDescripcion);
                cell.Controls.Add(lblCantidad);
                cell.Align = "left";

                row = new HtmlTableRow();
                row.Cells.Add(cell);

                tblLabels.Rows.Add(row);
            }
        }

        private void LoadLabelsAyer(int cochesCount, int entradasBaseCount, int entradasGeocercaCount, double averageSpeed, double averageTimeGeofence, int tickets)
        {
            lblMovimiento.Text = cochesCount > 0 ? String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(_totalMovementTime / cochesCount).Hours, TimeSpan.FromHours(_totalMovementTime / cochesCount).Minutes, TimeSpan.FromHours(_totalMovementTime / cochesCount).Seconds)) : "00:00:00";
            lblDetencion.Text = cochesCount > 0 ? String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(_totalStoppedTime / cochesCount).Hours, TimeSpan.FromHours(_totalStoppedTime / cochesCount).Minutes, TimeSpan.FromHours(_totalStoppedTime / cochesCount).Seconds)) : "00:00:00";
            lblMayor15.Text = _stopsHigher15.ToString();
            lblMayor1.Text = _stopsHigher1.ToString();
            lblInfracciones.Text = _infracciones.ToString();
            lblVelPromedio.Text = string.Format("{0:0.00}km/h", averageSpeed);
            lblVelMax.Text = string.Format("{0:0.00}km/h", _maxSpeed);
            lblInfVehiculos.Text = _vehiculosConInfraccion.ToString();
            lblKmRecorridos.Text = _totalKilometers > 0 && cochesCount > 0 ? string.Format("{0:0.00}km", _totalKilometers / cochesCount) : "0.00km";
            lblEntradasBase.Text = entradasBaseCount.ToString();
            lblEntradasGeocerca.Text = (entradasGeocercaCount - entradasBaseCount).ToString();
            lblPromedioGeocerca.Text = string.Format("{0:0}min", averageTimeGeofence);
            lblDetenidoOn.Text = string.Format("{0:0}min", _detenidoMotorOn * 60);
            lblDetenidoOff.Text = string.Format("{0:0}min", _detenidoMotorOff * 60);
            lblDetenidoOnPromedio.Text = string.Format("{0:0}min", cochesCount > 0 && _detenidoMotorOn > 0 ? _detenidoMotorOn*60/cochesCount : 0);
            lblDetenidoOffPromedio.Text = string.Format("{0:0}min", cochesCount > 0 && _detenidoMotorOff > 0 ? _detenidoMotorOff*60/cochesCount : 0);

            lblTickets.Text = tickets.ToString();
            lblEnHoraAyer.Text = _movilesEnHoraAyer.ToString();
            lblDemoradosAyer.Text = _movilesDemoradosAyer.ToString();
            lblAdelantadosAyer.Text = _movilesAdelantadosAyer.ToString();
        }

        private void LoadLabelsMes()
        {
            lblEnHoraMes.Text = _movilesEnHoraMes.ToString();
            lblDemoradosMes.Text = _movilesDemoradosMes.ToString();
            lblAdelantadosMes.Text = _movilesAdelantadosMes.ToString();
        }

        private void LoadGaugesHoy()
        {
            var maxAct = (Convert.ToInt32(_movilesActivos / 10) + 1) * 10;
            gaugeActivos.Gauges[0].Maximum = maxAct;
            gaugeActivos.Gauges[0].Value = _movilesActivos;

            var maxInacBase = (Convert.ToInt32(_movilesInactivosBase / 10) + 1) * 10;
            gaugeInactivosBase.Gauges[0].Maximum = maxInacBase;
            gaugeInactivosBase.Gauges[0].Value = _movilesInactivosBase;

            var maxMovDet = _movilesEnMovimiento > _movilesDetenidos ? (Convert.ToInt32(_movilesEnMovimiento / 10) + 1) * 10
                                                                   : (Convert.ToInt32(_movilesDetenidos / 10) + 1) * 10;
            gaugeMovDet.Gauges[0].Maximum = maxMovDet;
            gaugeMovDet.Gauges[0].Value = _movilesEnMovimiento;
            gaugeMovDet.Gauges[0].MorePointers[0].Value = _movilesDetenidos;

            var maxBaseGeo = _movilesEnBase > _movilesEnGeocerca ? (Convert.ToInt32(_movilesEnBase / 10) + 1) * 10
                                                               : (Convert.ToInt32(_movilesEnGeocerca / 10) + 1) * 10;
            gaugeBaseGeocerca.Gauges[0].Maximum = maxBaseGeo;
            gaugeBaseGeocerca.Gauges[0].Value = _movilesEnBase;
            gaugeBaseGeocerca.Gauges[0].MorePointers[0].Value = _movilesEnGeocerca;

            var maxMas1Hora = _movilesEnBaseMas1Hora > _movilesEnGeocercaMas1Hora ? (Convert.ToInt32(_movilesEnBaseMas1Hora / 10) + 1) * 10
                                                                                : (Convert.ToInt32(_movilesEnGeocercaMas1Hora / 10) + 1) * 10;
            gaugeMas1Hora.Gauges[0].Maximum = maxMas1Hora;
            gaugeMas1Hora.Gauges[0].Value = _movilesEnBaseMas1Hora;
            gaugeMas1Hora.Gauges[0].MorePointers[0].Value = _movilesEnGeocercaMas1Hora;

            var maxEnHora = (Convert.ToInt32(_movilesEnHoraHoy / 10) + 1) * 10;
            gaugeEnHoraHoy.Gauges[0].Maximum = maxEnHora;
            gaugeEnHoraHoy.Gauges[0].Value = _movilesEnHoraHoy;

            var maxDemorados = (Convert.ToInt32(_movilesDemoradosHoy / 10) + 1) * 10;
            gaugeDemoradosHoy.Gauges[0].Maximum = maxDemorados;
            gaugeDemoradosHoy.Gauges[0].Value = _movilesDemoradosHoy;

            var maxAdelantados = (Convert.ToInt32(_movilesAdelantadosHoy / 10) + 1) * 10;
            gaugeAdelantadosHoy.Gauges[0].Maximum = maxAdelantados;
            gaugeAdelantadosHoy.Gauges[0].Value = _movilesAdelantadosHoy;

            var maxEnPlanta = (Convert.ToInt32(_movilesEnPlanta / 10) + 1) * 10;
            gaugeEnPlanta.Gauges[0].Maximum = maxEnPlanta;
            gaugeEnPlanta.Gauges[0].Value = _movilesEnPlanta;

            var maxEnCliente = (Convert.ToInt32(_movilesEnCliente / 10) + 1) * 10;
            gaugeEnCliente.Gauges[0].Maximum = maxEnCliente;
            gaugeEnCliente.Gauges[0].Value = _movilesEnCliente;

            var maxEnViaje = (Convert.ToInt32(_movilesEnViaje / 10) + 1) * 10;
            gaugeEnViaje.Gauges[0].Maximum = maxEnViaje;
            gaugeEnViaje.Gauges[0].Value = _movilesEnViaje;
        }
        
        private void LoadGaugesAyer(int cochesCount, int entradasBaseCount, int entradasGeocercaCount, double averageSpeed, int tickets)
        {
            var maxStop1 = (Convert.ToInt32(_stopsHigher1 / 50) + 1) * 50;
            gaugeDet1.Gauges[0].Maximum = maxStop1;
            gaugeDet1.Gauges[0].Value = _stopsHigher1;
            gaugeDet1.Gauges[0].MorePointers[0].Value = _stopsHigher15;

            var maxVel = (Convert.ToInt32(averageSpeed / 20) + 1) * 20;
            gaugeAverageSpeed.Gauges[0].Maximum = maxVel > 120 ? maxVel : 120;
            ((C1GaugeLabels)gaugeAverageSpeed.Gauges[0].Decorators[3]).Interval = maxVel > 120 ? 20 : 10;
            gaugeAverageSpeed.Gauges[0].Value = averageSpeed;

            maxVel = (Convert.ToInt32(_maxSpeed / 20) + 1) * 20;
            gaugeMaxSpeed.Gauges[0].Maximum = maxVel > 120 ? maxVel : 120;
            ((C1GaugeLabels)gaugeMaxSpeed.Gauges[0].Decorators[3]).Interval = maxVel > 120 ? 20 : 10;
            gaugeMaxSpeed.Gauges[0].Value = _maxSpeed;

            var maxInf = (Convert.ToInt32(_infracciones / 10) + 1) * 10;
            gaugeInfracciones.Gauges[0].Maximum = maxInf;
            gaugeInfracciones.Gauges[0].Value = _infracciones;
            gaugeInfracciones.Gauges[0].MorePointers[0].Value = _vehiculosConInfraccion;

            var maxKm = cochesCount > 0 ? (Convert.ToInt32(_totalKilometers / cochesCount / 100) + 1) * 100 : 100;
            gaugeKm.Gauges[0].Maximum = maxKm;
            gaugeKm.Gauges[0].Value = _totalKilometers > 0 && cochesCount > 0 ? _totalKilometers / cochesCount : 0;

            var maxEnt = entradasGeocercaCount > entradasBaseCount ? (Convert.ToInt32(entradasGeocercaCount / 10) + 1) * 10
                                                                   : (Convert.ToInt32(entradasBaseCount / 10) + 1) * 10;
            gaugeEntradas.Gauges[0].Maximum = maxEnt;
            gaugeEntradas.Gauges[0].Value = (entradasGeocercaCount - entradasBaseCount);
            gaugeEntradas.Gauges[0].MorePointers[0].Value = entradasBaseCount;

            var maxTickets = (Convert.ToInt32(tickets / 50) + 1) * 50;
            gaugeTickets.Gauges[0].Maximum = maxTickets;
            gaugeTickets.Gauges[0].Value = tickets;

            var maxEnHora = (Convert.ToInt32(_movilesEnHoraAyer / 10) + 1) * 10;
            gaugeEnHoraAyer.Gauges[0].Maximum = maxEnHora;
            gaugeEnHoraAyer.Gauges[0].Value = _movilesEnHoraAyer;

            var maxDemorados = (Convert.ToInt32(_movilesDemoradosAyer / 10) + 1) * 10;
            gaugeDemoradosAyer.Gauges[0].Maximum = maxDemorados;
            gaugeDemoradosAyer.Gauges[0].Value = _movilesDemoradosAyer;

            var maxAdelantados = (Convert.ToInt32(_movilesAdelantadosAyer / 10) + 1) * 10;
            gaugeAdelantadosAyer.Gauges[0].Maximum = maxAdelantados;
            gaugeAdelantadosAyer.Gauges[0].Value = _movilesAdelantadosAyer;
        }

        private void LoadGaugesMes()
        {
            var maxEnHora = (Convert.ToInt32(_movilesEnHoraMes / 100) + 1) * 100;
            gaugeEnHoraMes.Gauges[0].Maximum = maxEnHora;
            gaugeEnHoraMes.Gauges[0].Value = _movilesEnHoraMes;

            var maxDemorados = (Convert.ToInt32(_movilesDemoradosMes / 100) + 1) * 100;
            gaugeDemoradosMes.Gauges[0].Maximum = maxDemorados;
            gaugeDemoradosMes.Gauges[0].Value = _movilesDemoradosMes;

            var maxAdelantados = (Convert.ToInt32(_movilesAdelantadosMes / 100) + 1) * 100;
            gaugeAdelantadosMes.Gauges[0].Maximum = maxAdelantados;
            gaugeAdelantadosMes.Gauges[0].Value = _movilesAdelantadosMes;
        }
    }
}
