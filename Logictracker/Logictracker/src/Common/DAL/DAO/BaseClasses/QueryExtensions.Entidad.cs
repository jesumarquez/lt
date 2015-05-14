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
        public static IQueryable<TQuery> FilterEntidad<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades)
            where TQuery : IHasEntidadPadre
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterEntidad(q, session, empresas, lineas, tiposEntidad, entidades, user);
        }
        public static IQueryable<TQuery> FilterEntidad<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, Usuario user)
            where TQuery : IHasEntidadPadre
        {
            var entidadesU = GetEntidad(session, empresas, lineas, tiposEntidad, entidades, user);
            if (entidadesU != null) q = q.Where(c => c.Entidad == null || entidadesU.Contains(c.Entidad));

            return q;
        }

        private static List<EntidadPadre> GetEntidad(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetEntidad(session, empresas, lineas, tiposEntidad, entidades, user);
        }
        private static List<EntidadPadre> GetEntidad(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEntidad, IEnumerable<int> entidades, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposEntidad) && IncludesAll(entidades)) return null;

            var entidadesQ = session.Query<EntidadPadre>();

            if (empresas != null && !empresas.Contains(-1))
                entidadesQ = entidadesQ.FilterEmpresa(session, empresas, user);
            if (lineas != null && !lineas.Contains(-1))
                entidadesQ = entidadesQ.FilterLinea(session, empresas, lineas, user);
            if (tiposEntidad != null && !tiposEntidad.Contains(-1))
                entidadesQ = entidadesQ.FilterTipoEntidad(session, empresas, lineas, tiposEntidad);

            var entidadesU = entidadesQ.Cacheable().ToList();

            if (!IncludesAll(entidades)) entidadesU = entidadesU.Where(l => entidades.Contains(l.Id)).ToList();

            return entidadesU;
        }
    }
}
