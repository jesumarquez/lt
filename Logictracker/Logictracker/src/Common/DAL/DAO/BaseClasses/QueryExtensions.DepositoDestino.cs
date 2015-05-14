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
        public static IQueryable<TQuery> FilterDepositoDestino<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositosDestino)
            where TQuery : IHasDepositoDestino
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterDepositoDestino(q, session, empresas, lineas, depositosDestino, user);
        }
        public static IQueryable<TQuery> FilterDepositoDestino<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositosDestino, Usuario user)
            where TQuery : IHasDepositoDestino
        {
            var depositosDestinoU = GetDepositoDestino(session, empresas, lineas, depositosDestino, user);

            if (depositosDestinoU != null) q = q.Where(m => m.DepositoDestino == null || depositosDestinoU.Contains(m.DepositoDestino));

            if (!IncludesAll(depositosDestino)) q = q.Where(m => m.DepositoDestino != null);

            return q;
        }

        private static List<Deposito> GetDepositoDestino(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositosDestino)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetDepositoDestino(session, empresas, lineas, depositosDestino, user);
        }
        private static List<Deposito> GetDepositoDestino(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositosDestino, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(depositosDestino)) return null;

            var depositosDestinoQ = session.Query<Deposito>().FilterEmpresa(session, empresas, user).Where(d => !d.Baja);

            if (!IncludesAll(lineas))
                depositosDestinoQ = depositosDestinoQ.FilterLinea(session, empresas, lineas, user);

            var depositosDestinoU = depositosDestinoQ.Cacheable().ToList();

            if (!IncludesAll(depositosDestino)) depositosDestinoU = depositosDestinoU.Where(l => depositosDestino.Contains(l.Id)).ToList();

            return depositosDestinoU;
        }
    }
}
