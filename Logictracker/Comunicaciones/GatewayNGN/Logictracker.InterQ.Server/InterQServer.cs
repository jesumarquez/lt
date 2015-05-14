#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Urbetrack.InterQ.Core.Transport;
using Urbetrack.Messaging.Batch;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.InterQ.Server
{
    public static class InterQServer
    {
        private static readonly Dictionary<string, QueueBatchConsumer> dispatchers = new Dictionary<string, QueueBatchConsumer>();
        private static TcpInterQueueServer server;
        
        public static string[] Queues()
        {
            return dispatchers.Keys.ToArray();
        }

        public static void Start()
        {
            var queues_directive = Config.GetConfigurationString("interqueue", "queues", "");
            server = new TcpInterQueueServer(new IPEndPoint(IPAddress.Any, Config.GetConfigurationInt("interqueue", "listen_port", 7532)),0,false);
            if (String.IsNullOrEmpty(queues_directive)) return;
            
            var queues = queues_directive.Split(",".ToCharArray());
            foreach (var queue in queues)
            {
                // validamos si la cola esta deshabilitada.
                if (Config.GetConfigurationBool(queue, "disabled", false)) continue;
                // creo e inicializo el nodo local, quien consume la cola.
                var local_queue = Config.GetConfigurationString(queue, "local_queue", "");
                if (String.IsNullOrEmpty(local_queue))
                {
                    T.ERROR("Queue: {0} falta setear el nombre local.", queue);
                    continue;
                }
                var label_reformat_expression = Config.GetConfigurationString(queue, "label_reformat_expression", "");
                var remote_queue = Config.GetConfigurationString(queue, "remote_queue", local_queue);
                var remote_host = Config.GetConfigurationIPAddress(queue, "remote_host", IPAddress.None);
                if (remote_host == IPAddress.None)
                {
                    T.ERROR("Queue: {0} falta setear el host remoto.", queue);
                    continue;
                }
                var remote_port = Config.GetConfigurationInt(queue, "remote_port", 7532);
                var remote_ep = new IPEndPoint(remote_host, remote_port);

                var tcpcli = new TcpInterQueueClient_V1_1(remote_ep, remote_queue);
                var dispatcher = new QueueBatchConsumer(local_queue, tcpcli, label_reformat_expression);
                dispatchers.Add(queue, dispatcher);
                dispatcher.Start();
            }
        }

        public static void Stop()
        {
            server.Close();
            foreach (var dispatcher in dispatchers.Values)
            {
                dispatcher.Stop();
            }
        }
    }
}
