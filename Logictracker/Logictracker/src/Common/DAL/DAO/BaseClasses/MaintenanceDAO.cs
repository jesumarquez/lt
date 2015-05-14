#region Usings

using System;
using System.Threading;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Criterion;

#endregion

namespace Logictracker.DAL.DAO.BaseClasses
{
    /// <summary>
    /// Data access class that contains all generic maintenance objects methods.
    /// </summary>
    /// <typeparam name="TDaotype"></typeparam>
    public abstract class MaintenanceDAO<TDaotype> : GenericDAO<TDaotype> where TDaotype : class, IAuditable
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        protected MaintenanceDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Delete all objects older than date.
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="n"></param>
        /// <param name="sleepInterval"></param>
        /// <param name="declareTransaction"></param>
        /// <param name="messageCount"></param>
        public void DeleteObjectsOlderThanDate(DateTime fecha, Int32 n, TimeSpan sleepInterval, Int32 messageCount)
        {
            try
            {
                var command = GetDeleteCommand();

                var result = DoDelete(command, n, fecha);

                while (result > 0)
                {
                    if (!sleepInterval.Equals(TimeSpan.Zero) && QueueStatus.QueueStatus.GetMaxEnqueuedMessagesCount() > messageCount) Thread.Sleep(sleepInterval);

                    result = DoDelete(command, n, fecha);
                }
            }
            catch (Exception ex) { throw new Exception("Error deleting possitions.", ex); }
        }

        /// <summary>
        /// Gets the first object.
        /// </summary>
        /// <returns></returns>
        public TDaotype GetFirstObject()
        {
            var objects = Session.CreateCriteria(typeof(TDaotype))
                .AddOrder(Order.Asc("Id"))
                .SetTimeout(0)
                .SetMaxResults(1)
                .List<TDaotype>();

            return objects == null || objects.Count.Equals(0) ? null : objects[0];
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the deletion sql command.
        /// </summary>
        /// <returns></returns>
        protected abstract String GetDeleteCommand();

        #endregion

        #region Private Methods

        /// <summary>
        /// Deletes the first n objects older than the specified date.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="n"></param>
        /// <param name="fecha"></param>
        /// <param name="declareTransaction"></param>
        /// <returns></returns>
        private int DoDelete(string command, int n, DateTime fecha)
        {
            var results = Session.CreateSQLQuery(command)
                .AddScalar("count", NHibernateUtil.Int32)
                .SetParameter("n", n)
                .SetParameter("date", fecha)
                .SetTimeout(0)
                .UniqueResult<Int32>();

            return results;
        }

        #endregion
    }
}