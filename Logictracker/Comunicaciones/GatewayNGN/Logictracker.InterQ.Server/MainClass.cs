#region Usings

using System;
using System.ServiceProcess;
using System.Windows.Forms;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.InterQ.Server
{
    public class MainClass
    {
        [STAThread]
        static void Main(string[] args)
        {
            // inicializacion global, trazadores, 
            // lectura de configuracion y remoting.
            Process.InitAppDomain();

            if (Environment.UserInteractive)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new InterQStandalone());
            }
            else
            {
                var ServicesToRun = new ServiceBase[] {new InterQService()};
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}