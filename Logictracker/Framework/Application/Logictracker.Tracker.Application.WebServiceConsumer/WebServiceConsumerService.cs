using Logictracker.Messaging.WebConsumer;
using Spring.Messaging.Core;

namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class WebServiceConsumerService : IWebServiceConsumerService
    {
        public MessageQueueTemplate MessageQueueTemplate { get; set; }

        public INoveltyCommand GenerateNoveltyCommand()
        {
            return new NoveltyCommand();
        }

        public void SendCommand(INoveltyCommand command)
        {
            MessageQueueTemplate.ConvertAndSend(command);
        }


    }
}
