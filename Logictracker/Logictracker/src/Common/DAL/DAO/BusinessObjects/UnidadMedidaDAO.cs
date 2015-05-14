using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class UnidadMedidaDAO : GenericDAO<UnidadMedida>
    {
//        public UnidadMedidaDAO(ISession session) : base(session) { }

        public List<UnidadMedida> GetList()
        {
            return Query.ToList();
        }

        public UnidadMedida FindByCode(string code)
        {
            return Query.FirstOrDefault(um => um.Codigo.Equals(code));
        }
    }
}