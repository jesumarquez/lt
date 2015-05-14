#region Usings

using Logictracker.MsmqMessaging.Opaque;

#endregion

namespace Logictracker.MsmqMessaging.Interfaces
{
    /// <summary>
    /// Interface para el procesamiento sincronico y secuencial de colas.
    /// </summary>
    public interface ISyncQueueDispatcher
    {
        bool CanPush { get; }

        bool BeginPush(OpaqueMessage msg);

        bool WaitCompleted(OpaqueMessage msg);
    }
}