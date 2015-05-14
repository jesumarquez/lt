using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Tickets;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Tickets
{
    public class DetalleTicketDAO : GenericDAO<DetalleTicket>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public DetalleTicketDAO(ISession session) : base(session) { }

        #endregion
    }
}