using System;
namespace Geocoder.Core.VO
{
	[Serializable]
	public class LocalidadVO
	{
		public int Id
		{
			get;
			set;
		}
		public string Nombre
		{
			get;
			set;
		}
		public int IdMapaUrbano
		{
			get;
			set;
		}
		public int IdPartido
		{
			get;
			set;
		}
		public string Partido
		{
			get;
			set;
		}
		public int IdProvincia
		{
			get;
			set;
		}
		public string Provincia
		{
			get;
			set;
		}
		public LocalidadVO()
		{
		}
		public LocalidadVO(Localidad localidad)
		{
			this.Id = localidad.Id;
			this.Nombre = localidad.Nombre;
			this.IdMapaUrbano = localidad.MapId;
			if (localidad.Partido != null)
			{
				this.IdPartido = localidad.Partido.PolId;
				this.Partido = localidad.Partido.Nombre;
				if (localidad.Partido.Provincia != null)
				{
					this.IdProvincia = localidad.Partido.Provincia.MapId;
					this.Provincia = localidad.Partido.Provincia.Nombre;
				}
			}
		}
	}
}
