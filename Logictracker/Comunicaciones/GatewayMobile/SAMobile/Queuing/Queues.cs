using System;
using System.Collections.Generic;
using Urbetrack.Mobile.Comm.Queuing;

namespace SAMobile.Queuing
{
    [CLSCompliant(true)]
    public class Queues
    {
        private readonly Dictionary<string, Base64MessageQueue> queues = new Dictionary<string, Base64MessageQueue>();

        public Base64MessageQueue GetQueue(string name)
        {
            if (queues.ContainsKey(name))
            {
                return queues[name];
            } 
            var c = new Base64MessageQueue {Name = name};
            queues.Add(name, c);
            return c;
        }

        public void Push(string cola, string label, byte[] data)
        {
            var c = GetQueue(cola);
            c.Push(label, data);
        }

        public byte[] Pop(string cola, ref string label)
        {
            var c = GetQueue(cola);
            return c.Pop(ref label);
        }

        public byte[] Peek(string cola, ref string label)
        {
            var c = GetQueue(cola);
            return c.Peek(ref label);
        }
    }
}