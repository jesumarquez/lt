using System;

using System.Collections.Generic;
using System.Text;
using System.Threading;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.Gateway
{
    public static class Network
    {
        private static Thread batch;
        public static void Initialize()
        {
            Connected = false;
            batch = new Thread(ThreadProc);
            T.INFO("NETWORK: lanzando hilo.");
            batch.Start();
        }

        public static bool Connected { get; private set; }

        private static void ThreadProc()
        {
            
        }

      

        
    }
}
