using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.ServiceProcess;

namespace avl2cmd
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "/game")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                ServiceBase[] services;
                services = new ServiceBase[] { new AVL2CMD() };
                ServiceBase.Run(services);
            }
        }
    }
}