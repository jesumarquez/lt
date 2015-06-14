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
        public static IQueryable<Linea> FilterLinea(this IQueryable<Linea> q, ISession session, IEnumerable<int> empresas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            var lineasU = GetLineas(session, empresas, new[] { -1 }, user);

            if (lineasU != null) q = q.Where(t => lineasU.Contains(t));
            return q;
        }

        public static IQueryable<TQuery> FilterLinea<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas)
            where TQuery : IHasLinea
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return FilterLinea(q, session, empresas, lineas, user);
        }
        public static IQueryable<TQuery> FilterLinea<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, Usuario user)
            where TQuery : IHasLinea
        {
            var lineasU = GetLineas(session, empresas, lineas, user);

            return FilterLinea(q, lineasU);
        }

        public static IQueryable<TQuery> FilterLinea<TQuery>(this IQueryable<TQuery> q, IEnumerable<Linea> lineas)
            where TQuery : IHasLinea
        {
            if (lineas != null) q = q.Where(t => t.Linea == null || lineas.Contains(t.Linea));
            return q;
        }

        public static IEnumerable<TQuery> FilterLinea<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, Usuario user)
            where TQuery : IHasLinea
        {
            var lineasU = GetLineas(session, empresas, lineas, user);
            return FilterLinea(q, lineasU);
        }
        public static IEnumerable<TQuery> FilterLinea<TQuery>(this IEnumerable<TQuery> q, IEnumerable<Linea> lineas)
            where TQuery : IHasLinea
        {
            if (lineas != null) q = q.Where(t => t.Linea == null || lineas.Contains(t.Linea));
            return q;
        }

        public static IQueryable<Linea> GetLineas(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetLineas(session, empresas, lineas, user);
        }
        public static IQueryable<Linea> GetLineas(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, Usuario user)
        {
            if (empresas == null && (user == null || !user.PorLinea) && IncludesAll(lineas))
                return null;

            var lineaDao = new LineaDAO();

            var lineasU = user != null && user.PorLinea
                                       ? user.Lineas.AsQueryable()
                                       : lineaDao.FindAll();

            lineasU = lineasU
                .FilterEmpresa(session, empresas, user)
                .Where(l => !l.Baja);

            if (!IncludesAll(lineas)) lineasU = lineasU.Where(l => lineas.Contains(l.Id));

            return lineasU;
        } 
    }
}
