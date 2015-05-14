#region Usings

using System;
using System.Messaging;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.MsmqMessaging
{
    public class BinaryMessageQueue
    {
        private string name;
        private MessageQueue queue;
        private string queuepath;

        public bool HaveMessages
        {
            get
            {
                if (queue == null) return false;
                try
                {
                    using (var m = queue.Peek(new TimeSpan(0)))
                        return m == null ? false : true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public DefaultPropertiesToSend DefaultPropertiesToSend
        {
            get { return queue.DefaultPropertiesToSend;  }
            set { queue.DefaultPropertiesToSend = value; }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                queuepath = name.StartsWith("FormatName") ? name : String.Format(".\\Private$\\{0}", name);

                try
                {
                    queue = MessageQueue.Exists(queuepath) ? new MessageQueue(queuepath) : MessageQueue.Create(queuepath);
					queue.SetPermissions(Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);
                    queue.MessageReadPropertyFilter.SetAll();
                    queue.Formatter = new BinaryMessageFormatter();
                    queue.DefaultPropertiesToSend.Recoverable = true;
                }
                catch (MessageQueueException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    throw;
                }
            }
        }

        public void Push(string label, object data)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            queue.Send(data, label);
        }

        public Message Pop()
        {
            return Pop(TimeSpan.FromMilliseconds(50));
        }

        public Message Pop(TimeSpan timeout)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            try
            {
                return queue.Receive(timeout);
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) return null;
                throw;
            }
        }

        public Message Peek()
        {
            return Peek(TimeSpan.FromMilliseconds(50));
        }

        public Message Peek(TimeSpan timeout)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            try
            {
                return queue.Peek(timeout);
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout) return null;
                throw;
            }
        }

        public void Close()
        {
            queue.Close();
            queue = null;
        }
    }
}