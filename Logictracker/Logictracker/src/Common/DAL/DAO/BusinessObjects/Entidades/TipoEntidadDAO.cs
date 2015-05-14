using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Entidades;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects.Entidades
{
    public class TipoEntidadDAO : GenericDAO<TipoEntidad>
    {
//        public TipoEntidadDAO(ISession session) : base(session) { }

        public List<TipoEntidad> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            var q = Query.FilterEmpresa(Session, empresas);

            if (!QueryExtensions.IncludesAll(lineas))
                q = q.FilterLinea(Session, empresas, lineas);

            return q.Where(m => !m.Baja)
                    .ToList();
        }

        public override void Delete(TipoEntidad obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        }

        public bool IsCodeUnique(int empresa, int linea, int idTipoEntidad, string code)
        {
            var q = Query.FilterEmpresa(Session, new[] { empresa }, null);

            if (linea != -1)
                q = q.FilterLinea(Session, new[] { empresa }, new[] { linea }, null);

            return q.FirstOrDefault(g => g.Id != idTipoEntidad
                                         && g.Codigo == code
                                         && !g.Baja) == null;
        }

        public TipoEntidad FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            var q = Query.FilterEmpresa(Session, empresas);

            if (!QueryExtensions.IncludesAll(lineas))
                q = q.FilterLinea(Session, empresas, lineas);

            q = q.Where(t => !t.Baja);

            return q.FirstOrDefault(t => t.Codigo == code);
        }
    }
}