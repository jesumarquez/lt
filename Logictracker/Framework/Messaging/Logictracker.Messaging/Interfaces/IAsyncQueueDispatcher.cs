#region Usings

using System;
using Logictracker.MsmqMessaging.Opaque;

#endregion

namespace Logictracker.MsmqMessaging.Interfaces
{
    /// <summary>
    /// Interface para el procesamiento asincronico de colas.
    /// </summary>
    public interface IAsyncQueueDispatcher
    {
        bool CanPush { get; }
        
        bool BeginPush(OpaqueMessage msg, AsyncCallback callback, object state);
        
    }
}