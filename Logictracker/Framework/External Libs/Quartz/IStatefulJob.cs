namespace Quartz
{
	/// <summary>
	/// A marker interface for <see cref="JobDetail" /> s that
	/// wish to have their state maintained between executions.
	/// </summary>
	/// <remarks>
	/// <see cref="IStatefulJob" /> instances follow slightly different rules from
	/// regular <see cref="IJob" /> instances. The key difference is that their
	/// associated <see cref="JobDataMap" /> is re-persisted after every
	/// execution of the job, thus preserving state for the next execution. The
	/// other difference is that stateful jobs are not allowed to Execute
	/// concurrently, which means new triggers that occur before the completion of
	/// the <see cref="IJob.Execute" /> method will be delayed.
	/// </remarks>
	/// <seealso cref="IJob" />
	/// <seealso cref="JobDetail" />
	/// <seealso cref="JobDataMap" />
	/// <seealso cref="IScheduler" /> 
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public interface IStatefulJob : IJob
	{
	}
}