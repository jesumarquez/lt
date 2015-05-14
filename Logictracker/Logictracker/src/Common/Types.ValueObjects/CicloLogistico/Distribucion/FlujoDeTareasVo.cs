using System;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class FlujoDeTareasVo
    {
        public const int IndexVehiculo = 0;
        public const int IndexDistribucion = 1;
        public const int IndexFlujo = 2;
        public const int IndexInicio = 3;
        public const int IndexFin = 4;
        public const int IndexTraslados = 5;
        public const int IndexKm = 6;
        public const int IndexInactividad = 7;
        public const int IndexTarea = 8;
        public const int IndexTotal = 9;

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 0, IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexDistribucion, ResourceName = "Labels", VariableName = "VIAJE", AllowGroup = true, IsInitialGroup = true, InitialGroupIndex = 1, IncludeInSearch = true)]
        public string Distribucion { get; set; }

        [GridMapping(Index = IndexFlujo, ResourceName = "Labels", VariableName = "FLUJO", AllowGroup = false)]
        public string Flujo { get; set; }

        [GridMapping(Index = IndexInicio, ResourceName = "Labels", VariableName = "DESDE", DataFormatString = "{0:HH:mm}", InitialSortExpression = true, AllowGroup = false)]
        public DateTime? Inicio { get; set; }

        [GridMapping(Index = IndexFin, ResourceName = "Labels", VariableName = "HASTA", DataFormatString = "{0:HH:mm}", AllowGroup = false)]
        public DateTime? Fin { get; set; }

        [GridMapping(Index = IndexTraslados, ResourceName = "Labels", VariableName = "TRASLADOS", DataFormatString = "{0:HH:mm}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:HH:mm}", AllowGroup = false)]
        public TimeSpan Traslados { get; set; }

        [GridMapping(Index = IndexKm, ResourceName = "Labels", VariableName = "KM", DataFormatString = "{0:0.00}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}", AllowGroup = false)]
        public double Km { get; set; }

        [GridMapping(Index = IndexInactividad, ResourceName = "Labels", VariableName = "INACTIVIDAD", DataFormatString = "{0:HH:mm}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:HH:mm}", AllowGroup = false)]
        public TimeSpan Inactividad { get; set; }

        [GridMapping(Index = IndexTarea, ResourceName = "Labels", VariableName = "TAREA", DataFormatString = "{0:HH:mm}", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:HH:mm}", AllowGroup = false)]
        public TimeSpan Tarea { get; set; }

        [GridMapping(Index = IndexTotal, ResourceName = "Labels", VariableName = "TOTAL", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:HH:mm}", AllowGroup = false)]
        public TimeSpan Total
        {
            get { return Inactividad.Add(Tarea).Add(Traslados); }
        }
    }
}
