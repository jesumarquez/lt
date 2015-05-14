using System;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.ValueObject.Messages;
using Logictracker.Types.ValueObject.Positions;

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Information about the positions reported by a mobile with a specific device.
    /// </summary>
    [Serializable]
    public class MobilePosition
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new empty mobile position.
        /// </summary>
        public MobilePosition() {}

        /// <summary>
        /// Gets a mobile position based on the givenn vehicle, position and rfid.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="lastPosition"></param>
        /// <param name="lastRfid"></param>
        public MobilePosition(Coche mobile, LogUltimaPosicionVo lastPosition, LogUltimoLoginVo lastRfid)
        {
            Dispositivo = lastPosition != null ? lastPosition.Dispositivo : mobile.Dispositivo != null ? mobile.Dispositivo.Codigo : string.Empty;
            Interno = mobile.Interno;
            Patente = mobile.Patente;
            Fecha = lastPosition != null ? lastPosition.FechaMensaje : (DateTime?) null;
            IdDispositivo = lastPosition != null ? lastPosition.IdDispositivo : mobile.Dispositivo != null ? mobile.Dispositivo.Id : 0;
            IdDispositivoCoche = mobile.Dispositivo != null ? mobile.Dispositivo.Id : 0;
            FechaAlta = lastPosition != null ? lastPosition.FechaRecepcion : (DateTime?) null;
            Latitud = lastPosition != null ? lastPosition.Latitud : 0;
            Longitud = lastPosition != null ? lastPosition.Longitud : 0;
            Responsable = lastPosition != null ? lastPosition.Responsable : string.Empty;
            IdMovil = mobile.Id;
            IdBase = mobile.Linea != null ? mobile.Linea.Id : 0;
            Velocidad = lastPosition != null ? lastPosition.Velocidad : -1;
            IdPosicion = lastPosition != null ? lastPosition.Id : 0;
            //Firmware = lastPosition != null ? lastPosition.Firmware : mobile.Dispositivo != null ? mobile.Dispositivo.FullFirmwareVersion : string.Empty;
            ReferenciaVehiculo = mobile.Referencia;
            TipoDispositivo = lastPosition != null ? lastPosition.TipoDispositivo : mobile.Dispositivo != null ? GetDeviceTypeDescription(mobile.Dispositivo) : string.Empty;
            EstadoReporte = lastPosition != null ? lastPosition.EstadoReporte : 4;
            Distrito = GetDistrito(mobile);
            Base = GetBase(mobile);
            Transportista = GetTransportista(mobile);
            TipoVehiculo = mobile.TipoCoche.Descripcion;
            IdDistrito = mobile.Empresa != null ? mobile.Empresa.Id : 0;
            //Qtree = lastPosition != null ? lastPosition.Qtree : string.Empty;
            Chofer = lastRfid != null ? lastRfid.Chofer : string.Empty;
            UltimoLogin = lastRfid != null && lastRfid.Fecha.HasValue ? lastRfid.Fecha.Value : (DateTime?) null;
            CentroDeCosto = mobile.CentroDeCostos != null ? mobile.CentroDeCostos.Descripcion : string.Empty;
        }

        /// <summary>
        /// Gets a mobile position based on the givenn vehicle, position and rfid.
        /// </summary>
        /// <param name="dispositivo"></param>
        /// <param name="coche"></param>
        /// <param name="lastPosition"></param>
        /// <param name="lastRfid"></param>
        public MobilePosition(Dispositivo dispositivo, Coche coche, LogUltimaPosicionVo lastPosition, LogUltimoLoginVo lastRfid)
        {
            Dispositivo = dispositivo.Codigo;
            Interno = coche != null ? coche.Interno : null;
            Patente = coche != null ? coche.Patente : null;
            Fecha = lastPosition != null ? lastPosition.FechaMensaje : (DateTime?) null;
            IdDispositivo = dispositivo.Id;
            IdDispositivoCoche = coche != null && coche.Dispositivo != null ? coche.Dispositivo.Id : 0;
            FechaAlta = lastPosition != null ? lastPosition.FechaRecepcion : (DateTime?) null;
            Latitud = lastPosition != null ? lastPosition.Latitud : 0;
            Longitud = lastPosition != null ? lastPosition.Longitud : 0;
            Responsable = lastPosition != null ? lastPosition.Responsable : string.Empty;
            IdMovil = coche != null ? coche.Id : 0;
            IdBase = dispositivo.Linea != null ? dispositivo.Linea.Id : 0;
            Velocidad = lastPosition != null ? lastPosition.Velocidad : -1;
            IdPosicion = lastPosition != null ? lastPosition.Id : 0;
            //Firmware = dispositivo.FullFirmwareVersion;
            ReferenciaVehiculo = coche != null ? coche.Referencia : string.Empty;
            TipoDispositivo = GetDeviceTypeDescription(dispositivo);
            EstadoReporte = lastPosition != null ? lastPosition.EstadoReporte : 4;
            Distrito = coche != null ? GetDistrito(coche) : GetDistrito(dispositivo);
            Base = coche != null ? GetBase(coche) : GetBase(dispositivo);
            Transportista = GetTransportista(coche);
            TipoVehiculo = coche != null ? coche.TipoCoche.Descripcion : string.Empty;
            IdDistrito = dispositivo.Empresa != null ? dispositivo.Empresa.Id : 0;
            //Qtree = dispositivo.QtreeRevision;
            Chofer = lastRfid != null ? lastRfid.Chofer : string.Empty;
            UltimoLogin = lastRfid != null && lastRfid.Fecha.HasValue ? lastRfid.Fecha.Value : (DateTime?) null;
            CentroDeCosto = lastPosition != null ? lastPosition.CentroDeCosto : string.Empty;
        }

        #endregion

        #region Public Properties

        public string Interno { get; set; }
        public DateTime? Fecha { get; set; }
        public Int32 IdDispositivo { get; set; }
        public Int32 IdDispositivoCoche { get; set; }
        public DateTime? FechaAlta { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Responsable { get; set; }
        public Int32 IdMovil { get; set; }
        public Int32 IdBase { get; set; }
        public Int32 Velocidad { get; set; }
        public Int32 IdPosicion { get; set; }
        //public string Firmware { get; set; }
        public string EstadoMovil { get; set; }
        public string EstadoDispositivo { get; set; }
        public string ReferenciaVehiculo { get; set; }
        public string TipoDispositivo { get; set; }
        public string Patente { get; set; }

        // -1: Sin Reportar - 0: Reportando - 1: Activo - 2: Inactivo
        public Int32 EstadoReporte { get; set; }

        public string Distrito { get; set;}
        public string Base { get; set; }
        public string Transportista { get; set; }
        public string CentroDeCosto { get; set; }
        public string Dispositivo { get; set; }
        public string TipoVehiculo { get; set; }
        public Int32 IdDistrito { get; set; }
        //public string Qtree { get; set; }
        public string Chofer { get; set; }
        public DateTime? UltimoLogin { get; set; }

        private string _esquinaCercana;
        public string EsquinaCercana
        {
            get
            {
                return _esquinaCercana ?? (_esquinaCercana = (Latitud != 0 && Longitud != 0) ? GeocoderHelper.GetDescripcionEsquinaMasCercana(Latitud, Longitud) : String.Empty);
            }
        }

        public TimeSpan TiempoDesdeUltimoLogin
        {
            get
            {
                if (!Fecha.HasValue || !UltimoLogin.HasValue) return TimeSpan.Zero;

                return Fecha.Value.Subtract(UltimoLogin.Value);
            }
        }

        #endregion

        #region Private Methods

        public static string GetDistrito(IHasEmpresa movil)
        {
            if (movil == null) return string.Empty;
            return movil.Empresa != null ? movil.Empresa.RazonSocial : string.Empty;
            
        }
        public static string GetBase(IHasLinea movil)
        {
            if (movil == null) return string.Empty;
            return movil.Linea != null ? movil.Linea.Descripcion : string.Empty;
        }
        public static string GetTransportista(IHasTransportista movil)
        {
            if (movil == null) return string.Empty;
            return movil.Transportista != null ? movil.Transportista.Descripcion : string.Empty;
        }

        /// <summary>
        /// Gets the device type description
        /// </summary>
        /// <param name="dispositivo"></param>
        /// <returns></returns>
        private static string GetDeviceTypeDescription(Dispositivo dispositivo) { return string.Concat(dispositivo.TipoDispositivo.Fabricante, " - ", dispositivo.TipoDispositivo.Modelo); }

        #endregion
    }
}
