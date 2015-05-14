using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileUtilizationVo
    {
        public const int KeyIndex = 0;

        public const int IndexCentro = 0;
        public const int IndexInterno = 1;
        public const int IndexHsTurno = 2;
        public const int IndexHsEsperadas = 3;
        public const int IndexHsTurnoReales = 4;
        public const int IndexPorcentajeTurno = 5;
        public const int IndexHsFueraTurno = 6;
        public const int IndexHsRealesFueraTurno = 7;
        public const int IndexPorcentajeFueraTurno = 8;
        public const int IndexPorcentajeProd = 9;
        public const int IndexMovFueraTurno = 10;
        public const int IndexPorcentajeTotal = 11;

        [GridMapping(Index = IndexCentro, ResourceName = "Entities", VariableName = "PARENTI37")]
        public string Centro { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Labels", VariableName = "INTERNO")]
        public string Interno { get; set; }

        [GridMapping(Index = IndexHsTurno, ResourceName = "Labels", VariableName = "TOTAL_TURNO", DataFormatString = "{0:2} hs", AllowGroup = false)]
        public double HsTurno { get; set; }

        [GridMapping(Index = IndexHsEsperadas, ResourceName = "Labels", VariableName = "ESPERADO_TURNO", DataFormatString = "{0:2} hs", AllowGroup = false)]
        public double HsEsperadas { get; set; }

        [GridMapping(Index = IndexHsTurnoReales, ResourceName = "Labels", VariableName = "REAL_TURNO", DataFormatString = "{0:2} hs", AllowGroup = false)]
        public double HsTurnoReales { get; set; }

        [GridMapping(Index = IndexPorcentajeTurno, ResourceName = "Labels", VariableName = "PORCENTAJE_TURNO", AllowGroup = false)]
        public double PorcentajeTurno { get; set; }

        [GridMapping(Index = IndexHsFueraTurno, ResourceName = "Labels", VariableName = "TOTAL_FUERA_TURNO", DataFormatString = "{0:2} hs", AllowGroup = false)]
        public double HsFueraTurno { get; set; }

        [GridMapping(Index = IndexHsRealesFueraTurno, ResourceName = "Labels", VariableName = "REAL_FUERA_TURNO", DataFormatString = "{0:2} hs", AllowGroup = false)]
        public double HsRealesFueraTurno { get; set; }

        [GridMapping(Index = IndexPorcentajeFueraTurno, ResourceName = "Labels", VariableName = "PORCENTAJE_FUERA_TURNO", AllowGroup = false)]
        public double PorcentajeFueraTurno { get; set; }

        [GridMapping(Index = IndexPorcentajeProd, ResourceName = "Labels", VariableName = "PRODUCTIVIDAD", AllowGroup = false)]
        public double PorcentajeProd { get; set; }

        [GridMapping(Index = IndexMovFueraTurno, ResourceName = "Labels", VariableName = "MOV_SIN_TURNO", DataFormatString = "{0:2} hs", AllowGroup = false)]
        public double MovFueraTurno { get; set; }

        [GridMapping(Index = IndexPorcentajeTotal, ResourceName = "Labels", VariableName = "TOTAL", AllowGroup = false)]
        public double PorcentajeTotal { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdVehiculo { get; set; }

        public MobileUtilizationVo(MobileUtilization mobileUtilization)
        {
            Centro = mobileUtilization.Centro;
            Interno = mobileUtilization.Interno;
            HsTurno = mobileUtilization.HsTurno;
            HsEsperadas = mobileUtilization.HsEsperadas;
            HsTurnoReales = mobileUtilization.HsTurnoReales;
            PorcentajeTurno = mobileUtilization.PorcentajeTurno;
            HsFueraTurno = mobileUtilization.HsFueraTurno;
            HsRealesFueraTurno = mobileUtilization.HsRealesFueraTurno;
            PorcentajeFueraTurno = mobileUtilization.PorcentajeFueraTurno;
            PorcentajeProd = mobileUtilization.PorcentajeProd;
            MovFueraTurno = mobileUtilization.PorcentajeFueraTurno;
            PorcentajeTotal = mobileUtilization.PorcentajeTotal;
            IdVehiculo = mobileUtilization.IdVehiculo;
        }
    }
}
