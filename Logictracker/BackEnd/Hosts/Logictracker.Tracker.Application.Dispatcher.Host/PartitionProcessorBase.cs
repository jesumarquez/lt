using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Kafka.Client.Messages;
using Logictracker.Model;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    public abstract class PartitionProcessorBase :  IPartitionProcessor
    {
        protected readonly BinaryFormatter Formatter = new BinaryFormatter();
        protected ChainHandler ChainHandler;

        protected PartitionProcessorBase(ChainHandler chainRoot)
        {
            if (chainRoot == null) throw new ArgumentNullException("chainRoot");
            ChainHandler = chainRoot;
        }
        
        protected void ProcessPayload(MessageAndOffset messageAndOffset)
        {
            using (var stm = new MemoryStream(messageAndOffset.Message.Payload, false))
            {
                stm.Position = 0;
                var msg = Formatter.Deserialize(stm);
                ChainHandler.ProcessMessage((IMessage)msg);
            }
        }

        public abstract void Process();

    }
}
