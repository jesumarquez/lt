using System;
namespace Geocoder.Core
{
	[Serializable]
	public class PalabraPosicionada
	{
		private int id;
		private Palabra palabra;
		private int posicion;
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
		public virtual Palabra Palabra
		{
			get
			{
				return this.palabra;
			}
			set
			{
				this.palabra = value;
			}
		}
		public virtual int Posicion
		{
			get
			{
				return this.posicion;
			}
			set
			{
				this.posicion = value;
			}
		}
		public PalabraPosicionada() : this(null, -1)
		{
		}
		public PalabraPosicionada(Palabra palabra, int posicion)
		{
			this.palabra = palabra;
			this.posicion = posicion;
			this.id = 0;
		}
	}
}
