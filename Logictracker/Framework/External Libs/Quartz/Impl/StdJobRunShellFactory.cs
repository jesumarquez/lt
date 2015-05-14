#region Usings

using Quartz.Core;
using Quartz.Spi;

#endregion

namespace Quartz.Impl
{
	/// <summary> 
	/// Responsible for creating the instances of <see cref="JobRunShell" />
	/// to be used within the <see cref="QuartzScheduler" /> instance.
	/// <p>
	/// This implementation does not re-use any objects, it simply makes a new
	/// JobRunShell each time <see cref="BorrowJobRunShell()" /> is called.
	/// </p>
	/// </summary>
	/// <author>James House</author>
	public class StdJobRunShellFactory : IJobRunShellFactory
	{
		private IScheduler scheduler;
		private SchedulingContext schedCtxt;

		/// <summary>
		/// Initialize the factory, providing a handle to the <see cref="IScheduler" />
		/// that should be made available within the <see cref="JobRunShell" /> and
		/// the <see cref="JobExecutionContext" /> s within it, and a handle to the
		/// <see cref="SchedulingContext" /> that the shell will use in its own
		/// operations with the <see cref="IJobStore" />.
		/// </summary>
		public virtual void Initialize(IScheduler sched, SchedulingContext ctx)
		{
			scheduler = sched;
			schedCtxt = ctx;
		}

		/// <summary>
		/// Called by the <see cref="QuartzSchedulerThread" /> to obtain instances of 
		/// <see cref="JobRunShell" />.
		/// </summary>
		public virtual JobRunShell BorrowJobRunShell()
		{
			return new JobRunShell(this, scheduler, schedCtxt);
		}

		/// <summary>
		/// Called by the <see cref="QuartzSchedulerThread" /> to return instances of 
		/// <see cref="JobRunShell" />.
		/// </summary>
		public virtual void ReturnJobRunShell(JobRunShell jobRunShell)
		{
			jobRunShell.Passivate();
		}
	}
}