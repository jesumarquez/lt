using Geocoder.Core.Interfaces;
using System;
using System.Collections.Generic;
namespace Geocoder.Core
{
	[Serializable]
	public class Partido : ITokenizable
	{
		private int id;
		private string nombre;
		private int polId;
		private int mapId;
		private Provincia provincia;
		private IList<Localidad> localidades;
		private IList<PalabraPosicionada> palabras;
		public virtual int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		public virtual string Nombre
		{
			get
			{
				return this.nombre;
			}
			set
			{
				this.nombre = value;
			}
		}
		public virtual int PolId
		{
			get
			{
				return this.polId;
			}
			set
			{
				this.polId = value;
			}
		}
		public virtual int MapId
		{
			get
			{
				return this.mapId;
			}
			set
			{
				this.mapId = value;
			}
		}
		public virtual Provincia Provincia
		{
			get
			{
				return this.provincia;
			}
			set
			{
				this.provincia = value;
			}
		}
		public virtual IList<Localidad> Localidades
		{
			get
			{
				return this.localidades;
			}
			set
			{
				this.localidades = value;
			}
		}
		public virtual IList<PalabraPosicionada> Palabras
		{
			get
			{
				return this.palabras;
			}
			set
			{
				this.palabras = value;
			}
		}
		int ITokenizable.NivelAbreviatura
		{
			get
			{
				return 1;
			}
		}
		public Partido() : this(null, 0, 0, null)
		{
		}
		public Partido(string nombre, int polId, int mapId, Provincia provincia)
		{
			this.id = 0;
			this.nombre = nombre;
			this.polId = polId;
			this.mapId = mapId;
			this.provincia = provincia;
			this.localidades = new List<Localidad>();
			this.palabras = new List<PalabraPosicionada>();
		}
	}
}
