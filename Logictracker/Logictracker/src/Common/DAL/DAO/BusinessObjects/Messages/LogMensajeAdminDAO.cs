using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class LogMensajeAdminDAO : MaintenanceDAO<LogMensajeAdmin>
    {
        protected override string GetDeleteCommand() { return "delete top(:n) from opeeven12 where opeeven12_datetime <= :date ; select @@ROWCOUNT as count;"; }
    }
}