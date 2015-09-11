using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reports.Messaging
{
    public interface IReportCommand
    {
        DateTime InitialDate { get; set; }
        DateTime FinalDate { get; set; }
        string Email { get; set; }
        ProgramacionReporte.FormatoReporte ReportFormat { get; set; }
    }
}