using System;
using System.Windows.Forms;
using Urbetrack.DDL;

namespace TestDDL
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var catalog = "testddl";
            try  { Toolkit.DropDatabase(Toolkit.GetDefaultInstance(), catalog);} catch {}
            Toolkit.InstallDatabase(catalog,"gateway_movil.sql");
        }
    }
}
