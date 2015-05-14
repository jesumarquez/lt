using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Urbetrack.Toolkit;

namespace TestDDL
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process.InitAppDomain();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
