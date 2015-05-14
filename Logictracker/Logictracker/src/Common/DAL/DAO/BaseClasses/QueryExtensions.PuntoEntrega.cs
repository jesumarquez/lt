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
        public static IQueryable<TQuery> FilterPuntoEntrega<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, IEnumerable<int> puntosEntrega)
            where TQuery : IHasPuntoEntrega
        {
            if (IncludesAll(puntosEntrega)) return q;

            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var puntosEntregaU = GetPuntoEntrega(session, empresas, lineas, clientes, puntosEntrega);

            var includesAll = IncludesAll(puntosEntrega);

            return FilterPuntoEntrega(q, puntosEntregaU, includesAll, user);
        }

        public static IQueryable<TQuery> FilterPuntoEntrega<TQuery>(this IQueryable<TQuery> q, List<PuntoEntrega> puntosEntrega, bool includesAll, Usuario user)
            where TQuery : IHasPuntoEntrega
        {
            if (puntosEntrega != null) q = q.Where(m => m.PuntoEntrega == null || puntosEntrega.Contains(m.PuntoEntrega));

            if (!includesAll) q = q.Where(m => m.PuntoEntrega != null);

            return q;
        }
        public static IEnumerable<TQuery> FilterPuntoEntrega<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, IEnumerable<int> puntosEntrega)
            where TQuery : IHasPuntoEntrega
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var puntosEntregaU = GetPuntoEntrega(session, empresas, lineas, clientes, puntosEntrega);

            var includesAll = IncludesAll(puntosEntrega);

            return FilterPuntoEntrega(q, puntosEntregaU, includesAll, user);
        }
        public static IEnumerable<TQuery> FilterPuntoEntrega<TQuery>(this IEnumerable<TQuery> q, List<PuntoEntrega> puntosEntrega, bool includesAll, Usuario user)
            where TQuery : IHasPuntoEntrega
        {
            if (puntosEntrega != null) q = q.Where(m => m.PuntoEntrega == null || puntosEntrega.Contains(m.PuntoEntrega));

            if (!includesAll) q = q.Where(m => m.PuntoEntrega != null);

            return q;
        }

        public static List<PuntoEntrega> GetPuntoEntrega(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, IEnumerable<int> puntosEntrega)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetPuntoEntrega(session, empresas, lineas, clientes, puntosEntrega, user);
        }
        public static List<PuntoEntrega> GetPuntoEntrega(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> clientes, IEnumerable<int> puntosEntrega, Usuario user)
        {
            if (empresas == null && lineas == null && clientes == null && IncludesAll(puntosEntrega))
                return null;

            if (puntosEntrega == null) puntosEntrega = new[] { -1 };

            var puntoEntregaQ = session.Query<PuntoEntrega>()
                .FilterCliente(session, empresas, lineas, clientes, user);

            var puntosEntregaU = puntoEntregaQ.Cacheable().ToList();

            if (!IncludesAll(puntosEntrega)) 
                puntosEntregaU = puntosEntregaU.Where(l => puntosEntrega.Contains(l.Id)).ToList();

            return puntosEntregaU;
        } 
    }
}
