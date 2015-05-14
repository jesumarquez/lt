using System;
using System.Xml;

namespace Logictracker.Interfaces.OrbComm
{
    public class QueryDeviceStatus
    {
        public int Result { get; set;}
		public String ResultDescription { get; set; }
		public String DeviceId { get; set; }
        public int GccId { get; set; }
        public DateTime LastMessageTime { get; set; }
        public DateTime LastGgramTime { get; set; }
        public DateTime LastReportTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
        
        public QueryDeviceStatus(XmlNode node)
        {
            if(node.Name.ToUpper() != "QUERYDEVICESTATUS") throw new ApplicationException("El nodo no es de tipo QUERYDEVICESTATUS");
            
            Result = node.GetInt32("RESULT");
            ResultDescription = node.GetValue("EXTEND_DESC");
            DeviceId = node.GetValue("DEVICE_ID");
            GccId = node.GetInt32("GCC_ID");
            LastMessageTime = node.GetDateTime("LAST_MSG_TIME");
            LastGgramTime = node.GetDateTime("LAST_GGRAM_TIME");
            LastReportTime = node.GetDateTime("LAST_REPORT_TIME");
            LastUpdateTime = node.GetDateTime("LAST_UPDATE_TIME");
        }
    }
}
