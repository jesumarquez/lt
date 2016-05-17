using System;
using System.Text;
using Kafka.Client.Consumers;
using Kafka.Client.Exceptions;
using Kafka.Client.Helper;
using Kafka.Client.Messages;
using Kafka.Client.ZooKeeperIntegration;
using log4net;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    class OffsetManager : IDisposable
    {
        internal static ILog Logger = LogManager.GetLogger(typeof(OffsetManager));

        private readonly KafkaSimpleManager<int, Message> _manager;
        private readonly IZooKeeperClient zkClient;

        public OffsetManager(KafkaSimpleManager<int, Message> manager)
        {
            _manager = manager;

            zkClient = new ZooKeeperClient(manager.Config.ZookeeperConfig.ZkConnect, manager.Config.ZookeeperConfig.ZkSessionTimeoutMs, ZooKeeperNetBinarySerializer.Serializer);
            zkClient.Connect();
        }

        public long GetOffset(string topic, string clientId, int partitionId)
        {
            var key = String.Format("/lt/offset/{0}/{1}/{2}", topic, clientId, partitionId);
            zkClient.MakeSurePersistentPathExists(key);
            var offset = zkClient.ReadData<OffsetManagerItem>(key);

            if (offset != null) return offset.Offset;

            long primero;
            long ultimo;
            long msgOffset;
            GetAdjustedOffset(topic, _manager, partitionId, KafkaOffsetType.Earliest, 0, 0, out primero,
                out ultimo, out msgOffset);
            offset = new OffsetManagerItem(topic, clientId, partitionId, msgOffset);
            zkClient.WriteData(key, offset);

            return offset.Offset;

        }

        public void SetOffset(string topic, string clientId, int partitionId, long msgOffset)
        {
            var key = String.Format("/lt/offset/{0}/{1}/{2}", topic, clientId, partitionId);

            var offset = new OffsetManagerItem(topic, clientId, partitionId, msgOffset);
            zkClient.WriteData(key, offset);
        }

        private static void GetAdjustedOffset<TKey, TData>(string topic, KafkaSimpleManager<TKey, TData> kafkaSimpleManager,
            int partitionId,
            KafkaOffsetType offsetType,
            long offset,
            int lastMessageCount, out long earliest, out long latest, out long offsetBase)
        {
            var sbSummaryOnfOnePartition = new StringBuilder();

            kafkaSimpleManager.RefreshAndGetOffset(0, String.Empty, 0, topic, partitionId, true, out earliest, out latest);
            sbSummaryOnfOnePartition.AppendFormat("\t\tearliest:{0}\tlatest:{1}\tlength:{2}"
                , earliest
                , latest
                , (latest - earliest) == 0 ? "(empty)" : (latest - earliest).ToString());

            if (offsetType == KafkaOffsetType.Timestamp)
            {
                var timestampVal = KafkaClientHelperUtils.DateTimeFromUnixTimestampMillis(offset);

                var timestampLong = KafkaClientHelperUtils.ToUnixTimestampMillis(timestampVal);
                try
                {
                    var timeStampOffset = kafkaSimpleManager.RefreshAndGetOffsetByTimeStamp(0, String.Empty, 0, topic, partitionId, timestampVal);

                    sbSummaryOnfOnePartition.AppendFormat("\r\n");
                    sbSummaryOnfOnePartition.AppendFormat("\t\ttimeStampOffset:{0}\ttimestamp(UTC):{1}\tTime(Local):{2}\tUnixTimestamp:{3}\t"
                        , timeStampOffset
                        , KafkaClientHelperUtils.DateTimeFromUnixTimestampMillis(timestampLong).ToString("s")
                        , timestampVal.ToString("s")
                        , timestampLong);

                    offsetBase = KafkaClientHelperUtils.GetValidStartReadOffset(offsetType, earliest, latest, timeStampOffset, lastMessageCount);
                }
                catch (TimeStampTooSmallException e)
                {
                    sbSummaryOnfOnePartition.AppendFormat("\r\n");
                    sbSummaryOnfOnePartition.AppendFormat("\t\ttimeStampOffset:{0}\ttimestamp(UTC):{1}\tTime(Local):{2}\tUnixTimestamp:{3}\t"
                        , "NA since no data before the time you specified, please retry with a bigger value."
                        , KafkaClientHelperUtils.DateTimeFromUnixTimestampMillis(timestampLong).ToString("s")
                        , timestampVal.ToString("s")
                        , timestampLong);

                    throw new ApplicationException(sbSummaryOnfOnePartition.ToString(), e);
                }
            }
            else
            {
                offsetBase = KafkaClientHelperUtils.GetValidStartReadOffset(offsetType, earliest, latest, 0, lastMessageCount);
            }

            Logger.Info(sbSummaryOnfOnePartition.ToString());
        }

        public void Dispose()
        {
            zkClient.Disconnect();
        }
    }
}
