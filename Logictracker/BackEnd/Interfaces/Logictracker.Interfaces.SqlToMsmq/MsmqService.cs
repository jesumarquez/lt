using System;
using System.Data.SqlTypes;
using System.Messaging;
using Microsoft.SqlServer.Server;

namespace Logictracker.Interfaces.SqlToMsmq
{
    public class MsmqService
    {
        /// <summary>
        /// Sends message to queue
        /// </summary>
        /// <param name="queue">Queue path</param>
		/// <param name="message">Message</param>
        /// <param name="errorMessage"></param>
        [SqlProcedure]
        public static void Send(SqlString queue, SqlString message, out SqlString errorMessage)
        {
            try
            {
                using (var msgQueue = new MessageQueue(queue.ToString(), QueueAccessMode.Send))
                {
					msgQueue.Formatter = new XmlMessageFormatter(new[] { typeof(String) });
                    msgQueue.Send(message.Value);
					errorMessage = new SqlString(String.Empty);
                }
            }
            catch (Exception ex)
            {
                errorMessage = new SqlString(ex.ToString());
            }

        }

        /// <summary>
        /// Peeks message from queue
        /// </summary>
        /// <param name="queue">Queue path</param>
        /// <param name="message"></param>
        /// <param name="errorMessage"></param>
        [SqlProcedure]
        public static void Peek(SqlString queue, out SqlString message, out SqlString errorMessage)
        {
            using (var msgQueue = new MessageQueue(queue.ToString(), QueueAccessMode.Peek))
            {
				msgQueue.Formatter = new XmlMessageFormatter(new[] { typeof(String) });
                try
                {
                    var queueMsg = msgQueue.Peek(TimeSpan.FromMilliseconds(10));
					message = new SqlString(queueMsg != null ? queueMsg.Body.ToString() : String.Empty);
					errorMessage = new SqlString(String.Empty);
                }
                catch (MessageQueueException ex)
                {
					message = new SqlString(String.Empty);
                    errorMessage = new SqlString(ex.ToString());
                }
                
            }
        }

        /// <summary>
        /// Receives message from queue
        /// </summary>
        /// <param name="queue">Queue path</param>
        /// <param name="message"></param>
        /// <param name="errorMessage"></param>
        [SqlProcedure]
        public static void Receive(SqlString queue, out SqlString message, out SqlString errorMessage)
        {
            using (var msgQueue = new MessageQueue(queue.ToString(), QueueAccessMode.Receive))
            {
				msgQueue.Formatter = new XmlMessageFormatter(new[] { typeof(String) });
                try
                {
                    var queueMsg = msgQueue.Receive(TimeSpan.FromMilliseconds(10));
					message = new SqlString(queueMsg != null ? queueMsg.Body.ToString() : String.Empty);
					errorMessage = new SqlString(String.Empty);
                }
                catch (MessageQueueException ex)
                {
					message = new SqlString(String.Empty);
                    errorMessage = new SqlString(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Gets the current message count for the specified queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="count"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        [SqlProcedure]
        public static void Count(SqlString queue, out SqlInt32 count, out SqlString errorMessage)
        {
            using (var msgQueue = new MessageQueue(queue.ToString(), QueueAccessMode.Peek))
            {
                try
                {
                    var cnt = 0;

                    var enumerator = msgQueue.GetMessageEnumerator2();

                    while (enumerator.MoveNext()) cnt++;

                    count = new SqlInt32(cnt);
					errorMessage = new SqlString(String.Empty);
                }
                catch (MessageQueueException ex)
                {
                    count = -1;
                    errorMessage = new SqlString(ex.ToString());
                }
            }
        }
    }
}
