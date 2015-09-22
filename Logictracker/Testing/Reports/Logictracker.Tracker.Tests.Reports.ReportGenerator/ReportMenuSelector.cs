using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Logictracker.Tracker.Application.WebServiceConsumer;
using Logictracker.Tracker.Services;

namespace Logictracker.Tracker.Tests.Reports.ReportGenerator
{
    public class ReportMenuSelector
    {
        public IReportService ReportService { get; set; }
        public IWebServiceConsumerService WebServiceConsumerService { get; set; }

        public ReportMenuSelector()
        {
            new Thread(Initialize).Start();
        }

        public void Initialize()
        {
            StringBuilder menuBuilder = new StringBuilder();
            menuBuilder.AppendLine("a. Generar un informe de eventos");
            menuBuilder.AppendLine("b. Generar un reporte de ejecucion finalizada");
            menuBuilder.AppendLine("c. Comando novedad webservice");
            menuBuilder.AppendLine("x. Para salir");
            menuBuilder.AppendLine("\n Seleccione una opcion: ");
            Console.Write(menuBuilder.ToString());

            ConsoleKeyInfo charOp;
            do
            {
                charOp = Console.ReadKey();
                Select(charOp.KeyChar);
            } while (charOp.KeyChar != 'x');
        }

        private void Select(char keyChar)
        {
            switch (keyChar)
            {
                case 'a':
                    GenerateReportEventCommand(75, "julian.millan@logictracker.com", 
                        new List<int> {8592,8594,8595},
                        new List<int> { 100 }, new List<int> { 0 }, new DateTime(2015, 06, 19, 03, 00, 00), new DateTime(2015, 06, 23, 03, 00, 00));
                    break;
                case 'b':
                    GenerateFinalReportExecutionCommand(DateTime.Now, "julian.millan@logictracker.com");
                    break;
                case 'c':
                    GenerateNoveltyCommand();
                    break;
                default:
                    Console.WriteLine(" <-- Opcion invalida");
                    break;
            }
        }

        private void GenerateNoveltyCommand()
        {
            WebServiceConsumerService.SendCommand(WebServiceConsumerService.GenerateNoveltyCommand());
            Console.WriteLine("MSG Sent");
        }

        private void GenerateFinalReportExecutionCommand(DateTime dateTime, string mail)
        {
            ReportService.GenerateFinalReportAndSendMail(dateTime, mail);
            Console.WriteLine("MSG Sent");
        }

        private void GenerateReportEventCommand(int customerId, string email, List<int> vehiclesId, List<int> messagesId, List<int> driversId, DateTime initialDate, DateTime finalDate)
        {
            ReportService.GenerateDailyEventReportAndSendMail(customerId,email,vehiclesId,messagesId,driversId,initialDate,finalDate);
            Console.WriteLine("MSG Sent");
        }
    }
}
