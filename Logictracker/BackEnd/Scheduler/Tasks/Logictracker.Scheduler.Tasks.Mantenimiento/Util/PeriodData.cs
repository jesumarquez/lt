using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Utils;

namespace Logictracker.Scheduler.Tasks.Mantenimiento.Util
{
    public class PeriodData: IDisposable
    {
        private DAOFactory DaoFactory { get; set; }
        public Coche Vehiculo { get; set; }

        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }

        private TimeZoneInfo Culture { get; set; }
        private double GmtModifier { get; set; }

        private List<Shift> Turnos { get; set; }
        private List<int> Feriados { get; set; }

        public IList<Ticket> Tickets { get; set; }
        public IList<ViajeDistribucion> Distribuciones { get; set; }

        public Dictionary<DateTime, int?> EventosRfid { get; set; }
        public Dictionary<DateTime, ReferenciaGeografica> EventosGeocerca { get; set; }
        public IList<Infraccion> Infracciones { get; set; }
        public Dictionary<DateTime, bool> EventosTimeTracking { get; set; }

        private Medicion UltimoConsumo { get; set; }
        public List<Medicion> Consumos { get; set; }

        public LogPosicion PosicionAnterior { get; set; }
        public LogPosicion PosicionSiguiente { get; set; }
        public IList<LogPosicion> Posiciones { get; set; }

        public List<DateTime> FechasDeCorte { get; set; }
        
        public PeriodData(DAOFactory daoFactory, Coche vehiculo, DateTime inicio, DateTime fin)
        {
            DaoFactory = daoFactory;
            Vehiculo = vehiculo;
            Inicio = inicio;
            Fin = fin;

            LoadData();
        }

        public void Dispose()
        {
            DaoFactory = null;
            Vehiculo = null;
            Turnos = null;
            Feriados = null;
            Tickets = null;
            Distribuciones = null;
            EventosRfid = null;
            EventosGeocerca = null;
            Infracciones = null;
            UltimoConsumo = null;
            Consumos = null;
            PosicionAnterior = null;
            PosicionSiguiente = null;
            Posiciones = null;
            FechasDeCorte = null;
            EventosTimeTracking = null;

            GC.Collect();
        }

        #region LoadData
        private void LoadData()
        {
            LoadCulture();
            LoadTurnos();
            LoadCicloLogistico();
            LoadEvents();
            
            //LoadTimeTracking();
            //LoadConsumos();

            LoadPosiciones();
            LoadFechasDeCorte();
        }
        private void LoadCulture()
        {   
            var timeZoneId = Vehiculo.Linea != null
                                ? Vehiculo.Linea.TimeZoneId
                                : Vehiculo.Empresa != null ? Vehiculo.Empresa.TimeZoneId : string.Empty;
			Culture = String.IsNullOrEmpty(timeZoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            GmtModifier = Culture != null ? Culture.BaseUtcOffset.TotalHours : 0;
        }
        private void LoadTurnos()
        {
            Turnos = DaoFactory.ShiftDAO.GetVehicleShifts(Vehiculo);
            Feriados = DaoFactory.FeriadoDAO.FindByEmpresaYLineaAndUser(Vehiculo.Empresa, Vehiculo.Linea, null).Select(feriado => feriado.Fecha.DayOfYear).ToList();
        }
        private void LoadCicloLogistico()
        {
            Tickets = DaoFactory.TicketDAO.FindList(Vehiculo.Id, Inicio, Fin);
            Distribuciones = DaoFactory.ViajeDistribucionDAO.FindList(Vehiculo.Id, Inicio, Fin);
        }
        private void LoadEvents()
        {
            var lastDatamart = DaoFactory.DatamartDAO.GetLastDatamart(Vehiculo.Id, Inicio);
            
            var lastDatamartDate = lastDatamart != null ? (DateTime?)lastDatamart.End : null;
            var lastDriver = lastDatamart != null ? lastDatamart.Employee : null;
            var lastGeocerca = lastDatamart != null ? lastDatamart.GeograficRefference : null;

            EventosRfid = new Dictionary<DateTime, int?> { { lastDatamartDate.HasValue ? lastDatamartDate.Value : DateTime.MinValue, lastDriver != null ? lastDriver.Id : new int?() } };
            EventosGeocerca = new Dictionary<DateTime, ReferenciaGeografica> { { lastDatamartDate.HasValue ? lastDatamartDate.Value : DateTime.MinValue, lastGeocerca } };
            Infracciones = DaoFactory.InfraccionDAO.GetByVehiculo(Vehiculo.Id, Inicio, Fin);
            
            var maxMonths = Vehiculo.Empresa != null ? Vehiculo.Empresa.MesesConsultaPosiciones : 3;

            var events = DaoFactory.LogMensajeDAO.GetEventos(Vehiculo.Id, Inicio, Fin, maxMonths,
                MessageCode.RfidDriverLogin.GetMessageCode(),
                MessageCode.RfidDriverLogout.GetMessageCode(),
                MessageCode.InsideGeoRefference.GetMessageCode(),
                MessageCode.OutsideGeoRefference.GetMessageCode()
                );
            
            foreach (var mensaje in events)
            {
                if (mensaje.Mensaje.Codigo.Equals(MessageCode.RfidDriverLogin.GetMessageCode()) ||
                    mensaje.Mensaje.Codigo.Equals(MessageCode.RfidDriverLogout.GetMessageCode()))
                {
                    var employee = mensaje.Mensaje.Codigo.Equals(MessageCode.RfidDriverLogout.GetMessageCode())
                                       ? new int?()
                                       : mensaje.Chofer != null ? mensaje.Chofer.Id : -1;
                    if (EventosRfid.ContainsKey(mensaje.Fecha)) EventosRfid[mensaje.Fecha] = employee;
                    else EventosRfid.Add(mensaje.Fecha, employee);
                }
                else if (mensaje.Mensaje.Codigo.Equals(MessageCode.InsideGeoRefference.GetMessageCode()) ||
                         mensaje.Mensaje.Codigo.Equals(MessageCode.OutsideGeoRefference.GetMessageCode()))
                {
                    var referencia =
                        mensaje.Mensaje.Codigo.Equals(MessageCode.OutsideGeoRefference.GetMessageCode()) ||
                        !mensaje.IdPuntoDeInteres.HasValue
                            ? null
                            : DaoFactory.ReferenciaGeograficaDAO.FindById(mensaje.IdPuntoDeInteres.Value);
                    if (EventosGeocerca.ContainsKey(mensaje.Fecha)) EventosGeocerca[mensaje.Fecha] = referencia;
                    else EventosGeocerca.Add(mensaje.Fecha, referencia);
                }
            }
        }
        private void LoadTimeTracking()
        {
            var prevEvent = DaoFactory.EventoViajeDAO.FindFirstInicioFin(Vehiculo.Id, null, Inicio);
            var events = DaoFactory.EventoViajeDAO.FindInicioFin(Vehiculo.Id, Inicio, Fin);

            var iniciado = prevEvent != null && (prevEvent.EsInicio && !prevEvent.EsEntrada);
            var fecha = prevEvent != null ? prevEvent.Fecha : DateTime.MinValue;

            EventosTimeTracking = new Dictionary<DateTime, bool> { { fecha, iniciado } };

            foreach (var eventoViaje in events)
            {
                var esInicio = eventoViaje.EsInicio && !eventoViaje.EsEntrada;
                if (esInicio && !iniciado)
                {
                    EventosTimeTracking.Add(eventoViaje.Fecha, iniciado = true);
                }
                else if(!esInicio && iniciado)
                {
                    EventosTimeTracking.Add(eventoViaje.Fecha, iniciado = false);
                }
            }
        }
        private void LoadConsumos()
        {
            var sensorConsumo = Vehiculo.Dispositivo != null ? DaoFactory.SensorDAO.FindByDispositivoAndTipoMedicion(Vehiculo.Dispositivo.Id, "NU") : null;

            if (sensorConsumo == null)
            {
                UltimoConsumo = null;
                Consumos = new List<Medicion>(0);
            }
            else
            {
                var medicionDao = DaoFactory.MedicionDAO;
                UltimoConsumo = medicionDao.FindLast(sensorConsumo.Id, Inicio);
                Consumos = medicionDao.FindList(sensorConsumo.Id, Inicio, Fin);
            }
        }

        #region Posicion
        private void LoadPosiciones()
        {   
            var posicionDao = DaoFactory.LogPosicionDAO;

            var maxMonths = Vehiculo.Empresa != null ? Vehiculo.Empresa.MesesConsultaPosiciones : 3;

            var t = new TimeElapsed();
            PosicionSiguiente = posicionDao.GetFirstPositionNewerThanDate(Vehiculo.Id, Fin, maxMonths);
            var ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Error("Logictracker.Scheduler.Tasks.Mantenimiento.DatamartGeneration", string.Format("GetFirstPositionNewerThanDate en {0} segundos", ts));

            t.Restart();
            PosicionAnterior = Vehiculo.LastOdometerUpdate.HasValue ? posicionDao.GetFirstPositionOlderThanDate(Vehiculo.Id, Inicio, maxMonths) : null;
            ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Error("Logictracker.Scheduler.Tasks.Mantenimiento.DatamartGeneration", string.Format("GetFirstPositionOlderThanDate en {0} segundos", ts));

            t.Restart();
            var originalPositions = posicionDao.GetPositionsBetweenDates(Vehiculo.Id, Inicio, Fin, maxMonths);
            ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 1) STrace.Error("Logictracker.Scheduler.Tasks.Mantenimiento.DatamartGeneration", string.Format("GetPositionsBetweenDates: {0} posiciones en {1} segundos", originalPositions.Count, ts));

            if (Vehiculo.Dispositivo != null && !DaoFactory.DetalleDispositivoDAO.GetDiscardsInvalidPositionsValue(Vehiculo.Dispositivo.Id))
            {
                Posiciones = CorrectGeorefferenciation(originalPositions);
            }
            else
            {
                var cleanPositions = new List<LogPosicion>();
                var lastValidPosition = PosicionAnterior;
                foreach (var position in originalPositions)
                {
                    if (!EsPosicionValida(lastValidPosition, position))
                    {
                        posicionDao.Delete(position);
                        var discartedPosition = new LogPosicionDescartada(position, DiscardReason.DatamartCleanUp);
                        DaoFactory.LogPosicionDescartadaDAO.Save(discartedPosition);
                    }
                    else
                    {
                        cleanPositions.Add(position);
                        lastValidPosition = position;
                    }
                }
                Posiciones = CorrectGeorefferenciation(cleanPositions);
            }
        }

        private IList<LogPosicion> CorrectGeorefferenciation(IList<LogPosicion> positions)
        {
            if (Vehiculo.Dispositivo != null && !DaoFactory.DetalleDispositivoDAO.GetCorrectsGeoreferenciationValue(Vehiculo.Dispositivo.Id)) return positions;

            var radius = Vehiculo.Dispositivo != null ? DaoFactory.DetalleDispositivoDAO.GetStoppedGeocercaRadiusValue(Vehiculo.Dispositivo.Id) : 25;
            var lastPosition = PosicionAnterior;

            for (var i = 0; i < positions.Count; i++)
            {
                if (lastPosition != null && lastPosition.Velocidad.Equals(0))
                {
                    var distance = GetDistance(lastPosition, positions[i]) * 1000;

                    if (distance <= radius)
                    {
                        CorrectGeoreferenciation(lastPosition, positions[i]);
                    }
                    else
                    {
                        if (i.Equals(positions.Count - 1))
                        {
                            if (PosicionSiguiente == null) break;
                            var finaldistance = GetDistance(lastPosition, PosicionAnterior) * 1000;
                            if (finaldistance <= radius) CorrectGeoreferenciation(lastPosition, positions[i]);
                        }
                        else
                        {
                            var outside = 0;
                            for (var j = i + 1; j < positions.Count; j++)
                            {
                                var auxDistance = GetDistance(lastPosition, positions[j]) * 1000;
                                if (auxDistance <= radius)
                                {
                                    CorrectGeoreferenciation(lastPosition, positions[i]);
                                    break;
                                }
                                if (++outside > 5) break;
                            }
                        }
                    }
                }

                lastPosition = positions[i];
            }

            return positions;
        }
        /// <summary>
        /// Corrects the current position georeferenciation in order to maintain the original stop position.
        /// </summary>
        /// <param name="lastPosition"></param>
        /// <param name="posicion"></param>
        private void CorrectGeoreferenciation(LogPosicionBase lastPosition, LogPosicion posicion)
        {
            posicion.Longitud = lastPosition.Longitud;
            posicion.Latitud = lastPosition.Latitud;
            posicion.Altitud = lastPosition.Altitud;
            posicion.Velocidad = lastPosition.Velocidad;

            DaoFactory.LogPosicionDAO.SaveOrUpdate(posicion);
        }

        private bool EsPosicionValida(LogPosicionBase a, LogPosicionBase b)
        {
            if (a == null || b == null) return true;

            //Calculo el tiempo entre posiciones, la distancia y la maxima distancia alcanzable por el movil en ese intervalo de tiempo.
            var tiempo = Math.Abs(b.FechaMensaje.Subtract(a.FechaMensaje).TotalSeconds);
            var distancia = Math.Abs(Distancias.Loxodromica(a.Latitud, a.Longitud, b.Latitud, b.Longitud));
            var maximaDistanciaTipoVehiculo = (Vehiculo.TipoCoche.MaximaVelocidadAlcanzable * tiempo) / 3.6;

            return distancia <= maximaDistanciaTipoVehiculo;
        }

        private static double GetDistance(LogPosicionBase initialPosition, LogPosicionBase lastPosition)
        {
            if (initialPosition == null || lastPosition == null) return 0;
            return Distancias.Loxodromica(initialPosition.Latitud, initialPosition.Longitud, lastPosition.Latitud, lastPosition.Longitud) / 1000.0;
        }
        #endregion

        #region FechasDeCorte
        public void LoadFechasDeCorte()
        {
            var horas = new List<DateTime>();
            var inicio = Inicio;
            while (inicio < Fin)
            {
                foreach (var turno in Turnos)
                {
                    if (turno.AppliesToDate(inicio, Feriados))
                    {
                        horas.Add(inicio.Date.AddHours(turno.Inicio - GmtModifier));
                        horas.Add(inicio.Date.AddHours(turno.Fin - GmtModifier));
                    }
                }
                for (var i = inicio.Date; i < inicio.Date.AddDays(1); i = i.AddHours(1))
                {
                    if (i < inicio) continue;
                    horas.Add(i);
                }
                inicio = inicio.Date.AddDays(1);
            }

            FechasDeCorte = Tickets.Select(t => t.FechaTicket.Value)
                .Union(Tickets.Where(t => t.FechaFin.HasValue).Select(t => t.FechaFin.Value))
                .Union(Distribuciones.Where(d => d.InicioReal.HasValue).Select(d => d.InicioReal.Value ))
                .Union(Distribuciones.Where(d => d.InicioReal.HasValue).Select(d => d.Fin))
                .Union(EventosRfid.Select(e => e.Key))
                .Union(EventosGeocerca.Select(e => e.Key))
                //.Union(EventosTimeTracking.Select(e => e.Key))
                .Union(horas)
                .Where(h=> h >= Inicio && h <= Fin)
                .Distinct()
                .OrderBy(h => h)
                .ToList();

            if(FechasDeCorte.Last() != Fin) FechasDeCorte.Add(Fin);
        }
        #endregion 

        #endregion

        #region GetData

        private int _indexRfid;
        public Empleado GetChofer(DateTime date)
        {
            var id = GetChoferId(date);
            return id.HasValue && id > 0 ? DaoFactory.EmpleadoDAO.FindById(id.Value) : null;
        }
        private int? GetChoferId(DateTime date)
        {
            if (!Vehiculo.IdentificaChoferes) return Vehiculo.Chofer != null ? Vehiculo.Chofer.Id : new int?();
            if (EventosRfid.Count == 0) return new int?();
            var chofer = new int?();
            DateTime key;
            while (_indexRfid < EventosRfid.Count && (key = EventosRfid.Keys.ElementAt(_indexRfid)) <= date)
            {
                chofer = EventosRfid[key];
                _indexRfid++;
            }
            if (_indexRfid > 0) _indexRfid--;
            return chofer;
        }

        private int _indexGeoReference;
        public ReferenciaGeografica GetGeoReference(DateTime date)
        {
            if (EventosGeocerca.Count == 0) return null;
            ReferenciaGeografica geocerca = null;
            DateTime key;
            while (_indexGeoReference < EventosGeocerca.Count && (key = EventosGeocerca.Keys.ElementAt(_indexGeoReference)) <= date)
            {
                geocerca = EventosGeocerca[key];
                _indexGeoReference++;
            }
            if (_indexGeoReference > 0) _indexGeoReference--;
            return geocerca;
        }

        private int _indexTimeTracking;
        public bool GetTimeTracking(DateTime date)
        {
            if (EventosTimeTracking == null || EventosTimeTracking.Count == 0) return false;
            var iniciado = false;
            DateTime key;
            while (_indexTimeTracking < EventosTimeTracking.Count && (key = EventosTimeTracking.Keys.ElementAt(_indexTimeTracking)) <= date)
            {
                iniciado = EventosTimeTracking[key];
                _indexTimeTracking++;
            }
            if (_indexTimeTracking > 0) _indexTimeTracking--;
            return iniciado;
        }

        public ExcesosDatamart GetExcesos(DateTime from, DateTime to)
        {
            var excesos = new ExcesosDatamart();
            if (Infracciones.Count().Equals(0)) return excesos;

            var infractions = Infracciones.Where(infraction => infraction.Fecha >= from && infraction.Fecha <= to).ToList();

            if (!infractions.Any()) return excesos;

            foreach (var infraction in infractions)
            {
                excesos.Excesos += 1;
                excesos.SegundosExceso += infraction.Duracion.TotalSeconds;
            }

            Infracciones = Infracciones.Where(infraction => infraction.Fecha > to).ToList();

            return excesos;
        }
        public Shift GetTurno(DateTime dateTime)
        {
            if (Turnos == null || Turnos.Count.Equals(0)) return null;
            var date = dateTime.AddHours(GmtModifier);
            return Turnos.FirstOrDefault(shift => shift.AppliesToDateTime(date, Feriados));
        }

        private int _indexConsumo;
        public double GetConsumo(DateTime date)
        {
            if (Consumos == null || Consumos.Count == 0 || _indexConsumo >= Consumos.Count) return 0;
            if (UltimoConsumo == null) UltimoConsumo = Consumos[_indexConsumo];

            var first = _indexConsumo;
            var last = Consumos.Skip(first).TakeWhile(c => c.FechaMedicion <= date).Count() + first;
            if (--last < 0) last = 0;
            _indexConsumo = last;
            
            return Consumos[last].ValorNum1 - Consumos[first].ValorNum1;
        }

        #region GetCicloLogistico
        public int? GetTipoCiclo(DateTime date, out int idciclo)
        {
            var ticket = GetTicket(date);
            if (ticket != null)
            {
                idciclo = ticket.Id;
                return Datamart.TiposCiclo.Hormigon;
            }
            var ditribucion = GetDistribucion(date);
            if (ditribucion != null)
            {
                idciclo = ditribucion.Id;
                return Datamart.TiposCiclo.Distribucion;
            }
            idciclo = 0;
            return null;
        }
        private Ticket GetTicket(DateTime date)
        {
            if (Tickets.Count == 0 || Tickets[0].FechaTicket > date) return null;

            while (Tickets.Count > 0)
            {
                var ticket = Tickets[0];
                DateTime maxDate;
                if (ticket.FechaFin.HasValue)
                {
                    maxDate = ticket.FechaFin.Value;
                }
                else
                {
                    var lastDetalle = ticket.Detalles.Cast<DetalleTicket>().Last();
                    maxDate = lastDetalle.Programado.Value;
                    if (lastDetalle.Automatico.HasValue && maxDate < lastDetalle.Automatico.Value) maxDate = lastDetalle.Automatico.Value;
                }

                if (maxDate < date) Tickets.RemoveAt(0);
                else if (date > ticket.FechaTicket && date < maxDate) return ticket;
                else break;
            }
            return null;
        }
        private ViajeDistribucion GetDistribucion(DateTime date)
        {
            var distribuciones = Distribuciones.Where(d => d.InicioReal.HasValue).ToList();
            if (!distribuciones.Any()) return null;
            
            while (distribuciones.Any())
            {
                var distribucion = distribuciones[0];
                var inicio = distribucion.InicioReal.Value;
                var fin = distribucion.Fin;

                if (distribucion.Fin < date) distribuciones.RemoveAt(0);
                else if (date >= inicio && date < fin) return distribucion;
                else break;
            }
            return null;
        }
        #endregion 
        #endregion

        public double GetDistancia(LogPosicionBase inicio, LogPosicionBase fin, DateTime? desde, DateTime? hasta)
        {
            if (inicio == null || fin == null) return 0;
            var totalDistance = Distancias.Loxodromica(inicio.Latitud, inicio.Longitud, fin.Latitud, fin.Longitud) / 1000.0;

            if (!desde.HasValue && !hasta.HasValue) return totalDistance;
            
            var fDesde = desde.HasValue ? desde.Value : inicio.FechaMensaje;
            var fHasta = hasta.HasValue ? hasta.Value : fin.FechaMensaje;

            var totalTime = fin.FechaMensaje.Subtract(inicio.FechaMensaje).TotalMinutes;
            var partialTime = fHasta.Subtract(fDesde).TotalMinutes;

            return totalTime > 0 ? (totalDistance * partialTime) / totalTime : 0;
        }

		public String GetHash(LogPosicionBase posicion)
        {
            return string.Format("{0}|{1}|{2}",
                posicion.Velocidad == 0 ? "D" : "M", // Detención/Movimiento
                posicion.MotorOn.HasValue ? (posicion.MotorOn.Value ? "E" : "A") : "D", // Estado Motor
                posicion.Zona != null ? posicion.Zona.Id : 0 // Zona de manejo
                ); 

        }
    }
}
