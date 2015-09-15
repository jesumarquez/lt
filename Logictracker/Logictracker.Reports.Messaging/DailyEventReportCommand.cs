using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class DailyEventReportCommand : IReportCommand
    {
        public int BaseId { get; set; }
        public int CustomerId { get; set; }
        public string ReportName { get; set; }
        public string Email { get; set; }
        public ProgramacionReporte.FormatoReporte ReportFormat { get; set; }
        public List<int> VehiclesId { get; set; }
        public IEnumerable<int> MessagesId { get; set; }
        public List<int> DriversId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
    }
}