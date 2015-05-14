#region Usings

using System;
using System.Linq;
using System.Messaging;
using System.Web;
using Urbetrack.AVL;
using Urbetrack.Common.Utils;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Description;
using Urbetrack.Model;

#endregion

namespace Urbetrack.SessionBorder
{
	[FrameworkElement(XName = "NodesTree", IsContainer = false)]
	public class CommanderReader : FrameworkElement, IService
    {
		#region Attributes
		
		[ElementAttribute(XName = "DataProvider", IsSmartProperty = true, IsRequired = true)]
		public IDataProvider DataProvider
		{
			get { return (IDataProvider)GetValue("DataProvider"); }
			set { SetValue("DataProvider", value); }
		}

		[ElementAttribute(XName = "MessageQueue", IsSmartProperty = true, IsRequired = true, DefaultValue = "commander")]
		public IMessageQueue MessageQueue
		{
			get { return (IMessageQueue)GetValue("MessageQueue"); }
			set { SetValue("MessageQueue", value); }
		}

		[ElementAttribute(XName = "ResponseDispatcher", IsSmartProperty = true, IsRequired = true, DefaultValue = null)]
		public IDispatcherLayer ResponseDispatcher
		{
			get { return (IDispatcherLayer)GetValue("ResponseDispatcher"); }
			set { SetValue("ResponseDispatcher", value); }
		}

		[ElementAttribute(XName = "ReadBodyStream", IsRequired = false, DefaultValue = "true")]
		public bool ReadBodyStream { get; set; }
		
		#endregion

		#region IService
		
		public bool ServiceStart()
		{
			STrace.Debug(GetType().FullName, "CommanderReader starting");
			_CommanderReaderTask = new CommanderReaderTask(this);
			_CommanderReaderTask.Start();
			return true;
		}

		public bool ServiceStop()
		{
			STrace.Debug(GetType().FullName, "CommanderReader stopping");
			_CommanderReaderTask.Stop();
			_CommanderReaderTask = null;
			return true;
		}
 
		#endregion

		#region Public Methods

		public static int RejectCommand(int NodeCode, int TrackingId, String TrackingExtraData, IDispatcherLayer Dispatcher, Uri request)
		{
			STrace.Debug(typeof(CommanderReader).FullName, NodeCode, "Comando Rechazado: " + request);
			var userMessage = new UserMessage(NodeCode, (ulong)TrackingId);
			userMessage.UserSettings.Add("user_message_code", "NACK");
			userMessage.UserSettings.Add("nack_reason", "REJECTED");
			userMessage.UserSettings.Add("original_request", request.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped));
			userMessage.UserSettings.Add("trackingId", TrackingId.ToString());
			userMessage.UserSettings.Add("trackingExtraData", TrackingExtraData);
			Dispatcher.Dispatch(userMessage);
			return 1;
		}

		public static void DispatchNack(int NodeCode, int TrackingId, String TrackingExtraData, IDispatcherLayer Dispatcher)
		{
			var userMessage = new UserMessage(NodeCode, (ulong)TrackingId);
			userMessage.UserSettings.Add("user_message_code", "NACK");
			userMessage.UserSettings.Add("trackingId", TrackingId.ToString());
			userMessage.UserSettings.Add("trackingExtraData", TrackingExtraData);
			Dispatcher.Dispatch(userMessage);
		}

		public static void DispatchResetStateMachine(int NodeCode, int TrackingId, String TrackingExtraData, IDispatcherLayer Dispatcher)
		{
			var userMessage = new UserMessage(NodeCode, (ulong)TrackingId);
			userMessage.UserSettings.Add("user_message_code", "FSM_RESET");
			userMessage.UserSettings.Add("trackingId", TrackingId.ToString());
			userMessage.UserSettings.Add("trackingExtraData", TrackingExtraData);
			Dispatcher.Dispatch(userMessage);
		}

		public static void CheckMessageTransaction(IMessage message)
		{
			if (ParserUtils.IsInvalidDeviceId(message.DeviceId)) return;
			var cmd = (from a in DevicesCommandStatus.GetDeviceList(message.DeviceId)
					   where a.gatewayMessageId == (int)message.UniqueIdentifier
					   select a).FirstOrDefault();
			if (cmd == null)
			{
				return;
			}
			message.UserSettings.Add("user_message_code", "ACK");
			message.UserSettings.Add("trackingId", cmd.trackingId.ToString());
			message.UserSettings.Add("trackingExtraData", cmd.TrackingExtraData);
			DevicesCommandStatus.RemoveCommand(message.DeviceId, (int)message.UniqueIdentifier);
		}
		
		#endregion

		#region Private Members

		private CommanderReaderTask _CommanderReaderTask;
		private class CommanderReaderTask : Task
        {
            private readonly CommanderReader Tree;

			public CommanderReaderTask(CommanderReader tree) : base("Commander Reader Background Task")
            {
                Tree = tree;
            }

            protected override int DoWork(ulong tick)
            {
            	var command = "";
                try
                {
                    if (Tree.MessageQueue == null)
                    {
                        STrace.Error(GetType().FullName, "Commander no tiene MSMQ asignada");

                        return 5000;
                    }

                    string textCommand;
                    try
                    {
                        var cmd = Tree.MessageQueue.Receive(new TimeSpan(0, 0, 30));

						if (cmd == null)
						{
							STrace.Debug(GetType().FullName, "Command received: vacio o nulo.");
							return 100;
						}

						textCommand = Tree.ReadBodyStream ? cmd.Body.ToString() : cmd.Label;
					}
                    catch (MessageQueueException e)
                    {
                    	return e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout ? 100 : 1000;
                    }

                    if (String.IsNullOrEmpty(textCommand))
                    {
						STrace.Debug(GetType().FullName, "Command received: vacio o nulo.");
                        return 100;
                    }

					STrace.Debug(GetType().FullName, "Command received: '{0}' ", textCommand);

                    // parseo el comando
                    var request = new Uri("http://server/" + textCommand);
                    command = request.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
                        
					var deviceId = Convert.ToInt32(request.GetQueryField("devId", "0"));
                    var trackingId = Convert.ToInt32(request.GetQueryField("trackingId", "0"));
					var trackingExtraData = request.GetQueryField("trackingExtraData", "");
                    var secondsTimeout = Convert.ToInt32(request.GetQueryField("timeToReceive", "30"));
                    var testMode = request.GetQueryField("testMode", "false") == "true";
					var iPoint = Tree.DataProvider.Get(deviceId);
                	var gatewayMessageId = (int) iPoint.NextSequence;
                    var iDispatcher = Tree.ResponseDispatcher;

					#region Comandos de IPowerBoot

					if (command.Contains("Device.Reboot"))
					{
						var iFeature = iPoint as IPowerBoot;
						if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.Reboot(gatewayMessageId), trackingExtraData, Tree);
						return 1;
					}

					#endregion

					#region Comandos de IRoutable

					if (command.Contains("Device.LoadRoute"))
					{
						var iFeature = iPoint as IRoutable;
						if (iFeature == null) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);
						var route = request.GetSerialQueryField<Destination[]>("route", "");
						if (route.Count() < 1) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);
						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.LoadRoute(trackingId, route), trackingExtraData, Tree);

						return 1;
					}

					#endregion

					#region Comandos de IQuadtree

                    // Request Soft Reboot
                    if (command.Contains("Device.Qtree") ||
                        command.Contains("Device.FullQtree"))
                    {
                        var iFeature = iPoint as IQuadtree;
                        if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

                        var fullQt = command.Contains("Device.FullQtree");

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.SyncronizeQuadtree(trackingId, fullQt), trackingExtraData, Tree);

                        return 1;
                    }

                    #endregion

					#region Comandos del VirtualDataProvider

					if (command.Contains("Device.Add"))
                    {
                    	var dp = Tree.DataProvider as VirtualDataProvider;
						if (dp != null)
						{
							var dev = dp.Add(deviceId, request.GetQueryField("devIMEI", "000000000000000"));

							if (dev is IKeepAliveInfo) (dev as IKeepAliveInfo).KeepAliveLapse = Convert.ToInt32(request.GetQueryField("devKeepAliveLapse", "10"));

						}
                    	return 1;
                    }

                    if (command == "Device.Delete")
                    {
                    	var dp = Tree.DataProvider as VirtualDataProvider;
						if (dp != null) dp.Del(deviceId);

                        return 1;
                    }

                    #endregion

                    #region Comandos de ICanQueueOutgoingMessages

                    if (command.Contains("Device.ResetStateMachine"))
                    {
						var iFeature = iPoint as ICanQueueOutgoingMessages;
                        if (iFeature != null)
							iFeature.CancelAllMessages();

						DispatchResetStateMachine(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher);
						return 1;
                    }

                    if (command.Contains("Device.CancelMessage"))
                    {
						var iFeature = iPoint as ICanQueueOutgoingMessages;
                        if (iFeature != null)
							iFeature.CancelMessage(trackingId);

                    	DispatchNack(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher);
						return 1;
                    }

                    #endregion

                    #region Comandos de IWorkflow

                    if (command.Contains("Device.SetWorkflowState"))
                    {
                        var iFeature = iPoint as IWorkflow;
                        if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						var WorkflowList = request.GetSerialQueryField<WorkflowMessage[]>("WorkflowList", "");
						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.SetWorkflowState(trackingId, Convert.ToInt32(request.GetQueryField("newState", "0")), WorkflowList), trackingExtraData, Tree);
                        return 1;
                    }

                    #endregion

                    #region Comandos de IShortMessage

                    if (command.Contains("Device.SubmitCannedMessage"))
                    {
                        var iFeature = iPoint as IShortMessage;
                        if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.SubmitCannedMessage(trackingId, Convert.ToInt32(request.GetQueryField("msgCode", "0")), null), trackingExtraData, Tree);
                        return 1;
                    }

                    if (command.Contains("Device.SubmitTextMessage"))
                    {
                        var iFeature = iPoint as IShortMessage;
                        if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

                        var text = HttpUtility.UrlDecode(request.GetQueryField("msgText", "0"));

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.SubmitTextMessage(trackingId, text, null), trackingExtraData, Tree);
                        return 1;
                    }

                    if (command.Contains("Device.DeleteCannedMessage"))
                    {
                        var iFeature = iPoint as IShortMessage;
                        if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.DeleteCannedMessage(trackingId, Convert.ToInt32(request.GetQueryField("msgCode", "0")), Convert.ToInt32(request.GetQueryField("msgRevision", "0"))), trackingExtraData, Tree);
                        return 1;
                    }

                    if (command.Contains("Device.UpdateCannedMessage"))
                    {
                        var iFeature = iPoint as IShortMessage;
                        if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.SetCannedMessage(trackingId, Convert.ToInt32(request.GetQueryField("msgCode", "0")), request.GetQueryField("msgText", "(vacio)"), Convert.ToInt32(request.GetQueryField("msgRevision", "0"))), trackingExtraData, Tree);
                        return 1;
                    }

                    if (command.Contains("Device.UpdateCannedResponse"))
                    {
                        var iFeature = iPoint as IShortMessage;
                        if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.SetCannedResponse(trackingId, Convert.ToInt32(request.GetQueryField("msgCode", "0")), request.GetQueryField("msgText", "(vacio)"), Convert.ToInt32(request.GetQueryField("msgRevision", "0"))), trackingExtraData, Tree);
                        return 1;
                    }

                    #endregion

					#region Comandos de IProvisioned

					if (command.Contains("Device.SetParameter"))
					{
						var iFeature = iPoint as IProvisioned;
						if (iFeature == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFeature.SetParameter(trackingId, request.GetQueryField("cfgParameter", "undefined"), request.GetQueryField("cfgValue", ""), Convert.ToInt32(request.GetQueryField("cfgRevision", "0"))), trackingExtraData, Tree);
						return 1;
					}

					#endregion

					#region Comandos de IFoteable

					if (command.Contains("Device.ReloadFirmware"))
					{
						var iFoteable = iPoint as IFoteable;
						if (iFoteable == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFoteable.ReloadFirmware(trackingId), trackingExtraData, Tree);
						return 1;
					}

					if (command.Contains("Device.ReloadConfiguration"))
					{
						var iFoteable = iPoint as IFoteable;
						if (iFoteable == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iFoteable.ReloadConfiguration(trackingId), trackingExtraData, Tree);
						return 1;
					}

					#endregion

					#region Comandos de IPicture

					if (command.Contains("Device.RetrievePictures"))
					{
						var iPictureable = iPoint as IPicture;
						if (iPictureable == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iPictureable.RetrievePictures(trackingId, request.GetQueryField("from", ""), request.GetQueryField("to", "")), trackingExtraData, Tree);
						return 1;
					}

					#endregion

					#region Comandos de ITemperature

					if (command.Contains("Device.ReportTemperatureStop"))
					{
						var iable = iPoint as ITemperature;
						if (iable == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iable.ReportTemperatureStop(trackingId), trackingExtraData, Tree);
						return 1;
					}

					if (command.Contains("Device.ReportTemperature"))
					{
						var iable = iPoint as ITemperature;
						if (iable == null || testMode) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iable.ReportTemperature(trackingId, Convert.ToInt32(request.GetQueryField("timerInterval", "20"))), trackingExtraData, Tree);
						return 1;
					}

					#endregion

					#region Comandos de IFuelControl

					if (command.Contains("Device.DisableFuel"))
					{
						var iable = iPoint as IFuelControl;
						if (iable == null) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iable.DisableFuel(trackingId, (request.GetQueryField("immediately", "False").ToLower() == "true")), trackingExtraData, Tree);
						return 1;
					}

					if (command.Contains("Device.EnableFuel"))
					{
						var iable = iPoint as IFuelControl;
						if (iable == null) return RejectCommand(iPoint.DeviceId, trackingId, trackingExtraData, iDispatcher, request);

						DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, trackingId, gatewayMessageId, secondsTimeout, () => iable.EnableFuel(trackingId), trackingExtraData, Tree);
						return 1;
					}

					#endregion

					STrace.Trace(GetType().FullName, "Comando no se proceso correctamente: {0}", command);
					return 1;
                }
                catch (Exception e)
                {
                    STrace.Trace(GetType().FullName, "Exception procesando comando '{0}' error: {1}", command, e);
                    return 1;
                }
            }
		}

		#endregion
    }
}