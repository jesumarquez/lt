using System;
using System.Collections.Generic;

namespace Urbetrack.Mobile.Comm.GEO
{
    /// <summary>
    /// Funciones de calculo de distancias entre posiciones GPS.
    /// Utilizando la funcion loxodromica (a vuelo de pajaro).
    /// </summary>
    public class Distances
    {
        /// <summary>
        /// Valor constante de un radian.
        /// </summary>
        public const double RADIANS = Math.PI / 180;
        /// <summary>
        /// Valor constante del diametro de la tierra en km.
        /// </summary>
        public const double EARTH_DIAMETER = 40024.0;
        /// <summary>
        /// Valor constante que representa en km la distancia entre 2 meridianos.
        /// </summary>
        public const double DGREE_ECUADOR = EARTH_DIAMETER / 360.0; // longitud en km de un grado en el ecuador
        /// <summary>
        /// Calcula la distancia loxodromica entre dos GPSPoint's
        /// </summary>
        /// <param name="pointA">Punto GPS 1</param>
        /// <param name="pointB">Punto GPS 2</param>
        /// <returns>Distancia en metros entre los 2 puntos GPS</returns>
        public static double Rhumb(GPSPoint pointA, GPSPoint pointB)
        {
            return Rhumb(pointA.Lat, pointA.Lon, pointB.Lat, pointB.Lon);
        }

        /// <summary>
        /// Calcula el largo de un recorrido de puntos GPSPoint ordenados.
        /// </summary>
        /// <param name="points">Lista de puntos GPS ordenados</param>
        /// <returns>Distancia a recorrer en metros para completar el rumbo especificado.</returns>
        public static double Rhumb(List<GPSPoint> points)
        {
            double distance = 0;
            GPSPoint prev = null;
            foreach (GPSPoint p in points)
            {
                if (prev == null)
                {
                    prev = p;
                    continue;
                }
                distance += Rhumb(prev, p);
                prev = p;
            }
            return distance;
        }

        /// <summary>
        /// Calcula la distancia loxodromica entre dos pares LAT/LON
        /// </summary>
        /// <param name="latA">Latitud del punto 1</param>
        /// <param name="lonA">Longitud del punto 1</param>
        /// <param name="latB">Latitud del punto 2</param>
        /// <param name="lonB">Longitud del punto 2</param>
        /// <returns>Distancia expresanda en metros entre los 2 pares LAT/LON</returns>
        public static double Rhumb(double latA, double lonA, double latB, double lonB)
        {
            if (latA == latB && lonA == lonB)
                return 0;

            double p, b;		// latitud de puntos P y B
            double deltGama;	// diferencia de longitud entre puntos P y B
            double t;
            double d;

            p = latA * RADIANS;
            b = latB * RADIANS;

            deltGama = Math.Abs(lonA - lonB) * RADIANS;
            t = (Math.Sin(p) * Math.Sin(b)) + (Math.Cos(p) * Math.Cos(b) * Math.Cos(deltGama));
            d = Math.Acos(t);

            return Math.Abs(((d * (180 / Math.PI)) * DGREE_ECUADOR) * 1000.0);

        }
    }
}