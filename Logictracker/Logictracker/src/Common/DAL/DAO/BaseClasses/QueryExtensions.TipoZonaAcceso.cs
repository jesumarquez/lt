using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoZonaAcceso<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZonaAcceso)
            where TQuery : IHasTipoZonaAcceso
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoZonaAcceso(q, session, empresas, lineas, tiposZonaAcceso, user);
        }
        public static IQueryable<TQuery> FilterTipoZonaAcceso<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZonaAcceso, Usuario user)
            where TQuery : IHasTipoZonaAcceso
        {
            var tiposU = GetTipoZonaAcceso(session, empresas, lineas, tiposZonaAcceso, user);
            if (tiposU != null) q = q.Where(c => tiposU.Contains(c.TipoZonaAcceso));

            return q;
        }

        private static List<TipoZonaAcceso> GetTipoZonaAcceso(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZonaAcceso)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoZonaAcceso(session, empresas, lineas, tiposZonaAcceso, user);
        }
        private static List<TipoZonaAcceso> GetTipoZonaAcceso(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposZonaAcceso, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposZonaAcceso)) return null;

            var tiposQ = session.Query<TipoZonaAcceso>()
                                .FilterEmpresa(session, empresas, user)
                                .FilterLinea(session, empresas, lineas, user);

            var tiposU = tiposQ.Cacheable().ToList();

            if (!IncludesAll(tiposZonaAcceso)) tiposU = tiposU.Where(l => tiposZonaAcceso.Contains(l.Id)).ToList();

            return tiposU;
        }
    }
}
