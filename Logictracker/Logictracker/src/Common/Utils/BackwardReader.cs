#region Usings

using System;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace Logictracker.Utils
{
	public class BackwardReader : IDisposable
	{
		private FileStream fs;
		private readonly String Path;

		public BackwardReader(String path)
		{
			Path = path;
			fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
			fs.Seek(0, SeekOrigin.End);
		}

		public String ReadLine()
		{
			if (fs == null) return null;
			var position = fs.Position;
			if (position == 0) return null;

			var text = new byte[1];
			fs.Seek(0, SeekOrigin.Current);
			//tenemos \r\n al final?
			if (fs.Length > 1)
			{
				var vagnretur = new byte[2];
				fs.Seek(-2, SeekOrigin.Current);
				fs.Read(vagnretur, 0, 2);
				if (Encoding.ASCII.GetString(vagnretur).Equals("\r\n"))
				{
					//retroceder antes
					fs.Seek(-2, SeekOrigin.Current);
					position = fs.Position;
				}
			}
			while (fs.Position > 0)
			{
				text.Initialize();
				//leer un char
				fs.Read(text, 0, 1);
				var asciiText = Encoding.ASCII.GetString(text);
				//retroceder a un char antes
				fs.Seek(-2, SeekOrigin.Current);
				if (!asciiText.Equals("\n")) continue;
				fs.Read(text, 0, 1);
				asciiText = Encoding.ASCII.GetString(text);
				if (!asciiText.Equals("\r")) continue;
				fs.Seek(1, SeekOrigin.Current);
				break;
			}
			var count = int.Parse((position - fs.Position).ToString(CultureInfo.InvariantCulture));
			var line = new byte[count];
			fs.Read(line, 0, count);
			fs.Seek(-count, SeekOrigin.Current);
			return Encoding.ASCII.GetString(line);
		}

		/// <summary>
		/// Borra las lineas leidas, si no quedan mas lineas borra el archivo.
		/// </summary>
		public void TruncateHere()
		{
			if (fs.Position == 0)
			{
				CloseAndDispose();
				File.Delete(Path);
				return;
			} 
			fs.SetLength(fs.Position);
		}

		public void Write(String s)
		{
			fs.Write(Encoding.ASCII.GetBytes(s), 0, s.Length);
		}

		private void CloseAndDispose()
		{
			if (fs == null) return;
			fs.Close();
			fs.Dispose();
			fs = null;
		}

		#region IDisposable

		public void Dispose()
		{
			CloseAndDispose();
		}

		#endregion
	}

}
