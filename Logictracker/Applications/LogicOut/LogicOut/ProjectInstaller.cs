﻿using System.ComponentModel;
using System.Configuration.Install;

namespace LogicOut
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
