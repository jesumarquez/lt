using System;
using System.Collections.Generic;
using System.Text;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.Messages;

namespace Logictracker.Scheduler.Tasks.QueueStatus
{
    /// <summary>
    /// Tasks for checking the current amount of enqueued messages.
    /// </summary>
    public class Task : BaseTask
    {
        #region Private Properties

        /// <summary>
        /// Configuration properties keys.
        /// </summary>
        private const String RefferenceCount = "MessageCount";

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            var colas = Logictracker.QueueStatus.QueueStatus.GetEnqueuedMessagesPerQueue();
            foreach (var cola in colas)
            {
                var oCola = DaoFactory.ColaMensajesDAO.FindByName(cola.Key);
                
                if (oCola == null) oCola = new ColaMensajes { Nombre = cola.Key };
                oCola.Cantidad = cola.Value;
                oCola.UltimaActualizacion = DateTime.UtcNow;
                
                DaoFactory.ColaMensajesDAO.SaveOrUpdate(oCola);
            }

            var maxCount = Logictracker.QueueStatus.QueueStatus.GetMaxEnqueuedMessagesCount(colas);

            STrace.Trace(GetType().FullName, String.Format("Maximum enqueued messages count: {0}.", maxCount));

            var refferenceCount = GetRefferenceCount();

            if (maxCount < refferenceCount) return;

            Notify();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the message refference count.
        /// </summary>
        /// <returns></returns>
        private Int32 GetRefferenceCount()
        {
            var count = GetInt32(RefferenceCount);

            return count.HasValue ? count.Value : 1000;
        }

        /// <summary>
        /// Notify that the refference message count has been reached.
        /// </summary>
        private void Notify()
        {
			STrace.Trace(GetType().FullName, "Alert interval reached.");

            var configFile = Config.Mailing.SchedulerQueueStatusMailingConfiguration;

			if (String.IsNullOrEmpty(configFile)) throw new Exception("Failed to load queue status mailing configuration.");

            var sender = new MailSender(configFile);

            var statusBuilder = new StringBuilder();

            foreach (var status in Logictracker.QueueStatus.QueueStatus.GetEnqueuedMessagesPerQueue())
            {
                statusBuilder.Append(String.Format("Queue: {0} - Messages: {1}", status.Key, status.Value));
                statusBuilder.Append("<br />");
            }

            var parameters = new List<String> { statusBuilder.ToString() };

            SendMailToAllDestinations(sender, parameters);
        }

        #endregion
    }
}
