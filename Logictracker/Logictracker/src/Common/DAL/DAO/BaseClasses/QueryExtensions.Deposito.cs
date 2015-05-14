using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterDeposito<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositos)
            where TQuery : IHasDeposito
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterDeposito(q, session, empresas, lineas, depositos, user);
        }
        public static IQueryable<TQuery> FilterDeposito<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositos, Usuario user)
            where TQuery : IHasDeposito
        {
            var depositosU = GetDeposito(session, empresas, lineas, depositos, user);

            if (depositosU != null) q = q.Where(m => m.Deposito == null || depositosU.Contains(m.Deposito));

            if (!IncludesAll(depositos)) q = q.Where(m => m.Deposito != null);

            return q;
        }

        private static List<Deposito> GetDeposito(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetDeposito(session, empresas, lineas, depositos, user);
        }
        private static List<Deposito> GetDeposito(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositos, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(depositos)) return null;

            var depositosQ = session.Query<Deposito>().FilterEmpresa(session, empresas, user).Where(d => !d.Baja);

            if (!IncludesAll(lineas))
                depositosQ = depositosQ.FilterLinea(session, empresas, lineas, user);

            var depositosU = depositosQ.Cacheable().ToList();

            if (!IncludesAll(depositos)) depositosU = depositosU.Where(l => depositos.Contains(l.Id)).ToList();

            return depositosU;
        }
    }
}
