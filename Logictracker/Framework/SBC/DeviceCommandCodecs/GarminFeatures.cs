using System;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    #region Enums

    /// <summary>
    /// Packet Parts
    /// </summary>
    public abstract class PacketParts
    {
        public const byte Dle = 0x10;
        public const byte Etx = 0x03;

        public static readonly byte[] FmiHeader = new[] {Dle, PacketIdCode.FleetManagementPacket};
        public static readonly byte[] Trail = new[] {Dle, Etx};
    }

    /// <summary>
    /// Packet ID type
    /// </summary>
    public abstract class PacketIdCode
    {
        public const byte Ack = 0x06;
        public const byte Command = 0xA;
        public const byte DateTimeData = 0xE;
        public const byte NAck = 0x15;
        public const byte ScRequestUnitIdEsn = 0x0E;
        public const byte CsRequestUnitIdEsnResponse = 0x26;
        public const byte UnitIdEsn = 0x26;
        public const byte PvtData = 0x33;
        public const byte StreetPilotStopMessage = 0x87;
        public const byte StreetPilotTextMessage = 0x88;
        public const byte FleetManagementPacket = 0xA1;
    }

    /// <summary>
    /// Fleet Management Packet IDs
    /// </summary>
    public abstract class FmiPacketId
    {
        public const UInt16 ScEnableFleetManagementProtocolRequest = 0x0000;
        public const UInt16 ScProductIdAndSupportRequest = 0x0001;
        public const UInt16 CsProductIdData = 0x0002;
        public const UInt16 CsProductSupportData = 0x0003;
        public const UInt16 CsUnicodeSupportRequest = 0x0004;
        public const UInt16 ScUnicodeSupportResponse = 0x0005;
        public const UInt16 CsTextMessageAcknowledgement = 0x0020;
        public const UInt16 ScTextMessageA602OpenServer2Client = 0x0021;
        public const UInt16 ScTextMessageSimpleAcknowledgement = 0x0022;
        public const UInt16 ScTextMessageYesNoConfirmation = 0x0023;
        public const UInt16 CsTextMessageOpenClient2Server = 0x0024;
        public const UInt16 ScTextMessageReceiptOpenClient2Server = 0x0025;
        public const UInt16 CsA607Client2ServerTextMessage = 0x0026;
        public const UInt16 ScSetCannedResponseList = 0x0028;
        public const UInt16 CsCannedResponseListReceipt = 0x0029;
        public const UInt16 ScTextMessageA604OpenServer2Client = 0x002A;
        public const UInt16 CsTextMessageReceiptA604OpenServer2Client = 0x002B;
        public const UInt16 ScTextMessageAckReceipt = 0x002C;
        public const UInt16 ScDeleteTextMessage = 0x002D;
        public const UInt16 CsDeleteTextMessageResponse = 0x002E;
        public const UInt16 ScSetCannedResponse = 0x0030;
        public const UInt16 ScDeleteCannedResponse = 0x0031;
        public const UInt16 CsSetCannedResponseReceipt = 0x0032;
        public const UInt16 CsDeleteCannedResponseReceipt = 0x0033;
        public const UInt16 CsRequestCannedResponseListRefresh = 0x0034;
        public const UInt16 ScTextMessageStatusRequest = 0x0040;
        public const UInt16 CsTextMessageStatus = 0x0041;
        public const UInt16 ScSetCannedMessage = 0x0050;
        public const UInt16 CsSetCannedMessageReceipt = 0x0051;
        public const UInt16 ScDeleteCannedMessage = 0x0052;
        public const UInt16 CsDeleteCannedMessageReceipt = 0x0053;
        public const UInt16 CsRefreshCannedMessageList = 0x0054;
        public const UInt16 ScLongTextMessageA611LongServer2Client = 0x0055;
        public const UInt16 CsLongTextMessageReceiptA611LongServer2Client = 0x0056;
        public const UInt16 ScA602Stop = 0x0100;
        public const UInt16 ScA603Stop = 0x0101;
        public const UInt16 ScSortStopList = 0x0110;
        public const UInt16 CsSortStopListAcknowledgement = 0x0111;
        public const UInt16 ScCreateWaypoint = 0x0130;
        public const UInt16 CsCreateWaypointReceipt = 0x0131;
        public const UInt16 ScDeleteWaypoint = 0x0132;
        public const UInt16 WaypointDeleted = 0x0133;
        public const UInt16 ScWaypointDeletedReceipt = 0x0134;
        public const UInt16 ScDeleteWaypointByCategory = 0x0135;
        public const UInt16 CsDeleteWaypointByCategoryReceipt = 0x0136;
        public const UInt16 ScCreateWaypointByCategory = 0x0137;
        public const UInt16 CsCreateWaypointByCategoryReceipt = 0x0138;
        public const UInt16 ScEtaDataRequest = 0x0200;
        public const UInt16 CsEtaData = 0x0201;
        public const UInt16 ScEtaDataReceipt = 0x0202;
        public const UInt16 ScStopStatusRequest = 0x0210;
        public const UInt16 CsStopStatus = 0x0211;
        public const UInt16 ScStopStatusReceipt = 0x0212;
        public const UInt16 ScAutoArrival = 0x0220;
        public const UInt16 ScDataDeletion = 0x0230;
        public const UInt16 ScUserInterfaceText = 0x0240;
        public const UInt16 CsUserInterfaceTextReceipt = 0x0241;
        public const UInt16 ScMessageThrottlingCommand = 0x0250;
        public const UInt16 CsMessageThrottlingResponse = 0x0251;
        public const UInt16 ScMessageThrottlingQuery = 0x0252;
        public const UInt16 CsMessageThrottlingQueryResponse = 0x0253;
        public const UInt16 PingCommunicationStatus = 0x0260;
        public const UInt16 PingCommunicationStatusResponse = 0x0261;
        public const UInt16 ScGpiFileTransferStart = 0x0400;
        public const UInt16 ScGpiFileDataPacket = 0x0401;
        public const UInt16 ScGpiFileTransferEnd = 0x0402;
        public const UInt16 CsGpiFileStartReceipt = 0x0403;
        public const UInt16 CsGpiPacketReceipt = 0x0404;
        public const UInt16 CsGpiFileEndReceipt = 0x0405;
        public const UInt16 ScGpiFileInformationRequest = 0x0406;
        public const UInt16 CsGpiFileInformation = 0x0407;
        public const UInt16 ScSetDriverStatusListItem = 0x0800;
        public const UInt16 ScDeleteDriverStatusListItem = 0x0801;
        public const UInt16 CsSetDriverStatusListItemReceipt = 0x0802;
        public const UInt16 CsDeleteDriverStatusListItemReceipt = 0x0803;
        public const UInt16 CsDriverStatusListRefresh = 0x0804;
        public const UInt16 ScRequestDriverId = 0x0810;
        public const UInt16 DriverIdUpdate = 0x0811;
        public const UInt16 DriverIdReceipt = 0x0812;
        public const UInt16 ScA607DriverIdUpdate = 0x0813;
        public const UInt16 ScRequestDriverStatus = 0x0820;
        public const UInt16 DriverStatusUpdate = 0x0821;
        public const UInt16 DriverStatusReceipt = 0x0822;
        public const UInt16 A607DriverStatusUpdate = 0x0823;
        public const UInt16 ScFmiSafeMode = 0x0900;
        public const UInt16 CsFmiSafeModeReceipt = 0x0901;
        public const UInt16 ScSpeedLimitAlertSetup = 0x1000;
        public const UInt16 CsSpeedLimitAlertSetupReceipt = 0x1001;
        public const UInt16 CsSpeedLimitAlert = 0x1002;
        public const UInt16 ScSpeedLimitAlertReceipt = 0x1003;
    }


    public enum DataDeletionProtocolId :uint
    {
        DeleteAllStopsOnTheClient = 0,
        DeleteAllMessagesOnTheClient = 1,
        DeleteActiveRouteOnTheClient = 2,
        DeleteAllCannedMessagesOnTheClient = 3,
        DeleteAllCannedRepliesOnTheClient = 4,
        DeleteFleetManagementGPIFileOnTheClient = 5,
        DeleteAllDriverIdAndStatusOnTheClient = 6,
        DeleteFleetManagementInterfaceOnTheClient = 7,
        DeleteWaypointsOnTheClient = 8,
        Reserved1 = 9,
        DeleteAllCustomFormsOnTheClient = 10,
        DeleteAllCustomAvoidancesOnTheClient = 11
    }

    public abstract class CommandsIds
    {
        public const byte RequestDateTimeData = 0x05;
        public const byte RequestUnitIdEsn = 0x0E;
        public const byte TurnOnPvtData = 0x31;
        public const byte TurnOffPvtData = 0x32;
    }


    /// <summary>
    /// Stop Status Posible Values
    /// </summary>
    public enum StopStatusValue :ushort
    {
        RequestingStopStatus = 0,
        MarkStopAsDone = 1,
        ActivateStop = 2,
        DeleteStop = 3,
        MoveStop = 4,
        StopStatusActive = 100,
        StopStatusDone = 101,
        StopStatusUnreadInactive = 102,
        StopStatusReadInactive = 103,
        StopStatusDeleted = 104
    }

    #endregion
}