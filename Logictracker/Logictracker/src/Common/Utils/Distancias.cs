#region Usings

using System;

#endregion

namespace Logictracker.Utils
{
    public static class Distancias
    {
        /// <summary>
        /// Valor constante que representa en km la distancia entre 2 meridianos.
        /// </summary>
        private const double DgreeEcuador = EarthDiameter / 360.0; // longitud en km de un grado en el ecuador

        /// <summary>
        /// Valor constante del diametro de la tierra en km.
        /// </summary>
        private const double EarthDiameter = 40024.0;

        /// <summary>
        /// Valor constante del radio de la tierra en metros.
        /// </summary>
        private const double EarthRadius = 6371000.0;

        /// <summary>
        /// Valor constante de un radian.
        /// </summary>
        private const double Radians = Math.PI / 180;

        /// <summary>
        /// Distancia Loxodromica en metros.
        /// </summary>
        /// <param name="latA"> Latitud del primer punto</param>
        /// <param name="lonA"> Longirud del primer punto</param>
        /// <param name="latB"> Latitud del segundo punto</param>
        /// <param name="lonB"> Latitud del segundo punto</param>
        /// <returns></returns>
        public static double Loxodromica(double latA, double lonA, double latB, double lonB)
        {
            if (latA == latB && lonA == lonB) return 0;

            var p = latA * Radians;
            var b = latB * Radians;

            var deltGama = Math.Abs(lonA - lonB) * Radians;
            var t = (Math.Sin(p) * Math.Sin(b)) + (Math.Cos(p) * Math.Cos(b) * Math.Cos(deltGama));
            var d = Math.Acos(t);

            return Math.Abs(((d * (180 / Math.PI)) * DgreeEcuador) * 1000.0);
        }

        /// <summary>
        /// Distancia Loxodromica con altitud en metros.
        /// </summary>
        /// <param name="latA"> Latitud del primer punto</param>
        /// <param name="lonA"> Longirud del primer punto</param>
        /// <param name="altA"> Altitud del primer punto</param>
        /// <param name="latB"> Latitud del segundo punto</param>
        /// <param name="lonB"> Latitud del segundo punto</param>
        /// <param name="altB"> Altitud del segundo punto</param>
        /// <returns></returns>
        public static double LoxodromicaConAltitud(double latA, double lonA, double altA, double latB, double lonB, double altB)
        {
            var iniLon = lonA * Radians;
            var finLon = lonB * Radians;
            var iniLat = latA * Radians;
            var finLat = latB * Radians;
            var iniAlt = altA + EarthRadius;
            var finAlt = altB + EarthRadius;

            var xi = iniAlt * Math.Cos(iniLat) * Math.Sin(iniLon);
            var yi = iniAlt * Math.Sin(iniLat);
            var zi = iniAlt * Math.Cos(iniLat) * Math.Cos(iniLon);
            var xf = finAlt * Math.Cos(finLat) * Math.Sin(finLon);
            var yf = finAlt * Math.Sin(finLat);
            var zf = finAlt * Math.Cos(finLat) * Math.Cos(@finLon);

            return Math.Sqrt((xf - xi) * (xf - xi) + (yf - yi) * (yf - yi) + (zf - zi) * (zf - zi));
        }
    }
}
