using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoVehiculo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo)
            where TQuery: IHasTipoVehiculo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoVehiculo(q, session, empresas, lineas, tiposVehiculo, user);
        }
        public static IQueryable<TQuery> FilterTipoVehiculo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo, Usuario user)
            where TQuery : IHasTipoVehiculo
        {
            var tiposVehiculoU = GetTipoVehiculo(session, empresas, lineas, tiposVehiculo, user);
            if (tiposVehiculoU != null) q = q.Where(c => tiposVehiculoU.Contains(c.TipoCoche));

            return q;
        }

        public static IEnumerable<TQuery> FilterTipoVehiculo<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo)
            where TQuery : IHasTipoVehiculo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoVehiculo(q, session, empresas, lineas, tiposVehiculo, user);
        }
        public static IEnumerable<TQuery> FilterTipoVehiculo<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo, Usuario user)
            where TQuery : IHasTipoVehiculo
        {
            var tiposVehiculoU = GetTipoVehiculo(session, empresas, lineas, tiposVehiculo, user);
            if (tiposVehiculoU != null) q = q.Where(c => tiposVehiculoU.Contains(c.TipoCoche));

            return q;
        }
        public static IQueryable<TQuery> FilterTipoVehiculo<TQuery>(this IQueryable<TQuery> q, List<TipoCoche> tipos)
            where TQuery : IHasTipoVehiculo
        {
            if (tipos != null) q = q.Where(t => t.TipoCoche == null || tipos.Contains(t.TipoCoche));
            return q;
        }

        public static List<TipoCoche> GetTipoVehiculo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoVehiculo(session, empresas, lineas, tiposVehiculo, user);
        }
        public static List<TipoCoche> GetTipoVehiculo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposVehiculo, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposVehiculo)) return null;

            var tiposVehiculoQ = session.Query<TipoCoche>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user);

            var tiposVehiculoU = tiposVehiculoQ.Cacheable().ToList();

            if (!IncludesAll(tiposVehiculo)) tiposVehiculoU = tiposVehiculoU.Where(l => tiposVehiculo.Contains(l.Id)).ToList();

            return tiposVehiculoU;
        }
    }
}
