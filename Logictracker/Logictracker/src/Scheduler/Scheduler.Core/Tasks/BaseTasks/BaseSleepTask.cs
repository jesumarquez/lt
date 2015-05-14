#region Usings

using System;
using System.Threading;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Scheduler.Core.Tasks.BaseTasks
{
    /// <summary>
    /// Base class for tasks that requires sleep intervals.
    /// </summary>
    public class BaseSleepTask : BaseTask
    {
        #region Private Properties

        /// <summary>
        /// Configuration parameters keys.
        /// </summary>
        private const String Sleep = "SleepInterval";
        private const String MessageCount = "MaxMessageCount";

        #endregion

        #region Protected Properties

        /// <summary>
        /// Sleep interval between vehicles periods.
        /// </summary>
        protected TimeSpan SleepInterval;

        /// <summary>
        /// Gets the maximum message count allowed in any of the monitored queues.
        /// </summary>
        protected Int32 MaxMessageCount;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            GetSleepInterval();

            GetMaxMessages();
        }

        /// <summary>
        /// Sleeps the configured time span.
        /// </summary>
        protected void DoSleep()
        {
            if (SleepInterval.Equals(TimeSpan.Zero)) return;

            int maxCount;
            while ((maxCount = QueueStatus.QueueStatus.GetMaxEnqueuedMessagesCount()) >= MaxMessageCount)
            {
                STrace.Debug(GetType().FullName, String.Format("{0} mensajes encolados. Sleep {1} seg.", maxCount, SleepInterval.TotalSeconds));
                Thread.Sleep(SleepInterval);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the configured time span for sleep between vehicles.
        /// </summary>
        /// <returns></returns>
        private void GetSleepInterval()
        {
            var seconds = GetInt32(Sleep);

            SleepInterval = seconds.HasValue ? TimeSpan.FromSeconds(seconds.Value) : TimeSpan.Zero;
        }

        /// <summary>
        /// Gets the maximum messages refference value.
        /// </summary>
        private void GetMaxMessages()
        {
            var count = GetInt32(MessageCount);

            MaxMessageCount = count.HasValue ? count.Value : 1000;
        }

        #endregion
    }
}