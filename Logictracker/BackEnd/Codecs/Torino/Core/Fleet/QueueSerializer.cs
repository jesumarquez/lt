#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using Urbetrack.Configuration;
using Urbetrack.Utils;

#endregion

namespace Urbetrack.Toolkit
{
    public class QueueSerializer
    {
        private static readonly string files_path;

        static QueueSerializer()
        {
			files_path = GetQtreeApplicationFolder();
        }

        static short DecodeShort(byte[] buffer, int pos)
        {
            return (short)(buffer[pos++] << 8 | buffer[pos]);
        }

        static void EncodeShort(ref byte[] buffer, short data)
        {
            var b = Convert.ToByte((byte)(data >> 8));
            buffer[0] = b;
            buffer[1] = Convert.ToByte(data & 0xFF);
        }

		public static string GetQtreeApplicationFolder()
		{
			return Config.Torino.QtreeRepository ?? Process.GetApplicationFolder("qtree");
		}

        public static void Store(string filename, Queue<byte[]> source)
        {
            var location = String.Format("{0}\\{1}", files_path, filename);
            using (var file = File.Create(location))
            {
                foreach (var block in source)
                {
                    var bsize = new byte[2];
                    EncodeShort(ref bsize, (short) block.GetLength(0));
                    file.Write(bsize, 0, 2);
                    file.Write(block, 0, block.GetLength(0));
                }
                file.Close();
            }
        }

        public static Queue<byte[]> Fetch(string filename)
        {
            var location = String.Format("{0}\\{1}", files_path, filename);
            if (!File.Exists(location)) return null;

            var result = new Queue<byte[]>();
            var data = File.ReadAllBytes(location);
            var cursor = 0;
            while (cursor < data.GetLength(0))
            {
                var bsize = new byte[2];
            	Array.Copy(data, cursor, bsize, 0, 2);
                var datasize = DecodeShort(bsize, 0);
                cursor += 2;
                if (datasize <= 0)
                    return null;
                var block = new byte[datasize];
                Array.Copy(data, cursor, block, 0, datasize);
                cursor += datasize;
                result.Enqueue(block);
            }
            return result;
        }

   }
}

