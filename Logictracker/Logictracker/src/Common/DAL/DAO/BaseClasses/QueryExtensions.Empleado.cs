using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate.Linq;
using NHibernate;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterEmpleado<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tipoEmpleado, IEnumerable<int> empleados)
            where TQuery : IHasEmpleado
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterEmpleado(q, session, empresas, lineas, transportistas, tipoEmpleado, empleados, user);
        }
        public static IQueryable<TQuery> FilterEmpleado<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tipoEmpleado, IEnumerable<int> empleados, Usuario user)
            where TQuery : IHasEmpleado
        {
            var empleadosU = GetEmpleado(session, empresas, lineas, transportistas, tipoEmpleado, empleados, user);

            var includesAll = IncludesAll(empleados);
            var includesNone = IncludesNone(empleados);

            return FilterEmpleado(q, empleadosU, includesAll, includesNone, user);
        }

        public static IQueryable<TQuery> FilterEmpleado<TQuery>(this IQueryable<TQuery> q, List<Empleado> empleados, bool includesAll, bool includesNone, Usuario user)
            where TQuery : IHasEmpleado
        {
            if (empleados != null) q = q.Where(m => m.Empleado == null || empleados.Contains(m.Empleado));

            if (!includesAll && !includesNone) q = q.Where(m => m.Empleado != null);

            return q;
        }

        public static List<Empleado> GetEmpleado(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tipoEmpleado, IEnumerable<int> empleados)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetEmpleado(session, empresas, lineas, transportistas, tipoEmpleado, empleados, user);
        }
        public static List<Empleado> GetEmpleado(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tiposEmpleado, IEnumerable<int> empleados, Usuario user)
        {
            if (empresas == null && lineas == null && transportistas == null && tiposEmpleado == null && IncludesAll(empleados))
                return null;

            var empleadoQ = session.Query<Empleado>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user)
                .FilterTransportista(session, empresas, lineas, transportistas)
                .FilterTipoEmpleado(session, empresas, lineas, tiposEmpleado, user);

            var empleadoU = empleadoQ.Cacheable().ToList();

            if (!IncludesAll(empleados)) empleadoU = empleadoU.Where(l => empleados.Contains(l.Id)).ToList();

            return empleadoU;
        } 
    }
}
