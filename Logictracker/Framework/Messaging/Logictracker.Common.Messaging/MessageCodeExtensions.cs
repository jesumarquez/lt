using System;
using System.Collections.Generic;
using Urbetrack.AVL;
using Urbetrack.Cache;
using Urbetrack.Common.Utils;
using Urbetrack.Model;

namespace Urbetrack.Common.Messaging
{
    public static class MessageCodeExtensions
    {
        #region Private Utility Lists

        /// <summary>
        /// List for containing all quality message codes associated to the messages that should be displayed at the quality monitor.
        /// </summary>
        public static readonly List<String> QualityMessages = new List<String>
	                                                      {
	                                                          MessageCode.DeviceOffline.GetMessageCode(),
	                                                          MessageCode.DeviceOnLine.GetMessageCode(),
	                                                          MessageCode.DeviceOnNet.GetMessageCode(),
	                                                          MessageCode.DeviceOnProxy.GetMessageCode(),
	                                                          MessageCode.DeviceReboot.GetMessageCode(),
	                                                          MessageCode.EngineOff.GetMessageCode(),
	                                                          MessageCode.EngineOn.GetMessageCode(),
	                                                          MessageCode.SpeedingTicket.GetMessageCode(),
	                                                          MessageCode.StoppedEvent.GetMessageCode()
	                                                      };

        private static readonly List<MessageIdentifier> NonGenericMessages = new List<MessageIdentifier>
	                                                                             {
                                                                                     MessageIdentifier.SwitchingOff,
                                                                                     MessageIdentifier.SwitchingOn,
	                                                                                 MessageIdentifier.HardwareChange,
	                                                                                 MessageIdentifier.DeviceShutdown,
	                                                                                 MessageIdentifier.DeviceOnLine,
	                                                                                 MessageIdentifier.DeviceOnNet,
	                                                                                 MessageIdentifier.DeviceOnProxy,
	                                                                                 MessageIdentifier.DeviceOffLine,
	                                                                                 MessageIdentifier.FotaSuccess,
	                                                                                 MessageIdentifier.FotaFail,
	                                                                                 MessageIdentifier.FotaPause,
	                                                                                 MessageIdentifier.FotaStart,
	                                                                                 MessageIdentifier.QtreeStart,
	                                                                                 MessageIdentifier.QtreePause,
	                                                                                 MessageIdentifier.QtreeSuccess,
	                                                                                 MessageIdentifier.ConfigSuccess,
	                                                                                 MessageIdentifier.ConfigFail,
	                                                                                 MessageIdentifier.RfidDetected,
	                                                                                 MessageIdentifier.SpeedingTicket,
																					 MessageIdentifier.TemperatureInfo,
																					 MessageIdentifier.TemperatureDisconected,
																					 MessageIdentifier.TemperaturePowerDisconected,
																					 MessageIdentifier.TemperaturePowerReconected,
																					 MessageIdentifier.TemperatureThawingButtonPressed,
																					 MessageIdentifier.TemperatureThawingButtonUnpressed,
	                                                                             };

        private static readonly List<MessageIdentifier> GenericMessagesWithExtraData = new List<MessageIdentifier>
	                                                                             {
                                                                                     MessageIdentifier.RpmEvent,
																					 MessageIdentifier.Picture,
																					 MessageIdentifier.TemperatureInfo,
																					 MessageIdentifier.TemperatureDisconected,
																					 MessageIdentifier.TemperaturePowerDisconected,
																					 MessageIdentifier.TemperaturePowerReconected,
																					 MessageIdentifier.TemperatureThawingButtonPressed,
																					 MessageIdentifier.TemperatureThawingButtonUnpressed,
                                                                                     // MessageIdentifier.AccelerationEvent,
	                                                                                 // MessageIdentifier.DesaccelerationEvent, // el gte no guarda datos de la frenada...
	                                                                             };

        private static readonly List<MessageIdentifier> TemperatureMessages = new List<MessageIdentifier>
	                                                                             {
																					 MessageIdentifier.TemperatureInfo,
																					 MessageIdentifier.TemperatureDisconected,
																					 MessageIdentifier.TemperaturePowerDisconected,
																					 MessageIdentifier.TemperaturePowerReconected,
																					 MessageIdentifier.TemperatureThawingButtonPressed,
																					 MessageIdentifier.TemperatureThawingButtonUnpressed,
	                                                                             };

        #endregion

        #region Public Methods

        public static String GetMessageCode(this MessageCode messageCode)
        {
            // TO-DO: Llevar esta informacion a la base de datos, indicando nombre del evento, identificador y codigo asociado.
            switch (messageCode)
            {
                case MessageCode.SpeedingTicket: return "92";
                case MessageCode.RfidDriverLogin: return "93";
                case MessageCode.RfidDriverLogout: return "94";
                case MessageCode.RfidEmployeeLogin: return "95";
                case MessageCode.RfidEmployeeLogout: return "96";
                case MessageCode.EngineOff: return "112";
                case MessageCode.EngineOn: return "113";
                case MessageCode.HardwareChange: return "114";
                case MessageCode.DeviceShutdown: return "115";
                case MessageCode.DeviceOnLine: return "118";
                case MessageCode.DeviceOnNet: return "119";
                case MessageCode.DeviceOnProxy: return "120";
                case MessageCode.DeviceOffline: return "121";
                case MessageCode.FotaSuccess: return "122";
                case MessageCode.FotaFail: return "123";
                case MessageCode.FotaPause: return "124";
                case MessageCode.FotaStart: return "125";
                case MessageCode.QtreeStart: return "126";
                case MessageCode.QtreePause: return "127";
                case MessageCode.QtreeSuccess: return "128";
                case MessageCode.ConfigurationSuccess: return "129";
                case MessageCode.ConfigurationFail: return "130";

                //mensajes generados por el dispatcher
                case MessageCode.StoppedEvent: return "100";
                case MessageCode.KilometersExceded: return "101";
                case MessageCode.OutOfShiftActivity: return "102";
                case MessageCode.DeviceReboot: return "105";
                case MessageCode.FirstPosition: return "110";
                case MessageCode.GenericMessage: return "111";
                case MessageCode.NoReport: return "199";
                case MessageCode.DocumentExpired: return "300";
                case MessageCode.DocumentFirstWarning: return "301";
                case MessageCode.DocumentSecondWarning: return "302";
                case MessageCode.OdometerExpired: return "310";
                case MessageCode.OdometerFirstWarning: return "311";
                case MessageCode.OdometerSecondWarning: return "312";
                case MessageCode.InsideGeoRefference: return "910";
                case MessageCode.OutsideGeoRefference: return "920";
                case MessageCode.TextEvent: return "999";
                case MessageCode.MixerStopped: return "2064";
                case MessageCode.MixerClockwise: return "2065";
                case MessageCode.MixerCounterClockwise: return "2066";
                case MessageCode.MixerCounterClockwiseFast: return "2069";
                case MessageCode.MixerClockwiseFast: return "2070";
                case MessageCode.MixerCounterClockwiseSlow: return "2071";
                case MessageCode.MixerClockwiseSlow: return "2072";
                case MessageCode.TolvaDeactivated: return "2080";
                case MessageCode.TolvaActivated: return "2081";

                // M2M
                case MessageCode.TemperatureInfo: return "2803";
                case MessageCode.TemperatureDisconected: return "2804";
                case MessageCode.TemperaturePowerDisconected: return "2805";
                case MessageCode.TemperaturePowerReconected: return "2806";
                case MessageCode.TemperatureThawingButtonPressed: return "2807";
                case MessageCode.TemperatureThawingButtonUnpressed: return "2808";
                    	
                // CicloLogístico
                case MessageCode.AtrasoTicket: return "1001";
                case MessageCode.EstadoLogisticoCumplido: return "1002";
                case MessageCode.CicloLogisticoIniciado: return "1003";
                case MessageCode.CicloLogisticoCerrado: return "1004";

                // Mensajes generados por el usuario
                case MessageCode.Create: return "4000";
	            case MessageCode.Add: return "4001";
                case MessageCode.Delete: return "4002";
                case MessageCode.SetWorkflowState: return "4003";
                case MessageCode.SubmitCannedMessage: return "4004";
                case MessageCode.SubmitTextMessage: return "4005";
                case MessageCode.DeleteCannedMessage: return "4006";
                case MessageCode.UpdateCannedMessage: return "4007";
                case MessageCode.UpdateCannedResponse: return "4008";
                case MessageCode.SetParameter: return "4009";
                case MessageCode.Qtree: return "4010";
                case MessageCode.FullQtree: return "4011";
                case MessageCode.ResetStateMachine: return "4012";
                case MessageCode.CancelMessage: return "4013";
                case MessageCode.LoadRoute: return "4014";
                case MessageCode.ReloadFirmware: return "4015";
                case MessageCode.ReloadConfiguration: return "4016";
                case MessageCode.RetrievePictures: return "4017";
                case MessageCode.ReportTemperature: return "4018";
                case MessageCode.ReportTemperatureStop: return "4019";
                case MessageCode.DisableFuel: return "4020";
                case MessageCode.EnableFuel: return "4021";

                default: return String.Empty;
            }
        }

        public static String GetExtraDataText(this IExtraData ev, MessageIdentifier messageIdentifier)
        {
            switch (messageIdentifier)
            {
                //case MessageIdentifier.AccelerationEvent:
                //case MessageIdentifier.DesaccelerationEvent:
                case MessageIdentifier.RpmEvent: // vel - rpm
                    return ev.Data.Count >= 3 ? String.Format(" - {0} - {1}", ev.Data[1], ev.Data[2]) : String.Empty;
                case MessageIdentifier.Picture:
                    return ev.Data.Count >= 2 ? String.Format(" - {0}", ev.Data[1]) : String.Empty;
				case MessageIdentifier.TemperatureInfo:
				case MessageIdentifier.TemperatureDisconected:
				case MessageIdentifier.TemperaturePowerDisconected:
				case MessageIdentifier.TemperaturePowerReconected:
				case MessageIdentifier.TemperatureThawingButtonPressed:
				case MessageIdentifier.TemperatureThawingButtonUnpressed:
					return ev.Data.Count >= 1 ? String.Format(" - {0:000.00}", ev.Data[0] / (float)100.0) : String.Empty;
            }

            return String.Empty;
        }

        public static String GetMessageCode(short identifier, Int32 data, String auxData)
        {
            var messageIdentifier = (MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), identifier.ToString(), true);

            if (messageIdentifier.Equals(MessageIdentifier.RfidDetected)) return GetRfidAction(data, auxData);

            if (IsEstadoLogistico(data)) return data.ToString();

            return (identifier.ToString() == MessageCode.SpeedingTicket.GetMessageCode()) ? MessageCode.SpeedingTicket.GetMessageCode() : GetCode(identifier, data);
        }

        public static bool IsEstadoLogistico(int data) { return data > 0 && data < 20; }

        public static bool IsTemperatureMessage(String code)
        {
        	return TemperatureMessages.Contains((MessageIdentifier) Enum.Parse(typeof (MessageIdentifier), code, true));
        }

        public static Boolean IsRfidEvent(short identifier) { return ((MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), identifier.ToString(), true)).Equals(MessageIdentifier.RfidDetected); }

        public static bool IsGeneric(this MessageIdentifier code) { return !NonGenericMessages.Contains(code); }

		public static bool IsGeneric(short code) { return ((MessageIdentifier)code).IsGeneric(); }

        public static bool HasExtraData(this MessageIdentifier code) { return GenericMessagesWithExtraData.Contains(code); }

		public static IMessage FactoryRfid(int DeviceId, ulong msgId, GPSPoint pos, DateTime dt, String rfid, int data) { return FactoryEvent(MessageIdentifier.RfidDetected, DeviceId, msgId, pos, dt, rfid, new List<int>{data}); }

		public static IMessage FactoryEvent(this MessageIdentifier code, int DeviceId, ulong msgId, GPSPoint pos, DateTime dt, String rfid, List<int> data) { return new Event((short)code, DeviceId, msgId, pos, dt, rfid, data, code.IsGeneric()); }

        #endregion

        #region Private Methods

        private static String GetCode(short identifier, Int32 data)
        {
            var messageIdentifier = (MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), identifier.ToString(), true);

            switch (messageIdentifier)
            {
                case MessageIdentifier.SwitchingOff: return MessageCode.EngineOff.GetMessageCode();
                case MessageIdentifier.SwitchingOn: return MessageCode.EngineOn.GetMessageCode();
                case MessageIdentifier.HardwareChange: return MessageCode.HardwareChange.GetMessageCode();
                case MessageIdentifier.DeviceShutdown: return MessageCode.DeviceShutdown.GetMessageCode();
                case MessageIdentifier.DeviceOnLine: return MessageCode.DeviceOnLine.GetMessageCode();
                case MessageIdentifier.DeviceOnNet: return MessageCode.DeviceOnNet.GetMessageCode();
                case MessageIdentifier.DeviceOnProxy: return MessageCode.DeviceOnProxy.GetMessageCode();
                case MessageIdentifier.DeviceOffLine: return MessageCode.DeviceOffline.GetMessageCode();
                case MessageIdentifier.FotaSuccess: return MessageCode.FotaSuccess.GetMessageCode();
                case MessageIdentifier.FotaFail: return MessageCode.FotaFail.GetMessageCode();
                case MessageIdentifier.FotaPause: return MessageCode.FotaPause.GetMessageCode();
                case MessageIdentifier.FotaStart: return MessageCode.FotaStart.GetMessageCode();
                case MessageIdentifier.QtreeStart: return MessageCode.QtreeStart.GetMessageCode();
                case MessageIdentifier.QtreePause: return MessageCode.QtreePause.GetMessageCode();
                case MessageIdentifier.QtreeSuccess: return MessageCode.QtreeSuccess.GetMessageCode();
                case MessageIdentifier.ConfigSuccess: return MessageCode.ConfigurationSuccess.GetMessageCode();
                case MessageIdentifier.ConfigFail: return MessageCode.ConfigurationFail.GetMessageCode();

                case MessageIdentifier.GenericMessage: return data.ToString();

                default: return messageIdentifier.ToString("d");
            }
        }

        private static String GetRfidAction(Int32 data, String auxData)
        {
            switch (data)
            {
                case 0: return MessageCode.RfidDriverLogin.GetMessageCode();
                case 1: return MessageCode.RfidDriverLogout.GetMessageCode();
                case 2: return MessageCode.RfidDriverLogout.GetMessageCode(); // Logout por contacto.
				case 3: return MessageCode.RfidEmployeeLogin.GetMessageCode();
				case 4: return MessageCode.RfidEmployeeLogout.GetMessageCode();
                case 0xFF: return GetEmployeeRfidAction(auxData);
            }

            return String.Empty;
        }

        private static String GetEmployeeRfidAction(String rfid)
        {
            var key = String.Format("employeeRfid:{0}", rfid);

            if (UrbeCache.KeyExists(typeof(String), key))
            {
                UrbeCache.Delete(typeof(String), key);

                return MessageCode.RfidEmployeeLogout.GetMessageCode();
            }

            UrbeCache.Store(typeof(String), key, rfid);

            return MessageCode.RfidEmployeeLogin.GetMessageCode();
        }

        #endregion
    }
}
