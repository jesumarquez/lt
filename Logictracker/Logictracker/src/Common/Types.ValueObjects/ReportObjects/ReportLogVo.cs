using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ReportLogVo
    {
        public const int IndexId = 0;
        public const int IndexDesde = 1;
        public const int IndexHasta = 2;
        public const int IndexDuracion = 3;
        public const int IndexModulo = 4;
        public const int IndexMensaje = 5;

        public int Id { get; set; }

        [GridMapping(Index = IndexDesde, ResourceName = "Labels", VariableName = "DESDE")]
        public DateTime Start { get; set; }

        [GridMapping(Index = IndexHasta, ResourceName = "Labels", VariableName = "HASTA")]
        public DateTime End { get; set; }

        [GridMapping(Index = IndexDuracion, ResourceName = "Labels", VariableName = "ROWS")]
        public int Rows { get; set; }

        [GridMapping(Index = IndexModulo, ResourceName = "Labels", VariableName = "ERRORS", IncludeInSearch = true, AllowGroup = true)]
        public string Errors { get; set; }

        [GridMapping(Index = IndexMensaje, ResourceName = "Labels", VariableName = "REPORT", IncludeInSearch = true)]
        public string Report { get; set; }

        public ReportLogVo(LogProgramacionReporte log)
        {
            Id = log.Id;
            Start = log.Inicio.ToDisplayDateTime();
            End = log.Fin.ToDisplayDateTime();
            Rows = log.Filas;
            Errors = log.Error? "Si" : "No";
            Report = log.ProgramacionReporte.ReportName;
        }
    }
}
