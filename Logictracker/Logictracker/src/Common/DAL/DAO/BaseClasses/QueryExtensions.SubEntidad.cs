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
        public static IQueryable<TQuery> FilterSubEntidad<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, IEnumerable<int> subentidades)
            where TQuery : IHasSubEntidad
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterSubEntidad(q, session, empresas, lineas, tiposEntidad, entidades, subentidades, user);
        }
        public static IQueryable<TQuery> FilterSubEntidad<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, IEnumerable<int> subentidades, Usuario user)
            where TQuery : IHasSubEntidad
        {
            var subentidadesU = GetSubEntidad(session, empresas, lineas, tiposEntidad, entidades, subentidades, user);
            if (subentidadesU != null) q = q.Where(c => c.SubEntidad == null || subentidadesU.Contains(c.SubEntidad));

            return q;
        }

        private static List<SubEntidad> GetSubEntidad(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, IEnumerable<int> subentidades)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetSubEntidad(session, empresas, lineas, tiposEntidad, entidades, subentidades, user);
        }
        private static List<SubEntidad> GetSubEntidad(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, IEnumerable<int> subentidades, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(subentidades)) return null;

            var subentidadesQ = session.Query<SubEntidad>();

            if (!IncludesAll(empresas))
                subentidadesQ = subentidadesQ.FilterEmpresa(session, empresas, user);
            if (!IncludesAll(lineas))
                subentidadesQ = subentidadesQ.FilterLinea(session, empresas, lineas, user);
            if (!IncludesAll(tiposEntidad) || !IncludesAll(entidades))
                subentidadesQ = subentidadesQ.FilterEntidad(session, empresas, lineas, tiposEntidad, entidades);

            var subentidadesU = subentidadesQ.Cacheable().ToList();

            if (!IncludesAll(subentidades)) 
                subentidadesU = subentidadesU.Where(l => subentidades.Contains(l.Id)).ToList();

            return subentidadesU;
        }
    }
}
