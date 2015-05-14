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
        public static IQueryable<TQuery> FilterMarca<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas)
            where TQuery : IHasMarca
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterMarca(q, session, empresas, lineas, marcas, user);
        }
        public static IQueryable<TQuery> FilterMarca<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas, Usuario user)
            where TQuery : IHasMarca
        {
            var marcasU = GetMarca(session, empresas, lineas, marcas, user);
            if (marcasU != null) q = q.Where(c => c.Marca == null || marcasU.Contains(c.Marca));

            return q;
        }

        private static List<Marca> GetMarca(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetMarca(session, empresas, lineas, marcas, user);
        }
        private static List<Marca> GetMarca(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(marcas)) return null;

            var marcasQ = session.Query<Marca>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user);

            var marcasU = marcasQ.Cacheable().ToList();

            if (!IncludesAll(marcas)) marcasU = marcasU.Where(l => marcas.Contains(l.Id)).ToList();

            return marcasU;
        }
    }
}
