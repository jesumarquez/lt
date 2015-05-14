using System;

namespace Geocoder.Core.VO
{
	[Serializable]
	public class DireccionVO
	{
		public string Direccion
		{
			get;
			set;
		}
		public double Latitud { get; set; }
		public double Longitud { get; set; }
		public int IdMapaUrbano { get; set; }
		public int IdPoligonal { get; set; }
		public int IdEsquina { get; set; }
		public int Altura { get; set; }
		public string Provincia { get; set; }
		public int IdProvincia { get; set; }
		public string Calle { get; set; }
		public string Partido { get; set; }
		
        public DireccionVO() { }
		public DireccionVO(Direccion direccion)
		{
			Direccion = direccion.ToString();
			Latitud = direccion.Latitud;
			Longitud = direccion.Longitud;
			IdMapaUrbano = direccion.Poligonal.MapId;
			IdPoligonal = direccion.Poligonal.PolId;
			IdEsquina = ((direccion.Esquina != null) ? direccion.Esquina.PolId : -1);
			Altura = direccion.Altura;
			Provincia = direccion.Poligonal.Partido.Provincia.Nombre;
			IdProvincia = direccion.Poligonal.Partido.Provincia.MapId;
			Calle = direccion.Poligonal.NombreLargo;
			Partido = direccion.Poligonal.Partido.Nombre;
		}
		public DireccionVO(Cruce cruce)
		{
			Direccion = cruce.ToString();
			Latitud = cruce.Latitud;
			Longitud = cruce.Longitud;
			IdMapaUrbano = cruce.Poligonal.MapId;
			IdPoligonal = cruce.Poligonal.PolId;
			IdEsquina = ((cruce.Esquina != null) ? cruce.Esquina.PolId : -1);
			Altura = 0;
			Provincia = cruce.Poligonal.Partido.Provincia.Nombre;
			IdProvincia = cruce.Poligonal.Partido.Provincia.MapId;
			Calle = cruce.Poligonal.NombreLargo;
			Partido = cruce.Poligonal.Partido.Nombre;
		}
	}
}
