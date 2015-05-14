using System;
namespace Geocoder.Core
{
	[Serializable]
	public class Altura
	{
		private int id;
		private Poligonal poligonal;
		private int alturaInicio;
		private int alturaFin;
		private double latitudInicio;
		private double longitudInicio;
		private double latitudFin;
		private double longitudFin;
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
		public virtual Poligonal Poligonal
		{
			get
			{
				return this.poligonal;
			}
			set
			{
				this.poligonal = value;
			}
		}
		public virtual int AlturaInicio
		{
			get
			{
				return this.alturaInicio;
			}
			set
			{
				this.alturaInicio = value;
			}
		}
		public virtual int AlturaFin
		{
			get
			{
				return this.alturaFin;
			}
			set
			{
				this.alturaFin = value;
			}
		}
		public virtual double LatitudInicio
		{
			get
			{
				return this.latitudInicio;
			}
			set
			{
				this.latitudInicio = value;
			}
		}
		public virtual double LongitudInicio
		{
			get
			{
				return this.longitudInicio;
			}
			set
			{
				this.longitudInicio = value;
			}
		}
		public virtual double LatitudFin
		{
			get
			{
				return this.latitudFin;
			}
			set
			{
				this.latitudFin = value;
			}
		}
		public virtual double LongitudFin
		{
			get
			{
				return this.longitudFin;
			}
			set
			{
				this.longitudFin = value;
			}
		}
		public Altura() : this(null, 0, 0, 0.0, 0.0, 0.0, 0.0)
		{
		}
		public Altura(Poligonal poligonal, int alturaInicio, int alturaFin, double latitudInicio, double longitudInicio, double latitudFin, double longitudFin)
		{
			this.poligonal = poligonal;
			this.alturaInicio = alturaInicio;
			this.alturaFin = alturaFin;
			this.latitudInicio = latitudInicio;
			this.longitudInicio = longitudInicio;
			this.latitudFin = latitudFin;
			this.longitudFin = longitudFin;
		}
		public virtual bool EsAlturaValida(int altura)
		{
			return altura >= this.AlturaInicio && altura <= this.AlturaFin;
		}
	}
}
