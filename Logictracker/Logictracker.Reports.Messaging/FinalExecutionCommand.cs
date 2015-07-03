using System;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class FinalExecutionCommand
    {
        public string Email { get; set; }
        public DateTime InitialDate { get; set; }
    }
}
