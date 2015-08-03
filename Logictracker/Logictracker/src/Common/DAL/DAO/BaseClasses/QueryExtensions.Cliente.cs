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
  
        private static IQueryable<Cliente> GetCliente(ISession session, IQueryable<Empresa> empresas, IQueryable<Linea> lineas, IEnumerable<int> clientes)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetCliente(session, empresas, lineas, clientes, user);
        }
        private static IQueryable<Cliente> GetCliente(ISession session, IQueryable<Empresa> empresas, IQueryable<Linea> lineas, IEnumerable<int> clientes, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(clientes)) return null;

            var clientesQ = session.Query<Cliente>()
                .FilterEmpresa(empresas)
                .FilterLinea(lineas);

            if (!IncludesAll(clientes)) clientesQ = clientesQ.Where(l => clientes.Contains(l.Id));

            var clientesU = clientesQ.Cacheable();

            return clientesU;
        } 
    }
}
