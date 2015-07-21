using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ProgramacionReporteVo
    {
        public const int IndexReporte = 0;
        public const int IndexReportName= 1;
        public const int IndexPeriodicidad = 2;
        public const int IndexMail = 3;
        public const int IndexFormato = 4;
        public const int IndexBaja = 5;

        public int Id { get; set; }

        [GridMapping(Index = IndexReporte, ResourceName = "Labels", VariableName = "REPORTE", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public string Reporte { get; set; }

        [GridMapping(Index = IndexReportName, ResourceName = "Labels", VariableName = "REPORTNAME", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public string ReportName { get; set; }

        [GridMapping(Index = IndexPeriodicidad, ResourceName = "Labels", VariableName = "PERIODICIDAD", AllowGroup = true)]
        public string Periodicidad { get; set; }

        [GridMapping(Index = IndexMail, ResourceName = "Labels", VariableName = "MAIL", AllowGroup = false, IncludeInSearch = true)]
        public string Mail { get; set; }

        [GridMapping(Index = IndexFormato, ResourceName = "Labels", VariableName = "FORMATO", AllowGroup = false, IncludeInSearch = true)]
        public string Formato { get; set; }

        [GridMapping(Index = IndexBaja, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true)]
        public string Estado { get; set; }

        public ProgramacionReporteVo(ProgramacionReporte progReporte)
        {
            switch (progReporte.Periodicity)
            {
                case 'D': Periodicidad = CultureManager.GetLabel("DIARIO"); break;
                case 'S': Periodicidad = CultureManager.GetLabel("SEMANAL"); break;
                case 'M': Periodicidad = CultureManager.GetLabel("MENSUAL"); break;
            }
            switch (progReporte.Format)
            {
                case ProgramacionReporte.FormatoReporte.Excel: Formato = "Excel Completo"; break;
                case ProgramacionReporte.FormatoReporte.Html: Formato = "Email Resumido"; break;
                default: Formato = "Ninguno"; break;
            }
            Id = progReporte.Id;
            Reporte =   CultureManager.GetLabel(progReporte.Report);
            ReportName = progReporte.ReportName;            
            Mail = progReporte.Mail;
            Estado = progReporte.Active ? CultureManager.GetLabel("ACTIVE") : CultureManager.GetLabel("INACTIVE");
        }
    }
}
