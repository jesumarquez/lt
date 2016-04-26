using System;
using System.Threading.Tasks;
using Logictracker.Tracker.Application.Dispatcher.Host.Handlers;
using Logictracker.Tracker.Application.Dispatcher.Host.Properties;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            const int maxPar = 10;
            var tsk = new Task[maxPar];
            for (var i = 0; i < maxPar; i++)
            {
                tsk[i] = Task.Factory.StartNew(ConsumeOnePartition, i,TaskCreationOptions.LongRunning);
            }

            Task.WaitAll(tsk);

        }
        private static void ConsumeOnePartition(object o)
        {
            var partitionId = (int) o;
            var options =
                new PartitionProcessorOptions(Settings.Default.kServer,  Settings.Default.kTopic, partitionId, Settings.Default.kClientId);

            var spdTck = new SpeedTicketQtree(null);

            var chain = new OdometerHandler(spdTck);
            
            IPartitionProcessor processor = new PartitionProcessor(options, chain);
            
            Console.WriteLine("Init consumer partition {1} - {0}",partitionId,Settings.Default.kClientId);
            processor.Process();

        }
    }
}
    