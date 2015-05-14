using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Criterion;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class LastVehicleEventDAO : GenericDAO<LastVehicleEvent>
    {
        #region private methods

        private DetachedCriteria makeDetachedCriteriaFor(int top, Coche vehiculo, int type)
        {
            return makeDetachedCriteriaFor(top, new [] {vehiculo}, type);
        }

        private DetachedCriteria makeDetachedCriteriaFor(int top, Coche[] vehiculos, int type)
        {
            var dc = DetachedCriteria.For<LastVehicleEvent>()
                .Add(Restrictions.In("Vehiculo", vehiculos))
                .Add(Restrictions.Eq("TipoEvento", type));

            if (top > 0)
                dc.SetMaxResults(top);

            return dc;
        }

        #endregion private methods

//        public LastVehicleEventDAO(ISession session) : base(session) { }

        public IList<LogMensaje> GetEventsByVehiclesAndType(Coche[] vehiculos, int type)
        {            
            var table = Ids2DataTable(vehiculos);

            var sqlQ =
                Session.CreateSQLQuery("exec [dbo].[sp_LastVehicleEventDAO_GetEventsByVehiclesAndType] @vehiculosIds = :ids, @eventType = :eventType;")
                    .AddEntity(typeof (LogMensaje))
                    .SetStructured("ids", table)
                    .SetInt32("eventType", type);            
            var results = sqlQ.List<LogMensaje>();
            return results;
        }

        public LogMensaje GetEventByVehicleAndType(Coche vehiculo, int type)
        {
            var events = GetEventsByVehiclesAndType(new[] {vehiculo}, type);
            return events.FirstOrDefault();
        }

        public void Save(LogMensaje logMensaje, int type)
        {
            using (var tx = SmartTransaction.BeginTransaction())
            {
                var dc = makeDetachedCriteriaFor(1, logMensaje.Coche, type).SetProjection(Projections.Property("Id"));

                var last = Session.CreateCriteria<LastVehicleEvent>().Add(Subqueries.PropertyIn("Id", dc)).UniqueResult<LastVehicleEvent>();

                if (last == null)
                {
                    last = new LastVehicleEvent {TipoEvento = type, Vehiculo = logMensaje.Coche};
                }

                last.LogMensaje = logMensaje;
                SaveOrUpdate(last);
                tx.Commit();
            }
        }
    }
}