using System;
using System.Collections.Generic;

namespace Logictracker.Tracker.Services
{
    public interface IReportGenerationCommand
    {
        int CustomerId { get; set; }
        string Email { get; set; }
        List<int> VehiclesId { get; set; }
        IEnumerable<int> MessagesId { get; set; }
        List<int> DriversId { get; set; }
        DateTime InitialDate { get; set; }
        DateTime FinalDate { get; set; }
    }
}
