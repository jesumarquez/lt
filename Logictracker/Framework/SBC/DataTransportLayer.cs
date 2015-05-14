#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;

#endregion

namespace Logictracker.Layers
{
	[FrameworkElement(XName = "DataTransportLayer", IsContainer = true)]
	public class DataTransportLayer : FrameworkElement, IDataTransportLayer
	{
		#region Attributes

		[ElementAttribute(XName = "DataLinkLayer", IsSmartProperty = true, IsRequired = true)]
		public IDataLinkLayer DataLinkLayer
		{
			get { return (IDataLinkLayer) GetValue("DataLinkLayer"); }
			set { SetValue("DataLinkLayer", value); }
		}

		#endregion

		#region ILayer

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

		public bool ServiceStart()
		{
			return true;
		}

		public bool ServiceStop()
		{
			return true;
		}

		#endregion

		#region IDataTransportLayer

		public void DispatchMessage(INode device, IMessage message)
		{
			try
			{
				if (IsRetransmission(device.Id, message.UniqueIdentifier))
				{
					//STrace.Debug(typeof(DataTransportLayer).FullName, Device.Id, "Descartado por Retransmission {0}", IsRetransmission(Device, message.UniqueIdentifier));
					return;
				}

				CommanderReader.CommanderReader.CheckMessageTransaction(message);
				if (DispatcherLayer != null) DispatcherLayer.Dispatch(message);
				foreach (var hook in Elements().OfType<IMessageHook>())
				{
					hook.HookUpMessage(message, device);
				}
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e, device.Id);
			}
		}

		#endregion

		#region Public Members

		public static bool IsRetransmission(int deviceId, ulong messageId)
		{
			var lastMsgIds = GetLastMessageIds(deviceId);
			lock (lastMsgIds)
			{
				if (lastMsgIds.Any(mid => mid == messageId))
				{
					//STrace.Debug(typeof(DataTransportLayer).FullName, Device.Id, "DevCahe ignorando {0}", messageId);
					return true;
				}

				lastMsgIds.Insert(0, messageId);
				if (lastMsgIds.Count() > 9) lastMsgIds.RemoveRange(9, lastMsgIds.Count-9);
				SetLastMessageIds(deviceId, lastMsgIds);
				return false;
			}
		}

		#endregion

		#region Private Members

		private IDispatcherLayer DispatcherLayer { get; set; }

		#region Private MsgIds Dictionary

		private static readonly Dictionary<int, List<ulong>> me = new Dictionary<int, List<ulong>>();

		private static List<ulong> GetLastMessageIds(int DeviceId)
		{
			try
			{
				lock (me)
				{
					return me.ContainsKey(DeviceId) ? me[DeviceId] : new List<ulong>();
				}
			}
			catch (Exception e)
			{
				STrace.Exception(typeof (DataTransportLayer).FullName, e, DeviceId);
				return null;
			}
		}

		private static void SetLastMessageIds(int deviceId, List<ulong> newlist)
		{
			lock (me)
			{
				if (me.ContainsKey(deviceId)) me[deviceId] = newlist;
				else me.Add(deviceId, newlist);
			}
		}

		#endregion

		#endregion
	}
}