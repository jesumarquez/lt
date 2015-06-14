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
    public class AbsolutDeviceCommand : TrimbleDeviceCommand
    {
        protected const string commandPattern = @"(?:\>(?<command>[^;]*)[^\<]*\<)|(?:\>(?<command>RGB).{16})";

        public String MessageOrigin { get; set; }
        public String ModemID { get; set; }

        public class Attributes : TrimbleDeviceCommand.Attributes
        {
            public const string MessageOrigin_NN = "#";

            public static string[] MessageOrigins = new[] { MessageOrigin_NN };
        }


        public AbsolutDeviceCommand(byte[] command) : this(command, null) { }

        public AbsolutDeviceCommand(string command) : this(command, null, null) { }

        public AbsolutDeviceCommand(byte[] command, INode node) : this(DecodeByteArr(command), node, null) { }

        private static string DecodeByteArr(byte[] b)
        {
            var parser = new AbsolutObjectSerialized();
            parser.SetStructData(b);
            var result = new StringBuilder();
            foreach (PropertyInfo propInfo in parser.GetType().GetProperties())
            {
                result.Append("<" + (string) propInfo.GetValue(parser, null) + ">");
            }
            return result.ToString();
        }




        public AbsolutDeviceCommand(string command, INode node, DateTime? ExpiresOn)
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
            var r = new AbsolutDeviceCommand(">ACK<")
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
            // orden del array

            //0   Encabezado del paquete
            //1   IMEI del equipo
            //2   Número de paquete            string[] parse = _command.Split(new[] { "<", ">" }, StringSplitOptions.RemoveEmptyEntries); 
            //3   Evento que generó el paquete            var eventoLength = (_command.Length == 48 || _command.Length == 46 ? 3 : 2);
            //4   Nivel de señal GSM. (Entre 0 y 31)            var evento = (byte)0;
            //5   U1 en voltios            var hdop = (byte)0;
            //6   I1 en Ampere por 10. (10 equivale a 1.0 Amp)            var entradas = Convert.ToByte("0", 16);
            //7   U2 en voltios            var time = DateTimeUtils.SafeParseFormat(parse[10] + parse[2].Split('.')[0], "ddMMyyHHmmss");
            //8   I2 en Ampere por 10. (27 equivale a 2.7 Amp)
            //9   D1 Entrada DIN1 inactiva            /*
            //10  D2 Entrada D2 activa                Latitude: DD MM.MMMM
            //11  T1 Temperatura 25ºC                Longitude: DDD MM.MMMM   T1:valor,T2:valor
            //12  T2 Temperatura 23ºC             3259.816776,N
            //13  T3 Sensor NTC3 abierto             09642.858868,W
            //14  T4 Sensor NTC4 en cortocircuito             latitud max -90 +90
            //15  Latitud             32*59.816776
            //16  Longitud             longitud max -180 + 180
            //17  Modelo y versión de firmware del equipo             096*42.858868             
            //18  Fecha y hora de la generación del evento GMT0             */
            //19  Fin de paquete


            string[] parse = _command.Split(new[] { "<", ">" }, StringSplitOptions.RemoveEmptyEntries);
            var entradas = Convert.ToByte("0", 16);
            var time = DateTimeUtils.SafeParseFormat(parse[17], "yyyy-MM-dd HH:mm:ss");

            //-34.603718 Latitud
            //-58.38158 Longitud
            var lat = Convert.ToSingle(parse[14].Split('.')[0]);
            var lon = Convert.ToSingle(parse[15].Split('.')[0]);
            var hdop = Convert.ToByte(parse[3]);
            var evento = Convert.ToByte(parse[3]);
            var vel = Convert.ToSingle(0);
            var dir = Convert.ToSingle(1); // (0 ..359), Norte = 0, Este = 90, Sur = 180, Oeste = 270
            _node.Imei = parse[3];

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
                STrace.Exception(typeof(AbsolutDeviceCommand).FullName, e, IdNum ?? 0, String.Format("Posición inválida {0}", getCommand()));
                gpoint = null;
            }

            var result = new DeviceStatus(devId, gpoint, evento, entradas);
            return result;
        }
    }
}
