using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class PlanDAO : GenericDAO<Plan>
    {
//        public PlanDAO(ISession session) : base(session) { }

        public IEnumerable<Plan> GetList(IEnumerable<int> empresas, IEnumerable<int> tiposLinea)
        {
            return Query.Where(p => (empresas.Contains(0) || empresas.Contains(-1) || empresas.Contains(p.Empresa))
                                 && (tiposLinea.Contains(0) || tiposLinea.Contains(-1) || tiposLinea.Contains(p.TipoLinea))
                                 && !p.Baja)
                        .ToList();
        }

        public override void Delete(Plan obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}