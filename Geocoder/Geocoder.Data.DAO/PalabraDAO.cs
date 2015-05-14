using Geocoder.Core;
using Geocoder.Data.SessionManagement;
using System;
using System.Collections.Generic;
namespace Geocoder.Data.DAO
{
	public class PalabraDAO : AbstractDAO<Palabra, int>
	{
		public PalabraDAO(string sessionFactoryConfigPath) : base(sessionFactoryConfigPath)
		{
		}
		public Palabra GetByLiteral(string literal)
		{
			IList<Palabra> list = base.NHibernateSession.CreateQuery("from Palabra p where p.Literal = :literal").SetParameter<string>("literal", literal).SetCacheable(true).List<Palabra>();
			return (list.Count > 0) ? list[0] : null;
		}
		public PalabraPosicionada GetByLiteralPosicion(int idPalabra, int posicion)
		{
			IList<PalabraPosicionada> list = base.NHibernateSession.CreateQuery("from PalabraPosicionada p where p.Palabra.Id = :pal and p.Posicion = :pos").SetParameter<int>("pal", idPalabra).SetParameter<int>("pos", posicion).SetCacheable(true).List<PalabraPosicionada>();
			return (list.Count > 0) ? list[0] : null;
		}
		public IList<PalabraPosicionada> GetAllPalabrasPosicionadas()
		{
			return base.NHibernateSession.CreateQuery("from PalabraPosicionada p").SetCacheable(true).List<PalabraPosicionada>();
		}
		public IList<Palabra> GetByLiteral(string[] normalizadas)
		{
			return base.NHibernateSession.CreateQuery("from Palabra p where p.Normalizada in (:normalizadas)").SetParameterList("normalizadas", normalizadas).SetCacheable(true).List<Palabra>();
		}
	}
}
