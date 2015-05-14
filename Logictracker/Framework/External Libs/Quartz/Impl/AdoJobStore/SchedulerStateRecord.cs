#region Usings

using System;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary>
	/// Conveys a scheduler-instance state record.
	/// </summary>
	/// <author>James House</author>
	[Serializable]
	public class SchedulerStateRecord
	{
	    /// <summary>
	    /// Gets or sets the checkin interval.
	    /// </summary>
	    /// <value>The checkin interval.</value>
	    public virtual TimeSpan CheckinInterval { get; set; }

	    /// <summary>
	    /// Gets or sets the checkin timestamp.
	    /// </summary>
	    /// <value>The checkin timestamp.</value>
	    public virtual DateTime CheckinTimestamp { get; set; }

	    /// <summary>
	    /// Gets or sets the scheduler instance id.
	    /// </summary>
	    /// <value>The scheduler instance id.</value>
	    public virtual string SchedulerInstanceId { get; set; }
	}

}