using Geocoder.Core;
using Geocoder.Data.SessionManagement;
using System;
using System.Collections.Generic;
namespace Geocoder.Data.DAO
{
	public class AbreviaturaDAO : AbstractDAO<Abreviatura, int>
	{
		public AbreviaturaDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
		{
		}
		public IList<Abreviatura> GetByLiteral(string literal)
		{
			return base.NHibernateSession.CreateQuery("from Abreviatura a where a.Literal = :literal").SetParameter<string>("literal", literal).List<Abreviatura>();
		}
	}
}
