#region Usings

using System;
using System.Globalization;
using System.IO;
using Common.Logging;

#endregion

namespace Quartz.Job
{
	/// <summary> 
	/// Inspects a file and compares whether it's "last modified date" has changed
	/// since the last time it was inspected.  If the file has been updated, the
	/// job invokes a "call-back" method on an identified 
	/// <see cref="IFileScanListener" /> that can be found in the 
	/// <see cref="SchedulerContext" />.
	/// </summary>
	/// <author>James House</author>
	/// <seealso cref="IFileScanListener" />
	public class FileScanJob : IStatefulJob
	{
		public const string FileName = "FILE_NAME";
		public const string FileScanListenerName = "FILE_SCAN_LISTENER_NAME";
		private const string LastModifiedTime = "LAST_MODIFIED_TIME";
		private readonly ILog log;


        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
	    protected ILog Log
	    {
	        get { return log; }
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanJob"/> class.
        /// </summary>
	    public FileScanJob()
	    {
	        log = LogManager.GetLogger(typeof (FileScanJob));
	    }

	    /// <summary>
		/// Called by the <see cref="IScheduler" /> when a <see cref="Trigger" />
		/// fires that is associated with the <see cref="IJob" />.
		/// <p>
		/// The implementation may wish to set a  result object on the
		/// JobExecutionContext before this method exits.  The result itself
		/// is meaningless to Quartz, but may be informative to
		/// <see cref="IJobListener" />s or
		/// <see cref="ITriggerListener" />s that are watching the job's
		/// execution.
		/// </p>
		/// </summary>
		/// <param name="context">The execution context.</param>
		/// <seealso cref="IJob">
		/// </seealso>
		public virtual void Execute(JobExecutionContext context)
		{
			var data = context.MergedJobDataMap;
			SchedulerContext schedCtxt;
			try
			{
				schedCtxt = context.Scheduler.Context;
			}
			catch (SchedulerException e)
			{
				throw new JobExecutionException("Error obtaining scheduler context.", e, false);
			}

			var fileName = data.GetString(FileName);
			var listenerName = data.GetString(FileScanListenerName);

			if (fileName == null)
			{
				throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture, "Required parameter '{0}' not found in JobDataMap", FileName));
			}
			if (listenerName == null)
			{
				throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture, "Required parameter '{0}' not found in JobDataMap", FileScanListenerName));
			}

			var listener = (IFileScanListener) schedCtxt[listenerName];

			if (listener == null)
			{
				throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture, "FileScanListener named '{0}' not found in SchedulerContext", listenerName));
			}

			var lastDate = DateTime.MinValue;
			if (data.Contains(LastModifiedTime))
			{
				lastDate = data.GetDateTime(LastModifiedTime);
			}

			var newDate = GetLastModifiedDate(fileName);

			if (newDate == DateTime.MinValue)
			{
				Log.Warn(string.Format(CultureInfo.InvariantCulture, "File '{0}' does not exist.", fileName));
				return;
			}

			if (lastDate != DateTime.MinValue && (newDate != lastDate))
			{
				// notify call back...
				Log.Info(string.Format(CultureInfo.InvariantCulture, "File '{0}' updated, notifying listener.", fileName));
				listener.FileUpdated(fileName);
			}
			else
			{
				Log.Debug(string.Format(CultureInfo.InvariantCulture, "File '{0}' unchanged.", fileName));
			}

			context.JobDetail.JobDataMap.Put(LastModifiedTime, newDate);
		}

		/// <summary>
		/// Gets the last modified date.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		protected internal virtual DateTime GetLastModifiedDate(string fileName)
		{
			var file = new FileInfo(fileName);

			bool tmpBool;
			if (File.Exists(file.FullName))
			{
				tmpBool = true;
			}
			else
			{
				tmpBool = Directory.Exists(file.FullName);
			}
			if (!tmpBool)
			{
				return DateTime.MinValue;
			}
			else
			{
				return file.LastWriteTime;
			}
		}
	}
}
