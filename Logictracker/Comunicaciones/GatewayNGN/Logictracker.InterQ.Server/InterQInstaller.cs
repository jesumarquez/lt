#region Usings

using System;
using System.ComponentModel;
using System.Configuration.Install;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.InterQ.Server
{
    [RunInstaller(true)]
    public partial class InterQInstaller : Installer
    {
        public InterQInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs ev)
        {
            try
            {
                //serviceController1.Start();
            }
            catch (Exception e)
            {
                T.EXCEPTION(e);
            }
        }

        private void serviceProcessInstaller1_BeforeUninstall(object sender, InstallEventArgs ev)
        {
            try
            {
               // serviceController1.Stop();
            }
            catch (Exception e)
            {
                T.EXCEPTION(e);
            }
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}