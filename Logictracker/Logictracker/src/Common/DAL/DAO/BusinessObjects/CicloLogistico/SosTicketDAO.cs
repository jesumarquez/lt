using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico
{
    public class SosTicketDAO : GenericDAO<SosTicket>
    {
        public SosTicket FindByCodigo(string codigo)
        {
            return Query.FirstOrDefault(t => t.NumeroServicio == codigo);
        }
    }
}
