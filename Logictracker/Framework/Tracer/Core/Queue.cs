#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Logictracker.DatabaseTracer.Types;

#endregion

namespace Logictracker.DatabaseTracer.Core
{
    /// <summary>
    /// Tracer internal queue for asyncronous database trace.
    /// </summary>
    internal static class Queue
    {
        #region Private Properties

        private static readonly ConcurrentQueue<Log> InternalQueue = new ConcurrentQueue<Log>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Enques a new log into the queue.
        /// </summary>
        /// <param name="log">The log instance to be enqueued.</param>
        public static void Enqueue(Log log)
        {
            InternalQueue.Enqueue(log);

            Consumer.Start();
        }

        /// <summary>
        /// Obtains a log instance from the queue to be traced.
        /// </summary>
        /// <returns></returns>
        public static Log Dequeue()
        {
            Log rv;
            return InternalQueue.TryDequeue(out rv) ? rv : null;
        }

        #endregion
    }
}
