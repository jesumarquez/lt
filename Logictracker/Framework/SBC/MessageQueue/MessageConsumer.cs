#region Usings

using System;
using System.Messaging;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Utils;

#endregion

namespace Logictracker.Layers.MessageQueue
{
    [FrameworkElement(XName = "MessageConsumer", IsContainer = false)]
    public class MessageConsumer : FrameworkElement, ILayer
	{
		#region Attributes

		[ElementAttribute(XName = "AbortOnFailure", DefaultValue = false, LoadOrder = 1)]
		public bool AbortOnFailure { get; set; }

		[ElementAttribute(XName = "ConfortTimeout", DefaultValue = 30, LoadOrder = 2)]
		public int ConfortTimeout { get; set; }

		private TimeSpan AsyncTimeout { get { return TimeSpan.FromSeconds(ConfortTimeout); } }

		[ElementAttribute(XName = "MessageQueue", IsSmartProperty = true, IsRequired = true, LoadOrder = 3)]
		public IMessageQueue MessageQueue
		{
			get { return (IMessageQueue)GetValue("MessageQueue"); }
			set { SetValue("MessageQueue", value); }
		}

		[ElementAttribute(XName = "DeadMessageQueue", DefaultValue = null, IsSmartProperty = true, IsRequired = false, LoadOrder = 4)]
		public IMessageQueue DeadMessageQueue
		{
			get { return (IMessageQueue)GetValue("DeadMessageQueue"); }
			set { SetValue("DeadMessageQueue", value); }
		}

		[ElementAttribute(XName = "MinMessagesToSleep", DefaultValue = 10, IsRequired = false)]
		public int MinMessagesToSleep { get; set; }

		[ElementAttribute(XName = "SleepTime", DefaultValue = 100, IsRequired = false)]
		public int SleepTime { get; set; }
		
		#endregion

		#region Properties

		private IDispatcherLayer DispatcherLayer;
        private System.Messaging.MessageQueue Queue { get { return MessageQueue.Handler; } }
        private System.Messaging.MessageQueue DeadQueue { get { return DeadMessageQueue == null ? null : DeadMessageQueue.Handler; } }

 		#endregion

		#region ILayer

		public bool ServiceStart()
		{
			if (MessageQueue.Transactional)
			{
			    STrace.Debug("DispatcherLock", "Queue.BeginPeek");
				Queue.BeginPeek(AsyncTimeout, null, MessagePeeked);
			}
			else
			{
                STrace.Debug("DispatcherLock", "Queue.BeginReceive");
				Queue.BeginReceive(AsyncTimeout, null, MessageReceived);
			}
			return true;
		}

		public bool ServiceStop()
		{
			if (Queue != null)
			{
				Queue.Close();
				Queue.Dispose();
			}
			if (DeadQueue != null)
			{
				DeadQueue.Close();
				DeadQueue.Dispose();
			}
			return true;
		}

		public bool StackBind(ILayer bottom, ILayer top)
		{
			if (top is IDispatcherLayer)
			{
				DispatcherLayer = top as IDispatcherLayer;
				return true;
			}
			STrace.Error(GetType().FullName, "Falta IDispatcherLayer!");
			return false;
		}

		#endregion

		#region Private Methods

        protected object peekLock = new object();

		private void MessagePeeked(IAsyncResult ar)
		{
	        try
	        {
	            var t = new TimeElapsed();
	            Queue.EndPeek(ar);
	            var ts = t.getTimeElapsed().TotalSeconds;
                if (ts > 1) STrace.Debug("DispatcherLock", "Queue.EndPeek: " + ts);
	            //transaction.Begin();
	            // Create a transaction.
	            //var msg = Queue.Receive(transaction);                
	            Message msg = null;
                lock (peekLock)
	            {
                    t.Restart();
	                msg = Queue.Receive(MessageQueueTransactionType.None);
	                ts = t.getTimeElapsed().TotalSeconds;
                    if (ts > 1) STrace.Debug("DispatcherLock", String.Format("Queue.Receive: {0}", ts));
	            }

	            if (msg == null)
	            {
	                //transaction.Abort();
	                return;
	            }
	            var payload = (IMessage) msg.Body;
	            if (payload == null)
	            {
	                STrace.Debug(GetType().FullName, String.Format("MessageConsumer: IMessage no castea. Body.ToString='{0}'", msg.Body));
	                if (DeadQueue != null)
	                    DeadQueue.Send(msg);

	                //transaction.Commit();
	                return;
	            }
	            if (DispatcherLayer.Dispatch(payload).Action != ReplyAction.None)
	            {
	                //if (AbortOnFailure)
	                    //transaction.Abort();
	                //else 
                    if (DeadQueue != null)
	                    DeadQueue.Send(msg);

	                return;
	            }
	            //var t = new TimeElapsed();
	            //transaction.Commit();
	            //if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", "transaction.Commit: " + t.getTimeElapsed().TotalSeconds);
	        }
	        catch (MessageQueueException e)
	        {
	            if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
	            {
                    //if (transaction != null)
                    //{
                    //    if (AbortOnFailure)
                    //        transaction.Abort();
                    //    else
                    //        transaction.Commit();
                    //}
	                STrace.Exception(GetType().FullName, e);
	            }
	        }
	        catch (Exception e)
	        {
                //if (transaction != null)
                //{
                //    if (AbortOnFailure)
                //        transaction.Abort();
                //    else
                //        transaction.Commit();
                //}
	            STrace.Exception(GetType().FullName, e);
	        }
	        finally
	        {
	            if (!CountMore(Queue, MinMessagesToSleep))
	            {
	                //STrace.Debug(GetType().FullName, "Sleep");
	                Thread.Sleep(SleepTime);
	            }
	            if (Queue != null)
	                Queue.BeginPeek(AsyncTimeout, null, MessagePeeked);
	        }
	    }

		private static bool CountMore(System.Messaging.MessageQueue queue, int top)
		{
			try
			{
				var cnt = 0;
				var enumerator = queue.GetMessageEnumerator2();
				while (enumerator.MoveNext() && cnt <= top) cnt++;
				return cnt > top;
			}
			catch (MessageQueueException)
			{
				return false;
			}
		}

		private void MessageReceived(IAsyncResult ar)
		{
		    var t = new TimeElapsed();

			try
			{
				var msg = Queue.EndReceive(ar);
				var payload = (IMessage)msg.Body;
				if (payload == null)
				{
					STrace.Debug(GetType().FullName, String.Format("MessageConsumer: IMessage no castea. Body.ToString='{0}'", msg.Body));
					if (DeadQueue != null)
						DeadQueue.Send(msg);
					return;
				}
				if (DispatcherLayer.Dispatch(payload).Action == ReplyAction.None) return;
				if (DeadQueue != null)
					DeadQueue.Send(msg);
			}
			catch (MessageQueueException e)
			{
				if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
				{
					STrace.Exception(GetType().FullName, e);
				}
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e);
			}
			finally
			{
				if (!CountMore(Queue, MinMessagesToSleep))
				{
					//STrace.Debug(GetType().FullName, "Sleep");
					Thread.Sleep(SleepTime);
				}
				if (Queue != null)
					Queue.BeginReceive(AsyncTimeout, null, MessageReceived);

			    var ts = t.getTimeElapsed().TotalSeconds;
                if (ts > 1) STrace.Debug("DispatcherLock", "MessageReceived: " + ts);
			}
		}

		#endregion
    }
}