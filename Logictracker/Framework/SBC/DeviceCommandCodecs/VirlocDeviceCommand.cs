using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    public class VirlocDeviceCommand : TrimbleDeviceCommand
    {
        protected const string commandPattern = @"(?:\>(?<command>[^;]*)[^\<]*\<)|(?:\>(?<command>RGB).{16})";

        public String MessageOrigin { get; set; }

        public class Attributes : TrimbleDeviceCommand.Attributes
        {
            public const string MessageOrigin_NN = "#";

            public static string[] MessageOrigins = new[] { MessageOrigin_NN };
        }

        public VirlocDeviceCommand(byte[] command) : this(command, null) { }

        public VirlocDeviceCommand(string command) : this(command, null, null) { }        

        public VirlocDeviceCommand(byte[] command, INode node) : this(DecodeByteArr(command), node, null) { }
        
        

        private static string DecodeByteArr(byte[] command)
        {
            if (command.Length >= 27 && isRGBCommand(command))
            {
                return BinaryCommand2StringCommand(command);
            }
            return byte2string(command);
        }

        private static bool isRGBCommand(byte[] bArr)
        {
            return Convert.ToChar(bArr[0]) == '>' && Convert.ToChar(bArr[1]) == 'R' && Convert.ToChar(bArr[2]) == 'G' && Convert.ToChar(bArr[3]) == 'B'; // prefix 4 bytes (>RGB) + 22 bytes of data (bits)
        }

        private static string BinaryCommand2StringCommand(byte[] b)
        {
            if (isRGBCommand(b))
            {
                var secsAnt = new BitArray(new[] {b[4 - 2]}).Append(b[4 - 1]).Append(b[4].LowBits(4)).ToNumeral();
                var semanasAnt = new BitArray(new[] { b[7 - 1].HighBits(4) }).Trim(4).Append(b[7]).ToNumeral();
                var fechaAnt = new DateTime(1980, 1, 6, 0, 0, 0, 0).AddDays(semanasAnt * 7).AddSeconds(secsAnt);

                var byte0 = b[3];
                var byte1 = b[4];
                var byte2 = b[5];
                var byte3 = b[6];
                
                var bSecs = new BitArray(new[] { byte2.LowBits(4) });
                bSecs = bSecs.Append(byte1);
                bSecs = bSecs.Append(byte0);
                var secs = bSecs.ReverseToNumeral();
                
                var bSemanas = new BitArray(new[] { byte3 });
                bSemanas = bSemanas.Append(byte2.HighBits(4));
                var semanas = bSemanas.ReverseToNumeral();
                
                var fecha = new DateTime(1980, 1, 6, 0, 0, 0, 0).AddDays(semanas*7).AddSeconds(secs);

                var evento = b[8];
                var direccion = b[9]*4;
                var entradas = b[10];
                var velocidad = b[11];
                var tipoposicion = b[12].LowBits(4);
                var edad = b[16].LowBits(5);

                #region longitud

                double longitud = -1;
                {
                    var a = new BitArray(new[] {b[15 - 3].HighBits(4)}).Trim(4).Append(b[15 - 2]).Append(b[15 - 1]).Append(b[15]);
                    var negado = a.Not();
                    negado.Set(0, a.Get(0));
                    var negadonum = negado.ToNumeral();
                    var sum1 = negadonum + 1;
                    var negadosum1 = sum1*-1;
                    longitud = negadosum1*.00001;
                }

                #endregion longitud

                #region latitud

                double latitud = -1;
                {
                    var a = new BitArray(new[] {b[19 - 3].HighBits(3)}).Trim(3).Append(b[19 - 2]).Append(b[19 - 1]).Append(b[19]);
                    var negado = a.Not();
                    negado.Set(0, a.Get(0));
                    var negadonum = negado.ToNumeral();
                    var sum1 = negadonum + 1;
                    var negadosum1 = sum1*-1;
                    latitud = negadosum1*.00001;
                }

                #endregion latitud

                var dispositivoId = System.Text.Encoding.ASCII.GetString(new[] {b[20], b[21], b[22], b[23]});
                var mensajeNro = BitConverter.ToInt16(b, 24);

                var result = new StringBuilder();

                result.Append(String.Format(">RGP{0}", fecha.ToString("ddMMyyHHmmss")));
                result.Append(String.Format("{0:00.00000}", latitud).Replace(".", "").Replace(",", ""));
                result.Append(String.Format("{0:000.00000}", longitud).Replace(".", "").Replace(",", ""));
                result.Append(String.Format("{0:000}{1:000}{2:0}{3:00}{4:X2}{5:#00}{6:00}", velocidad, direccion, tipoposicion, edad, entradas, evento, 0));
                result.Append(String.Format(";ID={0:0000};#{1:X4}<", dispositivoId, mensajeNro));

                return result.ToString();
            }

            return byte2string(b);

        }

        public VirlocDeviceCommand(string command, INode node, DateTime? ExpiresOn) : base(command, node, ExpiresOn)
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
            return false;
        }

        protected override bool isSettingIDMessage()
        {
            var cmd = getCommand();
            return (cmd.StartsWith("SID"));
        }

        public static bool isGarminMessage(string message)
        {
            return false;
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
                        (cmdPrefix.Contains("*") && cmdPrefix == rCmdPrefix.Replace("*", " "))) ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid;

                }
                else
                {
                    var cmdPrefix = _command.Substring(1, 2);
                    var rCmdPrefix = rcmd.Substring(1, 2);
                    result = (DeviceCommandResponseStatus.Valid == result && (cmdPrefix == rCmdPrefix)) ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid;

                    if (DeviceCommandResponseStatus.Valid != result)
                        error += " + '" + rCmdPrefix + "' IS NOT AN EXPECTED RESPONSE FOR '" + cmdPrefix + "'";
                    else
                    if (DeviceCommandResponseStatus.Valid == result)
                    {
                        result = ((response.MessageId == null || response.MessageId == MessageId) && response.IdNum == IdNum) ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid;
                        if (DeviceCommandResponseStatus.Valid != result)
                            error += " + INVALID ID/MESSAGE  ID=" + MessageId + "/" + response.MessageId + " - " + IdNum +
                                     "/" + response.IdNum;
                    }
                }

                if (!String.IsNullOrEmpty(error))
                    STrace.Debug(typeof (VirlocDeviceCommand).FullName, IdNum ?? 0, "GTE LEVEL   :" + error);
            }
            return result;
        }

        protected override void addCustomToString(bool clean, ref StringBuilder result)
        {
            if (MessageId != null)
            {
                UInt16 mid = Convert.ToUInt16(MessageId);
                result.AppendFormat(";{0}{1:X4}", Attributes.MessageId, mid);
            }
        }

        protected override void addCustomCheckSum(bool clean, ref StringBuilder result)
        {
            result.Append(";");
            result.AppendFormat("*{0:X2}", CalculateCheckSum(result.ToString()));
        }

        #region Commands Parsers
        public DeviceStatus ParsePosition()
        {            
            if (!(new[] {"RGP", "RGL"}.Any(d=> _command.StartsWith(d)))) return null;



            #region command dependent lengths attributes (RGP=47|48/RGL=45|46)
            var eventoLength = (_command.Length == 48 || _command.Length == 46 ? 3 : 2);
            var evento = Convert.ToByte(_command.Substring(43, eventoLength));
            var hdop = _command.Length < 47 ? 0 : Convert.ToByte(_command.Substring(43 + eventoLength, 2));
            #endregion command dependent lengths attributes

            var entradas = Convert.ToByte(_command.Substring(41, 2), 16);
            //var tipoDePos = Convert.ToSingle(_command.Substring(38, 1));
            //var edad = Convert.ToByte(_command.Substring(39, 2), 16);

            //if (edad > 300)
            //    return null;

            var time = DateTimeUtils.SafeParseFormat(_command.Substring(3, 12), "ddMMyyHHmmss");
            var lat = Convert.ToSingle(_command.Substring(15, 8))*(float) 0.00001;
            var lon = Convert.ToSingle(_command.Substring(23, 9))*(float) 0.00001;
            var vel = Convert.ToSingle(_command.Substring(32, 3));
            var dir = Convert.ToSingle(_command.Substring(35, 3)); // (0 ..359), Norte = 0, Este = 90, Sur = 180, Oeste = 270
            
            /*
            Estado del posicionamiento del GPS  de la ultima posición valida

            0 = Posicionando con solo 1 satélite.

            1 = Posicionando con 2 satélites 

            2 = Posicionando con 3 satélites (2D).

            3 = Posicionando con 4 o más satélites (3D).

            4 = Posición en 2D calculada por aproximación de cuadrados mínimos.

            5 = Posición en 3D calculada por aproximación de cuadrados mínimos.

            6 = Posición calculada sin satélites, por velocidad, sentido y tiempo.

            8 = Antena en corto circuito

            9 = Antena no conectada

             */
            

            /*

            bit 7 Ignición.

            bit 6 Fuente de poder principal.

            bit 5 Entrada digital 5

            bit 4 Entrada digital 4

            bit 3 Entrada digital 3.

            bit 2 Entrada digital 2.

            bit 1 Entrada digital 1.

            bit 0 Entrada digital 0.
            */

            var devId = (Int32?) null;

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
                STrace.Exception(typeof(VirlocDeviceCommand).FullName, e, IdNum ?? 0, String.Format("Posición inválida {0}", getCommand()));
                gpoint = null;
            }

            var result = new DeviceStatus(devId, gpoint, evento, entradas);
            return result;
        }

        #endregion Commands Parsers

        #region instance tools
        public override BaseDeviceCommand BuildAck()
        {
            var r = new VirlocDeviceCommand(">ACK<")
            {
                IdNum = IdNum,
                MessageId = MessageId
            };
            return r;
        }
        #endregion instance tools

        protected override string getCommandPattern()
        {
            return commandPattern;
        }

    }
}
