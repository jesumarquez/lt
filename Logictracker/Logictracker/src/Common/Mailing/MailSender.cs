#region Usings

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing.AuxClasses;

#endregion

namespace Logictracker.Mailing
{
    #region Public Classes

    /// <summary>
    /// Mail sender helper class.
    /// </summary>
    public class MailSender
    {
        #region Public Properties

        /// <summary>
        /// Mail sender config info.
        /// </summary>
        public readonly Configuration Config;

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new MailSender using the config info in the provided file.
        /// </summary>
        /// <param name="configfile"></param>
        public MailSender(string configfile)
        {
            try
            {
            	Config = new Configuration(configfile);
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(MailSender).FullName, e);
            }
        }

        #endregion

        #region Public Methods

        public void SendMail(params string[] par)
        {
            SendMail(null, par);
        }

        public void SendMail(Attachment attach, params string[] par)
        {
            try
            {
                var subject = Config.Subject.Trim();
                var body = Config.Body;
                var fromAddress = Config.FromAddress;
                var fromDisplayName = Config.FromDisplayName;
                var toAddress = Config.ToAddress;
                var toDisplayName = Config.ToDisplayName;

                for (var i = 0; i < par.Length; i++)
                {
                    var s = par[i];

                    subject = subject.Replace(string.Concat("{", i, "}"), s);
                    body = body.Replace(string.Concat("{", i, "}"), s);
                    fromAddress = fromAddress.Replace(string.Concat("{", i, "}"), s);
                    fromDisplayName = fromDisplayName.Replace(string.Concat("{", i, "}"), s);
                    toAddress = toAddress.Replace(string.Concat("{", i, "}"), s);
                    toDisplayName = toDisplayName.Replace(string.Concat("{", i, "}"), s);
                }

                var to = new MailAddress(toAddress, toDisplayName);
                var from = new MailAddress(fromAddress, fromDisplayName);
                var msgMail = new MailMessage(from, to) { Subject = subject, IsBodyHtml = Config.IsHTML, Body = body, SubjectEncoding = Encoding.GetEncoding("iso-8859-15"), BodyEncoding = Encoding.GetEncoding("iso-8859-15") };
                
                if (attach != null) 
                    msgMail.Attachments.Add(attach);
                
                var credentials = Config.UseCredentials ? new NetworkCredential(Config.Username, Config.Password) : null;

                var email = new Email(msgMail, Config.Host, Config.Port, credentials, Config.EnableSSL);
                Queue.Enqueue(email);
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(MailSender).FullName, e);
            }
        }
               
        #endregion
    }

    #endregion
}