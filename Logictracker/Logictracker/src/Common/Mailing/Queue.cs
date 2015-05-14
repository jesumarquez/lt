#region Usings

using System.Collections.Generic;
using Logictracker.Mailing.AuxClasses;

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
        private static readonly List<Email> Emails = new List<Email>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Enqueues the specified email.
        /// </summary>
        /// <param name="email"></param>
        public static void Enqueue(Email email)
        {
            lock (Emails)
            {
                Emails.Add(email);

                Consumer.Start();
            }
        }

        /// <summary>
        /// Dequeues a mail message if any is available.
        /// </summary>
        /// <returns></returns>
        public static Email Dequeue()
        {
            lock (Emails)
            {
                if (Emails.Count.Equals(0)) return null;

                var email = Emails[0];

                Emails.RemoveAt(0);

                return email;
            }
        }

        #endregion
    }
}