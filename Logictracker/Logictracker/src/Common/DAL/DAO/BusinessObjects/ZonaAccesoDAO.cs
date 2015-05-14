using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class ZonaAccesoDAO : GenericDAO<ZonaAcceso>
    {
//        public ZonaAccesoDAO(ISession session) : base(session) { }

        public List<ZonaAcceso> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZonaAcceso)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTipoZonaAcceso(Session, empresas, lineas, tiposZonaAcceso)
                        .Where(z => !z.Baja)
                        .ToList();
        }

        public override void Delete(ZonaAcceso obj)
        {
            if (obj == null) return;

            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public bool IsCodeUnique(int empresa, int linea, int idZonaAcceso, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .FirstOrDefault(g => g.Id != idZonaAcceso && g.Codigo == code && !g.Baja) == null;
        }
    }
}
