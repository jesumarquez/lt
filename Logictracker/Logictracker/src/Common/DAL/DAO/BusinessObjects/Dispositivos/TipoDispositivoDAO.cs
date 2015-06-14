#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    /// <summary>
    /// Device type data access class.
    /// </summary>
    public class TipoDispositivoDAO : GenericDAO<TipoDispositivo>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TipoDispositivoDAO(ISession session) : base(session) { }

        #endregion

        private DetachedCriteria getBaseDetachedCriteria(int top)
        {
            var dc = DetachedCriteria.For<TipoDispositivo>()
                            .Add(Restrictions.Eq("Baja", false))                            
                            .SetProjection(Projections.Property("Id"));
            if (top > 0)
                dc.SetMaxResults(top);

            return dc;
        }
        
        public TipoDispositivo FindByModelo(string modelo)
        {
            var dc = getBaseDetachedCriteria(1)
                .Add(Restrictions.Eq("Modelo", modelo));

            var c = Session.CreateCriteria<TipoDispositivo>()
                .Add(Subqueries.PropertyIn("Id", dc));

            return c.UniqueResult<TipoDispositivo>();
        }


        #region Public Methods

        /// <summary>
        /// Finds all active device types.
        /// </summary>
        /// <returns></returns>
        public override IQueryable<TipoDispositivo> FindAll()
        {
            var dc = getBaseDetachedCriteria(0);

            var c = Session.CreateCriteria<TipoDispositivo>()
                .Add(Subqueries.PropertyIn("Id", dc));

            return c.Future<TipoDispositivo>().AsQueryable();
        }

        /// <summary>
        /// Deletes the device type associated to the specified id.
        /// </summary>
        /// <param name="type"></param>
        public override void Delete(TipoDispositivo type)
        {
            if (type == null) return;

            type.Baja = true;

            SaveOrUpdate(type);
        }


        #endregion
    }
}