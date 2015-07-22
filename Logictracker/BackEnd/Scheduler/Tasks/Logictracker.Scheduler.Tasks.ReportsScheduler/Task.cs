﻿using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
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

            //genera un FinalExecutionCommand
            queue.Send(GenerateReportCommand(new ProgramacionReporte()));
        }

        private IReportCommand GenerateReportCommand(ProgramacionReporte prog)
        {
            switch (prog.Report)
            {
                case "EventsReport":
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
                case "AccumulatedKilometersReport":
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
                case "VehicleActivityReport":
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
                case "VehicleInfractionsReport":
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
                case "DriversInfractionsReport":
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
                case "GeofenceEventsReport":
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
                case "MobilesTimeReport":
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
                default:
                    return new FinalExecutionCommand
                    {
                        InitialDate = DateTime.Now
                    };
            }
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
