using System;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import.Client.Transform
{
    [Serializable]
    public class DataTransformV1: IDataTransform
    {
        protected byte[] GetBytes(object data)
        {
            var bFormatter = new BinaryFormatter();
            var stream = new MemoryStream();
            var gz = new GZipStream(stream, CompressionMode.Compress);

            bFormatter.Serialize(gz, data);
            gz.Close();
            var buff = stream.ToArray();
            
            stream.Close();
            
            return buff;
        }

        protected object FromBytes(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            var gzip = new GZipStream(stream, CompressionMode.Decompress, false);
            var bformatter = new BinaryFormatter();

            var obj = bformatter.Deserialize(gzip);

            stream.Close();

            return obj;
        }

        #region IDataTransform Members

        public string Encode(IData data)
        {
            var buff = GetBytes(data.Pack());
            return Convert.ToBase64String(buff);
        }

        public IData Decode(string encoded)
        {
            var buff = Convert.FromBase64String(encoded);
            return FromBytes(buff) as IData;
        }

        #endregion

    }
}
