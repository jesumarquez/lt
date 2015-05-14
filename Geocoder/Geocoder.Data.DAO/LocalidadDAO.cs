using Geocoder.Core;
using Geocoder.Data.DAO.QueryBuilders;
using Geocoder.Data.SessionManagement;
using System;
using System.Collections.Generic;
namespace Geocoder.Data.DAO
{
	public class LocalidadDAO : AbstractDAO<Localidad, int>
	{
		private const int START_WITH_LENGTH = 3;
		public LocalidadDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
		{
		}
		public long GetCountSinPalabras()
		{
			return (long)base.NHibernateSession.CreateQuery("select count(l)from Localidad l where size(l.Palabras) = 0").UniqueResult();
		}
		public IList<Localidad> GetSinPalabras()
		{
			return base.NHibernateSession.CreateQuery("from Localidad l where size(l.Palabras) = 0").List<Localidad>();
		}
		public Localidad GetByMapId(int mapId)
		{
			IList<Localidad> list = base.NHibernateSession.CreateQuery("from Localidad l where l.MapId = :mapId").SetParameter<int>("mapId", mapId).List<Localidad>();
			return (list.Count > 0) ? list[0] : null;
		}
		public IList<Localidad> GetByPartido(int idPartido, int idProvincia)
		{
			return base.NHibernateSession.CreateQuery("from Localidad l where l.Partido.PolId = :idPartido and l.Partido.Provincia.MapId = :idProvincia order by l.Nombre").SetParameter<int>("idPartido", idPartido).SetParameter<int>("idProvincia", idProvincia).List<Localidad>();
		}
		public IList<Localidad> GetByProvincia(string[] tokens, int idMapProvincia)
		{
			return base.NHibernateSession.CreateQuery(QueryBuilder.CreateLocalidadQueryBuilder().AddLocalidad(tokens, 3).AddProvincia(idMapProvincia).GetQuery()).List<Localidad>();
		}
	}
}
