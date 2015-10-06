using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class FinalExecutionCommand : IReportCommand
    {
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public string ReportName { get; set; }
        public string Email { get; set; }
        public int BaseId { get; set; }
        public ProgramacionReporte.FormatoReporte ReportFormat { get; set; }
    }
}
