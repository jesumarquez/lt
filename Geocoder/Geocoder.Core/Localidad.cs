using Geocoder.Core.Interfaces;
using System;
using System.Collections.Generic;
namespace Geocoder.Core
{
	[Serializable]
	public class Localidad : ITokenizable
	{
		private int id;
		private string nombre;
		private int mapId;
		private Partido partido;
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
		public virtual Partido Partido
		{
			get
			{
				return this.partido;
			}
			set
			{
				this.partido = value;
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
				return 2;
			}
		}
		public Localidad() : this(null, 0, null)
		{
		}
		public Localidad(string nombre, int mapId, Partido partido)
		{
			this.id = 0;
			this.nombre = nombre;
			this.mapId = mapId;
			this.partido = partido;
			this.palabras = new List<PalabraPosicionada>();
		}
	}
}
