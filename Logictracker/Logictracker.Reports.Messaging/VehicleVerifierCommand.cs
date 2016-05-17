using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class VehicleVerifierCommand : IReportCommand
    {
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public string Email { get; set; }
        public int CustomerId { get; set; }
        public int BaseId { get; set; }
        public string ReportName { get; set; }
        public int ReportId { get; set; }
        public int CostCenter { get; set; }
        public int VehicleType { get; set; }
        public int Carrier{ get; set; }

        public ProgramacionReporte.FormatoReporte ReportFormat { get; set; }
    }
}
