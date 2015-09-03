using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Messaging;
using Logictracker.AVL.Messages;
using Logictracker.Cache;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.ExpressionEvaluator;
using Logictracker.ExpressionEvaluator.Contexts;
using Logictracker.Layers.MessageQueue;
using Logictracker.Mailing;
using Logictracker.Messages.Saver.BaseClasses;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Process.Geofences;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.ValueObject.Messages;
using Logictracker.Utils;
using Logictracker.Culture;
using Logictracker.Reports.Messaging;

namespace Logictracker.Messages.Saver
{
    public class MessageSaver: IMessageSaver
    {
        protected DAOFactory DaoFactory { get; private set; }

        public MessageSaver(DAOFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        #region Save Methods
        public LogMensajeBase Save(string codigo, Coche coche, DateTime fecha, GPSPoint inicio, string texto)
        {
            return Save(null, codigo, coche != null && coche.Dispositivo != null ? coche.Dispositivo : null, coche, null, fecha, inicio, null, texto, null, null, null);
        }
        public LogMensajeBase Save(string codigo, Coche coche, Empleado empleado, DateTime fecha, GPSPoint inicio, string texto)
        {
            return Save(null, codigo, coche != null && coche.Dispositivo != null ? coche.Dispositivo : null, coche, empleado, fecha, inicio, null, texto, null, null, null);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, null, texto, null, null, null);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto, ViajeDistribucion viaje, EntregaDistribucion entrega)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, null, texto, null, null, null, null, viaje, entrega);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto, Zona zonaManejo)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, null, texto, null, null, null, zonaManejo);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, fin, texto, null, null, null);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int velPermitida, int velAlcanzada)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, fin, texto, velPermitida, velAlcanzada, null);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, string texto, int idReferenciaGeografica)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, null, texto, null, null, idReferenciaGeografica);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int? velPermitida, int? velAlcanzada, int? idReferenciaGeografica)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, fin, texto, velPermitida, velAlcanzada, idReferenciaGeografica, null);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int? velPermitida, int? velAlcanzada, int? idReferenciaGeografica, Zona zonaManejo)
        {
            return Save(evento, codigo, dispositivo, coche, chofer, fecha, inicio, fin, texto, velPermitida, velAlcanzada, idReferenciaGeografica, zonaManejo, null, null);
        }
        public LogMensajeBase Save(IMessage evento, string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, string texto, int? velPermitida, int? velAlcanzada, int? idReferenciaGeografica, Zona zonaManejo, ViajeDistribucion viaje, EntregaDistribucion entrega)
        {
            try
            {
                var device = dispositivo ?? DaoFactory.DispositivoDAO.GetGenericDevice(coche.Empresa);
                var driver = chofer ?? (coche != null && !coche.IdentificaChoferes ? coche.Chofer : null);
                
                if (string.IsNullOrEmpty(codigo.Trim())) return null;
                var mensaje = DaoFactory.MensajeDAO.GetByCodigo(codigo, coche != null ? coche.Empresa : null, coche != null ? coche.Linea : null);
                if (mensaje == null) return DiscardDueToInvalidMessage(codigo, coche, inicio, device, fin, driver, fecha);
                
                var ticket = DaoFactory.TicketDAO.FindEnCurso(dispositivo);
                var detalleTicket = ticket == null ? null : ticket.GetDetalleProximo();
                driver = driver ?? (ticket != null ? ticket.Empleado : null);

                if (mensaje.Acceso >= Usuario.NivelAcceso.SysAdmin)
                {
                    return null;

                    //var log = new LogMensajeAdmin
                    //{
                    //    Chofer = driver,
                    //    Coche = coche,
                    //    Dispositivo = device,
                    //    Estado = 0,
                    //    Fecha = fecha,
                    //    FechaAlta = DateTime.UtcNow,
                    //    Expiracion = DateTime.UtcNow.AddDays(1),
                    //    Horario = ticket,
                    //    DetalleHorario = detalleTicket,
                    //    Usuario = null,
                    //    Latitud = inicio != null ? inicio.Lat : 0,
                    //    Longitud = inicio != null ? inicio.Lon : 0,
                    //    FechaFin = fin != null ? fin.Date : (DateTime?)null,
                    //    LatitudFin = fin != null ? new Double?(fin.Lat) : null,
                    //    LongitudFin = fin != null ? new Double?(fin.Lon) : null,
                    //    VelocidadAlcanzada = velAlcanzada,
                    //    VelocidadPermitida = velPermitida,
                    //    IdPuntoDeInteres = idReferenciaGeografica,
                    //    Mensaje = DaoFactory.MensajeDAO.FindById(mensaje.Id),
                    //    Texto = String.Concat(mensaje.Texto, ' ', texto),
                    //    TieneFoto = codigo == ((int)MessageIdentifier.Picture).ToString(CultureInfo.InvariantCulture)
                    //};

                    //ProcessActions(log);

                    //return log;
                }
                else
                {
                    var log = new LogMensaje
                    {
                        Chofer = driver,
                        Coche = coche,
                        Dispositivo = device,
                        Estado = 0,
                        Fecha = fecha,
                        FechaAlta = DateTime.UtcNow,
                        Expiracion = DateTime.UtcNow.AddDays(1),
                        Horario = ticket,
                        DetalleHorario = detalleTicket,
                        Usuario = null,
                        Latitud = inicio != null ? inicio.Lat : 0,
                        Longitud = inicio != null ? inicio.Lon : 0,
                        FechaFin = fin != null ? fin.Date : (DateTime?) null,
                        LatitudFin = fin != null ? new Double?(fin.Lat) : null,
                        LongitudFin = fin != null ? new Double?(fin.Lon) : null,
                        VelocidadAlcanzada = velAlcanzada,
                        VelocidadPermitida = velPermitida,
                        IdPuntoDeInteres = idReferenciaGeografica,
                        Mensaje = DaoFactory.MensajeDAO.FindById(mensaje.Id),
                        Texto = String.Concat(mensaje.Texto, ' ', texto),
                        TieneFoto = codigo == ((int) MessageIdentifier.Picture).ToString(CultureInfo.InvariantCulture),
                        Viaje = viaje,
                        Entrega = entrega
                    };

                    ProcessActions(log);

                    if (MessageIdentifierX.IsEngineOnOffEvent(log.Mensaje))
                        DaoFactory.LastVehicleEventDAO.Save(log, Coche.Totalizador.EstadoMotor);
                    else if (MessageIdentifierX.IsGarminOnOffEvent(log.Mensaje))
                        DaoFactory.LastVehicleEventDAO.Save(log, Coche.Totalizador.EstadoGarmin);
                    else if (MessageIdentifierX.IsPrivacyOnOffEvent(log.Mensaje))
                        DaoFactory.LastVehicleEventDAO.Save(log, Coche.Totalizador.EstadoGps);

                    return log;
                }
            }
            catch (Exception ex)
            {
                STrace.Exception(GetType().FullName, ex);

                return null;
            }
        } 
        #endregion

        #region Eventos Descartados
        private bool DiscartInhibitor(MensajeVO mensaje, Coche coche, double latitude, double longitude)
        {
            if (mensaje == null || coche == null) return true;

            return mensaje.EsAlarma && GetInhibidores(coche).Any(domicilio => IsInsideInhibitor(latitude, longitude, domicilio));
        }
        private IEnumerable<ReferenciaGeografica> GetInhibidores(Coche coche)
        {
            var empresa = coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1;
            var linea = coche.Linea != null ? coche.Linea.Id : -1;

            return DaoFactory.ReferenciaGeograficaDAO.FindInhibidores(empresa, linea);
        }
        private static bool IsInsideInhibitor(double latitude, double longitude, ReferenciaGeografica domicilio)
        {
            if (latitude.Equals(0) || longitude.Equals(0) || domicilio == null || domicilio.Poligono == null) return false;

            return domicilio.Poligono.Contains(latitude, longitude);
        }
        private LogMensajeBase DiscardDueToInvalidMessage(String code, Coche coche, GPSPoint inicio, Dispositivo device, GPSPoint fin, Empleado driver, DateTime dt)
        {
            if (coche != null)
            {
                STrace.Error(GetType().FullName, coche.Dispositivo.Id, String.Format("A message with code {0} was not found for the vehicle {1} (id={2}).", code, coche.Interno, coche.Id));
            }
            else
            {
                STrace.Error(GetType().FullName, device.Id, String.Format("A message with code {0} was not found.", code));
            }

            DiscardEvent(null, device, coche, driver, dt, inicio, fin, DiscardReason.NoMessageFound, code);

            return null;
        }
        private LogMensajeBase DiscardDueToInhibitor(Coche coche, GPSPoint inicio, MensajeVO mensaje, Dispositivo device, GPSPoint fin, Empleado driver, DateTime dt)
        {
            DiscardEvent(mensaje, device, coche, driver, dt, inicio, fin, DiscardReason.InsideInhibitor, string.Empty);
            return null;
        }

        public void Discard(string codigo, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, DiscardReason discardReason)
        {
            var mensaje = DaoFactory.MensajeDAO.GetByCodigo(codigo, coche != null ? coche.Empresa : null, coche != null ? coche.Linea : null);
            DiscardEvent(mensaje, dispositivo, coche, chofer, fecha, inicio, fin, discardReason, codigo);
        }

        protected void DiscardEvent(MensajeVO mensaje, Dispositivo dispositivo, Coche coche, Empleado chofer, DateTime fecha, GPSPoint inicio, GPSPoint fin, DiscardReason discardReason, string codigo)
        {
			STrace.Debug(GetType().FullName, dispositivo.GetId(), String.Format(
				"Descartando: mensaje={0} coche={1} chofer={2} fecha={3} inicio={4} fin={5} discardReason={6}",
				mensaje,
				coche,
				chofer,
				fecha,
				inicio,
				fin,
				discardReason));
            var log = new LogMensajeDescartado
            {
                Chofer = chofer,
                Coche = coche,
                Dispositivo = dispositivo,
                Fecha = inicio != null ? inicio.Date : fecha,
                Expiracion = DateTime.UtcNow.AddDays(1),
                Latitud = inicio != null ? inicio.Lat : 0,
                Longitud = inicio != null ? inicio.Lon : 0,
                FechaFin = fin != null ? fin.Date : (DateTime?)null,
                LatitudFin = fin != null ? new Double?(fin.Lat) : null,
                LongitudFin = fin != null ? new Double?(fin.Lon) : null,
                Mensaje = mensaje != null ? DaoFactory.MensajeDAO.FindById(mensaje.Id) : null,
                Texto = mensaje != null ? mensaje.Texto : codigo,
				MotivoDescarte = (int)discardReason,
            };

            DaoFactory.LogMensajeDescartadoDAO.Save(log);
        } 
        #endregion
        
        #region Action Filtering
        /// <summary>
        /// Performs all actions associated to the event and saves it into database.
        /// </summary>
        /// <param name="log"></param>
        private void ProcessActions(LogMensaje log)
        {
            var appliesToAnyAction = false;

            var t = new TimeElapsed();
            var actions = DaoFactory.AccionDAO.FindByMensaje(log.Mensaje);
            var totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/FindByMensaje ({0} secs)", totalSeconds));

            t.Restart();
            var filteredActions = FilterActions(actions, log);
            totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/FilterActions ({0} secs)", totalSeconds));

            t.Restart();
            foreach (var accion in filteredActions)
            {
                appliesToAnyAction = true;

                DaoFactory.RemoveFromSession(log);

                log.Id = 0;
                log.Accion = accion;

                if (accion.CambiaMensaje) log.Texto += string.Concat(" ", accion.MensajeACambiar);
                if (accion.PideFoto) PedirFoto(log);
                if (accion.GrabaEnBase) GuardarEvento(log);
                if (accion.EsAlarmaDeMail) SendMail(log);
                if (accion.EsAlarmaSms) SendSms(log);
                if (accion.Habilita) HabilitarUsuario(log.Accion);
                if (accion.Inhabilita) InhabilitarUsuario(log.Accion);
                if (accion.ReportarAssistCargo) ReportarAssistCargo(log, accion.CodigoAssistCargo);
                if (accion.EnviaReporte) EnviarReporte(log);
            }
            totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/foreach ({0} secs)", totalSeconds));

            t.Restart();
            if (!appliesToAnyAction) GuardarEvento(log);
            totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/GuardarEvento ({0} secs)", totalSeconds));
        }

        private void ProcessActions(LogMensajeAdmin log)
        {
            var appliesToAnyAction = false;

            var t = new TimeElapsed();
            var actions = DaoFactory.AccionDAO.FindByMensaje(log.Mensaje);
            var totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/FindByMensaje ({0} secs)", totalSeconds));

            t.Restart();
            var filteredActions = FilterActions(actions, log);
            totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/FilterActions ({0} secs)", totalSeconds));

            t.Restart();
            foreach (var accion in filteredActions)
            {
                appliesToAnyAction = true;

                DaoFactory.RemoveFromSession(log);

                log.Id = 0;
                log.Accion = accion;

                if (accion.CambiaMensaje) log.Texto += string.Concat(" ", accion.MensajeACambiar);
                if (accion.PideFoto) PedirFoto(log);
                if (accion.GrabaEnBase) GuardarEvento(log);
                if (accion.EsAlarmaDeMail) SendMail(log);
                if (accion.EsAlarmaSms) SendSms(log);
                if (accion.Habilita) HabilitarUsuario(log.Accion);
                if (accion.Inhabilita) InhabilitarUsuario(log.Accion);
                if (accion.ReportarAssistCargo) ReportarAssistCargo(log, accion.CodigoAssistCargo);
            }
            totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/foreach ({0} secs)", totalSeconds));

            t.Restart();
            if (!appliesToAnyAction) GuardarEvento(log);
            totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("ProcessActions/GuardarEvento ({0} secs)", totalSeconds));
        }

        private void GuardarEvento(LogMensaje log)
        {
            var t = new TimeElapsed();
            DaoFactory.LogMensajeDAO.Save(log);
            var totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("GuardarEvento/LogMensajeDAO.Save ({0} secs)", totalSeconds));

            if (log.Viaje == null && log.Entrega == null) return;

            t.Restart();
            var evenDistri = new EvenDistri
                                 {
                                     LogMensaje = log,
                                     Fecha = log.Fecha,
                                     Entrega = log.Entrega,
                                     Viaje = log.Viaje ?? log.Entrega.Viaje
                                 };

            DaoFactory.EvenDistriDAO.Save(evenDistri);

            totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("GuardarEvento/EvenDistriDAO.Save ({0} secs)", totalSeconds));
        }

        private void GuardarEvento(LogMensajeAdmin log)
        {
            var t = new TimeElapsed();
            DaoFactory.LogMensajeAdminDAO.Save(log);
            var totalSeconds = t.getTimeElapsed().TotalSeconds;
            if (totalSeconds > 1)
                STrace.Debug("DispatcherLock", log.Dispositivo.Id, String.Format("GuardarEvento/LogMensajeAdminDAO.Save ({0} secs)", totalSeconds));
        }

        private IEnumerable<Accion> FilterActions(ICollection<Accion> actions, LogMensajeBase mensaje)
        {
            if (actions == null || actions.Count.Equals(0)) return new List<Accion>();

            var actionsToCheck = from action in actions.ToList()
                                 where mensaje.Coche != null
                                 where action.Empresa == null || mensaje.Coche.Empresa == null || action.Empresa.Id.Equals(mensaje.Coche.Empresa.Id)
                                 where action.Linea == null || mensaje.Coche.Linea == null || action.Linea.Id.Equals(mensaje.Coche.Linea.Id)
                                 where action.TipoVehiculo == null || action.TipoVehiculo.Id.Equals(mensaje.Coche.TipoCoche.Id)
                                 where action.Transportista == null || (mensaje.Coche.Transportista != null && action.Transportista.Id.Equals(mensaje.Coche.Transportista.Id)) || (mensaje.Chofer != null && mensaje.Chofer.Transportista != null && action.Transportista.Id.Equals(mensaje.Chofer.Transportista.Id))
                                 where action.Departamento == null || (mensaje.Coche.CentroDeCostos != null && mensaje.Coche.CentroDeCostos.Departamento != null && action.Departamento.Id.Equals(mensaje.Coche.CentroDeCostos.Departamento.Id))
                                 where action.CentroDeCostos == null || (mensaje.Coche.CentroDeCostos != null && action.CentroDeCostos.Id.Equals(mensaje.Coche.CentroDeCostos.Id))
                                 select action;

            var actionsToApply = actionsToCheck.Where(action => ApplyAction(mensaje, action));

            var filteredActions = new List<Accion>(actionsToCheck.Count());

            var poi = mensaje.IdPuntoDeInteres.HasValue ? DaoFactory.ReferenciaGeograficaDAO.FindById(mensaje.IdPuntoDeInteres.Value) : null;

            filteredActions.AddRange(poi != null
                ? actionsToApply.Where(action => action.TipoReferenciaGeografica == null || action.TipoReferenciaGeografica.Id.Equals(poi.TipoReferenciaGeografica.Id))
                : actionsToApply);
            if (filteredActions.Any(accion => accion.EvaluaGeocerca) && mensaje.Coche != null)
            {
                filteredActions = filteredActions.Where(accion => CheckGeofence(mensaje.Coche, accion, DaoFactory)).ToList();
            }

            return filteredActions;
        }

	    private static Boolean ApplyAction(LogMensajeBase log, Accion accion)
        {
            if (accion == null || accion.Condicion == null || accion.Condicion.Trim() == string.Empty) return true;

            try
            {
                var context = new EventContext
                {
                    Dispositivo = log.Dispositivo.Codigo,
                    Duracion = log.Duracion,
                    Exceso = log.Exceso,
                    Interno = log.Coche.Interno,
                    Legajo = log.Chofer != null ? log.Chofer.Legajo : string.Empty,
                    Texto = log.Texto,
                    TieneTicket = log.Horario != null,
                    VelocidadAlcanzada = log.VelocidadAlcanzada.HasValue ? log.VelocidadAlcanzada.Value : -1,
                    VelocidadPermitida = log.VelocidadPermitida.HasValue ? log.VelocidadPermitida.Value : -1,
                    Fecha = log.Fecha,
                    FechaFin = log.FechaFin
                };

                var expression = ExpressionContext.CreateExpression(accion.Condicion, context);
                var cachedResult = LogicCache.Retrieve<Object>(typeof(Boolean), expression);
                if (cachedResult != null) return Convert.ToBoolean(cachedResult);
                var result = Logictracker.ExpressionEvaluator.ExpressionEvaluator.Evaluate<bool>(expression);
                LogicCache.Store(typeof(Boolean), expression, result);
                return result;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(BaseEventSaver).FullName, e, String.Format("Error procesando condicion: {0} | Accion: {1}", accion.Condicion, accion.Descripcion));
                return false;
            }
        }

        private static bool CheckGeofence(Coche vehiculo, Accion accion, DAOFactory daoFactory)
        {
            if (!accion.EvaluaGeocerca) return true;

            var estados = GeocercaManager.GetEstadoVehiculo(vehiculo, daoFactory);
            var dentroDeAlguna = estados.GeocercasDentro.Any(geo => geo.Geocerca.TipoReferenciaGeograficaId == accion.TipoGeocerca.Id);

            return (accion.DentroGeocerca && dentroDeAlguna) || (!accion.DentroGeocerca && !dentroDeAlguna);
        }
        #endregion

        #region Execute Actions
        /// <summary>
        /// Sends a SMS with info about the event.
        /// </summary>
        /// <param name="log"></param>
        private static void SendSms(LogMensajeBase log)
        {
            var parameters = log.Coche != null
                                 ? new List<string>
                                       {
                                           log.Coche.Linea != null? log.Coche.Linea.Descripcion : log.Coche.Empresa != null ? log.Coche.Empresa.RazonSocial : "Sistema",
                                           log.Coche.Interno,
                                           log.Coche.ToLocalString(log.Fecha, false),
                                           log.Texto,
                                           GeocoderHelper.GetDescripcionEsquinaMasCercana(log.Latitud, log.Longitud),
                                           log.Coche.Chofer != null ? log.Coche.Chofer.Entidad.Descripcion : string.Empty
                                       }
                                 : new List<string>
                                       {
                                           "Sistema",
                                           "(Ninguno)",
                                           string.Format("{0} {1}", log.Fecha.ToShortDateString(),log.Fecha.ToShortTimeString()),
                                           log.Texto,
                                           GeocoderHelper.GetDescripcionEsquinaMasCercana(log.Latitud, log.Longitud),
                                           string.Empty
                                       };

            SendSms(log.Accion.DestinatariosSms, parameters);
        }
        protected static void SendSms(string destinatarios, List<string> parameters)
        {
            var configFile = Config.Mailing.MailingSmsConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de envio de sms.");

            var sender = new MailSender(configFile);

            SendSmsToAllDestinations(destinatarios, sender, parameters);
        }
        
        /// <summary>
        /// Enables the user
        /// </summary>
        /// <param name="accion"></param>
        protected void HabilitarUsuario(Accion accion)
        {
            DaoFactory.UsuarioDAO.HabilitarUsuario(accion.UsuarioHabilitado, accion.HorasHabilitado);
        }

        /// <summary>
        /// Disables the user
        /// </summary>
        /// <param name="accion"></param>
        protected void InhabilitarUsuario(Accion accion)
        {
            DaoFactory.UsuarioDAO.InhabilitarUsuario(accion.UsuarioInhabilitado);
        }

	    private void ReportarAssistCargo(LogMensajeBase log, string assistCargoCode)
        {
            try
            {
                if (!log.Coche.ReportaAssistCargo) return;

                STrace.Debug(GetType().FullName, log.Dispositivo.Id, String.Format("AssistCargo Event: {0} -> {1}: {2}", log.Coche.Patente, Config.AssistCargo.AssistCargoEventQueue, assistCargoCode));

                var queue = new IMessageQueue(Config.AssistCargo.AssistCargoEventQueue);

                if (queue.LoadResources())
                {
                    var data = new TextEvent(log.Dispositivo.Id, 0, DateTime.UtcNow)
                    {
                        Text = assistCargoCode,
                        GeoPoint = new GPSPoint(log.Fecha, (float)log.Latitud, (float)log.Longitud)
                    };
                    queue.Send(data, MessageQueueTransactionType.Automatic);
                }
                else
                {
                    STrace.Error(GetType().FullName, log.Dispositivo.Id, String.Format("Problemas cargando la cola: {0}", Config.AssistCargo.AssistCargoEventQueue));
                }
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e, log.Dispositivo.Id);
            }
        }

	    private void PedirFoto(LogMensajeBase log)
        {
            log.TieneFoto = true;

            DateTime from;
            DateTime to;
            if (log.FechaFin.HasValue)
            {
                to = log.FechaFin.Value;
                from = log.FechaFin.Value.AddSeconds(-log.Accion.SegundosFoto);
                if (log.Fecha > from) from = log.Fecha;
            }
            else
            {
                to = log.Fecha;
                from = log.Fecha.AddSeconds(-log.Accion.SegundosFoto);
            }

            MessageSender.CreateRetrievePictures(log.Dispositivo, this).AddDateRange(from, to).Send();
        }

        /// <summary>
        /// Sends a SMS message to all recipients.
        /// </summary>
        /// <param name="destinatarios"></param>
        /// <param name="sender"></param>
        /// <param name="parameters"></param>
        private static void SendSmsToAllDestinations(string destinatarios, MailSender sender, List<string> parameters)
        {
            if (string.IsNullOrEmpty(destinatarios)) return;

            var destinatariosSms = destinatarios.Replace(',', ';');

            var destinos = destinatariosSms.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos.Where(destinatario => !string.IsNullOrEmpty(destinatario)))
            {
                sender.Config.ToAddress = destinatario.Trim();

                sender.SendMail(parameters.ToArray());
            }
        }

	    private static void SendMail(LogMensajeBase log)
        {
            const int minutes = 15;
            var monitor = Config.Monitor.HistoricMonitorLink;
            var link = log.Coche != null
                ? String.Format(@"{0}?Planta={1}&TypeMobile={2}&Movil={3}&InitialDate={4}&FinalDate={5}&MessageCenterIndex={6}&ShowMessages=1&Empresa={7}&ShowPOIS=1",
                monitor,
                log.Coche.Linea != null ? log.Coche.Linea.Id : -1,
                log.Coche.TipoCoche.Id, log.Coche.Id,
                log.Fecha.AddMinutes(-minutes).ToString(CultureInfo.InvariantCulture),
                log.Fecha.AddMinutes(minutes).ToString(CultureInfo.InvariantCulture),
                log.Id,
                log.Coche.Empresa != null ? log.Coche.Empresa.Id : log.Coche.Linea != null ? log.Coche.Linea.Empresa.Id : -1)
                : monitor;

            var chofer = log.Chofer != null ? string.Format("Chofer: {0}", log.Chofer.Entidad.Descripcion) : "Sin Chofer Identificado";

            var responsable = log.Coche != null && log.Coche.Chofer != null && log.Coche.Chofer.Entidad != null ? log.Coche.Chofer.Entidad.Descripcion : "Sin Responsable Asignado";

            var parameters = log.Coche != null
                                 ? new List<string>
                                       {
                                           log.Coche.Linea != null ? log.Coche.Linea.Descripcion : log.Coche.Empresa != null ? log.Coche.Empresa.RazonSocial : "Sistema",
                                           string.Concat(log.Coche.Interno, " - ", chofer),
                                           responsable,
                                           log.Coche.ToLocalString(log.Fecha, true),
                                           GeocoderHelper.GetDescripcionEsquinaMasCercana(log.Latitud, log.Longitud),
                                           log.Texto,
                                           link
                                       }
                                 : new List<string>
                                       {
                                           "Sistema",
                                           "(Ninguno)",
                                           responsable,
                                           string.Format("{0} {1}", log.Fecha.ToShortDateString(),log.Fecha.ToShortTimeString()),
                                           GeocoderHelper.GetDescripcionEsquinaMasCercana(log.Latitud, log.Longitud),
                                           log.Texto,
                                           link
                                       };

	        var destinatarios = log.Accion.DestinatariosMail;
            if (log.Accion.ReportaDepartamento && log.Accion.Departamento.Empleado != null && log.Accion.Departamento.Empleado.Mail != string.Empty)
                destinatarios = destinatarios + ";" + log.Accion.Departamento.Empleado.Mail;
            if (log.Accion.ReportaCentroDeCostos && log.Accion.CentroDeCostos.Empleado != null && log.Accion.CentroDeCostos.Empleado.Mail != string.Empty)
                destinatarios = destinatarios + ";" + log.Accion.CentroDeCostos.Empleado.Mail;

	        var asunto = log.Accion.AsuntoMail.Trim() != string.Empty
	                         ? log.Accion.AsuntoMail.Trim()
	                         : log.Texto;

            SendMailToAllDestinations(destinatarios, log.Chofer, asunto, parameters);
        }

        private void EnviarReporte(LogMensaje log)
        {
            var queue = GetMailReportMsmq();
            IReportCommand reportCommand = null;

            switch (log.Accion.Reporte)
            {
                case ProgramacionReporte.Reportes.EstadoEntregas:
                    reportCommand = new DeliverStatusReportCommand
                    {
                        ReportId = log.Id,
                        CustomerId = log.Viaje.Empresa.Id,
                        Email = log.Accion.DestinatariosMailReporte,
                        FinalDate = log.Fecha,
                        InitialDate = log.Viaje.InicioReal.Value,
                        VehiclesId = new List<int>{log.Viaje.Vehiculo.Id},
                        ReportName = log.Viaje.Codigo                        
                    };
                    break;
                default:
                    break;
            }

            if (queue == null) { throw new ApplicationException("No se pudo acceder a la cola"); }
            if (reportCommand != null) queue.Send(reportCommand);
        }

        private IMessageQueue GetMailReportMsmq()
        {
            var queueName = Config.ReportMsmq.QueueName;
            var queueType = Config.ReportMsmq.QueueType;
            if (String.IsNullOrEmpty(queueName)) return null;

            var umq = new IMessageQueue(queueName);
            if (queueType.ToLower() == "xml") umq.Formatter = "XmlMessageFormatter";

            return !umq.LoadResources() ? null : umq;
        }

        protected static void SendMailToAllDestinations(string destinations, Empleado chofer, string asunto, List<string> parameters)
        {
            var configFile = Config.Mailing.MailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de mailing.");

            var sender = new MailSender(configFile);

            if (string.IsNullOrEmpty(destinations)) return;

            var originalSubject = !string.IsNullOrEmpty(asunto) ? asunto : sender.Config.Subject;

            var destinatariosMail = destinations.Replace(',', ';');

            var destinos = destinatariosMail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos)
            {
                sender.Config.Subject = originalSubject;

                var destinatarios = string.IsNullOrEmpty(destinatario.Trim()) ? null : GetMailRealDestinatarios(destinatario, chofer, sender);

                if (destinatarios == null || destinatarios.Count.Equals(0)) continue;

                foreach (var dest in destinatarios.Where(dest => !string.IsNullOrEmpty(dest)))
                {
                    sender.Config.ToAddress = dest.Trim();
                    sender.SendMail(parameters.ToArray());
                }
            }
        }

	    private static List<string> GetMailRealDestinatarios(string destinatario, Empleado chofer, MailSender sender)
        {
            destinatario = destinatario.Trim();

            var reporta1 = destinatario.ToUpper().Equals("REPORTA1");
            var reporta2 = destinatario.ToUpper().Equals("REPORTA2");
            var reporta3 = destinatario.ToUpper().Equals("REPORTA3");

            if (!reporta1 && !reporta2 && !reporta3) return new List<string> { destinatario };

            if (chofer == null) return null;

            sender.Config.Subject = string.Concat(sender.Config.Subject, " - Envio a Responsable");

            if (reporta1) return chofer.Reporta1 == null ? null : new List<string> { chofer.Reporta1.Mail };

            if (reporta2) return chofer.Reporta2 == null ? null : new List<string> { chofer.Reporta2.Mail };

            return chofer.Reporta3 == null ? null : new List<string> { chofer.Reporta3.Mail };
        } 
        #endregion
    }
}
