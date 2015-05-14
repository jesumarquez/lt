using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterPedido<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> pedidos, IEnumerable<int> bocasDeCarga)
            where TQuery : IHasPedido
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterPedido(q, session, empresas, lineas, pedidos, bocasDeCarga, user);
        }
        public static IQueryable<TQuery> FilterPedido<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> pedidos, IEnumerable<int> bocasDeCarga, Usuario user)
            where TQuery : IHasPedido
        {
            var pedidosU = GetPedido(session, empresas, lineas, pedidos, bocasDeCarga, user);
            if (pedidosU != null)
            {
                if (!IncludesNone(bocasDeCarga) && !IncludesAll(bocasDeCarga))
                    q = q.Where(c => c.Pedido != null && pedidosU.Contains(c.Pedido));
                else
                    q = q.Where(c => c.Pedido == null || pedidosU.Contains(c.Pedido));
            }

            return q;
        }

        private static List<Pedido> GetPedido(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> pedidos, IEnumerable<int> bocasDeCarga)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetPedido(session, empresas, lineas, pedidos, bocasDeCarga, user);
        }
        private static List<Pedido> GetPedido(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> pedidos, IEnumerable<int> bocasDeCarga, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(pedidos)) return null;

            var pedidosQ = session.Query<Pedido>()
                                  .FilterEmpresa(session, empresas, user)
                                  .FilterLinea(session, empresas, lineas, user)
                                  .FilterBocaDeCarga(session, empresas, lineas, bocasDeCarga, user);

            var pedidosU = pedidosQ.Cacheable().ToList();

            if (!IncludesAll(pedidos)) pedidosU = pedidosU.Where(l => pedidos.Contains(l.Id)).ToList();

            return pedidosU;
        }
    }
}
