#region Usings

using System.Collections.Generic;
using Logictracker.Metrics.Core.AuxClasses;

#endregion

namespace Logictracker.Metrics.Core
{
    /// <summary>
    /// Class for performing internal metrics storage for providing asyncronous process capability.
    /// </summary>
    internal static class Queue
    {
        #region Private Properties

        /// <summary>
        /// Internal metrics queue.
        /// </summary>
        private static readonly List<MetricInstance> Metrics = new List<MetricInstance>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Equeues in the internal metrics queue the given metric instance.
        /// </summary>
        /// <param name="instance"></param>
        public static void Enqueue(MetricInstance instance)
        {
            lock (Metrics)
            {
                Metrics.Add(instance);

                Consumer.Start();
            }
        }

        /// <summary>
        /// Dequeues from the internal metric queue a metric instance.
        /// </summary>
        /// <returns></returns>
        public static MetricInstance Dequeue()
        {
            lock (Metrics)
            {
                if (Metrics.Count.Equals(0)) return null;

                var instance = Metrics[0];

                Metrics.RemoveAt(0);

                return instance;
            }
        }

        #endregion
    }
}