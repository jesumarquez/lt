#region Usings

using System;
using System.Net;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Model.Utils;
using NHibernate;

#endregion

namespace Logictracker.Layers
{
	[FrameworkElement(XName = "DataLinkLayer", IsContainer = false)]
	public class DataLinkLayer : FrameworkElement, IDataLinkLayer
	{
		#region Attributes

		[ElementAttribute(XName = "DataProvider", IsSmartProperty = true, IsRequired = true)]
		public Model.IDataProvider DataProvider
		{
			get { return (Model.IDataProvider)GetValue("DataProvider"); }
			set { SetValue("DataProvider", value); }
		}

		[ElementAttribute(XName = "DataTransportLayer", IsSmartProperty = true, IsRequired = true)]
		public IDataTransportLayer DataTransportLayer
		{
			get { return (IDataTransportLayer)GetValue("DataTransportLayer"); }
			set { SetValue("DataTransportLayer", value); }
		}

		[ElementAttribute(XName = "NetworkServer", IsSmartProperty = true, IsRequired = true)]
		public IUnderlayingNetworkLayer NetworkServer
		{
			get { return (IUnderlayingNetworkLayer)GetValue("NetworkServer"); }
			set { SetValue("NetworkServer", value); }
		}

		[ElementAttribute(XName = "LogCambioDeIp", DefaultValue = false)]
		public bool LogCambioDeIp { get; set; }
		
		[ElementAttribute(XName = "LinksTree", IsSmartProperty = true, IsRequired = true)]
		public LinksTree LinksTree
		{
			get { return (LinksTree)GetValue("LinksTree"); }
			set { SetValue("LinksTree", value); }
		}
		
		#endregion

		#region IDataLinkLayer

		public void SendMessage(int deviceId, IMessage message)
		{
			var link = LinksTree.Get(deviceId);
			if (message.IsPending()) link.UnderlayingNetworkLayer.SendFrame(link, new Frame(message.GetPending(), deviceId));            
		}

		public ILink OnLinkTranslation(IUnderlayingNetworkLayer unl, EndPoint ep, IFrame frame)
		{
			try
			{
				var former = LinksTree.Find(ep, unl).GetDeviceId();
				var dev = unl.Parser.Factory(frame, former) ?? unl.Parser;

				if (ParserUtils.IsInvalidDeviceId(dev.GetDeviceId())) return null;

			    var link = LinksTree.GetOrCreate(dev, unl, ep, former);

				if (LogCambioDeIp)
				{
					if (dev.GetDeviceId() != former)
					{
						STrace.Trace(GetType().FullName, dev.GetDeviceId(), String.Format("Detectado cambio de id: Former={0} Received={1} Address={2} Payload={3}", former, dev.GetDeviceId(), frame.RemoteAddressAsString, Encoding.ASCII.GetString(frame.Payload)));
					}

					if (!ep.Equals(link.EndPoint)) // el operador == siempre da falso por que solo compara la referencia y son 2 instancias distintas con los mismos datos!
					{
						STrace.Trace(GetType().FullName, dev.GetDeviceId(), String.Format("Detectado cambio de ip: FormerAddress={0} ReceivedAddress={1} DeviceId={2} Payload={3}", link.EndPoint, ep, dev.GetDeviceId(), Encoding.ASCII.GetString(frame.Payload)));
					}
				}

				return link;
			}
			catch (ObjectNotFoundException)
			{
				STrace.Debug(GetType().FullName, String.Format("Dispositivo no registrado: {0}", frame.PayloadAsString));
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e, String.Format("OnLinkTranslation: {0}", frame.PayloadAsString));
			}
			return null;
		}

		public bool OnFrameReceived(ILink link, IFrame frame, INode parser)
		{
			var dev = link != null ? link.Device : parser;

            if (dev.Id == 2089)
                STrace.Trace(typeof(DataLinkLayer).FullName, dev.Id, frame.PayloadAsString);

            dev.LastPacketReceivedAt = DateTime.UtcNow;

			IMessage msg = null;

//            STrace.Debug(dev.GetType().FullName, dev.GetDeviceId(), "2Decode: " + frame.PayloadAsString);
			var sucess = dev.ExecuteOnGuard(() => msg = dev.Decode(frame), "Device decode", frame.PayloadAsString);


            if (!sucess) return false;

			if (msg == null)
			{
				STrace.Debug(dev.GetType().FullName, dev.GetDeviceId(), String.Format("rx. {0}", Encoding.ASCII.GetString(frame.Payload)));
				return false;
			}

			if (dev.ChecksCorrectIdFlag && (!ParserUtils.IsInvalidDeviceId(dev.GetDeviceId())) && (link != null) && (link.Device.GetDeviceId() != dev.GetDeviceId()))
			{
				STrace.Trace(dev.GetType().FullName, dev.GetDeviceId(), String.Format("Reporte ignorado por no coincidir Id Dispo: {0} con Id Link {1} Reporte: {2}", dev.GetDeviceId(), link.Device.GetDeviceId(), msg.GetPendingAsString()));
				return false;
			}

			if (!msg.IsInvalidDeviceId()) //(!ParserUtils.IsInvalidDeviceId(dev.GetDeviceId()))
			{
				DataTransportLayer.DispatchMessage(dev, msg);
			}
			STrace.Debug(dev.GetType().FullName, dev.GetDeviceId(), String.Format("RX: '{0}'", Encoding.ASCII.GetString(frame.Payload)));
			if (!String.IsNullOrEmpty(msg.GetPendingAsString())) STrace.Debug(dev.GetType().FullName, dev.GetDeviceId(), String.Format("TX: '{0}'", msg.GetPendingAsString()));
			if (!String.IsNullOrEmpty(msg.GetPendingPostAsString())) STrace.Debug(dev.GetType().FullName, dev.GetDeviceId(), String.Format("TX: '{0}'", msg.GetPendingPostAsString()));

			var pending = msg.GetPending();
			var pendingpost = msg.GetPendingPost();
			if ((pending == null) && (pendingpost == null)) return false;

			if (pending != null)
			{
				frame.Reuse(pending);
				if (pendingpost != null)
				{
					frame.PostSend(pendingpost);
				}
			}
			else
			{
				frame.Reuse(pendingpost);
			}

			return true;
		}

		public void OnNetworkSuspend(ILink link)
		{
			if (link == null) return;

			link.State = LinkStates.Expired;
		}

		#region ILayer

		public bool StackBind(ILayer bottom, ILayer top)
		{
			return true;
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

		#endregion
	}
}