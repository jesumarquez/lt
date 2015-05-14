#region Usings

using System;

#endregion

namespace Logictracker.InterQueue.OpaqueMessage
{
    [Serializable]
    public class OpaqueMessageReply
    {
        /// <summary>
        /// Identificador univoco del mensaje.
        /// </summary>
        public ulong RepliedUniqueIdentifier { get; internal set; }

        public OpaqueMessageReply(OpaqueMessage src)
        {
            RepliedUniqueIdentifier = src.UniqueIdentifier;
        }
    }
}