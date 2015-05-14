#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

#endregion

namespace Logictracker.Description.Console
{
    [FrameworkElement(XName = "ApplicationSelector", IsContainer = false)]
	public class ApplicationSelector : FrameworkElement, IConsoleInputListener
    {
		#region IConsoleInputListener

		public void OnKey(ConsoleKeyInfo inKey)
		{
			if (!_map.ContainsKey((int)inKey.Key)) return;
			var filename = _map[(int)inKey.Key];

			Application.ReturnValue = filename;
			Application.StopApplication();
		}

		public void OnLine(String line)
		{
			throw new NotImplementedException();
		}

		public ConsoleInputMode InputMode { get { return ConsoleInputMode.GetKey; } }

		#endregion

		#region FrameworkElement
		
		public override bool LoadResources()
		{
    		const string repository = "Applications";
            System.Console.Clear();
            System.Console.WriteLine(" Aplicaciones Disponibles en la Ruta: " + repository + " (10 maximo)");
            System.Console.WriteLine();
            String[] files = null;
			try
			{
        		files = Directory.GetFiles(repository, "*.xml");
			}
			catch (Exception)
			{
				System.Console.Clear();
				System.Console.WriteLine("Hubo un problema al intentar cargar las aplicaciones Disponibles en la Ruta: {0}", repository);
				System.Console.ReadLine();
				Environment.Exit(0);
			}
        	_map.Clear();
            var ix = 0;
            foreach (var file in files)
            {
                System.Console.WriteLine("{0} - {1}", ix, file);
                _map.Add(((int)ConsoleKey.D0) + ix, file);
                ix++;
                if (ix > 9) break;
            }
            if (ix == 0)
            {
                System.Console.WriteLine("No hay aplicaciones en la ruta indicada.");
                return false;
            }
            System.Console.WriteLine();
            System.Console.WriteLine("Elija una opcion (0-{0})", ix-1);
			_consoleInput = new ConsoleInput(this);
			return true;
		}

	    protected override bool UnloadResources()
		{
			if (_consoleInput != null) _consoleInput.Dispose();
			return true;
		}
		
		#endregion

		#region Private Members

		private ConsoleInput _consoleInput;
		private readonly Dictionary<int, string> _map = new Dictionary<int, String>();
		
		#endregion
	}
}