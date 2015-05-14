#region Usings

using System.Runtime.Serialization;
using System.ServiceModel;

#endregion

namespace Logictracker.XSynq.Utilities
{
    [DataContract(Name="XSynqFaultCode", 
        Namespace = "http://walks.ath.cx/xsynq")]
    public enum XSynqFaultCode
    {
        [EnumMember]
        DocumentNotFound,
        [EnumMember]
        Undefined
    }

    public class XSynqFaultException : FaultException<XSynqFaultCode>
    {
        public XSynqFaultException(XSynqFaultCode xSynqFaultCode)
            : base(xSynqFaultCode)
        {
        }
    }
}