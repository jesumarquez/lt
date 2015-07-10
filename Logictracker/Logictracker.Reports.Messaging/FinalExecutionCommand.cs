using System;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class FinalExecutionCommand : IReportCommand
    {
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public string Email { get; set; }
    }
}
