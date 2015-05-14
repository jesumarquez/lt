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
        public static IQueryable<TQuery> FilterTipoEmpleado<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado)
            where TQuery : IHasTipoEmpleado
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoEmpleado(q, session, empresas, lineas, tiposEmpleado, user);
        }
        public static IQueryable<TQuery> FilterTipoEmpleado<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado, Usuario user)
            where TQuery : IHasTipoEmpleado
        {
            var tiposEmpleadoU = GetTipoEmpleado(session, empresas, lineas, tiposEmpleado, user);
            if (tiposEmpleadoU != null) q = q.Where(c => c.TipoEmpleado == null || tiposEmpleadoU.Contains(c.TipoEmpleado));

            var includesAll = IncludesAll(tiposEmpleado);
            var includesNone = IncludesNone(tiposEmpleado);

            if (!includesNone && !includesAll) q = q.Where(t => t.TipoEmpleado != null);

            return q;
        }

        private static List<TipoEmpleado> GetTipoEmpleado(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoEmpleado(session, empresas, lineas, tiposEmpleado, user);
        }
        private static List<TipoEmpleado> GetTipoEmpleado(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposEmpleado, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposEmpleado))
                return null;

            var tiposEmpleadoQ = session.Query<TipoEmpleado>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user);

            var tiposEmpleadoU = tiposEmpleadoQ.Cacheable().ToList();

            if (!IncludesAll(tiposEmpleado))
                tiposEmpleadoU = tiposEmpleadoU.Where(l => tiposEmpleado.Contains(l.Id)).ToList();

            return tiposEmpleadoU;
        }
    }
}
