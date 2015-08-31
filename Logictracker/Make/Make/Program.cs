using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Make
{
	internal class Program
	{
	    private static void Main(string[] args)
	    {
	        
			var num = 0;
			if (args.Length > 0)
			{
				if (!int.TryParse(args[0], out num))
				{
					if (args[0] == "help")
					{
						Help();
					}
					return;
				}
			}
            var text = File.ReadAllText("Logictracker\\src\\Web\\Logictracker\\Logictracker.Common.Web.CustomWebControls\\Labels\\version.txt").Trim();
            Console.WriteLine("Logictracker ver. " + text);
			var environmentVariable = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
			var environmentVariable2 = Environment.GetEnvironmentVariable("PROGRAMFILES");
			var stopwatch = new Stopwatch();
			if (num <= 1)
			{
				Console.WriteLine();
				Console.Write("1. Compilando Solución...");
				stopwatch.Start();
                var text2 = string.Format("{0}\\Microsoft Visual Studio 12.0\\Common7\\IDE\\devenv.exe", environmentVariable);
                var args2 = string.Format("\"Logictracker.Full.sln\" /build Debug /out \"{0}\"", "compile.log");
				Execute(text2, args2);
				Console.WriteLine(stopwatch.Elapsed);
				var text3 = File.ReadAllLines("compile.log").Last(x => !string.IsNullOrEmpty(x));
				Console.WriteLine();
				Console.WriteLine(text3);
				var num2 = Convert.ToInt32(text3.Split(new[] {','})[1].Trim().Split(new[]{' '})[0]);
				if (num2 > 0)
				{
					Console.WriteLine();
					Console.WriteLine("Hay errores de compilación. Deteniendo el proceso.");
					return;
				}
			}
			if (num <= 2)
			{
				Console.WriteLine();
				Console.Write("2. Publicando Web...");
				stopwatch.Reset();
                const string text2 = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\aspnet_compiler";
                const string args2 = "-nologo -v / -p \"Logictracker\\src\\Web\\Logictracker\\Logictracker.Web\" -u \"PrecompiledWeb\\Logictracker.Web\" -f -fixednames";
				Execute(text2, args2);
				Console.WriteLine(stopwatch.Elapsed);
				Console.WriteLine();
			}
			if (num <= 3)
			{
				Console.WriteLine();
				Console.Write("3. Preparando archivos...");
				stopwatch.Reset();
				const string text2 = "Preparar Actualizacion.bat";
				const string args2 = "";
				Execute(text2, args2);
				Console.WriteLine(stopwatch.Elapsed);
			}
			if (num <= 4)
			{
				var currentDirectory = Directory.GetCurrentDirectory();
				Directory.SetCurrentDirectory("Actualizacion");
				Console.WriteLine();
				Console.Write("4. Comprimiendo archivos...");
				stopwatch.Reset();
				var text2 = string.Format("{0}\\WinRAR\\rar.exe", environmentVariable);
				if (!File.Exists(text2))
				{
					text2 = string.Format("{0}\\WinRAR\\rar.exe", environmentVariable2);
				}
				if (!File.Exists(text2))
				{
					Console.Write("No se encontró el archivo rar.exe");
				}
				else
				{
					var args2 = string.Format("a -r {0}.rar Express Tester LogicLink LogicLink.Install LogicOut LogicOut.Install Web ReportsHost", text);
					Execute(text2, args2);
					Directory.SetCurrentDirectory(currentDirectory);
					Console.WriteLine(stopwatch.Elapsed);
				}
			}
			Console.WriteLine();
			Console.WriteLine("Terminado");
		}

		public static void Execute(string command, string args)
		{
			Console.WriteLine();
			Console.WriteLine();
			var process = new Process();
			var startInfo = new ProcessStartInfo(command, args)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardInput = true,
				RedirectStandardError = true
			};
			process.StartInfo = startInfo;
			process.Start();
			process.OutputDataReceived += ProcOutputDataReceived;
			process.BeginOutputReadLine();
			process.WaitForExit();
		}

		private static void ProcOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			try
			{
				Console.SetCursorPosition(0, Console.CursorTop - 1);
				Console.Write(e.Data.PadRight(128, ' '));
			}
			catch { }
		}

		public static void Help()
		{
            var str = File.ReadAllText("Logictracker\\src\\Web\\Logictracker\\Logictracker.Common.Web.CustomWebControls\\Labels\\version.txt");
            Console.WriteLine("Logictracker ver. " + str);
			Console.WriteLine();
			Console.WriteLine("Make [step]");
			Console.WriteLine();
			Console.WriteLine("Steps:");
			Console.WriteLine("      1. Compilacion");
			Console.WriteLine("      2. Publicación");
			Console.WriteLine("      3. Preparación de Archivos");
			Console.WriteLine("      4. Compresión");
		}
	}
}
