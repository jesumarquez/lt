using System;
using System.Collections.Generic;
using Logictracker.Tracker.Services;

namespace Logictracker.Tracker.Application.Reports
{
    [Serializable]
    public class DailyEventReportCommand : IReportGenerationCommand
    {
        public int BaseId { get; set; }
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public List<int> VehiclesId { get; set; }
        public IEnumerable<int> MessagesId { get; set; }
        public List<int> DriversId { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
    }
}