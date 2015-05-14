using System;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class ControlDistribucionVo
    {
        public const int IndexVehiculo = 0;
        public const int IndexTipoVehiculo = 1;
        public const int IndexCodigo = 2;
        public const int IndexInicio = 3;
        public const int IndexParadas = 4;
        public const int IndexKmControlado = 5;

        public int Id { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Labels", VariableName = "TYPE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Tipo { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", IsAggregate = true, AggregateType = GridAggregateType.Count, AllowGroup = false)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "DATE", DataFormatString = "{0:d}")]
        public DateTime Inicio { get; set; }

        [GridMapping(Index = IndexParadas, ResourceName = "Labels", VariableName = "ENTREGAS", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, DataFormatString = "{0:0}")]
        public int Paradas { get; set; }

        [GridMapping(Index = IndexKmControlado, ResourceName = "Labels", VariableName = "KmControlado", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, DataFormatString = "{0:0.00} km")]
        public double KmControlado { get; set; }

        public ControlDistribucionVo(ViajeDistribucionVo viaje)
        {
            Id = viaje.Id;
            Vehiculo = viaje.Vehiculo;
            Tipo = viaje.Tipo;
            Codigo = viaje.Codigo;
            Inicio = viaje.Inicio;
            Paradas = viaje.Paradas;
            KmControlado = viaje.KmControlado;
        }
    }
}
