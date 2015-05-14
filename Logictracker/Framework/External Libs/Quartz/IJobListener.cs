namespace Quartz
{
	/// <summary>
	/// The interface to be implemented by classes that want to be informed when a
	/// <see cref="JobDetail" /> executes. In general,  applications that use a 
	/// <see cref="IScheduler" /> will not have use for this mechanism.
	/// </summary>
	/// <seealso cref="IScheduler" />
	/// <seealso cref="IJob" />
	/// <seealso cref="JobExecutionContext" />
	/// <seealso cref="JobExecutionException" />
	/// <seealso cref="ITriggerListener" />
	/// <author>James House</author>
	public interface IJobListener
	{
		/// <summary>
		/// Get the name of the <see cref="IJobListener" />.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Called by the <see cref="IScheduler" /> when a <see cref="JobDetail" />
		/// is about to be executed (an associated <see cref="Trigger" />
		/// has occured).
		/// <p>
		/// This method will not be invoked if the execution of the Job was vetoed
		/// by a <see cref="ITriggerListener" />.
		/// </p>
		/// </summary>
		/// <seealso cref="JobExecutionVetoed(JobExecutionContext)" />
		void JobToBeExecuted(JobExecutionContext context);

		/// <summary>
		/// Called by the <see cref="IScheduler" /> when a <see cref="JobDetail" />
		/// was about to be executed (an associated <see cref="Trigger" />
		/// has occured), but a <see cref="ITriggerListener" /> vetoed it's 
		/// execution.
		/// </summary>
		/// <seealso cref="JobToBeExecuted(JobExecutionContext)" />
		void JobExecutionVetoed(JobExecutionContext context);


		/// <summary>
		/// Called by the <see cref="IScheduler" /> after a <see cref="JobDetail" />
		/// has been executed, and be for the associated <see cref="Trigger" />'s
		/// <see cref="Trigger.Triggered" /> method has been called.
		/// </summary>
		void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException);
	}
}