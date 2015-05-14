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
    /// Task for gathering information about the current expensive queries.
    /// </summary>
    public class ExpensiveQueries : BaseTask
    {
        #region Private Classes

        /// <summary>
        /// Class that represents information about a expensive query.
        /// </summary>
        private class ExpensiveQuery
        {
            public String Text { get; set; }
            public Int64 AvgCpuTime { get; set; }
            public Int64 AvgLogicalWrites { get; set; }
            public Int64 AvgLogicalReads { get; set; }
            public Int64 AvgPhysicalReads { get; set; }
            public Int64 AvgElapsedTime { get; set; }
            public Int64 AvgClrTime { get; set; }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Category constants definition.
        /// </summary>
        private const String ClrTime = "clr time";
        private const String ElapsedTime = "elapsed time";
        private const String PhysicalReads = "physical reads";
        private const String LogicalReads = "logical reads";
        private const String LogicalWrites = "logical writes";
        private const String CpuTime = "cpu time";

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs task main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
			STrace.Trace(GetType().FullName, "Gathering expensive queries information.");

            TraceQueries(GetQueries(GetClrTimeScript()), ClrTime);

            TraceQueries(GetQueries(GetElapsedTimeScript()), ElapsedTime);

            TraceQueries(GetQueries(GetPhysicalReadsScript()), PhysicalReads);

            TraceQueries(GetQueries(GetLogicalReadsScript()), LogicalReads);

            TraceQueries(GetQueries(GetLogicalWritesScript()), LogicalWrites);

            TraceQueries(GetQueries(GetCpuTimeScript()), CpuTime);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Trace information about each offensive query of the specified category.
        /// </summary>
        /// <param name="queries"></param>
        /// <param name="category"></param>
        private void TraceQueries(ICollection<ExpensiveQuery> queries, String category)
        {
            STrace.Trace(GetType().FullName, String.Format("Retrieving information about expensive queries for the following category: {0}", category));

            if (queries.Count.Equals(0))
            {
	            STrace.Trace(GetType().FullName, String.Format("No expensive queries found for category: {0}", category));
            }
            else
			{
				foreach (var expensiveQuery in queries)
					STrace.Trace(GetType().FullName, String.Format("Refference: {0} - Script: {1}", GetRefference(expensiveQuery, category), expensiveQuery.Text));
			}
        }

        /// <summary>
        /// Gets refference value for the current expensive query depending on the specified category.
        /// </summary>
        /// <param name="expensiveQuery"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private static Int64 GetRefference(ExpensiveQuery expensiveQuery, String category)
        {
            switch (category)
            {
                case ClrTime: return expensiveQuery.AvgClrTime;
                case ElapsedTime: return expensiveQuery.AvgElapsedTime;
                case PhysicalReads: return expensiveQuery.AvgPhysicalReads;
                case LogicalReads: return expensiveQuery.AvgLogicalReads;
                case LogicalWrites: return expensiveQuery.AvgLogicalWrites;
                case CpuTime: return expensiveQuery.AvgCpuTime;
            }

            return 0;
        }

        /// <summary>
        /// Gets information about the expensive queries associated to the specified sql script.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private ICollection<ExpensiveQuery> GetQueries(String sql)
        {
            using (var command = DaoFactory.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 0;

                return MapResults(command.ExecuteReader());
            }
        }

        /// <summary>
        /// Maps sql results into bussiness objects.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static ICollection<ExpensiveQuery> MapResults(IDataReader reader)
        {
            using (reader)
            {
                var expensiveQueries = new List<ExpensiveQuery>();

                while (reader.Read())
                {
                    var expensiveQuerie = new ExpensiveQuery
                                              {
                                                  AvgClrTime = reader.GetInt64(5),
                                                  AvgCpuTime = reader.GetInt64(0),
                                                  AvgElapsedTime = reader.GetInt64(4),
                                                  AvgLogicalReads = reader.GetInt64(2),
                                                  AvgLogicalWrites = reader.GetInt64(1),
                                                  AvgPhysicalReads = reader.GetInt64(3),
                                                  Text = reader.GetString(6)
                                              };

                    expensiveQueries.Add(expensiveQuerie);
                }

                return expensiveQueries;
            }
        }

        /// <summary>
        /// Gets the expensive queries base script.
        /// </summary>
        /// <returns></returns>
        private static String GetBaseScript()
        {
            return @"
                SELECT TOP 10
                        total_worker_time / execution_count,
                        total_logical_writes / execution_count,
                        total_logical_reads / execution_count,
                        total_physical_reads / execution_count,
                        total_elapsed_time / execution_count,
                        total_clr_time / execution_count,
                        SUBSTRING(execText.text, deqs.statement_start_offset / 2, 
                                  ( ( CASE WHEN deqs.statement_end_offset = -1
                                     THEN DATALENGTH(execText.text)
                                     ELSE deqs.statement_end_offset
                                     END ) - deqs.statement_start_offset ) / 2)
                FROM    sys.dm_exec_query_stats deqs
                        CROSS APPLY sys.dm_exec_sql_text(deqs.plan_handle) AS execText";
        }

        /// <summary>
        /// Gets the expensive queries script ordered by cpu time.
        /// </summary>
        /// <returns></returns>
        private static String GetCpuTimeScript() { return String.Format("{0} ORDER BY 1 DESC", GetBaseScript()); }

        /// <summary>
        /// Gets the expensive queries script ordered by logical writes time.
        /// </summary>
        /// <returns></returns>
        private static String GetLogicalWritesScript() { return String.Format("{0} ORDER BY 2 DESC", GetBaseScript()); }

        /// <summary>
        /// Gets the expensive queries script ordered by logical reads time.
        /// </summary>
        /// <returns></returns>
        private static String GetLogicalReadsScript() { return String.Format("{0} ORDER BY 3 DESC", GetBaseScript()); }

        /// <summary>
        /// Gets the expensive queries script ordered by physical reads time.
        /// </summary>
        /// <returns></returns>
        private static String GetPhysicalReadsScript() { return String.Format("{0} ORDER BY 4 DESC", GetBaseScript()); }

        /// <summary>
        /// Gets the expensive queries script ordered by elapsed time.
        /// </summary>
        /// <returns></returns>
        private static String GetElapsedTimeScript() { return String.Format("{0} ORDER BY 5 DESC", GetBaseScript()); }

        /// <summary>
        /// Gets the expensive queries script ordered by clr time.
        /// </summary>
        /// <returns></returns>
        private static String GetClrTimeScript() { return String.Format("{0} ORDER BY 6 DESC", GetBaseScript()); }

        #endregion
    }
}
