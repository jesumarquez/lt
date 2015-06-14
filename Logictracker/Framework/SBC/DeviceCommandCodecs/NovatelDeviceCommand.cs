using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;
using System.Reflection;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    public class NovatelDeviceCommand : TrimbleDeviceCommand
    {
        protected const string commandPattern = @"(?:\>(?<command>[^;]*)[^\<]*\<)|(?:\>(?<command>RGB).{16})";

        public String MessageOrigin { get; set; }
        public String ModemID { get; set; }

        public class Attributes : TrimbleDeviceCommand.Attributes
        {
            public const string MessageOrigin_NN = "#";

            public static string[] MessageOrigins = new[] { MessageOrigin_NN };
        }


        public NovatelDeviceCommand(byte[] command) : this(command, null) { }

        public NovatelDeviceCommand(string command) : this(command, null, null) { }

        public NovatelDeviceCommand(byte[] command, INode node) : this(DecodeByteArr(command), node, null) { }

        private static string DecodeByteArr(byte[] b)
        {
            NovatelObjectSerialized parser = new NovatelObjectSerialized();
            parser.SetStructData(b);
            var result = new StringBuilder();
            foreach (PropertyInfo propInfo in parser.GetType().GetProperties())
            {
                result.Append("<" + (string) propInfo.GetValue(parser, null) + ">");
            }
           /* string edad = "0";
            string entradas = "0";
            string evento = "0";
            result.Append(String.Format(">RGP{0}", parser.Date + parser.UTCPosition));
            result.Append(String.Format("{0:00.00000}", parser.Latitude).Replace(".", "").Replace(",", ""));
            result.Append(String.Format("{0:000.00000}", parser.Longitude).Replace(".", "").Replace(",", ""));
            result.Append(String.Format("{0:000}{1:000}{2:0}{3:00}{4:X2}{5:#00}{6:00}", parser.SpeedOverGround, parser.CourseOverGround, parser.Indicator, edad, entradas, evento, 0));
            result.Append(String.Format(";ID={0:0000};#{1:X4}<", parser.ModemID, parser.MessageID));*/
            return result.ToString();
        }
    
        
        

        public NovatelDeviceCommand(string command, INode node, DateTime? ExpiresOn)
            : base(command, node, ExpiresOn)
        {
            #region MessageId

            string msgkey = Attributes.MessageOrigins.FirstOrDefault(_attributes.ContainsKey);
            if (msgkey != null)
            {
                if (_attributes.ContainsKey(msgkey))
                {
                    try
                    {
                        MessageId = Convert.ToUInt16(_attributes[msgkey], 16);
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

        protected override string getCommandPattern()
        {
            return commandPattern;
        }

        public override bool isGarminMessage()
        {
            return false;
        }

        protected override bool isSettingIDMessage()
        {
            var cmd = getCommand();
            return (cmd.StartsWith("SID"));
        }

        protected override void addCustomToString(bool clean, ref StringBuilder result)
        {
            if (MessageId != null)
            {
                UInt16 mid = Convert.ToUInt16(MessageId);
                result.AppendFormat(";{0}{1:X4}", Attributes.MessageId, mid);
            }
        }


        #region instance tools
        public override BaseDeviceCommand BuildAck()
        {
            var r = new NovatelDeviceCommand(">ACK<")
            {
                IdNum = IdNum,
                MessageId = MessageId
            };
            return r;
        }
        #endregion instance tools

        protected override void addCustomCheckSum(bool clean, ref StringBuilder result)
        {
            result.Append(";");
            result.AppendFormat("*{0:X2}", CalculateCheckSum(result.ToString()));
        }

        public DeviceStatus ParsePosition()
        {
            var eventoLength = (_command.Length == 48 || _command.Length == 46 ? 3 : 2);
            var evento = (byte)0;// Convert.ToByte(_command.Substring(43, eventoLength));
            var hdop = (byte)0;// _command.Length < 47 ? 0 : Convert.ToByte(_command.Substring(43 + eventoLength, 2));
            string[] parse = _command.Split(new[] {"<", ">"}, StringSplitOptions.RemoveEmptyEntries); 
            var entradas = Convert.ToByte("0", 16);
            var time = DateTimeUtils.SafeParseFormat(parse[10] + parse[2].Split('.')[0], "ddMMyyHHmmss");

            /*
                Latitude: DD MM.MMMM
                Longitude: DDD MM.MMMM
             3259.816776,N
             09642.858868,W
             latitud max -90 +90
             32*59.816776
             longitud max -180 + 180
             096*42.858868             
             */

            /*
             If latitude direction is South, then:
             Latitude = Latitude * -1
             */
            int latitudSignal = 1;
            if (parse[5].Equals("S"))
                latitudSignal = -1;
            /*
                If longitude direction is West, then:
                Longitude = Longitude * -1
             */
            int longitudSignal = 1;
            if (parse[7].Equals("W"))
                longitudSignal = -1;

            parse[4] = parse[4].Insert(2,",").Replace(".","").Replace(",",".");//(double.Parse(parse[4].Replace('.', ',')) * latitudSignal) / 100).ToString("0.00001").Replace('.', ',');
            parse[6] = parse[6].Insert(3, ",").Replace(".", "").Replace(",", "."); // ((double.Parse(parse[6].Replace('.', ',')) * longitudSignal) / 100).ToString("0.00001").Replace('.', ',');

            var lat = Convert.ToSingle(parse[4].Split('.')[0]) * latitudSignal;
            var lon = Convert.ToSingle(parse[6].Split('.')[0]) * longitudSignal;
            var vel = Convert.ToSingle(parse[8]);
            var dir = Convert.ToSingle(parse[9]); // (0 ..359), Norte = 0, Este = 90, Sur = 180, Oeste = 270

            var devId = (Int32?)null;

            if (_node != null)
                devId = _node.Id;


            GPSPoint gpoint = null;
            try
            {
                gpoint = new GPSPoint(time, lat, lon, vel, GPSPoint.SourceProviders.Unespecified, 0)
                {
                    Course = new Course(dir),
                    HDOP = hdop,
                    IgnitionStatus = BitHelper.AreBitsSet(entradas, 7) ? IgnitionStatus.On : IgnitionStatus.Off
                };
                if (devId != null)
                    gpoint.DeviceId = devId.Value;
            }
            catch (ArgumentOutOfRangeException e)
            {
                STrace.Exception(typeof(NovatelDeviceCommand).FullName, e, IdNum ?? 0, String.Format("Posición inválida {0}", getCommand()));
                gpoint = null;
            }

            var result = new DeviceStatus(devId, gpoint, evento, entradas);
            return result;
        }
    }
}
