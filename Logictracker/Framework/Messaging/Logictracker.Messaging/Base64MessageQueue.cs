#region Usings

using System;
using System.Messaging;
using System.Text;
using System.Threading;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.MsmqMessaging
{
    public class Base64MessageQueue 
    {
        private string nombre;
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
                } catch (Exception)
                {
                    return false;
                }
            }
        }

        public string Nombre {
            get
            {
                return nombre;
            }
            set
            {
                nombre = value;
                queuepath = nombre.StartsWith("FormatName") || nombre.StartsWith(".") ? nombre : String.Format(".\\Private$\\{0}", nombre);

                try
                {
                    queue = MessageQueue.Exists(queuepath) ? new MessageQueue(queuepath) : MessageQueue.Create(queuepath);
					queue.SetPermissions(Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);
                    queue.MessageReadPropertyFilter.SetAll();
                    queue.Formatter = new XmlMessageFormatter(new[] {typeof (String)});
                    queue.DefaultPropertiesToSend.Recoverable = true;
                } catch (MessageQueueException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    throw;
                }
            }
        }

        public void Push(string label)
        {
            queue.Send(null, label);
        }

        public void Push(string label, byte[] data)
        {
            try
            {
                if (queue == null) throw new ExceptionMessageQueueInvalid();

                var base64Data = Convert.ToBase64String(data);

                queue.Send(base64Data, label, queue.Transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
            }
            catch
            {
                STrace.Debug(GetType().FullName, String.Format("COMMANDER: Error convirtiendo a string base 64 - {0}", data));
            }
        }

        public void Push(string label, string data)
        {
            Push(label, Encoding.ASCII.GetBytes(data));
        }
        
        
        public byte[] Pop(ref string label)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            var m = queue.Receive(new TimeSpan(0));
            if (m == null) throw new ExceptionMessageQueueEmpty();
            label = m.Label;
            return Convert.FromBase64String(m.Body as String);
        }

        public string JustPop()
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            var m = queue.Receive(new TimeSpan(0));
            if (m == null) throw new ExceptionMessageQueueEmpty();
            return m.Label;
        }

        public string JustPopSafe()
        {
            try
            {
                if (queue == null) return null;
                var m = queue.Receive(new TimeSpan(0, 0, 3));
                return m == null ? null : m.Label;
            }
            catch { return null; }            
        }

        public byte[] Peek(ref string label)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            try
            {
                var m = queue.Peek(new TimeSpan(0));
                if (m == null) throw new ExceptionMessageQueueEmpty();
                label = m.Label;
                return Convert.FromBase64String(m.Body as String);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string PeekLabel()
        {
            return PeekLabel(0);
        }

        public string PeekLabel(int milliseconds)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            try
            {
                var m = queue.Peek(new TimeSpan(0, 0, 0, 0, milliseconds));
                if (m == null) throw new ExceptionMessageQueueEmpty();
                return m.Label;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string PeekLabelSafe(int milliseconds)
        {
            if (queue == null) return null;
            try
            {
                var m = queue.Peek(new TimeSpan(0, 0, 0, 0, milliseconds));
                return m == null ? "" : m.Label;
            }
            catch (ThreadAbortException)
            {
                STrace.Debug(GetType().FullName," ---- ABORT ----- ");
                throw;
            }
            catch(ThreadInterruptedException)
            {
                STrace.Debug(GetType().FullName," ---- INTERRUPT ----- ");
                throw;
            }
            catch(MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    return null;
                //if (e.MessageQueueErrorCode == MessageQueueErrorCode.QueueDeleted)
                //    throw new ThreadInterruptedException();
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Close()
        {
            queue.Close();
            queue.Dispose();
            queue = null;
        }

        public void Hardreset()
        {
            MessageQueue.Delete(queue.Path);
            queue.Close();
            queue.Dispose();
            queue = null;
        }
    }
}