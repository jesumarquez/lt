using System;

namespace Logictracker.Types.ValueObjects.ReportObjects.Bolland
{
    [Serializable]
    public class ActividadDiariaVo
    {
        public const int IndexFecha = 0;
        public const int IndexHora = 1;
        public const int IndexEvento = 2;
        public const int IndexVelocidad = 3;
        public const int IndexKilometraje = 4;
        public const int IndexEmpleado = 5;

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = true, IsInitialGroup = true, AggregateTextFormat = "{0:dd/MM/yyyy}", DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexHora, ResourceName = "Labels", VariableName = "HORA", AllowGroup = false, InitialSortExpression = true)]
        public DateTime Hora { get; set; }

        [GridMapping(Index = IndexEvento, ResourceName = "Labels", VariableName = "EVENTO", IncludeInSearch = true)]
        public string Evento { get; set; }

        [GridMapping(Index = IndexVelocidad, ResourceName = "Labels", VariableName = "VELOCIDAD")]
        public int Velocidad { get; set; }

        [GridMapping(Index = IndexKilometraje, ResourceName = "Labels", VariableName = "KM")]
        public int Kilometraje { get; set; }

        [GridMapping(Index = IndexEmpleado, ResourceName = "Entities", VariableName = "PARENTI09", IncludeInSearch = true)]
        public string Empleado { get; set; }
     
        [GridMapping(IsDataKey = true)]
        public int Id { get; set; }
    }
}
