#region Usings

using Common.Logging;

#endregion

namespace Quartz.Listener
{
    /// <summary>
    ///  A helpful abstract base class for implementors of 
    /// <see cref="ITriggerListener" />.
    ///  </summary>
    /// <remarks>
    /// <p>
    /// The methods in this class are empty so you only need to override the  
    /// subset for the <see cref="ITriggerListener" /> events
    /// you care about.
    /// </p>
    /// 
    /// <p>
    /// You are required to implement <see cref="ITriggerListener.Name" /> 
    /// to return the unique name of your <see cref="ITriggerListener" />.  
    /// </p>
    ///</remarks>
    /// <seealso cref="ITriggerListener" />
    public abstract class TriggerListenerSupport : ITriggerListener
    {
        private readonly ILog log;


        protected TriggerListenerSupport()
        {
            log = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Get the <see cref="ILog" /> for this
        /// class's category.  This should be used by subclasses for logging.
        /// </summary>
        protected ILog Log
        {
            get { return log; }
        }

        /// <summary>
        /// Get the name of the <see cref="ITriggerListener"/>.
        /// </summary>
        /// <value></value>
        public abstract string Name { get; }

        public virtual void TriggerFired(Trigger trigger, JobExecutionContext context)
        {
        }

        public virtual bool VetoJobExecution(Trigger trigger, JobExecutionContext context)
        {
            return false;
        }

        public virtual void TriggerMisfired(Trigger trigger)
        {
        }

        public virtual void TriggerComplete(Trigger trigger, JobExecutionContext context, SchedulerInstruction triggerInstructionCode)
        {
        }
    }
}
