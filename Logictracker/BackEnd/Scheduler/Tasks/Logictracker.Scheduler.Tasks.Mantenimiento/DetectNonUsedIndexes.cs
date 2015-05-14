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
    /// Task for detecting non currently used indexes.
    /// </summary>
    public class DetectNonUsedIndexes : BaseTask
    {
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
        /// Performs task main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            var indexes = GetNonUsedIndexes();

            foreach (var index in indexes) STrace.Trace(GetType().FullName, String.Format("Non used index detected: {0} ({1}) - {2}", index.IndexName, index.TableName, index.Command));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the list of all indeces that are not being used.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Index> GetNonUsedIndexes()
        {
            var script = GetIndexesCommand();

            using (var command = DaoFactory.CreateCommand())
            {
                command.CommandText = script;
                command.CommandType = CommandType.Text;

                return MapResults(command);
            }
        }

        /// <summary>
        /// Gets the script for gathering information about the indexes that are not being used..
        /// </summary>
        /// <returns></returns>
        private static String GetIndexesCommand()
        {
            return @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DMDB_INDEX_USAGE_STATS]') AND type in (N'U'))
	                select * into dbo.DMDB_INDEX_USAGE_STATS from SYS.DM_DB_INDEX_USAGE_STATS where 1=0;

                insert dbo.DMDB_INDEX_USAGE_STATS
                select a.* 
                from SYS.DM_DB_INDEX_USAGE_STATS a
                left join dbo.DMDB_INDEX_USAGE_STATS b
	                on a.database_id = b.database_id 
	                and a.object_id = b.object_id
	                and a.index_id = b.index_id
                where b.database_id is null

                update b
                set	b.[last_user_seek] = ISNULL(a.[last_user_seek], b.[last_user_seek])
	                ,b.[last_user_scan] = ISNULL(a.[last_user_scan], b.[last_user_scan])
	                ,b.[last_user_lookup] = ISNULL(a.[last_user_lookup], b.[last_user_lookup])
	                ,b.[last_user_update] = ISNULL(a.[last_user_update], b.[last_user_update])
	                ,b.[last_system_seek] = ISNULL(a.[last_system_seek], b.[last_system_seek])
	                ,b.[last_system_scan] = ISNULL(a.[last_system_scan], b.[last_system_scan])
	                ,b.[last_system_lookup] = ISNULL(a.[last_system_lookup], b.[last_system_lookup])
	                ,b.[last_system_update] = ISNULL(a.[last_system_update], b.[last_system_update])
                from SYS.DM_DB_INDEX_USAGE_STATS a
                join dbo.DMDB_INDEX_USAGE_STATS b
	                on a.database_id = b.database_id 
	                and a.object_id = b.object_id
	                and a.index_id = b.index_id
                	
                if OBJECT_ID('tempdb..#indices_usados') is not null 
                    drop table #indices_usados
                                
                select	*
                into	#indices_usados
                from	dbo.DMDB_INDEX_USAGE_STATS
                where	1=0;

                insert	#indices_usados
                select	*
                from	dbo.DMDB_INDEX_USAGE_STATS
                where	
                    database_id = DB_ID()
                    AND index_id <> 0
                    AND upper(OBJECT_NAME(OBJECT_ID)) not like 'SYS%'
                    AND upper(OBJECT_NAME(OBJECT_ID)) not like 'MS%'

                if OBJECT_ID('tempdb..#todos_los_indices') is not null
                    drop table #todos_los_indices

                SELECT   
                    DB_NAME() AS DATABASENAME,
                    OBJECT_NAME(B.OBJECT_ID) AS TABLENAME,
                    B.NAME AS INDEXNAME,
                    'DROP INDEX ' + '[' + B.NAME + '] ON [dbo].[' + OBJECT_NAME(B.OBJECT_ID) + '] WITH ( ONLINE = OFF )' as DROPstatement,
                    B.INDEX_ID,
                    A.OBJECT_ID
                into	#todos_los_indices
                FROM	SYS.OBJECTS A
                JOIN	SYS.INDEXES B   
                    ON	A.OBJECT_ID = B.OBJECT_ID          
                where
                    b.index_id <> 0
                    AND A.TYPE = 'U'
                    AND upper(OBJECT_NAME(B.OBJECT_ID)) not like 'SYS%'
                    AND upper(OBJECT_NAME(B.OBJECT_ID)) not like 'MS%'
					AND upper(B.NAME) not like '%PK%'
					AND upper(B.NAME) not like '%UNIQUE%'

                SELECT   
                    TABLENAME,
                    INDEXNAME,
                    DROPstatement   
                FROM	#todos_los_indices AB
                LEFT JOIN #indices_usados C
                    ON	AB.OBJECT_ID = C.OBJECT_ID          
                    AND AB.INDEX_ID = C.INDEX_ID
                where
                    C.database_id is null
                ORDER BY 1, 2";
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

                STrace.Trace(GetType().FullName, String.Format("{0} indexes are not being used.", results.Count));

                return results;
            }
        }

        #endregion
    }
}
