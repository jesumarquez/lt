using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Layers.MessageQueue;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Messages.Sender
{
    public class M2mMessageSender
    {
        private readonly IM2mMessageSaver _saver;
        private readonly Dispositivo _dispositivo;
        private DAOFactory _daoFactory;
        private string _command;
        public string Code;
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>(3);
        
        protected DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }

        protected M2mMessageSender(Dispositivo dispositivo, IM2mMessageSaver saver)
        {
            _dispositivo = dispositivo;
            _saver = saver;
        }

        public static M2mMessageSender Create(Dispositivo dispositivo, IM2mMessageSaver saver) { return new M2mMessageSender(dispositivo, saver); }
        
        public M2mMessageSender AddCommand(string command)
        {
            _command = command;
            Code = Comandos.GetCode(command);
            if (_command == Comandos.Add) AddDeviceInfo(_dispositivo.Imei, _dispositivo.TipoDispositivo.Fabricante);
            return this;
        }

        public M2mMessageSender AddConfigParameter(string cfgParameter, string cfgValue, int cfgRevision)
        {
            if (_command != Comandos.SetParameter) throw new Exception("Solo se pueden agregar parametros de configuracion a un comando SetParameter");
            AddParameter("cfgParameter", cfgParameter);
            AddParameter("cfgValue", cfgValue);
            AddParameter("cfgRevision", cfgRevision.ToString());
            return this;
        }
        
        private void AddDeviceInfo(string imei, string classType)
        {
            if (_command != Comandos.Add) throw new Exception("Solo se puede agregar informacion del dispositivo a comandos Add");
            AddParameter("devIMEI", imei);
            AddParameter("devClassType", classType);
    		return;
        }
        
        public M2mMessageSender AddParameter(string name, string value)
        {
            _parameters.Add(name, value);
            return this;
        } 

        public bool Send()
        {
            var parameters = string.Join("",_parameters.Select(p => string.Format("&{0}={1}", p.Key, p.Value)).ToArray());
            var commandText = string.Format("/Device.{0}?devId={1}&idNum={2}{3}", _command, _dispositivo.Id, _dispositivo.IdNum, parameters);
            
            var sensor = DaoFactory.SensorDAO.FindByDispositivo(_dispositivo.Id);
            var subentidad = DaoFactory.SubEntidadDAO.FindBySensor(sensor.Id);
            var encolado = Enqueue(commandText);
            var msg = "";

            if (_parameters.Keys.Contains("msgText"))
                msg = _parameters["msgText"];
            
            if (encolado && _saver != null) _saver.Save(Code, _dispositivo, sensor, subentidad, DateTime.UtcNow, DateTime.UtcNow, msg);

            return encolado;
        }

        private bool Enqueue(string label)
        {
            try
            {
                var cola = GetCommandQueue(_dispositivo);

                cola.Send(label, label.Length < 200 ? label : label.Substring(0, 200));

                return true;
            }
            catch { return false; }
        }

        public static IMessageQueue GetCommandQueue(Dispositivo dispositivo)
        {
            var queueName = dispositivo.TipoDispositivo.ColaDeComandos;

        	var umq = new IMessageQueue(queueName);
        	return !umq.LoadResources() ? null : umq;
        }

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
            /// </remarks>
            public const string LoadRoute = "LoadRoute";
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
                    case Add: return MessageCode.Add.GetMessageCode();
                    case Delete: return MessageCode.Delete.GetMessageCode();
                    case Reboot: return MessageCode.DeviceReboot.GetMessageCode();
                    case SetWorkflowState: return MessageCode.SetWorkflowState.GetMessageCode();
                    case SubmitCannedMessage: return MessageCode.SubmitCannedMessage.GetMessageCode();
                    case SubmitTextMessage: return MessageCode.SubmitTextMessage.GetMessageCode();
                    case DeleteCannedMessage: return MessageCode.DeleteCannedMessage.GetMessageCode();
                    case UpdateCannedMessage: return MessageCode.UpdateCannedMessage.GetMessageCode();
                    case UpdateCannedResponse: return MessageCode.UpdateCannedResponse.GetMessageCode();
                    case SetParameter: return MessageCode.SetParameter.GetMessageCode();
                    case Qtree: return MessageCode.Qtree.GetMessageCode();
                    case FullQtree: return MessageCode.FullQtree.GetMessageCode();
                    case ResetStateMachine: return MessageCode.ResetStateMachine.GetMessageCode();
                    case LoadRoute: return MessageCode.LoadRoute.GetMessageCode();
                    case ReloadFirmware: return MessageCode.ReloadFirmware.GetMessageCode();
                    case ReloadConfiguration: return MessageCode.ReloadConfiguration.GetMessageCode();
                    case RetrievePictures: return MessageCode.RetrievePictures.GetMessageCode();
                    case ReportTemperature: return MessageCode.ReportTemperature.GetMessageCode();
                    case ReportTemperatureStop: return MessageCode.ReportTemperatureStop.GetMessageCode();
                    case DisableFuel: return MessageCode.DisableFuel.GetMessageCode();
                    case EnableFuel: return MessageCode.EnableFuel.GetMessageCode();
                    default: return string.Empty;
                }
            }
        }
    }
}