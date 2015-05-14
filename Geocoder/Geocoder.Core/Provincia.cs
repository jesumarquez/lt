using Geocoder.Core.Interfaces;
using System;
using System.Collections.Generic;
namespace Geocoder.Core
{
	[Serializable]
	public class Provincia : ITokenizable
	{
		private int id;
		private int mapId;
		private string nombre;
		private IList<Partido> partidos;
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
		public virtual IList<Partido> Partidos
		{
			get
			{
				return this.partidos;
			}
			set
			{
				this.partidos = value;
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
				return 0;
			}
		}
		public Provincia() : this(0, null)
		{
		}
		public Provincia(int mapId, string nombre)
		{
			this.id = 0;
			this.mapId = mapId;
			this.nombre = nombre;
			this.partidos = new List<Partido>();
			this.palabras = new List<PalabraPosicionada>();
		}
	}
}
