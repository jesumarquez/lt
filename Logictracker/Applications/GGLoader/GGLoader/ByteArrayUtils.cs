#region Usings

using System.Collections.Generic;
using System.IO;

#endregion

namespace GGLoader
{
	public static class ByteArrayUtils
    {
		#region Funciones de ByteArray

        public static byte[] ConcatByteArrays(params byte[][] args)
        {
            var bytes = new List<byte>();
            foreach (var param in args) bytes.AddRange(param);
            return bytes.ToArray();
        }

		public static void BytesToFile(string fn, byte[] ba, int offset)
		{
			using (var fs = new FileStream(fn, FileMode.OpenOrCreate, FileAccess.Write))
			{
				fs.Seek(offset, SeekOrigin.Begin);
				fs.Write(ba, 0, ba.Length);
			}
		}

		#endregion
    }
}
