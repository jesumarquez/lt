using System.Collections.Generic;
using System.Globalization;
using Geocoder.Core.Interfaces;
using Geocoder.Core.VO;
using Geocoder.Logic;
using Logictracker.Configuration;
using Logictracker.Utils;

namespace Logictracker.Services.Helpers
{
    public static class GeocoderHelper
    {
        #region Private Properties

        /// <summary>
        /// Geocoder nomenclator.
        /// </summary>
        private static INomenclador _geocoder;

        /// <summary>
        /// Geocoder cleaning.
        /// </summary>
        private static ICleaning _cleaning;

        /// <summary>
        /// Geocoder singleton.
        /// </summary>
        private static INomenclador Geocoder { get { return _geocoder ?? (_geocoder = new Nomenclador(Config.Map.GeocoderSessionFactory)); } }

        /// <summary>
        /// Cleaning singleton.
        /// </summary>
        public static ICleaning Cleaning { get { return _cleaning ?? (_cleaning = new Cleaning(Config.Map.GeocoderSessionFactory)); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the description associated to the nearest corner to the position givenn.
        /// </summary>
        /// <param name="latitud">The position latitude.</param>
        /// <param name="longitud">The position longitude.</param>
        /// <returns></returns>
        public static string GetDescripcionEsquinaMasCercana(double latitud, double longitud)
        {
            var direccion = GetEsquinaMasCercana(latitud, longitud);
            return direccion.Direccion;
        }

        /// <summary>
        /// Gets all provinces.
        /// </summary>
        /// <returns></returns>
        public static IList<ProvinciaVO> BuscarProvincias() { return Geocoder.BuscarProvincias(); }

        /// <summary>
        /// Gets all directions that mathces the givenn search criteria.
        /// </summary>
        /// <param name="calle">The street.</param>
        /// <param name="altura">The street altitude.</param>
        /// <param name="esquina">The street cross.</param>
        /// <param name="partido">The departmentof the street.</param>
        /// <param name="provincia">The province of the street.</param>
        /// <returns></returns>
        public static IList<DireccionVO> GetDireccion(string calle, int altura, string esquina, string partido, int provincia)
        {
            return Geocoder.GetDireccion(calle, altura, esquina, partido, provincia);
        }

        /// <summary>
        /// Gets all directions that mathces the givenn search criteria.
        /// </summary>
        /// <param name="frase"></param>
        /// <returns></returns>
        public static IList<DireccionVO> GetDireccionSmartSearch(string frase) { return Geocoder.GetSmartSearch(frase); }

        /// <summary>
        /// Gets the neearests cross to the givenn position.
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>
        public static DireccionVO GetEsquinaMasCercana(double latitud, double longitud)
        {
            var direccion = Geocoder.GetEsquinaMasCercana(latitud, longitud);
            if (direccion != null) return direccion;
            var googleDir = GoogleGeocoder.ReverseGeocoding(latitud, longitud);
            if (googleDir != null) return googleDir;
            direccion = new DireccionVO
                            {
                                Altura = -1,
                                Calle = string.Empty,
                                Direccion = string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture),
                                                          longitud.ToString(CultureInfo.InvariantCulture)),
                                IdEsquina = -1,
                                IdMapaUrbano = -1,
                                IdPoligonal = -1,
                                IdProvincia = -1,
                                Latitud = latitud,
                                Longitud = longitud,
                                Partido = string.Empty,
                                Provincia = string.Empty
                            };
            return direccion;
        }


        public static double CalcularDistacia(double lat1, double lon1, double lat2, double lon2)
        {
            if (lat1.Equals(lat2) && lon1.Equals(lon2)) return 0;
            try
            {
                var dirs = GoogleDirections.GetDirections(new LatLon(lat1, lon1), new LatLon(lat2, lon2), GoogleDirections.Modes.Driving, string.Empty);
                if (dirs!=null)
                return dirs.Distance / 1000.0;

                //var dir1 = GetEsquinaMasCercana(lat1, lon1);
                //var dir2 = GetEsquinaMasCercana(lat2, lon2);
                //if (dir1 != null && dir2 != null)
                //{
                //    var d1 = new Direccion(dir1);
                //    var d2 = new Direccion(dir2);
                //    var reco = new Recorrido(d1, d2);
                //    var dist = reco.Distancia;
                //    if (dist > 0) return dist;
                //}
            }
            catch
            {

            }
            return Distancias.Loxodromica(lat1, lon1, lat2, lon2) / 1000.0;
        }
        #endregion
    }
}