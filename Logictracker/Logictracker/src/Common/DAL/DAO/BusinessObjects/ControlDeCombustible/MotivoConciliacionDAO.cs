#region Usings

using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using NHibernate;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.ControlDeCombustible
{
    public class MotivoConciliacionDAO: GenericDAO<MotivoConciliacion>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public MotivoConciliacionDAO(ISession session) : base(session) { }

        #endregion
    }
}
