using System;
using System.ServiceProcess;

namespace sap2avlSvc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                ServiceBase[] services;
                services = new ServiceBase[] {new SAP2AVL()};
                ServiceBase.Run(services);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Excepcion interna {0}", e));
            }
        }
    }
}
