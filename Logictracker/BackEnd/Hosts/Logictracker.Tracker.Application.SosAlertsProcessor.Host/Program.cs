using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Logictracker.Tracker.Application.SosAlertsProcessor.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            HostFactory.Run(x =>
            {
                x.Service<SosServiceHost>(s =>
                {
                    s.ConstructUsing(name => new SosServiceHost());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("LogicTracker Webservice Sos Host");
                x.SetDisplayName("LogicTracker WebService Sos");
                x.SetServiceName("LogicTracker.Ws.Sos");
                x.DependsOnMsmq();
                x.DependsOnEventLog();
            });
        }
    }
}
