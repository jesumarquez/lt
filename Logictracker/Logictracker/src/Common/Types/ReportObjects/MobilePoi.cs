#region Usings

using System;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.ValueObject;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents info about a mobile near to a point of interest.
    /// </summary>
    [Serializable]
    public class MobilePoi
    {
        #region Constructors

        public MobilePoi() {}

        public MobilePoi(LogUltimaPosicionVo posicion, ReferenciaGeografica referencia, Empleado chofer)
        {
            IdVehiculo = posicion.IdCoche;
            Interno = posicion.Coche;
            TipoVehiculo = posicion.TipoCoche;
            Distancia = Distancias.Loxodromica(posicion.Latitud, posicion.Longitud, referencia.Latitude, referencia.Longitude);
            Latitud = posicion.Latitud;
            Longitud = posicion.Longitud;
            PuntoDeInteres = referencia.Descripcion;
            Velocidad = posicion.Velocidad;
            Responsable = posicion.Responsable;
            Chofer = chofer != null ? chofer.Entidad.Descripcion : string.Empty;
        }
        public MobilePoi(LogUltimaPosicionVo posicion, Geocerca referencia, Empleado chofer)
        {
            IdVehiculo = posicion.IdCoche;
            Interno = posicion.Coche;
            TipoVehiculo = posicion.TipoCoche;
            Distancia = Distancias.Loxodromica(posicion.Latitud, posicion.Longitud, referencia.Latitude, referencia.Longitude);
            Latitud = posicion.Latitud;
            Longitud = posicion.Longitud;
            PuntoDeInteres = referencia.Descripcion;
            Velocidad = posicion.Velocidad;
            Responsable = posicion.Responsable;
            Chofer = chofer != null ? chofer.Entidad.Descripcion : string.Empty;
        }

        #endregion

        #region Public Properties


        public int IdVehiculo { get; set; }
        /// <summary>
        /// The mobile intern.
        /// </summary>
        public string Interno { get; set; }

        /// <summary>
        /// The mobile type.
        /// </summary>
        public string TipoVehiculo { get; set; }

        /// <summary>
        /// The mobile driver.
        /// </summary>
        public string Chofer { get; set; }

        /// <summary>
        /// The mobile responsable.
        /// </summary>
        public string Responsable { get; set;}

        /// <summary>
        /// The current distance to the point of interest.
        /// </summary>
        public double Distancia { get; set; }

        /// <summary>
        /// The current latitude.
        /// </summary>
        public double Latitud { get; set; }

        /// <summary>
        /// The current longitude.
        /// </summary>
        public double Longitud { get; set; }

        /// <summary>
        /// The point of interest description.
        /// </summary>
        public string PuntoDeInteres { get; set; }

        /// <summary>
        /// The current speed of the mobile.
        /// </summary>
        public int Velocidad { get; set; }

        /// <summary>
        /// The nearest corner to the actual position of the mobile.
        /// </summary>
        public string Esquina
        {
            get
            {
                return _esquina ?? (_esquina = GeocoderHelper.GetDescripcionEsquinaMasCercana(Latitud, Longitud));
            }
        }

        private string _esquina;
        #endregion
    }
}
