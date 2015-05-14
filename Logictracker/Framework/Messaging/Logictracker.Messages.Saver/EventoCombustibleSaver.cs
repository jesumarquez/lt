#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Messages.Saver.BaseClasses;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.Messages.Saver
{
    /// <summary>
    /// Class for saving fuel messages into database and performing all associated actions.
    /// </summary>
    public class EventoCombustibleSaver : BaseEventSaver
    {
        #region Public Methods

        /// <summary>
        /// Creates and saves a new EventCombustible in the Database.
        /// </summary>
        /// <param name="motorCode"></param>
        /// <param name="tankCode"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="fecha"></param>
        public void CreateNewEvent(string motorCode, string tankCode, string code, string message, DateTime fecha)
        {
            try
            {
                var motor = motorCode != null ? DaoFactory.CaudalimetroDAO.GetByCode(motorCode) : null;
                var tanque = tankCode != null ? DaoFactory.TanqueDAO.FindByCode(tankCode) : null;

                var mensaje = GetMessage(motor, tanque, code);

                var m = new EventoCombustible
                {
                    Fecha = fecha,
                    FechaAlta = DateTime.Now,
                    MensajeDescri = String.Concat(mensaje.Texto, message),
                    Motor = motor,
                    Tanque = tanque,
                    Mensaje = mensaje
                };

                SaveEvent(m);
            }
            catch (Exception ex)
            {
                STrace.Exception(GetType().FullName, ex);
            }
            finally
            {
				DisposeResources();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the current message to be saved.
        /// </summary>
        /// <param name="motor"></param>
        /// <param name="tanque"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private Mensaje GetMessage(Caudalimetro motor, Tanque tanque, string code)
        {
            var linea = motor != null ? motor.Equipo != null ? motor.Equipo.Linea : null : tanque != null ? tanque.Linea ?? (tanque.Equipo != null ? tanque.Equipo.Linea : null) : null;

            var empresa = linea != null ? linea.Empresa : null;

            return DaoFactory.MensajeDAO.FindById(DaoFactory.MensajeDAO.GetByCodigo(code, empresa, linea).Id);
        }

        /// <summary>
        /// Performs all actions associated to the event and saves it into database.
        /// </summary>
        /// <param name="log"></param>
        private void SaveEvent(EventoCombustible log)
        {
            try
            {
                var appliesToAnyAction = false;

                var actions = GetActions(log.Mensaje);

                foreach (var accion in actions.Where(accion => ApplyAction(log, accion)))
                {
                    appliesToAnyAction = true;

                    DaoFactory.RemoveFromSession(log);

                    log.Id = 0;
                    log.Accion = accion;

                    if (accion.GrabaEnBase) DaoFactory.EventoCombustibleDAO.Save(log);

                    if (accion.EsAlarmaDeMail) SendMail(log);

                    if (accion.EsAlarmaSms) SendSms(log);
                }

                if (!appliesToAnyAction) DaoFactory.EventoCombustibleDAO.Save(log);
            }
            catch (Exception ex) { STrace.Exception(GetType().FullName, ex); }
        }

        /// <summary>
        /// Sends a SMS with info about the event.
        /// </summary>
        /// <param name="log"></param>
        private static void SendSms(EventoCombustible log)
        {
            var configFile = Config.Mailing.FuelMailingSmsConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de envio de sms.");

            var sender = new MailSender(configFile);

            var parameters = new List<string>
                                 {
                                     log.Motor != null ? "Motor" : log.Tanque!= null ? "Tanque" : "Entidad",
                                     log.Motor != null ? log.Motor.Descripcion : log.Tanque != null ? log.Tanque.Descripcion : "Sin Identificar",
                                     log.Motor != null ? "Equipo" : log.Tanque != null ? log.Tanque.Equipo != null ? "Equipo" : "Base" : "Locacion",
                                     log.Motor != null ? log.Motor.Equipo != null ? log.Motor.Equipo.Descripcion : "Desconocido"
                                        : log.Tanque != null ? log.Tanque.Equipo != null ? log.Tanque.Equipo.Descripcion : log.Tanque.Linea.Descripcion : "Desconocido",
                                     GetFecha(log),
                                     log.MensajeDescri
                                 };

            SendSmsToAllDestinations(log, sender, parameters);
        }

        /// <summary>
        /// Sends a SMS message to all recipients.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="sender"></param>
        /// <param name="parameters"></param>
        private static void SendSmsToAllDestinations(EventoCombustible log, MailSender sender, List<string> parameters)
        {
            if (string.IsNullOrEmpty(log.Accion.DestinatariosSms)) return;

            var destinatariosSms = log.Accion.DestinatariosSms.Replace(',', ';');

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
        private static void SendMail(EventoCombustible log)
        {
            var configFile = Config.Mailing.FuelMailingConfiguration;

            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de mailing.");

            var sender = new MailSender(configFile);

            var parameters = new List<string>
                                 {
                                     log.Motor != null ? "Motor" : log.Tanque!= null ? "Tanque" : "Entidad",
                                     log.Motor != null ? log.Motor.Descripcion : log.Tanque != null ? log.Tanque.Descripcion : "Sin Identificar",
                                     log.Motor != null ? "Equipo" : log.Tanque != null ? log.Tanque.Equipo != null ? "Equipo" : "Base" : "Locacion",
                                     log.Motor != null ? log.Motor.Equipo != null ? log.Motor.Equipo.Descripcion : "Desconocido"
                                        : log.Tanque != null ? log.Tanque.Equipo != null ? log.Tanque.Equipo.Descripcion : log.Tanque.Linea.Descripcion : "Desconocido",
                                     GetFecha(log),
                                     log.MensajeDescri
                                 };

            SendMailToAllDestinations(log, sender, parameters);
        }

        /// <summary>
        /// Gets the date and time of the message in the corresponding time zone.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private static string GetFecha(EventoCombustible log) { return string.Format("{0} {1}", log.Fecha.ToShortDateString(), log.Fecha.ToShortTimeString()); }

        /// <summary>
        /// Sends a mail with the givenn parameters to all the directions givenn in the message.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="sender"></param>
        /// <param name="parameters"></param>
        private static void SendMailToAllDestinations(EventoCombustible log, MailSender sender, List<string> parameters)
        {
            if (string.IsNullOrEmpty(log.Accion.DestinatariosMail)) return;

            var destinatariosMail = log.Accion.DestinatariosMail.Replace(',', ';');

            var destinos = destinatariosMail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos.Where(destinatario => !string.IsNullOrEmpty(destinatario)))
            {
                sender.Config.ToAddress = destinatario.Trim();

                sender.SendMail(parameters.ToArray());
            }
        }

	
		#endregion
    }
}