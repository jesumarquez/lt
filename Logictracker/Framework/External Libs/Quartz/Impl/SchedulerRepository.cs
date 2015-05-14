#region Usings

using System;
using System.Collections;
using System.Globalization;

#endregion

namespace Quartz.Impl
{
	/// <summary>
	/// Holds references to Scheduler instances - ensuring uniqueness, and
	/// preventing garbage collection, and allowing 'global' lookups.
	/// </summary>
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public class SchedulerRepository
	{
        private readonly IDictionary schedulers;
        private static readonly SchedulerRepository inst = new SchedulerRepository();
        private readonly object syncRoot = new object();
        
        /// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <value>The instance.</value>
		public static SchedulerRepository Instance
		{
			get { return inst; }
		}

		private SchedulerRepository()
		{
			schedulers = new Hashtable();
		}

        /// <summary>
        /// Binds the specified sched.
        /// </summary>
        /// <param name="sched">The sched.</param>
		public virtual void Bind(IScheduler sched)
		{
			lock (syncRoot)
			{
				if (schedulers[sched.SchedulerName] != null)
				{
					throw new SchedulerException(string.Format(CultureInfo.InvariantCulture, "Scheduler with name '{0}' already exists.", sched.SchedulerName),
					                             SchedulerException.ErrorBadConfiguration);
				}

				schedulers[sched.SchedulerName] = sched;
			}
		}

        /// <summary>
        /// Removes the specified sched name.
        /// </summary>
        /// <param name="schedName">Name of the sched.</param>
        /// <returns></returns>
		public virtual bool Remove(string schedName)
		{
			lock (syncRoot)
			{
				Object tempObject;
				tempObject = schedulers[schedName];
				schedulers.Remove(schedName);
				return (tempObject != null);
			}
		}

        /// <summary>
        /// Lookups the specified sched name.
        /// </summary>
        /// <param name="schedName">Name of the sched.</param>
        /// <returns></returns>
		public virtual IScheduler Lookup(string schedName)
		{
			lock (syncRoot)
			{
				return (IScheduler) schedulers[schedName];
			}
		}

        /// <summary>
        /// Lookups all.
        /// </summary>
        /// <returns></returns>
		public virtual ICollection LookupAll()
		{
			lock (syncRoot)
			{
				return ArrayList.ReadOnly(new ArrayList(schedulers.Values));
			}
		}
	}
}
