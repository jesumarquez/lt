#region Usings

using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Tickets;
using NHibernate;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Tickets
{
    public class DetalleCicloDAO: GenericDAO<DetalleCiclo>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public DetalleCicloDAO(ISession session) : base(session) { }

        #endregion
    }
}
