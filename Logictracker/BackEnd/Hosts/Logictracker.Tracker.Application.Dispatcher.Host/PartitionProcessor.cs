using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Kafka.Client.Cfg;
using Kafka.Client.Consumers;
using Kafka.Client.Helper;
using Kafka.Client.Messages;
using Kafka.Client.Requests;
using Logictracker.Model;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    public class PartitionProcessor
    {
        private readonly PartitionProcessorOptions _processorOptions;
        private readonly BinaryFormatter _formatter = new BinaryFormatter();
        private readonly ChainHandler _chainHandler;

        public PartitionProcessor(PartitionProcessorOptions processorOptions, ChainHandler chainRoot)
        {
            if (processorOptions == null) throw new ArgumentNullException("processorOptions");
            if (chainRoot == null) throw new ArgumentNullException("chainRoot");
            _processorOptions = processorOptions;
            _chainHandler = chainRoot;
        }

        private void ProcessPayload(MessageAndOffset messageAndOffset)
        {
            using (var stm = new MemoryStream(messageAndOffset.Message.Payload, false))
            {
                stm.Position = 0;
                var msg = _formatter.Deserialize(stm);
                _chainHandler.ProcessMessage((IMessage)msg);
            }
        }

        public void Process()
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



            MainLoop(consumer, correlationId, manager);
        }

        private void MainLoop(Consumer consumer, int correlationId, KafkaSimpleManager<int, Message> manager)
        {
            var sw = new Stopwatch();
            sw.Start();
            long primero;
            long ultimo;
            long messageOffset;

            OffsetHelper.GetAdjustedOffset(_processorOptions.Topic, manager, _processorOptions.PartitionId, KafkaOffsetType.Earliest, 0, 0, out primero, out ultimo, out messageOffset);

            while (true)
            {
                var response = consumer.Fetch(_processorOptions.ClientId, _processorOptions.Topic, correlationId,
                    _processorOptions.PartitionId, messageOffset,
                    consumer.Config.FetchSize, manager.Config.MaxWaitTime, manager.Config.MinWaitBytes);

             
                var partitionData = response.PartitionData(_processorOptions.Topic, _processorOptions.PartitionId);

                var messageAndOffsets = partitionData.GetMessageAndOffsets();

                messageAndOffsets.ForEach(ProcessPayload);

                messageOffset = messageAndOffsets.Last().MessageOffset;

                Console.WriteLine("{3} [{4}-{5}] | {0}/{1} seg => {2}", messageAndOffsets.Count, sw.Elapsed.TotalSeconds, messageAndOffsets.Count / sw.Elapsed.TotalSeconds,_processorOptions.PartitionId,messageAndOffsets.First().MessageOffset,messageAndOffsets.Last().MessageOffset);
             
                   sw.Restart();
                
            }

        }
    }
}
