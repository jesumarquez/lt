#region Usings

using System;
using System.Messaging;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;

#endregion

namespace Logictracker.Layers.MessageQueue
{
    /// <summary>
    /// Implementa la funciones de acceso a la Message Queue de Windows.
    /// </summary>
    [FrameworkElement(XName = "MessageQueue", IsContainer = false)]
    public class IMessageQueue : FrameworkElement
    {
		#region Attributes

		[ElementAttribute(XName = "Formatter", DefaultValue = "BinaryFormatter", LoadOrder = 0)]
		public String Formatter { get; set; }

		/// <summary>
		/// Obtiene o establece la capacidad transaccional de la cola.
		/// </summary>
		[ElementAttribute(XName = "Transactional", DefaultValue = true, LoadOrder = 1)]
		public bool Transactional { get; set; }

		/// <summary>
		/// Obtiene o establece si los mensajes en cola son persistentes
		/// frente a un shutdown o reboot del sistema.
		/// </summary>
		[ElementAttribute(XName = "Recoverable", DefaultValue = true, LoadOrder = 2)]
		public bool Recoverable { get; set; }

		/// <summary>
		/// Obtiene o Establece el Grupo de usuarios que al crear la cola 
		/// obtendra control total.
		/// Por defecto utiliza el Grupo "Users" o "Usuarios".
		/// </summary>
		[ElementAttribute(XName = "OwnerGroup", DefaultValue = null, LoadOrder = 3)]
		public string OwnerGroup { get; set; }

		/// <summary>
		/// Obtiene o Establece la ruta de la cola asociada.
		/// </summary>
		[ElementAttribute(XName = "Path", IsRequired = true, LoadOrder = 4)]
		public string QueueName { get; set; }

		#endregion

		#region Constructors

		public IMessageQueue()
		{
		}

		public IMessageQueue(String name)
		{
			QueueName = name;
			Recoverable = String.IsNullOrEmpty(Config.Queue.QueueRecoverable) || Convert.ToBoolean(Config.Queue.QueueRecoverable);
			Transactional = String.IsNullOrEmpty(Config.Queue.QueueTransactional) || Convert.ToBoolean(Config.Queue.QueueTransactional);
			Formatter = !String.IsNullOrEmpty(Config.Queue.QueueFormater) ? Config.Queue.QueueFormater : "BinaryFormatter";
		}

		#endregion

		#region FrameworkElement

		public override bool LoadResources()
		{
			try
			{
				if (System.Messaging.MessageQueue.Exists(QueueName))
				{
					Handler = new System.Messaging.MessageQueue(QueueName);
					if (Transactional != Handler.Transactional)
					{
						STrace.Error(typeof(IMessageQueue).FullName, String.Format("MSMQ '{0}' - 'Transactional' no coincide.", QueueName));
						Transactional = Handler.Transactional;
					}
					/*if (Recoverable != Handler.DefaultPropertiesToSend.Recoverable)
					{
						STrace.Debug(typeof(IMessageQueue).FullName, String.Format("MSMQ '{0}' - 'Recoverable' no coincide.", QueueName));
						Recoverable = Handler.DefaultPropertiesToSend.Recoverable;
					}//*/
				}
				else
				{
					Handler = System.Messaging.MessageQueue.Create(QueueName, Transactional);
					Handler.DefaultPropertiesToSend.Recoverable = Recoverable;
				}

				Handler.SetPermissions(OwnerGroup ?? Config.Queue.QueueUser, MessageQueueAccessRights.FullControl);

				var formatter = (Formatter ?? "BINARY").ToUpper();
				if (formatter.StartsWith("BINARY"))
				{
					Handler.Formatter = new BinaryMessageFormatter();
				}
				else if (formatter.StartsWith("XML"))
				{
					Handler.Formatter = new XmlMessageFormatter(new [] { typeof(String) });
				}
				else if (formatter.StartsWith("ACTIVEX"))
				{
					Handler.Formatter = new ActiveXMessageFormatter();
				}
				return true;
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(IMessageQueue).FullName, e, String.Format("Error inicializando la cola '{0}'", QueueName));
				Handler = null;
				return false;
			}
		}

		#endregion

		#region Public Members

		public Boolean Send(Object message)
		{
			var msgsnd = new Message(message);
			Send(msgsnd);
			return true;
		}

		public Boolean Send(Object message, String label)
		{
			var msgsnd = new Message(message) {Label = label};
			Send(msgsnd);
			return true;
		}

		public Boolean Send(Message msgsnd)
		{
			if (Handler == null) return false;
			msgsnd.Formatter = Handler.Formatter;
			msgsnd.Recoverable = Recoverable;

			Handler.Send(msgsnd, Transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
			return true;
		}

		public Boolean Send(object msgsnd, MessageQueueTransactionType transactionType)
		{
			Handler.Send(msgsnd, transactionType);
			return true;
		}

		public Message Receive(TimeSpan timeout)
		{
			return Handler.Receive(timeout);
		}

		public void SendStOnline()
		{
			const string msg = "ST,ONLINE";
			var msgsnd = new Message(msg)
			{
				Label = msg,
				Priority = MessagePriority.Highest,
				TimeToBeReceived = new TimeSpan(0, 5, 0),
			};
			Send(msgsnd);
		}

		public int GetCount()
		{
			var count = 0;
			var cursor = Handler.CreateCursor();

			if (PeekWithoutTimeout(Handler, cursor, PeekAction.Current) != null)
			{
				count = 1;
				while ((PeekWithoutTimeout(Handler, cursor, PeekAction.Next)) != null)
				{
					count++;
				}
			}
			return count; 
		}

	    private static Message PeekWithoutTimeout(System.Messaging.MessageQueue q, Cursor cursor, PeekAction action)
		{
			Message ret = null;
			try
			{
				ret = q.Peek(new TimeSpan(1), cursor, action);
			}
			catch (MessageQueueException mqe)
			{
				if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
				{
					throw;
				}
			}
			return ret;
		}

		#endregion

		#region Internal Members

		internal System.Messaging.MessageQueue Handler { get; private set; }

		#endregion
    }
}
