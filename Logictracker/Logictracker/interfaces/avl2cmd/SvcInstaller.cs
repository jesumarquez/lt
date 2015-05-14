using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace avl2cmd
{
    [RunInstaller(true)]
    public partial class SvcInstaller : Installer
    {
        public SvcInstaller()
        {
            InitializeComponent();
        }
    }
}