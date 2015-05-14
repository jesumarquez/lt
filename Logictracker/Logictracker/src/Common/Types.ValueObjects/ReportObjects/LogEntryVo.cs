using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.DatabaseTracer.ValueObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class LogEntryVo
    {
        public const int IndexDateTime = 0;
        public const int IndexType = 1;
        public const int IndexModule = 2;
        public const int IndexComponent = 3;
        public const int IndexMessage = 4;
        public const int IndexIdVehiculo = 5;
        public const int IndexIdDispositivo = 6;

        [GridMapping(Index = IndexDateTime, ResourceName = "Labels", VariableName = "FECHA", AllowGroup = false)]
        public DateTime DateTime { get; set; }

        [GridMapping(Index = IndexType, ResourceName = "Labels", VariableName = "TYPE")]
        public string Type { get; set; }

        [GridMapping(Index = IndexModule, ResourceName = "Labels", VariableName = "MODULO")]
        public string Module { get; set; }

        [GridMapping(Index = IndexComponent, ResourceName = "Labels", VariableName = "COMPONENTE", IncludeInSearch = true)]
        public string Component { get; set; }

        [GridMapping(Index = IndexMessage, ResourceName = "Labels", VariableName = "MENSAJE", IncludeInSearch = true)]
        public string Message { get; set; }

        [GridMapping(Index = IndexIdVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IncludeInSearch = true)]
        public string IdVehiculo { get; set; }

        [GridMapping(Index = IndexIdDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", IncludeInSearch = true)]
        public string IdDispositivo { get; set; }
      
        [GridMapping(IsDataKey = true)]
        public int Id { get; set; }

        public LogEntryVo(LogEntry logEntry)
        {
            Id = logEntry.Id;
            DateTime = logEntry.DateTime;
            Type = Enum.GetName(typeof(LogTypes), logEntry.Type);
            Module = logEntry.Module;
            Component = logEntry.Component;
            Message = logEntry.Message;
            IdVehiculo = logEntry.Vehicle.HasValue ? logEntry.Vehicle.Value.ToString() : "Ninguno";
            IdDispositivo = logEntry.Device.HasValue ? logEntry.Device.Value.ToString() : "Ninguno";
        }
    }
}
