#region Usings

using System;
using System.Messaging;
using System.Windows.Forms;
using Urbetrack.Toolkit;

#endregion

namespace MessageQueueTool
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
            
           /* foreach(var q in MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName))
            {
                if (q.QueueName.StartsWith(@"private$\dev_"))
                {
                    T.TRACE("DEVICE {0}", q.QueueName);
                    MessageQueue.Delete(@".\" + q.QueueName);
                } else
                {
                    T.TRACE("QUEUE  {0}", q.QueueName);
                }
            }*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new QueueDocument());
            //Application.Run(new MainForm());
        }
    }
}
