using System.Collections.Generic;
using System.Threading;
using System.Timers;
using log4net;
using Logictracker.Model;
using Logictracker.Tracker.Parser.Spi;
using Spring.Aspects;
using Spring.Messaging.Core;

namespace Logictracker.Tracker.Application.Parser.Host
{
    class GatewayParserClientController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GatewayParserClientController));

        public List<IParserServer> ProtocolServers { get; set; }
        public MessageQueueTemplate TrackMessageQueueTemplate { get; set; }
        public System.Timers.Timer RestarterTimer;
        public int RestartTime { get; set; }

       // private Thread executeThread;
        private IParserServer parser;

        public void Start()
        {
            StartProtocolServers();
            RestarterTimer = new System.Timers.Timer(RestartTime*1000);
            RestarterTimer.Elapsed += OnTimedEvent;
            RestarterTimer.AutoReset = true;
            RestarterTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Stop();
            Logger.Info("Restarting...");
            
            Thread.Sleep(2000);

            StartProtocolServers();
        }

        public void Stop()
        {
            //Logger.Warn("Stopping protocol servers...");

            //foreach (IParserServer protocolServer in ProtocolServers)
            //{
            //    Logger.InfoFormat("Stopping protocol server {0}", protocolServer.Name);
            //    protocolServer.Stop();
            //}

            //Logger.Info("All protocol servers stopped");

            Logger.Info("Parser caesat stopped");
            parser.Stop();
        }

        private void StartProtocolServers()
        {
            //Logger.Info("Starting protocols...");
            //foreach (IParserServer protocolServer in ProtocolServers)
            //{
            //    protocolServer.Callback = SendtrackReport;
            //    Logger.InfoFormat("Starting protocol server {0} ", protocolServer.Name);
            //    protocolServer.Start();
            //}
            //Logger.Info("All protocol servers started");

            Logger.Info("Parser caesat started");
            parser = ProtocolServers[0];
            parser.Callback = SendtrackReport;
            parser.Start();

        }

        private void SendtrackReport(IMessage msg)
        {
            if (msg != null)
            {
                Logger.InfoFormat("Message sent {0} ", msg.DeviceId);
                TrackMessageQueueTemplate.ConvertAndSend(msg);
            }
            Logger.InfoFormat("SendtrackReport");
        }
    }
}
