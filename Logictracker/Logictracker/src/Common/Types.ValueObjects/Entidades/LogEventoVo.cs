using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class LogEventoVo
    {   
        public const int IndexFecha = 0;
        public const int IndexMensaje = 1;
        public const int IndexTexto = 2;
        public const int IndexDispositivo = 3;
        public const int IndexEntidad = 4;
        public const int IndexSubEntidad = 5;
        public const int IndexSensor = 6;
        public const int IndexDynamicColumns = 7;
        
        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", AllowGroup = false, InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public string Fecha { get; set; }

        [GridMapping(Index = IndexMensaje, ResourceName = "Entities", VariableName = "PAREVEN01", AllowGroup = true)]
        public string Mensaje { get; set; }

        [GridMapping(Index = IndexTexto, ResourceName = "Labels", VariableName = "TEXTO", IncludeInSearch = true)]
        public string Texto { get; set; }

        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", AllowGroup = true, IncludeInSearch = true)]
        public string Dispositivo { get; set; }

        [GridMapping(Index = IndexEntidad, ResourceName = "Entities", VariableName = "PARENTI79", AllowGroup = true, IncludeInSearch = true)]
        public string Entidad { get; set; }

        [GridMapping(Index = IndexSubEntidad, ResourceName = "Entities", VariableName = "PARENTI81", AllowGroup = true, IncludeInSearch = true)]
        public string SubEntidad { get; set; }

        [GridMapping(Index = IndexSensor, ResourceName = "Entities", VariableName = "PARENTI80", AllowGroup = true, IncludeInSearch = true)]
        public string Sensor { get; set; }

        public int IdDispositivo { get; set; }

        public LogEventoVo(LogEvento logEvento)
        {
            Fecha = logEvento.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss");
            Mensaje = logEvento.Mensaje.Descripcion;
            Texto = logEvento.Texto;
            Dispositivo = logEvento.Dispositivo.Codigo;
            IdDispositivo = logEvento.Dispositivo.Id;
            Entidad = logEvento.SubEntidad.Entidad.Descripcion;
            SubEntidad = logEvento.SubEntidad.Descripcion;
            Sensor = logEvento.Sensor.Descripcion;
        }
    }
}
