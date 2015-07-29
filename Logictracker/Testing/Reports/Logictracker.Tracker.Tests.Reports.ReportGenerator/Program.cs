using Spring.Context;
using Spring.Context.Support;

namespace Logictracker.Tracker.Tests.Reports.ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            IApplicationContext ctx = ContextRegistry.GetContext();

            //new ReportMenuSelector().Initialize();
        }
    }
}
