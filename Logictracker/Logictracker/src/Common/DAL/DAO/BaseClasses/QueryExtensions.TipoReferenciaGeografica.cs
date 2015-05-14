using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoReferenciaGeografica<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef)
            where TQuery : IHasTipoReferenciaGeografica
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterTipoReferenciaGeografica(q, session, empresas, lineas, tiposGeoRef, user);
        }
        public static IQueryable<TQuery> FilterTipoReferenciaGeografica<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef, Usuario user)
            where TQuery : IHasTipoReferenciaGeografica
        {
            var tiposGeoRefU = GetTipoReferenciaGeografica(session, empresas, lineas, tiposGeoRef, user);
            if (tiposGeoRefU != null) q = q.Where(c => tiposGeoRefU.Contains(c.TipoReferenciaGeografica));

            return q;
        }

        private static List<TipoReferenciaGeografica> GetTipoReferenciaGeografica(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetTipoReferenciaGeografica(session, empresas, lineas, tiposGeoRef, user);
        }
        private static List<TipoReferenciaGeografica> GetTipoReferenciaGeografica(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposGeoRef, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(tiposGeoRef)) return null;

            var tipoGeoRefQ = session.Query<TipoReferenciaGeografica>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user);

            var tipoGeoRefU = tipoGeoRefQ.Cacheable().ToList();

            if (!IncludesAll(tiposGeoRef)) tipoGeoRefU = tipoGeoRefU.Where(l => tiposGeoRef.Contains(l.Id)).ToList();

            return tipoGeoRefU;
        }
    }
}
