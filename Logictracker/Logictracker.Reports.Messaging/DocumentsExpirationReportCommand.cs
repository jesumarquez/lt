using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class DocumentsExpirationReportCommand : IReportCommand
    {
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public string Email { get; set; }
        public ProgramacionReporte.FormatoReporte ReportFormat { get; set; }

        public string ReportName { get; set; }
        public int ReportId { get; set; }
        public int CustomerId { get; set; }
        public List<int> Documents{ get; set; }
    }
}
