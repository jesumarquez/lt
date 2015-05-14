#region Usings

using System;
using System.Data;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

#endregion

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    /// <summary>
    /// Task for performing database log file shrink.
    /// </summary>
    public class ShrinkLogFile : BaseTask
    {
        #region Protected Methods

        /// <summary>
        /// Process handler main task.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            using (var command = DaoFactory.CreateCommand())
            {
                var database = GetString("Database");
                var logFile = GetString("LogFile");

                STrace.Trace(GetType().FullName, String.Format("Shrinking database log file: {0} - {1}", database, logFile));

                command.CommandType = CommandType.Text;
                command.CommandTimeout = 0;
				command.CommandText = String.Format("USE [{0}] DBCC SHRINKFILE (N'{1}' , 0, TRUNCATEONLY)", database, logFile);

                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
