#region Usings

using System.Messaging;

#endregion

namespace Urbetrack.Common.QueueSupport
{
    public interface IQueueHandler
    {
        MessageQueue queue
        {
            get;
        }
    }
}
