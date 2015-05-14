using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Geocoder.Core.Security
{
	public sealed class SoftwareProtection
	{
		private static List<char> alfa = new List<char>("ONS9XFG1YQ23CIB8LZT67P5J4KARUVHMWDE0".ToCharArray());
		private static string clave = "1rAmIrO2bUgAlLo8";
		public static string RetrieveHardKey()
		{
			var hardwareInfo = new HardwareInfo();
            var cpuId = hardwareInfo.GetCPUId();
            var volumeSerial = hardwareInfo.GetVolumeSerial("C");
            var macAddress = hardwareInfo.GetMACAddress();
            var text = cpuId + volumeSerial + macAddress;
            var num = text.Length - 1;
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < 15; i += 3)
			{
				if (stringBuilder.ToString() != "") stringBuilder.Append("-");
				stringBuilder.Append(text[i]);
				stringBuilder.Append(text[num]);
				stringBuilder.Append(text[i + 1]);
				stringBuilder.Append(text[num - 1]);
				stringBuilder.Append(text[i + 2]);
				num -= 2;
			}
			return stringBuilder.ToString();
		}
		public static bool ValidateKey(string key)
		{
			return key == GenerateKey(RetrieveHardKey());
		}
		public static bool VerifyKey()
		{
			var array = ReadKey();
			bool result;
			if (array == null)
			{
				result = false;
			}
			else
			{
				var text = GenerateKey(RetrieveHardKey());
				var array2 = Encrypt(text.Substring(0, text.Length - 6), array[0], array[1]);
				result = CompareByteArray(array2[2], array[2]);
			}
			return result;
		}
		private static string GenerateKey(string key)
		{
			key = key.ToUpper();
			var text = "";
			var num = 0;
			for (var i = 0; i < key.Length; i++)
			{
				if (key[i] == '-')
				{
					text += "-";
				}
				else
				{
					var num2 = alfa.IndexOf(key[i]);
					num2 += alfa.IndexOf(clave[num]);
					text += alfa[GetNewPos(num2)];
					if (num < clave.Length - 1)
						num++;
					else
						num = 0;
				}
			}
			return text;
		}
		private static int GetNewPos(int idx)
		{
			while (idx >= alfa.Count)
			{
				idx -= alfa.Count;
			}
			return idx;
		}
		private static int GetOldPos(int idx)
		{
			while (idx < 0)
			{
				idx += alfa.Count;
			}
			return idx;
		}
		private static string GetOldString(string str)
		{
			var num = 0;
			var text = "";
			for (var i = 0; i < str.Length; i++)
			{
				var item = str[i];
				var num2 = alfa.IndexOf(item);
				num2 -= alfa.IndexOf(clave[num]);
				text += alfa[GetOldPos(num2)];
				if (num < clave.Length - 1)
					num++;
				else
					num = 0;
			}
			return text;
		}
		public static void SaveKey(string key)
		{
			var str = Getpath();
            var str2 = key.Substring(0, key.Length - 6);
            var s = key.Substring(key.Length - 5, 5);
            var bytes = Encoding.ASCII.GetBytes(s);
            var fileStream = File.Create(str + "\\licence.spk");
            var array = Encrypt(str2);
			fileStream.Write(array[0], 0, array[0].Length);
			fileStream.Write(array[1], 0, array[1].Length);
			fileStream.Write(array[2], 0, array[2].Length);
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Close();
		}
		private static byte[][] ReadKey()
		{
			var str = Getpath();
			byte[][] result;
			if (!File.Exists(str + "\\licence.spk"))
			{
				result = null;
			}
			else
			{
				var fileStream = File.Open(str + "\\licence.spk", FileMode.Open);
                var array = new byte[32];
                var array2 = new byte[16];
				var array3 = new byte[fileStream.Length - 32L - 16L - 5L];
                var array4 = new byte[5];
				fileStream.Read(array, 0, 32);
				fileStream.Read(array2, 0, 16);
				fileStream.Read(array3, 0, Convert.ToInt32(fileStream.Length - 32L - 16L - 5L));
				fileStream.Read(array4, 0, 5);
				fileStream.Close();
			    result = new[]
			                 {
			                     array,
			                     array2,
			                     array3,
			                     array4
			                 };
			}
			return result;
		}
		private static byte[][] Encrypt(string str, byte[] key, byte[] iv)
		{
			var bytes = Encoding.ASCII.GetBytes(str);
			var rijndaelManaged = new RijndaelManaged();
			var cryptoStream = new CryptoStream(new MemoryStream(bytes), rijndaelManaged.CreateEncryptor(key, iv), CryptoStreamMode.Read);
			var array = new byte[str.Length];
			cryptoStream.Read(array, 0, array.Length);
			cryptoStream.Close();
		    return new[]
		               {
		                   key,
		                   iv,
		                   array
		               };
		}
		private static byte[][] Encrypt(string str)
		{
			var bytes = Encoding.ASCII.GetBytes(str);
			var rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.GenerateKey();
			rijndaelManaged.GenerateIV();
			var key = rijndaelManaged.Key;
			var iV = rijndaelManaged.IV;
			var cryptoStream = new CryptoStream(new MemoryStream(bytes), rijndaelManaged.CreateEncryptor(key, iV), CryptoStreamMode.Read);
			var array = new byte[str.Length];
			cryptoStream.Read(array, 0, array.Length);
			cryptoStream.Close();
		    return new[]
		               {
		                   key,
		                   iV,
		                   array
		               };
		}
		private static string Getpath()
		{
			var text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			if (text != null && text.StartsWith("file:\\"))
			{
				text = text.Substring(6);
			}
			return text;
		}
		private static bool CompareByteArray(byte[] b1, byte[] b2)
		{
			bool result;
			if (b1 == null && b2 == null)
			{
				result = true;
			}
			else
			{
				if (b1 == null || b2 == null)
				{
					result = false;
				}
				else
				{
					if (b1.Length != b2.Length)
					{
						result = false;
					}
					else
					{
						for (var i = 0; i < b1.Length; i++)
						{
							if (b1[i] != b2[i])
							{
								result = false;
								return result;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}
	}
}
