#region Usings

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Policy;
using Logictracker.Mailing.AuxClasses;
using Microsoft.Win32;

#endregion

namespace Logictracker.Mailing
{
    /// <summary>
    /// Logictracker mailing internal queue.
    /// </summary>
    internal static class Queue
    {
        #region Private Properties

        /// <summary>
        /// Mailing internal queue.
        /// </summary>
        private static readonly ConcurrentQueue<Email> Emails = new ConcurrentQueue<Email>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueues the specified email.
        /// </summary>
        /// <param name="email"></param>
        public static void Enqueue(Email email)
        {
            Emails.Enqueue(email);
            Consumer.Start();
        }

        /// <summary>
        /// Dequeues a mail message if any is available.
        /// </summary>
        /// <returns></returns>
        public static Email Dequeue()
        {
            Email rv;
            return Emails.TryDequeue(out rv) ? rv : null;
        }

        #endregion
    }
}