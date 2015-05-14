using System;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ValueObject.Positions;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class OdometroStatusVo
    {
        public const int IndexOdometro = 0;
        public const int IndexInterno = 1;
        public const int IndexTipo = 2;
        public const int IndexPatente = 3;
        public const int IndexCentroDeCosto = 4;
        public const int IndexResponsable = 5;
        public const int IndexReferencia = 6;
        public const int IndexKilometrosReferencia = 7;
        public const int IndexTiempoReferencia = 8;
        public const int IndexHorasReferencia = 9;
        public const int IndexKilometrosFaltantes = 10;
        public const int IndexTiempoFaltante = 11;
        public const int IndexHorasFaltantes = 12;
        public const int IndexKmTotales = 13;
        public const int IndexUltimoUpdate = 14;

        [GridMapping(Index = IndexOdometro, ResourceName = "Entities", VariableName = "PARENTI40", IsInitialGroup = true)]
        public string Odometro { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Labels", VariableName = "INTERNO")]
        public string Interno { get; set; }

        [GridMapping(Index = IndexTipo, ResourceName = "Entities", VariableName = "PARENTI17")]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE")]
        public string Patente { get; set; }

        [GridMapping(Index = IndexCentroDeCosto, ResourceName = "Entities", VariableName = "PARENTI37")]
        public string CentroDeCosto { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE")]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexReferencia, ResourceName = "Labels", VariableName = "REFERENCIA")]
        public string Referencia { get; set; }

        [GridMapping(Index = IndexKilometrosReferencia, ResourceName = "Labels", VariableName = "KM_REF", DataFormatString = "{0:0.00}", AllowGroup = false)]
        public double? KilometrosReferencia { get; set; }

        [GridMapping(Index = IndexTiempoReferencia, ResourceName = "Labels", VariableName = "T_REF", AllowGroup = false, DataFormatString = "{0:0}")]
        public int? TiempoReferencia { get; set; }

        [GridMapping(Index = IndexHorasReferencia, ResourceName = "Labels", VariableName = "HS_REF", AllowGroup = false, DataFormatString = "{0:0.00}")]
        public double? HorasReferencia { get; set; }

        [GridMapping(Index = IndexKilometrosFaltantes, ResourceName = "Labels", VariableName = "KM_FALT", AllowGroup = false, DataFormatString = "{0:0.00}")]
        public double? KilometrosFaltantes { get; set; }

        [GridMapping(Index = IndexTiempoFaltante, ResourceName = "Labels", VariableName = "T_FALT", AllowGroup = false, DataFormatString = "{0:0}")]
        public int? TiempoFaltante { get; set; }

        [GridMapping(Index = IndexHorasFaltantes, ResourceName = "Labels", VariableName = "HS_FALT", AllowGroup = false, DataFormatString = "{0:0.00}")]
        public double? HorasFaltantes { get; set; }

        [GridMapping(Index = IndexKmTotales, ResourceName = "Labels", VariableName = "KM_TOTALES", AllowGroup = false, DataFormatString = "{0:0.00}")]
        public double? KmTotales { get; set; }

        [GridMapping(Index = IndexUltimoUpdate, ResourceName = "Labels", VariableName = "ULTIMO_UPDATE", AllowGroup = false)]
        public DateTime UltimoUpdate { get; set; }

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public OdometroStatusVo(OdometroStatus odometroStatus)
        {
            Odometro = odometroStatus.Odometro;
            Interno = odometroStatus.Interno;
            Tipo = odometroStatus.Tipo;
            Patente = odometroStatus.Patente;
            CentroDeCosto = odometroStatus.CentroDeCosto;
            Responsable = odometroStatus.Responsable;
            Referencia = odometroStatus.Referencia;
            KilometrosReferencia = odometroStatus.KilometrosReferencia;
            TiempoReferencia = odometroStatus.TiempoReferencia;
            HorasReferencia = odometroStatus.HorasReferencia;
            KilometrosFaltantes = odometroStatus.KilometrosFaltantes;
            TiempoFaltante = odometroStatus.TiempoFaltante;
            HorasFaltantes = odometroStatus.HorasFaltantes;
            KmTotales = null;
            UltimoUpdate = odometroStatus.UltimoUpdate;
            Red = odometroStatus.Red;
            Green = odometroStatus.Green;
            Blue = odometroStatus.Blue;
        }

        public OdometroStatusVo(Coche coche)
        {
            Odometro = CultureManager.GetLabel("KM_TOTALES");
            Interno = coche.Interno;
            Tipo = coche.TipoCoche.Descripcion;
            Patente = coche.Patente;
            CentroDeCosto = coche.CentroDeCostos != null ? coche.CentroDeCostos.Descripcion : string.Empty;
            Responsable = coche.Chofer != null && coche.Chofer.Entidad != null
                              ? coche.Chofer.Entidad.Descripcion
                              : string.Empty;
            Referencia = coche.Referencia;
            KilometrosReferencia = null;
            TiempoReferencia = null;
            HorasReferencia = null;
            KilometrosFaltantes = null;
            TiempoFaltante = null;
            HorasFaltantes = null;
            KmTotales = Convert.ToDouble((coche.InitialOdometer + coche.ApplicationOdometer).ToString("#0.00"));
            LogUltimaPosicionVo lastPos;
            UltimoUpdate = coche.LastOdometerUpdate.HasValue
                               ? coche.LastOdometerUpdate.Value.ToDisplayDateTime()
                               : (lastPos = coche.RetrieveLastPosition()) != null ? lastPos.FechaMensaje.ToDisplayDateTime() : DateTime.MinValue;
            Red = 255;
            Green = 255;
            Blue = 255;
        }
    }
}
