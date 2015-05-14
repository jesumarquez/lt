using Geocoder.Core;
using Geocoder.Data.DAO.QueryBuilders;
using Geocoder.Data.SessionManagement;
using System.Collections.Generic;

namespace Geocoder.Data.DAO
{
	public class PoligonalDAO : AbstractDAO<Poligonal, int>
	{
	    public PoligonalDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath) { }
		public IList<Poligonal> GetSinCruceOAltura(int idMapa)
		{
			return NHibernateSession.CreateQuery("from Poligonal p where (size(p.Cruces) = 0 or size(p.Alturas) = 0) and p.MapId = :idMapa").SetParameter("idMapa", idMapa).List<Poligonal>();
		}
		public long GetCountSinPalabras()
		{
			return (long)NHibernateSession.CreateQuery("select count(*) from Poligonal p where size(p.Palabras) = 0").UniqueResult();
		}
		public IList<Poligonal> GetSinPalabras()
		{
			return NHibernateSession.CreateQuery("from Poligonal p where size(p.Palabras) = 0").List<Poligonal>();
		}
		public Poligonal GetByIndex(int index)
		{
			return NHibernateSession.CreateQuery("from Poligonal p where p.Index = :index").SetParameter("index", index).UniqueResult<Poligonal>();
		}
		public IList<Poligonal> GetByIdMapa(int idMapa)
		{
			return NHibernateSession.CreateQuery("from Poligonal p where p.MapId = :idMapa").SetParameter("idMapa", idMapa).List<Poligonal>();
		}
		public Poligonal GetByCMapId(int mapid, int polid)
		{
			var list = NHibernateSession.CreateQuery("from Poligonal p where p.MapId = :mapid and p.PolId = :polid").SetParameter("mapid", mapid).SetParameter("polid", polid).List<Poligonal>();
			return (list.Count > 0) ? list[0] : null;
		}
		public IList<Direccion> GetByAlturaEnProvincia(string[] tokens, int altura, int provincia)
		{
			var list = NHibernateSession.CreateQuery(QueryBuilder.CreateAlturaQueryBuilder(altura).AddCalle(tokens, 3).AddProvincia(provincia).GetQuery()).SetCacheable(true).List<Altura>();
			var list2 = new List<Direccion>(list.Count);
			foreach (var current in list)
			{
				list2.Add(new Direccion(current, altura));
			}
			return list2;
		}
		public IList<Poligonal> GetEnPartidoYProvincia(string[] tokens, int partido, int provincia)
		{
			return NHibernateSession.CreateQuery(QueryBuilder.CreatePoligonalQueryBuilder().AddCalle(tokens, 3).AddPartido(partido).AddProvincia(provincia).GetQuery()).List<Poligonal>();
		}
		public IList<Poligonal> GetEnPartidoYProvincia(string[] tokens, int partido, int provincia, int altura)
		{
			return NHibernateSession.CreateQuery(QueryBuilder.CreatePoligonalQueryBuilder().AddCalle(tokens, 3).AddPartido(partido).AddProvincia(provincia).AddAltura(altura).GetQuery()).List<Poligonal>();
		}
		public IList<Poligonal> GetEnPartidoYProvincia(int idPartido, int idProvincia)
		{
			return NHibernateSession.CreateQuery("from Poligonal p where p.Partido.PolId = :idPartido and p.Partido.Provincia.MapId = :idProvincia order by p.NombreLargo ").SetParameter("idPartido", idPartido).SetParameter("idProvincia", idProvincia).List<Poligonal>();
		}
		public IList<Poligonal> GetEnProvincia(string[] tokens, int provincia)
		{
			return NHibernateSession.CreateQuery(QueryBuilder.CreatePoligonalQueryBuilder().AddCalle(tokens, 3).AddProvincia(provincia).GetQuery()).List<Poligonal>();
		}
		public IList<Direccion> GetByAlturaSinProvincia(string[] tokens, int altura)
		{
			var list = NHibernateSession.CreateQuery(QueryBuilder.CreateAlturaQueryBuilder(altura).AddCalle(tokens, 3).GetQuery()).SetCacheable(true).List<Altura>();
			var list2 = new List<Direccion>(list.Count);
			foreach (var current in list)
			{
				list2.Add(new Direccion(current, altura));
			}
			return list2;
		}
		public IList<Direccion> GetByEsquinaEnProvincia(string[] tokens, string[] tokensEsquina, int provincia)
		{
			var list = NHibernateSession.CreateQuery(QueryBuilder.CreateCruceQueryBuilder().AddCalle(tokens, 3).AddEsquina(tokensEsquina, 3).AddProvincia(provincia).GetQuery()).SetCacheable(true).List<Cruce>();
			var list2 = new List<Direccion>(list.Count);
			foreach (var current in list)
			{
				list2.Add(new Direccion(current));
			}
			return list2;
		}
		public IList<Direccion> GetByEsquinaSinProvincia(string[] tokens, string[] tokensEsquina)
		{
			var list = NHibernateSession.CreateQuery(QueryBuilder.CreateCruceQueryBuilder().AddCalle(tokens, 3).AddEsquina(tokensEsquina, 3).GetQuery()).SetCacheable(true).List<Cruce>();
			var list2 = new List<Direccion>(list.Count);
			foreach (var current in list)
			{
				list2.Add(new Direccion(current));
			}
			return list2;
		}
		public IList<Direccion> GetDireccion(string[] tokensCalle, int altura, string[] tokensEsquina, string[] tokensPartido, string[] tokensProvincia, double lat, double lon)
		{
			var flag = tokensPartido != null && tokensPartido.Length > 0;
			var flag2 = tokensProvincia != null && tokensProvincia.Length > 0;
			var flag3 = !flag && !flag2 && lat < 1.7976931348623157E+308 && lon < 1.7976931348623157E+308;
			double latitud;
			double latitud2;
			double longitud;
			double longitud2;
			if (flag3)
			{
				latitud = lat - 0.6;
				latitud2 = lat + 0.6;
				longitud = lon - 0.6;
				longitud2 = lon + 0.6;
			}
			else
			{
				latitud2 = (latitud = (longitud = (longitud2 = 0.0)));
			}
			var list = new List<Direccion>();
			if (altura > 0)
			{
				var alturaQueryBuilder = QueryBuilder.CreateAlturaQueryBuilder(altura).AddCalle(tokensCalle, 3);
				if (flag2)
				{
					alturaQueryBuilder = alturaQueryBuilder.AddProvincia(tokensProvincia, 3);
				}
				if (flag3)
				{
					alturaQueryBuilder = alturaQueryBuilder.AddLatitud(latitud, latitud2);
					alturaQueryBuilder = alturaQueryBuilder.AddLongitud(longitud, longitud2);
				}
				var list2 = NHibernateSession.CreateQuery(alturaQueryBuilder.GetQuery()).List<Altura>();
				foreach (var current in list2)
				{
					list.Add(new Direccion(current, altura));
				}
			}
			else
			{
				var cruceQueryBuilder = QueryBuilder.CreateCruceQueryBuilder().AddCalle(tokensCalle, 3).AddEsquina(tokensEsquina, 3);
				if (flag2)
				{
					cruceQueryBuilder = cruceQueryBuilder.AddProvincia(tokensProvincia, 3);
				}
				if (flag3)
				{
					cruceQueryBuilder = cruceQueryBuilder.AddLatitud(latitud, latitud2);
					cruceQueryBuilder = cruceQueryBuilder.AddLongitud(longitud, longitud2);
				}
				var list3 = NHibernateSession.CreateQuery(cruceQueryBuilder.GetQuery()).List<Cruce>();
				foreach (var current2 in list3)
				{
					list.Add(new Direccion(current2));
				}
			}
			return list;
		}
		public IList<Direccion> GetDireccion(string[] tokensCalle, int altura, string[] tokensEsquina, string[] tokensPartido, string[] tokensProvincia)
		{
			var flag = tokensPartido != null && tokensPartido.Length > 0;
			var flag2 = tokensProvincia != null && tokensProvincia.Length > 0;
			var list = new List<Direccion>();
			if (altura > 0)
			{
				var alturaQueryBuilder = QueryBuilder.CreateAlturaQueryBuilder(altura).AddCalle(tokensCalle, 3);
				if (flag)
				{
					alturaQueryBuilder = alturaQueryBuilder.AddPartido(tokensPartido, 3);
				}
				if (flag2)
				{
					alturaQueryBuilder = alturaQueryBuilder.AddProvincia(tokensProvincia, 3);
				}
				var list2 = NHibernateSession.CreateQuery(alturaQueryBuilder.GetQuery()).List<Altura>();
				foreach (var current in list2)
				{
					list.Add(new Direccion(current, altura));
				}
			}
			else
			{
				var cruceQueryBuilder = QueryBuilder.CreateCruceQueryBuilder().AddCalle(tokensCalle, 3).AddEsquina(tokensEsquina, 3);
				if (flag)
				{
					cruceQueryBuilder = cruceQueryBuilder.AddPartido(tokensPartido, 3);
				}
				if (flag2)
				{
					cruceQueryBuilder = cruceQueryBuilder.AddProvincia(tokensProvincia, 3);
				}
				var list3 = NHibernateSession.CreateQuery(cruceQueryBuilder.GetQuery()).List<Cruce>();
				foreach (var current2 in list3)
				{
					list.Add(new Direccion(current2));
				}
			}
			return list;
		}
	}
}
