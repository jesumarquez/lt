using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoEntidad<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad)
            where TQuery : IHasTipoEntidad
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoEntidad(q, session, empresas, lineas, tiposEntidad, user);
        }
        public static IQueryable<TQuery> FilterTipoEntidad<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, Usuario user)
            where TQuery : IHasTipoEntidad
        {
            var tiposU = GetTipoEntidad(session, empresas, lineas, tiposEntidad, user);
            if (tiposU != null) q = q.Where(c => c.TipoEntidad == null || tiposU.Contains(c.TipoEntidad));

            return q;
        }

        private static List<TipoEntidad> GetTipoEntidad(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoEntidad(session, empresas, lineas, tiposEntidad, user);
        }
        private static List<TipoEntidad> GetTipoEntidad(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposEntidad)) return null;

            var tiposQ = session.Query<TipoEntidad>()
                                .FilterEmpresa(session, empresas, user)
                                .FilterLinea(session, empresas, lineas, user);

            var tiposU = tiposQ.Cacheable().ToList();

            if (!IncludesAll(tiposEntidad)) tiposU = tiposU.Where(l => tiposEntidad.Contains(l.Id)).ToList();

            return tiposU;
        }
    }
}
