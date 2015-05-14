using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Support;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterCategoria<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias)
            where TQuery : IHasCategoria
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterCategoria(q, session, empresas, categorias, user);
        }
        public static IQueryable<TQuery> FilterCategoria<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias, Usuario user)
            where TQuery : IHasCategoria
        {
            var categoriasU = GetCategoria(session, empresas, categorias, user);

            if (categoriasU != null) q = q.Where(o => o.CategoriaObj == null || categoriasU.Contains(o.CategoriaObj));

            return q;
        }

        private static List<Categoria> GetCategoria(ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetCategoria(session, empresas, categorias, user);
        }
        private static List<Categoria> GetCategoria(ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias, Usuario user)
        {
            if (empresas == null && IncludesAll(categorias)) return null;

            var categoriasQ = session.Query<Categoria>().FilterEmpresa(session, empresas, user).Where(d => !d.Baja);

            var categoriasU = categoriasQ.Cacheable().ToList();

            if (!IncludesAll(categorias)) categoriasU = categoriasU.Where(l => categorias.Contains(l.Id)).ToList();

            return categoriasU;
        }
    }
}
