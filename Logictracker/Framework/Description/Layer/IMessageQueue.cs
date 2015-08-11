#region Usings

using System;
using System.Messaging;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Layers.MessageQueue.Implementations;
using Logictracker.Messaging;
using Logictracker.Model;

#endregion

namespace Logictracker.Layers.MessageQueue
{
    /// <summary>
    /// Implementa la funciones de acceso a la Message Queue de Windows.
    /// </summary>
    [FrameworkElement(XName = "MessageQueue", IsContainer = false)]
    public class IMessageQueue : FrameworkElement, IDisposable
    {
        private IMessageQueueImplementation implementation;
        #region Attributes

        [ElementAttribute(XName = "Formatter", DefaultValue = "BinaryFormatter", LoadOrder = 0)]
        public String Formatter { get; set; }

        /// <summary>
        /// Obtiene o establece la capacidad transaccional de la cola.
        /// </summary>
        [ElementAttribute(XName = "Transactional", DefaultValue = true, LoadOrder = 1)]
        public bool Transactional { get; set; }

        /// <summary>
        /// Obtiene o establece si los mensajes en cola son persistentes
        /// frente a un shutdown o reboot del sistema.
        /// </summary>
        [ElementAttribute(XName = "Recoverable", DefaultValue = true, LoadOrder = 2)]
        public bool Recoverable { get; set; }

        /// <summary>
        /// Obtiene o Establece el Grupo de usuarios que al crear la cola 
        /// obtendra control total.
        /// Por defecto utiliza el Grupo "Users" o "Usuarios".
        /// </summary>
        [ElementAttribute(XName = "OwnerGroup", DefaultValue = null, LoadOrder = 3)]
        public string OwnerGroup { get; set; }

        /// <summary>
        /// Obtiene o Establece la ruta de la cola asociada.
        /// </summary>
        [ElementAttribute(XName = "Path", IsRequired = true, LoadOrder = 4)]
        public string QueueName { get; set; }

        /// <summary>
        /// Obtiene o Establece la ruta de la cola asociada.
        /// </summary>
        [ElementAttribute(XName = "QueueType", IsRequired = false, DefaultValue = "msmq", LoadOrder = 5)]
        public string QueueType { get; set; }
        #endregion

        #region Constructors

        public IMessageQueue()
        {
        }

        public IMessageQueue(String name)
        {
            QueueName = name;
            Recoverable = String.IsNullOrEmpty(Config.Queue.QueueRecoverable) || Convert.ToBoolean(Config.Queue.QueueRecoverable);
            Transactional = String.IsNullOrEmpty(Config.Queue.QueueTransactional) || Convert.ToBoolean(Config.Queue.QueueTransactional);
            Formatter = !String.IsNullOrEmpty(Config.Queue.QueueFormater) ? Config.Queue.QueueFormater : "BinaryFormatter";
            QueueType = !String.IsNullOrEmpty(Config.Queue.QueueType) ? Config.Queue.QueueType : "msmq";
        }

        #endregion

        #region FrameworkElement

        public override bool LoadResources()
        {
            try
            {
                switch (QueueType.ToLower())
                {
                    case "msmq":
                        //QueueType = "Logictracker.Messaging.MsmqQueue.MessageQueueMsmq , Logictracker.Messaging.MsmqQueue, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                        implementation = new MessageQueueMsmq();
                        break;
                    case "rabbitmq":
                        QueueType = "Logictracker.Messaging.RabbitmqQueue.MessageQueueRabbitMq,Logictracker.Messaging.RabbitmqQueue";
                        implementation = Activator.CreateInstance(Type.GetType(QueueType, true)) as IMessageQueueImplementation;
                        break;
                }
                if (implementation == null) throw new ArgumentException("QueueType invalid", "QueueType");

                implementation.MessageQueue = this;
                
                return implementation.LoadResources();
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(IMessageQueue).FullName, e, String.Format("Error inicializando la cola '{0}'", QueueName));
                //        Handler = null;
                return false;
            }
        }

        #endregion

        #region Public Members

        public Boolean Send(Object message)
        {

            using (var msgsnd = new Message(message))
                Send(msgsnd);

            return true;
        }

        public Boolean Send(Object message, String label)
        {
            using (var msgsnd = new Message(message) { Label = label })
                Send(msgsnd);
            return true;
        }

        public Boolean Send(Message msgsnd)
        {
            return implementation.Send(msgsnd);
        }

        public Boolean Send(object msgsnd, MessageQueueTransactionType transactionType)
        {
            return implementation.Send(msgsnd, transactionType);
        }

        public Message Receive(TimeSpan timeout)
        {
            return implementation.Receive(timeout);
        }

        public void SendStOnline()
        {
            const string msg = "ST,ONLINE";
            using (
                var msgsnd = new Message(msg)
                {
                    Label = msg,
                    Priority = MessagePriority.Highest,
                    TimeToBeReceived = new TimeSpan(0, 5, 0),
                })
            {
                Send(msgsnd);
            }
        }

        public int GetCount()
        {
            return implementation.GetCount();
        }



        #endregion



        public Message Receive(MessageQueueTransactionType messageQueueTransactionType)
        {
            return implementation.Receive(messageQueueTransactionType);
        }

        public bool CountMore(int minMessagesToSleep)
        {
            return implementation.CountMore(minMessagesToSleep);
        }

        public Message EndPeek(IAsyncResult ar)
        {
            return implementation.EndPeek(ar);
        }

        public void Dispose()
        {
            implementation.Dispose();
        }

        void IDisposable.Dispose()
        {
            implementation.Dispose();
        }

        public void Close()
        {
            implementation.Close();

        }

        public System.Messaging.Message EndReceive(IAsyncResult ar)
        {
            return implementation.EndReceive(ar);
        }

        public IAsyncResult BeginReceive(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessageReceived)
        {
            return implementation.BeginReceive(AsyncTimeout, stateObject, MessageReceived);
        }

        public IAsyncResult BeginPeek(TimeSpan AsyncTimeout, object stateObject, AsyncCallback MessagePeeked)
        {
            return implementation.BeginPeek(AsyncTimeout, stateObject, MessagePeeked);
        }
    }
}
