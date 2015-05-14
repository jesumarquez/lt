#region Usings

using Common.Logging;

#endregion

namespace Quartz.Listener
{
    /// <summary>
    /// A helpful abstract base class for implementors of <see cref="IJobListener" />.
    /// </summary>
    /// <remarks>
    /// <p>
    /// The methods in this class are empty so you only need to override the  
    /// subset for the <see cref="IJobListener" /> events you care about.
    /// </p>
    /// 
    /// <p>
    /// You are required to implement <see cref="IJobListener.Name" /> 
    /// to return the unique name of your <see cref="IJobListener" />.  
    /// </p>
    /// </remarks>
    /// <seealso cref="IJobListener" />
    public abstract class JobListenerSupport : IJobListener
    {
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobListenerSupport"/> class.
        /// </summary>
        protected JobListenerSupport()
        {
            log = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Get the <see cref="ILog" /> for this  class's category.  
        /// This should be used by subclasses for logging.
        /// </summary>
        protected ILog Log
        {
            get { return log; }
        }

        /// <summary>
        /// Get the name of the <see cref="IJobListener"/>.
        /// </summary>
        /// <value></value>
        public abstract string Name { get; }

        /// <summary>
        /// Called by the <see cref="IScheduler"/> when a <see cref="JobDetail"/>
        /// is about to be executed (an associated <see cref="Trigger"/>
        /// has occured).
        /// <p>
        /// This method will not be invoked if the execution of the Job was vetoed
        /// by a <see cref="ITriggerListener"/>.
        /// </p>
        /// </summary>
        /// <param name="context"></param>
        /// <seealso cref="JobExecutionVetoed(JobExecutionContext)"/>
        public virtual void JobToBeExecuted(JobExecutionContext context)
        {
        }

        /// <summary>
        /// Called by the <see cref="IScheduler"/> when a <see cref="JobDetail"/>
        /// was about to be executed (an associated <see cref="Trigger"/>
        /// has occured), but a <see cref="ITriggerListener"/> vetoed it's
        /// execution.
        /// </summary>
        /// <param name="context"></param>
        /// <seealso cref="JobToBeExecuted(JobExecutionContext)"/>
        public virtual void JobExecutionVetoed(JobExecutionContext context)
        {
        }

        /// <summary>
        /// Called by the <see cref="IScheduler"/> after a <see cref="JobDetail"/>
        /// has been executed, and be for the associated <see cref="Trigger"/>'s
        /// <see cref="Trigger.Triggered"/> method has been called.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        public virtual void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException)
        {
        }
    }
}