using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoZona<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZona)
            where TQuery : IHasTipoZona
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoZona(q, session, empresas, lineas, tiposZona, user);
        }
        public static IQueryable<TQuery> FilterTipoZona<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZona, Usuario user)
            where TQuery : IHasTipoZona
        {
            var tiposU = GetTipoZona(session, empresas, lineas, tiposZona, user);
            if (tiposU != null) q = q.Where(c => c.TipoZona == null || tiposU.Contains(c.TipoZona));

            return q;
        }

        private static List<TipoZona> GetTipoZona(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZona)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoZona(session, empresas, lineas, tiposZona, user);
        }
        private static List<TipoZona> GetTipoZona(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZona, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposZona)) return null;

            var tiposQ = session.Query<TipoZona>()
                                .FilterEmpresa(session, empresas, user)
                                .FilterLinea(session, empresas, lineas, user);

            var tiposU = tiposQ.Cacheable().ToList();

            if (!IncludesAll(tiposZona)) tiposU = tiposU.Where(l => tiposZona.Contains(l.Id)).ToList();

            return tiposU;
        }
    }
}
