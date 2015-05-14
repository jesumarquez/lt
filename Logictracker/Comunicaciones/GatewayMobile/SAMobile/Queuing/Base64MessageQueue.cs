using System;
using System.Messaging;
using System.Threading;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.Comm.Queuing
{
    [CLSCompliant(true)]
    public class Base64MessageQueue 
    {
        public class EmptyMessageQueueException : Exception
        {
        }

        public class InvalidMessageQueueException : Exception
        {
        }

        private string name;
        private string queuepath;
        private MessageQueue queue;

        public delegate bool IncommingMessageHandler(Base64MessageQueue sender, Message m);
        
        public event IncommingMessageHandler IncommingMessage;

        public bool HaveMessages
        {
            get
            {
                try
                {
                    using (var m = queue.Peek(new TimeSpan(0)))
                        return m == null ? false : true;
                } catch (Exception)
                {
                    return false;
                }
            }
        }

        public string Name {
            get
            {
                return name;
            }
            set
            {
                name = value;
                queuepath = String.Format(".\\Private$\\{0}", name);
                try
                {
                    queue = MessageQueue.Exists(queuepath) ? new MessageQueue(queuepath) : MessageQueue.Create(queuepath);
                    queue.Formatter = new XmlMessageFormatter(new[] {typeof (String)});
                    queue.DefaultPropertiesToSend.Recoverable = true;
                } catch (MessageQueueException e)
                {
                    T.ERROR("-");
                    T.ERROR(string.Format("No fue posible utilizar la cola {0} (path: {1})", name, queuepath));
                    T.ERROR(string.Format("Excepcion: {0} ", e.Message));
                    T.ERROR("-");
                    throw;
                }
            }
        }

        public void Push(string label, byte[] data)
        {
            if (queue == null) throw new InvalidMessageQueueException();
            queue.Send(Convert.ToBase64String(data), label);
        }

        public byte[] Pop(ref string label)
        {
            if (queue == null) throw new InvalidMessageQueueException();
            try
            {
                var m = queue.Receive(new TimeSpan(0));
                if (m == null) throw new EmptyMessageQueueException();
                label = m.Label;
                return m.Body as String == "" ? new byte[1] : Convert.FromBase64String(m.Body as String);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public byte[] SafePop(ref string label)
        {
            if (queue == null) return null;
            try
            {
                var m = queue.Receive(new TimeSpan(1000));
                if (m == null) return null;
                label = m.Label;
                return m.Body as String == "" ? new byte[1] : Convert.FromBase64String(m.Body as String);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public byte[] Peek(ref string label)
        {
            if (queue == null) throw new InvalidMessageQueueException();
            try
            {
                var m = queue.Peek(new TimeSpan(0));
                if (m == null) throw new EmptyMessageQueueException();
                label = m.Label;
                return m.Body as String == "" ? new byte[1] : Convert.FromBase64String(m.Body as String);
            } catch (Exception)
            {
                return null;
            }
        }

        private bool eventos_activos;
        public void RehabilitarEventos()
        {
            if (eventos_activos) return;
            eventos_activos = true;
        }

        internal void queue_PeekCompleted(object sender, PeekCompletedEventArgs asyncResult)
        {
            // Connect to the queue.
            var mq = (MessageQueue)sender;
            if (mq != queue)
            {
                throw new Exception("Esta cola no es la cola que esperada. :D");
            }
            // End the asynchronous peek operation.
            var m = mq.EndPeek(asyncResult.AsyncResult);
            if (IncommingMessage != null)
            {
                if (IncommingMessage(this, m))
                {
                    //queue.Receive(); // desencolamos.
                    eventos_activos = false;
                    //RehabilitarEventos(); // volvemos a moniterear la cola.
                    return;
                }
            }
            eventos_activos = false;
        }

        public void Clear()
        {
            queue.Purge();
        }
    }
}