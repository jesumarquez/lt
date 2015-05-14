#region Usings

using System;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using NHibernate;

#endregion

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    /// <summary>
    /// Task for maintaining trace database tables.
    /// </summary>
    public class TraceMaintenance : BaseTask
    {
        #region Private Properties

        /// <summary>
        /// Min log datetime to keep.
        /// </summary>
        private DateTime _minDate;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            var daysToKeep = GetInt32("DaysToKeep");

            _minDate = DateTime.UtcNow.AddDays(daysToKeep.HasValue ? -(daysToKeep.Value) : -7);

            DeleteLogContexts();

            DeleteLogs();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Deletes old logs contexts.
        /// </summary>
        private void DeleteLogContexts()
        {
			STrace.Trace(GetType().FullName, "Deleting old log contexts.");

            Delete("delete from audlog02 where rela_audlog01 in (select id_audlog01 from audlog01 where audlog01_datetime <= :date)");
        }

        /// <summary>
        /// Delete old logs.
        /// </summary>
        private void DeleteLogs()
        {
			STrace.Trace(GetType().FullName, "Deleting old logs.");

            Delete("delete from audlog01 where audlog01_datetime <= :date");
        }

        /// <summary>
        /// Performs a database delete with the givenn query.
        /// </summary>
        /// <param name="sql"></param>
        private void Delete(String sql)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    DaoFactory.CreateSqlQuery(String.Format("{0} ; select @@rowcount as count;", sql))
                        .AddScalar("count", NHibernateUtil.Int32)
                        .SetParameter("date", _minDate)
                        .SetTimeout(0)
                        .UniqueResult();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    STrace.Exception(GetType().FullName, ex);
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        #endregion
    }
}
