#region Usings

using System;
using Logictracker.Services.Helpers;

#endregion

namespace Logictracker.Types.ReportObjects.RankingDeOperadores
{
    /// <summary>
    /// The details of a infraction.
    /// </summary>
    [Serializable]
    public class InfractionDetail
    {
        #region Private Properties

        private string _cornerNearest;

        #endregion

        #region Public Properties

        public int Id { get; set; }
        /// <summary>
        /// The calification of the infraction.
        /// </summary>
        public int Calificacion { get; set; }

        /// <summary>
        /// The operator responsable of the infraction.
        /// </summary>
        public string Operador { get; set; }

        /// <summary>
        /// The operator responsable of the infraction.
        /// </summary>
        public string Transportista { get; set; }


        /// <summary>
        /// The mobile envolved in the infraction.
        /// </summary>
        public string Vehiculo { get; set; }

        /// <summary>
        /// The area where the infraction toke place.
        /// </summary>
        public string Zona { get; set; }

        /// <summary>
        /// The specific road where the infraction toke place.
        /// </summary>
        public string Camino { get; set; }

        /// <summary>
        /// The date time of the infraction.
        /// </summary>
        public DateTime Inicio { get; set; }

        /// <summary>
        /// The elapsed time of the infraction.
        /// </summary>
        public TimeSpan Duracion { get { return TimeSpan.FromSeconds(DuracionSegundos) ; } }

        /// <summary>
        /// The speed maximum value.
        /// </summary>
        public int Pico { get; set; }

        /// <summary>
        /// The speed excess.
        /// </summary>
        public int Exceso { get; set; }

        /// <summary>
        /// The infraction weight.
        /// </summary>
        public double Ponderacion { get; set; }

        /// <summary>
        /// Latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Corner nearest of infraction.
        /// </summary>
        public string CornerNearest { get { return _cornerNearest ?? (_cornerNearest = GeocoderHelper.GetDescripcionEsquinaMasCercana(Latitude, Longitude)); } }

        /// <summary>
        /// The elapsed time of the infraction in seconds. 
        /// </summary>
        public int DuracionSegundos { get; set; }

        public string TipoInfraccion { get; set; }

        #endregion
    }
}
