using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ControlAcceso;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate.Linq;
using NHibernate;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterCategoriaAcceso<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> categoriasAcceso)
            where TQuery : IHasCategoriaAcceso
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterCategoriaAcceso(q, session, empresas, lineas, categoriasAcceso, user);
        }
        public static IQueryable<TQuery> FilterCategoriaAcceso<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> categoriasAcceso, Usuario user)
            where TQuery : IHasCategoriaAcceso
        {
            var categoriasU = GetCategoriaAcceso(session, empresas, lineas, categoriasAcceso, user);

            var includesAll = IncludesAll(categoriasAcceso);
            var includesNone = IncludesNone(categoriasAcceso);

            return FilterCategoriaAcceso(q, categoriasU, includesAll, includesNone, user);
        }

        public static IQueryable<TQuery> FilterCategoriaAcceso<TQuery>(this IQueryable<TQuery> q, List<CategoriaAcceso> categoriasAcceso, bool includesAll, bool includesNone, Usuario user)
            where TQuery : IHasCategoriaAcceso
        {
            if (categoriasAcceso != null) q = q.Where(m => m.Categoria == null || categoriasAcceso.Contains(m.Categoria));

            if (!includesAll && !includesNone) q = q.Where(m => m.Categoria != null);

            return q;
        }

        public static List<CategoriaAcceso> GetCategoriaAcceso(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> categoriasAcceso)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetCategoriaAcceso(session, empresas, lineas, categoriasAcceso, user);
        }
        public static List<CategoriaAcceso> GetCategoriaAcceso(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> categoriasAcceso, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(categoriasAcceso))
                return null;

            var categoriasAccesoQ = session.Query<CategoriaAcceso>()
                                           .FilterEmpresa(session, empresas, user)
                                           .FilterLinea(session, empresas, lineas, user);
                                           

            var categoriasAccesoU = categoriasAccesoQ.Cacheable().ToList();

            if (!IncludesAll(categoriasAcceso)) categoriasAccesoU = categoriasAccesoU.Where(c => categoriasAcceso.Contains(c.Id)).ToList();

            return categoriasAccesoU;
        } 
    }
}
