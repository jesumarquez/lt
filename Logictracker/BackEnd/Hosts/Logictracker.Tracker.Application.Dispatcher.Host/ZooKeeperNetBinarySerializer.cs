using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Kafka.Client.ZooKeeperIntegration;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    public class ZooKeeperNetBinarySerializer : IZooKeeperSerializer
    {
        public static ZooKeeperNetBinarySerializer Serializer = new ZooKeeperNetBinarySerializer();

        protected readonly BinaryFormatter Formatter = new BinaryFormatter();
        public byte[] Serialize(object obj)
        {
            using (var stm = new MemoryStream())
            {
                Formatter.Serialize(stm, obj);
                return stm.GetBuffer();
            }
        }

        public object Deserialize(byte[] bytes)
        {
            if (bytes == null)
                return null;
            using (var stm = new MemoryStream(bytes, false))
            {
                stm.Position = 0;
                return Formatter.Deserialize(stm);
            }
        }
    }
}