using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Mail;
using MailMessage = System.Net.Mail.MailMessage;

namespace Logictracker.Tracker.Application.Reports
{
    public static class Notifier
    {
        public static void SmtpMail(string @from, List<string> destiny, string subject, string body, Stream reportStream, string fileName, int smtpPort, string smtpAddress, string passwd, bool isHtml)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(from);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = isHtml;
            
            foreach (var address in destiny)
            {
                mail.To.Add(new MailAddress(address));                    
            }

            if ((reportStream != null) && (fileName != null))
            {
                var data = new Attachment(reportStream, fileName)
                {
                    ContentType = new ContentType("application/vnd.ms-excel")
                };

                mail.Attachments.Add(data);                
            }
            
            var smtpServer = new SmtpClient(smtpAddress);
            smtpServer.Port = smtpPort;
            smtpServer.Credentials = new System.Net.NetworkCredential(from, passwd);
            smtpServer.EnableSsl = true;

            smtpServer.Send(mail);         
        }
    }
}
