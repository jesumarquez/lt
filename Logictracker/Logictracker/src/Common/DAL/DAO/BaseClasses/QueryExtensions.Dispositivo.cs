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
        public static IQueryable<TQuery> FilterDispositivo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos)
            where TQuery : IHasDispositivo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterDispositivo(q, session, empresas, lineas, dispositivos, user);
        }
        public static IQueryable<TQuery> FilterDispositivo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, Usuario user)
            where TQuery : IHasDispositivo
        {
            var dispositivosU = GetDispositivo(session, empresas, lineas, dispositivos, user);
            if (dispositivosU != null) q = q.Where(c => c.Dispositivo == null || dispositivosU.Contains(c.Dispositivo));

            return q;
        }

        private static List<Dispositivo> GetDispositivo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetDispositivo(session, empresas, lineas, dispositivos, user);
        }
        private static List<Dispositivo> GetDispositivo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(dispositivos)) return null;

            var dispositivosQ = session.Query<Dispositivo>()
                                       .FilterEmpresa(session, empresas, user)
                                       .FilterLinea(session, empresas, lineas, user);

            var dispositivosU = dispositivosQ.Cacheable().ToList();

            if (!IncludesAll(dispositivos)) dispositivosU = dispositivosU.Where(l => dispositivos.Contains(l.Id)).ToList();

            return dispositivosU;
        }
    }
}
