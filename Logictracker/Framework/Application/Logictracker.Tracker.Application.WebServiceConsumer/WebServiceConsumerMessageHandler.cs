using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Logictracker.Messaging.WebConsumer;

namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class WebServiceConsumerMessageHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebServiceConsumerMessageHandler));

        public static IWebServiceConsumerService WebServiceConsumerService { get; set; }

        public void HandleMessage(INoveltyCommand command)
        {
            Logger.DebugFormat("Received message command of type {0} ", command.GetType());

            try
            {
                //NHibernateHelper.CreateSession();
                Logger.Debug("Nhibernate session created");

                ProcessNoveltyCommand(command);

                //NHibernateHelper.CloseSession();
                Logger.Debug("Nhibernate close session");

            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }

        private void ProcessNoveltyCommand(INoveltyCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
