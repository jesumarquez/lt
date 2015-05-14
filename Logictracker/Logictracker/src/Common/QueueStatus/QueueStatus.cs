#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using Logictracker.Configuration;

#endregion

namespace Logictracker.QueueStatus
{
    /// <summary>
    /// Class for gathering information about the system queues state.
    /// </summary>
    public static class QueueStatus
    {
        #region Public Methods

        /// <summary>
        /// Gets the max number of currently enqueued messages in the monitored queues.
        /// </summary>
        /// <returns></returns>
        public static Int32 GetMaxEnqueuedMessagesCount()
        {
            var values = GetEnqueuedMessagesPerQueue().Select(result => result.Value).ToList();

            return values.Any() ? values.Max() : 0;
        }

        /// <summary>
        /// Gets the current amount of enqueued message for each monitored queue.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, Int32> GetEnqueuedMessagesPerQueue()
        {
            var results = new Dictionary<String, Int32>();
            
            foreach (var queueName in Config.Queue.LogictrackerQueues)
            {
                if (!MessageQueue.Exists(queueName))
                {
                    results.Add(queueName, -1);
                }
                else
                {

                    var queue = GetQueue(queueName);

                    var count = GetMessageCount(queue);

                    results.Add(queueName, count);
                }
            }

            return results;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the queue associated to the givenn name.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        private static MessageQueue GetQueue(String queueName)
        {
            var queue = new MessageQueue(queueName);

        	var usersgroupname = Config.Queue.QueueUser;
			queue.SetPermissions(usersgroupname, MessageQueueAccessRights.FullControl);

            return queue;
        }

        /// <summary>
        /// Gets the current message count for the specified queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        private static int GetMessageCount(MessageQueue queue)
        {
            var count = 0;

            var enumerator = queue.GetMessageEnumerator2();

            while (enumerator.MoveNext()) count++;
            
            return count;
        }

        #endregion
    }
}
