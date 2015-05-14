#region Usings

using System;

#endregion

namespace Quartz.Core
{
	/// <summary>
	/// An object used to pass information about the 'client' to the <see cref="QuartzScheduler" />.
	/// </summary>
	/// <seealso cref="QuartzScheduler" />
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	[Serializable]
	public class SchedulingContext
	{
	    /// <summary>
	    /// get the instanceId in the cluster.
	    /// </summary>
	    /// <summary> <p>
	    /// Set the instanceId.
	    /// </p>
	    /// </summary>
	    public virtual string InstanceId { get; set; }
	}
}