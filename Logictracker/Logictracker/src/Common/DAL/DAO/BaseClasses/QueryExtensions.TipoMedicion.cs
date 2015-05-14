using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BaseClasses
{
    public static partial class QueryExtensions
    {
        public static IQueryable<TQuery> FilterTipoMedicion<TQuery>(this IQueryable<TQuery> q, ISession session, IEnumerable<int> tiposMedicion)
            where TQuery : IHasTipoMedicion
        {
            var tiposU = GetTipoMedicion(session, tiposMedicion);
            if (tiposU != null) q = q.Where(c => c.TipoMedicion == null || tiposU.Contains(c.TipoMedicion));

            return q;
        }

        private static List<TipoMedicion> GetTipoMedicion(ISession session, IEnumerable<int> tiposMedicion)
        {
            if (IncludesAll(tiposMedicion)) return null;

            var tiposQ = session.Query<TipoMedicion>();

            var tiposU = tiposQ.Cacheable().ToList();

            if (!IncludesAll(tiposMedicion)) tiposU = tiposU.Where(l => tiposMedicion.Contains(l.Id)).ToList();

            return tiposU;
        }
    }
}
