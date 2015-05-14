using System.ServiceProcess;
using Logictracker.Process.Import.Client;

namespace LogicLink
{
    public partial class Service1 : ServiceBase
    {
        protected SyncService SyncService = new SyncService();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SyncService.Start();
        }

        protected override void OnStop()
        {
            SyncService.Stop();
        }
    }
}
