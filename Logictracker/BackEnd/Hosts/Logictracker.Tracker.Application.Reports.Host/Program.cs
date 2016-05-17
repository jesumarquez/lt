using Topshelf;

namespace Logictracker.Tracker.Application.Reports.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            HostFactory.Run(x =>
            {
                x.Service<ReportServiceHost>(s =>
                {
                    s.ConstructUsing(name => new ReportServiceHost());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("LogicTracker Reports Service Host");
                x.SetDisplayName("LogicTracker Reports");
                x.SetServiceName("LogicTracker.Reports");
                x.DependsOnMsmq();
                x.DependsOnEventLog();
            });  
        }
    }
}
