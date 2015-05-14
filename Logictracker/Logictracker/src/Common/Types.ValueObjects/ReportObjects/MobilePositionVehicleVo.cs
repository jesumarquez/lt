using System;
using System.Globalization;
using Logictracker.Cache;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobilePositionVehicleVo
    {
        public const int KeyIndexIdMovil = 0;
        public const int KeyIndexIdDispositivo = 1;

        public const int IndexEstadoReporte = 0;
        public const int IndexVehiculoId = 1;
        public const int IndexVehiculo = 2;
        public const int IndexEstadoMovil = 3;
        public const int IndexUbicacion = 4;
        public const int IndexCcReferenciaResponsableVehiculo = 5;
        public const int IndexUltimoChoferLogin = 6;
        public const int IndexDispositivo = 7;
        public const int IndexFecha = 8;
        public const int IndexVelocidad = 9;
        public const int IndexEsquinaCercana = 10;
        public const int IndexTiempoAUltimoLogin = 11;
        public const int IndexLastPacketReceivedAt = 12;
        public const int IndexGarminStatus = 13;
        public const int IndexEstadoStr = 14;
        private readonly MobilePosition _mobilePosition;

        [GridMapping(IsDataKey = true)]
        public int IdMovil { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdDispositivo { get; set; }

        [GridMapping(Index = IndexEstadoReporte, ResourceName = "Labels", VariableName = "GROUP_ESTADO", IsInitialGroup = true)]
        public int EstadoReporte { get; set; }

        [GridMapping(Index = IndexVehiculoId, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "{0} Vehículos", IncludeInSearch = true)]
        public string VehiculoId { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true, IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexEstadoMovil, ResourceName = "Labels", VariableName = "ESTADO_VEHICULO", AllowGroup = true)]
        public string EstadoMovil { get; set; }

        [GridMapping(Index = IndexUbicacion, ResourceName = "Entities", VariableName = "PARENTI02")]
        public string Ubicacion { get; set; }

        [GridMapping(Index = IndexCcReferenciaResponsableVehiculo, ResourceName = "Labels", VariableName = "REFFERENCE", AllowGroup = false, IncludeInSearch = true)]
        public string CcReferenciaResponsableVehiculo { get; set; }

        [GridMapping(Index = IndexUltimoChoferLogin, ResourceName = "Labels", VariableName = "ULTIMO_LOGIN", AllowGroup = false, IncludeInSearch = true)]
        public string UltimoChoferLogin { get; set; }

        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", AllowGroup = false, IncludeInSearch = true)]
        public string Dispositivo { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime? Fecha { get; set; }

        [GridMapping(Index = IndexVelocidad, ResourceName = "Labels", VariableName = "VELOCIDAD_KM", AllowGroup = false)]
        public string Velocidad { get; set; }

        [GridMapping(Index = IndexEsquinaCercana, ResourceName = "Labels", VariableName = "ESQUINA", IsTemplate = true, AllowGroup = false)]
        public string EsquinaCercana { get { return _mobilePosition.EsquinaCercana; } }

        [GridMapping(Index = IndexTiempoAUltimoLogin, ResourceName = "Labels", VariableName = "TIEMPO_A_ULTIMO_LOGIN", AllowGroup = false)]
        public string TiempoAUltimoLogin { get; set; }

        [GridMapping(Index = IndexLastPacketReceivedAt, ResourceName = "Labels", VariableName = "LASTPACKET_RECEIVEDAT", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime? LastPacketReceivedAt { get; set; }

        [GridMapping(Index = IndexGarminStatus, ResourceName = "Labels", VariableName = "GARMIN_CONNECTED", AllowGroup = false)]
        public string GarminStatus { get; set; }

        [GridMapping(Index = IndexEstadoStr, ResourceName = "Labels", VariableName = "ESTADO_STR")]
        public string EstadoStr { get; set; }

        private static string MakeLastPacketReceivedAtKey(int deviceId)
        {
            return "device_" + deviceId + "_lastPacketReceivedAt";
        }

        public DateTime? FechaLastPacketReceivedAt
        {
            get
            {
                // Get from cache the DateTime where the Last Packet was received on the device assigned to the vehicle
                var key = MakeLastPacketReceivedAtKey(IdDispositivo);
                var dt = LogicCache.Retrieve<string>(key);
                if (string.IsNullOrEmpty(dt))
                    return null;
                
                try
                {
                    return DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                } 
                catch (Exception)
                {
                    return null;
                }
            }
        }
        
        public int IdDispositivoCoche { get; set; }

        public TimeSpan TiempoDesdeUltimoLogin { get; set; }

        public MobilePositionVehicleVo(MobilePosition mobilePosition)
        {
            _mobilePosition = mobilePosition;
            IdMovil = mobilePosition.IdMovil;
            IdDispositivo = mobilePosition.IdDispositivo;
            IdDispositivoCoche = mobilePosition.IdDispositivoCoche;
            EstadoReporte = mobilePosition.EstadoReporte;
            EstadoStr = GetReportStatusDescription(EstadoReporte);

            var interno = mobilePosition.Interno;
            var patente = mobilePosition.Patente;
            VehiculoId = (!string.IsNullOrEmpty(interno) ? interno + "|" : string.Empty) +
                         (!string.IsNullOrEmpty(patente) ? patente : string.Empty);
            
            var coche = new DAOFactory().CocheDAO.FindById(mobilePosition.IdMovil);
            var cocheModelo = coche.Modelo != null ? coche.Modelo.Descripcion : string.Empty;
            var cocheAno = coche.AnioPatente.ToString("#0");            
            var tipoVehiculo = mobilePosition.TipoVehiculo;

            Vehiculo = (!string.IsNullOrEmpty(tipoVehiculo) ? tipoVehiculo + (!string.IsNullOrEmpty(cocheModelo) || !string.IsNullOrEmpty(cocheAno) ? "|" : string.Empty) : string.Empty);
            if (!string.IsNullOrEmpty(cocheModelo))
                Vehiculo += cocheModelo + " (" + cocheAno + ")";
            EstadoMovil = mobilePosition.EstadoMovil;

            var ubicacionDistrito = mobilePosition.Distrito;
            var ubicacionBase = mobilePosition.Base;
            var ubicacionTransportista = mobilePosition.Transportista;
            Ubicacion = (!string.IsNullOrEmpty(ubicacionDistrito) ? ubicacionDistrito : string.Empty);
            if (!string.IsNullOrEmpty(ubicacionBase))
                Ubicacion += (!string.IsNullOrEmpty(ubicacionDistrito) ? "|" : string.Empty) + ubicacionBase;
            if (!string.IsNullOrEmpty(ubicacionTransportista))
                Ubicacion += (!string.IsNullOrEmpty(ubicacionTransportista) ? "|" : string.Empty) + ubicacionTransportista;

            CcReferenciaResponsableVehiculo = mobilePosition.CentroDeCosto + "|" + mobilePosition.ReferenciaVehiculo + "|" + mobilePosition.Responsable;

            var chofer = mobilePosition.Chofer;
            var ultimoLogin = (string) null;
            if (mobilePosition.UltimoLogin != null)
                ultimoLogin = (((DateTime) mobilePosition.UltimoLogin).ToDisplayDateTime().ToString());

            var ultimoChoferLogin = (!string.IsNullOrEmpty(chofer) ? chofer : string.Empty);

            UltimoChoferLogin = ultimoChoferLogin + (!string.IsNullOrEmpty(ultimoLogin) ? "|" : string.Empty) + ultimoLogin;

            Dispositivo = mobilePosition.Dispositivo;
            Fecha = mobilePosition.Fecha;
            Velocidad = mobilePosition.Velocidad.Equals(-1) ? string.Empty : mobilePosition.Velocidad.ToString("#0");
            TiempoDesdeUltimoLogin = mobilePosition.TiempoDesdeUltimoLogin;
            TiempoAUltimoLogin = TiempoDesdeUltimoLogin.ToString();
            
            var dd = FechaLastPacketReceivedAt;
            LastPacketReceivedAt = dd.HasValue ? FechaLastPacketReceivedAt.Value.ToDisplayDateTime() : mobilePosition.Fecha;

            // Get from cache the Status of the Garmin device attached to the vehicle
            var garminStatus = GetCachedDeviceProperty(mobilePosition.IdDispositivo, "GarminConnected");
            GarminStatus = !string.IsNullOrEmpty(garminStatus) && garminStatus != "GARMINCONNECTED_NOTCONFIGURED"
                                ? CultureManager.GetLabel(garminStatus) : null;
        }

        private static string GetCachedDeviceProperty(int idDispositivo, string propertyName)
        {
            var key = "device_" + Convert.ToString(idDispositivo) + "_" + propertyName;
            return LogicCache.Retrieve<string>(key);
        }

        public static string GetReportStatusDescription(int estadoReporte)
        {
            switch (estadoReporte)
            {
                case 0:
                    return CultureManager.GetLabel("REPORTANDO");
                case 1:
                    return CultureManager.GetLabel("DESCARGANDO_POSICIONES");
                case 2:
                    return CultureManager.GetLabel("ACTIVO");
                case 3:
                    return CultureManager.GetLabel("INACTIVO");
            }

            return CultureManager.GetLabel("SIN_REPORTAR");
        }
    }
}