#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Win32;
using Urbetrack.Comm.Tools;
using Urbetrack.Messaging;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Comm.PumpControl
{
    public class ServerHandler : IService, IEnvironmentMonitor
    {
        private readonly Thread worker;
        private string cluster_name;
        private bool running;
        private int join_timeout;
        private int pool_interval;
        public string Hostname;
        private string gateway_id;
        public int Port;
        private int keep_alive_ticks;
        private const int offline_keep_alive_ticks = 10;
        private readonly Queue<string> query_queue = new Queue<string>();
        private readonly BinaryMessageQueue cola_envios = new BinaryMessageQueue();

        public ServerHandler()
        {
            keep_alive_ticks = offline_keep_alive_ticks;
            worker = new Thread(WorkerProc);
        }

        private void PushUp(string label, string data)
        {
            T.TRACE(0, "PUSH:{0}:{1}", label, data);
            cola_envios.Push(String.Format("{0};P;{1};{2}", gateway_id, DateTime.Now, label), data);
        }

        bool dc_activo = false;
        private void WorkerProc()
        {
            PushUp("SERVICE", "STARTED");
            
            while(running)
            {
                var logs_state = PumpProtocol.QuerySignle(Hostname, Port, "@M1.0.0.0|L|2@F");
                if (ProcessDCState(logs_state)) goto WorkSleep;
                if (logs_state.Contains("@D")) 
                    PushUp("FROMLOG", logs_state);
                var reply = PumpProtocol.QuerySignle(Hostname, Port, "@M1.0.0.0|E|2@F");
                if (ProcessDCState(reply)) goto WorkSleep;
                PushUp("SAMPLE", reply);
            WorkSleep:
                Thread.Sleep(pool_interval);
            }
            PushUp("SERVICE", "STOPED");
        }

        public bool ProcessDCState(string reply)
        {
            if (string.IsNullOrEmpty(reply))
            {
                if (dc_activo)
                {
                    PushUp("DC", "OFFLINE");
                    dc_activo = false;
                }
                else
                {
                    keep_alive_ticks--;
                    if (keep_alive_ticks == 0)
                    {
                        PushUp("SERVICE", "KEEPALIVE");
                        keep_alive_ticks = offline_keep_alive_ticks;
                    }
                }
                return true;
            }
            if (dc_activo) return false;
            PushUp("DC", "ONLINE");
            dc_activo = true;
            keep_alive_ticks = offline_keep_alive_ticks;
            return false;
        }

        public void QueryStates()
        {
            lock (query_queue)
            {
                query_queue.Enqueue("@M1.0.0.0|E|2@F");
            }
        }

        public void QueryLogSize(char type)
        {
            lock (query_queue)
            {
                query_queue.Enqueue("@M1.0.0.0|L|7@D" + type + "|0@F");
            }
        }

        public void Start(string clusterName)
        {
            cluster_name = clusterName;
            running = true;
            gateway_id = Config.GetConfigurationString(cluster_name, "gateway_id", "394");
            join_timeout = Config.GetConfigurationInt(cluster_name, "join_timeout", 5000);
            pool_interval = Config.GetConfigurationInt(cluster_name, "pool_interval", 60000);
            Hostname = Config.GetConfigurationString(cluster_name, "dc_hostname", "192.168.10.18");
            Port = Config.GetConfigurationInt(cluster_name, "dc_tcpport", 1024);
            cola_envios.Name = Config.GetConfigurationString(cluster_name, "destination_queue", "combustible");
            cola_envios.DefaultPropertiesToSend.Recoverable = true;
            // la siguiente sentencia, condiciona al InterQ el cual no reescribe el label.
            cola_envios.DefaultPropertiesToSend.AppSpecific = 0x23570001;
            // limites minimos.
            if (pool_interval < 1000) pool_interval = 1000;

            worker.Start();
        }
        
        public void Stop()
        {
            running = false;
            if (!worker.Join(join_timeout))
            {
                worker.Abort();
            }
        }

        public bool Running
        {
            get { return running; }
        }

        public void OnCrashRecovery()
        {
            PushUp("SERVICE","CRASH_RECOVERY");
        }

        public void OnProcessEnding(Process.ExitCodes code)
        {
            PushUp("SERVICE", "ENDING;" + code.Description());
        }

        public void OnSystemShutdown(SessionEndReasons reason)
        {
            PushUp("SERVICE", reason == SessionEndReasons.SystemShutdown ? "SYSTEM_SHUTDOWN" : "SESSION_LOGOFF");
        }
    }
}