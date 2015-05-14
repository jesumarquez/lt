#region Usings

using System.Collections;
using Quartz.Impl;

#endregion

namespace Quartz
{
	/// <summary>
	/// Provides a mechanism for obtaining client-usable handles to <see cref="IScheduler" />
	/// instances.
	/// </summary>
	/// <seealso cref="IScheduler" />
	/// <seealso cref="StdSchedulerFactory" />
	/// <author>James House</author>
	public interface ISchedulerFactory
	{
		/// <summary>
		/// Returns handles to all known Schedulers (made by any SchedulerFactory
		/// within this app domain.).
		/// </summary>
		ICollection AllSchedulers { get; }

		/// <summary>
		/// Returns a client-usable handle to a <see cref="IScheduler" />.
		/// </summary>
		IScheduler GetScheduler();

		/// <summary>
		/// Returns a handle to the Scheduler with the given name, if it exists.
		/// </summary>
		IScheduler GetScheduler(string schedName);
	}
}