using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoInsumo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposInsumo)
            where TQuery : IHasTipoInsumo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoInsumo(q, session, empresas, lineas, tiposInsumo, user);
        }
        public static IQueryable<TQuery> FilterTipoInsumo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposInsumo, Usuario user)
            where TQuery : IHasTipoInsumo
        {
            var tiposInsumoU = GetTipoInsumo(session, empresas, lineas, tiposInsumo, user);
            if (tiposInsumoU != null) q = q.Where(c => tiposInsumoU.Contains(c.TipoInsumo));

            return q;
        }

        private static List<TipoInsumo> GetTipoInsumo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposInsumo)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoInsumo(session, empresas, lineas, tiposInsumo, user);
        }
        private static List<TipoInsumo> GetTipoInsumo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposInsumo, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposInsumo)) return null;

            var tiposInsumoQ = session.Query<TipoInsumo>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user);

            var tiposInsumoU = tiposInsumoQ.Cacheable().ToList();

            if (!IncludesAll(tiposInsumo)) tiposInsumoU = tiposInsumoU.Where(l => tiposInsumo.Contains(l.Id)).ToList();

            return tiposInsumoU;
        }
    }
}
