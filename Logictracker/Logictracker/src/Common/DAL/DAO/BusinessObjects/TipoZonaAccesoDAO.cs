using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class TipoZonaAccesoDAO : GenericDAO<TipoZonaAcceso>
    {
//        public TipoZonaAccesoDAO(ISession session) : base(session) { }

        public List<TipoZonaAcceso> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .Where(z => !z.Baja)
                        .ToList();
        }

        public override void Delete(TipoZonaAcceso obj)
        {
            if (obj == null) return;

            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public bool IsCodeUnique(int empresa, int linea, int idTipoZonaAcceso, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .FirstOrDefault(g => g.Id != idTipoZonaAcceso && g.Codigo == code && !g.Baja) == null;
        }
    }
}
