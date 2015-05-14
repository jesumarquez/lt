using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class ConsumoVo
    {
        public const int IndexKeyIdCaudalimetro = 0;

        public const int IndexMotor = 0;
        public const int IndexFecha = 1;
        public const int IndexCentroDeCostos = 2;
        public const int IndexVolumen = 3;
        public const int IndexHsEnMarcha = 4;
        public const int IndexCaudal = 5;

        [GridMapping(Index = IndexMotor, ResourceName = "Entities", VariableName = "PARENTI39", AllowGroup = true, IsInitialGroup = true)]
        public string Motor { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false, DataFormatString = "{0:G}", InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI36", AllowGroup = false)]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexVolumen, ResourceName = "Labels", VariableName = "VOLUMEN", AllowGroup = false, DataFormatString = "{0:2} lit", IsAggregate = true, AggregateTextFormat = "Volumen Consumido: {0} lit")]
        public double Volumen { get; set; }

        [GridMapping(Index = IndexHsEnMarcha, ResourceName = "Labels", VariableName = "HS_EN_MARCHA", DataFormatString = "{0} hs", AllowGroup = false)]
        public double HsEnMarcha { get; set; }

        [GridMapping(Index = IndexCaudal, ResourceName = "Labels", VariableName = "CAUDAL", DataFormatString = "{0:2} lit/h", AllowGroup = false)]
        public double Caudal { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdCaudalimetro { get; set; }

        public ConsumoVo(Movimiento m)
        {
            IdCaudalimetro = m.Id;
            Motor = m.Caudalimetro != null ? m.Caudalimetro.Descripcion : string.Empty;
            Fecha = m.Fecha;
            CentroDeCostos = m.Tanque != null ? m.Tanque.Equipo.Descripcion : String.Empty;
            Volumen = m.Volumen;
            HsEnMarcha = m.HsEnMarcha;
            Caudal = m.Caudal;
        }
    }
}
