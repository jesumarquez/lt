#region Usings

using System;
using Quartz.Util;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary>
	/// Conveys the state of a fired-trigger record.
	/// </summary>
	/// <author>James House</author>
	[Serializable]
	public class FiredTriggerRecord
	{
	    /// <summary>
	    /// Gets or sets the fire instance id.
	    /// </summary>
	    /// <value>The fire instance id.</value>
	    public virtual string FireInstanceId { get; set; }

	    /// <summary>
	    /// Gets or sets the fire timestamp.
	    /// </summary>
	    /// <value>The fire timestamp.</value>
	    public virtual long FireTimestamp { get; set; }

	    /// <summary>
	    /// Gets or sets a value indicating whether [job is stateful].
	    /// </summary>
	    /// <value><c>true</c> if [job is stateful]; otherwise, <c>false</c>.</value>
	    public virtual bool JobIsStateful { get; set; }

	    /// <summary>
	    /// Gets or sets the job key.
	    /// </summary>
	    /// <value>The job key.</value>
	    public virtual Key JobKey { get; set; }

	    /// <summary>
	    /// Gets or sets the scheduler instance id.
	    /// </summary>
	    /// <value>The scheduler instance id.</value>
	    public virtual string SchedulerInstanceId { get; set; }

	    /// <summary>
	    /// Gets or sets the trigger key.
	    /// </summary>
	    /// <value>The trigger key.</value>
	    public virtual Key TriggerKey { get; set; }

	    /// <summary>
	    /// Gets or sets the state of the fire instance.
	    /// </summary>
	    /// <value>The state of the fire instance.</value>
	    public virtual string FireInstanceState { get; set; }

	    /// <summary>
	    /// Gets or sets a value indicating whether [job requests recovery].
	    /// </summary>
	    /// <value><c>true</c> if [job requests recovery]; otherwise, <c>false</c>.</value>
	    public virtual bool JobRequestsRecovery { get; set; }

	    /// <summary>
	    /// Gets or sets a value indicating whether [trigger is volatile].
	    /// </summary>
	    /// <value><c>true</c> if [trigger is volatile]; otherwise, <c>false</c>.</value>
	    public virtual bool TriggerIsVolatile { get; set; }

	    /// <summary>
	    /// Gets or sets the priority.
	    /// </summary>
	    /// <value>The priority.</value>
	    public int Priority { get; set; }
	}

}
