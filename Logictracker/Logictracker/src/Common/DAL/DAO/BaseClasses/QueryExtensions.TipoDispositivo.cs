using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoDispositivo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> tiposDispositivo)
            where TQuery : IHasTipoDispositivo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoDispositivo(q, session, tiposDispositivo, user);
        }
        public static IQueryable<TQuery> FilterTipoDispositivo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> tiposDispositivo, Usuario user)
            where TQuery : IHasTipoDispositivo
        {
            var tiposDispositivoU = GetTipoDispositivo(session, tiposDispositivo, user);
            if (tiposDispositivoU != null) q = q.Where(c => c.TipoDispositivo == null || tiposDispositivoU.Contains(c.TipoDispositivo));

            var includesAll = IncludesAll(tiposDispositivo);
            var includesNone = IncludesNone(tiposDispositivo);

            if (!includesNone && !includesAll) q = q.Where(t => t.TipoDispositivo != null);

            return q;
        }

        private static List<TipoDispositivo> GetTipoDispositivo(ISession session, IEnumerable<int> tiposDispositivo)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoDispositivo(session, tiposDispositivo, user);
        }
        private static List<TipoDispositivo> GetTipoDispositivo(ISession session, IEnumerable<int> tiposDispositivo, Usuario user)
        {
            if (IncludesAll(tiposDispositivo)) return null;

            var tiposDispositivoU = session.Query<TipoDispositivo>()
                .Where(t => !t.Baja)
                .Cacheable()
                .ToList();

            if (!IncludesAll(tiposDispositivo))
                tiposDispositivoU = tiposDispositivoU.Where(l => tiposDispositivo.Contains(l.Id)).ToList();

            return tiposDispositivoU;
        }
    }
}
