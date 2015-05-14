using Geocoder.Core;
using Geocoder.Data.SessionManagement;
using System.Collections.Generic;

namespace Geocoder.Data.DAO
{
	public class CruceDAO : AbstractDAO<Cruce, int>
	{
		public CruceDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath) { }
		
        public Cruce GetEsquinaMasCercana(double lat, double lon)
		{
			var list = NHibernateSession.CreateQuery("from Cruce c where (c.Latitud\r\n                >= :latitud - :radio and c.Latitud <= :latitud + :radio) and (c.Longitud >= :longitud - :radio\r\n                and c.Longitud <= :longitud + :radio) order by abs(c.Latitud - :latitud) + abs(c.Longitud -\r\n                :longitud)").SetParameter("latitud", lat).SetParameter("longitud", lon).SetParameter("radio", 0.005).SetCacheable(true).List<Cruce>();
			return (list.Count > 0) ? list[0] : null;
		}
		public int DeleteCrucesByIdMap(int idMapa)
		{
			return NHibernateSession.Delete("from Cruce c where c.Poligonal.Partido.MapId = " + idMapa);
		}
		public IList<Cruce> GetCruce(int poligonal1, int poligonal2, int idMapaUrbano)
		{
			return NHibernateSession.CreateQuery("from Cruce c where c.Poligonal.MapId = :map \r\n                    and c.Poligonal.PolId = :pol1 and c.Esquina.PolId = :pol2").SetParameter("map", idMapaUrbano).SetParameter("pol1", poligonal1).SetParameter("pol2", poligonal2).SetCacheable(true).List<Cruce>();
		}
	}
}
