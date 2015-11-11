using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    public class GTEDeviceCommand : TrimbleDeviceCommand
    {
        public String MessageOrigin { get; set; }

        public class Attributes : TrimbleDeviceCommand.Attributes
        {
            public const string MessageOrigin_NN = "#";
            public const string MessageOrigin_LOG = "#LOG";
            public const string MessageOrigin_IP0 = "#IP0";
            public const string MessageOrigin_IP1 = "#IP1";
            public const string MessageOrigin_SM0 = "#SM0";
            public const string MessageOrigin_SM1 = "#SM1";
            public const string MessageOrigin_SDM = "#SDM";

            public static string[] MessageOrigins = new[] { MessageOrigin_NN, MessageOrigin_IP0, MessageOrigin_IP1, MessageOrigin_LOG, MessageOrigin_SDM, MessageOrigin_SM0, MessageOrigin_SM1 };
        }        

        #region GARMIN

        private class GarminCommand
        {            

            private static Dictionary<short, short> validA1Combinations = new Dictionary<short, short>()
                                                  {
                                                      {0x0101, 0x0211}, // Stop Status Protocol (esta combinacion es custom, no es de manual)
                                                      {0x002A, 0x002B}, // 5.1.5.1.1 A604 Server to Client Open Text Message Protocol
                                                                        // 5.1.5.1.3 Server to Client Canned Response Text Message Protocol
                                                      {0x0055, 0x0056}, // 5.1.5.1.2 A611 Server to Client Long Text Message Protocol 
                                                                        // 5.1.5.1.3 Server to Client Canned Response Text Message Protocol
                                                      {0x0028, 0x0029}, // 5.1.5.1.3 Server to Client Canned Response Text Message Protocol
                                                      {0x0040, 0x0041}, // 5.1.5.2 Message Status Protocol
                                                      {0x002D, 0x002E}, // 5.1.5.3 Message Delete Protocol
                                                      {0x0030, 0x0032}, // 5.1.5.4.1 Set Canned Response Protocol
                                                      {0x0031, 0x0033}, // 5.1.5.4.2 Delete Canned Response Protocol
                                                      {0x0050, 0x0051}, // 5.1.5.6.1 Set Canned Message Protocol
                                                      {0x0052, 0x0053}, // 5.1.5.6.2 Delete Canned Message Protocol
                                                      {0x0400, 0x0403}, // 5.1.6.2.2 Path Specific Stop (PSS) send to Client Protocol
                                                                        // 5.1.13.1 Server to Client - File Transfer Protocol
                                                                        // 5.1.13.2 Client to Server File Transfer Protocol
                                                      {0x0401, 0x0404}, // 5.1.6.2.2 Path Specific Stop (PSS) send to Client Protocol
                                                                        // 5.1.13.1 Server to Client - File Transfer Protocol
                                                                        // 5.1.13.2 Client to Server File Transfer Protocol
                                                      {0x0402, 0x0405}, // 5.1.6.2.2 Path Specific Stop (PSS) send to Client Protocol
                                                                        // 5.1.13.1 Server to Client - File Transfer Protocol
                                                                        // 5.1.13.2 Client to Server File Transfer Protocol
                                                      {0x0210, 0x0211}, // 5.1.7 Stop Status Protocol
                                                      {0x0200, 0x0201}, // 5.1.8 Estimated Time of Arrival (ETA) Protocol
                                                      {0x0110, 0x0111}, // 5.1.10 Sort Stop List Protocol
                                                      {0x0130, 0x0131}, // 5.1.11.1 Create Waypoint Protocol
                                                      {0x0132, 0x0133}, // 5.1.11.3 Delete Waypoint Protocol
                                                      {0x0135, 0x0136}, // 5.1.11.4 Delete Waypoint by Category Protocol
                                                      {0x0137, 0x0138}, // 5.1.11.5 Create Waypoint Category Protocol
                                                      {0x0813, 0x0812}, // 5.1.12.1.1 A607 Server to Client Driver ID Update Protocol
                                                      {0x0810, 0x0813}, // 5.1.12.1.3 A607 Server to Client Driver ID Request Protocol
                                                      {0x0800, 0x0802}, // 5.1.12.3.1 Set Driver Status List Item Protocol
                                                      {0x0801, 0x0803}, // 5.1.12.3.2 Delete Driver Status List Item Protocol
                                                      {0x0823, 0x0822}, // 5.1.12.4.1 A607 Server to Client Driver Status Update Protocol
                                                      {0x0820, 0x0823}, // 5.1.12.4.3 A607 Server to Client Driver Status Request Protocol
                                                      {0x0406, 0x0407}, // 5.1.13.3.1 GPI File Information
                                                      {0x0240, 0x0241}, // 5.1.15 User Interface Text Protocol
                                                      {0x0260, 0x0261}, // 5.1.16 Ping (Communication Link Status) Protocol
                                                      {0x0250, 0x0251}, // 5.1.17.1 Message Throttling Control Protocol
                                                      {0x0252, 0x0253}, // 5.1.17.2 Message Throttling Query Protocol
                                                      {0x0900, 0x0901}, // 5.1.18 FMI Safe Mode Protocol
                                                      {0x1000, 0x1001}, // 5.1.19.1 Speed Limit Alert Setup Protocol
                                                      {0x1200, 0x1201}, // 5.1.21.4 A612 Custom Form Template delete on Client Protocol
                                                      {0x1202, 0x1203}, // 5.1.21.5 A612 Custom Form Template move position on Client Protocol
                                                      {0x1204, 0x1205}, // 5.1.21.6 A612 Custom Form Template position request on Client Protocol
                                                      {0x1236, 0x1237}, // 5.1.22.1 A613 Custom Avoidance Area Feature Enable Protocol
                                                      {0x1230, 0x1231}, // 5.1.22.2 A613 Custom Avoidance New/Modify Protocol
                                                      {0x1232, 0x1233}, // 5.1.22.3 A613 Custom Avoidance Delete Protocol
                                                      {0x1234, 0x1235}, // 5.1.22.4 A613 Custom Avoidance Enable/Disable Protocol
                                                      {0x0011, 0x0012}  // 5.1.23 A616 Set Baud Rate Protocol                                                      
                                                  };

            private static Dictionary<short, short> validCombinations = new Dictionary<short, short>()
                                                                            {
                                                                                {14, 38}, // 5.2.2 Unit ID/ESN Protocol
                                                                                {5, 15},  // 5.2.3 Date and Time Protocol
                                                                                {49, 51}, // 5.2.4 Position, Velocity, and Time (PVT) Protocol
                                                                                {50, 51}, // 5.2.4 Position, Velocity, and Time (PVT) Protocol
                                                                            };

            private static Int16[] validPacketIds = new Int16[] {6, 10, 14, 21, 38, 51, 135, 136, 161};

            private static Int16[] validA1NonReturn = new Int16[]
                                                          {
                                                              0x0000, // 5.1.2 Enable Fleet Management Protocol
                                                              0x0005, // 5.1.4 Unicode Support Protocol
                                                              0x002c, // 5.1.5.1.3 Server to Client Canned Response Text Message Protocol
                                                              0x0025, // 5.1.5.5 A607 Client to Server Open Text Message Protocol
                                                              //0x0101, // 5.1.6.1 A603 Stop Protocol
                                                              0x0212, // 5.1.7 Stop Status Protocol
                                                              0x0202, // 5.1.8 Estimated Time of Arrival (ETA) Protocol
                                                              0x0220, // 5.1.9 Auto-Arrival at Stop Protocol
                                                              0x0134, // 5.1.11.2 Waypoint Deleted Protocol
                                                              0x0812, // 5.1.12.1.2 A607 Client to Server Driver ID Update Protocol
                                                              0x0822, // 5.1.12.4.2 A607 Client to Server Driver Status Update Protocol
                                                              0x0403, // 5.1.13.2 Client to Server File Transfer Protocol
                                                              0x0404, // 5.1.13.2 Client to Server File Transfer Protocol
                                                              0x0405, // 5.1.13.2 Client to Server File Transfer Protocol
                                                              0x0230, // 5.1.14 Data Deletion Protocol
                                                              0x0261, // 5.1.16 Ping (Communication Link Status) Protocol
                                                              0x1003, // 5.1.19.2 Speed Limit Alert Protocol
                                                              0x1010  // 5.1.20 A609 Remote Reboot
                                                         };

            private const String FMICommand =
                @"[\>](?<COMMANDTYPE>[S|R|E|Q])(?:(?<COMMAND>FM.,READY)|(?:(?<COMMAND>FM)(?:[.|P],)(?<PACKETID>[A-Z0-9]{2})(?<PAYLOADSIZE>[A-Z0-9]{2})(?<PAYLOAD>(?<COMMANDID>[A-Z0-9]{4})[A-Z0-9]*)))(?=(?:;ID=(?<DEVICEID>[0-9]{4})|;#(?<MESSAGEID>[A-Z0-9]{4}))*)?[^\<]*?[\<]";

            public string CommandType { get; set; }
            public string Command { get; set; }
            public UInt32 CommandId { get; set; }
            public UInt16 MessageId { get; set; }
            public Int16 IdNum { get; set; }
            public byte PacketId { get; set; }
            public byte PayloadSize { get; set; }
            public string Payload { get; set; }
            public bool Success { get; set; }
            public bool isAcknowledge { get; set; }

            public static GarminCommand createFrom(String command)
            {
                return new GarminCommand(command);
            }

            public GarminCommand()
            {
                throw new NotSupportedException();
            }

            public GarminCommand(String command)
            {
                try
                {
                    var match = Regex.Match(command, FMICommand);

                    Success = (match.Success && match.Groups.Count > 0);

                    if (Success)
                    {
                        var commandType = match.Groups["COMMANDTYPE"].Value;
                        var cmd = match.Groups["COMMAND"].Value;
                        var deviceId = match.Groups["DEVICEID"].Value;
                        
                        IdNum = Convert.ToInt16(deviceId);
                        CommandType = commandType;
                        Command = command;

                        if (match.Groups["MESSAGEID"].Success)
                        {
                            var messageId = match.Groups["MESSAGEID"].Value;
                            MessageId = Convert.ToUInt16(messageId, 16);
                        }
                        else
                        {
                            //STrace.Debug(typeof(GTEDeviceCommand).FullName, DeviceId, "MESSAGEID not found on: " + command);
                        }
                        
                        switch (cmd)
                        {
                            case "FM":
                                if (match.Groups["COMMANDID"].Success)
                                {
                                    var cmdId = match.Groups["COMMANDID"].Value;
                                    var CommandIdByteArr = StringUtils.HexStringToByteArray(cmdId);
                                    CommandId = BitConverter.ToUInt16(CommandIdByteArr, 0);

                                }
                                else
                                {
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, IdNum, "COMMANDID not found on: " + command);
                                }
                                if (match.Groups["PACKETID"].Success)
                                {
                                    var packetId = match.Groups["PACKETID"].Value;
                                    PacketId = Convert.ToByte(packetId, 16);
                                }
                                else
                                {
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, IdNum, "PACKETID not found on: " + command);
                                }
                                if (match.Groups["PAYLOADSIZE"].Success)
                                {
                                    var payloadSize = match.Groups["PAYLOADSIZE"].Value;
                                    PayloadSize = Convert.ToByte(payloadSize, 16);
                                }
                                else
                                {
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, IdNum, "PAYLOADSIZE not found on: " + command);
                                }
                                if (match.Groups["PAYLOAD"].Success)
                                {
                                    var payload = match.Groups["PAYLOAD"].Value;
                                    Payload = payload;
                                }
                                else
                                {
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, IdNum, "PAYLOAD not found on: " + command);
                                }

                                
                                isAcknowledge = false;
                                break;
                            case "FM.,READY":
                                isAcknowledge = true;
                                break;
                            default:
                                Success = false;
                                break;
                        }
                    }
                } catch (Exception e)
                {
                    STrace.Exception(typeof(GTEDeviceCommand).FullName, e, "Parsing: " + command);
                    throw e;
                }
            }

            public DeviceCommandResponseStatus IsValidResponse(GarminCommand garminCommandResponse)
            {
                // Valid Packet Ids in both commands
                var cond1 = validPacketIds.Any(l => l == PacketId);
                var cond2 = validPacketIds.Any(l => l == garminCommandResponse.PacketId);
                var result = cond1 && (cond2 || garminCommandResponse.isAcknowledge);

                if (result == false) STrace.Error("GARMIN RESPONSE", "Invalid Packet Ids");

                if (result)
                {
                    if (!garminCommandResponse.isAcknowledge)
                    {
                        if (PacketId == 0xA1 && garminCommandResponse.PacketId == 0xA1)
                        {
                            result = validA1Combinations.Any(kv => CommandId == kv.Key && garminCommandResponse.CommandId == kv.Value);
                            if (result == false) STrace.Error("GARMIN RESPONSE", "Invalid A1 Combinations");
                        }
                    } 
                    else
                    {
                        result = (MessageId == garminCommandResponse.MessageId && ((PacketId == 0xA1 && validA1NonReturn.Any(v => v == CommandId)) || PacketId == 0x06));
                        if (result == false) STrace.Error("GARMIN RESPONSE", "Invalid MessageId");
                    }
                }

                if (result && !garminCommandResponse.isAcknowledge) // check for exceptions
                {
                    var pArr = StringUtils.HexStringToByteArray(Payload);
                    var rPArr = StringUtils.HexStringToByteArray(garminCommandResponse.Payload);
                    switch (garminCommandResponse.CommandId)
                    {
                        case FmiPacketId.CsStopStatus:

                            //response && lastsent
                            /*
                            typedef struct / D603 /
                            {
                            uint32 unique_id;
                            uint16 stop_status;
                            uint16 stop_index_in_list;
                            } stop_status_data_type; 
                            */                            
                            var rUId = BitConverter.ToUInt32(rPArr, 2);
                            var rStopStatus = BitConverter.ToUInt16(rPArr, 4);

                            if (((StopStatusValue)rStopStatus) != StopStatusValue.StopStatusUnreadInactive)
                                break;

                            var UId = BitConverter.ToUInt32(pArr, 2);

                            if (UId != rUId) STrace.Error("GARMIN RESPONSE", "UId != rUId");

                            return (UId == rUId ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid);
                        case FmiPacketId.CsTextMessageReceiptA604OpenServer2Client:
                            /*
                             typedef struct / D604 /
                                {
                                    time_type origination_time;
                                    uint8 id_size;
                                    boolean result_code;
                                    uint16 reserved; / set to 0 /
                                    uint8 id[/ 16 bytes /];
                                } server_to_client_text_msg_receipt_data_type;
                             * 
                             * status_code:
                             *      0 == false
                             *      1 == true
                            */

                            if (rPArr.Length > 5)
                            {
                                return (rPArr[6] == 0 ? DeviceCommandResponseStatus.Exception : DeviceCommandResponseStatus.Valid);
                            }
                            break;
                        case FmiPacketId.CsTextMessageStatus:
                            /*
                            typedef struct / D604 /
                            {
                                uint8 id_size;
                                uint8 status_code;
                                uint16 reserved; / set to 0 /
                                uint8 id[/ 16 bytes /];
                            } message_status_data_type;
                             * 
                             * status_code:
                             *      0 Message is unread
                             *      1 Message is read
                             *      2 Message is not found (e.g. deleted)
                             * 
                            */
                            if (rPArr.Length > 1)
                            {
                                return (rPArr[2] == 2 ? DeviceCommandResponseStatus.Exception : DeviceCommandResponseStatus.Valid);
                            }
                            break;
                        case FmiPacketId.CsDeleteTextMessageResponse:
                            /*
                            typedef struct / D607 /
                            {
                                uint8 id_size;
                                boolean status_code;
                                uint16 reserved; / set to 0 /
                                uint8 id[/ 16 bytes /];
                            } message_delete_response_data_type;
                             * 
                             * status_code:
                             *      0 == message was found but could not be deleted
                             *      1 == otherwise
                            */
                            if (rPArr.Length > 1)
                            {
                                return (rPArr[2] == 0 ? DeviceCommandResponseStatus.Exception : DeviceCommandResponseStatus.Valid);
                            }
                            break;
                        case FmiPacketId.CsCreateWaypointReceipt:
                            /*
                            typedef struct / D607 /
                            {
                                uint16 unique_id;
                                boolean status_code;
                                uint8 reserved; / set to 0 /
                            } waypoint_receipt_data_type;
                             * 
                             * status_code:
                             *      1 == true if the operation was succesful
                             *      0 == otherwise
                             */
                            if (rPArr.Length > 2)
                            {
                                return (rPArr[3] == 0 ? DeviceCommandResponseStatus.Exception : DeviceCommandResponseStatus.Valid);
                            }
                            break;
                        case FmiPacketId.WaypointDeleted:
                            /*
                            typedef struct / D607 /
                            {
                                uint16 unique_id;
                                boolean status_code;
                                uint8 reserved; / set to 0 /
                            } waypoint_deleted_data_type;
                             * 
                             * status_code:
                             *      1 == true if the waypoint with the specified unique_id no longer exists
                             *      0 == otherwise
                             */
                            if (rPArr.Length > 2)
                            {
                                return (rPArr[3] == 0 ? DeviceCommandResponseStatus.Exception : DeviceCommandResponseStatus.Valid);
                            }
                            break;
                        case FmiPacketId.CsCreateWaypointByCategoryReceipt:
                            /*
                            typedef struct / D607 /
                            {
                                uint8 cat_id;
                                boolean status_code;
                            } waypoint_cat_receipt_data_type;
                             * 
                             * status_code:
                             *      0 == otherwise
                             *      1 == true if the operation was succesful
                             */
                            if (rPArr.Length > 1)
                            {
                                return (rPArr[2] == 0 ? DeviceCommandResponseStatus.Exception : DeviceCommandResponseStatus.Valid);
                            }
                            break;
                    }
                }

                return result ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid;
            }
        }

#endregion GARMIN

        private static Dictionary<String, String> validGAPCombinations = new Dictionary<string, string>()
                                                                             {
                                                                                 {"FM.,", "FMP,"},
                                                                                 {"SR55", "SR"}
                                                                             };


        public GTEDeviceCommand(byte[] command) : this(command, null) { }

        public GTEDeviceCommand(string command) : this(command, null, null) { }        

        public GTEDeviceCommand(byte[] command, INode node) : this(byte2string(command), node, null) { }    

        public GTEDeviceCommand(string command, INode node, DateTime? ExpiresOn) : base(command, node, ExpiresOn)
        {

            #region MessageId

            string msgkey = Attributes.MessageOrigins.FirstOrDefault(_attributes.ContainsKey);
            if (msgkey != null)
            {
                if (_attributes.ContainsKey(msgkey))
                {
                    try
                    {
                        MessageId = (UInt32?) Convert.ToUInt16(_attributes[msgkey], 16);
                        MessageOrigin = msgkey;
                        _attributes.Remove(msgkey);
                    }
                    catch
                    {
                    }
                }
            }

            #endregion MessageId
        }

/*        public static int getDeviceIDFrom(String command)
        {
            var result = -1;
            try
            {
                var mCommand = Regex.Match(command, deviceIDPattern);
                if (mCommand.Success &&
                    mCommand.Groups["value"].Success)
                {
                    result = Int16.Parse(mCommand.Groups["value"].Value);
                }
                else
                {
                    result = 0;
                }
            } catch ()
            {
                
            }
            return result;
        }
*/
        public override bool isGarminMessage()
        {
            return isGarminMessage(_command);
        }

        protected override bool isSettingIDMessage()
        {
            var cmd = getCommand();
            return (cmd.StartsWith("SID"));
        }

        public static bool isGarminMessage(string message)
        {
            if (message.StartsWith(">"))
                message = message.Substring(1);
            return (!String.IsNullOrEmpty(message) &&
                    (new String[] {"SFM.", "SFMP", "RFM.", "RFMP"}.Any(message.StartsWith)));

        }

        public override DeviceCommandResponseStatus isExpectedResponse(BaseDeviceCommand response)
        {
            var error = "";                        
            var result = base.isExpectedResponse(response);

            if (DeviceCommandResponseStatus.Valid == result)
            {
                var rcmd = response.getCommand();

                const string zz =
                    @"^[E|Q|R|S]{1}(?<COMMANDPREFIX>[A-Z]{2}(?:[*|U]|\s|[0-9]{1}(?:$|\s)|[A-Z0-9]{2}|[SP.],))";
                var matchCmd = Regex.Match(_command, zz);
                var matchRCmd = Regex.Match(rcmd, zz);

                if (matchCmd.Success && matchRCmd.Success && matchCmd.Groups["COMMANDPREFIX"].Success &&
                    matchRCmd.Groups["COMMANDPREFIX"].Success)
                {
                    var cmdPrefix = matchCmd.Groups["COMMANDPREFIX"].Value;
                    var rCmdPrefix = matchRCmd.Groups["COMMANDPREFIX"].Value;
                    result = (cmdPrefix == rCmdPrefix ||
                        (cmdPrefix.Contains("*") && cmdPrefix == rCmdPrefix.Replace("*", " ")) ? DeviceCommandResponseStatus.Valid: DeviceCommandResponseStatus.Invalid) ;

                    if (DeviceCommandResponseStatus.Valid != result) 
                    {
                        if (validGAPCombinations.ContainsKey(cmdPrefix))
                        {
                            result = validGAPCombinations[cmdPrefix] == rCmdPrefix
                                         ? DeviceCommandResponseStatus.Valid
                                         : DeviceCommandResponseStatus.Invalid;
                        }
                    }
                }
                else
                {
                    var cmdPrefix = _command.Substring(1, 2);
                    var rCmdPrefix = rcmd.Substring(1, 2);
                    result = (result == DeviceCommandResponseStatus.Valid && (cmdPrefix == rCmdPrefix)) ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid;

                    if (result != DeviceCommandResponseStatus.Valid)
                        error += " + '" + rCmdPrefix + "' IS NOT AN EXPECTED RESPONSE FOR '" + cmdPrefix + "'";

                    if (result == DeviceCommandResponseStatus.Valid)
                    {
                        result = ((response.MessageId == null || response.MessageId == MessageId) && response.IdNum == IdNum) ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid;
                        if (result != DeviceCommandResponseStatus.Valid)
                        {
                            error += " + INVALID ID/MESSAGE  ID=" + MessageId + "/" + response.MessageId + " - " + IdNum +
                                     "/" + response.IdNum;
                            error += Environment.NewLine +
                                     String.Format("=>{0}{1}=>{2}", getCommand(), "/", response.getCommand());
                        }
                    }
                }

                if (!String.IsNullOrEmpty(error))
                    STrace.Debug(typeof (GTEDeviceCommand).FullName, IdNum ?? response.IdNum ?? 0, "GTE LEVEL   :" + error);
                error = "";

                if (DeviceCommandResponseStatus.Valid == result)
                {
                    var gCommand = ">" + _command + ";ID=" + String.Format("{0:0000}", IdNum) +
                                   (MessageId != null ? ";#" + String.Format("{0:X4}", MessageId.Value) : "") + "<";
                    var gDC =
                        GarminCommand.createFrom(gCommand);
                    if (gDC.Success) // Success Parsing Garmin Command // its a Garmin Command
                    {
                        var grDC = GarminCommand.createFrom(response.ToString(true));
                        if (grDC.Success)
                        {
                            result = gDC.IsValidResponse(grDC);
                            /*
                            if (!result)
                                error += " + '" + response + "' WAS NOT AN EXPECTING RESPONSE";
                             */
                        }
                        if (!String.IsNullOrEmpty(error))
                            STrace.Debug(typeof(GTEDeviceCommand).FullName, IdNum ?? response.IdNum ?? 0, "GARMIN LEVEL:" + error);
                        else
                        {
                            STrace.Debug(typeof(GTEDeviceCommand).FullName, IdNum ?? response.IdNum ?? 0, "GARMIN LEVEL: + EXPECTED RESPONSE");
                            result = DeviceCommandResponseStatus.Valid;
                        }
                    }
                    else
                    {
                        STrace.Debug(typeof(GTEDeviceCommand).FullName, IdNum ?? response.IdNum ?? 0, "GTE LEVEL   : + EXPECTED RESPONSE");
                    }
                }
            }
            STrace.Debug(typeof (GTEDeviceCommand).FullName, IdNum??response.IdNum??0, String.Format("- GET GARMIN RESPONSE {0} -", result));
            return result;
        }

        protected override void addCustomToString(bool clean, ref StringBuilder result)
        {
            if (MessageId != null)
            {
                UInt16 mid = Convert.ToUInt16(MessageId);
                if (MessageOrigin == null ||
                    MessageOrigin == Attributes.MessageOrigin_NN)
                    result.AppendFormat(";{0}{1:X4}", Attributes.MessageId, mid);
                else
                    result.AppendFormat(";{0}:{1:X4}", MessageOrigin, mid);
            }
        }

        protected override void addCustomCheckSum(bool clean, ref StringBuilder result)
        {
            result.Append(";*");
            result.AppendFormat("{0:X2}", CalculateCheckSum(result.ToString()));
        }

        #region instance tools
        public override BaseDeviceCommand BuildAck()
        {
            var r = new GTEDeviceCommand(">SAK<")
            {
                IdNum = IdNum,
                MessageOrigin = MessageOrigin,
                MessageId = MessageId
            };
            return r;
        }
        #endregion instance tools

        #region Commands Parsers
        public DeviceStatus ParsePosition()
        {
            if (!(new[] { "RPH", "RPI" }.Any(d => _command.StartsWith(d)))) return null;
            // RPI 190814161155 -3468748 -05847549 125 000 1 12 0000 1 008 1 1 16 0008042322 EF 27;ID=4629;#LOG:9C78;*72<
            // RPH 141014140025 -3460429 -05837906 145 000 1 03 0000 0 256 1 1 16 0000114489 0F 67;ID=5182;#LOG:506F;*0A<

            #region parse line

            var datetimeStr = _command.Substring(3, 12);
            var latStr = _command.Substring(15, 8);
            var lonStr = _command.Substring(23, 9);
            var curseStr = _command.Substring(32, 3); // Norte = 0, Este = 90, Sur = 180, Oeste = 270
            var speedStr = _command.Substring(35, 3);
            var gpsOnOffStr = _command.Substring(38, 1);
            var qSatStr = _command.Substring(39, 2);
            var posAgeStr = _command.Substring(41, 4);
            var oneStr = _command.Substring(45, 1);
            var hdopStr = _command.Substring(46, 3);
            var gprsWithValidIPOnOffStr = _command.Substring(49, 1);
            var gsmStatusStr = _command.Substring(50, 1);
            /*
                Estado de registro GSM
                    0 not registered, ME is not currently searching a new operator to register to
                    1 registered, home network
                    2 not registered, but ME is currently searching a new operator to register to
                    3 registration denied
                    4 unknown
                    5 registered, roaming
            */
            var gsmSignalStr = _command.Substring(51, 2);
            /*
                Nivel de señal GSM
                    0 -113 dBm o menor
                    1 -111 dBm
                    2...30 -109... -53 dBm
                    31 -51 dBm o mayor
                    99 no detectado
             */
            var odometerStr = _command.Substring(53, 10);
            var inputsStr = _command.Substring(63, 2);
            /*
                Estado de las entradas digitales en hexadecimal
                    0x80 Estado de Contacto o Ignición
                    0x40 Estado de Alimentación Principal
                    0x20 Entrada digital 5
                    0x10 Entrada digital 4
                    0x08 Entrada digital 3
                    0x04 Entrada digital 2
                    0x02 Entrada digital 1
                    0x01 Entrada digital 0
             */
             var eventStr = _command.Substring(65, 2);

            #endregion parse line
            // -----------------------
            
            var time = DateTimeUtils.SafeParseFormat(datetimeStr, "ddMMyyHHmmss");
            var lat = Convert.ToSingle(latStr)*(float) 0.00001;
            var lon = Convert.ToSingle(lonStr)*(float) 0.00001;
            var vel = Convert.ToSingle(speedStr);
            var dir = Convert.ToSingle(curseStr); // (0 ..359), Norte = 0, Este = 90, Sur = 180, Oeste = 270
            var hdop = Convert.ToSingle(hdopStr) / 10;
            var entradas = Convert.ToByte(inputsStr, 16);
            var evento = Convert.ToByte(eventStr);
            var posEdad = Convert.ToInt32(posAgeStr);

            var devId = (Int32?) null;

            if (_node != null)
                devId = _node.Id;

            GPSPoint gpoint = null;
            try
            {
                gpoint = new GPSPoint(time, lat, lon, vel, GPSPoint.SourceProviders.Unespecified, 0)
                             {   
                                 Age = posEdad,
                                 Course = new Course(dir),
                                 HDOP = hdop,
                                 IgnitionStatus = BitHelper.AreBitsSet(entradas, 0x80) ? IgnitionStatus.On : IgnitionStatus.Off
                             };
                if (devId != null)
                    gpoint.DeviceId = devId.Value;
            }
            catch (ArgumentOutOfRangeException e)
            {
                STrace.Exception(typeof(VirlocDeviceCommand).FullName, e, IdNum ?? 0, String.Format("Posición inválida {0}", getCommand()));
                gpoint = null;
            }

            var result = new DeviceStatus(devId, gpoint, evento, entradas);            
            return result;
        }
        #endregion Commands Parsers

        protected override string getCommandPattern()
        {
            return commandPattern;
        }
    }
}
