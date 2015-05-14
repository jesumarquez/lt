using Geocoder.Core;
using Geocoder.Data.DAO.QueryBuilders;
using Geocoder.Data.SessionManagement;
using System;
using System.Collections.Generic;
namespace Geocoder.Data.DAO
{
	public class PartidoDAO : AbstractDAO<Partido, int>
	{
		private const int START_WITH_LENGTH = 3;
		public PartidoDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
		{
		}
		public int DeletePoligonalesByMapId(int mapId)
		{
			return base.NHibernateSession.Delete("from Poligonal poli where poli.Partido.MapId = " + mapId);
		}
		public int DeletePoligonales(int partido)
		{
			return base.NHibernateSession.Delete("from Poligonal poli where poli.Partido.Id = " + partido);
		}
		public Partido GetByMapId(int mapId)
		{
			IList<Partido> list = base.NHibernateSession.CreateQuery("from Partido p where p.MapId = :mapId").SetParameter<int>("mapId", mapId).List<Partido>();
			return (list.Count > 0) ? list[0] : null;
		}
		public long GetCountSinPalabras()
		{
			return (long)base.NHibernateSession.CreateQuery("select count(p) from Partido p where size(p.Palabras) = 0").UniqueResult();
		}
		public IList<Partido> GetSinPalabras()
		{
			return base.NHibernateSession.CreateQuery("from Partido p where size(p.Palabras) = 0").List<Partido>();
		}
		public IList<Partido> GetByProvinciaMapId(int idMapProvincia)
		{
			return base.NHibernateSession.CreateQuery("from Partido p where p.Provincia.MapId = :idMapProvincia order by p.Nombre").SetParameter<int>("idMapProvincia", idMapProvincia).List<Partido>();
		}
		public IList<Partido> GetByProvinciaId(int idProvincia)
		{
			return base.NHibernateSession.CreateQuery("from Partido p where p.Provincia.Id = :idProvincia order by p.Nombre").SetParameter<int>("idProvincia", idProvincia).List<Partido>();
		}
		public IList<Partido> GetEnProvincia(string[] tokens, int idMapProvincia)
		{
			return base.NHibernateSession.CreateQuery(QueryBuilder.CreatePartidoQueryBuilder().AddPartido(tokens, 3).AddProvincia(idMapProvincia).GetQuery()).List<Partido>();
		}
	}
}
