using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class SetMessageFlag
    {
        public int Result { get; set;}
		public String ResultDescription { get; set; }
        public int Messages { get; set; }
        
        public SetMessageFlag(XmlNode node)
        {
            if(node.Name.ToUpper() != "SETMESSAGEFLAG") throw new ApplicationException("El nodo no es de tipo SETMESSAGEFLAG");
            
            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
            Messages = node.GetInt32("MESSAGES");
        }
    }
}
