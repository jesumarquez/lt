using System;
namespace Geocoder.Core.VO
{
	[Serializable]
	public class PoligonalVO
	{
		private int id;
		private string nombre;
		private string nombreCorto;
		private int alturaInicial;
		private int alturaFinal;
		private string partido;
		private int idMapaUrbano;
		private int idPartido;
		private string provincia;
		private int idMapProvincia;
		private string tipo;
		public int Id
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
		public string Nombre
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
		public string NombreCorto
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
		public string Partido
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
		public string Provincia
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
		public int AlturaInicial
		{
			get
			{
				return this.alturaInicial;
			}
			set
			{
				this.alturaInicial = value;
			}
		}
		public int AlturaFinal
		{
			get
			{
				return this.alturaFinal;
			}
			set
			{
				this.alturaFinal = value;
			}
		}
		public int IdMapaUrbano
		{
			get
			{
				return this.idMapaUrbano;
			}
			set
			{
				this.idMapaUrbano = value;
			}
		}
		public string Tipo
		{
			get
			{
				return this.tipo;
			}
			set
			{
				this.tipo = value;
			}
		}
		public int IdPartido
		{
			get
			{
				return this.idPartido;
			}
			set
			{
				this.idPartido = value;
			}
		}
		public int IdMapProvincia
		{
			get
			{
				return this.idMapProvincia;
			}
			set
			{
				this.idMapProvincia = value;
			}
		}
		public PoligonalVO()
		{
		}
		public PoligonalVO(Poligonal poligonal)
		{
			this.Id = poligonal.PolId;
			this.Nombre = poligonal.NombreLargo;
			this.NombreCorto = poligonal.NombreCorto;
			this.Partido = poligonal.Partido.Nombre;
			this.Provincia = poligonal.Partido.Provincia.Nombre;
			this.AlturaInicial = poligonal.AlturaMinima;
			this.AlturaFinal = poligonal.AlturaMaxima;
			this.IdMapaUrbano = poligonal.MapId;
			this.Tipo = poligonal.Nivel.ToString();
			this.IdPartido = poligonal.Partido.PolId;
			this.IdMapProvincia = poligonal.Partido.Provincia.MapId;
		}
	}
}
