using System;
using log4net;
using Logictracker.DAL.NHibernate;
using Logictracker.Reports.Messaging;

namespace Logictracker.Tracker.Application.Reports
{
    public class MailReportMessageHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MailReportMessageHandler));

        public static ReportService ReportService { get; set; }

        //MailReportMessageHandler()
        //{
        //    ReportService = new ReportService();
        //}

        //handlers
        public void HandleMessage(EventReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateEventDailyReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(AccumulatedKilometersReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateAccumulatedKilometersReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(VehicleActivityReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateVehicleActivityReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }
        
        public void HandleMessage(VehicleInfractionsReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateVehicleInfractionsReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(DriversInfractionsReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateDriversInfractionsReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(GeofenceEventsReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateGeofenceEventsReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(MobilesTimeReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateMobilesTimeReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(DocumentsExpirationReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateDocumentsExpirationReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(OdometersReportCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateOdometersReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
            finally
            {
                ReportService.LogReportExecution(statusReport);
            }
        }

        public void HandleMessage(FinalExecutionCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            var statusReport = new ReportStatus();
            try
            {
                NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessGenerateFinalExceutionReportCommand(command, statusReport);

                NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                statusReport.Error = true;
                Logger.Error(e);
                throw;
            }
        }

        public void HandleMessage(IReportCommand command)
        {
            Logger.WarnFormat("Does not exist a report generator implementation for {0}", command.GetType());
        }

        //processors
        private void ProcessGenerateFinalExceutionReportCommand(FinalExecutionCommand command, ReportStatus statusReport)
        {
            var reportExecution = ReportService.GenerateFinalExcecutionReport(command, statusReport);
            ReportService.SendReport(reportExecution,  "Reporte de Ejecucion");
        }

        private void ProcessGenerateVehicleInfractionsReportCommand(VehicleInfractionsReportCommand command, ReportStatus statusReport)
        {
            using (
                var reportStream = ReportService.GenerateVehicleInfractionsReport(command, statusReport))
                        {
                            ReportService.SendReport(reportStream, command, command.ReportName);
                        }
        }

        private void ProcessGenerateAccumulatedKilometersReportCommand(AccumulatedKilometersReportCommand command, ReportStatus statusReport)
        {
            using (
                var reportStream = ReportService.GenerateAccumulatedKilometersReport(command, statusReport))
                {
                    ReportService.SendReport(reportStream, command, command.ReportName);
                }
        }

        private void ProcessGenerateVehicleActivityReportCommand(VehicleActivityReportCommand command, ReportStatus statusReport)
        {
            using (
                var reportStream = ReportService.GenerateVehicleActivityReport(command, statusReport))
                {
                    ReportService.SendReport(reportStream, command, command.ReportName);
                }
        }

        private void ProcessGenerateDriversInfractionsReportCommand(DriversInfractionsReportCommand command, ReportStatus statusReport)
        {
            using (
                var reportStream = ReportService.GenerateDriversInfractionReport(command, statusReport))
                        {
                            ReportService.SendReport(reportStream, command, command.ReportName);
                        }
        }

        private void ProcessGenerateGeofenceEventsReportCommand(GeofenceEventsReportCommand command, ReportStatus statusReport)
        {
            using ( var reportStream = ReportService.GenerateGeofenceEventsReport(command, statusReport))
                {
                    ReportService.SendReport(reportStream, command, command.ReportName);
                }
        }

        private void ProcessGenerateMobilesTimeReportCommand(MobilesTimeReportCommand command, ReportStatus statusReport)
        {
            using (var reportStream = ReportService.GenerateMobilesTimeReport(command, statusReport))
            {
                ReportService.SendReport(reportStream, command, command.ReportName);
            }
        }

        private void ProcessGenerateEventDailyReportCommand(EventReportCommand command, ReportStatus statusReport)
        {
            using (
                var reportStream = ReportService.GenerateDailyEventReport(command, statusReport))
            {
                ReportService.SendReport(reportStream, command, command.ReportName);
            }
        }

        private void ProcessGenerateDocumentsExpirationReportCommand(DocumentsExpirationReportCommand command, ReportStatus statusReport)
        {
            using (
                var reportStream = ReportService.GenerateDocumentExpirationReport(command, statusReport))
                        {
                            ReportService.SendReport(reportStream, command, command.ReportName);
                        }
        }

        private void ProcessGenerateOdometersReportCommand(OdometersReportCommand command, ReportStatus statusReport)
        {
            using (
                var reportStream = ReportService.GenerateOdometersReport(command, statusReport))
                {
                    ReportService.SendReport(reportStream, command, command.ReportName);
                }
        }
    }
}
