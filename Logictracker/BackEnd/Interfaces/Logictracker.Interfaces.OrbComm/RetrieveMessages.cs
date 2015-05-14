using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class RetrieveMessages
    {
        public int Result { get; set;}
		public String ResultDescription { get; set; }
        public List<Message> Messages { get; set; }

        public RetrieveMessages(XmlNode node)
        {
            if(node.Name.ToUpper() != "RETRIEVEMESSAGES") throw new ApplicationException("El nodo no es de tipo RETRIEVEMESSAGES");

            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
            Messages = node.ChildNodes.OfType<XmlNode>()
                .Where(n => n.Name == "MESSAGE")
                .Select(childNode => new Message(childNode))
                .ToList();
        }
    }
}
