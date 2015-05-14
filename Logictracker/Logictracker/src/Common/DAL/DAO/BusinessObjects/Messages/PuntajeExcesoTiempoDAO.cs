#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    /// <summary>
    /// Excess time points data access class.
    /// </summary>
    public class PuntajeExcesoTiempoDAO : GenericDAO<PuntajeExcesoTiempo>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public PuntajeExcesoTiempoDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all available points.
        /// </summary>
        /// <returns></returns>
        public List<PuntajeExcesoTiempo> GetPoints() { return Session.Query<PuntajeExcesoTiempo>().Cacheable().ToList(); }

        #endregion
    }
}