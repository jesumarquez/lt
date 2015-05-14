using System.Collections.Generic;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Sync;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Criterion;
using System;

namespace Logictracker.DAL.DAO.BusinessObjects.Sync
{
    public class OutQueueDAO : GenericDAO<OutQueue>
    {
//        public OutQueueDAO(ISession session) : base(session) { }

        public IList<OutQueue> GetNovedades(int empresa, int linea, string query, string server)
        {
            OutQueue q = null;
            OutState s = null;
            var sq = QueryOver.Of(() => s)
                .Where(Restrictions.EqProperty(Projections.Property(() => q.Id), Projections.Property(() => s.OutQueue.Id)))
                .And(Restrictions.Eq(Projections.Property(()=>s.Server), server))
                .And(Restrictions.Eq(Projections.Property(()=>s.Sincronizado), true));


            var qy = Session.QueryOver(()=>q);
            if(empresa > 0)
            {
                qy = qy.Where(Restrictions.Or(Restrictions.Eq(Projections.Property(() => q.Empresa.Id), empresa),
                                      Restrictions.IsNull(Projections.Property(() => q.Empresa))));
                if (linea > 0)
                {
                    qy = qy.And(Restrictions.Or(Restrictions.Eq(Projections.Property(() => q.Linea.Id), linea),
                                                Restrictions.IsNull(Projections.Property(() => q.Linea))));
                }
            }
            return qy.WithSubquery.WhereNotExists(sq.Select(x => x.Id))
                .OrderBy(Projections.Property<OutQueue>(p => p.Prioridad)).Desc
                .ThenBy(Projections.Property<OutQueue>(p => p.FechaAlta)).Asc
                .List();
        }

        public void Enqueue<T>(T obj, string query, string operacion) where T: ISecurable, IAuditable
        {
            if (obj.Empresa == null) return;
            switch(query)
            {
                case OutQueue.Queries.Molinete: if (!obj.Empresa.LogicoutMolinete) return; break;
                default: return;
            }

            var q = new OutQueue
                        {
                            Empresa = obj.Empresa,
                            Linea = obj.Linea,
                            FechaAlta = DateTime.UtcNow,
                            Query = query,
                            Operacion = operacion,
                            Parametros = obj.Id.ToString(),
                            Transaction = 0,
                            Prioridad = 0
                        };

            Save(q);
        }
    }
}