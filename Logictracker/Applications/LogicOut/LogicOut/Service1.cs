using System.ServiceProcess;
using LogicOut.Core;

namespace LogicOut
{
    public partial class Service1 : ServiceBase
    {
        protected OutService Service;
        public Service1()
        {
            InitializeComponent();
            Service = new OutService();
        }

        protected override void OnStart(string[] args)
        {
            Service.OnStart(args);
        }

        protected override void OnStop()
        {
            Service.OnStop();
        }
    }
}
