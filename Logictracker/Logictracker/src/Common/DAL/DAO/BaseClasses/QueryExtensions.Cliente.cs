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
        public static IQueryable<TQuery> FilterCliente<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes)
            where TQuery: IHasCliente
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterCliente(q, session, empresas, lineas, clientes, user);
        }
        public static IQueryable<TQuery> FilterCliente<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, Usuario user)
            where TQuery : IHasCliente
        {
            if (IncludesAll(clientes)) return q;
            var empresasU = GetEmpresas(session, empresas, user);
            var lineasU = GetLineas(session, empresas, lineas, user);
            var clientesU = GetCliente(session, empresasU, lineasU, clientes, user);
            if (clientesU != null) q = q.Where(c => clientesU.Contains(c.Cliente));

            return q;
        }
        public static IEnumerable<TQuery> FilterCliente<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes)
            where TQuery: IHasCliente
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterCliente(q, session, empresas, lineas, clientes, user);
        }
        public static IEnumerable<TQuery> FilterCliente<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, Usuario user)
            where TQuery : IHasCliente
        {
            var empresasU = GetEmpresas(session, empresas, user);
            var lineasU = GetLineas(session, empresas, lineas, user);
            var clientesU = GetCliente(session, empresasU, lineasU, clientes, user);
            if (clientesU != null) q = q.Where(c => clientesU.Contains(c.Cliente));

            return q;
        }

        private static IEnumerable<Cliente> GetCliente(ISession session, IEnumerable<Empresa> empresas, IEnumerable<Linea> lineas, IEnumerable<int> clientes)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetCliente(session, empresas, lineas, clientes, user);
        }
        private static IEnumerable<Cliente> GetCliente(ISession session, IEnumerable<Empresa> empresas, IEnumerable<Linea> lineas, IEnumerable<int> clientes, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(clientes)) return null;

            var clientesQ = session.Query<Cliente>()
                .FilterEmpresa(empresas)
                .FilterLinea(lineas);

            var clientesU = clientesQ.Cacheable().ToList();

            if (!IncludesAll(clientes)) clientesU = clientesU.Where(l => clientes.Contains(l.Id)).ToList();

            return clientesU.ToList();
        } 
    }
}
