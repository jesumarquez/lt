using System.ComponentModel;
using System.Configuration.Install;

namespace Logictracker.Process.Import.Client
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
