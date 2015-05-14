#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class SistemaDAO: GenericDAO<Sistema>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public SistemaDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds all systems.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Sistema> FindAll()
        {
            return Session.Query<Sistema>()
                .Where(s => s.Enabled == 1)
                .Cacheable()
                .ToList();
        }

        /// <summary>
        /// Deletes the system associated to the givenn id.
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        public override void Delete(Sistema system)
        {
            if (system == null) return;

            system.Enabled = 0;

            SaveOrUpdate(system);
        }

        #endregion
    }
}