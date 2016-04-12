using System.Collections.Generic;
using Kafka.Client.Cfg;
using Kafka.Client.Messages;
using Kafka.Client.Producers;
using Kafka.Client.Producers.Partitioning;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

namespace Logictracker.Dispatcher.Handlers
{
    [FrameworkElement(XName = "KafkaEnqueu", IsContainer = false)]
    public class KafkaEnqueu : DeviceBaseHandler<IMessage>
    {
        private ProducerDevId _producer;
        
        [ElementAttribute(XName = "Topic", IsRequired = false, DefaultValue = "lt_dispatcher")]
        public string Topic { get; set; }

        [ElementAttribute(XName = "Host", IsRequired = false, DefaultValue = "localhost")]
        public string Host { get; set; }

        [ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 9092)]
        public int Port { get; set; }

        [ElementAttribute(XName = "BrockerId", IsRequired = false, DefaultValue = 666)]
        public int BrokerId { get; set; }
        
        public override bool LoadResources()
        {
            var brokerConfig = new BrokerConfiguration()
            {
                BrokerId = BrokerId,
                Host = Host,
                Port = Port
            };

            var config = new ProducerConfiguration(new List<BrokerConfiguration>() {brokerConfig})
            {
                PartitionerClass = typeof (ProducerKeyPatition).AssemblyQualifiedName
            };

            _producer = new ProducerDevId(config);
            
            return true;
        }

        protected override HandleResults OnDeviceHandleMessage(IMessage message)
        {
            _producer.Send(BuildMessage(message));
            return HandleResults.Success;
        }

        private ProducerData<int, Message> BuildMessage(IMessage message)
        {
            var rv = new ProducerData<int, Message>(Topic, message.DeviceId, new Message(message.BinarySerialized()));
            return rv;
        }
        
        public class ProducerDevId : Producer<int, Message>
        {
            public ProducerDevId(ICallbackHandler<int, Message> callbackHandler) : base(callbackHandler)
            {
            }

            public ProducerDevId(ProducerConfiguration config) : base(config)
            {
            }
        }
        
        public class ProducerKeyPatition : IPartitioner<int>
        {
            public int Partition(int key, int numPartitions)
            {
                return key % 10;
            }
        }

    }

}
