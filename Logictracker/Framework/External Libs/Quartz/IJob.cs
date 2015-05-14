namespace Quartz
{
	/// <summary> 
	/// The interface to be implemented by classes which represent a 'job' to be
	/// performed.
	/// </summary>
	/// <remarks>
	/// Instances of this interface must have a <see langword="public" />
	/// no-argument constructor. <see cref="JobDataMap" /> provides a mechanism for 'instance member data'
	/// that may be required by some implementations of this interface.
    /// </remarks>
	/// <seealso cref="JobDetail" />
	/// <seealso cref="IStatefulJob" />
	/// <seealso cref="Trigger" />
	/// <seealso cref="IScheduler" />
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public interface IJob
	{
		/// <summary>
		/// Called by the <see cref="IScheduler" /> when a <see cref="Trigger" />
		/// fires that is associated with the <see cref="IJob" />.
        /// </summary>
		/// <remarks>
		/// The implementation may wish to set a  result object on the 
		/// JobExecutionContext before this method exits.  The result itself
		/// is meaningless to Quartz, but may be informative to 
		/// <see cref="IJobListener" />s or 
		/// <see cref="ITriggerListener" />s that are watching the job's 
		/// execution.
		/// </remarks>
		/// <param name="context">The execution context.</param>
		void Execute(JobExecutionContext context);
	}
}