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

                x.SetDescription("LogicTracker Sos Alert Processor Host");
                x.SetDisplayName("LogicTracker Sos Alert Processor ");
                x.SetServiceName("Lt.Sos.Alert.Processor");
                x.DependsOnMsmq();
                x.DependsOnEventLog();
            });
        }
    }
}
