using System;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    /// <summary>
    /// Info about the current queue state.
    /// </summary>
    [Serializable]
    public class QueueVo
    {
        public const int IndexName = 0;
        public const int IndexMessages = 1;

        /// <summary>
        /// The name of the queue.
        /// </summary>
        [GridMapping(Index = IndexName, ResourceName = "Labels", VariableName = "NAME", AllowGroup = false, InitialSortExpression = true)]
        public String Name { get; set; }

        /// <summary>
        /// The amount of messages in the queue.
        /// </summary>
        [GridMapping(Index = IndexMessages, ResourceName = "Labels", VariableName = "MENSAJES", AllowGroup = false)]
        public Int32 Messages { get; set; }

    }
}
