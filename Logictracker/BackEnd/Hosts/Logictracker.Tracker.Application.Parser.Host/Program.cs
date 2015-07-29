using log4net.Config;
using Topshelf;

namespace Logictracker.Tracker.Application.Parser.Host
{
    class Program
    {
        public static void Main()
        {
            XmlConfigurator.Configure();

            HostFactory.Run(x =>                                 
            {
                x.Service<SpringServiceHost>(s =>                        
                {
                    s.ConstructUsing(name => new SpringServiceHost());   
                    s.WhenStarted(tc => tc.Start());             
                    s.WhenStopped(tc => tc.Stop());              
                });
                x.RunAsLocalSystem();                            

                x.SetDescription("Gateway Caesat Client Host");        
                x.SetDisplayName("3-Gateway Caesat");             
                x.SetServiceName("LT.Caesat.Gateway");
                x.DependsOnMsmq();
                x.DependsOnEventLog();
            });                                                  
        }
    }
}
