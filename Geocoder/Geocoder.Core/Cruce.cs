using System;
using System.Collections.Generic;

namespace Geocoder.Core
{
	[Serializable]
	public class Cruce
	{
		private int _id;
		private Poligonal _poligonal;
		private Poligonal _esquina;
		private double _latitud;
		private double _longitud;
		
        public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}
		public virtual Poligonal Poligonal
		{
			get { return _poligonal; }
			set { _poligonal = value; }
		}
		public virtual Poligonal Esquina
		{
			get { return _esquina; }
			set { _esquina = value; }
		}
		public virtual double Latitud
		{
			get { return _latitud; }
			set { _latitud = value; }
		}
		public virtual double Longitud
		{
			get { return _longitud; }
			set { _longitud = value; }
		}
		
        public Cruce() : this(null, null, 0.0, 0.0) { }
		public Cruce(Poligonal poligonal, Poligonal esquina, double latitud, double longitud)
		{
			_id = 0;
			_poligonal = poligonal;
			_esquina = esquina;
			_latitud = latitud;
			_longitud = longitud;
		}
		
        public override string ToString()
		{
			string result;
			if (_poligonal == null)
			{
				result = "Sin Descripción";
			}
			else
			{
			    var localidad = string.Empty;
			    var localidades = new List<Localidad>();
                localidades.AddRange(_esquina.Localidades);
                localidades.AddRange(_poligonal.Localidades);
			    foreach (var loc in localidades)
			    {
			        if (loc.Nombre.Trim() != string.Empty)
			        {
			            localidad = loc.Nombre.Trim();
                        break;
			        }
			    }
                
                var text = _poligonal.NombreLargo + " y " + _esquina.NombreLargo;
				text += ", " + _poligonal.Partido.Nombre;
                if (localidad != string.Empty) text += ", " + localidad;
				result = text;
			}
			return result;
		}
	}
}
