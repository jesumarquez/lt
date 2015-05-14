using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class ConsumosMotorVo
    {
        public const int KeyIndexIdMotor = 0;

        public const int IndexCentroDeCostos = 0;
        public const int IndexDescripcionMotor = 1;
        public const int IndexDifVolumen = 2;
        public const int IndexCaudalMinimo = 3;
        public const int IndexCaudalMaximo = 4;
        public const int IndexHsEnMarcha = 5;
        public const int IndexCantidadConsumos = 6;
        public const int IndexConsumoPromedio = 7;
        public const int IndexUltiMedicion = 8;


        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI19", AllowGroup = false, InitialSortExpression = true)]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexDescripcionMotor, ResourceName = "Entities", VariableName = "PARENTI39", AllowGroup = false)]
        public string DescripcionMotor { get; set; }

        [GridMapping(Index = IndexDifVolumen, ResourceName = "Labels", VariableName = "VOLUME_DIFFERENCE", DataFormatString = "{0:2}", AllowGroup = false)]
        public double DifVolumen { get; set; }

        [GridMapping(Index = IndexCaudalMinimo, ResourceName = "Labels", VariableName = "CAUDAL_MIN", AllowGroup = false)]
        public int CaudalMinimo { get; set; }

        [GridMapping(Index = IndexCaudalMaximo, ResourceName = "Labels", VariableName = "CAUDAL_MAX", AllowGroup = false)]
        public int CaudalMaximo { get; set; }

        [GridMapping(Index = IndexHsEnMarcha, ResourceName = "Labels", VariableName = "HS_EN_MARCHA", AllowGroup = false)]
        public double HsEnMarcha { get; set; }

        [GridMapping(Index = IndexCantidadConsumos, ResourceName = "Labels", VariableName = "MEDICIONES_TOMADAS", AllowGroup = false)]
        public int CantidadConsumos { get; set; }

        [GridMapping(Index = IndexConsumoPromedio, ResourceName = "Labels", VariableName = "CONSUMO_PROMEDIO", DataFormatString = "{0} l/h", AllowGroup = false)]
        public double ConsumoPromedio { get; set; }

        [GridMapping(Index = IndexUltiMedicion, ResourceName = "Labels", VariableName = "ULTIMA_MEDICION", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime UltiMedicion { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdMotor { get; set; }


        public ConsumosMotorVo(ConsumosMotor c)
        {
            CentroDeCostos = c.CentroDeCostos;
            DescripcionMotor = c.DescripcionMotor;
            DifVolumen = c.DifVolumen;
            CaudalMinimo = (int)c.CaudalMinimo;
            CaudalMaximo = (int)c.CaudalMaximo;
            HsEnMarcha = c.HsEnMarcha;
            CantidadConsumos = c.CantidadConsumos;
            UltiMedicion = c.UltiMedicion;
            IdMotor = c.IDMotor;
            ConsumoPromedio = c.HsEnMarcha != 0 ? c.DifVolumen/c.HsEnMarcha : 0; 
        }
    }
}
