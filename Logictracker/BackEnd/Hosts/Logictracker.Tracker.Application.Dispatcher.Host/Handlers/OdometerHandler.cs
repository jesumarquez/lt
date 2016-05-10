using System;
using Logictracker.AVL.Messages;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

namespace Logictracker.Tracker.Application.Dispatcher.Host.Handlers
{
    class OdometerHandler : ChainHandler, IMessageHandler<Position>
    {
        public override HandleResults HandleMessage(IMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var p = message as Position;

            return p != null ? HandleMessage(p) : HandleResults.BreakSuccess;
        }

        public HandleResults HandleMessage(Position position)
        {
            if (position == null) throw new ArgumentNullException("position");
   
            return HandleResults.Success;
        }

        public OdometerHandler(ChainHandler nextHandler)
            : base(nextHandler)
        {
        }

    }
}
