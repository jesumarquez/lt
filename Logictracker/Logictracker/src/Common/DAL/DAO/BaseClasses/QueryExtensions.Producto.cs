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
        public static IQueryable<TQuery> FilterProducto<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> productos)
            where TQuery : IHasProducto
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterProducto(q, session, empresas, lineas, productos, user);
        }

        public static IQueryable<TQuery> FilterProducto<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> productos, Usuario user)
            where TQuery : IHasProducto
        {
            if (IncludesAll(productos)) return q;
            var productosU = GetProducto(session, empresas, lineas, productos, user);
            if (productosU != null) q = q.Where(c => c.Producto == null || productosU.Contains(c.Producto));

            return q;
        }
        public static IEnumerable<TQuery> FilterProducto<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> productos)
            where TQuery : IHasProducto
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterProducto(q, session, empresas, lineas, productos, user);
        }
        public static IEnumerable<TQuery> FilterProducto<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> productos, Usuario user)
            where TQuery : IHasProducto
        {
            var productosU = GetProducto(session, empresas, lineas, productos, user);
            if (productosU != null) q = q.Where(c => c.Producto == null || productosU.Contains(c.Producto));

            return q;
        }

        private static List<Producto> GetProducto(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> productos, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(productos)) return null;

            var productosQ = session.Query<Producto>()
                                    .FilterEmpresa(session, empresas, user)
                                    .FilterLinea(session, empresas, lineas, user);

            var productosU = productosQ.Cacheable().ToList();

            if (!IncludesAll(productos)) productosU = productosU.Where(l => productos.Contains(l.Id)).ToList();

            return productosU;
        }
    }
}
