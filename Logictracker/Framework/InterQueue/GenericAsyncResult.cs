#region Usings

using System;
using System.ComponentModel;
using System.Threading;

#endregion

namespace Logictracker.InterQueue
{
    /// <summary>
    /// Resultado Generico para operaciones asincronicas.
    /// </summary>
    public class GenericAsyncResult : IAsyncResult
    {
        private ManualResetEvent manualResetEvent;
        
        internal AutoResetEvent syncro = new AutoResetEvent(true);

        public override string ToString()
        {
            try
            {
                return AsyncState == null ? base.ToString() : AsyncState.ToString();
            }
            catch (Exception)
            {
                return base.ToString();   
            }
        }

        /// <summary>
        /// Construye un resultado Generico.
        /// </summary>
        /// <param name="obj"></param>
        public GenericAsyncResult(object obj)
        {
            lock (this)
            {
                syncro.WaitOne();
                AsyncState = obj;
                
                IsCompleted = false;
                CompletedSynchronously = false;
                syncro.Set();
            }
        }

        /// <summary>
        /// Clave unica del resultado generico.
        /// </summary>
        public ulong UniqueIdentifier { get; set; }

        #region Implementation of IAsyncResult

        private bool isCompleted;

        /// <summary>
        /// Gets an indication whether the asynchronous operation has completed.
        /// </summary>
        /// <returns>
        /// true if the operation is complete; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsCompleted
        {
            get { return isCompleted; }
            internal set { isCompleted = value; }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Threading.WaitHandle" /> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.WaitHandle" /> that is used to wait for an asynchronous operation to complete.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (manualResetEvent != null)
                {
                    return manualResetEvent;
                }

                lock (this)
                {
                    if (manualResetEvent == null)
                    {
                        manualResetEvent = new ManualResetEvent(isCompleted);
                    }
                }
                return manualResetEvent;
            }
        }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A user-defined object that qualifies or contains information about an asynchronous operation.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object AsyncState { get; private set; }

        /// <summary>
        /// Gets an indication of whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>
        /// true if the asynchronous operation completed synchronously; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool CompletedSynchronously { get; private set; }

        #endregion

        /// <summary>
        /// Permite establecer que la operacion ya se completo de forma asincronica
        /// </summary>
        /// <remarks>
        /// CUIDADO!!!
        /// Este metodo, nunca deberia ser llamado por quien hizo la solicitud.
        /// Si lo hace, el comportamiento de la solicitud es impredecible. 
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetCompletedAsynchronously()
        {
            isCompleted = true;
            CompletedSynchronously = false;
        }

        /// <summary>
        /// Permite establecer que la operacion ya se completo de forma sincronica
        /// </summary>
        /// <remarks>
        /// CUIDADO!!!
        /// Este metodo, nunca deberia ser llamado por quien hizo la solicitud.
        /// Si lo hace, el comportamiento de la solicitud es impredecible.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetCompletedSynchronously()
        {
            isCompleted = true;
            CompletedSynchronously = true;
        }

        /// <summary>
        /// Permite establecer donde se recibira la notificacion cuando se complete
        /// la operacion.
        /// </summary>
        /// <remarks>
        /// CUIDADO!!!
        /// Esta propiedad, nunca deberia ser manipulada por quien hizo la solicitud.
        /// Si lo hace, el comportamiento de la solicitud es impredecible.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AsyncCallback Callback { get; set; }
    }
}