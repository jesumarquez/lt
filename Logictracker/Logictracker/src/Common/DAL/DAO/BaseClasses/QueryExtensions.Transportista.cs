using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<Transportista> FilterTransportista(this IQueryable<Transportista> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var transportistasU = GetTransportistas(session, empresas, lineas, new[] { -1 }, user);

            if (transportistasU != null) q = q.Where(t => transportistasU.Contains(t));
            return q;
        }
        public static IQueryable<TQuery> FilterTransportista<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas)
            where TQuery : IHasTransportista
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTransportista(q, session, empresas, lineas, transportistas, user);
        }
        public static IQueryable<TQuery> FilterTransportista<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, Usuario user)
            where TQuery : IHasTransportista
        {
            var transportistasU = GetTransportistas(session, empresas, lineas, transportistas);

            var includesAll = IncludesAll(transportistas);
            var includesNone = IncludesNone(transportistas);

            return FilterTransportista(q, transportistasU, includesAll, includesNone, user);
        }

        public static IQueryable<TQuery> FilterTransportista<TQuery>(this IQueryable<TQuery> q, IEnumerable<Transportista> transportistas, bool includesAll, bool includesNone, Usuario user)
            where TQuery : IHasTransportista
        {
            if (transportistas != null) q = q.Where(t => t.Transportista == null || transportistas.Contains(t.Transportista));

            var porTransportista = user != null && user.PorTransportista;
            var mostrarSinTransportista = user != null && user.MostrarSinTransportista;

            if ((!includesNone && !includesAll) || (porTransportista && !mostrarSinTransportista)) q = q.Where(t => t.Transportista != null);

            return q;
        }
        public static IEnumerable<TQuery> FilterTransportista<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, Usuario user)
            where TQuery : IHasTransportista
        {
            var transportistasU = GetTransportistas(session, empresas, lineas, transportistas);

            var includesAll = IncludesAll(transportistas);
            var includesNone = IncludesNone(transportistas);

            return FilterTransportista(q, transportistasU, includesAll, includesNone, user);
        }
        public static IEnumerable<TQuery> FilterTransportista<TQuery>(this IEnumerable<TQuery> q, IEnumerable<Transportista> transportistas, bool includesAll, bool includesNone, Usuario user)
            where TQuery : IHasTransportista
        {
            if (transportistas != null) q = q.Where(t => t.Transportista == null || transportistas.Contains(t.Transportista));

            var porTransportista = user != null && user.PorTransportista;
            var mostrarSinTransportista = user != null && user.MostrarSinTransportista;
            if ((!includesNone && !includesAll) || (porTransportista && !mostrarSinTransportista)) q = q.Where(t => t.Transportista != null);

            return q;
        }
        public static IEnumerable<Transportista> GetTransportistas(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTransportistas(session, empresas, lineas, transportistas, user);
        }

        public static IEnumerable<Transportista> GetTransportistas(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, Usuario user)
        {
            if (empresas == null && lineas == null && (user == null || !user.PorTransportista) && IncludesAll(transportistas))
                return null;

            var transportistaDao = new TransportistaDAO();

            var transportistasU = user != null && user.PorTransportista
                                       ? user.Transportistas.OfType<Transportista>().ToList()
                                       : transportistaDao.FindAll();

            transportistasU = transportistasU
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user)
                .Where(t => !t.Baja)
                .ToList();

            if (!IncludesAll(transportistas)) transportistasU = transportistasU.Where(l => transportistas.Contains(l.Id)).ToList();

            return transportistasU;
        } 
    }
}
