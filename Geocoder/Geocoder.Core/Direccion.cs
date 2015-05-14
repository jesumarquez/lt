using System;
namespace Geocoder.Core
{
	[Serializable]
	public class Direccion
	{
		private Poligonal poligonal;
		private Poligonal esquina;
		private int altura;
		private double latitud;
		private double longitud;
		public Poligonal Poligonal
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
		public Poligonal Esquina
		{
			get
			{
				return this.esquina;
			}
			set
			{
				this.esquina = value;
			}
		}
		public int Altura
		{
			get
			{
				return this.altura;
			}
			set
			{
				this.altura = value;
			}
		}
		public double Latitud
		{
			get
			{
				return this.latitud;
			}
			set
			{
				this.latitud = value;
			}
		}
		public double Longitud
		{
			get
			{
				return this.longitud;
			}
			set
			{
				this.longitud = value;
			}
		}
		public Direccion()
		{
		}
		public Direccion(Cruce cruce)
		{
			this.poligonal = cruce.Poligonal;
			this.esquina = cruce.Esquina;
			this.latitud = cruce.Latitud;
			this.longitud = cruce.Longitud;
			this.altura = -1;
		}
		public Direccion(Altura altura, int alturaExacta)
		{
			this.poligonal = altura.Poligonal;
			this.esquina = null;
			this.altura = alturaExacta;
			if (alturaExacta < altura.AlturaInicio)
			{
				this.latitud = altura.LatitudInicio;
				this.longitud = altura.LongitudInicio;
			}
			else
			{
				double num = (double)(alturaExacta - altura.AlturaInicio) / (double)(altura.AlturaFin - altura.AlturaInicio);
				this.latitud = altura.LatitudInicio + (altura.LatitudFin - altura.LatitudInicio) * num;
				this.longitud = altura.LongitudInicio + (altura.LongitudFin - altura.LongitudInicio) * num;
			}
		}
		public override string ToString()
		{
			string result;
			if (this.poligonal == null)
			{
				result = "Sin Descripcion";
			}
			else
			{
				string text = this.poligonal.NombreLargo;
				if (this.altura > 0)
				{
					text += " " + this.altura;
				}
				if (this.esquina != null)
				{
					text += ((this.altura > 0) ? (" (esq. " + this.esquina.NombreLargo + ")") : (" y " + this.esquina.NombreLargo));
				}
				text += ", " + this.poligonal.Partido.Nombre;
				if (this.poligonal.MapId > 0)
				{
					text += ", " + this.poligonal.Partido.Provincia.Nombre;
				}
				result = text;
			}
			return result;
		}
	}
}
