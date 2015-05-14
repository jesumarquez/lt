using Geocoder.Core.Interfaces;
using System;
using System.Collections.Generic;
namespace Geocoder.Core
{
	[Serializable]
	public class Poligonal : ITokenizable
	{
		private int id;
		private string nombreCorto;
		private string nombreLargo;
		private int mapId;
		private int polId;
		private int alturaMinima;
		private int alturaMaxima;
		private int nivel;
		private int index;
		private Partido partido;
		private bool esExCalle;
		private Poligonal poligonalActual;
		private IList<Cruce> cruces;
		private IList<Localidad> localidades;
		private IList<PalabraPosicionada> palabras;
		private IList<Altura> alturas;
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
		public virtual int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}
		public virtual string NombreCorto
		{
			get
			{
				return this.nombreCorto;
			}
			set
			{
				this.nombreCorto = value;
			}
		}
		public virtual string NombreLargo
		{
			get
			{
				return this.nombreLargo;
			}
			set
			{
				this.nombreLargo = value;
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
		public virtual int AlturaMinima
		{
			get
			{
				return this.alturaMinima;
			}
			set
			{
				this.alturaMinima = value;
			}
		}
		public virtual int AlturaMaxima
		{
			get
			{
				return this.alturaMaxima;
			}
			set
			{
				this.alturaMaxima = value;
			}
		}
		public virtual int Nivel
		{
			get
			{
				return this.nivel;
			}
			set
			{
				this.nivel = value;
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
		public virtual bool EsExCalle
		{
			get
			{
				return this.esExCalle;
			}
			set
			{
				this.esExCalle = value;
			}
		}
		public virtual Poligonal PoligonalActual
		{
			get
			{
				return this.poligonalActual;
			}
			set
			{
				this.poligonalActual = value;
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
		public virtual IList<Cruce> Cruces
		{
			get
			{
				return this.cruces;
			}
			set
			{
				this.cruces = value;
			}
		}
		public virtual IList<Altura> Alturas
		{
			get
			{
				return this.alturas;
			}
			set
			{
				this.alturas = value;
			}
		}
		int ITokenizable.NivelAbreviatura
		{
			get
			{
				return 3;
			}
		}
		public Poligonal() : this(-1, null, null, 0, 0, 0, 0, 1200, null)
		{
		}
		public Poligonal(int index, string nombreCorto, string nombreLargo, int mapId, int polId, int alturaMinima, int alturaMaxima, int nivel, Partido partido) : this(index, nombreCorto, nombreLargo, mapId, polId, alturaMinima, alturaMaxima, nivel, partido, false, null)
		{
		}
		public Poligonal(int index, string nombreCorto, string nombreLargo, int mapId, int polId, int alturaMinima, int alturaMaxima, int nivel, Partido partido, bool esExCalle, Poligonal poligonalActual)
		{
			this.index = index;
			this.nombreCorto = nombreCorto;
			this.nombreLargo = nombreLargo;
			this.mapId = mapId;
			this.polId = polId;
			this.alturaMinima = alturaMinima;
			this.alturaMaxima = alturaMaxima;
			this.nivel = nivel;
			this.partido = partido;
			this.esExCalle = esExCalle;
			this.poligonalActual = poligonalActual;
			this.localidades = new List<Localidad>();
			this.palabras = new List<PalabraPosicionada>();
			this.cruces = new List<Cruce>();
			this.alturas = new List<Altura>();
		}
		public virtual bool EsAlturaValida(int altura)
		{
			return (altura < this.AlturaMinima || altura > this.AlturaMaxima) && false;
		}
		public virtual bool EsCruceValido(Poligonal poligonal)
		{
			return this.EsCruceValido(poligonal.Id);
		}
		public virtual bool EsCruceValido(int poligonal)
		{
			return (poligonal == this.Id || poligonal < 0) && false;
		}
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.PolId,
				") ",
				this.NombreLargo,
				", ",
				this.Partido.Nombre
			});
		}
	}
}
