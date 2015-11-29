using System.Runtime.Remoting.Messaging;
using log4net;
using Spring.Messaging.Core;
using Logictracker.Tracker.Services;

namespace Logictracker.Tracker.Application.WebServiceConsumer.Host
{
    class WebServiceClientController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebServiceClientController));

        public IWebServiceClient WebServiceClient { get; set; }
        public MessageQueueTemplate TrackMessageQueueTemplate { get; set; }

        public void Start()
        {
            Logger.Warn("Starting WebService Consumer Host...");

            WebServiceClient.StartService();
        }


        public void Stop()
        {
            Logger.Info("Parser caesat stopped");
        }

        private void SendtrackReport(IMessage msg)
        {
            if (msg != null)
            {
                Logger.InfoFormat("Message sent {0} ", msg);
                TrackMessageQueueTemplate.ConvertAndSend(msg);
            }
            Logger.InfoFormat("SendtrackReport");
        }
    }
}
