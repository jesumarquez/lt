using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Layers.MessageQueue.Implementations
{
    public interface IMessageQueueImplementation : IDisposable
    {
        bool LoadResources();
        Boolean Send(Message msgsnd);
        Boolean Send(object msgsnd, MessageQueueTransactionType transactionType);
        int GetCount();


        Message Receive(TimeSpan timeout);

        Message Receive(MessageQueueTransactionType messageQueueTransactionType);

        IMessageQueue MessageQueue { get; set; }

        Message EndReceive(IAsyncResult ar);

        Message EndPeek(IAsyncResult ar);

        bool CountMore(int minMessagesToSleep);

        IAsyncResult BeginPeek(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessagePeeked);

        IAsyncResult BeginReceive(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessageReceived);

        void Close();
    }

}
