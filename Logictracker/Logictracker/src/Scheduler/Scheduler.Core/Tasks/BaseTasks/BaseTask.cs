using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;

namespace Logictracker.Scheduler.Core.Tasks.BaseTasks
{
    /// <summary>
    /// Base task class for data access scheduled tasks.
    /// </summary>
    public abstract class BaseTask : ITask
    {
        #region Private Properties

        /// <summary>
        /// Data access factory class.
        /// </summary>
        private DAOFactory _daoFactory;

        /// <summary>
        /// Logictracker messages saver.
        /// </summary>
        private IMessageSaver _messageSaver;

        /// <summary>
        /// Logictracker fuel message saver.
        /// </summary>
        private EventoCombustibleSaver _fuelMessageSaver;

        /// <summary>
        /// Report data access factory class.
        /// </summary>
        private ReportFactory _reportFactory;

        /// <summary>
        /// Dictionary for holding all current infractions and their number of ocurrences.
        /// </summary>
        private Dictionary<Exception, Int32> _exceptions = new Dictionary<Exception, Int32>();

        /// <summary>
        /// The timer that triggered the current task.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Parameters for the current task.
        /// </summary>
        private readonly Dictionary<String, String> _parameters = new Dictionary<String, String>();

        #endregion

        #region Protected Properties

        /// <summary>
        /// Data access factory class.
        /// </summary>
        protected DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }

        /// <summary>
        /// Logictracker messages saver singleton accesor.
        /// </summary>
        protected IMessageSaver MessageSaver { get { return _messageSaver ?? (_messageSaver = new MessageSaver(DaoFactory)); } }

        /// <summary>
        /// Logictracker fuel message saver.
        /// </summary>
        protected EventoCombustibleSaver FuelMessageSaver { get { return _fuelMessageSaver ?? (_fuelMessageSaver = new EventoCombustibleSaver()); } }

        /// <summary>
        /// Report data access factory class.
        /// </summary>
        protected ReportFactory ReportFactory { get { return _reportFactory ?? (_reportFactory = new ReportFactory(DaoFactory)); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Handles standart data access logic.
        /// </summary>
        /// <param name="timer"></param>
        public void Execute(Timer timer)
        {
            try
            {
                _timer = timer;
                SessionHelper.CreateSession();
                OnExecute(timer);                
            }
            catch (Exception ex)
            {
                AddError(ex);
            }
            finally
            {                
                InformStatus();

                DisposeResources();

                SessionHelper.CloseSession();
            }
        }

        /// <summary>
        /// Sets tasks parameters.
        /// </summary>
        /// <param name="parameters"></param>
        public void SetParameters(String parameters)
        {
			if (String.IsNullOrEmpty(parameters)) return;

            _parameters.Clear();
            var spplited = parameters.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var parameter in spplited)
            {
                var parts = parameter.Split('=');

                var key = parts[0];
                var value = parts[1];

                _parameters.Add(key, value);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes tasks speficic logic.
        /// </summary>
        /// <param name="timer"></param>
        protected abstract void OnExecute(Timer timer);

        /// <summary>
        /// Dispose all allocated resources for saving messages.
        /// </summary>
        private void DisposeMessageSaver()
        {
            if (_messageSaver == null) return;

            _messageSaver = null;
        }

        /// <summary>
        /// Dispose all allocated resources for saving fuel messages.
        /// </summary>
        private void DisposeFuelMessageSaver()
        {
            if (_fuelMessageSaver == null) return;

            _fuelMessageSaver = null;
        }

        /// <summary>
        /// Adds the current exception as an error to be informed.
        /// </summary>
        /// <param name="ex"></param>
        protected void AddError(Exception ex)
        {
			STrace.Exception(GetType().FullName, ex);

            if (_exceptions.ContainsKey(ex)) _exceptions[ex] += 1;
            else _exceptions.Add(ex, 1);
        }

        /// <summary>
        /// Returns the value of the givenn key as a datetime or null if the key does not exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected DateTime? GetDateTime(String key) { return _parameters.ContainsKey(key) ? (DateTime?)Convert.ToDateTime(_parameters[key]) : null; }

        /// <summary>
        /// Returns the value of the givenn key as a string or null if the key does not exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected String GetString(String key) { return _parameters.ContainsKey(key) ? _parameters[key] : null; }

        /// <summary>
        /// Returns the value of the givenn key as a nullable int or null if the key does not exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected Int32? GetInt32(String key) { return _parameters.ContainsKey(key) ? (Int32?)Convert.ToInt32(_parameters[key]) : null; }

        /// <summary>
        /// Returns the value of the givenn key as a nullable boolean or null if the key does not exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected Boolean? GetBoolean(String key) { return _parameters.ContainsKey(key) ? (Boolean?) Convert.ToBoolean(_parameters[key]) : null; }

        /// <summary>
        /// Returns the value of the givenn key as a list of int or null if the key does not exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected List<Int32> GetListOfInt(String key)
        {
            if (!_parameters.ContainsKey(key)) return null;

            var value = _parameters[key];
            var splitted = value.Split(',');

            return splitted.Select(id => Convert.ToInt32(id)).ToList();
        }

        /// <summary>
        /// Sends a mail with the givenn parameters to all the configured directions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="parameters"></param>
        protected void SendMailToAllDestinations(MailSender sender, List<String> parameters)
        {
            var destinos = GetDestinatarios(sender);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos)
            {
                sender.Config.ToAddress = destinatario.Trim();

                sender.SendMail(parameters.ToArray());
            }
        }

        protected void SendMailToAllDestinations(MailSender sender, List<String> parameters, string destinatarios)
        {
            var destinatariosMail = destinatarios.Replace(',', ';');

            var destinos = destinatariosMail.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (destinos.Count().Equals(0)) return;

            foreach (var destinatario in destinos)
            {
                sender.Config.ToAddress = destinatario.Trim();

                sender.SendMail(parameters.ToArray());
            }
        }

        /// <summary>
        /// Dispose all current resources assigned to the nhibernate sessions.
        /// </summary>
        private void DisposeSessions()
        {
            if (_daoFactory != null) _daoFactory.Dispose();

            _daoFactory = null;
        }

        /// <summary>
        /// Clear all current data loaded into sessions.
        /// </summary>
        protected void ClearSessions() { if (_daoFactory != null) _daoFactory.SessionClear(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the current mail destinatios.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private IEnumerable<String> GetDestinatarios(MailSender sender)
        {
            var destinatarios = GetString("Destinatarios");

            var destinos = destinatarios != null ? String.Concat(sender.Config.ToAddress, ",", destinatarios) : sender.Config.ToAddress;

            return destinos.Replace(',', ';').Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Dispose all allocated resources.
        /// </summary>
        private void DisposeResources()
        {
            DisposeSessions();

            DisposeMessageSaver();

            DisposeFuelMessageSaver();

            GC.Collect();
        }

        /// <summary>
        /// Informs all error that toke place during the execution of the current task.
        /// </summary>
        private void InformStatus()
        {
            if (_exceptions.Count.Equals(0)) NotifySuccess();
            else NotifyErrors();

            _exceptions = null;
        }

        /// <summary>
        /// Sends mails to all task notifiers informing a successfull execution.
        /// </summary>
        private void NotifySuccess()
        {
            if (_timer == null || !_timer.NotifyStatus) return;

            var configFile = Config.Mailing.SchedulerTaskSuccesMailingConfiguration;

			if (String.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de mailing por notificacion de tarea exitosa.");

            var parameters = new List<String> { String.Format("La ejecucion planificada de la tarea {0} finalizo correctamente.", GetType().FullName) };

            SendMailToAllDestinations(parameters, configFile);
        }

        /// <summary>
        /// Send mails to all task notifiers informing errors detected during execution..
        /// </summary>
        private void NotifyErrors()
        {
            var configFile = Config.Mailing.SchedulerErrorMailingConfiguration;

			if (String.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de mailing por notificacion de errores.");     

            var parameters = new List<String> { ExceptionsToText() };

            SendMailToAllDestinations(parameters, configFile);
        }

        /// <summary>
        /// Creates a string representation of all exceptions that toke place during the execution of the current task.
        /// </summary>
        /// <returns></returns>
		private String ExceptionsToText()
        {
            var builder = new StringBuilder();

            foreach (var exception in _exceptions.OrderByDescending(exception => exception.Value))
            {
                builder.AppendLine(String.Format("La siguiente excepcion ocurrio {0} veces:", exception.Value));

                builder.AppendLine("<br/><br/>");

                builder.AppendLine(exception.Key.ToString());

                builder.AppendLine("<br/><br/><hr/><br/>");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Sends a mail with the givenn parameters to all the directions givenn in the message.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="configFile"></param>
        private void SendMailToAllDestinations(List<String> parameters, String configFile)
        {
            if (_timer == null) return;

            var notifiers = _timer.Notifiers.Replace(',', ';').Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries); 

            if (notifiers.Count().Equals(0)) return;

            var sender = new MailSender(configFile);

            sender.Config.Subject = String.Format("{0} - Tarea: {1}", sender.Config.Subject, GetType().FullName);

            foreach (var notifier in notifiers)
            {
                sender.Config.ToAddress = notifier.Trim();

                sender.SendMail(parameters.ToArray());
            }
        }

        #endregion
    }
}
