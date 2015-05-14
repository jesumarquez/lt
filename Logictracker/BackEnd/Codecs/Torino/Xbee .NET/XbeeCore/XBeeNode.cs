using System;
using Urbetrack.DatabaseTracer.Core;

namespace XbeeCore
{
    public class XBeeNode
    {
        public ushort Address { get; set; }
        public char Signal { get; set; }
        public string Id { get; set; }
        public DateTime LastUpdate { get; set; }        

        public string IMEI()
        {
            var parts = Id.Split(":".ToCharArray());
            return parts[1];            
        }

        public void Trace(String s)
        {
            STrace.Debug(GetType().FullName, String.Format("{0}: addr={1} signal={2} id=[{3}]\r\n", s, Address, Signal, Id));
        }
    }
}
