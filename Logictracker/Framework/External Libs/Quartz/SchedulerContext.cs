#region Usings

using System;
using System.Collections;
using Quartz.Util;

#endregion

namespace Quartz
{
	/// <summary>
	/// Holds context/environment data that can be made available to Jobs as they
	/// are executed. 
	/// </summary>
	/// <seealso cref="IScheduler.Context" />
	/// <author>James House</author>
	[Serializable]
    public class SchedulerContext : StringKeyDirtyFlagMap
	{

		/// <summary>
		/// Create an empty <see cref="JobDataMap" />.
		/// </summary>
		public SchedulerContext() : base(15)
		{
		}

		/// <summary>
		/// Create a <see cref="JobDataMap" /> with the given data.
		/// </summary>
		public SchedulerContext(IDictionary map) : this()
		{
			PutAll(map);
		}

	}
}