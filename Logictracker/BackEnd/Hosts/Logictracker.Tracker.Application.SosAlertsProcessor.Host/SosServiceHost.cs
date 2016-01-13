using log4net;
using Spring.Context;
using Spring.Context.Support;

namespace Logictracker.Tracker.Application.SosAlertsProcessor.Host
{
    public class SosServiceHost
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SosServiceHost));
        private IApplicationContext _ioc;

        public void Start()
        {
            Logger.Info("Starting...");
            _ioc = ContextRegistry.GetContext();
            Logger.Info("Started");

        }

        public void Stop()
        {
            Logger.Info("Stopping...");
            ((AbstractApplicationContext)_ioc).Stop();
            Logger.Info("Stopped.");
        }
    }
}