#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.MsmqMessaging.Interfaces;
using Logictracker.MsmqMessaging.Opaque;

#endregion

namespace Logictracker.MsmqMessaging.Batch
{
    /// <summary>
    /// Dada una cola, crea 2 colas con igual contenido que la original.
    /// </summary>
    public class QueueCloneDispatcher : ISyncQueueDispatcher
    {

        private readonly OpaqueMessageQueue one = new OpaqueMessageQueue("");
        private readonly OpaqueMessageQueue two = new OpaqueMessageQueue("");
 
        private string queueOne;

        private string queueTwo;

        public string QueueOne
        {
            get { return queueOne; }
            set { one.Name = queueOne = value; }
        }

        public string QueueTwo
        {
            get { return queueTwo; }
            set { two.Name = queueTwo = value; }
        }

        #region ISyncQueueDispatcher Members

        public bool CanPush
        {
            get
            {
                return !string.IsNullOrEmpty(QueueOne) && !string.IsNullOrEmpty(QueueTwo);
            }
        }

        public bool BeginPush(OpaqueMessage msg)
        {
            try
            {
                one.Push(msg);
                two.Push(msg);
                return true; 
            } catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e);
                return false;
            }
        }

        public bool WaitCompleted(OpaqueMessage msg)
        {
            return true;
        }

        #endregion
    }
}
