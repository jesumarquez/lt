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
        public static IQueryable<TQuery> FilterProveedor<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores)
            where TQuery : IHasProveedor
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterProveedor(q, session, empresas, lineas, tiposProveedor, proveedores, user);
        }
        public static IQueryable<TQuery> FilterProveedor<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores, Usuario user)
            where TQuery : IHasProveedor
        {
            var proveedoresU = GetProveedor(session, empresas, lineas, tiposProveedor, proveedores, user);
            
            if (proveedoresU != null) q = q.Where(c => c.Proveedor == null || proveedoresU.Contains(c.Proveedor));

            if (!IncludesAll(proveedores)) q = q.Where(c => c.Proveedor != null);

            return q;
        }

        private static List<Proveedor> GetProveedor(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetProveedor(session, empresas, lineas, tiposProveedor, proveedores, user);
        }
        private static List<Proveedor> GetProveedor(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposProveedor, IEnumerable<int> proveedores, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(proveedores)) return null;

            var proveedoresQ = session.Query<Proveedor>().FilterEmpresa(session, empresas, user);

            if (!IncludesAll(lineas))
                proveedoresQ = proveedoresQ.FilterLinea(session, empresas, lineas, user);
            
            if (!IncludesAll(tiposProveedor))
                proveedoresQ = proveedoresQ.FilterTipoProveedor(session, empresas, lineas, tiposProveedor);

            var proveedoresU = proveedoresQ.Cacheable().ToList();

            if (!IncludesAll(proveedores)) proveedoresU = proveedoresU.Where(l => proveedores.Contains(l.Id)).ToList();

            return proveedoresU;
        }
    }
}
