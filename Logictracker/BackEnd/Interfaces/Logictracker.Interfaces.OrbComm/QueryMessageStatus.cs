using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class QueryMessageStatus
    {
        public DateTime MessageTime { get; set; }
        public int Result { get; set;}
		public String ResultDescription { get; set; }
		public String MessageFrom { get; set; }
		public String MessageTo { get; set; }
		public String MessagePriority { get; set; }
		public String MessageSubject { get; set; }
		public String MessageEncoding { get; set; }
		public String MessageBody { get; set; }
		public String MessageStatus { get; set; }


        public QueryMessageStatus(XmlNode node)
        {
            if(node.Name.ToUpper() != "QUERYMESSAGESTATUS") throw new ApplicationException("El nodo no es de tipo QUERYMESSAGESTATUS");
            
            MessageTime = node.GetDateTime("MESSAGE_TIME");
            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
            MessageFrom = node.GetValue("MESSAGE_FROM");
            MessageTo = node.GetValue("MESSAGE_TO");
            MessagePriority = node.GetValue("MESSAGE_PRIORITY");
            MessageSubject = node.GetValue("MESSAGE_SUBJECT");
            MessageEncoding = node.GetValue("MESSAGE_ENCODING");
            MessageBody = node.GetValue("MESSAGE_BODY");
            MessageStatus = node.GetValue("MESSAGE_STATUS");
        }
    }
}
