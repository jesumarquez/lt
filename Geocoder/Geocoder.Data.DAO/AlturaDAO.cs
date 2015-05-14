using Geocoder.Core;
using Geocoder.Data.SessionManagement;
using System.Globalization;

namespace Geocoder.Data.DAO
{
	public class AlturaDAO : AbstractDAO<Altura, int>
	{
		public AlturaDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath) { }

		public int DeleteAlturasByIdMap(int idMapa)
		{
			return NHibernateSession.Delete("from Altura a where a.Poligonal.Partido.MapId = " + idMapa);
		}
		public Altura GetAltura(int poligonal, int altura, int idMapaUrbano)
		{
			var list = NHibernateSession.CreateQuery("from Altura a where a.Poligonal.MapId = :map \r\n                    and a.Poligonal.PolId = :pol and a.AlturaInicio-1 <= :alt and a.AlturaFin >= :alt").SetParameter("map", idMapaUrbano).SetParameter("pol", poligonal).SetParameter<int>("alt", altura).SetCacheable(true).List<Altura>();
			return (list.Count > 0) ? list[0] : null;
		}
		public Altura GetDireccionMasCercana(double lat, double lon)
		{
			var str = string.Format("order by ((a.LatitudFin + a.LatitudInicio - 2 * {0}) * (a.LatitudFin + a.LatitudInicio - 2 * {0})) \r\n                + ((a.LongitudFin + a.LongitudInicio - 2 * {1}) * (a.LongitudFin + a.LongitudInicio - 2 * {1}))", lat.ToString(CultureInfo.InvariantCulture), lon.ToString(CultureInfo.InvariantCulture));
			var list = NHibernateSession.CreateQuery("from Altura a where abs(a.LatitudInicio - :lat) < 0.01 and abs(a.LongitudInicio - :lon) < 0.01 " + str).SetParameter("lat", lat).SetParameter("lon", lon).SetMaxResults(1).List<Altura>();
			return (list.Count > 0) ? list[0] : null;
		}
	}
}
