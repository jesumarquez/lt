#region Usings

using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace Logictracker.InterQueue
{
    public class GZip
    {
        public static byte[] Serialize(object graph)
        {
            var binaryStrm = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(binaryStrm, graph, null);
            return binaryStrm.ToArray();
        }

        public static byte[] SerializeAndCompress(object graph)
        {
            var binaryStrm = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(binaryStrm, graph, null);
            return Compress(binaryStrm.ToArray());
        }

        public static byte[] Compress(byte[] buffer)
        {
            var ms = new MemoryStream();
            var gZipStream = new GZipStream(ms, CompressionMode.Compress, true);
            gZipStream.Write(buffer, 0, buffer.GetLength(0));
            gZipStream.Close();
            ms.Position = 0;
            return ms.ToArray();
        }

        public static byte[] ReadToArray(Stream stream, int blockSize)
        {
            var ms = new MemoryStream();
            while (true)
            {
                var buffer = new byte[blockSize];
                var bytesRead = stream.Read(buffer, 0, blockSize);
                if (bytesRead == 0)
                {
                    break;
                }
                ms.Write(buffer,0,bytesRead);
            }
            ms.Position = 0;
            return ms.ToArray();
        } 

        public static byte[] Decompress(byte[] buffer)
        {
            var ms = new MemoryStream(buffer);
            return Decompress(ms);
        }

        public static byte[] Decompress(Stream source)
        {
            var zipStream = new GZipStream(source, CompressionMode.Decompress);
            return ReadToArray(zipStream, 100);
        }

        public static object DecompressAndDeserialize(byte[] buffer)
        {
            var formatter = new BinaryFormatter();
            var binaryStrm = new MemoryStream(Decompress(buffer));
            binaryStrm.Seek(0, 0);
            return formatter.Deserialize(binaryStrm);
        }

        public static object DecompressAndDeserialize(Stream source)
        {
            var formatter = new BinaryFormatter();
            var binaryStrm = new MemoryStream(Decompress(source));
            binaryStrm.Seek(0, 0);
            return formatter.Deserialize(binaryStrm);
        }

        public static object Deserialize(Stream source)
        {
            var formatter = new BinaryFormatter();
            return formatter.Deserialize(source);
        }
    }
}
