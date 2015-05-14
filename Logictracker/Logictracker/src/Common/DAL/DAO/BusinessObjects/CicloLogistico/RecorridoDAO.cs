using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico
{
    public class RecorridoDAO:GenericDAO<Recorrido>
    {
//        public RecorridoDAO(ISession session): base(session) {}

        public Recorrido FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null).FirstOrDefault(r => !r.Baja && r.Codigo == code);
        }
        public List<Recorrido> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(r => !r.Baja)
                .ToList();
        }
        public override void Delete(Recorrido obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}
