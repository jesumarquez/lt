using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace cmd2avlSvc
{
    [RunInstaller(true)]
    public partial class SvcInstaller : Installer
    {
        public SvcInstaller()
        {
            InitializeComponent();
        }

        private void SvcInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

    }
}