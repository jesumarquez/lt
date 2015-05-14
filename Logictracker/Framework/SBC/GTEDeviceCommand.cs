using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Utils;

namespace Urbetrack.Layers
{
    public class GTEDeviceCommand
    {

        private class GarminCommand
        {            

            private static Dictionary<short, short> validA1Combinations = new Dictionary<short, short>()
                                                  {
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
                                                              0x0101, // 5.1.6.1 A603 Stop Protocol
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
            public Int16 MessageId { get; set; }
            public Int16 DeviceId { get; set; }
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
                        
                        DeviceId = Convert.ToInt16(deviceId);
                        CommandType = commandType;
                        Command = command;

                        if (match.Groups["MESSAGEID"].Success)
                        {
                            var messageId = match.Groups["MESSAGEID"].Value;
                            MessageId = Convert.ToInt16(messageId, 16);
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
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, DeviceId, "COMMANDID not found on: " + command);
                                }
                                if (match.Groups["PACKETID"].Success)
                                {
                                    var packetId = match.Groups["PACKETID"].Value;
                                    PacketId = Convert.ToByte(packetId, 16);
                                }
                                else
                                {
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, DeviceId, "PACKETID not found on: " + command);
                                }
                                if (match.Groups["PAYLOADSIZE"].Success)
                                {
                                    var payloadSize = match.Groups["PAYLOADSIZE"].Value;
                                    PayloadSize = Convert.ToByte(payloadSize, 16);
                                }
                                else
                                {
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, DeviceId, "PAYLOADSIZE not found on: " + command);
                                }
                                if (match.Groups["PAYLOAD"].Success)
                                {
                                    var payload = match.Groups["PAYLOAD"].Value;
                                    Payload = payload;
                                }
                                else
                                {
                                    STrace.Error(typeof(GTEDeviceCommand).FullName, DeviceId, "PAYLOAD not found on: " + command);
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

            public bool isValidResponse(GarminCommand garminCommandResponse)
            {
                // Valid Packet Ids in both commands
                var result = validPacketIds.Any(l => l == PacketId) && (garminCommandResponse.isAcknowledge || validPacketIds.Any(l => l == garminCommandResponse.PacketId));

                if (result)
                {
                    if (!garminCommandResponse.isAcknowledge)
                    {
                        if (PacketId == 0xA1 && garminCommandResponse.PacketId == 0xA1)
                        {
                            result =
                                validA1Combinations.Any(
                                    kv => CommandId == kv.Key && garminCommandResponse.CommandId == kv.Value);
                        }
                    } else
                    {
                        result = (MessageId == garminCommandResponse.MessageId && ((PacketId == 0xA1 && validA1NonReturn.Any(v => v == CommandId)) || PacketId == 0x06));
                    }
                }            

                return result;
            }
        }

        public class Attributes
        {
            public const string DeviceId = "ID";
            public const string MessageId = "#";
            public const string Tries = "TRIES";
            public const string Status = "STATUS";
            public const string Status_Sent = "SENT";
            public const string Status_WaitingAck = "WACK";
            public const string Status_Expired = "EXPIRED";
            public const string Status_GarminNotConnected = "NOGARMIN";
            public const string LastTry = "LASTTRY";
            public const string ExpiresOn = "EXPIRESON";
            
        }

        private static Dictionary<String, String> validGAPCombinations = new Dictionary<string, string>()
                                                                             {
                                                                                 {"FM.,", "FMP,"},
                                                                                 {"SR55", "SR"}
                                                                             };

        private const string commandPattern = @"\>(?<command>[^;]*)[^\<]*\<";
        private const string commandAttributesPattern = @"(?:(?<=[;])(?:(?<param>[^;\<#]*?)[:|=](?<value>[^;\<]*?)|(?<param>#[^;\<#]*?)(?<value>[0-9A-F]{4}))(?=;|\<|$))";
        private const string deviceIDPattern = @"(?:(?<=[;])(?:(?:ID)[=](?<value>[^;\<]*?))(?=;|\<|$))";

        public int DeviceId { get; set; }
        public ulong? MessageId { get; set; }
        public uint? Tries { get; set; }
        public DateTime? LastTry { get; set; }
        public DateTime? ExpiresOn { get; set; }

        private string _command;
        private Dictionary<string, string> _attributes;

        private string getCommandFrom(String command)
        {
            var mCommand = Regex.Match(command, commandPattern);
            if (mCommand.Success &&
                mCommand.Groups["command"].Success)
            {
                return mCommand.Groups["command"].Value;
            }
            else
            {
                STrace.Error(typeof(GTEDeviceCommand).FullName, DeviceId, "RECEIVED COMMAND: " + command);
                return null;
            }
        }

        private Dictionary<string, string> getCommandAttributesFrom(String command)
        {
            var mAttributes = Regex.Matches(command, commandAttributesPattern);

            return mAttributes.Cast<Match>().ToDictionary(ma => ma.Groups["param"].Value, ma => ma.Groups["value"].Value);
        }

        GTEDeviceCommand()
        {
            throw new NotSupportedException();
        }

        GTEDeviceCommand(string command) : this(command, null) { }

        GTEDeviceCommand(string command, int? deviceId)
        {
            _command = getCommandFrom(command);
            _attributes = getCommandAttributesFrom(command);

            #region DeviceId
            if (deviceId == null)
            {
                if (_attributes.ContainsKey(Attributes.DeviceId))
                {
                    try
                    {
                        DeviceId = Convert.ToInt32(_attributes[Attributes.DeviceId]);
                        _attributes.Remove(Attributes.DeviceId);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            else
            {
                DeviceId = deviceId.Value;
                if (_attributes.ContainsKey(Attributes.DeviceId))
                    _attributes.Remove(Attributes.DeviceId);
            }
            #endregion DeviceId

            #region MessageId
            if (MessageId == null)
            {
                if (_attributes.ContainsKey(Attributes.MessageId))
                {
                    try
                    {
                        MessageId = (ulong?)Convert.ToInt16(_attributes[Attributes.MessageId], 16);
                        _attributes.Remove(Attributes.MessageId);
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                if (_attributes.ContainsKey(Attributes.MessageId))
                    _attributes.Remove(Attributes.MessageId);
            }
            #endregion MessageId

            #region Tries
            if (Tries == null)
            {
                if (_attributes.ContainsKey(Attributes.Tries))
                {
                    try
                    {
                        Tries = (uint?)Convert.ToInt32(_attributes[Attributes.Tries]);
                        _attributes.Remove(Attributes.Tries);
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                if (_attributes.ContainsKey(Attributes.Tries))
                    _attributes.Remove(Attributes.Tries);
            }
            #endregion Tries


            #region LastTry
            if (_attributes.ContainsKey(Attributes.LastTry))
            {
                try
                {
                    var dt = _attributes[Attributes.LastTry];
                    if (!String.IsNullOrEmpty(dt))
                    {
                        LastTry = DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None);
                        _attributes.Remove(Attributes.LastTry);
                    }
                }
                catch (Exception e)
                {
                }
            }
            else
                LastTry = null;
            #endregion LastTry

            #region Expiration
            if (_attributes.ContainsKey(Attributes.ExpiresOn))
            {
                try
                {
                    var dt = _attributes[Attributes.ExpiresOn];
                    if (!String.IsNullOrEmpty(dt))
                    {
                        ExpiresOn = DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None);
                        _attributes.Remove(Attributes.ExpiresOn);
                    }
                }
                catch (Exception e)
                {
                }
            }
            else
                ExpiresOn = null;
            #endregion Expiration
        }

        public string getCommand() { return _command; }

        public Dictionary<string, string> getAttributes() { return _attributes; }

        public bool isAlreadySent()
        {
            return _attributes.ContainsKey(Attributes.Status) && _attributes[Attributes.Status] == Attributes.Status_Sent;
        }

        public static GTEDeviceCommand createFrom(string command) { return new GTEDeviceCommand(command); }
        public static GTEDeviceCommand createFrom(string command, int deviceId) { return new GTEDeviceCommand(command, deviceId); }
        public static GTEDeviceCommand createFrom(string command, int deviceId, ulong messageId)
        {
            var cmd = new GTEDeviceCommand(command, deviceId);
            cmd.MessageId = messageId;
            return cmd;
        }

        public string ToString()
        {
            return ToString(false);
        }
        public string ToString(bool clean)
        {
            var result = new StringBuilder();
            result.AppendFormat(">{0}", _command);
            foreach (var attribute in _attributes)
            {
                if (!(attribute.Key == Attributes.Status && clean == true))
                    result.AppendFormat(";{0}={1}", attribute.Key, attribute.Value);
            }

            if (DeviceId != null)
                result.AppendFormat(";{0}={1:D4}", Attributes.DeviceId, DeviceId);
            if (Tries != null && clean == false)
                result.AppendFormat(";{0}={1}", Attributes.Tries, Tries.Value);
            if (LastTry != null && clean == false)
                result.AppendFormat(";{0}={1}", Attributes.LastTry, LastTry.Value.ToString("O"));            

            if (MessageId != null)
                result.AppendFormat(";{0}{1:X4}", Attributes.MessageId, MessageId);

            result.Append("<");
            return result.ToString();
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
        public bool isGarminMessage()
        {
            return isGarminMessage(_command);
        }

        public static bool isGarminMessage(string message)
        {
            if (message.StartsWith(">"))
                message = message.Substring(1);
            return (!String.IsNullOrEmpty(message) &&
                    (new String[] {"SFM.", "SFMP", "RFM.", "RFMP"}.Any(message.StartsWith)));

        }

        public bool isExpectedResponse(string response)
        {
            var result = false;

            var r = createFrom(response);
            var rcmd = r.getCommand();

            #region Verify if we are receiving an expecting response for the last fota sent
            var rcmd_fistLetter = rcmd.Substring(0, 1);
            var cmd_firstLetter = _command.Substring(0, 1);

            var error = "";

            if (new String[] {"E", "R"}.Any(l=> l == rcmd_fistLetter))
            {
                result = new String[] {"Q", "S"}.Any(l => l == cmd_firstLetter);
                if (!result)
                    error += " + WE WERE EXPECTING RESPONSE (R) OR ERROR (E)";
/*
                if (!result)
                    STrace.Debug(typeof(GTEDeviceCommand).FullName, DeviceId, "GTE #0.1: NOT AN EXPECTED RESPONSE " + cmd_firstLetter + " => " + rcmd_fistLetter );
*/
                const string zz = @"^[E|Q|R|S]{1}(?<COMMANDPREFIX>[A-Z]{2}(?:[*|U]|\s|[0-9]{1}(?:$|\s)|[A-Z0-9]{2}|[SP.],))";
                var matchCmd = Regex.Match(_command, zz);
                var matchRCmd = Regex.Match(rcmd, zz);

                if (matchCmd.Success && matchRCmd.Success && matchCmd.Groups["COMMANDPREFIX"].Success && matchRCmd.Groups["COMMANDPREFIX"].Success)
                {
                    var cmdPrefix = matchCmd.Groups["COMMANDPREFIX"].Value;
                    var rCmdPrefix = matchRCmd.Groups["COMMANDPREFIX"].Value;
                    result = cmdPrefix == rCmdPrefix || (cmdPrefix.Contains("*") && cmdPrefix == rCmdPrefix.Replace("*", " "));

                    if (!result)
                    {
                        if (validGAPCombinations.ContainsKey(cmdPrefix))
                        {
                            result = validGAPCombinations[cmdPrefix] == rCmdPrefix;
                        }
                    }
/*
                    if (!result)
                        error += " + '" + rCmdPrefix + "' IS NOT AN EXPECTED RESPONSE FOR '" + cmdPrefix + "'";
 */
                }
                else
                {
                    var cmdPrefix = _command.Substring(1, 2);
                    var rCmdPrefix = rcmd.Substring(1, 2);
                    result = result && (cmdPrefix == rCmdPrefix);

                    if (!result)
                        error += " + '" + rCmdPrefix + "' IS NOT AN EXPECTED RESPONSE FOR '" + cmdPrefix + "'";

                    if (result)
                    {
                        result = (r.MessageId == null || r.MessageId == MessageId) && r.DeviceId == DeviceId;
                        if (!result)
                            error += " + INVALID ID/MESSAGE  ID=" + MessageId + "/" + r.MessageId + " - " + DeviceId + "/" + r.DeviceId;
                    }
                }
            }
            #endregion Verify if we are receiving an expecting response for the last fota sent

            if (!String.IsNullOrEmpty(error))
                STrace.Debug(typeof(GTEDeviceCommand).FullName, DeviceId, "GTE LEVEL   :" + error);
            error = "";

            if (result)
            {
                var gCommand = ">" + _command + ";ID=" + String.Format("{0:0000}", DeviceId) +
                               (MessageId != null ? ";#" + String.Format("{0:X4}", MessageId.Value) : "") + "<";
                var gDC =
                    GarminCommand.createFrom(gCommand);
                if (gDC.Success) // Success Parsing Garmin Command // its a Garmin Command
                {
                    var grDC = GarminCommand.createFrom(response);
                    if (grDC.Success)
                    {
                        result = gDC.isValidResponse(grDC);
                        /*
                        if (!result)
                            error += " + '" + response + "' WAS NOT AN EXPECTING RESPONSE";
                         */
                    }
                    if (!String.IsNullOrEmpty(error))
                        STrace.Debug(typeof(GTEDeviceCommand).FullName, DeviceId, "GARMIN LEVEL:" + error);
                    else
                        STrace.Debug(typeof(GTEDeviceCommand).FullName, DeviceId, "GARMIN LEVEL: + EXPECTED RESPONSE");
                } else
                {
                    STrace.Debug(typeof(GTEDeviceCommand).FullName, DeviceId, "GTE LEVEL   : + EXPECTED RESPONSE");
                }
            }

            return result;
        }
        public String getStatus()
        {
            return (_attributes.ContainsKey(Attributes.Status) ? _attributes[Attributes.Status] : null);

        }
        public void setStatus(String newStatus)
        {
            if (newStatus == null && _attributes.ContainsKey(Attributes.Status))
            {
                _attributes.Remove(Attributes.Status);
            }else
                _attributes[Attributes.Status] = newStatus;
        }
    }
}
