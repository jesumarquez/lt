using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class DatamartLogVo
    {
        public const int IndexId = 0;
        public const int IndexDesde = 1;
        public const int IndexHasta = 2;
        public const int IndexDuracion = 3;
        public const int IndexModulo = 4;
        public const int IndexMensaje = 5;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexDesde, ResourceName = "Labels", VariableName = "DESDE")]
        public DateTime? Desde { get; set; }

        [GridMapping(Index = IndexHasta, ResourceName = "Labels", VariableName = "HASTA")]
        public DateTime? Hasta { get; set; }

        [GridMapping(Index = IndexDuracion, ResourceName = "Labels", VariableName = "DURACION")]
        public string Duracion { get; set; }

        [GridMapping(Index = IndexModulo, ResourceName = "Labels", VariableName = "MODULO", IncludeInSearch = true, AllowGroup = true)]
        public string Modulo { get; set; }

        [GridMapping(Index = IndexMensaje, ResourceName = "Labels", VariableName = "MENSAJE", IncludeInSearch = true)]
        public string Mensaje { get; set; }

        public DatamartLogVo(DataMartsLog log)
        {
            Id = log.Id;
            Desde = log.FechaInicio.HasValue ? log.FechaInicio.Value.ToDisplayDateTime() : (DateTime?) null;
            Hasta = log.FechaFin.HasValue ? log.FechaFin.Value.ToDisplayDateTime() : (DateTime?) null;

            var segundos = (int)(log.Duracion*60);
            var ts = new TimeSpan(0, 0, 0, segundos);
            Duracion = string.Format("{0} horas, {1} minutos, {2} segundos", ts.Hours, ts.Minutes, ts.Seconds);
            Modulo = DataMartsLog.Moludos.GetString(log.Modulo);
            Mensaje = log.Mensaje;
        }
    }
}
