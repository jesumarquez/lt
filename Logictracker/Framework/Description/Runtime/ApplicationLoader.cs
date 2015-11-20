#region Usings

using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Common;
using Logictracker.Diagnostics;
using Logictracker.Utils;

#endregion

namespace Logictracker.Description.Runtime
{
	public class ApplicationLoader
	{
		#region Public Methods

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		public static void LoadAndRun(String[] args)
		{
			var appFile = String.Format(@"{0}\{1}", Process.GetApplicationFolder(), ((args.Length == 0) ? "AutoRun.xml" : args[0]));

		    STrace.Debug(typeof(ApplicationLoader).FullName, "Creating Backend Category into PerformanceCounterHelper");
            PerformanceCounterHelper.Create(new BackendCategory());
            //STrace.Debug(typeof(ApplicationLoader).FullName, "Launching Debugger...");
            //Debugger.LaunchDebugger(1);

            STrace.Debug(typeof(ApplicationLoader).FullName, "Initiating AppDomain...");
			Process.InitAppDomain();

			try
			{
                STrace.Debug(typeof(ApplicationLoader).FullName, "Loading MetadataDirectory...");
				MetadataDirectory.Load();
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(ApplicationLoader).FullName, e, "No se pudieron cargar los assemblies, borre y vuelva a copiar todos los archivos .exe .dll y .pdb");
				Thread.Sleep(2000);
				return;
			}

            if (!IOUtils.FileExists(appFile)) STrace.Error(typeof(ApplicationLoader).FullName, String.Format("No existe el archivo '{0}'", appFile));

			var mode = FrameworkApplication.GetRunMode(appFile);

			switch (mode)
			{
				case RunMode.WinService: ServiceMain(appFile, args[1]); break;
				case RunMode.WinForm: WindowsMain(appFile); break;
				case RunMode.Hidden: HiddenInstallMain(appFile); break;
				case RunMode.Console: ConsoleMain(appFile); break;
				default: STrace.Error(typeof(ApplicationLoader).FullName, "No se pudo determinar el modo requerido de ejecucion para la aplicacion."); break;
			}
			Environment.Exit(0);
		}

		#endregion

		#region Private Methods

		private static void HiddenInstallMain(String file)
		{
			try
			{
				var fa = new FrameworkApplication();
				var returnValue = fa.Run(file);
				fa.Unload();
				if (!(returnValue is String)) return;

				fa.Run(returnValue as String);
				fa.Unload();
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(ApplicationLoader).FullName, e);
			}
		}

		private static void WindowsMain(string file)
		{
			if (!Environment.UserInteractive)
			{
				STrace.Error(typeof(ApplicationLoader).FullName, String.Format("La aplicacion {0} se declaro como WinForm, pero no hay escritorio accesible.", file));
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Mainform());
		}

		private static void ConsoleMain(String file)
		{
			System.Console.BackgroundColor = ConsoleColor.DarkBlue;
			System.Console.Clear();
			try
			{
				var fa = new FrameworkApplication();
				var returnValue = fa.Run(file);
				fa.Unload();
				if (!(returnValue is String)) return;

				System.Console.Clear();
				System.Console.WriteLine("Aplicacion: {0}", returnValue);
				
				fa.Run(returnValue as String);
				fa.Unload();
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(ApplicationLoader).FullName, e);
				System.Console.WriteLine("Presione Entrar para salir.");
				System.Console.ReadLine();
			}
		}

		private static void ServiceMain(string file, string svcName)
		{
			try
			{
				Debugger.LaunchDebugger(2);

				var servicesToRun = new ServiceBase[] {new ServiceApplicationHost(svcName, file)};

				ServiceBase.Run(servicesToRun);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(ApplicationLoader).FullName, e);
			}
		}

		#endregion
	}
}
