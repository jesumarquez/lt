using System;
namespace Geocoder.Core.VO
{
	[Serializable]
	public class ProvinciaVO
	{
		private int id;
		private string nombre;
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
		public ProvinciaVO()
		{
		}
		public ProvinciaVO(Provincia provincia)
		{
			this.Id = provincia.MapId;
			this.Nombre = provincia.Nombre;
		}
	}
}
