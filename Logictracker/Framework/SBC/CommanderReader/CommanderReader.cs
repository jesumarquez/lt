#region Usings

using System;
using System.Globalization;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Web;
using Logictracker.AVL.Messages;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Layers.MessageQueue;
using Logictracker.Model;
using Logictracker.Model.IAgent;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

#endregion

namespace Logictracker.Layers.CommanderReader
{
    [FrameworkElement(XName = "NodesTree", IsContainer = false)]
    public class CommanderReader : FrameworkElement, IService
    {
        #region Attributes

        [ElementAttribute(XName = "DataProvider", IsSmartProperty = true, IsRequired = true)]
        public IDataProvider DataProvider
        {
            get { return (IDataProvider) GetValue("DataProvider"); }
            set { SetValue("DataProvider", value); }
        }

        [ElementAttribute(XName = "MessageQueue", IsSmartProperty = true, IsRequired = true)]
        public IMessageQueue MessageQueue
        {
            get { return (IMessageQueue) GetValue("MessageQueue"); }
            set { SetValue("MessageQueue", value); }
        }

        [ElementAttribute(XName = "DataTransportLayer", IsSmartProperty = true, IsRequired = true)]
        public IDataTransportLayer DataTransportLayer
        {
            get { return (IDataTransportLayer) GetValue("DataTransportLayer"); }
            set { SetValue("DataTransportLayer", value); }
        }

        [ElementAttribute(XName = "ReadBodyStream", IsRequired = false, DefaultValue = "true")]
        public bool ReadBodyStream { get; set; }

        [ElementAttribute(XName = "ReEnqueue", IsRequired = false, DefaultValue = "true")]
        public bool ReEnqueue { get; set; }

        [ElementAttribute(XName = "LinksTree", IsSmartProperty = true, IsRequired = true)]
        public LinksTree LinksTree
        {
            get { return (LinksTree) GetValue("LinksTree"); }
            set { SetValue("LinksTree", value); }
        }

        [ElementAttribute(XName = "DefaultParser", IsSmartProperty = true, IsRequired = false)]
        public INode DefaultParser
        {
            get { return (INode) GetValue("DefaultParser"); }
            set { SetValue("DefaultParser", value); }
        }

        #endregion

        #region IService

        public bool ServiceStart()
        {
            _commanderReaderTask = new CRTask(this);
            _commanderReaderTask.Start();
            return true;
        }

        public bool ServiceStop()
        {
            _commanderReaderTask.Stop();
            _commanderReaderTask = null;
            return true;
        }

        #endregion

        #region Public Methods

        private static int RejectCommand(
            INode device, ulong trackingId, String trackingExtraData,
            IDataTransportLayer dispatcher, Uri request)
        {
            STrace.Debug(typeof (CommanderReader).FullName, device.Id, String.Format("Comando Rechazado: {0}", request));
            var userMessage = new UserMessage(device.Id, trackingId);
            userMessage.UserSettings.Add("user_message_code", "NACK");
            userMessage.UserSettings.Add("nack_reason", "REJECTED");
            userMessage.UserSettings.Add("original_request",
                                         request.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped));
            userMessage.UserSettings.Add("trackingId", trackingId.ToString(CultureInfo.InvariantCulture));
            userMessage.UserSettings.Add("trackingExtraData", trackingExtraData);
            dispatcher.DispatchMessage(device, userMessage);
            return 1;
        }

        public static void DispatchNack(
            INode device, ulong trackingId, String trackingExtraData,
            IDataTransportLayer dispatcher)
        {
            var userMessage = new UserMessage(device.Id, trackingId);
            userMessage.UserSettings.Add("user_message_code", "NACK");
            userMessage.UserSettings.Add("trackingId", trackingId.ToString(CultureInfo.InvariantCulture));
            userMessage.UserSettings.Add("trackingExtraData", trackingExtraData);
            STrace.Debug(typeof (CommanderReader).FullName, device.Id,
                         String.Format("Nack tracking id = {0}", trackingId));
            dispatcher.DispatchMessage(device, userMessage);
        }

        private static void DispatchResetStateMachine(INode device, ulong trackingId, String trackingExtraData, IDataTransportLayer dispatcher)
        {
            var userMessage = new UserMessage(device.Id, trackingId);
            userMessage.UserSettings.Add("user_message_code", "FSM_RESET");
            userMessage.UserSettings.Add("trackingId", trackingId.ToString(CultureInfo.InvariantCulture));
            userMessage.UserSettings.Add("trackingExtraData", trackingExtraData);
            dispatcher.DispatchMessage(device, userMessage);
        }

        public static void CheckMessageTransaction(IMessage message)
        {
            if (ParserUtils.IsInvalidDeviceId(message.DeviceId)) return;
            DeviceCommand cmd = (from a in DevicesCommandStatus.GetDeviceList(message.DeviceId)
                                 where a.GatewayMessageId == (int) message.UniqueIdentifier
                                       || a.MessageId == message.UniqueIdentifier
                                 select a).FirstOrDefault();
            if (cmd == null)
            {
                //STrace.Debug(typeof(CommanderReader).FullName, message.DeviceId, "Ack NO: {0}", message.UniqueIdentifier);
                return;
            }
            STrace.Debug(typeof (CommanderReader).FullName, message.DeviceId,
                         String.Format("Ack SI: message id = {0} tracking id = {1}", message.UniqueIdentifier,
                                       cmd.MessageId));

            if (message.UserSettings.ContainsKey("user_message_code"))
                STrace.Debug(typeof (CommanderReader).FullName, message.DeviceId,
                             String.Format("ACK, ya tenia codigo: {0}:old={1}:new={2}", message.UniqueIdentifier,
                                           message.UserSettings["user_message_code"], "ACK"));
            else message.UserSettings.Add("user_message_code", "ACK");

            if (message.UserSettings.ContainsKey("trackingId"))
                STrace.Debug(typeof (CommanderReader).FullName, message.DeviceId,
                             String.Format("ACK, ya tenia trackingId: {0}:old={1}:new={2}", message.UniqueIdentifier,
                                           message.UserSettings["trackingId"], cmd.MessageId));
            else message.UserSettings.Add("trackingId", cmd.MessageId.ToString(CultureInfo.InvariantCulture));

            if (message.UserSettings.ContainsKey("trackingExtraData"))
                STrace.Debug(typeof (CommanderReader).FullName, message.DeviceId,
                             String.Format("ACK, ya tenia trackingExtraData: {0}:old={1}:new={2}",
                                           message.UniqueIdentifier, message.UserSettings["trackingExtraData"],
                                           cmd.TrackingExtraData));
            else message.UserSettings.Add("trackingExtraData", cmd.TrackingExtraData);

            DevicesCommandStatus.RemoveCommand(message.DeviceId, message.UniqueIdentifier);
        }

        #endregion

        private CRTask _commanderReaderTask;

        #region Nested type: CRTask

        private class CRTask : Task
        {
            private readonly CommanderReader _tree;

            private String _defaultTTR;

            public CRTask(CommanderReader tree)
                : base("Commander Reader Background Task")
            {
                _tree = tree;
            }

            private String defaultTTR
            {
                get { return _defaultTTR ?? (_defaultTTR = Config.DefaultTimeToReceive); }
            }

            protected override int DoWork(ulong tick)
            {
                int deviceId = 0;
                String textCommand = null;                
                if (_tree.MessageQueue == null)
                {
                    STrace.Error(GetType().FullName, deviceId, "Commander no tiene MSMQ asignada");
                    return 5000;
                }

                try
                {
                    Message cmd;
                    try
                    {
                        cmd = _tree.MessageQueue.Receive(new TimeSpan(0, 0, 30));

                        if (cmd == null)
                        {
                            STrace.Debug(GetType().FullName, "Command Message received: vacio o nulo.");
                            return 100;
                        }

                        if (_tree.ReadBodyStream)
                        {
                            if (_tree.MessageQueue.Handler.Formatter is XmlMessageFormatter)
                            {
                                textCommand = HttpUtility.UrlDecode((String) cmd.Body);
                            }
                            else
                            {
                                textCommand = cmd.Body.ToString();
                            }
                        }
                        else
                        {
                            textCommand = cmd.Label;
                        }
                    }
                    catch (MessageQueueException e)
                    {
                        if (e.MessageQueueErrorCode !=
                            MessageQueueErrorCode.IOTimeout)
                        {
                            STrace.Log(GetType().FullName, e, 0, LogTypes.Debug, null,
                                       String.Format("MessageQueueErrorCode:{0} ErrorCode:{1} Error:{2}",
                                                     e.MessageQueueErrorCode, e.ErrorCode, e));
                        }
                        return e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout ? 100 : 1000;
                    }

                    if (String.IsNullOrEmpty(textCommand))
                    {
                        STrace.Debug(GetType().FullName, "Command Text received: vacio o nulo.");
                        return 100;
                    }

                    // parseo el comando
                    var request = new Uri("http://server/" + textCommand);

                    deviceId = Convert.ToInt32(request.GetQueryField("devId", "0"));
                    var idNum = Convert.ToInt32(request.GetQueryField("idNum", "0"));
                    ulong messageId = Convert.ToUInt32(request.GetQueryField("trackingId", "0"));
                    var iLink = _tree.LinksTree.Get(deviceId);
                    INode iPoint;
                    if (iLink == null)
                    {
                        if (_tree.DefaultParser == null)
                        {
                            if (_tree.ReEnqueue)
                            {
                                STrace.Trace(GetType().FullName, deviceId,
                                             String.Format(
                                                 "Aun no hubo comunicacion con el dispositivo, reencolando comando: {0}",
                                                 textCommand));
                                _tree.MessageQueue.Send(cmd);
                            }
                            else
                            {
                                STrace.Trace(GetType().FullName, deviceId,
                                             String.Format(
                                                 "Aun no hubo comunicacion con el dispositivo, descartando comando: {0}",
                                                 textCommand));
                            }
                            return 1000;
                        }
                        iPoint = _tree.DataProvider.Get(deviceId, _tree.DefaultParser);
                    }
                    else
                    {
                        iPoint = iLink.Device;
                    }
                    var gatewayMessageId =
                        (int) (iPoint == null ? RandomUtils.RandomNumber(1, 99) : iPoint.NextSequence);
                    var trackingExtraData = request.GetQueryField("trackingExtraData", "");
                    var secondsTimeout = Convert.ToInt32(request.GetQueryField("timeToReceive", defaultTTR));
                    var testMode = request.GetQueryField("testMode", "false") == "true";
                    var iDispatcher = _tree.DataTransportLayer;

                    var command = request.GetComponents(UriComponents.Path, UriFormat.UriEscaped).TrimStart('/');
                    STrace.Debug(GetType().FullName, deviceId, String.Format("Command received: '{0}' ", textCommand));

                    const string commandPrefix = "Device.";
                    var commandResolved = false;

                    if (command.StartsWith(commandPrefix))
                    {
                        var subCommand = command.Substring(commandPrefix.Length);

                        #region Comandos de IPowerBoot

                        if (!commandResolved)
                        {
                            var iPowerBoot = iPoint as IPowerBoot;
                            {

                                    switch (subCommand)
                                    {
                                        case "Reboot":
                                            if (iPowerBoot == null || testMode)
                                                return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                            DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                               gatewayMessageId,
                                                                               secondsTimeout,
                                                                               GetActionDeviceReboot(messageId,
                                                                                                     iPowerBoot),
                                                                               trackingExtraData, _tree);
                                            commandResolved = true;
                                            break;
                                    }
                            }
                        }

                        #endregion

                        #region Comandos de IRoutable
                        if (!commandResolved)
                        {
                            var iRoutable = iPoint as IRoutable;
                            var route = request.GetSerialQueryField<Destination[]>("route", "");
                            int routeId = Convert.ToInt32(request.GetQueryField("routeId", "0"));

                            switch (subCommand)
                            {
                                case "LoadRoute":
                                    {
                                        if (iRoutable == null)
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);

                                        var sort = request.GetSerialQueryField<bool>("sort", "false");
                                        if (!route.Any())
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);
                                        DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                           gatewayMessageId,
                                                                           secondsTimeout,
                                                                           GetActionDeviceLoadRoute(route, sort,
                                                                                                    iRoutable,
                                                                                                    messageId,
                                                                                                    routeId),
                                                                           trackingExtraData, _tree);
                                        commandResolved = true;
                                        break;
                                    }
                                case "ReloadRoute":
                                    {
                                        if (iRoutable == null)
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);

                                        var sort = request.GetSerialQueryField<bool>("sort", "false");
                                        if (!route.Any())
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);
                                        DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                           gatewayMessageId,
                                                                           secondsTimeout,
                                                                           GetActionDeviceReloadRoute(route, sort,
                                                                                                      iRoutable,
                                                                                                      messageId,
                                                                                                      routeId),
                                                                           trackingExtraData, _tree);
                                        commandResolved = true;
                                        break;
                                    }
                                case "UnloadRoute":
                                    if (iRoutable == null)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                             request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       !route.Any()
                                                                           ? GetActionDeviceUnloadRoute(
                                                                               iRoutable,
                                                                               messageId,
                                                                               routeId)
                                                                           : GetActionDeviceUnloadRoute(route,
                                                                                                        iRoutable,
                                                                                                        messageId,
                                                                                                        routeId),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "UnloadStop":
                                    if (iRoutable == null)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                             request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceUnloadStop(route,
                                                                                                        iRoutable,
                                                                                                        messageId),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;

                            }

                        }

                        #endregion

                        #region Comandos de IQuadtree
                        if (!commandResolved)
                        {
                            var iQuadtree = iPoint as IQuadtree;
                            {
                                switch (subCommand)
                                {
                                    case "Qtree":
                                    case "FullQtree":
                                        if (iQuadtree == null || testMode)
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);

                                        bool fullQt = subCommand.Equals("FullQtree");
                                        int baserev =
                                            _tree.DataProvider.GetDetalleDispositivo(iPoint.Id,
                                                                                    "known_qtree_revision").
                                                As(0);
                                        DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                           gatewayMessageId,
                                                                           secondsTimeout,
                                                                           GetActionDeviceSyncronizeQuadtree(fullQt,
                                                                                                             baserev,
                                                                                                             iQuadtree,
                                                                                                             messageId),
                                                                           trackingExtraData, _tree);
                                        commandResolved = true;
                                        break;
                                }
                            }
                            // Request Soft Reboot
                        }

                        #endregion

                        #region Comandos de IWorkflow
                        if (!commandResolved)
                        {
                            var iWorkflow = iPoint as IWorkflow;
                            switch (subCommand)
                            {
                                case "SetWorkflowState":
                                    if (iWorkflow == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    var workflowList = request.GetSerialQueryField<WorkflowMessage[]>(
                                        "WorkflowList", "");
                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceSetWorkflowState(request,
                                                                                                       workflowList,
                                                                                                       iWorkflow,
                                                                                                       messageId),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                            }
                        }

                        #endregion

                        #region Comandos de IShortMessage
                        if (!commandResolved)
                        {
                            var iShortMessage = iPoint as IShortMessage;
                            string msgText = HttpUtility.UrlDecode(request.GetQueryField("msgText", "0"));
                            switch (subCommand)
                            {
                                case "SubmitCannedMessage":
                                    if (iShortMessage == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceSubmitCannedMessage(
                                                                           request,
                                                                           messageId,
                                                                           iShortMessage),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "SubmitTextMessage":
                                    if (iShortMessage == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceSubmitTextMessage(msgText,
                                                                                                        messageId,
                                                                                                        iShortMessage),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "SubmitCannedResponsesTextMessage":
                                    if (iShortMessage == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    var replies = request.GetSerialQueryField<uint[]>("replies", "");
                                    var msgTextId = request.GetSerialQueryField<uint>("textMessageId", "-1");
                                    var ackEvent = request.GetSerialQueryField<int>("ackEvent", "-1");

                                    if (!replies.Any() /*|| msgTextId == -1*/)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                             request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceSubmitCannedResponsesTextMessage
                                                                           (msgText,
                                                                            msgTextId,
                                                                            replies,
                                                                            messageId,
                                                                            iShortMessage,
                                                                            ackEvent),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "DeleteCannedMessage":
                                    if (iShortMessage == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceDeleteCannedMessage(
                                                                           request,
                                                                           messageId,
                                                                           iShortMessage),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "UpdateCannedMessage":
                                    if (iShortMessage == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceUpdateCannedMessage(
                                                                           request,
                                                                           messageId,
                                                                           iShortMessage),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "UpdateCannedResponse":
                                    if (iShortMessage == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceUpdateCannedResponse(
                                                                           request,
                                                                           messageId,
                                                                           iShortMessage),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                            }
                        }

                        #endregion

                        #region Comandos de IProvisioned
                        if (!commandResolved)
                        {
                            var iProvisioned = iPoint as IProvisioned;
                            switch (subCommand)
                            {
                                case "SetParameter":
                                    if (iProvisioned == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DetalleDispositivo[] parametros;
                                    int hash = iProvisioned.GetHashFromRevisions(out parametros);
                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceSetParameter(request,
                                                                                                   messageId,
                                                                                                   iProvisioned,
                                                                                                   hash),
                                                                       trackingExtraData,
                                                                       _tree);
                                    commandResolved = true;
                                    break;
                            }
                        }

                        #endregion

                        #region Comandos de IFoteable
                        if (!commandResolved)
                        {
                            var iFoteable = iPoint as IFoteable;
                            {
                                switch (subCommand)
                                {
                                    case "ResetStateMachine":
                                        if (iFoteable != null)
                                        {
                                            try
                                            {
                                                Fota.ResetStateMachine(iFoteable);
                                            }
                                            catch (Exception e)
                                            {
                                                STrace.Exception(typeof (Fota).FullName, e, deviceId);
                                            }
                                        }

                                        DispatchResetStateMachine(iPoint, messageId, trackingExtraData, iDispatcher);
                                        commandResolved = true;
                                        break;
                                    case "ReloadFirmware":
                                        if (iFoteable == null || testMode)
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);

                                        DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                           gatewayMessageId,
                                                                           secondsTimeout,
                                                                           GetActionDeviceReloadFirmware(messageId,
                                                                                                         iFoteable),
                                                                           trackingExtraData, _tree);
                                        commandResolved = true;
                                        break;
                                    case "ReloadConfiguration":
                                        if (iFoteable == null || testMode)
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);

                                        DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                           gatewayMessageId,
                                                                           secondsTimeout,
                                                                           GetActionDeviceReloadConfiguration(messageId,
                                                                                                              iFoteable),
                                                                           trackingExtraData, _tree);
                                        commandResolved = true;
                                        break;
                                    case "ReloadMessages":
                                        if (iFoteable == null || testMode)
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);

                                        DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                           gatewayMessageId,
                                                                           secondsTimeout,
                                                                           GetActionDeviceReloadMessages(messageId,
                                                                                                              iFoteable),
                                                                           trackingExtraData, _tree);
                                        commandResolved = true;
                                        break;
                                    case "ResetFMIOnGarmin":
                                        if (iFoteable == null || testMode)
                                            return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher,
                                                                 request);

                                        DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                           gatewayMessageId,
                                                                           secondsTimeout,
                                                                           GetActionResetFMIOnGarmin(messageId,
                                                                                                              iFoteable),
                                                                           trackingExtraData, _tree);
                                        commandResolved = true;
                                        break;

                                }
                            }
                        }

                        #endregion

                        #region Comandos de IPicture
                        if (!commandResolved)
                        {
                            var iPicture = iPoint as IPicture;
                            switch (subCommand)
                            {
                                case "RetrievePictures":
                                    if (iPicture == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       () =>
                                                                       iPicture.RetrievePictures(messageId,
                                                                                                 request.
                                                                                                     GetQueryField
                                                                                                     (
                                                                                                         "from",
                                                                                                         ""),
                                                                                                 request.
                                                                                                     GetQueryField
                                                                                                     (
                                                                                                         "to",
                                                                                                         "")),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                            }
                        }

                        #endregion

                        #region Comandos de ITemperature
                        if (!commandResolved)
                        {
                            var iTemperature = iPoint as ITemperature;
                            switch (subCommand)
                            {
                                case "ReportTemperatureStop":
                                    if (iTemperature == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceReportTemperatureStop(
                                                                           messageId, iTemperature),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "ReportTemperature":
                                    if (iTemperature == null || testMode)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceReportTemperature(request,
                                                                                                        messageId,
                                                                                                        iTemperature),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                            }
                        }

                        #endregion

                        #region Comandos de IFuelControl
                        if (!commandResolved)
                        {
                            var iFuelControl = iPoint as IFuelControl;
                            switch (subCommand)
                            {
                                case "DisableFuel":
                                    if (iFuelControl == null)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceDisableFuel(request,
                                                                                                  messageId,
                                                                                                  iFuelControl),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                                case "EnableFuel":
                                    if (iFuelControl == null)
                                        return RejectCommand(iPoint, messageId, trackingExtraData, iDispatcher, request);

                                    DevicesCommandStatus.CreateCommand(iPoint, iDispatcher, messageId,
                                                                       gatewayMessageId,
                                                                       secondsTimeout,
                                                                       GetActionDeviceEnableFuel(messageId,
                                                                                                 iFuelControl),
                                                                       trackingExtraData, _tree);
                                    commandResolved = true;
                                    break;
                            }
                        }

                        #endregion

                        #region Comandos del VirtualDataProvider
                        if (!commandResolved)
                        {
                            if (subCommand.Equals("Add") ||
                                subCommand.Equals("Delete"))
                            {
                                var dp = _tree.DataProvider as VirtualDataProvider;
                                switch (subCommand)
                                {
                                    case "Add":
                                        if (dp != null)
                                        {
                                            INode dev = dp.Add(deviceId, idNum,
                                                               request.GetQueryField("devIMEI", "000000000000000"));
                                            if (dev is IKeepAliveInfo)
                                                (dev as IKeepAliveInfo).KeepAliveLapse =
                                                    Convert.ToInt32(request.GetQueryField("devKeepAliveLapse", "10"));
                                        }
                                        commandResolved = true;
                                        break;
                                    case "Delete":
                                        if (dp != null)
                                        {
                                            dp.Del(deviceId);
                                        }
                                        commandResolved = true;
                                        break;
                                }
                            }
                        }

                        #endregion
                    }
                    if (!commandResolved)
                        STrace.Trace(GetType().FullName, deviceId,
                                     String.Format("Comando no se proceso correctamente: {0}", command));

                    return 1;
                }
                catch (ThreadAbortException)
                {
                    return 5000;
                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName, e, deviceId,
                                     String.Format("Exception procesando comando '{0}' error: {1}",
                                                   textCommand ?? "vacio", e));
                    return 1;
                }
            }

            #region GetActions

            private static Action GetActionDeviceEnableFuel(ulong messageId, IFuelControl iable)
            {
                return () => iable.EnableFuel(messageId);
            }

            private static Action GetActionDeviceDisableFuel(Uri request, ulong messageId, IFuelControl iable)
            {
                return
                    () =>
                    iable.DisableFuel(messageId, (request.GetQueryField("immediately", "False").ToLower() == "true"));
            }

            private static Action GetActionDeviceReportTemperature(Uri request, ulong messageId, ITemperature iable)
            {
                return
                    () =>
                    iable.ReportTemperature(messageId, Convert.ToInt32(request.GetQueryField("timerInterval", "20")));
            }

            private static Action GetActionDeviceReportTemperatureStop(ulong messageId, ITemperature iable)
            {
                return () => iable.ReportTemperatureStop(messageId);
            }

            private static Action GetActionDeviceReloadMessages(ulong messageId, IFoteable iFoteable)
            {
                return () => iFoteable.ReloadMessages(messageId);
            }

            private static Action GetActionResetFMIOnGarmin(ulong messageId, IFoteable iFoteable)
            {
                return () => iFoteable.ResetFMIOnGarmin(messageId);
            }
            
            private static Action GetActionDeviceReloadConfiguration(ulong messageId, IFoteable iFoteable)
            {
                return () => iFoteable.ReloadConfiguration(messageId);
            }

            private static Action GetActionDeviceReloadFirmware(ulong messageId, IFoteable iFoteable)
            {
                return () => iFoteable.ReloadFirmware(messageId);
            }

            private static Action GetActionDeviceSetParameter(Uri request, ulong messageId, IProvisioned iFeature, int hash)
            {
                return
                    () =>
                    iFeature.SetParameter(messageId, request.GetQueryField("cfgParameter", "undefined"),
                                          request.GetQueryField("cfgValue", ""),
                                          Convert.ToInt32(request.GetQueryField("cfgRevision", "0")), hash);
            }

            private static Action GetActionDeviceUpdateCannedResponse(Uri request, ulong messageId, IShortMessage iFeature)
            {
                return
                    () =>
                    iFeature.SetCannedResponse(messageId, Convert.ToInt32(request.GetQueryField("msgCode", "0")),
                                               request.GetQueryField("msgText", "(vacio)"),
                                               Convert.ToInt32(request.GetQueryField("msgRevision", "0")));
            }

            private static Action GetActionDeviceUpdateCannedMessage(Uri request, ulong messageId, IShortMessage iFeature)
            {
                return
                    () =>
                    iFeature.SetCannedMessage(messageId, Convert.ToInt32(request.GetQueryField("msgCode", "0")),
                                              request.GetQueryField("msgText", "(vacio)"),
                                              Convert.ToInt32(request.GetQueryField("msgRevision", "0")));
            }

            private static Action GetActionDeviceDeleteCannedMessage(Uri request, ulong MessageId, IShortMessage iFeature)
            {
                return
                    () =>
                    iFeature.DeleteCannedMessage(MessageId, Convert.ToInt32(request.GetQueryField("msgCode", "0")),
                                                 Convert.ToInt32(request.GetQueryField("msgRevision", "0")));
            }

            private static Action GetActionDeviceSubmitTextMessage(string text, ulong MessageId, IShortMessage iFeature)
            {
                return () => iFeature.SubmitTextMessage(MessageId, text, new int[] {});
            }

            private static Action GetActionDeviceSubmitCannedResponsesTextMessage(string text, uint entrega, uint[] responses, ulong MessageId, IShortMessage iFeature)
            {
                return GetActionDeviceSubmitCannedResponsesTextMessage(text, entrega, responses, MessageId, iFeature, -1);
            }
            private static Action GetActionDeviceSubmitCannedResponsesTextMessage(string text, uint entrega, uint[] responses, ulong MessageId, IShortMessage iFeature, int ackEvent)
            {
                return () => iFeature.SubmitTextMessage(MessageId, entrega, text, responses, ackEvent);
            }
            
            private static Action GetActionDeviceSubmitCannedMessage(Uri request, ulong MessageId, IShortMessage iFeature)
            {
                return
                    () =>
                    iFeature.SubmitCannedMessage(MessageId, Convert.ToInt32(request.GetQueryField("msgCode", "0")), null);
            }

            private static Action GetActionDeviceSetWorkflowState(
                Uri request, WorkflowMessage[] WorkflowList,
                IWorkflow iFeature, ulong MessageId)
            {
                return
                    () =>
                    iFeature.SetWorkflowState(MessageId, Convert.ToInt32(request.GetQueryField("newState", "0")),
                                              WorkflowList);
            }

            private static Action GetActionDeviceSyncronizeQuadtree(
                bool fullQt, int baserev, IQuadtree iFeature,
                ulong MessageId)
            {
                return () => iFeature.SyncronizeQuadtree(MessageId, fullQt, baserev);
            }


            private static Action GetActionDeviceLoadRoute(
                Destination[] route, bool sort, IRoutable iFeature,
                ulong MessageId, int routeId)
            {
                return () => iFeature.LoadRoute(MessageId, route, sort, routeId);
            }

            private static Action GetActionDeviceReloadRoute(
                Destination[] route, bool sort, IRoutable iFeature,
                ulong MessageId, int routeId)
            {
                return () => iFeature.ReloadRoute(MessageId, route, sort, routeId);
            }
            
            private static Action GetActionDeviceUnloadRoute(
                Destination[] route, IRoutable iFeature, ulong MessageId,
                int routeId)
            {
                return () => iFeature.UnloadRoute(MessageId, route, routeId);
            }

            private static Action GetActionDeviceUnloadStop(
                Destination[] route, IRoutable iFeature, ulong MessageId)
            {
                return () => iFeature.UnloadStop(MessageId, route);
            }


            private static Action GetActionDeviceUnloadRoute(IRoutable iFeature, ulong MessageId, int routeId)
            {
                return () => iFeature.UnloadRoute(MessageId, routeId);
            }

            private static Action GetActionDeviceReboot(ulong MessageId, IPowerBoot iFeature)
            {
                return () => iFeature.Reboot(MessageId);
            }

            #endregion
        }

        #endregion
    }
}