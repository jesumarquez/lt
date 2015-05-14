using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace sap2avlSvc
{
    [RunInstaller(true)]
    public partial class SAP2AVLInstaller : Installer
    {
        public SAP2AVLInstaller()
        {
            InitializeComponent();
        }
    }
}