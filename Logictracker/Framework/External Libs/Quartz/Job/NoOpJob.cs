namespace Quartz.Job
{
	/// <summary>
	/// An implementation of Job, that does absolutely nothing - useful for system
	/// which only wish to use <see cref="ITriggerListener" />s
	/// and <see cref="IJobListener" />s, rather than writing
	/// Jobs that perform work.
	/// </summary>
	/// <author>James House</author>
	public class NoOpJob : IJob
	{
		/// <summary>
		/// Do nothing.
		/// </summary>
		public void Execute(JobExecutionContext context)
		{
		}
	}
}