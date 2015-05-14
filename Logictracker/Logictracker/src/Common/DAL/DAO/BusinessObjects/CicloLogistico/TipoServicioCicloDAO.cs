using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico
{
    public class TipoServicioCicloDAO:GenericDAO<TipoServicioCiclo>
    {
//        public TipoServicioCicloDAO(ISession session): base(session) {}
        
        public TipoServicioCiclo FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .FirstOrDefault(r => !r.Baja && r.Codigo == code);
        }
        
        public List<TipoServicioCiclo> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .Where(r => !r.Baja)
                        .ToList();
        }
        
        public TipoServicioCiclo FindDefault(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        .FilterLinea(Session, empresas, lineas, null)
                        .FirstOrDefault(r => !r.Baja && r.Default);
        }

        public List<TipoServicioCiclo> FindDefaults(int empresa, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, lineas, null)
                        .Where(r => !r.Baja && r.Default)
                        .ToList();
        }
        
        public List<TipoServicioCiclo> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(r => !r.Baja)
                        .ToList();
        }
        
        public override void Delete(TipoServicioCiclo obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}
