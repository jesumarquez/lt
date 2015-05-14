#region Usings

using System;
using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Positions;
using NHibernate;
using NHibernate.Criterion;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Positions
{
    public class LogPosicionHistoricaDAO : MaintenanceDAO<LogPosicionHistorica>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public LogPosicionHistoricaDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the last position for the specified device id and date.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IList<LogPosicionHistorica> GetPositionsBetweenDates(int id, DateTime desde, DateTime hasta)
        {
            return Session.CreateCriteria(typeof(LogPosicionHistorica))
                .SetTimeout(0)
                .CreateAlias("Coche", "c")
                .Add(Restrictions.Eq("c.Id", id))
                .Add(Restrictions.Ge("FechaMensaje", desde))
                .Add(Restrictions.Le("FechaMensaje", hasta))
                .AddOrder(Order.Asc("FechaMensaje"))
                .List<LogPosicionHistorica>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the positions deletion sql command.
        /// </summary>
        /// <returns></returns>
        protected override String GetDeleteCommand()
        {
            return "delete top(:n) from [logictracker_history].[dbo].[ope.ope_posi_01_log_posiciones] where opeposi01_fechora <= :date ; select @@ROWCOUNT as count;";
        }

        #endregion
    }
}
