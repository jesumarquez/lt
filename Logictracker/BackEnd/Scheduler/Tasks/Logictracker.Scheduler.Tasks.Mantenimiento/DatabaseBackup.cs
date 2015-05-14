#region Usings

using System;
using System.Data;
using System.IO;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

#endregion

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    /// <summary>
    /// Task for performing database backup.
    /// </summary>
    public class DatabaseBackup : BaseTask
    {
        #region Private Properties

        /// <summary>
        /// Compression configuration variable name.
        /// </summary>
		private const String Compression = "Compresion";

        #endregion

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
                var backupDirectory = GetString("BackupDirectory");
                var fileName = String.Format("{0}_{1:0000}{2:00}{3:00}.bak", database, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var backup = String.Concat(backupDirectory, fileName);

                var directory = new DirectoryInfo(backupDirectory);

                if (!directory.Exists) directory.Create();

                STrace.Trace(GetType().FullName, String.Format("Performing database backup: {0} - {1}", database, backup));

                command.CommandType = CommandType.Text;
                command.CommandTimeout = 0;
                command.CommandText = GetBackupCommand(database, backup);
                
                command.ExecuteNonQuery();

                foreach (var file in directory.GetFiles().Where(file => !file.Name.Equals(fileName))) file.Delete();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if compression is enabled.
        /// </summary>
        /// <returns></returns>
        private Boolean IsCompressionEnables()
        {
            var compression = GetBoolean(Compression);

            return compression.HasValue && compression.Value;
        }

        /// <summary>
        /// Gets the backup sql command text.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="backup"></param>
        /// <returns></returns>
        private String GetBackupCommand(String database, String backup)
        {
            var compression = String.Concat(IsCompressionEnables() ? "COMPRESSION, " : String.Empty, "STATS = 10");

            return String.Format("BACKUP DATABASE [{0}] TO DISK = '{1}' WITH NOFORMAT, NOINIT, NAME = '{0}', SKIP, NOREWIND, NOUNLOAD, {2}", database, backup, compression);
        }

        #endregion
    }
}