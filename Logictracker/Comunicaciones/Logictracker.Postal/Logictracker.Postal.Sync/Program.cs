#region Usings

using System;
using System.Windows.Forms;
using Urbetrack.Postal.Sync.Forms;

#endregion

namespace Urbetrack.Postal.Sync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
