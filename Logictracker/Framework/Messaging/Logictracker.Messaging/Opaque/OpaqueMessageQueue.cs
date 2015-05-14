#region Usings

using System;
using System.Messaging;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.MsmqMessaging.Opaque
{
    public class OpaqueMessageQueue
    {
        private readonly string readLabelReformatExpression;
        private string name;
        private MessageQueue queue;
        private MessageEnumerator windowEnumerator;
        private string queuepath;
        private int windowSlide;
        private int WindowSlide
        {
            get { return windowSlide; }
            set
            {
                STrace.Debug(GetType().FullName, String.Format("OpaqueQueue: WINDOW={0}/{1}", value, WindowSize));
                windowSlide = value;
            }
        }

        public OpaqueMessageQueue(String _readLabelReformatExpression)
        {
            readLabelReformatExpression = _readLabelReformatExpression;
        }

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

        private int windowSize;
        public int WindowSize
        {
            get { return windowSize; }
            set
            {
                WindowSlide = windowSize = value;
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
                queuepath = name.StartsWith("FormatName") || name.StartsWith(".") ? name : String.Format(".\\Private$\\{0}", name);

                try
                {
                    queue = MessageQueue.Exists(queuepath) ? new MessageQueue(queuepath) : MessageQueue.Create(queuepath);
					queue.SetPermissions(Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);
                    queue.MessageReadPropertyFilter.SetAll();
                    queue.Formatter = new OpaqueMessageFormatter(readLabelReformatExpression);
                    queue.DefaultPropertiesToSend.Recoverable = true;
                } catch (MessageQueueException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    throw;
                }
            }
        }

        public void Push(OpaqueMessage msg)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            queue.Send(msg);
        }

        public OpaqueMessage Pop(long id, bool incWindow)
        {
            try
            {
                if (queue == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: la cola no fue asignada.");
                    return null;
                }
                var m = queue.ReceiveByLookupId(id);
                if (m == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje retornado es nulo.");
                    return null;
                }
                var om = (OpaqueMessage)m.Body;
                if (om != null)
                {
                    om.SourceQueueName = queue.QueueName;
                    om.Id = id;
                    if (incWindow) WindowSlide++;
                }
                else
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje no es de tipo OpaqueMessage.");
                }
                return om;
            }
            catch
            {
                return null;
            }
        }

        public OpaqueMessage WindowPeek(int timeout)
        {
            try
            {
                if (queue == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: la cola no fue asignada.");
                    return null;
                }
                if (windowEnumerator == null)
                {
                    windowEnumerator = queue.GetMessageEnumerator2();
                    WindowSlide = 5;
                }
                if (WindowSlide == 0) return null;
                if (!windowEnumerator.MoveNext(new TimeSpan(timeout))) return null;
                var m = windowEnumerator.Current;
                if (m == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje retornado es nulo.");
                    return null;
                }
                var om = (OpaqueMessage)m.Body;
                if (om != null)
                {
                    om.SourceQueueName = queue.QueueName;
                    om.Id = m.LookupId;
                    WindowSlide--;
                }
                else
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje no es de tipo OpaqueMessage.");
                }

                return om;
            }
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName, e.ToString());
                return null;
            }    
        }

        public OpaqueMessage Peek()
        {
            try
            {
                if (queue == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: la cola no fue asignada.");
                    return null;
                }
                var m = queue.Peek(new TimeSpan(0));
                if (m == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje retornado es nulo.");
                    return null;
                }
                var om = (OpaqueMessage) m.Body;
                if (om != null)
                {
                    om.SourceQueueName = queue.QueueName;
                    om.Id = m.LookupId;
                }
                else
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje no es de tipo OpaqueMessage.");
                }

                return om;
            } 
            catch (Exception e)
            {
                STrace.Debug(GetType().FullName, e.ToString());
                return null;
            }
        }

        public void Close()
        {
            queue.Close();
            queue = null;
        }
    }
}
