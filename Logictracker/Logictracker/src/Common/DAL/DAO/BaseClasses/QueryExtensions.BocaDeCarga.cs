using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterBocaDeCarga<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga)
            where TQuery : IHasBocaDeCarga
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterBocaDeCarga(q, session, empresas, lineas, bocasDeCarga, user);
        }
        public static IQueryable<TQuery> FilterBocaDeCarga<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga, Usuario user)
            where TQuery : IHasBocaDeCarga
        {
            if (IncludesAll(bocasDeCarga)) return q;
            var bocasDeCargaU = GetBocaDeCarga(session, empresas, lineas, bocasDeCarga, user);
            if (bocasDeCargaU != null)
            {
                if (IncludesNone(bocasDeCarga))
                    q = q.Where(c => c.BocaDeCarga == null);
                else
                    q = q.Where(c => c.BocaDeCarga == null || bocasDeCargaU.Contains(c.BocaDeCarga));
            }

            return q;
        }

        public static IEnumerable<TQuery> FilterBocaDeCarga<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga)
            where TQuery : IHasBocaDeCarga
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterBocaDeCarga(q, session, empresas, lineas, bocasDeCarga, user);
        }
        public static IEnumerable<TQuery> FilterBocaDeCarga<TQuery>(this IEnumerable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga, Usuario user)
            where TQuery : IHasBocaDeCarga
        {
            var bocasDeCargaU = GetBocaDeCarga(session, empresas, lineas, bocasDeCarga, user);
            if (bocasDeCargaU != null) q = q.Where(c => c.BocaDeCarga == null || bocasDeCargaU.Contains(c.BocaDeCarga));

            return q;
        }

        private static List<BocaDeCarga> GetBocaDeCarga(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetBocaDeCarga(session, empresas, lineas, bocasDeCarga, user);
        }
        private static List<BocaDeCarga> GetBocaDeCarga(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> bocasDeCarga, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(bocasDeCarga)) return null;

            var bocasDeCargaQ = session.Query<BocaDeCarga>()
                .FilterLinea(session, empresas, lineas, user);

            var bocasDeCargaU = bocasDeCargaQ.Cacheable().ToList();

            if (!IncludesAll(bocasDeCarga)) bocasDeCargaU = bocasDeCargaU.Where(l => bocasDeCarga.Contains(l.Id)).ToList();

            return bocasDeCargaU;
        }
    }
}
