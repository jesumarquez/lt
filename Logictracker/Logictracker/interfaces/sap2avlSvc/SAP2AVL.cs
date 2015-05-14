using System;
using System.Diagnostics;
using System.ServiceProcess;
using etao.sap2avl;

namespace sap2avlSvc
{
    partial class SAP2AVL : ServiceBase
    {
        Watcher w;
        EventLog el;
        public SAP2AVL()
        {
            try
            {
                InitializeComponent();
                if (!EventLog.SourceExists("SAP2AVL"))
                {
                    EventLog.CreateEventSource("SAP2AVL", "Application");
                }
                el = new EventLog();
                el.Source = "SAP2AVL";
                w = new Watcher(el);
            }
            catch (Exception e)
            {
                el.WriteEntry(String.Format("Excepcion al iniciar el monitor de archivos: {0}", e));
            }
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public Watcher W
        {
            get { return w; }
        }
    }
}
