using System;
using System.Messaging;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers.MessageQueue;
using Logictracker.Layers.MessageQueue.Implementations;

namespace Logictracker.Messaging
{

    public class MessageQueueMsmq : IMessageQueueImplementation
    {
        internal MessageQueue Handler { get; private set; }

        public bool LoadResources()
        {

            try
            {
                if (System.Messaging.MessageQueue.Exists(MessageQueue.QueueName))
                {
                    Handler = new MessageQueue(MessageQueue.QueueName);
                    if (MessageQueue.Transactional != Handler.Transactional)
                    {
                        STrace.Error(typeof(IMessageQueue).FullName, String.Format("MSMQ '{0}' - 'Transactional' no coincide.", MessageQueue.QueueName));
                        MessageQueue.Transactional = Handler.Transactional;
                    }
                    /*if (Recoverable != Handler.DefaultPropertiesToSend.Recoverable)
                    {
                        STrace.Debug(typeof(IMessageQueue).FullName, String.Format("MSMQ '{0}' - 'Recoverable' no coincide.", QueueName));
                        Recoverable = Handler.DefaultPropertiesToSend.Recoverable;
                    }//*/
                }
                else
                {
                    Handler = System.Messaging.MessageQueue.Create(MessageQueue.QueueName, MessageQueue.Transactional);
                    Handler.DefaultPropertiesToSend.Recoverable = MessageQueue.Recoverable;
                }

                Handler.SetPermissions(MessageQueue.OwnerGroup ?? Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);

                var formatter = (MessageQueue.Formatter ?? "BINARY").ToUpper();
                if (formatter.StartsWith("BINARY"))
                {
                    Handler.Formatter = new BinaryMessageFormatter();
                }
                else if (formatter.StartsWith("XML"))
                {
                    Handler.Formatter = new XmlMessageFormatter(new[] { typeof(String) });
                }
                else if (formatter.StartsWith("ACTIVEX"))
                {
                    Handler.Formatter = new ActiveXMessageFormatter();
                }
                return true;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(IMessageQueue).FullName, e, String.Format("Error inicializando la cola '{0}'", MessageQueue.QueueName));
                Handler = null;
                return false;
            }
        }

        public Boolean Send(Message msgsnd)
        {
            if (Handler == null) return false;
            msgsnd.Formatter = Handler.Formatter;
            msgsnd.Recoverable = MessageQueue.Recoverable;

            Handler.Send(msgsnd, MessageQueue.Transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
            return true;
        }

        public Message Receive(TimeSpan timeout)
        {
            return Handler.Receive(timeout);
        }

        public Boolean Send(object msgsnd, MessageQueueTransactionType transactionType)
        {
            Handler.Send(msgsnd, transactionType);
            return true;
        }

        private static Message PeekWithoutTimeout(MessageQueue q, Cursor cursor, PeekAction action)
        {
            Message ret = null;
            try
            {
                ret = q.Peek(new TimeSpan(1), cursor, action);
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    throw;
                }
            }
            return ret;
        }

        public int GetCount()
        {
            var count = 0;
            var cursor = Handler.CreateCursor();

            if (PeekWithoutTimeout(Handler, cursor, PeekAction.Current) == null) return count;
            count = 1;
            while ((PeekWithoutTimeout(Handler, cursor, PeekAction.Next)) != null)
            {
                count++;
            }
            return count;
        }

        bool IMessageQueueImplementation.CountMore(int top)
        {

            try
            {
                var cnt = 0;
                using (var enumerator = Handler.GetMessageEnumerator2())
                {
                    while (enumerator.MoveNext() && cnt <= top) cnt++;
                }
                return cnt > top;
            }
            catch (MessageQueueException)
            {
                return false;
            }
        }

        public IMessageQueue MessageQueue { get; set; }

        void IMessageQueueImplementation.Close()
        {
            Handler.Close();
        }

        void IDisposable.Dispose()
        {
            Handler.Dispose();
        }

        public Message EndReceive(IAsyncResult ar)
        {
            return Handler.EndReceive(ar);
        }

        internal void Dispose()
        {
            Handler.Dispose();
        }
        IAsyncResult IMessageQueueImplementation.BeginReceive(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessageReceived)
        {
            return Handler.BeginReceive(AsyncTimeout, null, MessageReceived);
        }


        IAsyncResult IMessageQueueImplementation.BeginPeek(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessagePeeked)
        {
            return Handler.BeginReceive(AsyncTimeout, null, MessagePeeked);
        }

        Message IMessageQueueImplementation.EndPeek(IAsyncResult ar)
        {
            return Handler.EndPeek(ar);
        }


        Message IMessageQueueImplementation.Receive(MessageQueueTransactionType messageQueueTransactionType)
        {
            return Handler.Receive(messageQueueTransactionType);
        }
    }
}
