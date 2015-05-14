#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

#endregion

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    /// <summary>
    /// Task for maintaining database messages.
    /// </summary>
    public class MantenimientoMensajes : BaseSleepTask
    {
        #region Private Properties

        /// <summary>
        /// Configuration constant keys.
        /// </summary>
        private const String RowCount = "RowCount";

        /// <summary>
        /// Number of affected rows per cicle.
        /// </summary>
        private Int32 _n;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            base.OnExecute(timer);

            var today = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59, 990);
            
            GetRowCount();

            DeleOldMessages(today.AddYears(-1));

            DeleteOldDiscartedMessages(today.AddMonths(-1));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Deletes old discarted messages older than the specified date.
        /// </summary>
        /// <param name="oneMonthAgo"></param>
        private void DeleteOldDiscartedMessages(DateTime oneMonthAgo)
        {
            var firstMessage = GetFirstDiscartedMessage();

            while (firstMessage <= oneMonthAgo)
            {
                STrace.Trace(GetType().FullName, String.Format("Deleting discarted messages older than: {0}.", firstMessage));

                DaoFactory.LogMensajeDescartadoDAO.DeleteObjectsOlderThanDate(firstMessage, _n, SleepInterval, MaxMessageCount);

                firstMessage = firstMessage.AddDays(1);
            }
        }

        /// <summary>
        /// Gets the date of the first discarted message.
        /// </summary>
        /// <returns></returns>
        private DateTime GetFirstDiscartedMessage()
        {
            var firstMessage = DaoFactory.LogMensajeDescartadoDAO.GetFirstObject();

            return firstMessage != null ? new DateTime(firstMessage.Fecha.Year, firstMessage.Fecha.Month, firstMessage.Fecha.Day, 23, 59, 59, 990) : DateTime.MinValue;
        }

        /// <summary>
        /// Deletes messages older than the specified date.
        /// </summary>
        /// <param name="oneYearAgo"></param>
        private void DeleOldMessages(DateTime oneYearAgo)
        {
            var firstMessage = GetFirstMessage();

            while (firstMessage <= oneYearAgo)
            {
                STrace.Trace(GetType().FullName, String.Format("Deleting messages older than: {0}.", firstMessage));

                DaoFactory.LogMensajeDAO.DeleteObjectsOlderThanDate(firstMessage, _n, SleepInterval, MaxMessageCount);

                firstMessage = firstMessage.AddDays(1);
            }
        }

        /// <summary>
        /// Gets the date of the first message.
        /// </summary>
        /// <returns></returns>
        private DateTime GetFirstMessage()
        {
            var firstMessage = DaoFactory.LogMensajeDAO.GetFirstObject();

            return firstMessage != null ? new DateTime(firstMessage.Fecha.Year, firstMessage.Fecha.Month, firstMessage.Fecha.Day, 23, 59, 59, 990) : DateTime.MinValue;
        }

        /// <summary>
        /// Gets the affected row count for deletes.
        /// </summary>
        /// <returns></returns>
        private void GetRowCount()
        {
            var n = GetInt32(RowCount);

            _n = n.HasValue ? n.Value : 50000;
        }

        #endregion
    }
}
