namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    public class PartitionProcessorOptions
    {

        public PartitionProcessorOptions(string zookeeper, string topic, int partitionId, string clientId)
        {
            Zookeeper = zookeeper;
            Topic = topic;
            PartitionId = partitionId;
            ClientId = clientId;
        }

        public string Zookeeper { get; private set; }
        public string Topic { get; private set; }
        public int PartitionId { get; private set; }
        public string ClientId { get; private set; }
    }
}