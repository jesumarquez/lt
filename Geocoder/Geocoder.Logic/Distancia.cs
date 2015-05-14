using System;

namespace Geocoder.Logic
{
	public static class Distancia
	{
		private const double RADIANES = 0.017453292519943295;
		private const double DIAMETRO_TIERRA = 40024.0;
		private const double GRADO_EN_ECUADOR = 111.17777777777778;
		public static double Loxodromica(double latA, double lonA, double latB, double lonB)
		{
			double result;
			if (latA == latB && lonA == lonB)
			{
				result = 0.0;
			}
			else
			{
				double num = latA * 0.017453292519943295;
				double num2 = latB * 0.017453292519943295;
				double d = Math.Abs(lonA - lonB) * 0.017453292519943295;
				double d2 = Math.Sin(num) * Math.Sin(num2) + Math.Cos(num) * Math.Cos(num2) * Math.Cos(d);
				double num3 = Math.Acos(d2);
				result = Math.Abs(num3 * 57.295779513082323 * 111.17777777777778 * 1000.0);
			}
			return result;
		}
	}
}
