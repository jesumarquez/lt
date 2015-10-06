using Topshelf;

namespace Logictracker.Tracker.Application.WebServiceConsumer.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            HostFactory.Run(x =>
            {
                x.Service<WebConsumerHost>(s =>
                {
                    s.ConstructUsing(name => new WebConsumerHost());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("LogicTracker WebService Consumer Host");
                x.SetDisplayName("Lt WebService Consumer");
                x.SetServiceName("Lt.App.WebServiceConsumer");
                x.DependsOnMsmq();
                x.DependsOnEventLog();
            });
        }
    }
}
