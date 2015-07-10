#region Usings

using System;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing.AuxClasses;

#endregion

namespace Logictracker.Mailing
{
    /// <summary>
    /// Internal mailing sender consumer.
    /// </summary>
    internal static class Consumer
    {
        #region Private Properties

        /// <summary>
        /// Consumer main thread.
        /// </summary>
        private static readonly Thread ConsumerThread;

        /// <summary>
        /// Event for syncronizing consumer thread.
        /// </summary>
        private static readonly Semaphore Semaphore;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialice tracer consumer enviroment.
        /// </summary>
        static Consumer()
        {
            Semaphore = new Semaphore(0, Int32.MaxValue);

            ConsumerThread = new Thread(Consume);

            ConsumerThread.Start();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setps up the consumer enviroment and triggers the consumption of the emails.
        /// </summary>
        public static void Start() { Semaphore.Release(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Consumes a email from the queue.
        /// </summary>
        private static void Consume()
        {
			while (true)
			{
				try
				{
					Semaphore.WaitOne();
				    Email em;
				    while ((em = Queue.Dequeue()) != null)
				    {
				        SendMail(em);
                    }
				}
				catch (ThreadAbortException)
				{
					return;
				}
				catch (Exception e)
				{
					STrace.Exception(typeof(Consumer).FullName, e);
				}
			}
        }

        /// <summary>
        /// Sends the specified email.
        /// </summary>
        /// <param name="email"></param>
        private static void SendMail(Email email)
        {
            try
            {
                var smtp = new SmtpClient(email.Host, email.Port) { EnableSsl = email.EnableSsl, Credentials = email.Credentials };

                smtp.Send(email.Message);
            }
            catch (Exception e)
            {
                try
                {
                    var s = new StringBuilder();
                    s.Append("Host: ");
                    s.Append(String.IsNullOrEmpty(email.Host) ? "N/A" : email.Host);
                    s.Append(", Port: ");
                    s.Append(email.Port.ToString());
                    s.Append(", SSL: ");
                    s.Append(email.EnableSsl ? "True" : "False");
                    s.Append(", Credentials: ");
                    if (email.Credentials != null)
                    {
                        s.Append("(");
                        s.Append(" user:");
                        s.Append(email.Credentials.UserName);
                        s.Append(", pwd:");
                        s.Append(email.Credentials.Password);
                        s.Append(")");
                    }
                    else s.Append("N/A");
                    s.Append(", message: ");
                    if (email.Message != null)
                    {
                        s.Append("(");
                        s.Append("From: ");
                        s.Append(email.Message.From.ToString());
                        s.Append(", To: ");
                        s.Append(email.Message.To.ToString());
                        s.Append(", Subject: ");
                        s.Append(email.Message.Subject.ToString());
                        s.Append(", Message: ");
                        s.Append(email.Message.Body.ToString());
                        s.Append(")");
                    }
                    else
                    {
                        s.Append("Invalid");
                    }
                    STrace.Exception(typeof (Consumer).FullName, e, s.ToString());
                } catch(Exception j)
                {
                    STrace.Exception(typeof(Consumer).FullName, e);
                    STrace.Exception(typeof(Consumer).FullName, j);
                }
            }
        }

        #endregion
    }
}