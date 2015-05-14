using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTaller<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> talleres)
            where TQuery : IHasTaller
        {
            var talleresU = GetTalleres(session, talleres);
            if (talleresU != null) q = q.Where(c => talleresU.Contains(c.Taller));

            return q;
        }

        private static List<Taller> GetTalleres(ISession session, IEnumerable<int> talleres)
        {
            var talleresQ = session.Query<Taller>().Where(t => !t.Baja);

            var talleresU = talleresQ.Cacheable().ToList();

            if (!IncludesAll(talleres)) talleresU = talleresU.Where(l => talleres.Contains(l.Id)).ToList();

            return talleresU;
        }
    }
}
