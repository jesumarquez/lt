using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class Refresh
    {
		public String SessionId { get; set; }
        public int Result { get; set;}
		public String ResultDescription { get; set; }
        public DateTime SessionExpiration { get; set; }
        public int TotalActiveSessions { get; set; }

        public Refresh(XmlNode node)
        {
            if (node.Name.ToUpper() != "REFRESH") throw new ApplicationException("El nodo no es de tipo REFRESH");

            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
            SessionExpiration = node.GetDateTime("SESS_EXPIRE_TIME");
            TotalActiveSessions = node.GetInt32("TOTAL_ACTIVE_SESS");
        }
    }
}
