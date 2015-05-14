#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

#endregion

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    /// <summary>
    /// Tasks for updating indexes stadistics.
    /// </summary>
    public class UpdateStadistics : BaseSleepTask
    {
        #region Private Classes

        /// <summary>
        /// Class that cointains information about te index to be updated.
        /// </summary>
        private class Stadistic
        {
            public String Table { get; set; }
            public String Index { get; set; }
            public String Command { get; set; }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Page percentage to be used for updating stadistics.
        /// </summary>
        private const String Percentage = "Percentage";

        /// <summary>
        /// Number of stadistics to be updated.
        /// </summary>
        private Int32 _stadistics;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            base.OnExecute(timer);

            var indexes = GetIndexesToUpdate();

            foreach (var stadistic in indexes) Update(stadistic);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the specified stadistic.
        /// </summary>
        /// <param name="stadistic"></param>
        private void Update(Stadistic stadistic)
        {
            STrace.Trace(GetType().FullName, String.Format("Updating stadistics for index: {0} ({1})", stadistic.Index, stadistic.Table));

            using (var command = DaoFactory.CreateCommand())
            {
                command.CommandText = stadistic.Command;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 0;

                command.ExecuteNonQuery();

                STrace.Trace(GetType().FullName, String.Format("{0} stadistics to update.", --_stadistics));

                DoSleep();
            }
        }

        /// <summary>
        /// Gets a list of stadistics to be updated.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Stadistic> GetIndexesToUpdate()
        {
            using (var command = DaoFactory.CreateCommand())
            {
                command.CommandText = GetUpdateIndexesScript();
                command.CommandType = CommandType.Text;

                return MapResults(command);
            }
        }

        /// <summary>
        /// Maps sql results into bussiness objects.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private IEnumerable<Stadistic> MapResults(IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                var results = new List<Stadistic>();

                while (reader.Read())
                {
                    var stadistic = new Stadistic { Table = reader.GetString(0), Index = reader.GetString(1), Command = reader.GetString(2) };

                    results.Add(stadistic);
                }

                _stadistics = results.Count;

                STrace.Trace(GetType().FullName, String.Format("{0} stadistics to update.", _stadistics));

                return results;
            }
        }

        /// <summary>
        /// Gets information about the indexes that need to be updated.
        /// </summary>
        /// <returns></returns>
        private String GetUpdateIndexesScript()
        {
            var percentage = GetPercentage();

            return String.Format(@"SELECT SCHEMA_NAME(A.SCHEMA_ID) + '.' + OBJECT_NAME(A.OBJECT_ID) AS TABLENAME,
		            B.NAME AS INDEXNAME,
		            'UPDATE STATISTICS [' + SCHEMA_NAME(A.SCHEMA_ID) + '].[' + OBJECT_NAME(A.OBJECT_ID) + '] [' + B.NAME + '] WITH SAMPLE {0} PERCENT;' as update_statistics,
		            STATS_DATE(A.object_id, B.index_id) AS statistics_update_date
	            FROM SYS.OBJECTS A
	            JOIN SYS.INDEXES B ON A.OBJECT_ID = B.OBJECT_ID          
	            where b.index_id <> 0
		            AND A.TYPE = 'U'
		            AND b.type <> 3
		            AND b.type <> 4
		            AND upper(OBJECT_NAME(A.OBJECT_ID)) not like 'SYS%'
		            AND upper(OBJECT_NAME(A.OBJECT_ID)) not like 'MS%'
		            AND	UPPER(SCHEMA_NAME(A.SCHEMA_ID)) <> 'SYS'
		            AND UPPER(SCHEMA_NAME(A.SCHEMA_ID)) <> 'INFORMATION_SCHEMA'
	            order by 4 ASC", percentage);
        }

        /// <summary>
        /// Gets the page percentage for updating stadistics.
        /// </summary>
        /// <returns></returns>
        private Int32 GetPercentage()
        {
            var percentage = GetInt32(Percentage);

            return percentage.HasValue ? percentage.Value : 30;
        }

        #endregion
    }
}