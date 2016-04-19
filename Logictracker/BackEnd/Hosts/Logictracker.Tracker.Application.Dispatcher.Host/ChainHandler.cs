using System;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

namespace Logictracker.Tracker.Application.Dispatcher.Host
{
    public abstract class ChainHandler : IMessageHandler<IMessage>
    {
        protected ChainHandler(ChainHandler nextHandler)
        {
            NextHandler = nextHandler;
        }

        protected ChainHandler NextHandler { get; set; }

        public HandleResults ProcessMessage(IMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var result = HandleMessage(message);

            return NextHandler != null ? NextHandler.ProcessMessage(message) : result;
        }

        public abstract HandleResults HandleMessage(IMessage message);
    }
}