using System;
using System.Collections.Generic;

namespace UrbeMobile
{
    /// <summary>
    /// Class used for ServiceApplication as windows massage listener
    /// </summary>
    internal class ServiceMessageWindow : Microsoft.WindowsCE.Forms.MessageWindow
    {
        #region Fields

        private List<int> _registeredMessages;

        #endregion

        #region Events and delegates

        public delegate void RegisteredMessageEventHandler(ref Microsoft.WindowsCE.Forms.Message message);
        public event RegisteredMessageEventHandler OnRegisteredMessage;

        #endregion

        #region Constructor

        public ServiceMessageWindow()
        {
            _registeredMessages = new List<int>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Register windows message
        /// </summary>
        /// <param name="messageToRegister">Message to be registered</param>
        /// <returns>False if message is already registered</returns>
        public bool RegisterMessage(int messageToRegister)
        {
            if (!IsMessageRegistered(messageToRegister))
                _registeredMessages.Add(messageToRegister);
            else
                return false;

            return true;
        }

        /// <summary>
        /// Indicates whether the specified message is registered
        /// </summary>
        /// <param name="messageToCheck">Message to be checked</param>  
        public bool IsMessageRegistered(int messageToCheck)
        {
            return _registeredMessages.Contains(messageToCheck);
        }

        /// <summary>
        /// Unregister windows message
        /// </summary>
        /// <param name="messageToUnregister">Mesage to be unregistered</param>
        public bool UnregisterMessage(int messageToUnregister)
        {
            if (IsMessageRegistered(messageToUnregister))
                _registeredMessages.Remove(messageToUnregister);
            else
                return false;

            return true;
        }

        #endregion

        #region Private and Protected methods

        /// <summary>
        /// Listening to incoming windows messages
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Microsoft.WindowsCE.Forms.Message m)
        {
            foreach (int message in _registeredMessages)
                if (message == m.Msg && OnRegisteredMessage != null) OnRegisteredMessage(ref m);

            base.WndProc(ref m);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets collection of registered windows messages
        /// </summary>
        public IEnumerable<int> RegisteredMessages
        {
            get
            {
                foreach (int msg in _registeredMessages)
                    yield return msg;
            }
        }

        #endregion
    }

}
