using System;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Tracker.Application.Reports
{
    public class ReportStatus : IReportStatus
    {
        public ReportStatus()
        {
            StartReport = DateTime.Now;
            Error = false;
        }

        public ProgramacionReporte ReportProg { get; set; }
        public DateTime StartReport { get; set; }
        public int RowCount { get; set; }
        public bool Error { get; set; }
    }
}