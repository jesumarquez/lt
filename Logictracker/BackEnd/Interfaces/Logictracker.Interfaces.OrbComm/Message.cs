using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class Message
    {
        public int ConfNum { get; set;}
        public int MessageId { get; set;}
        public int SmtpMessageId { get; set;}
        public int GccId { get; set; }
		public String MessagePriority { get; set; }
		public String MessageTime { get; set; }
		public String MessageFrom { get; set; }
		public String MessageTo { get; set; }
		public String MessageSubject { get; set; }
		public String MessageEncoding { get; set; }
		public String MessageBody { get; set; }
		public String MessageFlag { get; set; }
		public String MessageStatus { get; set; }
		public String DeliveredFailedTime { get; set; }
		public String MessageDirection { get; set; }
        public int NetworkId { get; set; }
        public int Result { get; set;}

        public Message(XmlNode node)
        {
            if (node.Name.ToUpper() != "MESSAGE") throw new ApplicationException("El nodo no es de tipo MESSAGE");
            ConfNum = node.GetInt32("CONF_NUM");
            MessageId = node.GetInt32("MESSAGE_ID");
            SmtpMessageId = node.GetInt32("SMTP_MSG_ID");
            GccId = node.GetInt32("GCC_ID");
            MessagePriority = node.GetValue("MESSAGE_PRIORITY");
            MessageTime = node.GetValue("MESSAGE_TIME");
            MessageFrom = node.GetValue("MESSAGE_FROM");
            MessageTo = node.GetValue("MESSAGE_TO");
            MessageSubject = node.GetValue("MESSAGE_SUBJECT");
            MessageEncoding = node.GetValue("MESSAGE_ENCODING");
            MessageBody = node.GetValue("MESSAGE_BODY");
            MessageFlag = node.GetValue("MESSAGE_FLAG");
            MessageStatus = node.GetValue("MESSAGE_STATUS");
            DeliveredFailedTime = node.GetValue("DELIVERED_FAILED_TIME");
            MessageDirection = node.GetValue("MESSAGE_DIRECTION");
            NetworkId = node.GetInt32("NETWORK_ID");
            Result = node.GetInt32("RESULT");
        }
    }
}
