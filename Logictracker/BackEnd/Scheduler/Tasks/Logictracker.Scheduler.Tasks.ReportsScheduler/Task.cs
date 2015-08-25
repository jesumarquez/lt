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
            var queue = GetDispatcherQueue();
            if (queue == null)
            {
                STrace.Error(ComponentName, "Cola no encontrada: revisar configuracion");
                return;
            }

            // BUSCO TODOS LAS PROGRAMACIONES DIARIAS
            var reportesProgramados = DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('D');
            
            // SI ES LUNES AGREGO LAS PROGRAMACIONES SEMANALES
            if (DateTime.UtcNow.ToDisplayDateTime().DayOfWeek == DayOfWeek.Monday)
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('S'));                
            
            // SI ES 1° AGREGO LAS PROGRAMACIONES MENSUALES
            if (DateTime.UtcNow.ToDisplayDateTime().Day == 1)
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('M'));

            foreach (var reporte in reportesProgramados)
            {
                switch (reporte.Format)
                {
                    case ProgramacionReporte.FormatoReporte.Excel:
                        queue.Send(GenerateReportCommand(reporte));
                        break;
                    case ProgramacionReporte.FormatoReporte.Html:
                        GenerateHtmlReport(reporte);
                        break;
                }
            }

            //genera un FinalExecutionCommand
            queue.Send(GenerateReportCommand(new ProgramacionReporte()));
        }

        private IReportCommand GenerateReportCommand(ProgramacionReporte prog)
        {
            switch (prog.Report)
            {
                case ProgramacionReporte.Reportes.ReporteEventos:
                    return new EventReportCommand
                    {
                        ReportId = prog.Id,
                        ReportName = prog.ReportName,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        DriversId = CsvToList(prog.Drivers),
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        MessagesId = CsvToList(prog.MessageTypes),
                        VehiclesId = CsvToList(prog.Vehicles)
                    };
                case ProgramacionReporte.Reportes.KilometrosAcumulados:
                    return new AccumulatedKilometersReportCommand
                    {
                        ReportId = prog.Id,
                        ReportName = prog.ReportName,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        InCicle = prog.InCicle,
                        VehiclesId = CsvToList(prog.Vehicles)
                    };
                case ProgramacionReporte.Reportes.ActividadVehicular:
                    return new VehicleActivityReportCommand
                    {
                        ReportId = prog.Id,
                        ReportName = prog.ReportName,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        OvercomeKilometers = prog.OvercomeKilometers,
                        VehiclesId = CsvToList(prog.Vehicles)
                    };
                case ProgramacionReporte.Reportes.InfraccionesVehiculo:
                    return new VehicleInfractionsReportCommand
                    {
                        ReportId = prog.Id,
                        ReportName = prog.ReportName,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        ShowCorners = prog.ShowCorners,
                        VehiclesId = CsvToList(prog.Vehicles)
                    };
                case ProgramacionReporte.Reportes.InfraccionesConductor:
                    return new DriversInfractionsReportCommand
                    {
                        ReportId = prog.Id,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        ShowCorners = prog.ShowCorners,
                        DriversId = CsvToList(prog.Drivers),
                        ReportName = prog.ReportName
                    };
                case ProgramacionReporte.Reportes.EventosGeocercas:
                    return new GeofenceEventsReportCommand
                    {
                        ReportId = prog.Id,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        Geofences = CsvToList(prog.Geofences),
                        VehiclesId = CsvToList(prog.Vehicles),
                        CalculateKm = prog.CalculateKm,
                        InGeofenceTime = prog.GeofenceTime, 
                        ReportName = prog.ReportName
                    };
                case ProgramacionReporte.Reportes.TiempoAcumulado:
                    return new MobilesTimeReportCommand
                    {
                        ReportId = prog.Id,
                        ReportName = prog.ReportName,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        VehiclesId = CsvToList(prog.Vehicles)                        
                    };
                case ProgramacionReporte.Reportes.VencimientoDocumentos:
                    return new DocumentsExpirationReportCommand
                    {
                        ReportId = prog.Id,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        Documents = CsvToList(prog.Documents),
                        ReportName = prog.ReportName
                    };
                case ProgramacionReporte.Reportes.ReporteOdometros:
                    return new OdometersReportCommand
                    {
                        ReportId = prog.Id,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        Odometers = CsvToList(prog.Odometers),
                        VehiclesId = CsvToList(prog.Vehicles),
                        ReportName = prog.ReportName
                    };
                case ProgramacionReporte.Reportes.EstadoEntregas:
                    return new DeliverStatusReportCommand
                    {
                        ReportId = prog.Id,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        VehiclesId = CsvToList(prog.Vehicles),
                        ReportName = prog.ReportName
                    };
                case ProgramacionReporte.Reportes.TrasladosViaje:
                    return new TransfersPerTripReportCommand
                    {
                        ReportId = prog.Id,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        VehiclesId = CsvToList(prog.Vehicles),
                        ReportName = prog.ReportName
                    };
                case ProgramacionReporte.Reportes.ResumenRutas:
                    return new SummaryRoutesReportCommand
                    {
                        ReportId = prog.Id,
                        CustomerId = prog.Empresa.Id,
                        Email = prog.Mail,
                        FinalDate = GetFinalDate(),
                        InitialDate = GetInitialDate(prog.Periodicity),
                        VehiclesId = CsvToList(prog.Vehicles),
                        ReportName = prog.ReportName
                    };
                default:
                    return new FinalExecutionCommand
                    {
                        InitialDate = DateTime.Now
                    };
            }
        }

        private void GenerateHtmlReport(ProgramacionReporte prog)
        {
            switch (prog.Report)
            {
                case ProgramacionReporte.Reportes.ReporteEventos:
                    var vehiclesId = prog.Vehicles.Split(',').Select(v => Convert.ToInt32(v)).ToList();
                    var tiposMensajeId = prog.MessageTypes.Split(',').Select(m => Convert.ToInt32(m));
                    var driversId = prog.Drivers.Split(',').Select(d => Convert.ToInt32(d)).ToList();
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

        private List<int> CsvToList(string csvValues)
        {
            if(csvValues == null) return new List<int>();

            var idArray = csvValues.Split(',');
            return (from v in idArray where !string.IsNullOrEmpty(v) select int.Parse(v)).ToList();
        }

        private IMessageQueue GetDispatcherQueue()
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
