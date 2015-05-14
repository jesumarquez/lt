#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Utils;

#endregion

namespace Logictracker.Model.Utils
{
    public delegate void MessageSupervisionCallback(MessageSupervisionResult result);

    /// <summary>
    /// Provee Notificaciones Asincronicas temporizadas basado en una
    /// lista de intervalos TimeSpan.
    /// </summary>
    public sealed class MessageSupervisor 
    {
    	/// <summary>
    	/// Inicia la supervision a un determinado mensaje, y
    	/// </summary>
    	/// <param name="message">Mensaje bajo supervision.</param>
    	/// <param name="callback"></param>
    	/// <param name="visits"></param>
    	/// <returns></returns>
    	public static MessageSupervisionResult Supervise(IMessage message, AsyncCallback callback, params TimeSpan[] visits)
        {
            var result = new MessageSupervisionResult(message) {Callback = callback};
            result.Sequence.AddRange(visits);
            result.Sequence.Reverse();
            Inquired(result);
            return result;
        }

        private static void OnTimeout(object state)
        {
            try
            {
                var result = (MessageSupervisionResult) state;
                // ignoro el timer final, si se dio por completa la transaccion.
                if (result.IsCompleted) return;
                if (result.Sequence.Count == 0)
                {
                    result.IsCompleted = true;
                }
                result.Callback(result);
            } catch (Exception e)
            {
				STrace.Exception(typeof(MessageSupervisor).FullName, e, state.ToString());
            }
        }

        /// <summary>
        /// pasar al siguiente timer, anula el actual
        /// </summary>
        /// <param name="result"></param>
        public static bool Inquired(MessageSupervisionResult result)
        {
            if (result.IsCompleted) return false;
            result.Syncro.WaitOne();
            var span = result.Sequence[0];
            result.Sequence.RemoveAt(0);
            TimerFactory.Schedule(OnTimeout, result, Convert.ToInt32(span.TotalMilliseconds));
            result.Syncro.Set();
            return true;
        }

        /// <summary>
        /// se deja de supervisar una instancia de MessageSupervisionResult
        /// </summary>
        /// <param name="result">Instancia de MessageSupervisionResult que se deja de supervisar</param>
        /// <returns></returns>
        public static bool EndSupervision(MessageSupervisionResult result)
        {   
            if (result.IsCompleted) return false;
            result.Syncro.WaitOne();
            result.IsCompleted = true;
            result.Syncro.Set();
            return true;    
        }
    }
}