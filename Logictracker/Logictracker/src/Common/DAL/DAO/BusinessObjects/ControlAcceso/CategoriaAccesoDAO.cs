using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.ControlAcceso;
using Logictracker.Utils;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects.ControlAcceso
{
    public class CategoriaAccesoDAO : GenericDAO<CategoriaAcceso>
    {
//        public CategoriaAccesoDAO(ISession session) : base(session) { }

        public CategoriaAcceso FindByName(IEnumerable<int> empresas, IEnumerable<int> lineas, string name)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(x => (!x.Baja) && (x.Nombre == name)).SafeFirstOrDefault();
        }
        public List<CategoriaAcceso> FindList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(x => !x.Baja)
                .ToList();
        }

        public List<CategoriaAcceso> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .Where(x => !x.Baja)
                .ToList();
        }

        public override void Delete(CategoriaAcceso obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }
    }
}
