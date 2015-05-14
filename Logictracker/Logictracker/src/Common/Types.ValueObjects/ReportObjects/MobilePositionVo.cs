using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobilePositionVo
    {
        public const int KeyIndexIdDispositivo = 0;
        public const int KeyIndexIdMovil = 1;

        public const int IndexEstadoReporte = 0;
        public const int IndexDispositivo = 1;
        public const int IndexTipoDispositivo = 2;
        public const int IndexFirmware = 3;
        public const int IndexQtree = 4;
        public const int IndexEstadoDispositivo = 5;
        public const int IndexInterno = 6;
        public const int IndexEmpresa = 7;
        public const int IndexLinea = 8;
        public const int IndexFecha = 9;
        public const int IndexVelocidad = 10;
        public const int IndexEsquinaCercana = 11;

        [GridMapping(Index = IndexEstadoReporte, ResourceName = "Labels", VariableName = "GROUP_ESTADO", IsInitialGroup = true)]
        public int EstadoReporte { get; set; }

        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "{0} Dispositivos", IncludeInSearch = true)]
        public string Dispositivo { get; set; }

        [GridMapping(Index = IndexTipoDispositivo, ResourceName = "Entities", VariableName = "PARENTI32", IncludeInSearch = true)]
        public string TipoDispositivo { get; set; }

        [GridMapping(Index = IndexFirmware, ResourceName = "Labels", VariableName = "FIRMWARE", IncludeInSearch = true)]
        public string Firmware { get; set; }

        [GridMapping(Index = IndexQtree, ResourceName = "Labels", VariableName = "QTREE", AllowGroup = false, IncludeInSearch = true)]
        public string Qtree { get; set; }

        [GridMapping(Index = IndexEstadoDispositivo, ResourceName = "Labels", VariableName = "ESTADO_DISPOSITIVO", AllowGroup = false)]
        public string EstadoDispositivo { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, IncludeInSearch = true)]
        public string Interno { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01")]
        public string Distrito { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02")]
        public string Base { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime? Fecha { get; set; }

        [GridMapping(Index = IndexVelocidad, ResourceName = "Labels", VariableName = "VELOCIDAD", AllowGroup = false)]
        public string Velocidad { get; set; }

        [GridMapping(Index = IndexEsquinaCercana, ResourceName = "Labels", VariableName = "ESQUINA", AllowGroup = false)]
        public string EsquinaCercana { get { return mobilePosition.EsquinaCercana; } }

        [GridMapping(IsDataKey = true)]
        public int IdDispositivo { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdMovil { get; set; }

        private MobilePosition mobilePosition;

        public MobilePositionVo(MobilePosition mobilePosition)
        {
            var dao = new DAOFactory();
            this.mobilePosition = mobilePosition;
            Interno = mobilePosition.Interno;
            EstadoReporte = mobilePosition.EstadoReporte;
            TipoDispositivo = mobilePosition.TipoDispositivo;
            Firmware = dao.DetalleDispositivoDAO.GetFullFirmwareVersionValue(dao.DispositivoDAO.FindById(mobilePosition.IdDispositivo));
            Qtree = dao.DetalleDispositivoDAO.GetQtreeRevisionNumberValue(mobilePosition.IdDispositivo);
            EstadoDispositivo = mobilePosition.EstadoDispositivo;
            Distrito = mobilePosition.Distrito;
            Base = mobilePosition.Base;
            Fecha = mobilePosition.Fecha;
            Velocidad = mobilePosition.Velocidad.Equals(-1) ? string.Empty : mobilePosition.Velocidad.ToString("#0");
            IdDispositivo = mobilePosition.IdDispositivo;
            Dispositivo = mobilePosition.Dispositivo;
            IdMovil = mobilePosition.IdMovil;
        }
    }
}
