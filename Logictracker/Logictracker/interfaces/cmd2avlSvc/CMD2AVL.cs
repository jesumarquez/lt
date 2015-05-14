using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using command_data;

namespace cmd2avlSvc
{
    partial class CMD2AVL : ServiceBase
    {
        TicketsWatcher tw = null;
        Thread t = null;
        public CMD2AVL()
        {
            InitializeComponent();
            
            tw = new TicketsWatcher();
            t = new Thread(new ThreadStart(this.ThreadStart));
            t.Start();
        }

        public void ThreadStart()
        {
            tw.Initialize();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
