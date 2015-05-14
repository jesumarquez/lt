using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;

namespace cmd2avlSvc
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ServiceBase[] services;
            services = new ServiceBase[] { new CMD2AVL() };
            ServiceBase.Run(services);
        }
    }
}
