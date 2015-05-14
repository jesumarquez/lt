#region Usings

using System;

#endregion

namespace Logictracker.MsmqMessaging.Opaque
{
    [Serializable]
    public class OpaqueMessage
    {
        public long Id { get; set; }
        public String SourceQueueName { get; set; }
        public String DestinationQueueName { get; set; }
        public String Label { get; set; }
        public int Length { get; set; }
        public byte[] OpaqueBody { get; set; }
        public int OpaqueBodyType { get; set; }
        public int AppSpecific { get; set; }
    }
}