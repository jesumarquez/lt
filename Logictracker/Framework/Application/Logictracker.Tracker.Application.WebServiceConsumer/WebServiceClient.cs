using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using log4net;
using log4net.Repository.Hierarchy;
using Logictracker.Tracker.Services;
using Spring.Messaging.Core;

namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class WebServiceClient : IWebServiceClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebServiceClient));
        public MessageQueueTemplate GarminQueueTemplate { get; set; }

        public WsSosService.Service WebService;
        public System.Timers.Timer Timer4Services;
        public List<Novelty> Novelties;
        public const int Time = 1;

        public void StartService()
        {
            //Timer4Services = new System.Timers.Timer(Time * 60000);
            Timer4Services = new System.Timers.Timer(10000);
            Timer4Services.Elapsed += OnTimedEvent;
            Timer4Services.AutoReset = true;
            Timer4Services.Enabled = true;

            WebService = new WsSosService.Service();
            //try
            //{
            //    new Thread(Run).Start();
            //}
            //catch (Exception e)
            //{
            //    Logger.ErrorFormat("StartService ex : {0}", e.ToString());
            //    //Stop();
            //}
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Logger.Info("Searching for a new alarm in S.O.S. service...");
            FindAlerts();
        }

        private void FindAlerts()
        {
            var webservice = new WsSosService.Service();

            var response = webservice.ObtenerAlertas();

            if (response != "")
            {
                var alerts = GetNovelties(response);

                if (alerts.Count > 0)
                    Timer4Services.Stop();

                Logger.InfoFormat("Found {0} services", alerts.Count);

                MostrarInformacionAlerta();
            }
        }

        private List<Novelty> GetNovelties(string response)
        {          
            var alerts = response.Split('#');
            foreach (var alert in alerts)
            {
                var novelty = TranslateFrameToClass.ParseFrame(alert);
                Novelties.Add(novelty);
            }

            return Novelties;
        }


        private void MostrarInformacionAlerta()
        {
            foreach (var novelty in Novelties)
            {
                Logger.Info(novelty.NumeroServicio);
                Logger.Info(novelty.Diagnostico);
                Logger.Info(novelty.Prioridad);
                Logger.Info(novelty.HoraServicio);
                Logger.Info(novelty.CobroAdicional);
                Logger.Info(novelty.Estado);
                Logger.Info(novelty.Vehiculo);
                Logger.Info(novelty.Origen);
                Logger.Info(novelty.Destino);
            }
        }
    }
}