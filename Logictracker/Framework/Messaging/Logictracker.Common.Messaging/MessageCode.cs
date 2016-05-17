using System;
using System.Collections.Generic;

namespace Logictracker.Messaging
{
	public enum MessageCode
	{
        PrivacyOn,
        PrivacyOff,
        TareaRealizada,
        TareaNoRealizada,
		StoppedEvent,
		SpeedingTicket,
		SampeSpeedingTicket,
		EngineOff,
		EngineOn,
        DeviceRebootSolicitation,
		DeviceReboot,
		DeviceOnLine,
		DeviceOnNet,
		DeviceOnProxy,
		DeviceOffLine,
		InsideGeoRefference,
		OutsideGeoRefference,
		KilometersExceded,
		OutOfShiftActivity,
		GenericMessage,
		RfidDriverLogin,
		RfidDriverLogout,
		RfidEmployeeLogin,
		RfidEmployeeLogout,
		HardwareChange,
		DeviceShutdown,
		FotaSuccess,
		FotaFail,
		FotaPause,
		FotaStart,
        GarminStopStatus,
		QtreeStart,
		QtreePause,
		QtreeSuccess,
		ConfigurationSuccess,
		ConfigurationFail,
        ValidacionRuteo,
		NoReport,
		DocumentFirstWarning,
		DocumentSecondWarning,
		DocumentExpired,
        OdometerFirstWarning,
        OdometerSecondWarning,
        OdometerExpired,
        MixerClockwise,
        MixerClockwiseSlow,
        MixerClockwiseFast,
        MixerCounterClockwise,
        MixerCounterClockwiseSlow,
        MixerCounterClockwiseFast,
        MixerStopped,
        TolvaActivated,
        TolvaDeactivated,
        TextEvent,
        FirstPosition,
		RpmEvent,
		AccelerationEvent,
		DesaccelerationEvent,

        AtrasoTicket,
        EstadoLogisticoCumplido,
        EstadoLogisticoCumplidoManual,
        EstadoLogisticoCumplidoManualRealizado,
        EstadoLogisticoCumplidoManualNoRealizado,
        EstadoLogisticoCumplidoEntrada,
        EstadoLogisticoCumplidoSalida,
        CicloLogisticoIniciado,
        CicloLogisticoCerrado,
        CicloLogisticoIniciadoDocumentosInvalidos,
        
        DesvioRecorrido,
        VueltaAlRecorrido,

        TareaRevertida,

        RutaAceptada,
        RutaRechazada,

        PermanenciaEnGeocercaExcedida,
        PermanenciaEnGeocercaExcedidaEnCicloLogistico,

	    Create,
	    Add,
	    Delete,
	    SetWorkflowState,
	    SubmitCannedMessage,
	    SubmitTextMessage,
	    DeleteCannedMessage,
	    UpdateCannedMessage,
	    UpdateCannedResponse,
	    SetParameter,
	    Qtree,
	    FullQtree,
	    ResetStateMachine,
	    LoadRoute,
        ReloadRoute,
	    ReloadFirmware,
	    ReloadConfiguration,
        ReloadMessages,
	    RetrievePictures,
	    ReportTemperature,
	    ReportTemperatureStop,
	    DisableFuel,
	    EnableFuel,

        SensorUpperLimitExceeded,
        SensorLowerLimitExceeded,

        TemperatureInfo,
        TemperatureDisconected,
        TemperaturePowerDisconected,
        TemperaturePowerReconected,
        TemperatureThawingButtonPressed,
        TemperatureThawingButtonUnpressed,

        KeyboardButton1,
        KeyboardButton2,
        KeyboardButton3,
        KeyboardKeepAlive,
        SensorPowerDisconected,
        SensorPowerReconected,
        TemperatureEvent,
        TrailerUnHooked,
        TrailerHooked,

        BateryDisconected,
        BateryReConected,
        BateryLow,
        DeviceOpenned,
        CheckpointReached,

        GarminOn,
        GarminOff,
        GarminStopStatusActive,
        GarminStopStatusDone,
        GarminStopStatusUnreadInactive,
        GarminStopStatusReadInactive,
        GarminStopStatusDeleted,
        GarminStopStatusNa,
        GarminTextMessageCannedResponse,
        GarminETAReceived,
        RouteStatusCancelled,

        UnloadRouteSuccess,
        GarminCannedMessageReceived,

        PanicButtonOn,
        SirenOn,

        ServicioAsignado,
        ServicioAsignadoRechazado,
        ServicioAsignadoAceptado,
        LlegadaServicio,
        SolicitaAsistencia,
        ServicioFinalizado,
        ServicioPreasignado,
        ServicioPreasignadoRechazado,
        ServicioPreasignadoAceptado,
        ServicioPreasignadoCancelado,

        VehiculoAgendado
	}

	public static class MessageCodeX
	{
		/// <summary>
		/// List for containing all quality message codes associated to the messages that should be displayed at the quality monitor.
		/// </summary>
		public static readonly List<String> QualityMessages = new List<String>
	                                                      {
	                                                          MessageCode.DeviceOffLine.GetMessageCode(),
	                                                          MessageCode.DeviceOnLine.GetMessageCode(),
	                                                          MessageCode.DeviceOnNet.GetMessageCode(),
	                                                          MessageCode.DeviceOnProxy.GetMessageCode(),
	                                                          MessageCode.DeviceReboot.GetMessageCode(),
	                                                          MessageCode.EngineOff.GetMessageCode(),
	                                                          MessageCode.EngineOn.GetMessageCode(),
	                                                          MessageCode.SpeedingTicket.GetMessageCode(),
	                                                          MessageCode.StoppedEvent.GetMessageCode()
	                                                      };

		public static String GetMessageCode(this MessageCode messageCode)
		{
			// TO-DO: mergear esta informacion en el enum "MessageIdentifier" e imprimir aqui el valor en lugar del switch
			switch (messageCode)
			{
                case MessageCode.RutaAceptada: return "10";
                case MessageCode.RutaRechazada: return "11";

                case MessageCode.TareaRealizada: return "21";
                case MessageCode.TareaNoRealizada: return "22";
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
				case MessageCode.DeviceOffLine: return "121";
				case MessageCode.FotaSuccess: return "122";
				case MessageCode.FotaFail: return "123";
				case MessageCode.FotaPause: return "124";
				case MessageCode.FotaStart: return "125";
				case MessageCode.QtreeStart: return "126";
				case MessageCode.QtreePause: return "127";
				case MessageCode.QtreeSuccess: return "128";
				case MessageCode.ConfigurationSuccess: return "129";
				case MessageCode.ConfigurationFail: return "130";

                case MessageCode.PrivacyOn: return "140";
                case MessageCode.PrivacyOff: return "141";                 

                case MessageCode.AccelerationEvent: return "3001";
                case MessageCode.DesaccelerationEvent:return "3002";
                case MessageCode.RpmEvent: return "90";
                    
                // mensajes de pánico
                case MessageCode.PanicButtonOn: return "513";
                case MessageCode.SirenOn: return "2311";

				// mensajes generados por el dispatcher
				case MessageCode.StoppedEvent: return "100";
				case MessageCode.KilometersExceded: return "101";
				case MessageCode.OutOfShiftActivity: return "102";
				case MessageCode.DeviceReboot: return "105";
				case MessageCode.FirstPosition: return "110";
				case MessageCode.GenericMessage: return "111";
                case MessageCode.ValidacionRuteo: return "198";
				case MessageCode.NoReport: return "199";
                case MessageCode.UnloadRouteSuccess: return "202";
                case MessageCode.GarminCannedMessageReceived: return "203";
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

				// CicloLogístico
                case MessageCode.EstadoLogisticoCumplidoManual: return "1000";
                case MessageCode.AtrasoTicket: return "1001";
				case MessageCode.EstadoLogisticoCumplido: return "1002";
				case MessageCode.CicloLogisticoIniciado: return "1003";
				case MessageCode.CicloLogisticoCerrado: return "1004";
                case MessageCode.CicloLogisticoIniciadoDocumentosInvalidos: return "1005";
                case MessageCode.EstadoLogisticoCumplidoEntrada: return "1009";
                case MessageCode.EstadoLogisticoCumplidoSalida: return "1010";
                case MessageCode.EstadoLogisticoCumplidoManualRealizado: return "1013";
                case MessageCode.EstadoLogisticoCumplidoManualNoRealizado: return "1014";

                // Agenda Vehicular
                case MessageCode.VehiculoAgendado: return "1050";

                // Desvío de Recorrido
                case MessageCode.DesvioRecorrido: return "1006";
                case MessageCode.VueltaAlRecorrido: return "1007";

                // Tarea revertida
                case MessageCode.TareaRevertida: return "1008";

                // Mensajes Garmin
                case MessageCode.GarminStopStatus: return "145"; 
                case MessageCode.GarminOff: return "2305";
                case MessageCode.GarminOn: return "2306";                
                case MessageCode.GarminStopStatusActive: return "3010";
                case MessageCode.GarminStopStatusDone: return "3011";
		        case MessageCode.GarminStopStatusUnreadInactive: return "3012";
                case MessageCode.GarminStopStatusReadInactive: return "3013";
                case MessageCode.GarminStopStatusDeleted: return "3014";
		        case MessageCode.GarminStopStatusNa: return "3015";
                case MessageCode.GarminETAReceived: return "3019";
                case MessageCode.GarminTextMessageCannedResponse: return "3020";
                case MessageCode.RouteStatusCancelled: return "3050";                    

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
				case MessageCode.LoadRoute: return "4014";
				case MessageCode.ReloadFirmware: return "4015";
				case MessageCode.ReloadConfiguration: return "4016";
				case MessageCode.RetrievePictures: return "4017";
				case MessageCode.ReportTemperature: return "4018";
				case MessageCode.ReportTemperatureStop: return "4019";
				case MessageCode.DisableFuel: return "4020";
				case MessageCode.EnableFuel: return "4021";
                case MessageCode.DeviceRebootSolicitation: return "4050";

                //M2M
                case MessageCode.BateryDisconected: return "768";
                case MessageCode.BateryReConected: return "769";
                case MessageCode.BateryLow: return "770";
		        case MessageCode.DeviceOpenned: return "771";
                case MessageCode.CheckpointReached: return "772";


                case MessageCode.SensorUpperLimitExceeded: return "2801";
                case MessageCode.SensorLowerLimitExceeded: return "2802";
                case MessageCode.TemperatureInfo: return "2803";
                case MessageCode.TemperatureDisconected: return "2804";
                case MessageCode.TemperaturePowerDisconected: return "2805";
                case MessageCode.TemperaturePowerReconected: return "2806";
                case MessageCode.TemperatureThawingButtonPressed: return "2807";
                case MessageCode.TemperatureThawingButtonUnpressed: return "2808";
                case MessageCode.KeyboardButton1: return "2809";
                case MessageCode.KeyboardButton2: return "2810";
                case MessageCode.KeyboardButton3: return "2811";
		        case MessageCode.KeyboardKeepAlive: return "2812";
                case MessageCode.SensorPowerDisconected: return "2813";
                case MessageCode.SensorPowerReconected: return "2814";
                case MessageCode.TemperatureEvent: return "2815";
		        case MessageCode.TrailerUnHooked: return "2816";
                case MessageCode.TrailerHooked: return "2817";

                //Tiempo Máximo en Geocerca
                case MessageCode.PermanenciaEnGeocercaExcedida: return "5500";
                case MessageCode.PermanenciaEnGeocercaExcedidaEnCicloLogistico: return "5501";

                // del 6000 al 6099 son eventos definibles para Estado Logistico (Categoria ESLO0 y ESLO1)

                    // SOS
                case MessageCode.ServicioAsignado: return "10035";
                case MessageCode.ServicioAsignadoRechazado: return "10040";
                case MessageCode.ServicioAsignadoAceptado: return "10045";
                case MessageCode.LlegadaServicio: return "10050";
                case MessageCode.SolicitaAsistencia: return "10055";
                case MessageCode.ServicioFinalizado: return "10060";
                case MessageCode.ServicioPreasignado: return "10100";
                case MessageCode.ServicioPreasignadoRechazado: return "10105";
                case MessageCode.ServicioPreasignadoAceptado: return "10110";
                case MessageCode.ServicioPreasignadoCancelado: return "10115";

				default: return String.Empty;
			}
		}
	}
}