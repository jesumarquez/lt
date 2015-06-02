using System;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate.Criterion;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class LogMensajeAdminDAO : MaintenanceDAO<LogMensajeAdmin>
    {
        protected override string GetDeleteCommand() { return "delete top(:n) from opeeven12 where opeeven12_datetime <= :date ; select @@ROWCOUNT as count;"; }

        public int Count(int vehiculo, DateTime desde, DateTime hasta)
        {
            return Session.QueryOver<LogMensajeAdmin>()
                          .Where(m => m.Coche.Id == vehiculo)
                          .And(m => m.Fecha >= desde && m.Fecha <= hasta)
                          .Select(Projections.RowCount())
                          .SingleOrDefault<int>();
        }
    }
}