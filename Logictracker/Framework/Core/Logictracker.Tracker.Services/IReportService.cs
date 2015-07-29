using System;
using System.Collections.Generic;
using System.IO;
using Logictracker.Reports.Messaging;

namespace Logictracker.Tracker.Services
{
    public interface IReportService
    {
        void GenerateDailyEventReportAndSendMail(int customerId, string email, List<int> vehiclesId, List<int> messagesId, List<int> driversId, DateTime initialDate, DateTime finalDate);
        Stream GenerateDailyEventReport(EventReportCommand command, IReportStatus statusReport);
    }
}
