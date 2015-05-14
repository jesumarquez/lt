#region Usings

using Common.Logging;

#endregion

namespace Quartz.Listener
{
    /// <summary>
    ///  A helpful abstract base class for implementors of 
    /// <see cref="ISchedulerListener" />.
    /// </summary>
    /// <remarks>
    /// The methods in this class are empty so you only need to override the  
    /// subset for the <see cref="ISchedulerListener" /> events you care about.
    /// </remarks>
    /// <seealso cref="ISchedulerListener" />
    public abstract class SchedulerListenerSupport : ISchedulerListener
    {
        private readonly ILog log;

        protected SchedulerListenerSupport()
        {
            log = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Get the <see cref="ILog" /> for this
        /// type's category.  This should be used by subclasses for logging.
        /// </summary>
        protected ILog Log
        {
            get { return log; }
        }

        public virtual void JobScheduled(Trigger trigger)
        {
        }

        public virtual void JobUnscheduled(string triggerName, string triggerGroup)
        {
        }

        public virtual void TriggerFinalized(Trigger trigger)
        {
        }

        public virtual void TriggersPaused(string triggerName, string triggerGroup)
        {
        }

        public virtual void TriggersResumed(string triggerName, string triggerGroup)
        {
        }

        public virtual void JobsPaused(string jobName, string jobGroup)
        {
        }

        public virtual void JobsResumed(string jobName, string jobGroup)
        {
        }

        public virtual void SchedulerError(string msg, SchedulerException cause)
        {
        }

        public virtual void SchedulerShutdown()
        {
        }
    }
}