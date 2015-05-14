#region Usings

#if NET_20
using Common.Logging;
using Quartz.Spi;
using NullableDateTime = System.Nullable<System.DateTime>;
#else
using Nullables;
#endif

#endregion

namespace Quartz.Core
{
	/// <summary> 
	/// An interface to be used by <see cref="IJobStore" /> instances in order to
	/// communicate signals back to the <see cref="QuartzScheduler" />.
	/// </summary>
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public class SchedulerSignalerImpl : ISchedulerSignaler
	{
		private readonly ILog log = LogManager.GetLogger(typeof (SchedulerSignalerImpl));
        protected QuartzScheduler sched;
        protected QuartzSchedulerThread schedThread;

        public SchedulerSignalerImpl(QuartzScheduler sched, QuartzSchedulerThread schedThread)
        {
            this.sched = sched;
            this.schedThread = schedThread;

            log.Info("Initialized Scheduler Signaller of type: " + GetType());
        }


        /// <summary>
        /// Notifies the scheduler about misfired trigger.
        /// </summary>
        /// <param name="trigger">The trigger that misfired.</param>
		public virtual void NotifyTriggerListenersMisfired(Trigger trigger)
		{
			try
			{
				sched.NotifyTriggerListenersMisfired(trigger);
			}
			catch (SchedulerException se)
			{
				log.Error("Error notifying listeners of trigger misfire.", se);
				sched.NotifySchedulerListenersError("Error notifying listeners of trigger misfire.", se);
			}
		}


        /// <summary>
        /// Notifies the scheduler about finalized trigger.
        /// </summary>
        /// <param name="trigger">The trigger that has finalized.</param>
        public void NotifySchedulerListenersFinalized(Trigger trigger)
        {
            sched.NotifySchedulerListenersFinalized(trigger);
        }

		/// <summary>
		/// Signals the scheduling change.
		/// </summary>
        public void SignalSchedulingChange(NullableDateTime candidateNewNextFireTime)
        {
            schedThread.SignalSchedulingChange(candidateNewNextFireTime);
        }
	}
}