using System;
using System.Windows.Forms;
using Urbetrack.Mobile.Install;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.MessageQueueTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            T.Initialize("mqtool");
            MSMQ.Install();
            Application.Run(new QueueLoopback());
        }
    }
}