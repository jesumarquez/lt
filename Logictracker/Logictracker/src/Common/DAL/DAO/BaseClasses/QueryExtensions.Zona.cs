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
        public static IQueryable<TQuery> FilterZona<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> zonas)
            where TQuery : IHasZona
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterZona(q, session, empresas, lineas, zonas, user);
        }
        public static IQueryable<TQuery> FilterZona<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> zonas, Usuario user)
            where TQuery : IHasZona
        {
            var zonasU = GetZona(session, empresas, lineas, zonas, user);
            if (zonasU != null) q = q.Where(c => c.Zona == null || zonasU.Contains(c.Zona));

            return q;
        }

        private static List<Zona> GetZona(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> zonas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetZona(session, empresas, lineas, zonas, user);
        }
        private static List<Zona> GetZona(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> zonas, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(zonas)) return null;

            var zonasQ = session.Query<Zona>()
                                .FilterEmpresa(session, empresas, user)
                                .FilterLinea(session, empresas, lineas, user);

            var zonasU = zonasQ.Cacheable().ToList();

            if (!IncludesAll(zonas)) zonasU = zonasU.Where(l => zonas.Contains(l.Id)).ToList();

            return zonasU;
        }
    }
}
