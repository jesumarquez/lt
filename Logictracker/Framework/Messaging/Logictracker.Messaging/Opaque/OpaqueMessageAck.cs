#region Usings

using System;

#endregion

namespace Logictracker.MsmqMessaging.Opaque
{
    [Serializable]
    public class OpaqueMessageAck
    {
        public enum Responses
        {
            Enqueued,
            Discarded,
            DataError,
        }

        public long LookupId { get; set; }
        public Responses Response { get; set; }
    }
}
