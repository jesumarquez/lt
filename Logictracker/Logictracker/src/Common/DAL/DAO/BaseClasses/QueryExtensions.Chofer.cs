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
        public static IQueryable<TQuery> FilterChofer<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tipoEmpleado, IEnumerable<int> choferes)
            where TQuery : IHasChofer
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterChofer(q, session, empresas, lineas, transportistas, tipoEmpleado, choferes, user);
        }
        public static IQueryable<TQuery> FilterChofer<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tipoEmpleado, IEnumerable<int> choferes, Usuario user)
            where TQuery : IHasChofer
        {
            var choferesU = GetChofer(session, empresas, lineas, transportistas, tipoEmpleado, choferes, user);

            var includesAll = IncludesAll(choferes);

            return FilterChofer(q, choferesU, includesAll, user);
        }

        public static IQueryable<TQuery> FilterChofer<TQuery>(this IQueryable<TQuery> q, List<Empleado> choferes, bool includesAll, Usuario user)
            where TQuery : IHasChofer
        {
            if (choferes != null) q = q.Where(m => m.Chofer == null || choferes.Contains(m.Chofer));

            if (!includesAll) q = q.Where(m => m.Chofer != null);

            return q;
        }

        public static List<Empleado> GetChofer(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tipoEmpleado, IEnumerable<int> choferes)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetChofer(session, empresas, lineas, transportistas, tipoEmpleado, choferes, user);
        }
        public static List<Empleado> GetChofer(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> tiposEmpleado, IEnumerable<int> choferes, Usuario user)
        {
            if (empresas == null && lineas == null && transportistas == null && tiposEmpleado == null && IncludesAll(choferes))
                return null;

            var choferesQ = session.Query<Empleado>();

            if (!IncludesAll(empresas))
                choferesQ = choferesQ.FilterEmpresa(session, empresas, user);
            if (!IncludesAll(lineas))
                choferesQ = choferesQ.FilterLinea(session, empresas, lineas, user);
            if (!IncludesAll(transportistas))
                choferesQ = choferesQ.FilterTransportista(session, empresas, lineas, transportistas, user);
            if (!IncludesAll(tiposEmpleado))
                choferesQ = choferesQ.FilterTipoEmpleado(session, empresas, lineas, tiposEmpleado, user);

            var choferesU = choferesQ.Cacheable().ToList();

            if (!IncludesAll(choferes)) choferesU = choferesU.Where(l => choferes.Contains(l.Id)).ToList();

            return choferesU;
        }
    }
}
