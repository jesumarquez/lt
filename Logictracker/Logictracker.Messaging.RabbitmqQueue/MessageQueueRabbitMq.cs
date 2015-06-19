using Logictracker.Layers.MessageQueue.Implementations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logictracker.Messaging.RabbitmqQueue
{
    public class MessageQueueRabbitMq : IMessageQueueImplementation
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        private QueueingBasicConsumer consumer;

        private QueueingBasicConsumer Consumer
        {
            get
            {
                if (consumer == null)
                {
                    consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("prueba", false, consumer);
                }
                return consumer;
            }
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }
        public bool LoadResources()
        {
            factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "iddqder" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare("prueba", false, false, false, null);
            return true;

        }

        public bool Send(System.Messaging.Message msgsnd)
        {
            return Send(msgsnd.Body, System.Messaging.MessageQueueTransactionType.None);
        }

        public bool Send(object msgsnd, System.Messaging.MessageQueueTransactionType transactionType)
        {
            var stm = new MemoryStream();
            new BinaryFormatter().Serialize(stm, msgsnd);
            channel.BasicPublish("", "prueba", null, stm.ToArray());
            return true;
        }

        public int GetCount()
        {
            var qOk = channel.QueueDeclarePassive("prueba");
            return (int)qOk.MessageCount;
        }

        public System.Messaging.Message Receive(TimeSpan timeout)
        {
            BasicDeliverEventArgs rbMsg = null;
            if (consumer.Queue.Dequeue((int)timeout.TotalMilliseconds, out rbMsg))
            {
                var memStr = new MemoryStream(rbMsg.Body);
                var msg = new System.Messaging.Message(new BinaryFormatter().Deserialize(memStr));
                return msg;
            }
            return null;
        }

        public System.Messaging.Message Receive(System.Messaging.MessageQueueTransactionType messageQueueTransactionType)
        {
            return Receive(TimeSpan.MaxValue);
        }

        public Layers.MessageQueue.IMessageQueue MessageQueue
        {
            get;
            set;
        }

        public System.Messaging.Message EndReceive(IAsyncResult ar)
        {
            return ((Task<System.Messaging.Message>)ar.AsyncState).Result;
        }

        public System.Messaging.Message EndPeek(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        public bool CountMore(int top)
        {
            return GetCount() > top;
        }

        public IAsyncResult BeginPeek(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessagePeeked)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceive(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessageReceived)
        {
            var tsk = Task.Factory.StartNew((s) => Receive(AsyncTimeout), stateObject);
            return tsk.ToApm(MessageReceived, stateObject);
        }

        public void Close()
        {
            if (consumer != null)
                consumer.Queue.Close();
            channel.Close();
            connection.Close();
        }


    }

    internal static class TaskExtensions
    {
        internal static Task<TResult> ToApm<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            if (task.AsyncState == state)
            {
                if (callback != null)
                {
                    task.ContinueWith(delegate { callback(task); },
                        CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
                }
                return task;
            }

            var tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else tcs.TrySetResult(task.Result);

                if (callback != null) callback(tcs.Task);

            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
            return tcs.Task;
        }
    }
}
