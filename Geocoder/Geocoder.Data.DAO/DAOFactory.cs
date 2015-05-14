using Geocoder.Data.SessionManagement;
using NHibernate;
using System;
namespace Geocoder.Data.DAO
{
	public class DAOFactory
	{
		private string sessionFactoryConfigPath;
		public string SessionFactoryConfigPath
		{
			get
			{
				return this.sessionFactoryConfigPath;
			}
			set
			{
				this.sessionFactoryConfigPath = value;
			}
		}
		public ProvinciaDAO ProvinciaDAO
		{
			get
			{
				return new ProvinciaDAO(this.sessionFactoryConfigPath);
			}
		}
		public PartidoDAO PartidoDAO
		{
			get
			{
				return new PartidoDAO(this.sessionFactoryConfigPath);
			}
		}
		public LocalidadDAO LocalidadDAO
		{
			get
			{
				return new LocalidadDAO(this.sessionFactoryConfigPath);
			}
		}
		public PoligonalDAO PoligonalDAO
		{
			get
			{
				return new PoligonalDAO(this.sessionFactoryConfigPath);
			}
		}
		public AbreviaturaDAO AbreviaturaDAO
		{
			get
			{
				return new AbreviaturaDAO(this.sessionFactoryConfigPath);
			}
		}
		public PalabraDAO PalabraDAO
		{
			get
			{
				return new PalabraDAO(this.sessionFactoryConfigPath);
			}
		}
		public CruceDAO CruceDAO
		{
			get
			{
				return new CruceDAO(this.sessionFactoryConfigPath);
			}
		}
		public AlturaDAO AlturaDAO
		{
			get
			{
				return new AlturaDAO(this.sessionFactoryConfigPath);
			}
		}
		protected ISession NHibernateSession
		{
			get
			{
				return NHibernateSessionManager.Instance.GetSessionFrom(this.SessionFactoryConfigPath);
			}
		}
		public DAOFactory(string sessionFactoryConfigPath)
		{
			if (sessionFactoryConfigPath == null)
			{
				throw new ArgumentNullException("sessionFactoryConfigPath may not be null");
			}
			this.SessionFactoryConfigPath = sessionFactoryConfigPath;
		}
		public void ClearSession()
		{
			this.NHibernateSession.Clear();
		}
	}
}
