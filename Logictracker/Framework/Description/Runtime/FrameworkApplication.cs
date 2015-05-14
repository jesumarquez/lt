#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Common;
using Logictracker.Model.IAgent;

#endregion

namespace Logictracker.Description.Runtime
{
    public class FrameworkApplication
    {
        #region Public Members

        public static RunMode GetRunMode(String file)
        {
            try
            {
                var xDoc = XDocument.Load(file);

                if (xDoc.Root == null) return RunMode.Invalid;

                var rm = xDoc.Root.Attribute("RunMode");

                if (rm == null) return RunMode.Invalid;

                return (RunMode) Enum.Parse(typeof(RunMode), rm.Value);
            }
            catch(Exception)
            {
                STrace.Error(typeof(FrameworkElement).FullName, String.Format("Error determinando el modo de ejecucion de la aplicacion {0}", file));

                return RunMode.Invalid;
            }
        }

    	public Object Run(String file)
        {
			STrace.Module = file.Split('\\').Last().Split('.')[0];

			State = States.CreatingElements;
			_rootElement = FrameworkElement.Factory(XDocument.Load(file).Root, this, null);
			var descendants = _rootElement.Descendants();

			State = States.ResolvingDependencies;
			foreach (var fe in descendants) fe.ResolveDependencies();
			_rootElement.ResolveDependencies();

			State = States.LoadingResources;
			foreach (var fe in descendants)
			{
				fe.SafeLoadResources();
			}
			_rootElement.SafeLoadResources();

			State = States.StartingComponents;
			foreach (var fe in descendants.OfType<IService>())
			{
				fe.SafeServiceStart();
			}
			_rootElement.StartServiceIfIService();

			State = States.Running;

			while (State == States.Running)
			{
				Thread.Sleep(1000);
			}

            return ReturnValue;
        }

		public void StopApplication()
		{
			State = States.Unloading;
		}

		public bool IsResolvingDependencies { get { return State == States.ResolvingDependencies; } }

    	public void Unload()
    	{
			State = States.StoppingComponents;
			var descendants = _rootElement.Descendants();

			foreach (var fe in descendants.OfType<IService>())
			{
				fe.SafeServiceStop();
			}
			_rootElement.StopServiceIfIService();

			State = States.UnloadingResources;
			foreach (var fe in descendants)
			{
				fe.SafeUnloadResources();
			}
			_rootElement.SafeUnloadResources();

			State = States.Finished;
		}

		public void RegisterFrameworkElement(FrameworkElement item) { if (item != null && !String.IsNullOrEmpty(item.StaticKey)) _staticsElements.Add(item.StaticKey, item); }

		public FrameworkElement GetFrameworkElement(String staticKey) { return _staticsElements.ContainsKey(staticKey) ? _staticsElements[staticKey] : null; }

		public Object ReturnValue;

		#endregion

        #region Private Members

		private States State { get; set; }

	    private enum States
		{
			CreatingElements,
			ResolvingDependencies,
			LoadingResources,
			StartingComponents,
			Running,
			Unloading,
			StoppingComponents,
			UnloadingResources,
			Finished,
		}

        /// <summary>
        /// Dictionary for holding eache element associated to the application.
        /// </summary>
        private readonly Dictionary<string, FrameworkElement> _staticsElements = new Dictionary<string, FrameworkElement>();

        /// <summary>
        /// The root element for the application.
        /// </summary>
        private FrameworkElement _rootElement;

        #endregion
    }
}