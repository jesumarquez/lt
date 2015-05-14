#region Usings

using System;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Utils;

#endregion

namespace Logictracker.Types.ReportObjects
{
    public class MobilePoiHistoric
    {
        #region Constructors

        public MobilePoiHistoric() {}

        public MobilePoiHistoric(LogPosicionBase posicion, ReferenciaGeografica referencia, DateTime fecha)
        {
            Interno = posicion.Coche.Interno;
            TipoVehiculo = posicion.Coche.TipoCoche.Descripcion;
            Distancia = Distancias.Loxodromica(posicion.Latitud, posicion.Longitud, referencia.Latitude, referencia.Longitude);
            Latitud = posicion.Latitud;
            Longitud = posicion.Longitud;
            PuntoDeInteres = referencia.Descripcion;
            Velocidad = posicion.Velocidad;
            Responsable = posicion.Coche.Chofer != null ? posicion.Coche.Chofer.Entidad.Descripcion : string.Empty;
            Fecha = fecha;
            RefId = referencia.Id;
        }

        #endregion

        #region Private Properties

        private string _esquina = string.Empty;

        #endregion

        /// <summary>
        /// The mobile intern.
        /// </summary>
        public string Interno { get; set; }

        /// <summary>
        /// The GeoRef id.
        /// </summary>
        public int RefId { get; set; }

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

        public DateTime Fecha { get; set; }

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
    }
}
