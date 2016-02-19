using System;
using System.Globalization;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.Cache;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.Geofences;
using Logictracker.Process.Geofences.Classes;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.TimeTracking;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;

namespace Logictracker.Dispatcher.Handlers
{
    /// <summary>
    /// Handler for porcessing positions packages.
    /// </summary>
    [FrameworkElement(XName = "PositionsHandler", IsContainer = false)]
    public class Positions : DeviceBaseHandler<Position>
    {
        #region Private Properties

        /// <summary>
        /// The message being handled.
        /// </summary>
        private Position _posicion;

        /// <summary>
        /// Determines if the current package beeing analyzed correspond to online positions.
        /// </summary>
        private bool _fechasOnline;

        /// <summary>
        /// The last online position reported for the current vehicle and package context.
        /// </summary>
        private GPSPoint _lastPosition;

        /// <summary>
        /// Determines if a message should be generated to inform the first position of the current device and vehicle combination.
        /// </summary>
        private Boolean _informFirstPosition;

        /// <summary>
        /// The last valid position reported for the current vehicle and package context.
        /// </summary>
        private GPSPoint _lastValidPosition;

        /// <summary>
        /// Current package accumalted kilometers.
        /// </summary>
        private Double _kilometers;

        /// <summary>
        /// Determines if the package is from a new day than the last position.
        /// </summary>
        private Boolean _changeDay;

        /// <summary>
        /// Current package accumulated running hours.
        /// </summary>
        private Double _hours;

        /// <summary>
        /// Current vehicles assigned culture.
        /// </summary>
        private TimeZoneInfo _culture;

        /// <summary>
        /// Current vehicles culture gmt modifier.
        /// </summary>
        private Double _gmtModifier;

        /// <summary>
        /// The last online position for the current combination of vehicle and device.
        /// </summary>
        private LogUltimaPosicionVo _lastOnlinePosition;


        private DateTime? LastPositionReceivedAt
        {
            get
            {
                var key = "device_" + Coche.Dispositivo.Id + "_lastPositionReceivedAt";

                if (!LogicCache.KeyExists(typeof(DateTime), key))
                    return null;

                var dt = LogicCache.Retrieve<string>(key);
                if (!String.IsNullOrEmpty(dt))
                    return DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture);
                
                LogicCache.Delete(typeof (string), key);
                return null;
            }
            set
            {
                var key = "device_" + Coche.Dispositivo.Id + "_lastPositionReceivedAt";
                if (value == null)
                {
                    LogicCache.Delete(key);
                }
                else
                {
                    var oldLastPositionReceivedAt = LastPositionReceivedAt;

                    LogicCache.Store(key, value.ToString());

                    if (oldLastPositionReceivedAt != null)
                    {
                        var dif = value.Value.Subtract(oldLastPositionReceivedAt.Value);
                        LastPositionInterval = dif.Minutes;
                    }
                    else
                    {
                        LastPositionInterval = null;
                    }
                }
            }
        }

        private Int32? LastPositionInterval
        {
            get
            {
                var key = "device_" + Coche.Dispositivo.Id + "_lastPositionInterval";
                if (!LogicCache.KeyExists(typeof(string), key))
                    return null;
                
                var val = LogicCache.Retrieve<string>(key);
                if (!String.IsNullOrEmpty(val))
                    return Convert.ToInt32(val);
                
                LogicCache.Delete(key);
                return null;
            }
            set
            {
                var key = "device_" + Coche.Dispositivo.Id + "_lastPositionInterval";
                if (value == null)
                    LogicCache.Delete(key);
                else 
                    LogicCache.Store(key, value.ToString());
            }
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Process handler main tasks.
        /// </summary>
        /// <param name="msg"></param>
        protected override HandleResults OnDeviceHandleMessage(Position msg)
        {
            try
            {
                //STrace.Debug(typeof(Events).FullName, msg.DeviceId, "OnDeviceHandleMessage (PositionsHandler)");
                //Si no se reportaron posiciones en el paquete no tiene sentido que siga procesando.)

                if (msg.GeoPoints.Count <= 0) return HandleResults.BreakSuccess;

                var t = new TimeElapsed();
                SetUpEnviroment(msg);
                var ts = t.getTimeElapsed().TotalSeconds;
                if (ts > 0.5) STrace.Debug("DispatcherLock", "SetUpEnviroment: " + ts);

                t.Restart();
                ProcessData();
                ts = t.getTimeElapsed().TotalSeconds;
                if (ts > 0.5) STrace.Debug("DispatcherLock", "ProcessData: " + ts);

                return HandleResults.Success;
            }
            catch (Exception)
            {
                try
                {
                    DiscartPositionsDueToException();
                }
                catch (Exception ex2)
                {
                    STrace.Exception(typeof (Events).FullName, ex2);
                }

                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handler enviroment initial set up.
        /// </summary>
        /// <param name="msg"></param>
        private void SetUpEnviroment(Position msg)
        {            
            _posicion = msg;

            _kilometers = 0;

            _hours = 0;
            
            GetLastOnlinePosition();

            var te = new TimeElapsed();
            OnlinePackage();
            var totalSecs = te.getTimeElapsed().TotalSeconds;
            if (totalSecs > 1) STrace.Error("DispatcherLock", msg.DeviceId, "OnlinePackage: " + totalSecs);

            te.Restart();
            GetLastValidPosition();
            totalSecs = te.getTimeElapsed().TotalSeconds;
            if (totalSecs > 1) STrace.Error("DispatcherLock", msg.DeviceId, "GetLastValidPosition: " + totalSecs);

            te.Restart();
            ChangeDay();
            totalSecs = te.getTimeElapsed().TotalSeconds;
            if (totalSecs > 1) STrace.Error("DispatcherLock", msg.DeviceId, "ChangeDay: " + totalSecs);

            te.Restart();
            GetGmtModifier();
            totalSecs = te.getTimeElapsed().TotalSeconds;
            if (totalSecs > 1) STrace.Error("DispatcherLock", msg.DeviceId, "GetGmtModifier: " + totalSecs);
        }

        /// <summary>
        /// Gets the current vehicles gmt modifier.
        /// </summary>
        private void GetGmtModifier()
        {
            _culture = GetCulture();

            _gmtModifier = _culture != null ? _culture.BaseUtcOffset.TotalHours : 0;
        }

        /// <summary>
        /// Determines if the package generates a date change.
        /// </summary>
        private void ChangeDay()
        {
            var initialDate = _lastValidPosition != null ? _lastValidPosition.Date : _posicion.GeoPoints[0].Date;

            _changeDay = ChangeDay(initialDate, _posicion.GeoPoints[_posicion.GeoPoints.Count - 1].Date);
        }

        /// <summary>
        /// Discarts current positions due to the raise of an exception.
        /// </summary>
        private void DiscartPositionsDueToException() { foreach (var position in _posicion.GeoPoints) SaveDiscartedPosition(position, DiscardReason.Exception); }

        /// <summary>
        /// Process handler core activities.
        /// </summary>
        private void ProcessData()
        {
            var procesarCicloLogisticoFlag = DaoFactory.DetalleDispositivoDAO.GetProcesarCicloLogisticoFlagValue(_posicion.DeviceId);
            var nonDiscarted = 0;

            foreach (var position in _posicion.GeoPoints.Where(position => !DiscardInvalidPosition(position)))
            {   
                nonDiscarted++;
                var t = new TimeElapsed();
                var correctsHdop = CorrectionByHdop(position);
                var ts = t.getTimeElapsed().TotalSeconds;
                if (ts > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "CorrectionByHdop: " + ts);

                if (correctsHdop)
                {
                    position.Lat = _lastPosition.Lat;
                    position.Lon = _lastPosition.Lon;
                    position.Height = _lastPosition.Height;
                    position.HDOP = _lastPosition.HDOP;
                }
                
                EstadoVehiculo estado = null;

                if (_fechasOnline)
                {
                    t.Restart();
                    estado = AnalizeGeoReferences(position, procesarCicloLogisticoFlag);
                    ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "AnalizeGeoReferences: " + ts);
                }
                else
                {
                    STrace.Error("FechasOnline Dispatcher", _posicion.DeviceId, "No se procesa el AnalizeGeoReferences");
                }

                t.Restart();
                SavePosition(position, estado);
                ts = t.getTimeElapsed().TotalSeconds;
                if (ts > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "SavePosition: " + ts);
            	
                if (_fechasOnline)
            	{
                    t.Restart();
                    AnalizeStoppedEvents(position, correctsHdop, estado);
                    ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "AnalizeStoppedEvents: " + ts);

                    t.Restart();
                    UpdateKilometers(position);
                    ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "UpdateKilometers: " + ts);

                    t.Restart();
            	    UpdateHours(position);
                    ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "UpdateHours: " + ts);

                    t.Restart();
                    AnalizeShifts(position);
                    ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "AnalizeShifts: " + ts);
            	}

            	_lastValidPosition = position;

            	if (!_informFirstPosition) continue;
                    
            	InformFirstPosition();
            }

            if (nonDiscarted.Equals(0) || !_fechasOnline) return;

            var time = new TimeElapsed();
            UpdateVehicleOdometers();
            var total = time.getTimeElapsed().TotalSeconds;
            if (total > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "UpdateVehicleOdometers: " + total);

            time.Restart();
            UpdateLastPosition(_lastValidPosition);
            total = time.getTimeElapsed().TotalSeconds;
            if (total > 1) STrace.Error("DispatcherLock", _posicion.DeviceId, "UpdateLastPosition: " + total);

            LastPositionReceivedAt = DateTime.UtcNow;
        }

        private bool CorrectionByHdop(GPSPoint position)
        {
            var result = false;

            if (!DeviceParameters.CorrectsPositionsGeoreferenciation) return false;

            if (_lastPosition != null &&                
                _lastPosition.Velocidad == 0 &&
                position.Velocidad == 0 && 
                position.HDOP != 0)                
            {
                if (_lastPosition.HDOP > 0 && position.HDOP >= _lastPosition.HDOP)
                {
                    var distance = GetDistance(_lastPosition, position);
                    var maxMeters = ((Math.Pow(position.HDOP,2) + 5) * 10); //S15 tiene una precision de 10 metros

                    result = (distance != 0 && distance < maxMeters);
                }
            }

            return result;
        }

        /// <summary>
        /// Generates a message to inform that the first position of a device and vehicle combination has been recieved.
        /// </summary>
        private void InformFirstPosition()
        {
            if (Coche == null) return;

            var text = String.Format(" - Vehiculo: {0} - Dispositivo {1}", Coche.Interno, Dispositivo.Codigo);

			MessageSaver.Save(_posicion, MessageCode.FirstPosition.GetMessageCode(), Dispositivo, Coche, null, _posicion.GetDateTime(), _lastValidPosition, text);

            _informFirstPosition = false;
        }

        /// <summary>
        /// Saves current position into database.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="estado"> </param>
        private void SavePosition(GPSPoint position, EstadoVehiculo estado)
        {
            var logPosicion = new LogPosicion(position, Coche)
                                  {
                                      Zona = estado != null && estado.ZonaManejo != null
                                                 ? DaoFactory.ZonaDAO.FindById(estado.ZonaManejo.ZonaManejo)
                                                 : null
                                  };
            DaoFactory.LogPosicionDAO.Save(logPosicion);
        }

        /// <summary>
        /// Analize if the vehicle has activity outside its assigned shifts.
        /// </summary>
        /// <param name="position"></param>
        private void AnalizeShifts(GPSPoint position)
        {
            if (position.Velocidad.Equals(0) || !Coche.TipoCoche.ControlaTurno) return;

            if (DaoFactory.ShiftDAO.IsPositionWithinShift(Coche, position.Date, _gmtModifier)) return;

            var lastShift = DaoFactory.ShiftDAO.GetLastShift(Coche, position.Date, _gmtModifier);

            SaveShiftEvent(position, lastShift);
        }

        /// <summary>
        /// Analize if the reported positions start or end an stopped event.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="correctsHdop"></param>
        private void AnalizeStoppedEvents(GPSPoint position, bool correctsHdop, EstadoVehiculo estado)
        {
            var wasStopped = Coche.IsStopped();

            var isNowStopped = position.Velocidad.Equals(0);

            if (!DeviceParameters.GeneratesStoppedEvents) return;

            if (!wasStopped)
            {
                if (isNowStopped)
                        StartStoppedEvent(position);
            }
            else
            {
                if (!isNowStopped)
                        EndStoppedEvent(position, estado);
                else 
                    if (correctsHdop)
                        UpdateStopInitialPosition(position);
            }
        }

        /// <summary>
        /// Updates current vehicles odometers.
        /// </summary>
        private void UpdateVehicleOdometers()
        {
            if (!DeviceParameters.CalculatesOdometers || (_kilometers.Equals(0.0) && !_changeDay && _hours.Equals(0.0))) return;

            UpdateOdometers(_kilometers, _lastValidPosition, _changeDay, _hours);
        }

        /// <summary>
        /// Updates current package accumulated kilometers.
        /// </summary>
        private void UpdateKilometers(GPSPoint position)
        {
            if (!DeviceParameters.CalculatesOdometers) return;

            _kilometers += _lastValidPosition == null ? 0.0 : GetDistance(_lastValidPosition, position) / 1000.0;
        }

        private void UpdateHours(GPSPoint position)
        {
            if (!DeviceParameters.CalculatesOdometers) return;

            _hours += _lastValidPosition != null ? GetHours(_lastValidPosition, position) : 0.0;
        }

        /// <summary>
        /// Update current vehicle last position.
        /// </summary>
        private void UpdateLastPosition(GPSPoint point)
        {
            var lastOnlinePosition = new LogPosicion
                                     	{
                                     		Coche = Coche,
                                     		Dispositivo = Dispositivo,
                                     		FechaMensaje = point.Date,
                                     		FechaRecepcion = DateTime.UtcNow,
                                     		Latitud = point.Lat,
                                     		Longitud = point.Lon,
                                     		Altitud = point.Height.Unpack(),
                                     		Curso = point.Course.Unpack(),
                                     		Status = point.LcyStatus,
                                     		VeloCalculada = false,
                                     		Velocidad = point.Velocidad,
                                            HDop = point.HDOP
                                     	};

        	//Actualizo la ultima posicion valida para el vehiculo.
            var lastPosition = new LogUltimaPosicionVo(lastOnlinePosition);
            Coche.StoreLastPosition(lastPosition);
            Dispositivo.StoreLastPosition(lastPosition);
        }

        /// <summary>
        /// Analize all geo references to determine if the vehicle has entered or leaved any of them.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="procesarCicloLogisticoFlag"> </param>
        private EstadoVehiculo AnalizeGeoReferences(GPSPoint position, bool procesarCicloLogisticoFlag)
        {
            var t = new TimeElapsed();
            var estado = GeocercaManager.Process(Coche, position, DaoFactory);
            var ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 0.5) STrace.Error("DispatcherLock", String.Format("GeocercaManager.Process ({0} secs)", ts));

            t.Restart();
            foreach (var evento in estado.Eventos)
            {
                try
                {
                    Empleado chofer = null;                    

                    if (new[] { GeocercaEventState.Sale, GeocercaEventState.Entra }.Any(e => e == evento.Evento))
                    {
                        chofer = DaoFactory.EmpleadoDAO.GetLoggedInDriver(Coche);                        
                    }

                    t.Restart();
                    switch (evento.Evento) 
                    {
                        case GeocercaEventState.Sale:
                            var isInicioDistribucionPorSalidaDeBase = Coche.Empresa.InicioDistribucionPorSalidaDeBase;                            
                            if (isInicioDistribucionPorSalidaDeBase)
                            {
                                if (DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche) == null)
                                {
                                    var linea = DaoFactory.LineaDAO.GetList(new[] {Coche.Empresa.Id})
                                                                    .FirstOrDefault(l => l.ReferenciaGeografica != null
                                                                                && l.ReferenciaGeografica.Id == evento.Estado.Geocerca.Id);
                                    if (linea != null)
                                    {
                                        var distribucion =
                                            DaoFactory.ViajeDistribucionDAO.FindPendiente(new[] {Coche.Empresa.Id},
                                                new[] {linea.Id}, new[] {Coche.Id}, DateTime.Today,
                                                DateTime.Today.AddDays(1));
                                        if (distribucion != null)
                                        {
                                            var ev = new InitEvent(_posicion.GetDateTime());
                                            var ciclo = new CicloLogisticoDistribucion(distribucion, DaoFactory,
                                                new MessageSaver(DaoFactory));
                                            ciclo.ProcessEvent(ev);
                                        }
                                    }
                                }
                            }
                            else 
                            {
                                var isInicioDistribucionPorSalidaDeGeocerca = Coche.Empresa.InicioDistribucionPorSalidaDeGeocerca;
                                if (isInicioDistribucionPorSalidaDeGeocerca)
                                {
                                    var idTipoGeocerca = Coche.Empresa.InicioDistribucionIdTipoGeocerca;
                                    if (evento.Estado.Geocerca.TipoReferenciaGeograficaId == idTipoGeocerca)
                                    {
                                        var distribucion = DaoFactory.ViajeDistribucionDAO.FindPendiente(new[] { Coche.Empresa.Id }, new[] { -1 }, new[] { Coche.Id }, DateTime.Today, DateTime.Today.AddDays(1));
                                        if (distribucion != null)
                                        {
                                            var ev = new InitEvent(_posicion.GetDateTime());
                                            var ciclo = new CicloLogisticoDistribucion(distribucion, DaoFactory, new MessageSaver(DaoFactory));
                                            ciclo.ProcessEvent(ev);
                                        }
                                    }
                                }
                            }

                            MessageSaver.Save(_posicion,
                                              MessageCode.OutsideGeoRefference.GetMessageCode(),
                                              Dispositivo,
                                              Coche,
                                              chofer,
                                              _posicion.GetDateTime(),
                                              estado.Posicion,
                                              evento.Estado.Geocerca.Descripcion ?? String.Empty,
                                              evento.Estado.Geocerca.Id);
                            
                            if (procesarCicloLogisticoFlag)
                            {
                                var enCurso = GetCiclo(Coche, new MessageSaver(DaoFactory), DaoFactory);
                                if (enCurso != null)
                                {
                                    var e = new GeofenceEvent(position.Date, evento.Estado.Geocerca.Id, GeofenceEvent.EventoGeofence.Salida, position.Lat, position.Lon, chofer);
                                    
                                    var ciclo = enCurso as CicloLogisticoDistribucion;
                                    if (ciclo != null) ciclo.ProcessEstadoLogistico(MessageCode.OutsideGeoRefference.GetMessageCode(), position.Date, evento.Estado.Geocerca.Id);

                                    enCurso.ProcessEvent(e);
                                }
                            }
                            if (evento.Estado.Geocerca.EsTaller)
                            {
                                var ticket = DaoFactory.TicketMantenimientoDAO.GetActive(Coche.Id, evento.Estado.Geocerca.Id);
                                if (ticket != null)
                                {
                                    ticket.Salida = position.Date;
                                    DaoFactory.TicketMantenimientoDAO.SaveOrUpdate(ticket);
                                }

                                if (Coche.Estado == Coche.Estados.EnMantenimiento)
                                {
                                    Coche.Estado = Coche.Estados.Activo;
                                    DaoFactory.CocheDAO.SaveOrUpdate(Coche);
                                }
                            }
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", position.DeviceId, String.Format("Switch evento.Evento/GeocercaEventState.Sale ({0} secs), geocerca: {1}", t.getTimeElapsed().TotalSeconds.ToString(), evento.Estado.Geocerca.Id));
                            break;
                        case GeocercaEventState.Entra:
                            if (Coche.Empresa.InicioDistribucionPorEntradaDeGeocerca && 
                                evento.Estado.Geocerca.TipoReferenciaGeograficaId == Coche.Empresa.InicioDistribucionIdTipoGeocerca)
                            {
                                if (DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche) == null)
                                {
                                    var distribucion = DaoFactory.ViajeDistribucionDAO.FindPendiente(new[] { Coche.Empresa.Id }, new[] { -1 }, new[] { Coche.Id }, DateTime.Today, DateTime.Today.AddDays(1));
                                    if (distribucion != null)
                                    {
                                        var ev = new InitEvent(_posicion.GetDateTime());
                                        var ciclo = new CicloLogisticoDistribucion(distribucion, DaoFactory, new MessageSaver(DaoFactory));
                                        ciclo.ProcessEvent(ev);
                                    }
                                }
                            }

                            MessageSaver.Save(_posicion,
                                              MessageCode.InsideGeoRefference.GetMessageCode(),
                                              Dispositivo,
                                              Coche,
                                              chofer,
                                              _posicion.GetDateTime(),
                                              estado.Posicion,
                                              evento.Estado.Geocerca.Descripcion ?? String.Empty,
                                              evento.Estado.Geocerca.Id);

                            var code = MessageCode.InsideGeoRefference.GetMessageCode() + '-' + evento.Estado.Geocerca.Id;
                            Coche.StoreLastMessageDate(code, _posicion.GetDateTime());

                            if (procesarCicloLogisticoFlag)
                            {
                                var enCurso = GetCiclo(Coche, new MessageSaver(DaoFactory), DaoFactory);
                                if (enCurso != null)
                                {
                                    var e = new GeofenceEvent(position.Date, evento.Estado.Geocerca.Id, GeofenceEvent.EventoGeofence.Entrada, position.Lat, position.Lon, chofer);
                                    
                                    var ciclo = enCurso as CicloLogisticoDistribucion;
                                    if (ciclo != null) ciclo.ProcessEstadoLogistico(MessageCode.InsideGeoRefference.GetMessageCode(), position.Date, evento.Estado.Geocerca.Id);

                                    enCurso.ProcessEvent(e);
                                }
                            }
                            if (evento.Estado.Geocerca.EsTaller)
                            {
                                Coche.Estado = Coche.Estados.EnMantenimiento;
                                DaoFactory.CocheDAO.SaveOrUpdate(Coche);

                                var ticket = DaoFactory.TicketMantenimientoDAO.GetActive(Coche.Id, evento.Estado.Geocerca.Id);
                                if (ticket != null)
                                {
                                    ticket.Entrada = position.Date;
                                    DaoFactory.TicketMantenimientoDAO.SaveOrUpdate(ticket);
                                }
                            }
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", position.DeviceId, String.Format("Switch evento.Evento/GeocercaEventState.Entra ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                            break;
                        case GeocercaEventState.ExcesoVelocidad:
                            {
                                var inicioExceso = evento.Estado.PosicionInicioExceso;
                                var posi = estado.Posicion;
                                var pico = evento.Estado.VelocidadPico;
                                var max = evento.Estado.VelocidadMaxima;
                                var duracion = position.Date.Subtract(inicioExceso.Date);
                                var descriGeo = evento.Estado.Geocerca.Descripcion;
                                var idGeo = evento.Estado.Geocerca.Id;
                                MessageSaver.Save(_posicion,
                                                  MessageCode.SpeedingTicket.GetMessageCode(),
                                                  Dispositivo,
                                                  Coche,
                                                  null,
                                                  inicioExceso.Date,
                                                  inicioExceso,
                                                  posi,
                                                  string.Format(" {0}km/h - Duracion: {1} - Geocerca {2}",
                                                               pico, duracion, descriGeo),
                                                  max,
                                                  pico,
                                                  idGeo);

                                var infraccion = new Infraccion
                                                     {
                                                         Vehiculo = Coche,
                                                         Alcanzado = evento.Estado.VelocidadPico,
                                                         CodigoInfraccion = Infraccion.Codigos.ExcesoVelocidad,
                                                         Empleado = DaoFactory.EmpleadoDAO.GetLoggedInDriver(Coche),
                                                         Fecha = evento.Estado.PosicionInicioExceso.Date,
                                                         Latitud = evento.Estado.PosicionInicioExceso.Lat,
                                                         Longitud = evento.Estado.PosicionInicioExceso.Lon,
                                                         FechaFin = estado.Posicion.Date,
                                                         LatitudFin = estado.Posicion.Lat,
                                                         LongitudFin = estado.Posicion.Lon,
                                                         Permitido = evento.Estado.VelocidadMaxima,
                                                         Zona = evento.Estado.Geocerca.ZonaManejo > 0? DaoFactory.ZonaDAO.FindById(evento.Estado.Geocerca.ZonaManejo) : null,
                                                         FechaAlta = DateTime.UtcNow
                                                     };

                                DaoFactory.InfraccionDAO.Save(infraccion);
                            }
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", position.DeviceId, String.Format("Switch evento.Evento/GeocercaEventState.ExcesodeVelocidad({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                            break;
                        case GeocercaEventState.TimeTrackingSalida:
                        case GeocercaEventState.TimeTrackingEntrada:
                            DaoFactory.EventoViajeDAO.Save(new EventoViaje
                                {
                                    Vehiculo = Coche,
                                    Empleado = DaoFactory.EmpleadoDAO.GetLoggedInDriver(Coche),
                                    ReferenciaGeografica = DaoFactory.ReferenciaGeograficaDAO.FindById(evento.Estado.Geocerca.Id),
                                    EsEntrada = evento.Evento == GeocercaEventState.TimeTrackingSalida,
                                    EsInicio = evento.Estado.Geocerca.EsInicio,
                                    EsIntermedio = evento.Estado.Geocerca.EsIntermedio,
                                    EsFin = evento.Estado.Geocerca.EsFin,
                                    Fecha = estado.Posicion.Date
                                });
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", position.DeviceId, String.Format("Switch evento.Evento/GeocercaEventState.TimeTrackingEntrada/Salida({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                            break;
                        case GeocercaEventState.ExcesoPermanencia:
                            MessageSaver.Save(_posicion,
                                              MessageCode.PermanenciaEnGeocercaExcedida.GetMessageCode(),
                                              Dispositivo,
                                              Coche,
                                              DaoFactory.EmpleadoDAO.GetLoggedInDriver(Coche),
                                              _posicion.GetDateTime(),
                                              evento.Estado.PosicionInicioExceso,
                                              evento.Estado.Geocerca.Descripcion ?? string.Empty,
                                              evento.Estado.Geocerca.Id);

                            code = MessageCode.PermanenciaEnGeocercaExcedida.GetMessageCode() + '-' + evento.Estado.Geocerca.Id;
                            Coche.StoreLastMessageDate(code, _posicion.GetDateTime());

                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", position.DeviceId, String.Format("Switch evento.Evento/GeocercaEventState.ExcesoPermanencia({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                            break;
                        case GeocercaEventState.ExcesoPermanenciaEntrega:
                            MessageSaver.Save(_posicion,
                                              MessageCode.PermanenciaEnGeocercaExcedidaEnCicloLogistico.GetMessageCode(),
                                              Dispositivo,
                                              Coche,
                                              DaoFactory.EmpleadoDAO.GetLoggedInDriver(Coche),
                                              _posicion.GetDateTime(),
                                              evento.Estado.PosicionInicioExceso,
                                              evento.Estado.Geocerca.Descripcion ?? string.Empty,
                                              evento.Estado.Geocerca.Id);

                            code = MessageCode.PermanenciaEnGeocercaExcedidaEnCicloLogistico.GetMessageCode() + '-' + evento.Estado.Geocerca.Id;
                            Coche.StoreLastMessageDate(code, _posicion.GetDateTime());

                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", position.DeviceId, String.Format("Switch evento.Evento/GeocercaEventState.ExcesoPermanenciaEntrega({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    STrace.Exception(evento.Estado.Estado.ToString(), ex, Coche.Dispositivo.Id);
                }
            }
            ts = t.getTimeElapsed().TotalSeconds;
            if (ts > 0.5) STrace.Error("DispatcherLock", position.DeviceId, String.Format("AnalizeGeoReferences/foreach ({0} secs)", ts));

            t.Restart();
            if (procesarCicloLogisticoFlag && !estado.Eventos.Any())
            {
                var enCurso = GetCiclo(Coche, new MessageSaver(DaoFactory), DaoFactory);
                if (enCurso != null)
                {
                    var e = new PositionEvent(position.Date, position.Lat, position.Lon);
                    enCurso.ProcessEvent(e);
                    ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 0.5) STrace.Error("DispatcherLock", position.DeviceId, String.Format("AnalizeGeoReferences/enCurso.ProcessEvent({0} secs)", ts));
                }
            }
            
            return estado;
        }

        /// <summary>
        /// Determines witherif the current package has online information.
        /// </summary>
        /// <returns></returns>
        private void OnlinePackage()
        {
            //Si la ultima posicion es null es online.
            _fechasOnline = Coche != null && (_lastPosition == null || _posicion.GeoPoints[_posicion.GeoPoints.Count - 1].Date >= _lastPosition.Date);

            if (!_fechasOnline && _lastPosition != null)
                STrace.Error("FechasOnline Dispatcher", string.Format("lastPosition: {0} - actualPosition {1}", _lastPosition.Date, _posicion.GeoPoints[_posicion.GeoPoints.Count-1].Date));
        }        

        /// <summary>
        /// Gets the last well known valid position for the current vehicle.
        /// </summary>
        /// <returns></returns>
        private void GetLastValidPosition() { _lastValidPosition = _fechasOnline ? _lastPosition : GetLastPosition(); }

        /// <summary>
        /// Determines if the givenn dates correspond to different days.
        /// </summary>
        /// <param name="lastPosition"></param>
        /// <param name="refference"></param>
        /// <returns></returns>
        private bool ChangeDay(DateTime lastPosition, DateTime refference)
        {
            var inicio = lastPosition.AddHours(_gmtModifier);
            var fin = refference.AddHours(_gmtModifier);

            return !inicio.DayOfYear.Equals(fin.DayOfYear);
        }

        /// <summary>
        /// Get the culture associated to the vehicle.
        /// </summary>
        /// <returns></returns>
        private TimeZoneInfo GetCulture()
        {
            if (Coche == null) return null;

            try
            {
                var timeZoneId = Coche.Linea != null
                                     ? Coche.Linea.TimeZoneId
									 : Coche.Empresa != null ? Coche.Empresa.TimeZoneId : String.Empty;

				return String.IsNullOrEmpty(timeZoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch(Exception ex)
            {
                STrace.Exception(GetType().FullName, ex);
                return null;
            }
        }

        /// <summary>
        /// Gets the distance in meters btween the givenn positions.
        /// </summary>
        /// <param name="lastPosition"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static double GetDistance(GPSPoint lastPosition, GPSPoint position)
        {
            return Distancias.Loxodromica(lastPosition.Lat, lastPosition.Lon, position.Lat, position.Lon);
        }

        private static double GetHours(GPSPoint lastPosition, GPSPoint position)
        {
            return lastPosition.IgnitionStatus == IgnitionStatus.On || 
                (lastPosition.IgnitionStatus == IgnitionStatus.Unknown && lastPosition.Velocidad > 0)
                ? position.Date.Subtract(lastPosition.Date).TotalHours : 0.0;
        }

        /// <summary>
        /// Adds the last well know position of the current device.
        /// </summary>
        private void GetLastOnlinePosition()
        {
            _lastOnlinePosition = Coche == null ? null : DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(Coche);
            
            _lastPosition = _lastOnlinePosition == null || !_lastOnlinePosition.IdDispositivo.Equals(Dispositivo.Id) ? null : GpsPointFromLastPosition(_lastOnlinePosition);
            
            _informFirstPosition = _lastOnlinePosition == null && Coche != null;
        }

        /// <summary>
        /// Gets a gps point from the last reported position.
        /// </summary>
        /// <param name="lastPosition"></param>
        /// <returns></returns>
        private static GPSPoint GpsPointFromLastPosition(LogPosicionVo lastPosition)
        {
			if (lastPosition == null) return null;
            return new GPSPoint
            {
                Date = lastPosition.FechaMensaje,
                Lat = (float)lastPosition.Latitud,
                Lon = (float)lastPosition.Longitud,
                Speed = new Speed(lastPosition.Velocidad),
                Course = new Course((float)lastPosition.Curso),
                Height = new Altitude((float)lastPosition.Altitud),
                HDOP = lastPosition.HDop ?? 0,
                DeviceId = lastPosition.IdDispositivo,
            };
        }

        /// <summary>
        /// Adds the last well know position of the current device.
        /// </summary>
        private GPSPoint GetLastPosition()
        {
            if (Coche == null) return null;

            var maxMonths = Coche.Empresa != null ? Coche.Empresa.MesesConsultaPosiciones : 3;
            var lastPosition = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(Coche.Id, _posicion.GeoPoints[0].Date, maxMonths);

            return lastPosition == null || !lastPosition.Dispositivo.Id.Equals(Dispositivo.Id) ? null : lastPosition.ToGpsPoint();
        }

    	private static string MakeLastDiscardedPositionReceivedAtKey(int deviceId)
        {
            return "device_" + deviceId + "_lastDiscardedPositionReceivedAt";
        }

        /// <summary>
        /// Saves the givenn position as a discarted one with the specified discart reason code.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="reasonCode"></param>
        private void SaveDiscartedPosition(GPSPoint position, DiscardReason reasonCode)
        {
            var posicionDescartada = new LogPosicionDescartada(position, Dispositivo, Coche, reasonCode);
            DaoFactory.LogPosicionDescartadaDAO.Save(posicionDescartada);

            var key = MakeLastDiscardedPositionReceivedAtKey(Dispositivo.Id);
            LogicCache.Store(key, DateTime.UtcNow.ToString("O"));
        }

        /// <summary>
        /// Discrds from the message the position with dates into the future or with invalid positions.
        /// </summary>
        /// <param name="position"></param>
        private Boolean DiscardInvalidPosition(GPSPoint position)
        {
            if (!DeviceParameters.DiscardsInvalidPositions) return false;

            position.Speed = new Speed(Convert.ToSingle(Convert.ToInt32(position.Speed.Unpack())));

            if (Coche == null) SaveDiscartedPosition(position, DiscardReason.NoAssignedMobile);
                
            else if (InvalidHdop(position)) SaveDiscartedPosition(position, DiscardReason.LowQualitySignal);
            //else if (InvalidHdop2(position)) SaveDiscartedPosition(position, DiscardReason.LowerQualitySignalOnSpeed0);
			else if (FechaInvalida(position.Date, DeviceParameters)) SaveDiscartedPosition(position, DiscardReason.InvalidDate);
            else if (FueraDelGlobo(position)) SaveDiscartedPosition(position, DiscardReason.OutOfGlobe);
            else if (VelocidadInvalida(position.Velocidad)) SaveDiscartedPosition(position, DiscardReason.InvalidSpeed);
            else if (!EsPosicionValida(_lastValidPosition, position, Coche.TipoCoche.MaximaVelocidadAlcanzable)) SaveDiscartedPosition(position, DiscardReason.InvalidDistance);
            else if (EsPosicionDuplicada(_lastValidPosition, position)) SaveDiscartedPosition(position, DiscardReason.DuplicatedPosition);
            else return false;

            return true;
        }

        /// <summary>
        /// Determines if the position has a invalid hdop value.
        /// </summary>
        /// <param name="posicion"></param>
        /// <returns></returns>
        private bool InvalidHdop(GPSPoint posicion)
        {
            var result = DeviceParameters.InformsHdop && posicion.HDOP > DeviceParameters.MaxHdop;
            return result;
        }
        
        /// <summary>
        /// Determines if the given speed is a valid one. Invalid speeds are those smaller than 0.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private bool VelocidadInvalida(int speed)
        {
            var velocidadNegativa = speed < 0;
            var velocidadMuyGrande = Coche != null && speed > Coche.TipoCoche.MaximaVelocidadAlcanzable;

            return velocidadNegativa || velocidadMuyGrande;
        }

        /// <summary>
        /// Determines if the given b position came duplicated (comparing to a).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool EsPosicionDuplicada(GPSPoint a, GPSPoint b)
        {
            return false; /* TODO:.... */
        }

        /// <summary>
        /// Determines if the given b position is valid taking as refference the a position.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="velocidadMaximaAlcanzable"></param>
        /// <returns></returns>
        private bool EsPosicionValida(GPSPoint a, GPSPoint b, int velocidadMaximaAlcanzable)
        {
            if (a == null || b == null) return true;

            //Calculo el tiempo entre posiciones, la distancia y la maxima distancia alcanzable por el movil en ese intervalo de tiempo.
            var tiempo = b.Date.Subtract(a.Date).TotalSeconds;
            var distancia = Distancias.Loxodromica(a.Lat, a.Lon, b.Lat, b.Lon);
            var maximaDistanciaTipoVehiculo = (velocidadMaximaAlcanzable * tiempo) / 3.6;

            return distancia <= maximaDistanciaTipoVehiculo;
        }
        
        /// <summary>
        /// Starts a stopped event for the current vehicle.
        /// </summary>
        /// <param name="posicion"></param>
        private void StartStoppedEvent(GPSPoint posicion)
        {
            var startPosition = new LogPosicion(posicion, Coche); 
            Coche.StartStoppedEvent(new LogPosicionVo(startPosition));
        }

        private void UpdateStopInitialPosition(GPSPoint posicion)
        {
            var start = GetStoppedEventStart();
            if (start == null) return;

            start.Lat = posicion.Lat;
            start.Lon = posicion.Lon;

            var startPosition = new LogPosicion(start, Coche);
            Coche.StartStoppedEvent(new LogPosicionVo(startPosition));
        }

        /// <summary>
        /// Generates a stopped event if the conditions are fullfiled.
        /// </summary>
        /// <param name="posicion"></param>
        private void EndStoppedEvent(GPSPoint posicion, EstadoVehiculo estado)
        {
            var start = GetStoppedEventStart();

			if (start == null) return;

            var duration = posicion.Date.Subtract(start.Date);

            if (duration.TotalSeconds < DeviceParameters.StoppedEventTime) return;

            var texto = String.Format(" {0} - Duración: {1}", GeocoderHelper.GetDescripcionEsquinaMasCercana(start.Lat, start.Lon), duration);

            //No se que poner como texto, no tengo el chofer y tampoco el id de geocerca. Las velocidades no aplican.

            int? idReferenciaGeografica = null;
            var salidas = estado.Eventos.Where(e => e.Evento == GeocercaEventState.Sale);
            if (salidas.Any())
                idReferenciaGeografica = salidas.First().Estado.Geocerca.Id;
            else if (estado.GeocercasDentro.Any())
                idReferenciaGeografica = estado.GeocercasDentro.First().Geocerca.Id;

            MessageSaver.Save(_posicion, MessageCode.StoppedEvent.GetMessageCode(), Dispositivo, Coche, DaoFactory.EmpleadoDAO.GetLoggedInDriver(Coche), start.Date, start, posicion, texto, null, null, idReferenciaGeografica);
        }

        /// <summary>
        /// Gets the start time for the current stopped event.
        /// </summary>
        /// <returns></returns>
        private GPSPoint GetStoppedEventStart()
        {
            var start = Coche.GetStoppedEventStart();

            Coche.EndStoppedEvent();

            return GpsPointFromLastPosition(start);
        }

        /// <summary>
        /// Saves a movent outside shift event and updates the mobile last raise refference value.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="shift"></param>
        private void SaveShiftEvent(GPSPoint position, Shift shift)
        {
            if (shift == null) return;

            var lastShiftEvent = DaoFactory.LogMensajeDAO.GetLastMessageDate(Coche, MessageCode.OutOfShiftActivity.GetMessageCode());

            var lastShiftEnd = shift.GetLastShiftEnd(position.Date, _culture);

            if (lastShiftEvent.HasValue && lastShiftEvent.Value >= lastShiftEnd) return;

            Coche.DeleteLastMessageDate(MessageCode.OutOfShiftActivity.GetMessageCode());

			MessageSaver.Save(_posicion, MessageCode.OutOfShiftActivity.GetMessageCode(), Dispositivo, Coche, null, _posicion.GetDateTime(), position, String.Empty);
        }

        /// <summary>
        /// Updates all associated odometers.
        /// </summary>
        /// <param name="kilometers"></param>
        /// <param name="lastPosition"></param>
        /// <param name="hours"></param>
        private void UpdateVehicleOdometers(Double kilometers, GPSPoint lastPosition, Double hours)
        {
            if (Coche.Odometros.Count.Equals(0)) return;

            foreach (MovOdometroVehiculo odometro in Coche.Odometros)
            {
                UpdateOdometerValues(odometro, lastPosition, kilometers, hours);

                if (odometro.Vencido() && !odometro.UltimoDisparo.HasValue)
                {
                    odometro.UltimoDisparo = lastPosition.Date;
                    if (odometro.Odometro.EsIterativo) odometro.ResetOdometerValues();
					MessageSaver.Save(_posicion, MessageCode.OdometerExpired.GetMessageCode(), Dispositivo, Coche, null, _posicion.GetDateTime(), lastPosition, odometro.Odometro.Descripcion);
                }
                else if (odometro.SuperoSegundoAviso() && !odometro.FechaSegundoAviso.HasValue)
                {
					odometro.FechaSegundoAviso = _posicion.GetDateTime();
					MessageSaver.Save(_posicion, MessageCode.OdometerSecondWarning.GetMessageCode(), Dispositivo, Coche, null, _posicion.GetDateTime(), lastPosition, odometro.Odometro.Descripcion);
                }
                else if (odometro.SuperoPrimerAviso() && !odometro.FechaPrimerAviso.HasValue)
                {
					odometro.FechaPrimerAviso = _posicion.GetDateTime();
					MessageSaver.Save(_posicion, MessageCode.OdometerFirstWarning.GetMessageCode(), Dispositivo, Coche, null, _posicion.GetDateTime(), lastPosition, odometro.Odometro.Descripcion);
                }
            }
        }

        /// <summary>
        /// Updates odometer days, kilometers and last update refference.
        /// </summary>
        /// <param name="odometro"></param>
        /// <param name="lastPosition"></param>
        /// <param name="kilometers"></param>
        /// <param name="hours"></param>
        private void UpdateOdometerValues(MovOdometroVehiculo odometro, GPSPoint lastPosition, double kilometers, double hours)
        {
            UpdateOdometerDays(odometro, lastPosition);

            UpdateOdometerKilometers(odometro, kilometers);

            UpdateOdometerHours(odometro, hours);

            odometro.UltimoUpdate = lastPosition.Date;
        }

        /// <summary>
        /// Updates odometer kilometers value.
        /// </summary>
        /// <param name="odometro"></param>
        /// <param name="kilometers"></param>
        private void UpdateOdometerKilometers(MovOdometroVehiculo odometro, double kilometers)
        {
            if (odometro.Odometro.PorKm)
            {
                if (odometro.Odometro.EsIterativo) odometro.Kilometros += kilometers;
                else odometro.Kilometros = Coche.TotalOdometer;
            }
            else odometro.Kilometros = 0;
        }

        /// <summary>
        /// Updates odometer days value.
        /// </summary>
        /// <param name="odometro"></param>
        /// <param name="lastPosition"></param>
        private void UpdateOdometerDays(MovOdometroVehiculo odometro, GPSPoint lastPosition)
        {
            if (odometro.Odometro.PorTiempo) { if (!odometro.UltimoUpdate.HasValue || ChangeDay(odometro.UltimoUpdate.Value, lastPosition.Date)) odometro.Dias++; }
            else odometro.Dias = 0;
        }

        /// <summary>
        /// Updates odometer hours value.
        /// </summary>
        /// <param name="odometro"></param>
        /// <param name="hours"></param>
        private void UpdateOdometerHours(MovOdometroVehiculo odometro, double hours)
        {
            if (odometro.Odometro.PorHoras) odometro.Horas += hours;
            else odometro.Horas = 0;
        }

        /// <summary>
        /// Checks if the daily odometer value reaches its refference value.
        /// </summary>
        /// <param name="lastPosition"></param>
        private void CheckDailyOdometer(GPSPoint lastPosition)
        {
            var refference = Coche.KilometrosDiarios.Equals(0) ? Coche.TipoCoche.KilometrosDiarios : Coche.KilometrosDiarios;

            if (refference.Equals(0)) return;

            if (Coche.DailyOdometer < refference) return;

            if (Coche.LastDailyOdometerRaise.HasValue && !ChangeDay(Coche.LastDailyOdometerRaise.Value, lastPosition.Date)) return;

            var text = String.Format(" (Referencia: {0}km)", (Int32)refference);

			MessageSaver.Save(_posicion, MessageCode.KilometersExceded.GetMessageCode(), Dispositivo, Coche, null, _posicion.GetDateTime(), lastPosition, text);

            Coche.LastDailyOdometerRaise = lastPosition.Date;
        }

        /// <summary>
        /// Updates the odometers of the specified vehicle in the givenn amount of kilometers.
        /// </summary>
        /// <param name="kilometers"></param>
        /// <param name="lastPosition"></param>
        /// <param name="resetDailyOdometer"></param>
        /// <param name="hours"></param>
        private void UpdateOdometers(Double kilometers, GPSPoint lastPosition, Boolean resetDailyOdometer, Double hours)
        {
            if (resetDailyOdometer) Coche.DailyOdometer = kilometers;
            else Coche.DailyOdometer += kilometers;

            Coche.ApplicationOdometer += kilometers;
            Coche.PartialOdometer += kilometers;

            Coche.LastOdometerUpdate = lastPosition.Date;

            UpdateVehicleOdometers(kilometers, lastPosition, hours);

            if (Coche.TipoCoche.ControlaKilometraje) CheckDailyOdometer(lastPosition);

            DaoFactory.CocheDAO.Update(Coche);
        }

        private static ICicloLogistico GetCiclo(Coche coche, IMessageSaver messageSaver, DAOFactory daoFactory)
        {
            if (coche.Dispositivo == null) return null;

            var distribucion = daoFactory.ViajeDistribucionDAO.FindEnCurso(coche);

            if (distribucion != null) return new CicloLogisticoDistribucion(distribucion, daoFactory, messageSaver);

            var ticket = daoFactory.TicketDAO.FindEnCurso(coche.Dispositivo);

            if (ticket != null) return new CicloLogisticoHormigon(ticket, daoFactory, messageSaver);

            return null;
        }

        #endregion
    }
}
