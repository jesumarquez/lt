using System;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    [Serializable]
    public class OffsetManagerItem
    {

        public string Topic { get; set; }
        public string ClientId { get; set; }
        public int PartitionId { get; set; }

        public long Offset { get; set; }

        public OffsetManagerItem(string topic, string clientId, int partitionId, long offset)
        {
            Topic = topic;
            ClientId = clientId;
            PartitionId = partitionId;
            Offset = offset;
        }
    }
}