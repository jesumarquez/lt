#region Usings

using System;
using System.Globalization;
using System.Net.Mail;
using Common.Logging;

#if NET_20

#else
using System.Web.Mail;
#endif

#endregion

namespace Quartz.Job
{
	/// <summary>
	/// A Job which sends an e-mail with the configured content to the configured
	/// recipient.
	/// </summary>
	/// <author>James House</author>
	/// <author>Marko Lahma (.NET)</author>
	public class SendMailJob : IJob
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (SendMailJob));

    	/// <summary> The host name of the smtp server. REQUIRED.</summary>
		public const string PropertySmtpHost = "smtp_host";

		/// <summary> The e-mail address to send the mail to. REQUIRED.</summary>
		public const string PropertyRecipient = "recipient";

		/// <summary> The e-mail address to cc the mail to. Optional.</summary>
		public const string PropertyCcRecipient = "cc_recipient";

		/// <summary> The e-mail address to claim the mail is from. REQUIRED.</summary>
		public const string PropertySender = "sender";

		/// <summary> The e-mail address the message should say to reply to. Optional.</summary>
		public const string PropertyReplyTo = "reply_to";

		/// <summary> The subject to place on the e-mail. REQUIRED.</summary>
		public const string PropertySubject = "subject";

		/// <summary> The e-mail message body. REQUIRED.</summary>
		public const string PropertyMessage = "message";

		/// <summary>
		/// Executes the job.
		/// </summary>
		/// <param name="context">The job execution context.</param>
		public virtual void Execute(JobExecutionContext context)
		{
			var data = context.JobDetail.JobDataMap;

			var smtpHost = data.GetString(PropertySmtpHost);
			var to = data.GetString(PropertyRecipient);
			var cc = data.GetString(PropertyCcRecipient);
			var from = data.GetString(PropertySender);
			var replyTo = data.GetString(PropertyReplyTo);
			var subject = data.GetString(PropertySubject);
			var message = data.GetString(PropertyMessage);

			if (smtpHost == null || smtpHost.Trim().Length == 0)
			{
				throw new ArgumentException("PropertySmtpHost not specified.");
			}
			if (to == null || to.Trim().Length == 0)
			{
				throw new ArgumentException("PropertyRecipient not specified.");
			}
			if (from == null || from.Trim().Length == 0)
			{
				throw new ArgumentException("PropertySender not specified.");
			}
			if (subject == null || subject.Trim().Length == 0)
			{
				throw new ArgumentException("PropertySubject not specified.");
			}
			if (message == null || message.Trim().Length == 0)
			{
				throw new ArgumentException("PropertyMessage not specified.");
			}

			if (cc != null && cc.Trim().Length == 0)
			{
				cc = null;
			}

			if (replyTo != null && replyTo.Trim().Length == 0)
			{
				replyTo = null;
			}

			var mailDesc = string.Format(CultureInfo.InvariantCulture, "'{0}' to: {1}", subject, to);

			Log.Info(string.Format(CultureInfo.InvariantCulture, "Sending message {0}", mailDesc));

			try
			{
				SendMail(smtpHost, to, cc, from, replyTo, subject, message);
			}
			catch (Exception ex)
			{
				throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture, "Unable to send mail: {0}", mailDesc), ex, false);
			}
		}


	    private void SendMail(string smtpHost, string to, string cc, string from, string replyTo, string subject,
		                      string message)
		{
#if NET_20
            var mimeMessage = new MailMessage(from, to, subject, message);
	        if (!String.IsNullOrEmpty(cc))
	        {
	            mimeMessage.CC.Add(cc);
	        }
	        if (!String.IsNullOrEmpty(replyTo))
	        {
	            mimeMessage.ReplyTo = new MailAddress(replyTo);
	        }

	        Send(mimeMessage, smtpHost);
#else
            MailMessage mimeMessage = new MailMessage();
			mimeMessage.To = to;
            if (cc != null && cc.Length > 0) 
            {
                mimeMessage.Cc = cc;
            }
			mimeMessage.From = from;
			mimeMessage.Subject = subject;
			mimeMessage.Body = message;
			
            SmtpMail.SmtpServer = smtpHost;
            SmtpMail.Send(mimeMessage);
#endif
        }

#if NET_20
	    protected virtual void Send(MailMessage mimeMessage, string smtpHost) 
        {
	        var client = new SmtpClient(smtpHost);
	        client.Send(mimeMessage);
	    }
#endif
	}
}
