using System.Timers;
using log4net;
using Logictracker.Tracker.Services;

namespace Logictracker.Tracker.Application.WebServiceConsumer.Host
{
    public class WebServiceClientController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebServiceClientController));
        public IIntegrationService IntegrationService { get; set; }

        public System.Timers.Timer Timer4Services;
        public const double Time = 00.1;

        public void Start()
        {
            //Timer4Services = new System.Timers.Timer(Time * 60000);
            //Timer4Services.Elapsed += OnTimedEvent;
            //Timer4Services.AutoReset = true;
            //Timer4Services.Enabled = true;

            Logger.Warn("Starting WebService Consumer Host...");
            IntegrationService.CheckServices();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            IntegrationService.CheckServices();
        }

        public void Stop()
        {
            IntegrationService.Close();
            Timer4Services.Stop();
            Timer4Services.Dispose();
            
            Logger.Info("WebService Consumer Host Stopped");
        }
    }
}
