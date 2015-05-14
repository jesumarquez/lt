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
    /// Task for defragment database indexes.
    /// </summary>
    public class IndexDefragmentation : BaseSleepTask
    {
        #region Private Properties

        /// <summary>
        /// Page percentage to be used for updating stadistics.
        /// </summary>
        private const String Percentage = "Percentage";

        /// <summary>
        /// Number of indexes to be defragmented.
        /// </summary>
        private Int32 _indexes;

        #endregion

        #region Private Classes

        /// <summary>
        /// Class that represents the information of the index to be defragmented.
        /// </summary>
        private class Index
        {
            public String TableName { get; set; }
            public String IndexName { get; set; }
            public String Command { get; set; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            base.OnExecute(timer);

            var indexes = GetIndexesToDefragmentate();

            foreach (var index in indexes) Defragmentate(index);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Defragmentates the specified index.
        /// </summary>
        /// <param name="index"></param>
        private void Defragmentate(Index index)
        {
			STrace.Trace(GetType().FullName, String.Format("Performing defragmentation of index: {0} ({1})", index.IndexName, index.TableName));

            using (var command = DaoFactory.CreateCommand())
            {
                command.CommandText = index.Command;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 0;

                command.ExecuteNonQuery();

                STrace.Trace(GetType().FullName, String.Format("{0} indexes to defragment.", --_indexes));

                DoSleep();
            }
        }

        /// <summary>
        /// Gets the list of all indeces that need to be defragmented.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Index> GetIndexesToDefragmentate()
        {
            var script = GetIndexesCommand();

            using (var command = DaoFactory.CreateCommand())
            {
                command.CommandText = script;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 0;

                return MapResults(command);
            }
        }

        /// <summary>
        /// Maps sql results into bussiness objects.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private IEnumerable<Index> MapResults(IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                var results = new List<Index>();

                while (reader.Read())
                {
                    var index = new Index { TableName = reader.GetString(0), IndexName = reader.GetString(1), Command = reader.GetString(2) };

                    results.Add(index);
                }

                _indexes = results.Count;

                return results;
            }
        }

        /// <summary>
        /// Gets the script for gathering information about the indexes that need to be defragmented.
        /// </summary>
        /// <returns></returns>
        private String GetIndexesCommand()
        {
            var percentage = GetPercentage();

            return String.Format(@"declare @scripts table
                (
                    ObjectName varchar(max),
                    ObjectId int,
                    IndexName varchar(max),
                    IndexId int,
                    Level int,
                    Pages int,
                    Rows int,
                    MinimumRecordSize int,
                    MaximumRecordSize int,
                    AverageRecordSize int,
                    ForwardedRecords int,
                    Extents int,
                    ExtentSwitches int,
                    AverageFreeBytes int,
                    AveragePageDensity int,
                    ScanDensity int,
                    BestCount int,
                    ActualCount int,
                    LogicalFragmentation int,
                    ExtentFragmentation int
                )

                DECLARE @tablename VARCHAR(128)

                DECLARE tables CURSOR LOCAL FOR
                    SELECT '[' + TABLE_SCHEMA + ']' + '.' + '[' + TABLE_NAME + ']'
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE = 'BASE TABLE'
                        AND	TABLE_NAME NOT LIKE 'MS%'
                        AND	TABLE_NAME NOT LIKE 'sys%'
                        AND	TABLE_SCHEMA <> 'INFORMATION_SCHEMA'
                        AND	TABLE_SCHEMA <> 'SYS'
                		
                OPEN tables

                FETCH NEXT FROM tables INTO @tablename

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    INSERT INTO @scripts
                    EXEC ('DBCC SHOWCONTIG (''' + @tablename + ''') WITH FAST, TABLERESULTS, ALL_INDEXES, NO_INFOMSGS')
                    FETCH NEXT FROM tables INTO @tablename
                END

                CLOSE tables
                DEALLOCATE tables

                SELECT ObjectName,
                    IndexName,
                    'DBCC INDEXDEFRAG (0, ' + RTRIM(ObjectId) + ', ' + RTRIM(IndexId) + ')'
                FROM @scripts
                WHERE LogicalFragmentation >= {0}
                    AND INDEXPROPERTY (ObjectId, IndexName, 'IndexDepth') > 0", percentage);
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
