using System;
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
            var reportesProgramados = new List<ProgramacionReporte>();
            
            // SI ES LUNES AGREGO LAS PROGRAMACIONES SEMANALES
            if (DateTime.UtcNow.ToDisplayDateTime().DayOfWeek == DayOfWeek.Monday)
            {
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('S'));                
            }
            else
            {
                reportesProgramados = DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('D');
            }

            // SI ES 1° AGREGO LAS PROGRAMACIONES MENSUALES
            if (DateTime.UtcNow.ToDisplayDateTime().Day == 1)
                reportesProgramados.AddRange(DaoFactory.ProgramacionReporteDAO.FindByPeriodicidad('M'));

            //solo para reportes de eventos
            foreach (var prog in reportesProgramados)
            {
                var msg = GenerateReportEventCommand(prog.Id, prog.ReportName, prog.Empresa.Id, prog.Mail, CsvToList(prog.Vehicles),
                    CsvToList(prog.MessageTypes), CsvToList(prog.Drivers), GetInitialDate(prog.Periodicity), GetFinalDate());
                queue.Send(msg);
            }
            //comando para generar el correo informando el estado de las ejecuciones
            queue.Send(GenerateFinalExecutionCommand());
        }

        private FinalExecutionCommand GenerateFinalExecutionCommand()
        {
            return new FinalExecutionCommand
            {
                InitialDate = DateTime.Now
            };
        }

        private DateTime GetFinalDate()
        {
            //diario, semanal, mensual
            var initialDate = DateTime.Now.AddDays(-1);
            return new DateTime(initialDate.Year, initialDate.Month, initialDate.Day, 23, 59, 59);
        }

        private DateTime GetInitialDate(char periodicity)
        {
            DateTime initialDate = DateTime.Now;

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
            if(csvValues==null)return new List<int>();

            var idArray = csvValues.Split(',');
            return (from v in idArray where !string.IsNullOrEmpty(v) select int.Parse(v)).ToList();
        }

        private EventReportCommand GenerateReportEventCommand(int id, string reportName, int customerId, string email, List<int> vehiclesId, List<int> messagesId, List<int> driversId, DateTime initialDate, DateTime finalDate)
        {
            return new EventReportCommand()
            {
                ReportId = id,
                ReportName = reportName,
                CustomerId = customerId,
                Email = email,
                DriversId = driversId,
                FinalDate = finalDate,
                InitialDate = initialDate,
                MessagesId = messagesId,
                VehiclesId = vehiclesId
            };
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
