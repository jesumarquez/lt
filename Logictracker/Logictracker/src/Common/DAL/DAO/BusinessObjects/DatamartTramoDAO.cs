using NHibernate;
using System;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.ReportObjects.Datamart;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class DatamartTramoDAO : GenericDAO<DatamartTramo>
    {
        public void DeleteRecords(int vehicle, DateTime desde, DateTime hasta)
        {
            using (var tx = SmartTransaction.BeginTransaction())
            {
                try
                {
                    Session.CreateSQLQuery("delete from opeposi10 where rela_parenti03 = :coche and opeposi10_inicio >= :desde and opeposi10_inicio <= :hasta ; select @@rowcount as count;")
                           .AddScalar("count", NHibernateUtil.Int32)
                           .SetParameter("coche", vehicle)
                           .SetParameter("desde", desde)
                           .SetParameter("hasta", hasta)
                           .SetTimeout(0)
                           .UniqueResult();
                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    throw new Exception(String.Format("Error deleting datamart tramo records for vehicle {0} from {1} to {2} ", vehicle, desde, hasta), ex);
                }
            }
        }
    }
}