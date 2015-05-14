#region Usings

using Quartz.Spi;

#endregion

namespace Quartz.Core
{
	/// <summary>
	/// Responsible for creating the instances of <see cref="JobRunShell" />
	/// to be used within the <see cref="QuartzScheduler" /> instance.
	/// </summary>
	/// <remarks>
	/// Although this interface looks a lot like an 'object pool', implementations
	/// do not have to support the re-use of instances. If an implementation does
	/// not wish to pool instances, then the <see cref="BorrowJobRunShell()" />
	/// method would simply create a new instance, and the <see cref="ReturnJobRunShell" /> 
	/// method would do nothing.
    /// </remarks>
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public interface IJobRunShellFactory
	{
		/// <summary>
		/// Initialize the factory, providing a handle to the <see cref="IScheduler" />
		/// that should be made available within the <see cref="JobRunShell" /> and
		/// the <see cref="JobExecutionContext" /> s within it, and a handle to the
		/// <see cref="SchedulingContext" /> that the shell will use in its own
		/// operations with the <see cref="IJobStore" />.
		/// </summary>
		void Initialize(IScheduler sched, SchedulingContext ctx);

		/// <summary>
		/// Called by the <see cref="QuartzSchedulerThread" />
		/// to obtain instances of <see cref="JobRunShell" />.
		/// </summary>
		JobRunShell BorrowJobRunShell();

		/// <summary>
		/// Called by the <see cref="QuartzSchedulerThread" />
		/// to return instances of <see cref="JobRunShell" />.
		/// </summary>
		void ReturnJobRunShell(JobRunShell jobRunShell);
	}
}