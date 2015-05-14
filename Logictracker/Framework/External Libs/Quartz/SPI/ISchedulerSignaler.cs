#region Usings

#if NET_20

using Quartz.Core;
using NullableDateTime = System.Nullable<System.DateTime>;
#else
using Nullables;
#endif

#endregion

namespace Quartz.Spi
{
	/// <summary> 
	/// An interface to be used by <see cref="IJobStore" /> instances in order to
	/// communicate signals back to the <see cref="QuartzScheduler" />.
	/// </summary>
	/// <author>James House</author>
	public interface ISchedulerSignaler
	{
		/// <summary>
		/// Notifies the scheduler about misfired trigger.
		/// </summary>
		/// <param name="trigger">The trigger that misfired.</param>
		void NotifyTriggerListenersMisfired(Trigger trigger);

        /// <summary>
        /// Notifies the scheduler about finalized trigger.
        /// </summary>
        /// <param name="trigger">The trigger that has finalized.</param>
        void NotifySchedulerListenersFinalized(Trigger trigger);

		/// <summary>
		/// Signals the scheduling change.
		/// </summary>
        void SignalSchedulingChange(NullableDateTime candidateNewNextFireTimeUtc);
	}
}