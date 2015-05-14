namespace Geocoder.SmartSearch
{
	public class ParsedDirection
	{
		private string _calle = string.Empty;
		private string _esquina = string.Empty;
		private int _altura = -1;
		private string _localidad = string.Empty;
		private double _probabilidad = 1.0;
		private string _provincia = string.Empty;
		public string Calle
		{
			get { return _calle; }
			set { _calle = value; }
		}
		public string Esquina
		{
			get { return _esquina; }
			set { _esquina = value; }
		}
		public int Altura
		{
			get { return _altura; }
			set { _altura = value; }
		}
		public string Localidad
		{
			get { return _localidad; }
			set { _localidad = value; }
		}
		public double Probabilidad
		{
			get { return _probabilidad; }
			set { _probabilidad = value; }
		}
		public string Provincia
		{
			get { return _provincia; }
			set { _provincia = value; }
		}
	}
}
