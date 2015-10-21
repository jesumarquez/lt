using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion
{
    public class EstadoDistribucionDAO:GenericDAO<EstadoDistribucion>
    {
        private DetachedCriteria GetEstadoDistribucionDetachedCriteria()
        {
            var dc = DetachedCriteria.For<EstadoDistribucion>("ded")
                .Add(Restrictions.EqProperty("ded.Id", "ed.Id"))
                .SetProjection(Projections.Property("Id"));
            return dc;
        }

        private DetachedCriteria GetEstadoDistribucionDetachedCriteria(ViajeDistribucion viaje)
        {
            var dc = GetEstadoDistribucionDetachedCriteria()
                            .Add(Restrictions.Eq("Viaje.Id", viaje.Id));
            return dc;
        }

        private ICriteria GetEstadoDistribucionCriteria(int top, DetachedCriteria dc, Order order)
        {
            var crit = Session.CreateCriteria<EstadoDistribucion>("ed")
                .Add(Subqueries.Exists(dc));

            if (top > 0)
                crit.SetMaxResults(top);

            if (order != null)
                crit.AddOrder(order);

            return crit;
        }

        public IList<EstadoDistribucion> GetEstados(ViajeDistribucion viaje)
        {
            var dc = GetEstadoDistribucionDetachedCriteria(viaje);

            var crit = GetEstadoDistribucionCriteria(0, dc, null);
            return crit.List<EstadoDistribucion>();
        }
    }
}
