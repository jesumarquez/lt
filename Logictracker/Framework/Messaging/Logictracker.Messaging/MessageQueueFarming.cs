#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.MsmqMessaging
{
    public class ExceptionMessageQueueEmpty : Exception
    {
    }

    public class ExceptionMessageQueueInvalid : Exception
    {
    }

    public class MessageQueueFarming : Dictionary<string, Base64MessageQueue>
    {
        private static MessageQueueFarming _instance;
        private readonly Dictionary<string, Base64MessageQueue> base64_queues = new Dictionary<string, Base64MessageQueue>();
        
        private MessageQueueFarming()
        {
        }

        public string DefaultUserGroup { get; set; }

        public void Close()
        {
            foreach(var c in base64_queues.Values)
            {
                c.Close();
            }
            base64_queues.Clear();
        }

        private Base64MessageQueue GetBase64Queue(string name)
        {
            if (base64_queues.ContainsKey(name))
            {
                return base64_queues[name];
            } 
            var c = new Base64MessageQueue {Nombre = name};
            base64_queues.Add(name, c);
            return c;
        }

        public void Base64Push(string cola, string label, byte[] data)
        {
            var c = GetBase64Queue(cola);
            c.Push(label, data);
        }

        public byte[] Base64Pop(string cola, ref string label)
        {
            var c = GetBase64Queue(cola);
            return c.Pop(ref label);
        }

        public byte[] Base64Peek(string cola, ref string label)
        {
            var c = GetBase64Queue(cola);
            return c.Peek(ref label);
        }

        public static MessageQueueFarming i()
        {
            if (_instance == null)
            {
                _instance = new MessageQueueFarming();
            }
            return _instance;
        }
    

    }
}