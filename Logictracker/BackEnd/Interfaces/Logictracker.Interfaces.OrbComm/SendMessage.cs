using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class SendMessage
    {
		public String ConfNum { get; set; }
        public int Result { get; set;}
		public String ResultDescription { get; set; }

        public SendMessage(XmlNode node)
        {
            if(node.Name.ToUpper() != "SENDMESSAGE") throw new ApplicationException("El nodo no es de tipo SENDMESSAGE");

            ConfNum = node.GetValue("CONF_NUM");
            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
        }
    }
}
