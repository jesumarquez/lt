#region Usings

using System.Xml;

#endregion

namespace Logictracker.Mailing.AuxClasses
{
    #region Public Classes

    /// <summary>
    /// MailSender configuration helper.
    /// </summary>
    public class Configuration
    {
        #region Private Const Properties

        private const string Address = "address";
        private const string MessageBody = "body";
        private const string Credentials = "credentials";
        private const string DisplayName = "displayName";
        private const string EnableSsl = "enableSSL";
        private const string From = "from";
        private const string SmtpHost = "host";
        private const string IsHtml = "isHTML";
        private const string UserPassword = "password";
        private const string SmtpPort = "port";
        private const string MessageSubject = "subject";
        private const string To = "to";
        private const string User = "username";

        #endregion

        #region Public Properties

        /// <summary>
        /// Mail Body.
        /// </summary>
        public string Body;

        /// <summary>
        /// Determines whither to enable SSL.
        /// </summary>
        public readonly bool EnableSSL;

        /// <summary>
        /// Mail from address
        /// </summary>
        public readonly string FromAddress;

        /// <summary>
        /// Mail from sender name.
        /// </summary>
        public readonly string FromDisplayName;

        /// <summary>
        /// Mail host.
        /// </summary>
        public readonly string Host;

        /// <summary>
        /// Determines is the mail content is HTML.
        /// </summary>
        public readonly bool IsHTML;

        /// <summary>
        /// Sender password.
        /// </summary>
        public readonly string Password;

        /// <summary>
        /// Mail host port.
        /// </summary>
        public readonly int Port;

        /// <summary>
        /// Mail subject.
        /// </summary>
        public string Subject;

        /// <summary>
        /// Mail to address.
        /// </summary>
        public string ToAddress;

        /// <summary>
        /// Mail to reciever name.
        /// </summary>
        public string ToDisplayName;

        /// <summary>
        /// Sender SMTP credentials.
        /// </summary>
        public readonly bool UseCredentials;

        /// <summary>
        /// Sender user account.
        /// </summary>
        public readonly string Username;

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new Configuration using the provided file.
        /// </summary>
        /// <param name="file"></param>
        public Configuration(string file)
        {
            var xml = new XmlDocument();

            xml.Load(file);

            var fromNode = xml.GetElementsByTagName(From)[0];
            var toNode = xml.GetElementsByTagName(To)[0];
            var subjectNode = xml.GetElementsByTagName(MessageSubject)[0];
            var bodyNode = xml.GetElementsByTagName(MessageBody)[0];
            var hostNode = xml.GetElementsByTagName(SmtpHost)[0];
            var credentialsNode = xml.GetElementsByTagName(Credentials)[0];

            FromAddress = fromNode.Attributes[Address].Value;

            FromDisplayName = fromNode.Attributes[DisplayName] != null ? fromNode.Attributes[DisplayName].Value : FromAddress;

            ToAddress = toNode.Attributes[Address].Value;

            ToDisplayName = toNode.Attributes[DisplayName] != null ? toNode.Attributes[DisplayName].Value : ToAddress;

            Subject = subjectNode.InnerText;

            Body = bodyNode.InnerText;

            IsHTML = bodyNode.Attributes[IsHtml] != null && bodyNode.Attributes[IsHtml].Value != "false";

            Host = hostNode.Attributes[Address] != null ? hostNode.Attributes[Address].Value : "localhost";

            int port;

            Port = !int.TryParse(hostNode.Attributes[SmtpPort] != null ? hostNode.Attributes[SmtpPort].Value : null, out port) ? 25 : port;

            EnableSSL = hostNode.Attributes[EnableSsl] != null && hostNode.Attributes[EnableSsl].Value != "false";

            UseCredentials = (credentialsNode != null);

            if (!UseCredentials || credentialsNode == null) return;

            Username = credentialsNode.Attributes[User].Value;
            Password = credentialsNode.Attributes[UserPassword].Value;
        }

        #endregion
    }

    #endregion
}