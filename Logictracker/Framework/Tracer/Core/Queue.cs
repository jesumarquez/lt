#region Usings

using System;
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

		private static readonly Object Locker = new Object();
		private static readonly List<Log> InternalQueue = new List<Log>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Enques a new log into the queue.
        /// </summary>
        /// <param name="log">The log instance to be enqueued.</param>
        public static void Enqueue(Log log)
        {
			lock (Locker)
            {
                InternalQueue.Add(log);

                Consumer.Start();
            }
        }

        /// <summary>
        /// Obtains a log instance from the queue to be traced.
        /// </summary>
        /// <returns></returns>
        public static Log Dequeue()
        {
			lock (Locker)
            {
                if (InternalQueue.Count.Equals(0)) return null;

                var log = InternalQueue[0];

                InternalQueue.RemoveAt(0);

                return log;
            }
        }

        #endregion
    }
}
