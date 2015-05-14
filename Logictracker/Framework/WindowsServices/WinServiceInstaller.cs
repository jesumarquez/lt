#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Logictracker.Description.Attributes;
using Logictracker.Description.Console;
using Logictracker.Description.Runtime;

#endregion

namespace Logictracker.WindowsServices
{
    /// <summary>
    /// Administrador de instalacion, mantenimiento y desintalacion de servicios
    /// windows agrupados por ServiceName, siempre y cuando este prefijo con
    /// lo definido en la propiedad <see cref="ServiceNamePrefix"/>.
    /// </summary>
	[FrameworkElement(XName = "WinServiceInstaller", IsContainer = true)]
    [ItemConstraint(typeof(WinService), MinItems = 1)]
    [ItemManagmentCommand("Add", typeof(WinService))]
    [ItemManagmentCommand("Delete", typeof(WinService), ConditionProperty = "IsRunnind", ConditionHint="Debe detener un servicio para poder eliminarlo.")]
	public class WinServiceInstaller : FrameworkElement, IConsoleInputListener
	{
		#region Attributes

        /// <summary>
        /// Prefijo de los servicios del grupo, es necesario para identificar los servicios
        /// que deben borrarse cuando esta activado AutomaticUninstall.
        /// </summary>
        [ElementAttribute(XName = "ServiceNamePrefix", IsRequired = true)]
        public string ServiceNamePrefix { get; set; }

		/// <summary>
		/// Si es true, elimina aquellos servicios de windows que comienzen con el prefijo indicado.
		/// </summary>
		/// <remarks>
		/// Por precaucion, esta funcionalidad solamente se dispara si el prefijo tiene 
		/// almenos 12 caracteres de longitud. En caso de tener menos, no hace nada.
		/// </remarks>
		[ElementAttribute(XName = "AutomaticUninstall", IsRequired = false, DefaultValue = true)]
		public bool AutomaticUninstall { get; set; }

		/// <summary>
		/// Si es true, todos los servicios se borran al comienzo de la actualizacion, sino solo los que no esten declarados en el xml.
		/// </summary>
        [ElementAttribute(XName = "Purge", IsRequired = false, DefaultValue = false)]
		public bool Purge { get; set; }

        /// <summary>
        /// Si es true, instala los servicios.
        /// </summary>
        [ElementAttribute(XName = "AutomaticInstall", IsRequired = false, DefaultValue = true)]
        public bool AutomaticInstall { get; set; }

        /// <summary>
        /// Si es true, actualiza las propiedades del servicio.
        /// </summary>
        [ElementAttribute(XName = "AutomaticUpdate", IsRequired = false, DefaultValue = true)]
        public bool AutomaticUpdate { get; set; }

        #endregion

		#region IConsoleInputListener

		public void OnKey(ConsoleKeyInfo inKey)
		{
			switch (inKey.Key)
			{
				case ConsoleKey.D0:
					Application.StopApplication();
					break;
				case ConsoleKey.D1:
					WinService.StopAndDisable(winServicesAll);
					break;
				case ConsoleKey.D2:
					WinService.EnableAndStart(winServicesAll);
					break;
				case ConsoleKey.D3:
					WinService.InstallAllServices(AutomaticUninstall, Purge, AutomaticInstall, AutomaticUpdate, ServiceNamePrefix, winServicesToInstall, this);
					winServicesAll = WinService.GetWindowsServices(ServiceNamePrefix);
					Console.WriteLine("Ready");
					break;
				case ConsoleKey.F5:
					Show();
					break;
			}
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
			winServicesToInstall = Elements().OfType<WinService>();

			winServicesAll = WinService.GetWindowsServices(ServiceNamePrefix);
			Show();
			ConsoleInput = new ConsoleInput(this);
			return true;
		}

	    protected override bool UnloadResources()
		{
			winServicesToInstall = null;

			foreach (var service in winServicesAll) service.Dispose();
			winServicesAll = null;
			ConsoleInput.Dispose();
			ConsoleInput = null;
			return true;
		}

    	#endregion

		#region Private Members

		private void Show()
		{
			Console.Clear();

			foreach (var service in winServicesAll)
			{
				service.Refresh();

				var status = service.Status.ToString();
				if (WinService.GetServiceStart(service.ServiceName) == ServiceStartMode.Disabled)
					status = "DISABLED (" + status + ")";

				Console.WriteLine(" {0} - {1}", status.PadRight(20), service.DisplayName.PadLeft(30));
				service.Close();
			}
			Console.WriteLine();
			Console.WriteLine("Elija Una Opcion: ");
			Console.WriteLine(" F5 - Refrescar");
			Console.WriteLine(" 0  - Salir");
			Console.WriteLine(" 1  - Detiene y Deshabilita Todos Los Servicios");
			Console.WriteLine(" 2  - Habilita e Inicia Todos Los Servicios");
			Console.WriteLine(" 3  - Desinstala e Instala Todos Los Servicios");
		}

		private ConsoleInput ConsoleInput;
		private IEnumerable<ServiceController> winServicesAll;
		private IEnumerable<WinService> winServicesToInstall;

		#endregion
	}
}
