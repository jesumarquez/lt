#region Usings

using System;
using System.Messaging;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.MsmqMessaging;

#endregion

namespace Logictracker.InterQueue.OpaqueMessage
{
    public class OpaqueMessageQueue
    {
        private readonly string readLabelReformatExpression;
        private string name;
        private MessageQueue queue;
        private string queuepath;
        
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
                        return m != null;
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
                queuepath = name.StartsWith("FormatName") || name.StartsWith(".") ? name : String.Format(".\\Private$\\{0}", name);

                try
                {
                    queue = MessageQueue.Exists(queuepath) ? new MessageQueue(queuepath) : MessageQueue.Create(queuepath);
					queue.SetPermissions(Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);
                    queue.MessageReadPropertyFilter.SetAll();
					queue.Formatter = new MsmqMessaging.Opaque.OpaqueMessageFormatter(readLabelReformatExpression);
                    queue.DefaultPropertiesToSend.Recoverable = true;
                } catch (MessageQueueException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    throw;
                }
            }
        }

		public void Push(MsmqMessaging.Opaque.OpaqueMessage msg)
        {
            if (queue == null) throw new ExceptionMessageQueueInvalid();
            queue.Send(msg);
        }

		public MsmqMessaging.Opaque.OpaqueMessage Pop()
        {
            try
            {
                if (queue == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: la cola no fue asignada.");
                    return null;
                }
                var m = queue.Receive(new TimeSpan(0));
                if (m == null)
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje retornado es nulo.");
                    return null;
                }
				var om = (MsmqMessaging.Opaque.OpaqueMessage)m.Body;

                if (om != null)
                {
                    om.SourceQueueName = queue.QueueName;
                } 
                else
                {
                    STrace.Debug(GetType().FullName,"OpaqueMessageQueue: el mensaje no es de tipo OpaqueMessage.");
                }
                return om;    
            } catch {
                return null;
            }
        }

		public MsmqMessaging.Opaque.OpaqueMessage Pop(long id)
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
// ReSharper disable HeuristicUnreachableCode
                {
                    STrace.Debug(GetType().FullName, "OpaqueMessageQueue: el mensaje retornado es nulo.");
                    return null;
                }
// ReSharper restore HeuristicUnreachableCode
				var om = (MsmqMessaging.Opaque.OpaqueMessage)m.Body;
                if (om != null)
                {
                    om.SourceQueueName = queue.QueueName;
                    om.Id = id;
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

		public MsmqMessaging.Opaque.OpaqueMessage Peek()
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
				var om = (MsmqMessaging.Opaque.OpaqueMessage)m.Body;
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
            } catch (Exception e) {
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