using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterDetalle<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipos,  IEnumerable<int> detallesPadres)
            where TQuery : IHasDetallePadre
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterDetalle(q, session, empresas, lineas, tipos, detallesPadres, user);
        }
        public static IQueryable<TQuery> FilterDetalle<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipos, IEnumerable<int> detallesPadres, Usuario user)
            where TQuery : IHasDetallePadre
        {
            var detallesU = GetDetalle(session, empresas, lineas, tipos, detallesPadres, user);
            if (detallesU != null) q = q.Where(c => c.DetallePadre == null || detallesU.Contains(c.DetallePadre));

            return q;
        }

        private static List<Detalle> GetDetalle(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipos, IEnumerable<int> detallesPadres)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetDetalle(session, empresas, lineas, tipos, detallesPadres, user);
        }
        private static List<Detalle> GetDetalle(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tipos, IEnumerable<int> detallesPadres, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tipos) && IncludesAll(detallesPadres)) return null;

            var detallesQ = session.Query<Detalle>()
                                   .FilterTipoEntidad(session, empresas, lineas, tipos, user);

            var detallesU = detallesQ.Cacheable().ToList();

            if (!IncludesAll(detallesPadres)) detallesU = detallesU.Where(l => detallesPadres.Contains(l.Id)).ToList();

            return detallesU;
        }
    }
}
