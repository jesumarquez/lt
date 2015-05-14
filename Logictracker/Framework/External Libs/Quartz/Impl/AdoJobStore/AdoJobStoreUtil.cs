#region Usings

using System;
using System.Globalization;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary> 
	/// This class contains utility functions for use in all delegate classes.
	/// </summary>
	/// <author><a href="mailto:jeff@binaryfeed.org">Jeffrey Wescott</a></author>
	public sealed class AdoJobStoreUtil
	{
	    private AdoJobStoreUtil()
	    {
	    }

	    /// <summary>
		/// Replace the table prefix in a query by replacing any occurrences of
		/// "{0}" with the table prefix.
		/// </summary>
		/// <param name="query">The unsubstitued query</param>
		/// <param name="tablePrefix">The table prefix</param>
		/// <returns>The query, with proper table prefix substituted</returns>
		public static string ReplaceTablePrefix(string query, string tablePrefix)
		{
			return string.Format(CultureInfo.InvariantCulture, query, tablePrefix);
		}

		/// <summary>
		/// Obtain a unique key for a given job.
		/// </summary>
		/// <param name="jobName">The job name</param>
		/// <param name="groupName">The group containing the job
		/// </param>
        /// <returns>A unique <see cref="string" /> key </returns>
		internal static string GetJobNameKey(string jobName, string groupName)
		{
			return String.Intern(string.Format(CultureInfo.InvariantCulture, "{0}_$x$x$_{1}", groupName, jobName));
		}

		/// <summary>
		/// Obtain a unique key for a given trigger.
		/// </summary>
		/// <param name="triggerName">The trigger name</param>
		/// <param name="groupName">The group containing the trigger</param>
        /// <returns>A unique <see cref="string" /> key</returns>
		internal static string GetTriggerNameKey(string triggerName, string groupName)
		{
			return String.Intern(string.Format(CultureInfo.InvariantCulture, "{0}_$x$x$_{1}", groupName, triggerName));
		}
	}
}