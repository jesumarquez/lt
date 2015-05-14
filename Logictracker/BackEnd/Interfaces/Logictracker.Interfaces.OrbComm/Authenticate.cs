using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class Authenticate
    {
		public String SessionId { get; set; }
        public int Result { get; set;}
		public String ResultDescription { get; set; }
		public String SessionExpiration { get; set; }
        public int TotalActiveSessions { get; set; }

        public Authenticate(XmlNode node)
        {
            if(node.Name.ToUpper() != "AUTHENTICATE") throw new ApplicationException("El nodo no es de tipo AUTHENTICATE");

            SessionId = node.GetValue("SESSION_ID");
            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
            SessionExpiration = node.GetValue("SESS_EXPIRE_TIME");
            TotalActiveSessions = node.GetInt32("TOTAL_ACTIVE_SESS");
        }
    }
}
