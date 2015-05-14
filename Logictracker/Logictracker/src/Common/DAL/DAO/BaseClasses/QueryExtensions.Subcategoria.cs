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
        public static IQueryable<TQuery> FilterSubcategoria<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias, IEnumerable<int> subcategorias)
            where TQuery : IHasSubcategoria
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterSubcategoria(q, session, empresas, categorias, subcategorias, user);
        }
        public static IQueryable<TQuery> FilterSubcategoria<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias, IEnumerable<int> subcategorias, Usuario user)
            where TQuery : IHasSubcategoria
        {
            var subcategoriasU = GetSubcategoria(session, empresas, categorias, subcategorias, user);

            if (subcategoriasU != null) q = q.Where(o => o.Subcategoria == null || subcategoriasU.Contains(o.Subcategoria));

            return q;
        }

        private static List<Subcategoria> GetSubcategoria(ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias, IEnumerable<int> subcategorias)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetSubcategoria(session, empresas, categorias, subcategorias, user);
        }
        private static List<Subcategoria> GetSubcategoria(ISession session, IEnumerable<int> empresas, IEnumerable<int> categorias, IEnumerable<int> subcategorias, Usuario user)
        {
            if (IncludesAll(empresas) && IncludesAll(categorias) && IncludesAll(subcategorias)) return null;

            var subcategoriasQ = session.Query<Subcategoria>().Where(d => !d.Baja);

            if (!IncludesAll(empresas) || !IncludesAll(categorias))
                subcategoriasQ = subcategoriasQ.FilterCategoria(session, empresas, categorias, user);
            
            var subcategoriasU = subcategoriasQ.Cacheable().ToList();

            if (!IncludesAll(subcategorias)) subcategoriasU = subcategoriasU.Where(s => subcategorias.Contains(s.Id)).ToList();

            return subcategoriasU;
        }
    }
}
