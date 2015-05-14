using Geocoder.Core;
using Geocoder.Data.DAO.QueryBuilders;
using Geocoder.Data.SessionManagement;
using System;
using System.Collections.Generic;
namespace Geocoder.Data.DAO
{
	public class ProvinciaDAO : AbstractDAO<Provincia, int>
	{
		private const int START_WITH_LENGTH = 3;
		public ProvinciaDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
		{
		}
		public IList<Provincia> GetTodas()
		{
			return base.NHibernateSession.CreateQuery("from Provincia p order by p.Nombre").SetCacheable(true).List<Provincia>();
		}
		public IList<Provincia> GetSinPalabras()
		{
			return base.NHibernateSession.CreateQuery("from Provincia p where size(p.Palabras) = 0").List<Provincia>();
		}
		public IList<Provincia> GetProvincias(string[] tokens)
		{
			return base.NHibernateSession.CreateQuery(QueryBuilder.CreateProvinciaQueryBuilder().AddProvincia(tokens, 3).GetQuery()).List<Provincia>();
		}
	}
}
