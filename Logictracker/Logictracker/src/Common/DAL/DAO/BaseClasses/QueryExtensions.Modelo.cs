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
        public static IQueryable<TQuery> FilterModelo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas, IEnumerable<int> modelos)
            where TQuery : IHasModelo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterModelo(q, session, empresas, lineas, marcas, modelos, user);
        }
        public static IQueryable<TQuery> FilterModelo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas, IEnumerable<int> modelos, Usuario user)
            where TQuery : IHasModelo
        {
            var modelosU = GetModelo(session, empresas, lineas, marcas, modelos, user);
            if (modelosU != null) q = q.Where(c => c.Modelo == null || modelosU.Contains(c.Modelo));

            return q;
        }

        private static List<Modelo> GetModelo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas, IEnumerable<int> modelos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetModelo(session, empresas, lineas, marcas, modelos, user);
        }
        private static List<Modelo> GetModelo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> marcas, IEnumerable<int> modelos, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(modelos)) return null;

            var modelosQ = session.Query<Modelo>()
                .FilterEmpresa(session, empresas, user)
                .FilterLinea(session, empresas, lineas, user)
                .FilterMarca(session, empresas, lineas, marcas, user);

            var modelosU = modelosQ.Cacheable().ToList();

            if (!IncludesAll(modelos)) modelosU = modelosU.Where(l => modelos.Contains(l.Id)).ToList();

            return modelosU;
        }
    }
}
