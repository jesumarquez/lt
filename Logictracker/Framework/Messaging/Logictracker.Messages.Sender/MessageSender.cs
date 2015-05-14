#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Logictracker.DAL.Factories;
using Logictracker.Layers.MessageQueue;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;

#endregion

namespace Logictracker.Messages.Sender
{
    public class MessageSender
    {
        private readonly Dispositivo _dispositivo;
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>(3);
        private readonly IMessageSaver _saver;
        public string Code;
        private string _command;
        private DAOFactory _daoFactory;

        protected MessageSender(Dispositivo dispositivo, IMessageSaver saver)
        {
            _dispositivo = dispositivo;
            _saver = saver;
        }

        #region Create

        public static MessageSender Create(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver);
        }

        public static MessageSender CreateAdd(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.Add);
        }

        public static MessageSender CreateDelete(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.Delete);
        }

        public static MessageSender CreateReboot(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.Reboot);
        }

        public static MessageSender CreateSetWorkflowState(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.SetWorkflowState);
        }

        public static MessageSender CreateSubmitCannedMessage(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.SubmitCannedMessage);
        }

        public static MessageSender CreateSubmitCannedResponsesTextMessage(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.SubmitCannedResponsesTextMessage);
        }
        
        public static MessageSender CreateSubmitTextMessage(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.SubmitTextMessage);
        }

        public static MessageSender CreateDeleteCannedMessage(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.DeleteCannedMessage);
        }

        public static MessageSender CreateUpdateCannedMessage(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.UpdateCannedMessage);
        }

        public static MessageSender CreateUpdateCannedResponse(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.UpdateCannedResponse);
        }

        public static MessageSender CreateSetParameter(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.SetParameter);
        }

        public static MessageSender CreateQtree(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.Qtree);
        }

        public static MessageSender CreateFullQtree(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.FullQtree);
        }

        public static MessageSender CreateResetStateMachine(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.ResetStateMachine);
        }

        public static MessageSender CreateLoadRoute(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.LoadRoute);
        }

        public static MessageSender CreateReloadRoute(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.ReloadRoute);
        }

        public static MessageSender CreateUnloadRoute(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.UnloadRoute);
        }

        public static MessageSender CreateUnloadStop(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.UnloadStop);
        }

        public static MessageSender CreateReloadFirmware(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.ReloadFirmware);
        }

        public static MessageSender CreateReloadMessages(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.ReloadMessages);
        }

        public static MessageSender CreateReloadConfiguration(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.ReloadConfiguration);
        }

        public static MessageSender CreateRetrievePictures(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.RetrievePictures);
        }

        public static MessageSender CreateReportTemperature(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.ReportTemperature);
        }

        public static MessageSender CreateReportTemperatureStop(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.ReportTemperatureStop);
        }

        public static MessageSender CreateDisableFuel(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.DisableFuel);
        }

        public static MessageSender CreateEnableFuel(Dispositivo dispositivo, IMessageSaver saver)
        {
            return new MessageSender(dispositivo, saver).AddCommand(Comandos.EnableFuel);
        }

        #endregion

        #region Add Parameters

        public MessageSender AddCommand(string command)
        {
            _command = command;
            Code = Comandos.GetCode(command);
            if (_command == Comandos.Add) AddDeviceInfo(_dispositivo.Imei, _dispositivo.TipoDispositivo.Fabricante);
            return this;
        }

        public MessageSender AddTrackingId(int trackingId)
        {
            AddParameter("trackingId", trackingId.ToString());
            return this;
        }

        public MessageSender AddTrackingExtraData(string trackingExtraData)
        {
            AddParameter("trackingExtraData", trackingExtraData);
            return this;
        }

        public MessageSender AddTimeToReceive(int timeToReceive)
        {
            AddParameter("timeToReceive", timeToReceive.ToString());
            return this;
        }

        public MessageSender AddAckEvent(string code)
        {
            AddParameter("ackEvent", code);
            return this;
        }

        public MessageSender AddTextMessageId(uint id)
        {
            AddParameter("textMessageId", Convert.ToString(id));
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinations"></param>
        /// <remarks>Solo válido para LoadRoute</remarks>
        /// <returns></returns>
        public MessageSender AddDestinations(Destination[] destinations)
        {

            if (!(new[] { Comandos.LoadRoute, Comandos.ReloadRoute, Comandos.UnloadRoute, Comandos.UnloadStop }.Any(c => _command == c)))
                throw new Exception("Solo se pueden agregar destinos a un comando LoadRoute, ReloadRoute, UnloadRoute y UnloadStop");

            AddParameter("route", new JavaScriptSerializer().Serialize(destinations));
            return this;
        }

        public MessageSender AddExpiration(DateTime expiresOn)
        {
            AddParameter("expiresOn", new JavaScriptSerializer().Serialize(expiresOn));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replies"></param>
        /// <remarks>Solo válido para LoadRoute</remarks>
        /// <returns></returns>
        public MessageSender AddCannedResponses(uint[] replies)
        {
            if (_command != Comandos.SubmitCannedResponsesTextMessage)
            {
                throw new Exception("Solo se pueden agregar Canned Responses a un comando SubmitTextMessage");
            }            

            AddParameter("replies", new JavaScriptSerializer().Serialize(replies));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <remarks>Solo válido para LoadRoute y UnloadRoute</remarks>
        /// <returns></returns>
        public MessageSender AddRouteId(int routeId)
        {
            if (!(new[] { Comandos.LoadRoute, Comandos.ReloadRoute, Comandos.UnloadRoute }.Any(c=> _command == c)))
                throw new Exception("Solo se pueden agregar el id de ruta a un comando LoadRoute ReloadRoute o UnloadRoute");
            AddParameter("routeId", routeId.ToString());
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <remarks>Solo válido para RetrievePictures</remarks>
        /// <returns></returns>
        public MessageSender AddDateRange(DateTime from, DateTime to)
        {
            if (_command != Comandos.RetrievePictures)
                throw new Exception("Solo se pueden agregar rango de fechas a un comando RetrievePictures");
            AddParameter("from", from.ToString("ddMMyyHHmmss"));
            AddParameter("to", to.ToString("ddMMyyHHmmss"));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="immediately"></param>
        /// <remarks>Solo válido para DisableFuel</remarks>
        /// <returns></returns>
        public MessageSender AddInmediately(bool immediately)
        {
            if (_command != Comandos.DisableFuel)
                throw new Exception("Solo se pueden agregar 'inmediatamente' a un comando DisableFuel");
            AddParameter("immediately", immediately.ToString());
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfgParameter"></param>
        /// <param name="cfgValue"></param>
        /// <param name="cfgRevision"></param>
        /// <remarks>Solo válido para SetParameter</remarks>
        /// <returns></returns>
        public MessageSender AddConfigParameter(string cfgParameter, string cfgValue, int cfgRevision)
        {
            if (_command != Comandos.SetParameter)
                throw new Exception("Solo se pueden agregar parametros de configuracion a un comando SetParameter");
            AddParameter("cfgParameter", cfgParameter);
            AddParameter("cfgValue", cfgValue);
            AddParameter("cfgRevision", cfgRevision.ToString());
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgCode"></param>
        /// <param name="msgText"></param>
        /// <param name="msgRevision"></param>
        /// <remarks>Solo válido para UpdateCannedMessage y UpdateCannedResponse</remarks>
        /// <returns></returns>
        public MessageSender AddCannedMessageInfo(string msgCode, string msgText, int msgRevision)
        {
            if (_command != Comandos.UpdateCannedMessage && _command != Comandos.UpdateCannedResponse)
                throw new Exception(
                    "Solo se puede agregar informacion de mensaje a comandos UpdateCannedMessage y UpdateCannedResponse");
            AddParameter("msgCode", msgCode);
            AddParameter("msgText", msgText);
            AddParameter("msgRevision", msgRevision.ToString());
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgCode"></param>
        /// <param name="msgRevision"></param>
        /// <remarks>Solo válido para DeleteCannedMessage</remarks>
        /// <returns></returns>
        public MessageSender AddCannedMessageInfo(string msgCode, int msgRevision)
        {
            if (_command != Comandos.DeleteCannedMessage)
                throw new Exception("Solo se puede agregar informacion de mensaje a comandos DeleteCannedMessage");
            AddParameter("msgCode", msgCode);
            AddParameter("msgRevision", msgRevision.ToString());
            return this;
        }

        public MessageSender AddCannedMessageCode(string msgCode)
        {
            AddParameter("msgCode", msgCode);
            Code = msgCode;
            return this;
        }

        public MessageSender AddMessageText(string msgText)
        {
            AddParameter("msgText", msgText);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <remarks>Solo válido para SetWorkflowState</remarks>
        /// <returns></returns>
        public MessageSender AddWorkflowState(string state)
        {
            if (_command != Comandos.SetWorkflowState)
                throw new Exception("Solo se puede agregar estado a comandos SetWorkflowState");
            AddParameter("newState", state);
            int stateAsInt, aux;
            int.TryParse(state, out stateAsInt);

            WorkflowMessage[] mensajes = (from m in DaoFactory.MensajeDAO.GetCannedMessagesTable(_dispositivo.Id, 0)
                                          where int.TryParse(m.Codigo, out aux)
                                          let code = Convert.ToInt32(m.Codigo)
                                          where code < 20 && code >= stateAsInt
                                          orderby code
                                          select new WorkflowMessage(Convert.ToInt32(m.Codigo), m.Texto))
                .ToArray();

            AddWorkflowList(mensajes);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <remarks>Solo válido para SetWorkflowState</remarks>
        /// <returns></returns>
        public MessageSender AddWorkflowList(WorkflowMessage[] list)
        {
            if (_command != Comandos.SetWorkflowState)
                throw new Exception("Solo se puede agregar WorkflowList a comandos SetWorkflowState");
            AddParameter("WorkflowList", new JavaScriptSerializer().Serialize(list));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imei"></param>
        /// <param name="classType"></param>
        /// <remarks>Solo válido para Add</remarks>
        /// <returns></returns>
        private void AddDeviceInfo(string imei, string classType)
        {
            if (_command != Comandos.Add)
                throw new Exception("Solo se puede agregar informacion del dispositivo a comandos Add");
            AddParameter("devIMEI", imei);
            AddParameter("devClassType", classType);
            return;
        }

        public MessageSender AddParameter(string name, string value)
        {
            _parameters.Add(name, value);
            return this;
        }

        #endregion

        protected DAOFactory DaoFactory
        {
            get { return _daoFactory ?? (_daoFactory = new DAOFactory()); }
        }

        public bool Send()
        {
            string parameters = string.Join("",
                                            _parameters.Select(p => string.Format("&{0}={1}", p.Key, Uri.EscapeDataString(p.Value))).ToArray());
            string commandText = string.Format("/Device.{0}?devId={1}&idNum={2}{3}", _command, _dispositivo.Id, _dispositivo.IdNum, parameters);

            Coche coche = DaoFactory.CocheDAO.FindMobileByDevice(_dispositivo.Id);
            bool encolado = Enqueue(commandText);
            string msg = "";

            if (_parameters.Keys.Contains("msgText"))
                msg = _parameters["msgText"];

            //if (_parameters.Keys.Contains("msgCode"))
            //{
            //    var cod = _parameters["msgCode"];
            //    var mensaje = DaoFactory.MensajeDAO.GetByCodigo(cod, coche.Empresa, coche.Linea);
            //    msg = mensaje.Texto;
            //}

            if (encolado && _saver != null) _saver.Save(Code, coche, DateTime.UtcNow, null, msg);

            return encolado;
        }

        /// <summary>
        /// Enqueues the givenn data in the commander queue
        /// </summary>
        /// <param name="label">The label of the message</param>
        /// <returns></returns>
        private bool Enqueue(string label)
        {
            try
            {
                IMessageQueue cola = GetCommandQueue(_dispositivo);

                cola.Send(label, label.Length < 200 ? label : label.Substring(0, 200));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the command queue for the current device.
        /// </summary>
        /// <returns></returns>
        public static IMessageQueue GetCommandQueue(Dispositivo dispositivo)
        {
            string queueName = dispositivo.TipoDispositivo.ColaDeComandos;

            var umq = new IMessageQueue(queueName);
            return !umq.LoadResources() ? null : umq;
        }

        #region Nested type: Comandos

        public static class Comandos
        {
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// devIMEI=[01231231231231231]
            /// devClassType=[Logictracker.Unetel.v1.Node, Logictracker.Unetel]
            /// </remarks>
            public const string Add = "Add";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// </remarks>
            public const string Delete = "Delete";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string Reboot = "Reboot";

            public const string RebootSolicitation = "RebootSolicitation";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// newState=[1]
            /// </remarks>
            public const string SetWorkflowState = "SetWorkflowState";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// msgCode=[23]
            /// </remarks>
            public const string SubmitCannedMessage = "SubmitCannedMessage";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// msgText=[texto_del_mensaje]
            /// </remarks>
            public const string SubmitTextMessage = "SubmitTextMessage";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// msgTextId=[id del mensaje]
            /// msgText=[texto_del_mensaje]
            /// replies=[array de Ids a posibles respuestas]
            /// </remarks>
            public const string SubmitCannedResponsesTextMessage = "SubmitCannedResponsesTextMessage";


            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// msgCode=[23]
            /// msgRevision=[543]
            /// </remarks>
            public const string DeleteCannedMessage = "DeleteCannedMessage";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// msgCode=[23]
            /// msgText=[texto_del_mensaje]
            /// msgRevision=[543]
            /// </remarks>
            public const string UpdateCannedMessage = "UpdateCannedMessage";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// msgCode=[23]
            /// msgText=[texto_del_mensaje]
            /// msgRevision=[543]
            /// </remarks>
            public const string UpdateCannedResponse = "UpdateCannedResponse";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// cfgParameter=[p1]
            /// cfgValue=[texto_del_config]
            /// cfgRevision=[543]
            /// </remarks>
            public const string SetParameter = "SetParameter";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string Qtree = "Qtree";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string FullQtree = "FullQtree";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string ResetStateMachine = "ResetStateMachine";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// route=[Destination[].ToJSONString()]
            /// routeId=[id_de_la_distribucion]
            /// </remarks>
            public const string LoadRoute = "LoadRoute";
            public const string ReloadRoute = "ReloadRoute";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// routeId=[id_de_la_distribucion]
            /// </remarks>
            public const string UnloadRoute = "UnloadRoute";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// routeId=[id_de_la_distribucion]
            /// </remarks>
            public const string UnloadStop = "UnloadStop";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string ReloadFirmware = "ReloadFirmware";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string ReloadMessages = "ReloadMessages";

            public const string ResetFMIOnGarmin = "ResetFMIOnGarmin";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string ReloadConfiguration = "ReloadConfiguration";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// from=[ddMMyyHHmmss]
            /// to=[ddMMyyHHmmss]
            /// </remarks>
            public const string RetrievePictures = "RetrievePictures";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// timerInterval=[123]
            /// </remarks>
            public const string ReportTemperature = "ReportTemperature";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string ReportTemperatureStop = "ReportTemperatureStop";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// immediately=[true_or_false]
            /// </remarks>
            public const string DisableFuel = "DisableFuel";

            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Parametros aceptados:
            /// devId=[123]
            /// trackingId=[112321212]
            /// timeToReceive=[1234]
            /// trackingExtraData=[string_devuelto_en_el_ack-nack_correspondiente]
            /// </remarks>
            public const string EnableFuel = "EnableFuel";

            public static string GetCode(string command)
            {
                switch (command)
                {
                    case Add:
                        return MessageCode.Add.GetMessageCode();
                    case Delete:
                        return MessageCode.Delete.GetMessageCode();
                    case Reboot:
                        return MessageCode.DeviceReboot.GetMessageCode();
                    case RebootSolicitation:
                        return MessageCode.DeviceRebootSolicitation.GetMessageCode();
                    case SetWorkflowState:
                        return MessageCode.SetWorkflowState.GetMessageCode();
                    case SubmitCannedMessage:
                        return MessageCode.SubmitCannedMessage.GetMessageCode();
                    case SubmitTextMessage:
                        return MessageCode.SubmitTextMessage.GetMessageCode();
                    case DeleteCannedMessage:
                        return MessageCode.DeleteCannedMessage.GetMessageCode();
                    case UpdateCannedMessage:
                        return MessageCode.UpdateCannedMessage.GetMessageCode();
                    case UpdateCannedResponse:
                        return MessageCode.UpdateCannedResponse.GetMessageCode();
                    case SetParameter:
                        return MessageCode.SetParameter.GetMessageCode();
                    case Qtree:
                        return MessageCode.Qtree.GetMessageCode();
                    case FullQtree:
                        return MessageCode.FullQtree.GetMessageCode();
                    case ResetStateMachine:
                        return MessageCode.ResetStateMachine.GetMessageCode();
                    case LoadRoute:
                        return MessageCode.LoadRoute.GetMessageCode();
                    case ReloadRoute:
                        return MessageCode.ReloadRoute.GetMessageCode();
                    case ReloadFirmware:
                        return MessageCode.ReloadFirmware.GetMessageCode();
                    case ReloadMessages:
                        return MessageCode.ReloadMessages.GetMessageCode();
                    case ReloadConfiguration:
                        return MessageCode.ReloadConfiguration.GetMessageCode();
                    case RetrievePictures:
                        return MessageCode.RetrievePictures.GetMessageCode();
                    case ReportTemperature:
                        return MessageCode.ReportTemperature.GetMessageCode();
                    case ReportTemperatureStop:
                        return MessageCode.ReportTemperatureStop.GetMessageCode();
                    case DisableFuel:
                        return MessageCode.DisableFuel.GetMessageCode();
                    case EnableFuel:
                        return MessageCode.EnableFuel.GetMessageCode();
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
    }
}