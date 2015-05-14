using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ProgramacionReporteVo
    {
        public const int IndexReporte = 0;
        public const int IndexPeriodicidad = 1;
        public const int IndexMail = 2;
        public const int IndexBaja = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexReporte, ResourceName = "Labels", VariableName = "REPORTE", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public string Reporte { get; set; }

        [GridMapping(Index = IndexPeriodicidad, ResourceName = "Labels", VariableName = "PERIODICIDAD", AllowGroup = true)]
        public string Periodicidad { get; set; }

        [GridMapping(Index = IndexMail, ResourceName = "Labels", VariableName = "MAIL", AllowGroup = false, IncludeInSearch = true)]
        public string Mail { get; set; }

        [GridMapping(Index = IndexBaja, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true)]
        public string Estado { get; set; }

        public ProgramacionReporteVo(ProgramacionReporte progReporte)
        {
            switch (progReporte.Periodicidad)
            {
                case 'D': Periodicidad = CultureManager.GetLabel("DIARIO"); break;
                case 'S': Periodicidad = CultureManager.GetLabel("SEMANAL"); break;
                case 'M': Periodicidad = CultureManager.GetLabel("MENSUAL"); break;
            }
            Id = progReporte.Id;
            Reporte = progReporte.Reporte;
            Mail = progReporte.Mail;
            Estado = progReporte.Baja ? CultureManager.GetLabel("SUSPENDIDO") : CultureManager.GetLabel("ACTIVO");
        }
    }
}
