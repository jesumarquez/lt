using System;
using System.Collections.Generic;
using System.IO;
using Logictracker.Reports.Messaging;

namespace Logictracker.Tracker.Services
{
    public interface IReportService
    {
        void GenerateDailyEventReportAndSendMail(int customerId, string email, List<int> vehiclesId, List<int> messagesId, List<int> driversId, DateTime initialDate, DateTime finalDate);
        void GenerateFinalReportAndSendMail(DateTime dateTime, string mail);

        Stream GenerateDailyEventReport(EventReportCommand command, IReportStatus statusReport);
        Stream GenerateVehicleInfractionsReport(VehicleInfractionsReportCommand command, IReportStatus reportStatus);
        Stream GenerateAccumulatedKilometersReport(AccumulatedKilometersReportCommand command, IReportStatus reportStatus);
        Stream GenerateVehicleActivityReport(VehicleActivityReportCommand command, IReportStatus reportStatus);
        Stream GenerateDriversInfractionReport(DriversInfractionsReportCommand command, IReportStatus reportStatus);
        Stream GenerateGeofenceEventsReport(GeofenceEventsReportCommand command, IReportStatus reportStatus);
        Stream GenerateMobilesTimeReport(MobilesTimeReportCommand command, IReportStatus reportStatus);
        Stream GenerateDocumentExpirationReport(DocumentsExpirationReportCommand command, IReportStatus reportStatus);
        Stream GenerateOdometersReport(OdometersReportCommand command, IReportStatus reportStatus);
        Stream GenerateTransfersPerTripReport(TransfersPerTripReportCommand command, IReportStatus reportStatus);
        Stream GenerateDeliverStatusReport(DeliverStatusReportCommand command, IReportStatus reportStatus);
        Stream GenerateSummaryRoutesReport(SummaryRoutesReportCommand command, IReportStatus reportStatus);
        string GenerateSummarizedDriversInfractionReport(DriversInfractionsReportCommand command, IReportStatus reportStatus);       
        string GenerateFinalExcecutionReport(FinalExecutionCommand command, IReportStatus reportStatus);       

        void LogReportExecution(IReportStatus reportStatus);
        void SendHtmlReport(string report, DriversInfractionsReportCommand command, string reportName);
        void SendEmptyReport(IReportCommand command, string reportName);
        void SendReport(string reportExecution, string reporteDeEjecucion);
        void SendReport(Stream reportStream, IReportCommand command, string reportName);
    }
}
