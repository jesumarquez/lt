#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using Logictracker.AVL.Messages;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;

#endregion

namespace Logictracker.Messaging
{
    public enum MessageIdentifier
    {
		ReservedDoNotUse = 0,
        EngineOffInternal = 1,
        EngineOnInternal = 2,
        HardwareChangeInternal = 3,
        DeviceShutdownInternal = 4,
        DeviceOnLineInternal = 5,
        DeviceOnNetInternal = 6,
        DeviceOnProxyInternal = 7,
        DeviceOffLineInternal = 8,

        // Aceptación de rutas
        RutaAceptada = 10,
        RutaRechazada = 11,

        TareaRealizada = 21,
        TareaNoRealizada = 22,
        RebootTimeout = 32,
        RebootSuccess = 33,
		RpmTicketInit = 89,
		RpmTicketEnd = 90,
		SpeedingTicketInit = 91,
		SpeedingTicketEnd = 92,
		RfidDriverLogin = 93,
        RfidDriverLogout = 94,
        RfidEmployeeLogin = 95,
        RfidEmployeeLogout = 96,
		StartMovementEvent = 99,
		StoppedEvent = 100,
		KilometersExceded = 101,
        OutOfShiftActivity = 102,
        DeviceTurnedOn = 105,
		ResetLostGeogrilla = 106,
		RegainGeogrilla = 107,
        FirstPosition = 110,
		GenericMessageInternal = 111,
        EngineOff = 112,
        EngineOn = 113,
        HardwareChange = 114,
		DeviceShutdown = 115,
		BeaconOff = 116,
        BeaconOn = 117,
        DeviceOnLine = 118,
        DeviceOnNet = 119,
        DeviceOnProxy = 120,
		DeviceOffLine = 121,
        FotaSuccess = 122,
        FotaFail = 123,
        FotaPause = 124,
        FotaStart = 125,
        QtreeStart = 126,
        QtreePause = 127,
        QtreeSuccess = 128,
        ConfigSuccess = 129,
        ConfigFail = 130,
        SlowingTicket = 131,
        DoorOpenned = 132,
        DoorClosed = 133,
        ConfigStart = 134,
        DisplayProblem = 135,
        //DisplayOk = 136, //obsolete
		KeepAlive = 137,
		KeepAliveEvent = 138,
		TechnicalService = 139,
        PrivacyOn = 140,
        PrivacyOff = 141,
        GarminStopStatusChanged = 145,
        MessagesStart = 150,
        MessagesSuccess = 151,
        ReloadRouteSuccess = 197,
		LoadRouteSuccess = 198,
		NoReport = 199,
        GpsSignalOff = 200,
        GpsSignalOn = 201,
        UnloadRouteSuccess = 202,
        GarminCannedMessageReceived = 203,
        //MainPowerOn = 202, //obsolete
		//MainPowerOff = 203, //obsolete
		CustomMsg1On = 204,
		CustomMsg1Off = 205,
		CustomMsg2On = 206,
		CustomMsg2Off = 207,
		CustomMsg3On = 208,
		CustomMsg3Off = 209,
		StoppedEventSampeV2 = 257,
		DocumentExpired = 300,
		DocumentFirstWarning = 301,
		DocumentSecondWarning = 302,
		OdometerExpired = 310,
		OdometerFirstWarning = 311,
		OdometerSecondWarning = 312,
        PanicButtonOff = 512,
        PanicButtonOn = 513,
        Picture = 514,
		PowerKeyShortPress = 515,
		Freefall = 516,
		BlackcallIncoming = 517,
		AckEvent = 666,
        BateryDisconected = 768,
        BateryReConected = 769,
        BateryLow = 770,
		DeviceOpenned = 771,
		CheckpointReached = 772,
		BateryInfo = 773,
		UsingInternalAntenna = 774,
		UsingExternalAntenna = 775,
		BatteryChargingStart = 776,
		BatteryChargingStop = 777,
        
		//Cusat
		DoorApCabinActive = 800, //ap_puerta_cabina_activa
		DisengageActive = 801, //desenganche_activa
		SubstituteViolation = 802, //violacion_sustituto
		DoorApVanActive = 803, //ap_puerta_furgon_activa
		DoorApCabinPassive = 804, //ap_puerta_cabina_pasiva
		DisengagePassive = 805, //desenganche_pasiva
		DoorApVanPassive = 806, //ap_puerta_furgon_pasiva
		ActiveAlertsInhibitorButtonOn = 807, //boton_inhibidor_de_alertas_activas_presionado
        
		InsideGeoRefference = 910,
        OutsideGeoRefference = 920,
		TextEvent = 999,

		// CicloLogístico
        LogisticStateFulfilledManual = 1000,
        TicketOverdue = 1001,
		LogisticStateFulfilled = 1002,
		LogisticsCycleInitiated = 1003,
		LogisticsCycleTerminated = 1004,
        LogisticsCycleInitiatedWithInvalidDocuments = 1005,

        DesvioRecorrido = 1006,
        VueltaAlRecorrido = 1007,

        TareaRevertida = 1008,

        LogisticStateFulfilledEnter = 1009,
        LogisticStateFulfilledExit = 1010,

		JammingOn = 1011,
		JammingOff = 1012,

        LogisticStateFulfilledManualRealized = 1013,
        LogisticStateFulfilledManualNotRealized = 1014,

		RfidDetected = 1024,
        NoDriverMovement = 1025,

        DigitalInput00Open = 1100,
        DigitalInput01Open = 1101,
        DigitalInput02Open = 1102,
        DigitalInput03Open = 1103,
        DigitalInput04Open = 1104,
        DigitalInput05Open = 1105,
        DigitalInput00Closed = 1110,
        DigitalInput01Closed = 1111,
        DigitalInput02Closed = 1112,
        DigitalInput03Closed = 1113,
        DigitalInput04Closed = 1114,
        DigitalInput05Closed = 1115,

        MixerStopped = 2064,
        MixerClockwise = 2065,
        MixerCounterClockwise = 2066,
        PosibleSpinRight = 2067,
        PosibleSpinLeft = 2068,
        MixerCounterClockwiseFast = 2069,
        MixerClockwiseFast = 2070,
        MixerCounterClockwiseSlow = 2071,
        MixerClockwiseSlow = 2072,
        SpinStop2 = 2073,
		TolvaDeactivated = 2080,
        TolvaActivated = 2081,
		NoEngineMovement = 2304,
        GarminOff = 2305,
        GarminOn = 2306,        
        GsmSignalOff = 2307,
        GsmSignalOn = 2308,
        GpsSignal3DOff = 2309,
        SirenOff = 2310,
        SirenOn = 2311,
        PistonOff = 2312,
        PistonOn = 2313,
        SubmitSm = 2314,
        UndeliverableSm = 2315,
        ShutdownDelayed = 2316,
        MessageTableSuccess = 2317,
        UnsuportedCmd = 2318,
        GpsSignal2DOn = 2319,
		SensorUpperLimitExceeded = 2801,
		SensorLowerLimitExceeded = 2802,
		TemperatureInfo = 2803,
		TemperatureDisconected = 2804,
		TemperaturePowerDisconected = 2805,
		TemperaturePowerReconected = 2806,
		TemperatureThawingButtonPressed = 2807,
		TemperatureThawingButtonUnpressed = 2808,
		KeyboardButton1 = 2809,
		KeyboardButton2 = 2810,
		KeyboardButton3 = 2811,
		KeyboardKeepAlive = 2812,
		SensorPowerDisconected = 2813,
		SensorPowerReconected = 2814,
        TemperatureEvent = 2815,
		TrailerUnHooked = 2816,
        TrailerHooked = 2817,
        FuelEnabled = 2840,
        FuelDisabled = 2841,

		//--
		TelemetricData = 2850,
		TelemetricEvent = 2851,
		//--

		//RpmEvent = 3000,
		AccelerationEvent = 3001,
        DesaccelerationEvent = 3002,

		//StopStatus
        GarminStopStatusActive = 3010,
        GarminStopStatusDone = 3011,
		GarminStopStatusUnreadInactive = 3012,
        GarminStopStatusReadInactive = 3013,
		GarminStopStatusDeleted = 3014,
		GarminStopStatusNa = 3015,

        // Garmin ETA
        GarminETA = 3019,

        // Garmin Text Message Canned Response
        GarminTextMessageCannedResponse = 3020,

        //StopAddress
        StopAddressChanged = 3030,

        //RouteStatus
		RouteStatusCancelled = 3050,

        //desvio de combustible
        ConsumptionDeviationHigh = 3100,
        ConsumptionDeviationLow = 3101,

        //alarma stock
        StockReposition = 3102,
        StockCritic = 3103,

		//Caudalimetro
		CaudalimeterTelemetricData = 3105,

		//Tacometro
		TacometerData = 3106,

		//Entradas Analogicas
		AnalogicInputs = 3107,

		DeviceMilked = 3110,

		//vending machines
		ReadError = 3120,
		PowerDisconnected = 3121,
		PowerReconnected = 3122,
		VendingTelemetricData = 3223,

		//can bus
		ExcessiveExhaustAlarm = 3200,

        SleepModeOn = 3300,
        SleepModeOff = 3301,
        GPSAntennaShort = 3302,
        GPSAntennaDisconnected = 3303,


        // Mensajes generados por el usuario
        UserMessageCreate = 4000,
	    UserMessageAdd = 4001,
        UserMessageDelete = 4002,
        UserMessageSetWorkflowState = 4003,
        UserMessageSubmitCannedMessage = 4004,
        UserMessageSubmitTextMessage = 4005,
        UserMessageDeleteCannedMessage = 4006,
        UserMessageUpdateCannedMessage = 4007,
        UserMessageUpdateCannedResponse = 4008,
        UserMessageSetParameter = 4009,
        UserMessageQtree = 4010,
        UserMessageFullQtree = 4011,
        UserMessageResetStateMachine = 4012,
        UserMessageCancelMessage = 4013,
        UserMessageLoadRoute = 4014,
        UserMessageReloadFirmware = 4015,
        UserMessageReloadConfiguration = 4016,
        UserMessageRetrievePictures = 4017,
        UserMessageReportTemperature = 4018,
        UserMessageReportTemperatureStop = 4019,
        UserMessageDisableFuel = 4020,
        UserMessageEnableFuel = 4021,

        PermanenciaEnGeocercaExcedida = 5000,
        PermanenciaEnGeocercaExcedidaEnCicloLogistico = 5001,

        CustomEvent0001 = 7000,
        CustomEvent0002 = 7001,
        CustomEvent0003 = 7002,
        CustomEvent0004 = 7003,
        CustomEvent0005 = 7004,
        CustomEvent0006 = 7005,
        CustomEvent0007 = 7006,
        CustomEvent0008 = 7007,
        CustomEvent0009 = 7008,
        CustomEvent0010 = 7009,
        CustomEvent0011 = 7010,
        CustomEvent0012 = 7011,
        CustomEvent0013 = 7012,
        CustomEvent0014 = 7013,
        CustomEvent0015 = 7014,
        CustomEvent0016 = 7015,
        CustomEvent0017 = 7016,
        CustomEvent0018 = 7017,
        CustomEvent0019 = 7018,
        CustomEvent0020 = 7019,
        CustomEvent0021 = 7020,
        CustomEvent0022 = 7021,
        CustomEvent0023 = 7022,
        CustomEvent0024 = 7023,
        CustomEvent0025 = 7024,
        CustomEvent0026 = 7025,
        CustomEvent0027 = 7026,
        CustomEvent0028 = 7027,
        CustomEvent0029 = 7028,
        CustomEvent0030 = 7029,
        CustomEvent0031 = 7030,
        CustomEvent0032 = 7031,
        CustomEvent0033 = 7032,
        CustomEvent0034 = 7033,
        CustomEvent0035 = 7034,
        CustomEvent0036 = 7035,
        CustomEvent0037 = 7036,
        CustomEvent0038 = 7037,
        CustomEvent0039 = 7038,
        CustomEvent0040 = 7039,
        CustomEvent0041 = 7040,
        CustomEvent0042 = 7041,
        CustomEvent0043 = 7042,
        CustomEvent0044 = 7043,
        CustomEvent0045 = 7044,
        CustomEvent0046 = 7045,
        CustomEvent0047 = 7046,
        CustomEvent0048 = 7047,
        CustomEvent0049 = 7048,
        CustomEvent0050 = 7049,
        CustomEvent0051 = 7050,
        CustomEvent0052 = 7051,
        CustomEvent0053 = 7052,
        CustomEvent0054 = 7053,
        CustomEvent0055 = 7054,
        CustomEvent0056 = 7055,
        CustomEvent0057 = 7056,
        CustomEvent0058 = 7057,
        CustomEvent0059 = 7058,
        CustomEvent0060 = 7059,
        CustomEvent0061 = 7060,
        CustomEvent0062 = 7061,
        CustomEvent0063 = 7062,
        CustomEvent0064 = 7063,
        CustomEvent0065 = 7064,
        CustomEvent0066 = 7065,
        CustomEvent0067 = 7066,
        CustomEvent0068 = 7067,
        CustomEvent0069 = 7068,
        CustomEvent0070 = 7069,
        CustomEvent0071 = 7070,
        CustomEvent0072 = 7071,
        CustomEvent0073 = 7072,
        CustomEvent0074 = 7073,
        CustomEvent0075 = 7074,
        CustomEvent0076 = 7075,
        CustomEvent0077 = 7076,
        CustomEvent0078 = 7077,
        CustomEvent0079 = 7078,
        CustomEvent0080 = 7079,
        CustomEvent0081 = 7080,
        CustomEvent0082 = 7081,
        CustomEvent0083 = 7082,
        CustomEvent0084 = 7083,
        CustomEvent0085 = 7084,
        CustomEvent0086 = 7085,
        CustomEvent0087 = 7086,
        CustomEvent0088 = 7087,
        CustomEvent0089 = 7088,
        CustomEvent0090 = 7089,
        CustomEvent0091 = 7090,
        CustomEvent0092 = 7091,
        CustomEvent0093 = 7092,
        CustomEvent0094 = 7093,
        CustomEvent0095 = 7094,
        CustomEvent0096 = 7095,
        CustomEvent0097 = 7096,
        CustomEvent0098 = 7097,
        CustomEvent0099 = 7098,
        CustomEvent0100 = 7099,

        // del 6000 al 6099 son eventos definibles para Estado Logistico (Categoria ESLO0 y ESLO1)        
		GenericMessage = Event.GenericMessage, // = 32767; //0x7FFF,
        NoMessage = 9999
    }

	public static class MessageIdentifierX
	{
		#region Private Utility Lists

		private static readonly List<MessageIdentifier> NonGenericMessages = new List<MessageIdentifier>
			{
				MessageIdentifier.EngineOffInternal,
				MessageIdentifier.EngineOnInternal,
				MessageIdentifier.HardwareChangeInternal,
				MessageIdentifier.DeviceShutdownInternal,
				MessageIdentifier.DeviceOnLineInternal,
				MessageIdentifier.DeviceOnNetInternal,
				MessageIdentifier.DeviceOnProxyInternal,
				MessageIdentifier.DeviceOffLineInternal,
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
				MessageIdentifier.SpeedingTicketInit,
				MessageIdentifier.SpeedingTicketEnd,
				MessageIdentifier.TelemetricData,
				MessageIdentifier.TelemetricEvent,
			};

		private static readonly List<MessageIdentifier> GenericMessagesWithExtraData = new List<MessageIdentifier>
			{
				MessageIdentifier.Picture,
				MessageIdentifier.BlackcallIncoming,
				MessageIdentifier.AccelerationEvent,
				MessageIdentifier.DesaccelerationEvent,
				MessageIdentifier.BateryInfo,
			};

		private static readonly List<MessageIdentifier> EntityMessages = new List<MessageIdentifier>
			{
				MessageIdentifier.TelemetricData,
				MessageIdentifier.TelemetricEvent
			};

		#endregion

		#region Public Methods

		public static bool IsEntityMessage(String code) { return EntityMessages.Contains((MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), code, true)); }

		public static Boolean IsRfidEvent(short identifier) { return ((MessageIdentifier)Enum.Parse(typeof(MessageIdentifier), identifier.ToString(CultureInfo.InvariantCulture), true)).Equals(MessageIdentifier.RfidDetected); }

		public static bool HasExtraData(this MessageIdentifier code) { return GenericMessagesWithExtraData.Contains(code); }

		public static Event FactoryRfid(int deviceId, ulong msgId, GPSPoint pos, DateTime dt, String rfid, Int64 data) { return FactoryEvent(MessageIdentifier.RfidDetected, deviceId, msgId, pos, dt, rfid, new List<Int64> { data }); }

		public static Event FactoryEvent(this MessageIdentifier code, int deviceId, ulong msgId, GPSPoint pos, DateTime dt, String rfid, IEnumerable<Int64> data) { return new Event(Event.GenericMessage, (short)code, deviceId, msgId, pos, dt, rfid, data, code.IsGeneric()); }

		public static Event FactoryEvent(this MessageIdentifier code, MessageIdentifier generic, int deviceId, ulong msgId, GPSPoint pos, DateTime dt, String rfid, IEnumerable<Int64> data) { return new Event((short)generic, (short)code, deviceId, msgId, pos, dt, rfid, data, true); }

		private static bool IsGeneric(this MessageIdentifier code) { return !NonGenericMessages.Contains(code); }

		public static bool IsEstadoLogistico(Int64 data)
		{
			return data > 0 && data < 20;
		}

        public static bool IsConfirmacionUbox(Int64 data)
        {
            return data == 21 || data == 22;
        }

        public static bool IsEngineOnOffEvent(Mensaje msg)
        {
            return msg.Codigo == MessageCode.EngineOn.GetMessageCode()
                || msg.Codigo == MessageCode.EngineOff.GetMessageCode();
        }

        public static bool IsGarminOnOffEvent(Mensaje msg)
        {
            return msg.Codigo == MessageCode.GarminOn.GetMessageCode()
                || msg.Codigo == MessageCode.GarminOff.GetMessageCode();
        }

        public static bool IsPrivacyOnOffEvent(Mensaje msg)
        {
            return msg.Codigo == MessageCode.PrivacyOn.GetMessageCode()
                || msg.Codigo == MessageCode.PrivacyOff.GetMessageCode();
        }

        public static bool IsPanicEvent(string code)
        {
            return code == MessageCode.PanicButtonOn.GetMessageCode()
                || code == MessageCode.SirenOn.GetMessageCode();
        }

		#endregion
    }
}
