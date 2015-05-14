using System;
namespace Geocoder.Core.VO
{
	[Serializable]
	public class PartidoVO
	{
		private int id;
		private string nombre;
		private string provincia;
		private int idMapaUrbano;
		private int idMapProvincia;
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
		public PartidoVO()
		{
		}
		public PartidoVO(Partido partido)
		{
			this.Id = partido.PolId;
			this.IdMapaUrbano = partido.MapId;
			this.Nombre = partido.Nombre;
			this.Provincia = partido.Provincia.Nombre;
			this.idMapProvincia = partido.Provincia.MapId;
		}
	}
}
