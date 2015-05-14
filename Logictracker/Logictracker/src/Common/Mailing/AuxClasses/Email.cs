#region Usings

using System;
using System.Net;
using System.Net.Mail;

#endregion

namespace Logictracker.Mailing.AuxClasses
{
    /// <summary>
    /// Auxiliar mail representation to be enqueued.
    /// </summary>
    internal class Email
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new auxiliar email to be enqueued.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="credentials"></param>
        /// <param name="enableSsl"></param>
        public Email(MailMessage message, String host, Int32 port, NetworkCredential credentials, Boolean enableSsl)
        {
            Message = message;
            Host = host;
            Port = port;
            Credentials = credentials;
            EnableSsl = enableSsl;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Mail message to be send.
        /// </summary>
        public MailMessage Message { get; private set; }

        /// <summary>
        /// Smtp host.
        /// </summary>
        public String Host { get; private set; }

        /// <summary>
        /// Smtp port.
        /// </summary>
        public Int32 Port { get; private set; }

        /// <summary>
        /// Smtp credentials.
        /// </summary>
        public NetworkCredential Credentials { get; private set; }

        /// <summary>
        /// Smtp ssl configuration.
        /// </summary>
        public Boolean EnableSsl { get; private set; }

        #endregion
    }
}
