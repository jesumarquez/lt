#region Usings

using System.ServiceProcess;

#endregion

namespace Urbetrack.InterQ.Server
{
    partial class InterQService : ServiceBase
    {
        public InterQService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            InterQServer.Start();
        }

        protected override void OnStop()
        {
            InterQServer.Stop();
        }
    }
}