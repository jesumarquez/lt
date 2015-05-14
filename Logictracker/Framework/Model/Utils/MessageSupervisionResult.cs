#region Usings

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Logictracker.Model.Utils
{
    /// <summary>
    /// Resultado de una supervision.
    /// </summary>
    public class MessageSupervisionResult : IAsyncResult
    {
        private static int _nextIdentifier;
        private ManualResetEvent _manualResetEvent;
        internal AutoResetEvent Syncro = new AutoResetEvent(true);
        internal int Identifier;
        internal readonly List<TimeSpan> Sequence = new List<TimeSpan>();
        internal AsyncCallback Callback;
        
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
        /// Construye un resultado de supervision.
        /// </summary>
        /// <param name="msg"></param>
        public MessageSupervisionResult(IMessage msg)
        {
            lock (this)
            {
                Syncro.WaitOne();
                Identifier = _nextIdentifier++;
                AsyncState = msg;
                
                IsCompleted = false;
                CompletedSynchronously = false;
                Syncro.Set();
            }
        }

        #region Implementation of IAsyncResult

    	/// <summary>
    	/// Gets an indication whether the asynchronous operation has completed.
    	/// </summary>
    	/// <returns>
    	/// true if the operation is complete; otherwise, false.
    	/// </returns>
    	/// <filterpriority>2</filterpriority>
    	public bool IsCompleted { get; internal set; }

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
                if (_manualResetEvent != null)
                {
                    return _manualResetEvent;
                }

                lock (this)
                {
                    if (_manualResetEvent == null)
                    {
                        _manualResetEvent = new ManualResetEvent(IsCompleted);
                    }
                }
                return _manualResetEvent;
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
    }
}