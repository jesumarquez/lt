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
    public class EntregaDistribucionDAO:GenericDAO<EntregaDistribucion>
    {
//        public EntregaDistribucionDAO(ISession session) : base(session) {}

        public List<EntregaDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> vehiculos, IEnumerable<int> viajes, IEnumerable<int> estados, DateTime? desde, DateTime? hasta)
        {
            return GetList(empresas, lineas, transportistas, new[] { -1 }, new[] { -1 }, new[] { -1 }, vehiculos, viajes, estados, desde, hasta);
        }

        public List<EntregaDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centros, IEnumerable<int> subcentros, IEnumerable<int> vehiculos, IEnumerable<int> viajes, IEnumerable<int> estados, DateTime? desde, DateTime? hasta)
        {
            var estadosList = estados.ToList();

            var q = Query.FilterViajeDistribucion(Session, empresas, lineas, transportistas, departamentos, centros, subcentros, vehiculos, viajes, desde, hasta);

            if (!estadosList.Contains(-2)) q = q.Where(t => estadosList.Contains(t.Estado));
            
            return q.ToList();
        }

        private DetachedCriteria GetEntregaDistribucionDetachedCriteria()
        {
            var dc = DetachedCriteria.For<EntregaDistribucion>("ded")
                .Add(Restrictions.EqProperty("ded.Id", "ed.Id"))
                .SetProjection(Projections.Property("Id"));
            return dc;
        }

        private DetachedCriteria GetEntregaDistribucionDetachedCriteria(ViajeDistribucion viaje)
        {
            var dc = GetEntregaDistribucionDetachedCriteria()
                            .Add(Restrictions.Eq("Viaje.Id", viaje.Id));
            return dc;
        }

        private ICriteria GetEntregaDistribucionCriteria(int top, DetachedCriteria dc, Order order)
        {
            var crit = Session.CreateCriteria<EntregaDistribucion>("ed")                                
                .Add(Subqueries.Exists(dc));

            if (top > 0)
                crit.SetMaxResults(top);

            if (order != null)
                crit.AddOrder(order);

            return crit;
        }
       

        public double GetKMCalculados(ViajeDistribucion viaje)
        {
            var dc = GetEntregaDistribucionDetachedCriteria(viaje);

            var crit = GetEntregaDistribucionCriteria(0, dc, null)
                .SetProjection(Projections.ProjectionList().Add(Projections.Sum("KmCalculado")));

            return crit.UniqueResult<double>();
        }

        public IList<EntregaDistribucion> GetEntregas(ViajeDistribucion viaje, bool includeBase)
        {
            var dc = GetEntregaDistribucionDetachedCriteria(viaje);

            if (!includeBase)
                dc.Add(Restrictions.IsNull("Linea"));

            var crit = GetEntregaDistribucionCriteria(0, dc, null);
            return crit.List<EntregaDistribucion>();
        }

        public IList<int> GetReferenciasGeograficasIds(ViajeDistribucion viaje)
        {
            var dc = Session.CreateCriteria<EntregaDistribucion>("ed")
                            .CreateAlias("PuntoEntrega", "pe", JoinType.InnerJoin)
                            .CreateAlias("PuntoEntrega.ReferenciaGeografica", "rg", JoinType.InnerJoin)
                            .Add(Restrictions.Eq("ed.Viaje.Id", viaje.Id))
                            .SetProjection(Projections.Property("rg.Id"));

            return dc.List<int>();
        }
    }
}
