using Geocoder.Core.VO;
using System;

namespace Geocoder.Logic
{
	[Serializable]
	public class GeocoderService
	{
		private string sessionFactoryConfigPath;
		private Nomenclador __nomenclador;
		private Nomenclador Nomenclador
		{
			get
			{
				if (this.__nomenclador == null)
				{
					this.__nomenclador = ((this.sessionFactoryConfigPath == null) ? new Nomenclador() : new Nomenclador(this.sessionFactoryConfigPath));
				}
				return this.__nomenclador;
			}
		}
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
		public GeocoderService() : this(null)
		{
		}
        public GeocoderService(string sessionFactoryConfigPath)
		{
			this.SessionFactoryConfigPath = sessionFactoryConfigPath;
		}
		public ProvinciaVO[] getAllProvincia()
		{
			ProvinciaVO[] result;
			try
			{
				result = this.Nomenclador.BuscarProvincias();
			}
			catch (Exception innerException)
			{
				throw new Exception("Error", innerException);
			}
			return result;
		}
		public bool AgregarSinonimo(string palabra, string sinonimo)
		{
			return false;
		}
		public bool EliminarSinonimo(string palabra)
		{
			return false;
		}
	}
}
