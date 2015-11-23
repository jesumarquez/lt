using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public IntegrationService IntegrationService { get; set; }

        public WsSosService.Service WebService;
        public System.Timers.Timer Timer4Services;
        public List<Novelty> Novelties;
        public const int Time = 1;

        public void StartService()
        {
            IntegrationService =new IntegrationService();
            Novelties = new List<Novelty>();
            WebService = new WsSosService.Service();

            //Timer4Services = new System.Timers.Timer(Time * 60000);
            Timer4Services = new System.Timers.Timer(10000);
            Timer4Services.Elapsed += OnTimedEvent;
            Timer4Services.AutoReset = true;
            Timer4Services.Enabled = true;           

            FindAlerts();
        }

        private void Run()
        {
            Timer4Services.Stop();

            Logger.Info("Searching for a new alarm in S.O.S. service...");

            var webservice = new WsSosService.Service();
            var response = webservice.ObtenerAlertas();

            if (response != "")
            {
                var alerts = GetNovelties(response);
                Logger.InfoFormat("Found {0} services", alerts.Count);
            }
            MostrarInformacionAlerta();
            IntegrationService.NewServices(Novelties);

            Timer4Services.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            FindAlerts();
        }

        private void FindAlerts()
        {
            try
            {
                new Thread(Run).Start();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("StartService ex : {0}", e.ToString());
                //Stop();
            }
        }

        private List<Novelty> GetNovelties(string response)
        {          
            var alerts = response.Split('#');
            foreach (var alert in alerts)
            {
                if (alert=="") continue;
                var novelty = TranslateFrameToClass.ParseFrame(alert);
                Novelties.Add(novelty);
            }

            return Novelties;
        }
        
        private void MostrarInformacionAlerta()
        {
            var salida = new StringBuilder();

            foreach (var novelty in Novelties)
            {
                salida.AppendLine("servicio:" + novelty.NumeroServicio);
                salida.AppendLine("diagnostico:" + novelty.Diagnostico);
                salida.AppendLine("prioridad:" + novelty.Prioridad);
                salida.AppendLine("hora:" + novelty.HoraServicio);
                salida.AppendLine("cobro:" + novelty.CobroAdicional);
                salida.AppendLine("estado:" + novelty.Estado);
                salida.AppendLine("operador:" + novelty.Operador);
                salida.AppendLine("patente:" + novelty.Vehiculo.Patente);
                salida.AppendLine("ubicacion:" + novelty.Origen.Referencia);
                salida.AppendLine("------------------------------------");
            }
            Logger.Info(salida.ToString());
        }
    }
}