#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using Logictracker.Configuration;

#endregion

namespace Logictracker.MsmqMessaging
{
    public class MessageQueueMultiplexor
    {
        private readonly Queue<MessageQueue> queues = new Queue<MessageQueue>();
        
        public MessageQueueMultiplexor(IEnumerable<string> queue_names)
        {
        	foreach (var mq in queue_names.Select(name => CreateQueue(name)))
        	{
        		queues.Enqueue(mq);
        	}
        }

    	private static MessageQueue CreateQueue(string queuepath)
        {
            // si existe se abre si no se crea.
            var queue = MessageQueue.Exists(queuepath) ? new MessageQueue(queuepath) : MessageQueue.Create(queuepath);
            // se fixean los permisos, de ser posible.
            queue.SetPermissions(Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);
            // cola de objetos net, por eso usamos el BinaryFormatter
            // (debe tener visible por reflexion el ensamblado que
            // define la clase antes de poder codificarla)
            queue.Formatter = new BinaryMessageFormatter();
            queue.DefaultPropertiesToSend.Recoverable = true;
            return queue;
        }

        public void Send(object obj, string label)
        {
            lock (queues)
            {
                var mq = queues.Dequeue();
                mq.Send(obj, label);
                queues.Enqueue(mq);
            }
        }
    }
}
