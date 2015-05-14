#region Usings

using System;
using System.Collections.Generic;
using System.Messaging;
using System.Runtime.Remoting.Messaging;

using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Common.QueueSupport
{
    class QueueHandler: IQueueHandler
    {
        private readonly MessageQueue mq_;
        private readonly List<IMessageHandler> msgHandlers_ = new List<IMessageHandler>();
        private TimeSpan timeout;

        public QueueHandler(MessageQueue queu, IEnumerable<DispCfgQueueHandler> handlers) : this(queu, handlers, 0) {}

        public QueueHandler(MessageQueue queu , IEnumerable<DispCfgQueueHandler> handlers, int minutes)
        {
            timeout = TimeSpan.FromMinutes(minutes);

            mq_ = queu;

            // Construyo los handler
            foreach (var hdl in handlers)
            {
                try
                {
                    var t = Type.GetType(hdl.@class,true);

                    if (t == null)
                    {
                        T.TRACE("No se puede cargar el tipo: "+hdl.@class);
                        continue;
                    }

                    var constInfo = t.GetConstructor(new Type[0]);

                    if (constInfo == null)
                    {
                        T.TRACE("No se puede construir la clase:" + hdl.@class);
                        continue;
                    }

                    msgHandlers_.Add((IMessageHandler)constInfo.Invoke(null));
                }
                catch (Exception ex)
                {
                    T.EXCEPTION(ex);

                    #if DEBUG

                    throw;

                    #endif
                }
            }

            mq_.Formatter = new BinaryMessageFormatter();
            mq_.ReceiveCompleted += mq__ReceiveCompleted;

            if (timeout.TotalMinutes == 0) mq_.BeginReceive();
            else mq_.BeginReceive(timeout);
        }

        void mq__ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var mq = (MessageQueue)sender;

            try
            {
                var msg = mq.EndReceive(e.AsyncResult);    
                var udpmsg = (IMessage)msg.Body;

                foreach (var msghdl in msgHandlers_)
                    try { msghdl.HandleMessage(udpmsg); }
                    catch (Exception ex) 
                    {
                        T.EXCEPTION(ex);

                        #if DEBUG

                        throw;

                        #endif
                    }
            }
            catch (MessageQueueException mex) { if (!mex.MessageQueueErrorCode.Equals(MessageQueueErrorCode.IOTimeout)) T.EXCEPTION(mex); }
            catch (Exception ex) { T.EXCEPTION(ex); }
            finally
            {
                if (timeout.TotalMinutes == 0) mq_.BeginReceive();
                else mq_.BeginReceive(timeout);
            }
        }

        public MessageQueue queue { get { return mq_; } }
    }
}
