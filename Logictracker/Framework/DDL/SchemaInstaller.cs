#region Usings

using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

#endregion

namespace Logictracker.DDL
{
    [RunInstaller(true)]
    public partial class SchemaInstaller : Installer
    {
        public SchemaInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            Toolkit.InstallDatabase(Context.Parameters["catalog"], Context.Parameters["embedded_script"]);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

    }
}
