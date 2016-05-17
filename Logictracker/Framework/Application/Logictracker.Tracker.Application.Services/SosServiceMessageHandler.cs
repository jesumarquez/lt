using System;
using log4net;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Tracker.Application.Services
{
    public class SosServiceMessageHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SosServiceMessageHandler));

        public static IIntegrationService IntegrationService{ get; set; }

        public void HandleMessage(SosTicket ticket)
        {
            try
            {
                IntegrationService.ProcessService(ticket);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error al obtener el ticket: {0} -> {1} ",ticket.NumeroServicio, ex.Message);
            }
        }
    }
}
