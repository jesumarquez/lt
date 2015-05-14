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
        public static IQueryable<TQuery> FilterSensor<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> sensores)
            where TQuery : IHasSensor
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterSensor(q, session, empresas, lineas, dispositivos, sensores, user);
        }
        public static IQueryable<TQuery> FilterSensor<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> sensores, Usuario user)
            where TQuery : IHasSensor
        {
            var sensoresU = GetSensor(session, empresas, lineas, dispositivos, sensores, user);
            if (sensoresU != null) q = q.Where(c => c.Sensor == null || sensoresU.Contains(c.Sensor));

            return q;
        }

        private static List<Sensor> GetSensor(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> sensores)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetSensor(session, empresas, lineas, dispositivos, sensores, user);
        }
        private static List<Sensor> GetSensor(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> dispositivos, IEnumerable<int> sensores, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(sensores)) return null;

            var sensoresQ = session.Query<Sensor>();

            if (!IncludesAll(empresas) || !IncludesAll(lineas) || !IncludesAll(dispositivos))
                sensoresQ = sensoresQ.FilterDispositivo(session, empresas, lineas, dispositivos ,user);

            var sensoresU = sensoresQ.Cacheable().ToList();

            if (!IncludesAll(sensores)) 
                sensoresU = sensoresU.Where(l => sensores.Contains(l.Id)).ToList();

            return sensoresU;
        }
    }
}
