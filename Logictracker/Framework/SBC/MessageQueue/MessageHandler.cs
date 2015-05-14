#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

#endregion

namespace Logictracker.Layers.MessageQueue
{
    [FrameworkElement(XName = "MessageHandler", IsContainer = true)]
    public class MessageHandler<T> : FrameworkElement, IMessageHandler<T> where T : IMessage
	{
		#region Attributes

		[ElementAttribute(XName = "MessageQueue", IsSmartProperty = true, IsRequired = true)]
		public IMessageQueue MessageQueue
		{
			get { return (IMessageQueue)GetValue("MessageQueue"); }
			set { SetValue("MessageQueue", value); }
		}
		
		#endregion

		#region IMessageHandler

		public HandleResults HandleMessage(T message)
		{
			try
			{
				MessageQueue.Send(message);
				return HandleResults.Success;
			}
			catch (Exception ex)
			{
				STrace.Exception(GetType().FullName, ex);
				return HandleResults.UnspecifiedFailure;
			}
		}
		
		#endregion

		#region FrameworkElement

		public override bool LoadResources()
		{
			if (MessageQueue == null)
			{
				STrace.Error(GetType().FullName, String.Format("MessageHandler<{0}> No tiene message queue.", typeof(T).Name));
				return false;
			}
			return true;
		} 

		#endregion
    }
}