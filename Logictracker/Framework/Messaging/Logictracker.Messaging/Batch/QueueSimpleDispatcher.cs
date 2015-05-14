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
    public class QueueSimpleDispatcher : ISyncQueueDispatcher
    {

        private readonly OpaqueMessageQueue dst = new OpaqueMessageQueue("");
 
        private string destination;
        public string Destination
        {
            get { return destination; }
            set { dst.Name = destination = value; }
        }

        #region ISyncQueueDispatcher Members

        public bool CanPush
        {
            get
            {
                return !string.IsNullOrEmpty(Destination);
            }
        }

        public bool BeginPush(OpaqueMessage msg)
        {
            try
            {
                dst.Push(msg);
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
