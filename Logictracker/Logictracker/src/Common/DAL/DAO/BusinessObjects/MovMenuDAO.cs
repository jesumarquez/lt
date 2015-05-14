#region Usings

using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class MovMenuDAO: GenericDAO<MovMenu>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public MovMenuDAO(ISession session) : base(session) { }

        #endregion
    }
}