using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class DeleteMessage
    {
        public int Result { get; set;}
		public String ResultDescription { get; set; }
        
        public DeleteMessage(XmlNode node)
        {
            if (node.Name.ToUpper() != "DELETEMESSAGE") throw new ApplicationException("El nodo no es de tipo DELETEMESSAGE");
            
            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
        }
    }
}
