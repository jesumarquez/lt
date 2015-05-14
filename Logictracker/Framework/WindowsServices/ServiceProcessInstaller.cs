#region Usings

using System.Diagnostics;

#endregion

namespace Logictracker.WindowsServices
{

    public class ServiceProcessInstaller : System.ServiceProcess.ServiceProcessInstaller
    {
        /// <value>Arreglo con los argumentos de la linea de comandos que inicia el servicio.</value> 
        public string[] CmdLineArgs { get; set; }

        /// <value>Linea de comandos que inicia el servicio.</value>
        private string commandLineCache;
        public string CommandLine
        {
            get
            {
                if (!string.IsNullOrEmpty(commandLineCache)) return commandLineCache;
                commandLineCache = Context.Parameters["assemblypath"] + "\"";
                foreach (var s in CmdLineArgs)
                {
                    commandLineCache += " " + s;
                }
                commandLineCache += " \"";
                return commandLineCache;
            }    
        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            Debug.WriteLine("ServiceProcessInstaller: Successfully Service Arguments ({0})", CommandLine);
            
            Context.Parameters["assemblypath"] = CommandLine;
            base.OnBeforeInstall(savedState);
        }

        public override void Commit(System.Collections.IDictionary savedState)
        {
            Debug.WriteLine("ServiceProcessInstaller: Commit ({0})", CommandLine);
            base.Commit(savedState);
        }
    }
}
