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
        public static IQueryable<TQuery> FilterInsumo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> insumos)
            where TQuery : IHasInsumo
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterInsumo(q, session, empresas, lineas, insumos, user);
        }
        public static IQueryable<TQuery> FilterInsumo<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> insumos, Usuario user)
            where TQuery : IHasInsumo
        {
            var insumosU = GetInsumo(session, empresas, lineas, insumos, user);
            if (insumosU != null) q = q.Where(c => c.Insumo == null || insumosU.Contains(c.Insumo));

            return q;
        }

        private static List<Insumo> GetInsumo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> insumos)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetInsumo(session, empresas, lineas, insumos, user);
        }
        private static List<Insumo> GetInsumo(ISession session, IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> insumos, Usuario user)
        {
            if (empresas == null && lineas == null && IncludesAll(insumos)) return null;

            var insumosQ = session.Query<Insumo>().FilterEmpresa(session, empresas, user);

            if (!IncludesAll(lineas))
                insumosQ = insumosQ.FilterLinea(session, empresas, lineas, user);

            var insumosU = insumosQ.Cacheable().ToList();

            if (!IncludesAll(insumos)) insumosU = insumosU.Where(l => insumos.Contains(l.Id)).ToList();

            return insumosU;
        }
    }
}
