#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Messaging;
using Urbetrack.AVL;
using Urbetrack.Common.Mailing;
using Urbetrack.Common.Messaging;
using Urbetrack.Common.Services.Helpers;
using Urbetrack.Common.Utils;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Messages.Saver.BaseClasses;
using Urbetrack.Messages.Sender;
using Urbetrack.Model;
using Urbetrack.Process.CicloLogistico;
using Urbetrack.Process.Geofences;
using Urbetrack.Layers;
using Urbetrack.Types.BusinessObjects;
using Urbetrack.Types.BusinessObjects.BaseObjects;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Messages;
using Urbetrack.Types.BusinessObjects.ReferenciasGeograficas;
using Urbetrack.Types.BusinessObjects.Vehiculos;
using Urbetrack.Types.ValueObject.Messages;
using Urbetrack.Common.Configuration;
using Urbetrack.Types.BusinessObjects.Entidades;

#endregion

namespace Urbetrack.Messages.Saver
{
    /// <summary>
    /// Class for saving messages into database and performing all associated actions.
    /// </summary>
    public class LogMensajeSaver : BaseEventSaver, IMessageSaver, IDisposable
    {
        #region IDisposable

        public void Dispose()
        {
            DisposeResources();
        }

        #endregion

		#region IMessageSaver

		/// <summary>
        /// Saves generic events into data base.
        /// </summary>
        /// <param name="dispositivo">The device that reported the current event.</param>
        /// <param name="coche">The vehicle that generated the event.</param>
        /// <param name="inicio">The initial position of the event.</param>
        /// <param name="fin">The final position of the event.</param>
        /// <param name="codigo">The event code.</param>
        /// <param name="texto">The evnt text.</param>
        /// <param name="chofer">The current driver of the mobile that reported the event.</param>
        /// <param name="velPermitida">The allowed speed.</param>
        /// <param name="velAlcanzada">The reached speed.</param>
        /// <param name="idPuntoDeInteres">The id of the geocerca where the event tke place.</param>
        public LogMensajeBase SaveLogEvento(Dispositivo dispositivo, Coche coche, GPSPoint inicio, GPSPoint fin, String codigo, String texto, Empleado chofer, Int32? velPermitida, Int32? velAlcanzada, Int32? idPuntoDeInteres)
        {
            return SaveLogEvento(dispositivo, coche, inicio, fin, codigo, texto, chofer, velPermitida, velAlcanzada, idPuntoDeInteres, null);
        }

        public LogMensajeBase SaveLogEvento(Dispositivo dispositivo, Coche coche, GPSPoint inicio, GPSPoint fin, String codigo, String texto, Empleado chofer, DateTime fecha)
        {
            var start = inicio ?? GetLastPosition(coche);
            if (inicio == null)
            {
                if (start == null) start = new GPSPoint();
            }
            start.Date = fecha;

            return SaveLogEvento(dispositivo, coche, inicio, fin, codigo, texto, chofer, null, null, null, null);
        }

        public LogMensajeBase SaveLogEvento(Dispositivo dispositivo, Coche coche, GPSPoint inicio, GPSPoint fin, String codigo, String texto, Empleado chofer, Int32? velPermitida, Int32? velAlcanzada, Int32? idPuntoDeInteres, IMessage evento)
        {
            try
            {
                var device = dispositivo ?? GetGenericDevice();

				STrace.Debug(typeof(LogEvento).FullName, device.Id, "SaveLogEvento: init");

                var driver = coche != null && !coche.IdentificaChoferes ? coche.Chofer : chofer;

                var start = inicio ?? GetLastPosition(coche);

                var mensaje = GetByCodigo(codigo, coche);

				var tiempo = evento != null ? evento.Tiempo : start != null ? start.Date : DateTime.UtcNow;

                if (inicio == null)
                {
                    if(start == null) start = new GPSPoint();
                    start.Date = tiempo;
                }

                if (mensaje == null) return DiscardDueToInvalidMessage(codigo, coche, inicio, device, fin, driver, tiempo);

                if (fin == null && DiscartInhibitor(mensaje, coche, start.Lat, start.Lon)) return DiscardDueToInhibitor(coche, inicio, mensaje, device, fin, driver, tiempo);

                var msg = SaveEvent(coche, device, mensaje, start, fin, driver, velAlcanzada, velPermitida, idPuntoDeInteres, texto, tiempo);

                // Proceso el ticket actual
                CheckTicket(coche, start, codigo, idPuntoDeInteres, evento);

                return msg;
            }
            catch (Exception ex)
            {
                STrace.Exception(GetType().FullName, ex);

                return null;
            }
            finally
            {
				DisposeResources();
            }
        }

        public LogEvento SaveLogEventoSensor(Dispositivo dispositivo, Sensor sensor, SubEntidad subEntidad, DateTime inicio, DateTime fin, String codigo, String texto, IMessage evento)
        {
            try
            {
                var device = dispositivo ?? GetGenericDevice();

                var mensaje = GetByCodigo(codigo, sensor);

                if (mensaje == null) return null;
                
                var msg = SaveLogEvento(device, sensor, subEntidad, mensaje, texto, inicio, fin);

                return msg;
            }
            catch (Exception ex)
            {
                STrace.Exception(GetType().FullName, ex);

                return null;
            }
            finally
            {
                DisposeResources();
            }
        }

        /// <summary>
        /// Generates a new log evento with the specified data.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="dispositivo"></param>
        /// <param name="codigo"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="chofer"></param>
        /// <param name="discardReason"></param>
    	/// <param name="dt"></param>
        /// <returns></returns>
    	public void SaveLogEventoDescartado(Coche coche, Dispositivo dispositivo, String codigo, GPSPoint inicio, GPSPoint fin, Empleado chofer, DiscardReason discardReason, DateTime dt)
        {
            var mensaje = !String.IsNullOrEmpty(codigo) ? GetByCodigo(codigo, coche) : null;

            SaveLogEventoDescartado(coche, dispositivo, mensaje, inicio, fin, chofer, discardReason, dt);
        }

    	/// <summary>
        /// Generates a new log evento with the specified data.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="dispositivo"></param>
        /// <param name="mensaje"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="chofer"></param>
        /// <param name="discardReason"></param>
    	/// <param name="dt"></param>
        /// <returns></returns>
    	public void SaveLogEventoDescartado(Coche coche, Dispositivo dispositivo, MensajeVO mensaje, GPSPoint inicio, GPSPoint fin, Empleado chofer, DiscardReason discardReason, DateTime dt)
        {
            var log = new LogMensajeDescartado
            {
                Chofer = chofer,
                Coche = coche,
                Dispositivo = dispositivo,
                Fecha = inicio != null ? inicio.Date : dt,
                Expiracion = DateTime.UtcNow.AddDays(1),
                Latitud = inicio != null ? inicio.Lat : 0,
                Longitud = inicio != null ? inicio.Lon : 0,
                FechaFin = fin != null ? fin.Date : (DateTime?)null,
                LatitudFin = fin != null ? new Double?(fin.Lat) : null,
                LongitudFin = fin != null ? new Double?(fin.Lon) : null,
                Mensaje = mensaje != null ? DaoFactory.MensajeDAO.FindById(mensaje.Id) : null,
                MotivoDescarte = discardReason.GetNumericCode()
            };

            if(DaoFactory.Session.Transaction != null && DaoFactory.Session.Transaction.IsActive)
                DaoFactory.LogMensajeDescartadoDAO.SaveWithoutTransaction(log);
            else DaoFactory.LogMensajeDescartadoDAO.Save(log);
        }

        public void SaveLogEventoDescartado(Sensor sensor, Dispositivo dispositivo, String codigo, GPSPoint inicio, GPSPoint fin, DiscardReason discardReason, DateTime dt)
        {
            var mensaje = !String.IsNullOrEmpty(codigo) ? GetByCodigo(codigo, sensor) : null;

            SaveLogEventoDescartado(sensor, dispositivo, mensaje, inicio, fin, discardReason, dt);
        }

        public void SaveLogEventoDescartado(Sensor sensor, Dispositivo dispositivo, MensajeVO mensaje, GPSPoint inicio, GPSPoint fin, DiscardReason discardReason, DateTime dt)
        {
            var log = new LogMensajeDescartado
            {
                Dispositivo = dispositivo,
                Fecha = inicio != null ? inicio.Date : dt,
                Expiracion = DateTime.UtcNow.AddDays(1),
                Latitud = inicio != null ? inicio.Lat : 0,
                Longitud = inicio != null ? inicio.Lon : 0,
                FechaFin = fin != null ? fin.Date : (DateTime?)null,
                LatitudFin = fin != null ? new Double?(fin.Lat) : null,
                LongitudFin = fin != null ? new Double?(fin.Lon) : null,
                Mensaje = mensaje != null ? DaoFactory.MensajeDAO.FindById(mensaje.Id) : null,
                MotivoDescarte = discardReason.GetNumericCode()
            };

            if (DaoFactory.Session.Transaction != null && DaoFactory.Session.Transaction.IsActive)
                DaoFactory.LogMensajeDescartadoDAO.SaveWithoutTransaction(log);
            else DaoFactory.LogMensajeDescartadoDAO.Save(log);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Discards the current message because the vehicle is inside a inhibitor.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="inicio"></param>
        /// <param name="mensaje"></param>
        /// <param name="device"></param>
        /// <param name="fin"></param>
        /// <param name="driver"></param>
    	/// <param name="dt"></param>
        /// <returns></returns>
    	private LogMensajeBase DiscardDueToInhibitor(Coche coche, GPSPoint inicio, MensajeVO mensaje, Dispositivo device, GPSPoint fin, Empleado driver, DateTime dt)
        {
            SaveLogEventoDescartado(coche, device, mensaje, inicio, fin, driver, DiscardReason.InsideInhibitor, dt);

            return null;
        }
        
        /// <summary>
        /// Discards the current essage becouse there is no message associated to the givenn code for the current vehicle.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="coche"></param>
        /// <param name="inicio"></param>
        /// <param name="device"></param>
        /// <param name="fin"></param>
        /// <param name="driver"></param>
    	/// <param name="dt"></param>
        /// <returns></returns>
    	private LogMensajeBase DiscardDueToInvalidMessage(String code, Coche coche, GPSPoint inicio, Dispositivo device, GPSPoint fin, Empleado driver, DateTime dt)
        {
            if (coche != null)
            {
            	STrace.Error(GetType().FullName, coche.Dispositivo.Id, "A message with code {0} was not found for the vehicle {1} (id={2}).", code, coche.Interno, coche.Id);
            }
            else
            {
            	STrace.Error(GetType().FullName, "A message with code {0} was not found.", code);
            }

            SaveLogEventoDescartado(coche, device, String.Empty, inicio, fin, driver, DiscardReason.NoMessageFound, dt);

            return null;
        }
        
        /// <summary>
        /// Generates a new log evento with the specified data.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="dispositivo"></param>
        /// <param name="mensaje"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="chofer"></param>
        /// <param name="velAlcanzada"></param>
        /// <param name="velPermitida"></param>
        /// <param name="idPuntoDeInteres"></param>
        /// <param name="texto"></param>
    	/// <param name="dt"></param>
        /// <returns></returns>
    	private LogMensajeBase SaveEvent(Coche coche, Dispositivo dispositivo, MensajeVO mensaje, GPSPoint inicio, GPSPoint fin, Empleado chofer, Int32? velAlcanzada, Int32? velPermitida, Int32? idPuntoDeInteres, String texto, DateTime dt)
        {
            var ticket = DaoFactory.TicketDAO.FindEnCurso(dispositivo);
            var detalleTicket = ticket == null ? null : ticket.GetDetalleProximo();

            var driver = chofer ?? (ticket != null ? ticket.Empleado : null);

            var log = new LogMensaje
            {
                Chofer = driver,
                Coche = coche,
                Dispositivo = dispositivo,
                Estado = 0,
                Fecha = inicio != null ? inicio.Date : (dt <= new DateTime(2010, 1, 1)) ? dt : DateTime.UtcNow,
                Expiracion = DateTime.UtcNow.AddDays(1),
                Horario = ticket,
                DetalleHorario = detalleTicket,
                Usuario = null,
                Latitud = inicio != null ? inicio.Lat : 0,
                Longitud = inicio != null ? inicio.Lon : 0,
                FechaFin = fin != null ? fin.Date : (DateTime?)null,
                LatitudFin = fin != null ? new Double?(fin.Lat) : null,
                LongitudFin = fin != null ? new Double?(fin.Lon) : null,
                VelocidadAlcanzada = velAlcanzada,
                VelocidadPermitida = velPermitida,
                IdPuntoDeInteres = idPuntoDeInteres,
                Mensaje = DaoFactory.MensajeDAO.FindById(mensaje.Id),
                Texto = string.Concat(mensaje.Texto, ' ', texto)
            };

            SaveEvent(log);

            return log;
        }
        
        private LogEvento SaveLogEvento(Dispositivo dispositivo, Sensor sensor, SubEntidad subEntidad, MensajeVO mensaje, string texto, DateTime inicio, DateTime fin)
        {
            var log = new LogEvento
            {
                Dispositivo = dispositivo,
                Sensor = sensor,
                SubEntidad = subEntidad,
                Mensaje = DaoFactory.MensajeDAO.FindById(mensaje.Id),
                Fecha = inicio,
                FechaFin = fin,
                Expiracion = DateTime.UtcNow.AddDays(1),
                Estado = 0,
                Texto = String.Concat(mensaje.Texto, ' ', texto)
            };

            SaveLogEvento(log);

            return log;
        }
        private void SaveLogEvento(LogEvento log)
        {
            var appliesToAnyAction = false;

            var actions = GetActions(log.Mensaje);

            var filteredActions = FilterActions(actions, log);

            foreach (var accion in filteredActions)
            {
                appliesToAnyAction = true;

                DaoFactory.RemoveFromSession(log);

                log.Id = 0;
                log.Accion = accion;

                if (accion.CambiaMensaje) log.Texto += string.Concat(" ", accion.MensajeACambiar);

                //if (accion.PideFoto) PedirFoto(log);

                if (accion.GrabaEnBase) DaoFactory.LogEventoDAO.Save(log);

                if (accion.EsAlarmaDeMail) SendMail(log);

                //if (accion.EsAlarmaSMS) SendSms(log);

                if (accion.Habilita) HabilitarUsuario(log);

                if (accion.Inhabilita) InhabilitarUsuario(log);

                //if (accion.ReportarAssistCargo) ReportarAssistCargo(log, accion.CodigoAssistCargo);
            }

            if (!appliesToAnyAction)
            {
                if (DaoFactory.Session.Transaction != null && DaoFactory.Session.Transaction.IsActive)
                    DaoFactory.LogEventoDAO.SaveWithoutTransaction(log);
                else DaoFactory.LogEventoDAO.Save(log);
            }
        }

        /// <summary>
        /// Gets a GPSPoint that represents the last vehicle position.
        /// </summary>
        /// <param name="coche"></param>
        /// <returns></returns>
        private GPSPoint GetLastPosition(Coche coche)
        {
            var lastPosition = DaoFactory.LogPosicionDAO.GetLastVehiclePosition(coche);

            return lastPosition == null ? new GPSPoint() : new GPSPoint(DateTime.UtcNow, (float)lastPosition.Latitud, (float)lastPosition.Longitud);
        }

        /// <summary>
        /// Gets the current device assigned to the vehicle.
        /// </summary>
        /// <returns></returns>
        private Dispositivo GetGenericDevice()
        {
            const String imei = "No borrar.";

            var dispositivo = DaoFactory.DispositivoDAO.GetByImei(imei);

            if (dispositivo != null) return dispositivo;

            dispositivo = new Dispositivo
            {
                Codigo = imei,
                TipoDispositivo = DaoFactory.TipoDispositivoDAO.FindAll()[0],
                Imei = imei,
                Port = 2020,
                Clave = imei,
                Telefono = imei,
                Tablas = "",
            };

            if (DaoFactory.Session.Transaction != null && DaoFactory.Session.Transaction.IsActive)
                DaoFactory.DispositivoDAO.SaveOrUpdateWithoutTransaction(dispositivo);
            else DaoFactory.DispositivoDAO.SaveOrUpdate(dispositivo);

            return dispositivo;
        }

        /// <summary>
        /// Performs all actions associated to the event and saves it into database.
        /// </summary>
        /// <param name="log"></param>
        private void SaveEvent(LogMensaje log)
        {
            var appliesToAnyAction = false;

            var actions = GetActions(log.Mensaje);

            var filteredActions = FilterActions(actions, log);

            foreach (var accion in filteredActions)
            {
                appliesToAnyAction = true;

                DaoFactory.RemoveFromSession(log);

                log.Id = 0;
                log.Accion = accion;

                if(accion.CambiaMensaje) log.Texto += string.Concat(" ", accion.MensajeACambiar);

                if (accion.PideFoto) PedirFoto(log);

                if (accion.GrabaEnBase) DaoFactory.LogMensajeDAO.Save(log);

                if (accion.EsAlarmaDeMail) SendMail(log);

                if (accion.EsAlarmaSMS) SendSms(log);

                if (accion.Habilita) HabilitarUsuario(log);

                if (accion.Inhabilita) InhabilitarUsuario(log);

                if (accion.ReportarAssistCargo) ReportarAssistCargo(log, accion.CodigoAssistCargo);
            }

            if (!appliesToAnyAction)
            {
                if (DaoFactory.Session.Transaction != null && DaoFactory.Session.Transaction.IsActive)
                    DaoFactory.LogMensajeDAO.SaveWithoutTransaction(log);
                else DaoFactory.LogMensajeDAO.Save(log);
            }
        }

        /// <summary>
        /// Filters the givenn messages collection to return only those wich applies to the givenn message context.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        private IEnumerable<Accion> FilterActions(ICollection<Accion> actions, LogMensajeBase mensaje)
        {
            if (actions == null || actions.Count.Equals(0)) return new List<Accion>();

            var actionsToCheck = GetMatchingActions(actions, mensaje);

            var actionsToApply = actionsToCheck.Where(action => ApplyAction(mensaje, action));

            var filteredActions = new List<Accion>(actionsToCheck.Count());

            var poi = mensaje.IdPuntoDeInteres.HasValue ? DaoFactory.ReferenciaGeograficaDAO.FindById(mensaje.IdPuntoDeInteres.Value) : null;

            filteredActions.AddRange(poi != null
                ? actionsToApply.Where(action => action.TipoReferenciaGeografica == null || action.TipoReferenciaGeografica.Id.Equals(poi.TipoReferenciaGeografica.Id))
                : actionsToApply);
            if (filteredActions.Any(accion => accion.EvaluaGeocerca) && mensaje.Coche != null)
            {
                filteredActions = filteredActions.Where(accion => CheckGeofence(mensaje.Coche, accion)).ToList();
            }

            return filteredActions;
        }
        private IEnumerable<Accion> FilterActions(ICollection<Accion> actions, LogEvento evento)
        {
            if (actions == null || actions.Count.Equals(0)) return new List<Accion>();

            var actionsToCheck = GetMatchingActions(actions, evento);

            var actionsToApply = actionsToCheck.Where(action => ApplyAction(evento, action));

            var filteredActions = new List<Accion>(actionsToCheck.Count());

            filteredActions.AddRange(actionsToApply);

            return filteredActions;
        }

        private static bool CheckGeofence(Coche vehiculo, Accion accion)
        {
            if (!accion.EvaluaGeocerca) return true;
            
            var geocercas = GeofenceManager.GetGeocercas(vehiculo)
                .Where(geo => geo.TipoReferenciaGeograficaId == accion.TipoGeocerca.Id)
                .Select(geocerca => GeofenceManager.GetState(vehiculo.Id, geocerca.Id));

            var inside = false;

            foreach (var matching in geocercas)
            {
                inside |= matching != null && matching.Estado == EstadosVehiculo.Dentro;
                if (accion.DentroGeocerca && inside) break;
            }
            return (accion.DentroGeocerca && inside) || (!accion.DentroGeocerca && !inside);
        }

        /// <summary>
        /// Gets all actions that matches the current message context.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        private static IEnumerable<Accion> GetMatchingActions(IEnumerable actions, LogMensajeBase mensaje)
        {
            if (actions == null) return new List<Accion>();

            return from action in actions.OfType<Accion>().ToList()
                   where mensaje.Coche != null
                   where action.Empresa == null || mensaje.Coche.Empresa == null || action.Empresa.Id.Equals(mensaje.Coche.Empresa.Id)
                   where action.Linea == null || mensaje.Coche.Linea == null || action.Linea.Id.Equals(mensaje.Coche.Linea.Id)
                   where action.TipoVehiculo == null || action.TipoVehiculo.Id.Equals(mensaje.Coche.TipoCoche.Id)
                   where action.Transportista == null || (mensaje.Coche.Transportista != null && action.Transportista.Id.Equals(mensaje.Coche.Transportista.Id))
                   select action;
        }
        private static IEnumerable<Accion> GetMatchingActions(IEnumerable actions, LogEvento evento)
        {
            if (actions == null) return new List<Accion>();

            return from action in actions.OfType<Accion>().ToList()
                   where action.Empresa == null || evento.Dispositivo.Empresa == null || action.Empresa.Id.Equals(evento.Dispositivo.Empresa.Id)
                   where action.Linea == null || evento.Dispositivo.Linea == null || action.Linea.Id.Equals(evento.Dispositivo.Linea.Id)
                   select action;
        }

        /// <summary>
        /// Sends a SMS with info about the event.
        /// </summary>
        /// <param name="log"></param>
        private static void SendSms(LogMensajeBase log)
        {
            var configFile = Config.Mailing.MailingSmsConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de envio de sms.");

            var sender = new MailSender(configFile);

            var parameters = new List<string>
                                 {
                                     log.Coche.Linea != null ? log.Coche.Linea.Descripcion : log.Coche.Empresa != null ? log.Coche.Empresa.RazonSocial : "Sistema",
                                     log.Coche.Interno,
                                     GetFecha(log, false),
                                     log.Texto
                                 };

            SendSmsToAllDestinations(log, sender, parameters);
        }

        /// <summary>
        /// Enables the user
        /// </summary>
        /// <param name="log"></param>
        private void HabilitarUsuario(LogMensajeBase log)
        {
            DaoFactory.UsuarioDAO.HabilitarUsuario(log.Accion.UsuarioHabilitado, log.Accion.HorasHabilitado);
        }
        private void HabilitarUsuario(LogEventoBase log)
        {
            DaoFactory.UsuarioDAO.HabilitarUsuario(log.Accion.UsuarioHabilitado, log.Accion.HorasHabilitado);
        }

        /// <summary>
        /// Disables the user
        /// </summary>
        /// <param name="log"></param>
        private void InhabilitarUsuario(LogMensajeBase log)
        {
            DaoFactory.UsuarioDAO.InhabilitarUsuario(log.Accion.UsuarioInhabilitado);
        }
        private void InhabilitarUsuario(LogEventoBase log)
        {
            DaoFactory.UsuarioDAO.InhabilitarUsuario(log.Accion.UsuarioInhabilitado);
        }

        private void ReportarAssistCargo(LogMensajeBase log, string assistCargoCode)
        {
            try
            {
                if(!log.Coche.ReportaAssistCargo) return;

            	STrace.Debug(GetType().FullName, "AssistCargo Event: {0} -> {1}: {2}", log.Coche.Patente, Config.AssistCargo.AssistCargoEventQueue, assistCargoCode);

                var queue = new IMessageQueue(Config.AssistCargo.AssistCargoEventQueue);

				if (queue.LoadResources())
				{
					var data = new TextEvent(log.Dispositivo.Id, 0, DateTime.UtcNow)
					           	{
					           		Text = assistCargoCode,
					           		GeoPoint = new GPSPoint(log.Fecha, (float) log.Latitud, (float) log.Longitud)
					           	};
					queue.Send(data, MessageQueueTransactionType.Automatic);
				}
				else
				{
					STrace.Error(GetType().FullName, "Problemas cargando la cola: {0}", Config.AssistCargo.AssistCargoEventQueue);
				}
            }
            catch(Exception ex)
            {
				STrace.Exception(GetType().FullName, ex);
            }
        }

        private static void PedirFoto(LogMensajeBase log)
        {
            log.TieneFoto = true;

            //var sender = new Sender.Sender(log.Dispositivo.TipoDispositivo.ColaDeComandos);

            DateTime from;
            DateTime to;
            if(log.FechaFin.HasValue)
            {
                to = log.FechaFin.Value;
                from = log.FechaFin.Value.AddSeconds(-log.Accion.SegundosFoto);
                if(log.Fecha > from) from = log.Fecha;
            }
            else
            {
                to = log.Fecha;
                from = log.Fecha.AddSeconds(-log.Accion.SegundosFoto);
            }
            //sender.SendRetrievePictures(log.Dispositivo.Id, from, to);
            MessageSender.CreateRetrievePictures(log.Dispositivo, new LogMensajeSaver()).AddDateRange(from, to).Send();
        }

        /// <summary>
        /// Sends a SMS message to all recipients.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="sender"></param>
        /// <param name="parameters"></param>
        private static void SendSmsToAllDestinations(LogMensajeBase log, MailSender sender, List<string> parameters)
        {
            if (string.IsNullOrEmpty(log.Accion.DestinatariosSMS)) return;

            var destinatariosSms = log.Accion.DestinatariosSMS.Replace(',', ';');

            var destinos = destinatariosSms.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos.Where(destinatario => !string.IsNullOrEmpty(destinatario)))
            {
                sender.Config.ToAddress = destinatario.Trim();

                sender.SendMail(parameters.ToArray());
            }
        }

        /// <summary>
        /// Sends a mail with info about the event.
        /// </summary>
        /// <param name="log"></param>
        private static void SendMail(LogMensaje log)
        {
            var configFile = Config.Mailing.MailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de mailing.");

            var sender = new MailSender(configFile);

            const int minutes = 15;

            var monitor = Config.Monitor.HistoricMonitorLink;

            var link = string.Format(@"{0}?Planta={1}&TypeMobile={2}&Movil={3}&InitialDate={4}&FinalDate={5}&MessageCenterIndex={6}&ShowMessages=1&Empresa={7}&ShowPOIS=1",
                monitor, log.Coche.Linea != null ? log.Coche.Linea.Id : -1, log.Coche.TipoCoche.Id, log.Coche.Id,
                log.Fecha.AddMinutes(-minutes).ToString(CultureInfo.InvariantCulture), log.Fecha.AddMinutes(minutes).ToString(CultureInfo.InvariantCulture), log.Id,
                log.Coche.Empresa != null ? log.Coche.Empresa.Id : log.Coche.Linea != null ? log.Coche.Linea.Empresa.Id : -1);

            var chofer = log.Chofer != null ? string.Format("Chofer: {0}", log.Chofer.Entidad.Descripcion) : "Sin Chofer Identificado";

            var responsable = log.Coche.Chofer != null && log.Coche.Chofer.Entidad != null ? log.Coche.Chofer.Entidad.Descripcion : "Sin Responsable Asignado";

            var parameters = new List<string>
                                     {
                                         log.Coche.Linea != null ? log.Coche.Linea.Descripcion : log.Coche.Empresa != null ? log.Coche.Empresa.RazonSocial : "Sistema",
                                         string.Concat(log.Coche.Interno, " - ", chofer),
                                         responsable,
                                         GetFecha(log, true),
                                         AddresserHelper.GetDescripcionEsquinaMasCercana(log.Latitud, log.Longitud),
                                         log.Texto,
                                         link
                                     };

            SendMailToAllDestinations(log, sender, parameters);
        }
        private static void SendMail(LogEvento log)
        {
            var configFile = Config.Mailing.MailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de mailing.");

            var sender = new MailSender(configFile);

            var monitor = Config.Monitor.SubEntidadesMonitorLink;

            var link = string.Format(@"{0}?ID_SUBENTIDAD={1}", monitor, log.SubEntidad != null ? log.SubEntidad.Id : -1);

            var parameters = new List<string>
                                     {
                                         log.SubEntidad != null && log.SubEntidad.Linea != null ? log.SubEntidad.Linea.Descripcion : log.SubEntidad != null && log.SubEntidad.Empresa != null ? log.SubEntidad.Empresa.RazonSocial : "Sistema",
                                         log.SubEntidad != null ? log.SubEntidad.Descripcion : "",
                                         log.Sensor != null ? log.Sensor.Descripcion : "",
                                         GetFecha(log, true),
                                         AddresserHelper.GetDescripcionEsquinaMasCercana(log.SubEntidad != null ? log.SubEntidad.Entidad.ReferenciaGeografica.Latitude : 0, log.SubEntidad != null ? log.SubEntidad.Entidad.ReferenciaGeografica.Longitude : 0),
                                         log.Texto,
                                         link
                                     };

            SendMailToAllDestinations(log, sender, parameters);
        }

        /// <summary>
        /// Gets the date and time of the message in the corresponding time zone.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="includeGmt"></param>
        /// <returns></returns>
        private static string GetFecha(LogMensajeBase log, bool includeGmt)
        {
            var timeZoneId = log.Coche.Linea != null ? log.Coche.Linea.TimeZoneId : log.Coche.Empresa != null ? log.Coche.Empresa.TimeZoneId : null;

            if (timeZoneId == null) return string.Concat(log.Fecha.ToString(), " (GMT)");

            var culture = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var referencia = log.Fecha.AddHours(culture.BaseUtcOffset.TotalHours);

            var fecha = string.Format("{0} {1}", referencia.ToShortDateString(), referencia.ToShortTimeString());

            return includeGmt ? string.Concat(fecha, string.Format(" ({0})", culture.DisplayName)) : fecha;
        }
        private static string GetFecha(LogEvento log, bool includeGmt)
        {
            var timeZoneId = log.SubEntidad.Linea != null ? log.SubEntidad.Linea.TimeZoneId : log.SubEntidad.Empresa != null ? log.SubEntidad.Empresa.TimeZoneId : null;

            if (timeZoneId == null) return string.Concat(log.Fecha.ToString(), " (GMT)");

            var culture = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var referencia = log.Fecha.AddHours(culture.BaseUtcOffset.TotalHours);

            var fecha = string.Format("{0} {1}", referencia.ToShortDateString(), referencia.ToShortTimeString());

            return includeGmt ? string.Concat(fecha, string.Format(" ({0})", culture.DisplayName)) : fecha;
        }

        /// <summary>
        /// Sends a mail with the givenn parameters to all the directions givenn in the message.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="sender"></param>
        /// <param name="parameters"></param>
        private static void SendMailToAllDestinations(LogMensaje log, MailSender sender, List<string> parameters)
        {
            if (string.IsNullOrEmpty(log.Accion.DestinatariosMail)) return;

            var originalSubject = !string.IsNullOrEmpty(log.Accion.AsuntoMail) ? log.Accion.AsuntoMail : sender.Config.Subject;

            var destinatariosMail = log.Accion.DestinatariosMail.Replace(',', ';');

            var destinos = destinatariosMail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos)
            {
                sender.Config.Subject = originalSubject;

                var destinatarios = GetDestinatarios(destinatario, log, sender);

                if (destinatarios == null || destinatarios.Count.Equals(0)) continue;

                foreach (var dest in destinatarios.Where(dest => !string.IsNullOrEmpty(dest)))
                {
                    sender.Config.ToAddress = dest.Trim();

                    sender.SendMail(parameters.ToArray());
                }
            }
        }

        private static void SendMailToAllDestinations(LogEvento log, MailSender sender, List<string> parameters)
        {
            if (string.IsNullOrEmpty(log.Accion.DestinatariosMail)) return;

            var originalSubject = !string.IsNullOrEmpty(log.Accion.AsuntoMail) ? log.Accion.AsuntoMail : sender.Config.Subject;

            var destinatariosMail = log.Accion.DestinatariosMail.Replace(',', ';');

            var destinos = destinatariosMail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos)
            {
                if (string.IsNullOrEmpty(destinatario)) continue;
                
                sender.Config.Subject = originalSubject;
                sender.Config.ToAddress = destinatario.Trim();
                sender.SendMail(parameters.ToArray());
            }
        }

        /// <summary>
        /// Gets the email address from the destination string.
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="log"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        private static List<string> GetDestinatarios(string destinatario, LogMensaje log, MailSender sender)
        {
            destinatario = destinatario.Trim();

            return string.IsNullOrEmpty(destinatario) ? null : GetMailRealDestinatarios(destinatario, log, sender);
        }

        /// <summary>
        /// Returns the real email address from the givenn destination string.
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="log"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        private static List<string> GetMailRealDestinatarios(string destinatario, LogMensaje log, MailSender sender)
        {
            var reporta1 = destinatario.ToUpper().Equals("REPORTA1");
            var reporta2 = destinatario.ToUpper().Equals("REPORTA2");
            var reporta3 = destinatario.ToUpper().Equals("REPORTA3");

            if (!reporta1 && !reporta2 && !reporta3) return new List<string> { destinatario };

            var empleadoEvento = GetEmpleadoEvento(log);

            if (empleadoEvento == null) return null;

            sender.Config.Subject = string.Concat(sender.Config.Subject, " - Envio a Responsable");

            if (reporta1) return empleadoEvento.Reporta1 == null ? null : new List<string> { empleadoEvento.Reporta1.Mail };

            if (reporta2) return empleadoEvento.Reporta2 == null ? null : new List<string> { empleadoEvento.Reporta2.Mail };

            return empleadoEvento.Reporta3 == null ? null : new List<string> { empleadoEvento.Reporta3.Mail };
        }

        /// <summary>
        /// Gets the vehicles drivers bosses.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private static Empleado GetEmpleadoEvento(LogMensajeBase log)
        {
            if (log.Chofer != null) return log.Chofer;

            var coche = log.Coche;

            return coche == null || coche.Transportista == null ? null : coche.Chofer;
        }

        /// <summary>
        /// Determines if the event should be discarted due to a inhibitor.
        /// </summary>
        /// <param name="mensaje"></param>
        /// <param name="coche"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private bool DiscartInhibitor(MensajeVO mensaje, Coche coche, double latitude, double longitude)
        {
            if (mensaje == null || coche == null) return true;

            return mensaje.EsAlarma && GetInhibidores(coche).Any(domicilio => IsInsideInhibitor(latitude, longitude, domicilio));
        }
 
        /// <summary>
        /// Get alarms inhibitors associated to the current mobile.
        /// </summary>
        /// <param name="coche"></param>
        /// <returns></returns>
        private IEnumerable<ReferenciaGeografica> GetInhibidores(Coche coche)
        {
            var empresa = coche.Empresa != null ? coche.Empresa.Id : coche.Linea != null ? coche.Linea.Empresa.Id : -1;
            var linea = coche.Linea != null ? coche.Linea.Id : -1;

            return DaoFactory.ReferenciaGeograficaDAO.FindInhibidores(empresa, linea);
        }

        /// <summary>
        /// Determines if the event toked place inside the givenn inhibitor.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="domicilio"></param>
        /// <returns></returns>
        private static bool IsInsideInhibitor(double latitude, double longitude, ReferenciaGeografica domicilio)
        {
            if (latitude.Equals(0) || longitude.Equals(0) || domicilio == null || domicilio.Poligono == null) return false;

            return domicilio.Poligono.Contains(latitude, longitude);
        }

    	/// <summary>
    	/// Analices changes in the current vehicle service.
    	/// </summary>
    	/// <param name="coche"></param>
    	/// <param name="inicio"></param>
    	/// <param name="codigo"></param>
    	/// <param name="idPuntoDeInteres"></param>
    	/// <param name="message"></param>
    	private void CheckTicket(Coche coche, GPSPoint inicio, string codigo, Int32? idPuntoDeInteres, IMessage message)
        {
            try
            {
                STrace.Debug(GetType().FullName, "Procesando evento. Codigo={0} Vehiculo={1} Fecha={2} Procesar={3}", codigo, coche.Id, inicio.Date.ToString("dd/MM/yyyy HH:mm:ss"), CicloLogisticoFactory.IsAutomaticCode(codigo));
                // Si no es uno de los codigos que cambian estados automáticos, salgo directamente;))
                if (!CicloLogisticoFactory.IsAutomaticCode(codigo)) return;

                var evento = EventFactory.GetEvent(inicio, codigo, idPuntoDeInteres, message);
                if(evento == null) return;

                var ciclo = CicloLogisticoFactory.GetCiclo(coche, new LogMensajeSaver());
                if (ciclo == null) return;

                ciclo.ProcessEvent(evento);
            }
			catch (Exception ex)
			{
				STrace.Exception(GetType().FullName, ex);
			}
        }

        /// <summary>
        /// Gets the message associated to the givenn code for the specified location and base.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="coche"></param>
        /// <returns></returns>
        private MensajeVO GetByCodigo(String codigo, Coche coche)
        {
            var empresa = coche != null ? coche.Empresa ?? (coche.Linea != null ? coche.Linea.Empresa : null) : null;
            var linea = coche != null ? coche.Linea : null;

            return DaoFactory.MensajeDAO.GetByCodigo(codigo, empresa, linea);
        }

        private MensajeVO GetByCodigo(String codigo, Sensor sensor)
        {
            var empresa = sensor != null && sensor.Dispositivo != null ? sensor.Dispositivo.Empresa ?? (sensor.Dispositivo.Linea != null ? sensor.Dispositivo.Linea.Empresa : null) : null;
            var linea = sensor != null && sensor.Dispositivo != null ? sensor.Dispositivo.Linea : null;

            return DaoFactory.MensajeDAO.GetByCodigo(codigo, empresa, linea);
        }

        #endregion
    }
}
