using System;
using Urbetrack.Toolkit;

namespace Base64toHexa
{
	class Program
	{
		static void Main()
		{
			while (true)
			{
				Console.WriteLine("Ingrese un String Base 64 para transformar a Hexa o nada para salir");
				var s = Console.ReadLine();
				if (String.IsNullOrEmpty(s)) return;
				var ss = Convert.FromBase64String(s);
				var sss = Utils.ByteArrayToHexString(ss, 0, ss.Length);
				Console.WriteLine(sss);
			}
		}
	}
}
