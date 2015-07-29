using log4net;
using Spring.Context;
using Spring.Context.Support;


namespace Logictracker.Tracker.Application.Parser.Host
{
    public class SpringServiceHost
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SpringServiceHost));
        private IApplicationContext _ioc;

        public void Start()
        {
            Logger.Info("Starting...");
            _ioc = ContextRegistry.GetContext();
            Logger.Info("Started");
        }

        public void Stop()
        {
            Logger.InfoFormat("Stopping");
            ((AbstractApplicationContext)_ioc).Stop();
            _ioc.Dispose();
            Logger.Info("Stopped.");
        }
    }
}
