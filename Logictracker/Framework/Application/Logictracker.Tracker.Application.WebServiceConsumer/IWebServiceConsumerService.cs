using Logictracker.Messaging.WebConsumer;

namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public interface IWebServiceConsumerService
    {
        INoveltyCommand GenerateNoveltyCommand();
        void SendCommand(INoveltyCommand generateNoveltyCommand);
    }
}