using System;
using System.Collections.Generic;
using Microsoft.WindowsCE.Forms;

namespace UrbeMobile
{
    /// <summary>
    /// Class provides functionality to run application on background and listen to windows messages.
    /// ServiceApplication is immune to smart minimizing on Windows Mobile
    /// </summary>
    /// <example>
    /// ServiceApplication.Init();
    /// 
    /// ServiceApplication.RegisterMessage(WM_QUIT_SERVICE);
    /// ServiceApplication.RegisterMessage(WM_PING);
    /// 
    /// ServiceApplication.OnRegisteredMessage += new ServiceApplication.RegisteredMessageEventHandler(ServiceApplication_OnRegisteredMessage);
    /// 
    /// ServiceApplication.Name = "TestService";
    /// ServiceApplication.Run();
    /// </example>
    public class ServiceApplication
    {
        #region Fields

        private static bool _exitMainLoop = false;
      
        /// <summary>
        /// Message window for the ServiceApplication
        /// </summary>
        private static ServiceMessageWindow _messageWindow;

        #endregion

        #region Events and delegates

        public delegate void RegisteredMessageEventHandler(ref Microsoft.WindowsCE.Forms.Message message);
        public static event RegisteredMessageEventHandler OnRegisteredMessage;

        #endregion

        #region Public methods

        /// <summary>
        /// Initialize ServiceApplication.
        /// </summary>
        static ServiceApplication()
        {
            _messageWindow = new ServiceMessageWindow();
            _messageWindow.OnRegisteredMessage += 
					new ServiceMessageWindow.RegisteredMessageEventHandler(_messageWindow_OnRegisteredMessage);
        }

        /// <summary>
        /// Begins application message loop.
        /// </summary>
        /// <exception cref="ApplicationNotInitializedException"></exception>
        public static void Run()
        {            
            while (!_exitMainLoop)
            {
               System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// Informs message loop to exit
        /// </summary>
        public static void Exit()
        {
            _exitMainLoop = true;
        }

        /// <summary>
        /// Register windows message
        /// </summary>
        /// <param name="messageToRegister">Message to be registered</param>
        /// <returns>False if message is already registered</returns>
        /// <exception cref="ApplicationNotInitializedException"></exception>
        public static bool RegisterMessage(int messageToRegister)
        {            
            return _messageWindow.RegisterMessage(messageToRegister);
        }

        /// <summary>
        /// Indicates whether the specified message is registered
        /// </summary>
        /// <param name="messageToCheck">Message to be checked</param>        
        public static bool IsMessageRegistered(int messageToCheck)
        {            
            return _messageWindow.IsMessageRegistered(messageToCheck);
        }

        /// <summary>
        /// Unregister windows message
        /// </summary>
        /// <param name="messageToUnregister">Mesage to be unregistered</param>
        public static bool UnregisterMessage(int messageToUnregister)
        {            
            return _messageWindow.IsMessageRegistered(messageToUnregister);
        }

        /// <summary>
        /// Sends message to the message window and wait until Microsoft.WindowsCE.Forms.MessageWindows.WndProc(Microsoft.WindowsCE.Forms.Message) method has proceed
        /// </summary>
        /// <param name="windowName">Name of the window</param>
        /// <param name="message">Windows message</param>
        /// <param name="Wparam">W parametr</param>
        /// <param name="Lparam">L parametr</param>
        public static void SendMessage(string windowName, int message, int Wparam, int Lparam)
        {
            IntPtr hwnd = SystemCalls.FindWindow(IntPtr.Zero, windowName);
            Message msg = Message.Create(hwnd, message, (IntPtr)Wparam, (IntPtr)Lparam);

            MessageWindow.SendMessage(ref msg);
        }

        /// <summary>
        /// Sends message to the message window and wait until Microsoft.WindowsCE.Forms.MessageWindows.WndProc(Microsoft.WindowsCE.Forms.Message) method has proceed
        /// </summary>
        /// <param name="windowName">Name of the window</param>
        /// <param name="message">Windows message</param>
        public static void SendMessage(string windowName, int message)
        {
            SendMessage(windowName, message, 0, 0);
        }

        /// <summary>
        /// Sends message to the message window and return imidiately
        /// </summary>
        /// <param name="windowName">Name of the window</param>
        /// <param name="message">Windows message</param>
        /// <param name="Wparam">W parametr</param>
        /// <param name="Lparam">L parametr</param>
        public static void PostMessage(string windowName, int message, int Wparam, int Lparam)
        {
            IntPtr hwnd = SystemCalls.FindWindow(IntPtr.Zero, windowName);
            Message msg = Message.Create(hwnd, message, (IntPtr)Wparam, (IntPtr)Lparam);

            MessageWindow.PostMessage(ref msg);
        }

        /// <summary>
        /// Sends message to the message window and return imidiately 
        /// </summary>
        /// <param name="windowName">Name of the window</param>
        /// <param name="message">Windows message</param>
        public static void PostMessage(string windowName, int message)
        {
            PostMessage(windowName, message, 0, 0);
        }


        #endregion

        #region Private methods

        /// <summary>
        /// Event handler for registered messages forwarding the event 
        /// </summary>
        private static void _messageWindow_OnRegisteredMessage(ref Microsoft.WindowsCE.Forms.Message message)
        {
            if (OnRegisteredMessage != null)
                    OnRegisteredMessage(ref message);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets collection of registered windows messages
        /// </summary>
        public static IEnumerable<int> RegisteredMessages
        {
            get
            {
                foreach (int msg in _messageWindow.RegisteredMessages)
                    yield return msg;
            }
        }

        /// <summary>
        /// Indicates whether ServiceApplication is in exit state or not
        /// </summary>
        public static bool Exiting
        {
            get { return _exitMainLoop; }
        }

        /// <summary>
        /// Gets or sets name of the ServiceApplication
        /// </summary>
        public static string Name
        {
            get 
            {
                
                return _messageWindow.Text; 
            }
            set { _messageWindow.Text = value; }
        }

        #endregion
    }
}
