using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Ordenes;
using NHibernate.Bytecode;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Ordenes
{
    public class OrderDAO : GenericDAO<Order>
    {
        public List<Order> FindByCustomer(Empresa customer)
        {
            //return Query.Where(f => f.Empresa.Equals(customer)).ToList();
            return new List<Order>(Session.Query<Order>().Where(f => f.Empresa.Id == customer.Id && (!f.Programado) )).ToList();
        }
    }
}
