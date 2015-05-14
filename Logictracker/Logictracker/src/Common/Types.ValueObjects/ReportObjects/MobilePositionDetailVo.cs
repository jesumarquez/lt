using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobilePositionDetailVo
    {
        public const int KeyIndexIdMovil = 0;
        public const int KeyIndexIdDistrito = 1;
        public const int KeyIndexIdBase = 2;
        public const int KeyIndexIdPosicion = 3;
        public const int KeyIndexIdDispositivo = 4;

        public const int IndexFechaAlta = 0;
        public const int IndexFecha = 1;
        public const int IndexVelocidad = 2;
        public const int IndexEsquinaCercana = 3;
        public const int IndexInterno = 4;
        public const int IndexTipoVehiculo = 5;
        public const int IndexEstadoMovil = 6;
        public const int IndexDistrito = 7;
        public const int IndexBase = 8;
        public const int IndexReferenciaVehiculo = 9;
        public const int IndexResponsable = 10;
        public const int IndexChofer = 11;
        public const int IndexUltimoLogin = 12;
        public const int IndexTiempoAUltimoLogin = 13;
        public const int IndexDispositivo = 14;
        public const int IndexTipoDispositivo = 15;
        public const int IndexFirmware = 16;
        public const int IndexQtree = 17;
        public const int IndexEstadoDispositivo = 18;

        [GridMapping(Index = IndexFechaAlta, ResourceName = "Labels", VariableName = "FECHA_ALTA", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime? FechaAlta { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime? Fecha { get; set; }

        [GridMapping(Index = IndexVelocidad, ResourceName = "Labels", VariableName = "VELOCIDAD", AllowGroup = false)]
        public string Velocidad { get; set; }

        [GridMapping(Index = IndexEsquinaCercana, ResourceName = "Labels", VariableName = "ESQUINA", AllowGroup = false)]
        public string EsquinaCercana { get { return mobilePosition.EsquinaCercana; } }

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false)]
        public string Interno { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = false)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexEstadoMovil, ResourceName = "Labels", VariableName = "ESTADO_VEHICULO", AllowGroup = false)]
        public string EstadoMovil { get; set; }

        [GridMapping(Index = IndexDistrito, ResourceName = "Entities", VariableName = "PARENTI01", AllowGroup = false)]
        public string Distrito { get; set; }

        [GridMapping(Index = IndexBase, ResourceName = "Entities", VariableName = "PARENTI02", AllowGroup = false)]
        public string Base { get; set; }

        [GridMapping(Index = IndexReferenciaVehiculo, ResourceName = "Labels", VariableName = "REFFERENCE", AllowGroup = false)]
        public string ReferenciaVehiculo { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE", AllowGroup = false)]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexChofer, ResourceName = "Labels", VariableName = "ULTIMO_CHOFER", AllowGroup = false)]
        public string Chofer { get; set; }

        [GridMapping(Index = IndexUltimoLogin, ResourceName = "Labels", VariableName = "ULTIMO_CHOFER", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime? UltimoLogin { get; set; }

        [GridMapping(Index = IndexTiempoAUltimoLogin, ResourceName = "Labels", VariableName = "TIEMPO_A_ULTIMO_LOGIN", AllowGroup = false)]
        public string TiempoAUltimoLogin { get; set; }

        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", AllowGroup = false)]
        public string Dispositivo { get; set; }

        [GridMapping(Index = IndexTipoDispositivo, ResourceName = "Entities", VariableName = "PARENTI32", AllowGroup = false)]
        public string TipoDispositivo { get; set; }

        [GridMapping(Index = IndexFirmware, ResourceName = "Labels", VariableName = "FIRMWARE", AllowGroup = false)]
        public string Firmware { get; set; }

        [GridMapping(Index = IndexQtree, ResourceName = "Labels", VariableName = "QTREE", AllowGroup = false)]
        public string Qtree { get; set; }

        [GridMapping(Index = IndexEstadoDispositivo, ResourceName = "Labels", VariableName = "ESTADO_DISPOSITIVO", AllowGroup = false)]
        public string EstadoDispositivo { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdMovil { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdDistrito { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdBase { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdPosicion { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdDispositivo { get; set; }

        public TimeSpan TiempoDesdeUltimoLogin { get; set; }

        private MobilePosition mobilePosition;

        public MobilePositionDetailVo(MobilePosition mobilePosition)
        {
            var dao = new DAOFactory();
            this.mobilePosition = mobilePosition;
            FechaAlta = mobilePosition.FechaAlta;
            Fecha = mobilePosition.Fecha;
            Velocidad = mobilePosition.Velocidad.Equals(-1) ? string.Empty : mobilePosition.Velocidad.ToString("#0");
            Interno = mobilePosition.Interno;
            TipoVehiculo = mobilePosition.TipoVehiculo;
            EstadoMovil = mobilePosition.EstadoMovil;
            Distrito = mobilePosition.Distrito;
            Base = mobilePosition.Base;
            ReferenciaVehiculo = mobilePosition.ReferenciaVehiculo;
            Responsable = mobilePosition.Responsable;
            Chofer = mobilePosition.Chofer;
            UltimoLogin = mobilePosition.UltimoLogin;
            TiempoDesdeUltimoLogin = mobilePosition.TiempoDesdeUltimoLogin;
            Dispositivo = mobilePosition.Dispositivo;
            TipoDispositivo = mobilePosition.TipoDispositivo;
            Firmware = dao.DetalleDispositivoDAO.GetFullFirmwareVersionValue(dao.DispositivoDAO.FindById((mobilePosition.IdDispositivo)));
            Qtree = dao.DetalleDispositivoDAO.GetQtreeRevisionNumberValue(mobilePosition.IdDispositivo);
            EstadoDispositivo = mobilePosition.EstadoDispositivo;
            IdMovil = mobilePosition.IdMovil;
            IdDistrito = mobilePosition.IdDistrito;
            IdBase = mobilePosition.IdBase;
            IdPosicion = mobilePosition.IdPosicion;
            IdDispositivo = mobilePosition.IdDispositivo;
        }
    }
}
