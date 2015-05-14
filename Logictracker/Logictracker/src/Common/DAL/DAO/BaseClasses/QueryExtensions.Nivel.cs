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
        public static IQueryable<TQuery> FilterNivel<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> niveles)
            where TQuery : IHasNivel
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;

            return FilterNivel(q, session, empresas, niveles, user);
        }
        public static IQueryable<TQuery> FilterNivel<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> empresas, IEnumerable<int> niveles, Usuario user)
            where TQuery : IHasNivel
        {
            var nivelesU = GetNivel(session, empresas, niveles, user);

            if (nivelesU != null) q = q.Where(o => o.NivelObj == null || nivelesU.Contains(o.NivelObj));

            return q;
        }

        private static List<Nivel> GetNivel(ISession session, IEnumerable<int> empresas, IEnumerable<int> niveles)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;
            var user = sessionUser != null ? new UsuarioDAO().FindById(sessionUser.Id) : null;
            return GetNivel(session, empresas, niveles, user);
        }
        private static List<Nivel> GetNivel(ISession session, IEnumerable<int> empresas, IEnumerable<int> niveles, Usuario user)
        {
            if (empresas == null && IncludesAll(niveles)) return null;

            var nivelesQ = session.Query<Nivel>().FilterEmpresa(session, empresas, user).Where(d => !d.Baja);

            var nivelesU = nivelesQ.Cacheable().ToList();

            if (!IncludesAll(niveles)) nivelesU = nivelesU.Where(n => niveles.Contains(n.Id)).ToList();

            return nivelesU;
        }
    }
}
