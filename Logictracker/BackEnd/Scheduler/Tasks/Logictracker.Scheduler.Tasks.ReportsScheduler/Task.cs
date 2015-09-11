using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Reports.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Security;
using Logictracker.Layers.MessageQueue;
using Logictracker.Tracker.Application.Reports;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Scheduler.Tasks.ReportsScheduler
{
    public class Task : BaseTask
    {
        private int _results;
        private const string ComponentName = "Report Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            //var mail = new MailSender(Config.Mailing.ReportSchedulerMailingConfiguration);
            var queue = GetMailReportQueue();
            if (queue == null)
            {
                STrace.Error(ComponentName, "Cola no encontrada: revisar configuracion");
                return;
            }

            // BUSCO TODOS LAS PROGRAMACIONES DIARIAS
            var reportesProgramados = new List<ProgramacionReporte>();

            // SI ES LUNES AGREGO LAS PROGRAMACIONES SEMANALES
            if (DateTime.UtcNow.ToDisplayDateTime().DayOfWeek == DayOfWeek.Monday)
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('S'));  
            else
                reportesProgramados = DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('D');
  
            // SI ES 1° AGREGO LAS PROGRAMACIONES MENSUALES
            if (DateTime.UtcNow.ToDisplayDateTime().Day == 1)
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('M'));

            foreach (var reporte in reportesProgramados)
            {
                if ("MANUAL".Equals(reporte.ReportName)) continue;

                var cmd = GenerateReportCommand(reporte);
                if (cmd != null) queue.Send(cmd);
            }

            //genera un FinalExecutionCommand
            queue.Send(GenerateReportCommand(new ProgramacionReporte()));
        }

        private IReportCommand GenerateReportCommand(ProgramacionReporte prog)
        {
            switch (prog.Report)
            {
                case ProgramacionReporte.Reportes.ReporteEventos:
                    return ReportService.CreateEventReportCommand(prog.Id, prog.Empresa.Id, prog.Linea.Id,
                        prog.Mail, prog.GetParameters(ParameterType.Vehicle), GetFinalDate(), GetInitialDate(prog.Periodicity),
                        prog.GetParameters(ParameterType.Message), prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.KilometrosAcumulados:
                    return ReportService.CreateAccumulatedKilometersReportCommand(prog.Id, prog.Empresa.Id,
                        prog.Linea.Id, prog.Mail, GetFinalDate(), GetInitialDate(prog.Periodicity),
                        prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.ActividadVehicular:
                    return ReportService.CreateVehicleActivityReportCommand(prog.Id, prog.Empresa.Id,
                        prog.Linea.Id, prog.Mail, GetFinalDate(), GetInitialDate(prog.Periodicity),
                        prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.InfraccionesVehiculo:
                    return ReportService.CreateVehicleInfractionsReportCommand(prog.Id, prog.Empresa.Id,
                        prog.Linea.Id, prog.Mail, GetFinalDate(), GetInitialDate(prog.Periodicity),
                        prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.InfraccionesConductor:
                    return ReportService.CreateDriversInfractionsReportCommand(prog.Id, prog.Empresa.Id,
                        prog.Linea.Id, prog.Mail, GetFinalDate(), GetInitialDate(prog.Periodicity),
                        prog.GetParameters(ParameterType.Driver), prog.Format);

                case ProgramacionReporte.Reportes.EventosGeocercas:
                    return ReportService.CreateGeofenceEventsReportCommand(prog.Id, prog.Empresa.Id,
                        prog.Linea.Id, prog.Mail, GetFinalDate(), GetInitialDate(prog.Periodicity),
                        prog.GetParameters(ParameterType.Vehicle), prog.GetParameters(ParameterType.Geofence),prog.Format);

                case ProgramacionReporte.Reportes.TiempoAcumulado:
                    return ReportService.CreateMobilesTimeReportCommand(prog.Id, prog.Empresa.Id,
                        prog.Linea.Id, prog.Mail, GetFinalDate(), GetInitialDate(prog.Periodicity),
                        prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.VencimientoDocumentos:
                    return ReportService.CreateDocumentExpirationReportCommand(prog.Id, prog.Empresa.Id,
                       prog.Linea.Id, prog.Mail, GetFinalDate(), GetInitialDate(prog.Periodicity),
                       prog.GetParameters(ParameterType.Document),prog.Format);

                case ProgramacionReporte.Reportes.ReporteOdometros:
                    return ReportService.CreateOdometerReportCommand(prog.Id, prog.Empresa.Id, prog.Linea.Id, prog.Mail,
                        GetFinalDate(), GetInitialDate(prog.Periodicity), prog.GetParameters(ParameterType.Odometer),
                        prog.GetParameters(ParameterType.Vehicle), prog.Format);

                case ProgramacionReporte.Reportes.EstadoEntregas:
                    return ReportService.CreateDeliverStatusReportCommand(prog.Id, prog.Empresa.Id, prog.Linea.Id, prog.Mail,
                        GetFinalDate(), GetInitialDate(prog.Periodicity), prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.TrasladosViaje:
                    return ReportService.CreateTransfersPerTripReportCommand(prog.Id, prog.Empresa.Id, prog.Linea.Id, prog.Mail,
                       GetFinalDate(), GetInitialDate(prog.Periodicity), prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.ResumenRutas:
                    return ReportService.CreateSummaryRoutesReportCommand(prog.Id, prog.Empresa.Id, prog.Linea.Id, prog.Mail,
                        GetFinalDate(), GetInitialDate(prog.Periodicity), prog.GetParameters(ParameterType.Vehicle));

                case ProgramacionReporte.Reportes.VerificadorVehiculos:
                    return ReportService.CreateVehicleVerifierCommand(prog.Id, prog.Empresa.Id, prog.Linea.Id,prog.Mail,
                        prog.GetParameters(ParameterType.Carrier), prog.GetParameters(ParameterType.CostCenter),
                        prog.GetParameters(ParameterType.VehicleType));

                case ProgramacionReporte.Reportes.EjecucionReportes:
                    return ReportService.CreateExecutionReportCommand();

                default:
                    return null;
            }
        }

        private void GenerateHtmlReport(ProgramacionReporte prog)
        {
            switch (prog.Report)
            {
                case ProgramacionReporte.Reportes.ReporteEventos:
                    var vehiclesId = prog.GetParameters(ParameterType.Vehicle);//prog.Vehicles.Split(',').Select(v => Convert.ToInt32(v)).ToList();
                    var tiposMensajeId = prog.GetParameters(ParameterType.Message);//prog.MessageTypes.Split(',').Select(m => Convert.ToInt32(m));
                    var driversId = prog.GetParameters(ParameterType.Driver);//prog.Drivers.Split(',').Select(d => Convert.ToInt32(d)).ToList();
                    var fin = GetFinalDate();
                    var inicio = GetInitialDate(prog.Periodicity);

                    var results = ReportFactory.MobileEventDAO.GetMobilesEvents(vehiclesId,
                                                                                tiposMensajeId,
                                                                                driversId,
                                                                                inicio,
                                                                                fin,
                                                                                3);


                    break;
                case ProgramacionReporte.Reportes.VerificadorVehiculos:
                    var mobiles = DaoFactory.CocheDAO.GetList(new[] {prog.Empresa.Id},
                                                              new[] {prog.Linea != null ? prog.Linea.Id : 0},
                                                              new[] { -1 }, // TipoVehiculo
                                                              new[] { -1 }, // Transportista
                                                              new[] { -1 }, // DEPARTAMENTOS
                                                              new[] { -1 }, // CostCenter
                                                              new[] { -1 }, // SUB CENTROS DE COSTO
                                                              true,         // DispositivosAsignados,
                                                              false         // SoloConGarmin
                                                              );

                    var lastPositions = ReportFactory.MobilePositionDAO.GetMobilesLastPosition(mobiles);

                    var activos = lastPositions.Count(p => p.EstadoReporte <= 2);
                    var inactivos = lastPositions.Count(p => p.EstadoReporte > 2);

                    SendVerificadorVehiculosHtmlReport(prog, activos, inactivos);
                    break;
                default:
                    break;
            }
        }

        private void SendVerificadorVehiculosHtmlReport(ProgramacionReporte programacion, int activos, int inactivos)
        {
            var configFile = Config.Mailing.VerificadorVehiculosMailingConfiguration;
            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuración de mailing");
            
            var sender = new MailSender(configFile);
            
            var parametros = new List<string> { programacion.ReportName, activos.ToString("#0"), inactivos.ToString("#0") };
            SendMailToAllDestinations(sender, parametros, programacion.Mail);
        }

        private DateTime GetFinalDate()
        {
            //diario, semanal, mensual
            var initialDate = DateTime.Now.AddDays(-1);
            return new DateTime(initialDate.Year, initialDate.Month, initialDate.Day, 23, 59, 59);
        }

        private DateTime GetInitialDate(char periodicity)
        {
            var initialDate = DateTime.Now;

            switch (periodicity)
            {
                case 'D':
                    initialDate = DateTime.Now.AddDays(-1);
                    return new DateTime(initialDate.Year, initialDate.Month, initialDate.Day, 0, 0, 0);
                case 'S':
                    initialDate = DateTime.Now.AddDays(-7);
                    return new DateTime(initialDate.Year, initialDate.Month, initialDate.Day, 0, 0, 0);
                case 'M':
                    initialDate = DateTime.Now.AddMonths(-1);
                    return new DateTime(initialDate.Year, initialDate.Month, initialDate.Day, 0, 0, 0);
                default:
                    return new DateTime(initialDate.Year, initialDate.Month, initialDate.Day, 0, 0, 0);
            }
        }

        private IMessageQueue GetMailReportQueue()
        {
            var queueName = GetString("queuename");
            var queueType = GetString("queuetype");
            if (string.IsNullOrEmpty(queueName)) return null;

            var umq = new IMessageQueue(queueName);
            if (queueType.ToLower() == "xml") umq.Formatter = "XmlMessageFormatter";

            return !umq.LoadResources() ? null : umq;
        }
    }

}
