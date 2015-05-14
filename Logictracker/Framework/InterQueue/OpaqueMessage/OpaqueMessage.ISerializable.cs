#region Usings

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace Logictracker.InterQueue.OpaqueMessage
{
    public partial class OpaqueMessage
    {
        protected OpaqueMessage(SerializationInfo info, StreamingContext context)
        {
            UniqueIdentifier = info.GetUInt64("UniqueIdentifier");
            DeviceId = info.GetInt32("DeviceId");
            UserSettings  = new Dictionary<string, string>();
            SourceQueueName = info.GetString("SourceQueueName");
            DestinationNodeCode = info.GetInt32("DestinationNodeCode");
            DestinationQueueName = info.GetString("DestinationQueueName");
            Label = info.GetString("Label");
            Length = info.GetInt32("Length");
            OpaqueBody = (byte[])info.GetValue("OpaqueBody", typeof (byte[]));
            OpaqueBodyType = info.GetInt32("OpaqueBodyType");
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version",1);
            info.AddValue("UniqueIdentifier", UniqueIdentifier);
            info.AddValue("DeviceId", DeviceId);
            info.AddValue("SourceQueueName",SourceQueueName);
            info.AddValue("DestinationNodeCode",DestinationNodeCode);
            info.AddValue("DestinationQueueName", DestinationQueueName);
            info.AddValue("Label",Label);
            info.AddValue("Length",Length);
            info.AddValue("OpaqueBody", OpaqueBody, typeof(byte[]));
            info.AddValue("OpaqueBodyType", OpaqueBodyType);
        }
    }
}