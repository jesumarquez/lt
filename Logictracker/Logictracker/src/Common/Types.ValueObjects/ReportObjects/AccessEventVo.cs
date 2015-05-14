using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class AccessEventVo
    {
        public const int IndexFecha = 0;
        public const int IndexEmpleado = 1;
        public const int IndexEvento = 2;
        public const int IndexPuerta = 3;

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexEmpleado, ResourceName = "Entities", VariableName = "PARENTI09", IncludeInSearch = true)]
        public string Empleado { get; set; }

        [GridMapping(Index = IndexEvento, ResourceName = "Labels", VariableName = "EVENTO")]
        public string Evento { get; set; }

        [GridMapping(Index = IndexPuerta, ResourceName = "Labels", VariableName = "PUERTA")]
        public string Puerta { get; set; }

        public bool Entrada { get; set; }
      
        [GridMapping(IsDataKey = true)]
        public int Id { get; set; }

        public AccessEventVo(EventoAcceso accessEvent)
        {
            Id = accessEvent.Id;
            Fecha = accessEvent.Fecha.ToDisplayDateTime();
            Empleado = accessEvent.Empleado != null ? accessEvent.Empleado.Entidad != null ? accessEvent.Empleado.Entidad.Descripcion : "Tarjeta no Asignada" : "Tarjeta no Asignada";
            Puerta = accessEvent.Puerta.Descripcion;
            Evento = accessEvent.Entrada ? "Entrada" : "Salida";
            Entrada = accessEvent.Entrada;
        }
    }
}
