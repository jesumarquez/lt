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
        public static IQueryable<Empresa> FilterEmpresa(this IQueryable<Empresa> q, ISession session)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var empresasU = GetEmpresas(session, new[] { -1 }, user);

            if (empresasU != null) q = q.Where(t => empresasU.Contains(t));
            return q;
        }

        public static IQueryable<TQuery> FilterEmpresa<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas)
            where TQuery : IHasEmpresa
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterEmpresa(q, session, empresas, user);
        }

        public static IQueryable<TQuery> FilterEmpresa<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, Usuario user)
            where TQuery : IHasEmpresa
        {
            return FilterEmpresa(q, GetEmpresas(session, empresas, user));
        }
     
        public static IQueryable<TQuery> FilterEmpresa<TQuery>(this IQueryable<TQuery> q, IQueryable<Empresa> empresas)
            where TQuery : IHasEmpresa
        {
            if (empresas != null) q = q.Where(t => t.Empresa == null || empresas.Contains(t.Empresa));

            return q;
        }

        public static IQueryable<Empresa> GetEmpresas(ISession session, IEnumerable<int> empresas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return GetEmpresas(session, empresas, user);
        }
        public static IQueryable<Empresa> GetEmpresas(ISession session, IEnumerable<int> empresas, Usuario user)
        {
            var empresaDao = new EmpresaDAO();

            if (empresas == null && (user == null || !user.PorEmpresa) && IncludesAll(empresas))
                return null;

            if (empresas == null) empresas = new[] { -1 };

            var empresasU = (user != null && user.PorEmpresa
                ? user.Empresas.AsQueryable()
                : empresaDao.FindAll()
                );

            if (!IncludesAll(empresas))
                empresasU = empresasU.Where(e => empresas.Contains(e.Id));
            return empresasU;
        } 
    }
}
