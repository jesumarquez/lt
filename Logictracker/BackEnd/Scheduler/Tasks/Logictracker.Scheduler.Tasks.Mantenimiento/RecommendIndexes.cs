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
    /// Tasks for gathering missing indexes recommendations.
    /// </summary>
    public class RecommendIndexes : BaseTask
    {
        #region Private Classes

        /// <summary>
        /// Class that represents a missing index recommendation.
        /// </summary>
        private class MissingIndex
        {
            public String Script { get; set; }
            public Double Impact { get; set; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs task main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(GetType().FullName, "Gathering missing indexes recommendations.");

            var recommendations = GetRecommendations();

            STrace.Trace(GetType().FullName, String.Format("Found {0} missing idexes recommendatios.", recommendations.Count));

            if (recommendations.Count.Equals(0)) return;

            foreach (var recommendation in recommendations) TraceRecommendation(recommendation);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Trace into database information about the specified recommendation.
        /// </summary>
        /// <param name="recommendation"></param>
        private void TraceRecommendation(MissingIndex recommendation)
        {
            STrace.Trace(GetType().FullName, String.Format("Impact: {0} - Script: {1}", recommendation.Impact.ToString("0.00"), recommendation.Script));
        }

        /// <summary>
        /// Get current missing idexes recommendations.
        /// </summary>
        /// <returns></returns>
        private List<MissingIndex> GetRecommendations()
        {
            using (var command = DaoFactory.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = GetScript();
                command.CommandTimeout = 0;

                return MapResults(command.ExecuteReader());
            }
        }

        /// <summary>
        /// Maps query results into bussiness objects.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static List<MissingIndex> MapResults(IDataReader reader)
        {
            var recommendations = new List<MissingIndex>();

            while (reader.Read())
            {
                var recommendation = new MissingIndex {Script = reader.GetString(0), Impact = reader.GetDouble(1)};

                recommendations.Add(recommendation);
            }

            return recommendations;
        }

        /// <summary>
        /// Gets the sql script for retrieving recommendations.
        /// </summary>
        /// <returns></returns>
        private static String GetScript()
        {
            return @"
                SELECT
                    'CREATE INDEX [missing_index_' + CONVERT (varchar, mig.index_group_handle) + '_' + CONVERT (varchar, mid.index_handle)
                        + '_' + LEFT (PARSENAME(mid.statement, 1), 32) + ']'
                        + ' ON ' + mid.statement
                        + ' (' + ISNULL (mid.equality_columns,'')
                        + CASE WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN ',' ELSE '' END
                        + ISNULL (mid.inequality_columns, '')
                        + ')'
                        + ISNULL (' INCLUDE (' + mid.included_columns + ')', '') AS create_index_statement,
                    migs.avg_user_impact
                FROM sys.dm_db_missing_index_groups mig
                INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
                INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
                WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
                ORDER BY migs.avg_total_user_cost * migs.avg_user_impact * (migs.user_seeks + migs.user_scans) DESC";
        }

        #endregion
    }
}
