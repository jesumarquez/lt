using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterDepartamento<TQuery>(this IQueryable<TQuery> q, IQueryable<Departamento> departamentos)
           where TQuery : IHasDepartamento
        {
            if (departamentos != null) q = q.Where(t => t.Departamento == null || departamentos.Contains(t.Departamento));
            return q;
        }
        public static IQueryable<TQuery> FilterDepartamento<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos)
            where TQuery : IHasDepartamento
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterDepartamento(q, session, empresas, lineas, departamentos, user);
        }
        public static IQueryable<TQuery> FilterDepartamento<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos, Usuario user)
            where TQuery : IHasDepartamento
        {
            var departamentosU = GetDepartamentos(session, empresas, lineas, departamentos, user);
            if (departamentosU != null) q = q.Where(c => c.Departamento == null || departamentosU.Contains(c.Departamento));

            //if (!IncludesNone(departamentos) && !IncludesAll(departamentos)) q = q.Where(c => c.Departamento != null);

            return q;
        }

        private static IQueryable<Departamento> GetDepartamentos(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetDepartamentos(session, empresas, lineas, departamentos, user);
        }
        private static IQueryable<Departamento> GetDepartamentos(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> departamentos, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(departamentos)) return null;

            var dao = new DepartamentoDAO();
            var deps = dao.GetList(empresas, lineas);

            if (!IncludesAll(departamentos)) deps = deps.Where(l => departamentos.Contains(l.Id));

            return deps;
        }

    }
}
