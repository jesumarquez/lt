using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoProveedor<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor)
            where TQuery : IHasTipoProveedor
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoProveedor(q, session, empresas, lineas, tiposProveedor, user);
        }
        public static IQueryable<TQuery> FilterTipoProveedor<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor, Usuario user)
            where TQuery : IHasTipoProveedor
        {
            var tiposProveedorU = GetTipoProveedor(session, empresas, lineas, tiposProveedor, user);
            if (tiposProveedorU != null) q = q.Where(c => tiposProveedorU.Contains(c.TipoProveedor));

            return q;
        }

        private static List<TipoProveedor> GetTipoProveedor(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoProveedor(session, empresas, lineas, tiposProveedor, user);
        }
        private static List<TipoProveedor> GetTipoProveedor(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposProveedor)) return null;

            var tiposProveedorQ = session.Query<TipoProveedor>().FilterEmpresa(session, empresas, user);

            if (!IncludesAll(lineas))
                tiposProveedorQ = tiposProveedorQ.FilterLinea(session, empresas, lineas, user);

            var tiposProveedorU = tiposProveedorQ.Cacheable().ToList();

            if (!IncludesAll(tiposProveedor)) tiposProveedorU = tiposProveedorU.Where(l => tiposProveedor.Contains(l.Id)).ToList();

            return tiposProveedorU;
        }
    }
}
