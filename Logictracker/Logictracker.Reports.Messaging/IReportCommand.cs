using System;

namespace Logictracker.Reports.Messaging
{
    public interface IReportCommand
    {
        DateTime InitialDate { get; set; }
        DateTime FinalDate { get; set; }
        string Email { get; set; }
    }
}