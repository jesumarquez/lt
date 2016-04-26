using System;
using System.Diagnostics;
using System.Linq;
using Kafka.Client.Cfg;
using Kafka.Client.Consumers;
using Kafka.Client.Helper;
using Kafka.Client.Messages;
using Kafka.Client.Requests;
using Kafka.Client.ZooKeeperIntegration;
using ZooKeeperNet;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    public class PartitionProcessor : PartitionProcessorBase
    {
        private readonly PartitionProcessorOptions _processorOptions;

        public PartitionProcessor(PartitionProcessorOptions processorOptions, ChainHandler chainRoot)
            : base(chainRoot)
        {
            if (processorOptions == null) throw new ArgumentNullException("processorOptions");
            _processorOptions = processorOptions;
        }

        public override void Process()
        {


            const int factor = 0x2;
            var config = new KafkaSimpleManagerConfiguration()
            {
                FetchSize = KafkaSimpleManagerConfiguration.DefaultFetchSize / factor,
                BufferSize = KafkaSimpleManagerConfiguration.DefaultBufferSize / factor,
                Zookeeper = _processorOptions.Zookeeper,
            };

            var manager = new KafkaSimpleManager<int, Message>(config);

            const int correlationId = 0;
            var clientId = _processorOptions.ClientId;
            const int versionId = 1;

            manager.RefreshMetadata(versionId, clientId, correlationId,
                _processorOptions.Topic, true);
            var consumer = manager.GetConsumer(_processorOptions.Topic, _processorOptions.PartitionId);

            consumer.Config.AutoCommit = true;

            MainLoop(consumer, correlationId, manager);
        }

        private void MainLoop(Consumer consumer, int correlationId, KafkaSimpleManager<int, Message> manager)
        {
            var sw = new Stopwatch();
            sw.Start();

            var offsetManager = new OffsetManager(manager);

            var messageOffset = offsetManager.GetOffset(_processorOptions.Topic, _processorOptions.ClientId,
                _processorOptions.PartitionId);

            while (true)
            {
                var response = consumer.Fetch(_processorOptions.ClientId, _processorOptions.Topic, correlationId,
                    _processorOptions.PartitionId, messageOffset,
                    consumer.Config.FetchSize, manager.Config.MaxWaitTime, manager.Config.MinWaitBytes);

                var partitionData = response.PartitionData(_processorOptions.Topic, _processorOptions.PartitionId);

                var messageAndOffsets = partitionData.MessageSet;

                var count = 0;
                var lstOffset = messageOffset;
                var fstOffset = messageOffset;

                foreach (var messageAndOffset in messageAndOffsets)
                {
                    count++;
                    ProcessPayload(messageAndOffset);
                    lstOffset = messageAndOffset.MessageOffset;
                   
                }

                messageOffset = lstOffset;

                offsetManager.SetOffset(_processorOptions.Topic,
                    _processorOptions.ClientId, _processorOptions.PartitionId, messageOffset);


                Console.WriteLine("{3} [{4}-{5}] | {0}/{1} seg => {2}",
                    count, sw.Elapsed.TotalSeconds,
                    count / sw.Elapsed.TotalSeconds,
                    _processorOptions.PartitionId,
                    fstOffset,
                    lstOffset);

                sw.Restart();

            }

        }
    }
}
