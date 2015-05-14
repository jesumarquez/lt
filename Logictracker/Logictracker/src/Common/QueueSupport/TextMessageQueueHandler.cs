#region Usings

using System;
using System.Collections.Generic;
using System.Messaging;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Common.QueueSupport
{
    public class TextMessageQueueHandler : IQueueHandler
    {
        #region Private Properties

        /// <summary>
        /// The message queue associated to the handler.
        /// </summary>
        private readonly MessageQueue mq_;

        /// <summary>
        /// The list of message handlers associated to the queue handler.
        /// </summary>
        private readonly List<ITextMsgHandler> msgHandlers_ = new List<ITextMsgHandler>();

        /// <summary>
        /// Timeout to be used for recieving messages.
        /// </summary>
        private TimeSpan timeout;

        #endregion

        #region Public Properties

        /// <summary>
        /// The mmessage queue associated to the handler.
        /// </summary>
        public MessageQueue queue { get { return mq_; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new TextMessageQueueHandler using the givenn queue and handlers.
        /// </summary>
        /// <param name="queu"></param>
        /// <param name="handlers"></param>
        public TextMessageQueueHandler(MessageQueue queu, IEnumerable<DispCfgQueueHandler> handlers) : this(queu, handlers, 0) { }

        /// <summary>
        /// Instanciates a new TextMessageQueueHandler using the givven queue, handlers and timeout minutes.
        /// </summary>
        /// <param name="queu"></param>
        /// <param name="handlers"></param>
        /// <param name="minutes"></param>
        public TextMessageQueueHandler(MessageQueue queu, IEnumerable<DispCfgQueueHandler> handlers, int minutes)
        {
            timeout = TimeSpan.FromMinutes(minutes);

            GenerateHandlers(handlers);

            mq_ = queu;
            mq_.Formatter = new BinaryMessageFormatter();
            mq_.ReceiveCompleted += mq__ReceiveCompleted;

            ReceiveMessage();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attends the arrival of a message from the queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mq__ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var mq = (MessageQueue)sender;

            try
            {
                var msg = mq.EndReceive(e.AsyncResult);

                foreach (var msghdl in msgHandlers_)
                    try { msghdl.DoIt(msg.Label, (string)msg.Body); }
                    catch (Exception ex) { T.EXCEPTION(ex); }
            }
            catch (MessageQueueException mex) { T.EXCEPTION(mex); }
            catch (Exception ex) { T.EXCEPTION(ex); }
            finally { ReceiveMessage(); }
        }

        /// <summary>
        /// Recieves a message from the queue.
        /// </summary>
        private void ReceiveMessage()
        {
            if (timeout.TotalMinutes == 0) mq_.BeginReceive();
            else mq_.BeginReceive(timeout);
        }

        /// <summary>
        /// Instanciates all configured message handlers.
        /// </summary>
        /// <param name="handlers"></param>
        private void GenerateHandlers(IEnumerable<DispCfgQueueHandler> handlers)
        {
            foreach (var hdl in handlers)
            {
                try
                {
                    var t = Type.GetType(hdl.@class, true);

                    if (t == null)
                    {
                        T.TRACE(string.Concat("No se puede cargar el tipo: ", hdl.@class));

                        continue;
                    }

                    var constInfo = t.GetConstructor(new Type[0]);

                    if (constInfo == null)
                    {
                        T.TRACE(string.Concat("No se puede construir la clase: ", hdl.@class));

                        continue;
                    }

                    msgHandlers_.Add((ITextMsgHandler)constInfo.Invoke(null));
                }
                catch (Exception ex) { T.EXCEPTION(ex); }
            }
        }

        #endregion
    }
}
