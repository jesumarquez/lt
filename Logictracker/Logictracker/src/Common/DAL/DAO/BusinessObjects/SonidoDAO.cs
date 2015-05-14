#region Usings

using System.Collections;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Criterion;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <summary>
    /// DAO de sonidos
    /// </summary>
    public class SonidoDAO : GenericDAO<Sonido>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public SonidoDAO(ISession session) : base(session) { }

        #endregion

        public new IList FindAll() { return Session.CreateCriteria(typeof (Sonido)).AddOrder(Order.Asc("Descripcion")).List(); }
    }
}