using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Tracker.Services
{
    public interface IReportStatus
    {
        //ProgramacionReporte ReportProg { get; set; }
        DateTime StartReport { get; set; }
        int RowCount { get; set; }
        bool Error { get; set; }
    }
}
