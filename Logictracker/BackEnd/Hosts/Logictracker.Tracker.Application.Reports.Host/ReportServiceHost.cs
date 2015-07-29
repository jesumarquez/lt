using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Spring.Context;
using Spring.Context.Support;

namespace Logictracker.Tracker.Application.Reports.Host
{
    class ReportServiceHost
    {   
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReportServiceHost));
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
