#region Usings

using System;
using System.IO;
using System.Messaging;
using System.Windows.Forms;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.GatewayMovil
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process.InitAppDomain();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!DDL.Toolkit.FindLocalInstalation())
            {
                T.ERROR("Falta instalar el servicio de SQL.-");
                MessageBox.Show(
                    "Esta applicacion, requiere la instalacion del SQL Server 2005 Express. Instalelo y vuelva a intentarlo.",
                    "Falta un Requisito del sistema.", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (!Messaging.Toolkit.InstalledLocally())
            {
                T.ERROR("Falta instalar el servicio de MSMQ.-");
                MessageBox.Show(
                    "Esta applicacion, requiere la instalacion del servicio Microsoft Message Queue. Instalelo y vuelva a intentarlo.",
                    "Falta un Requisito del sistema.", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

              Application.Run(new MainForm());
        }
    }
}