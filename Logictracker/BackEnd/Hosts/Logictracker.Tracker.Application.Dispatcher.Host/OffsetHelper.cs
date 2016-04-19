using System;
using System.Text;
using Kafka.Client.Consumers;
using Kafka.Client.Exceptions;
using Kafka.Client.Helper;
using log4net;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    class OffsetHelper
    {
        internal static ILog Logger = LogManager.GetLogger(typeof(OffsetHelper));

        internal static void GetAdjustedOffset<TKey, TData>(string topic, KafkaSimpleManager<TKey, TData> kafkaSimpleManager,
            int partitionId,
            KafkaOffsetType offsetType,
            long offset,
            int lastMessageCount, out long earliest, out long latest, out long offsetBase)
        {
            var sbSummaryOnfOnePartition = new StringBuilder();

            kafkaSimpleManager.RefreshAndGetOffset(0, string.Empty, 0, topic, partitionId, true, out earliest, out latest);
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
                    var timeStampOffset = kafkaSimpleManager.RefreshAndGetOffsetByTimeStamp(0, string.Empty, 0, topic, partitionId, timestampVal);

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
    }
}
