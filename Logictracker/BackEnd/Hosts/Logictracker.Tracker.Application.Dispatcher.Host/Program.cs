using System;
using System.Threading.Tasks;
using Logictracker.Tracker.Application.Dispatcher.Host.Handlers;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var maxPar = 1;
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
                new PartitionProcessorOptions("190.111.252.242:2181", "lt_dispatcher", partitionId, "jose_test");

            var spdTck = new SpeedTicketQtree(null);

            var chain = new OdometerHandler(spdTck);
            

            var processor = new PartitionProcessor(options, chain);


            processor.Process();

        }
    }
}
