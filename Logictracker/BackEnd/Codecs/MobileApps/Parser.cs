using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace Logictracker.MobileApps.v1
{
    [FrameworkElement(XName = "MobileAppsParser", IsContainer = false)]
    public class Parser : BaseCodec, IShortMessage, IFoteable, IRoutable
    {
        public override NodeTypes NodeType { get { return NodeTypes.Mobileapps; } }

        #region Attributes

        [ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2007)]
        public override int Port { get; set; }

        private String _fotaFolder;

        public String FotaFolder
        {
            get
            {
                if (_fotaFolder == null)
                    _fotaFolder = Process.GetApplicationFolder("FOTA");
                return _fotaFolder;
            }
        }

        #endregion

        #region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

		public override INode Factory(IFrame frame, int formerId)
        {
            var buffer = GetEntrada(frame.Payload);
            if (String.IsNullOrEmpty(buffer)) return null;
            var x = buffer.StartsWith(Reporte.HandShake) ?
                DataProvider.FindByIMEI(buffer.Split(',')[2], this) :
                DataProvider.Get(ParserUtils.GetDeviceIdTaip(buffer), this);
            Console.WriteLine("Factory X: " + x.GetDeviceId());
            return x;
        }

        public override IMessage Decode(IFrame frame)
        {
            var buffer = GetEntradas(frame.Payload);
            //var salida = new UserMessage(Id, NextSequence);
            //foreach (var m in buffer.Select(s => Decode2(s)))
            //{
              //  salida.AddStringToSend(m.GetPendingAsString());
                //salida.AddStringToPostSend(m.GetPendingPostAsString());
            //}
            return Decode2(buffer.FirstOrDefault());
        }

        private IMessage Decode2(string buffer)
        {
            if (buffer == null) return null;
            if (ParserUtils.IsInvalidDeviceId(Id) && !buffer.StartsWith(Reporte.HandShake)) return FactoryQueryHandShake(this);

            IMessage salida;
			var data = buffer.Split(';')[0].Split(',');
			var pos = GetPos(data);
			var msgId = GetMessageId(buffer);
	        var tipoReporte = data[0];
			//var requestTimestamp = data.SingleOrDefault(s => s.StartsWith("rt"));

            switch (tipoReporte)
            {
                case Reporte.HandShake:
                    salida = FactoryHandShake(this, data, msgId, pos, DataProvider);
                    break;
                case Reporte.KeepAlive:
                    salida = FactoryKeepAlive(this, data, msgId);
                    break;
                case Reporte.Position:
                    salida = FactoryPositionOrEvent(this, data, msgId);
                    break;
                case Reporte.Mensaje:
                    salida = FactoryMensaje(this, data, msgId, pos);
                    break;
                default: //es un ack o un mensaje no reconocido, no se responde, solo paso la info arriba
                    salida = new UserMessage(Id, msgId);
                    break;
            }

            CheckFota((int)msgId);

            var fotaCmd = Fota.Peek(this);

            if (!(salida is ConfigRequest) && !string.IsNullOrEmpty(fotaCmd))
            {
                Lastsentmessageid = (ulong)GetMsgId(fotaCmd);
                salida.AddStringToPostSend(fotaCmd);
            }

            return salida;
        }

		public ulong GetMessageId(String line)
	    {
			var msgIdS = line.Split(';').SingleOrDefault(s => s.StartsWith("MSG="));
		    var msgId = msgIdS != null ? Convert.ToUInt64(msgIdS.Split('=')[1], 16) : ParserUtils.MsgIdNotSet;
		    return msgId;
	    }

	    public INodeMessage LastSent { get; set; }

	    private void CheckFota(int msgId)
        {
            if (Lastsentmessageid == (ulong)msgId)
            {
                Fota.Dequeue(this, null);
            }
        }

		private static GPSPoint GetPos(string[] data)
        {
			var dt = data.SingleOrDefault(s => s.StartsWith("t="));
			if (dt == null) return null;
            var lat = Convert.ToSingle(data.SingleOrDefault(s => s.StartsWith("lat=")) ?? "0", CultureInfo.InvariantCulture);
			if (lat == 0.0) return null;
            var lon = Convert.ToSingle(data.SingleOrDefault(s => s.StartsWith("lon=")) ?? "0", CultureInfo.InvariantCulture);
			return lon == 0.0 ? null : GPSPoint.Factory(DateTimeUtils.SafeParseFormat(dt, "ddMMyyHHmmss"), lat, lon);
        }

        protected override void OnMemberwiseClone()
        {
        }

        public override bool IsPacketCompleted(byte[] payload, int start, int count, out int detectedCount, out bool ignoreNoise)
        {
            ignoreNoise = false;
            for (var i = start; i < count; i++)
            {
                var b = payload[i];
                if (b != '<') continue;

                detectedCount = i - start + 1;
                var text = new byte[detectedCount];
                Array.Copy(payload, text, detectedCount);
                var mystring = Encoding.ASCII.GetString(text);
                Console.WriteLine("Detected PDU: " + mystring);
                return true;
            }
            detectedCount = count - start;
            return false;
        }

        #endregion

        #region IShortMessage

        public bool SetCannedMessage(ulong messageId, int code, String message, int revision)
        {
            //Console.WriteLine("------------- > SetCannedMessage did={0} text={1}", Id, message);
            if (messageId == 0) messageId = NextSequence;
            var msg = Factory<String>((ulong)messageId, this, "SP,0,messages,{0},{1},{2}", code, HttpUtility.UrlEncode(message), revision);

            SendMessages(msg, NextSequence, messageId, false);
            return true;
        }

        public bool SetCannedResponse(ulong messageId, int code, String response, int revision)
        {
            // Console.WriteLine("============= > SetCannedResponse did={0} text={1}", Id, response);
            if (messageId == 0) messageId = NextSequence;
            var msg = Factory<String>((ulong)messageId, this, "SP,0,messages,{0},{1},{2}", code, HttpUtility.UrlEncode(response), revision);

            SendMessages(msg, NextSequence, messageId, false);
            return true;
        }

        public bool DeleteCannedMessage(ulong messageId, int code, int revision)
        {
            Console.WriteLine("Delete Canned Message Received");
            if (messageId == 0) messageId = NextSequence;
            //if (code == 0) //borrar todos los canned messages y enviarlos nuevamente
            {
                SendMessages(Factory<String>((ulong)messageId, this, "DA,0"), (ulong)messageId, messageId, false);
                var messagesParameters = DataProvider.GetCannedMessagesTable(Id, 0).OrderBy(m => m.Codigo).Where(m => !m.TipoMensaje.DeEstadoLogistico).ToList();
                Console.WriteLine("Messages: Q={0}", messagesParameters.Count());
                foreach (var mensaje in messagesParameters)
                {
                    Console.WriteLine("Send Canned Messsage: {0}", mensaje.Texto);
                    var mid = NextSequence;
                    SendMessages(Factory<String>(mid, this, "SP,0,messages,{0},{1},{2}", mensaje.Codigo, HttpUtility.UrlEncode(mensaje.Texto), mensaje.Revision), mid, mid, false);
                }
            }
            /*else
            {
                throw new NotImplementedException("DeleteCannedMessage de mobile apps aun no implementa aun el borrado de un solo mensaje predefinido");
            }*/
            return true;
        }

        public bool SubmitCannedMessage(ulong messageId, int code, int[] replies)
        {
            //Console.WriteLine("############# > SubmitCannedMessage did={0} code={1}", Id, code);
            if (messageId == 0) messageId = NextSequence;
            var m = DataProvider.GetCannedMessagesTable(Id, 0).SingleOrDefault(mm => Convert.ToInt32(mm.Codigo) == code);
            const string prio = "medium";
            const string source = "server";
            var destination = Id.ToString("D4");
            const string contentType = "text/predefined";
            var msg = Factory<String>((ulong)messageId, this, "TM,0,{0},{1},{2},{3},{4},{5:yyMMddhhmmss},{6},{7}", messageId, prio, source, destination, contentType, DateTime.UtcNow, m.Codigo, HttpUtility.UrlEncode(m.Texto));

            SendMessages(msg, NextSequence, messageId, false);
            return true;
        }

        public bool SubmitTextMessage(ulong messageId, uint textMessageId, string textMessage, uint[] replies, int ackEvent)
        {
            throw new NotImplementedException();
        }

        public bool SubmitTextMessage(ulong messageId, String textMessage, int[] replies)
        {
            if (textMessage.StartsWith("8492:"))
            {
                return ProcessTurno(messageId, textMessage, replies);
            }
            return SubmitTextMessage2(messageId, textMessage, replies);
        }

        private bool ProcessTurno(ulong messageId, string textMessage, int[] replies)
        {
            var para = textMessage.Split(':');
            if (para[0] != "8492") return false;
            if (para.Length != 5) return false;
            var mid = NextSequence;
            SendMessages(Factory<String>(mid, this, "SP,0,{0},{1},{2},{3}", para[1], para[2], para[3], para[4]), mid, mid, false);
            return true;
        }

        private bool SubmitTextMessage2(ulong messageId, String textMessage, int[] replies)
        {
            //Console.WriteLine("############# > SubmitTextMessage did={0} text={1}", Id, textMessage);
            if (messageId == 0) messageId = NextSequence;
            const string prio = "medium";
            const string source = "server";
            var destination = Id.ToString("D4");
            const string contentType = "text/plain";
            var msg = Factory<String>((ulong)messageId, this, String.Format("TM,0,{0},{1},{2},{3},{4},{5:yyMMddhhmmss},{6},{7}", messageId, prio, source, destination, contentType, DateTime.UtcNow, 0, HttpUtility.UrlEncode(textMessage)));

            SendMessages(msg, NextSequence, messageId, false);
            return true;
        }

        private void SendMessages(IEnumerable<string> text, ulong messgaId, Boolean sendnowflag)
        {
            text.ToList().ForEach(t => SendMessages(t, NextSequence, messgaId, sendnowflag));
        }

        private void SendMessages(String text, ulong messgaId, Boolean sendnowflag)
        {
            SendMessages(text, NextSequence, messgaId, sendnowflag);
        }

        private void SendMessages(String text, ulong sequence, ulong messageId, Boolean sendnowflag)
        {
            //STrace.Debug("", Id, "SendMessages did={0} msg={1}", Id, text);
            var msg = new UserMessage(Id, sequence).AddStringToSend(text);

            if (!sendnowflag)
                Fota.Enqueue(this, messageId, text);
            else
                DataLinkLayer.SendMessage(Id, msg);
        }

        #endregion

        #region Factory

        private static T Factory<T>(ulong messageId, INode dev, String cmd, params object[] values)
        {
            if ((messageId == ParserUtils.MsgIdNotSet || messageId == 0) && dev != null) messageId = dev.NextSequence;

            var did = dev == null ? 0 : dev.Id;

            var resBuilder = new StringBuilder(">");

            if (values != null && values.Length > 0)
                resBuilder.AppendFormat(CultureInfo.InvariantCulture, cmd, values);
            else
                resBuilder.Append(cmd);

            //if (did != 0)
            resBuilder.AppendFormat(";ID={0:D4}", did);

            if ((messageId != ParserUtils.CeroMsgId) && (messageId != 0))
                resBuilder.AppendFormat(";MSG={0:X4}", messageId);

            resBuilder.Append(";*");

            resBuilder.AppendFormat("{0:X2}<", GetCheckSum(resBuilder.ToString()));
            var res = resBuilder.ToString();

            if (typeof(T) == typeof(String))
                return (T)(object)res;
            if (typeof(T) == typeof(UserMessage))
                return (T)(object)new UserMessage(did, messageId).AddStringToSend(res);

            throw new NotSupportedException(String.Format(@"El Tipo ""{0}"" no esta implementado en {1}.{2}", typeof(T), typeof(Parser).FullName, "Factory"));
        }
        #endregion


        private static int GetMsgId(string buffer)
        {
            var ini = buffer.IndexOf(";MSG=", StringComparison.Ordinal);

            if (ini == -1) return (int)ParserUtils.MsgIdNotSet;

            ini += 5;

            var len = buffer.IndexOf(';', ini) - ini;

            var res = Convert.ToInt32(buffer.Substring(ini, len), 16);
            if (res == 0) res = (int)ParserUtils.CeroMsgId;
            return res;
        }

        #region Factorys Especificos

        private static IMessage FactoryQueryHandShake(Parser dev)
        {
            return Factory<UserMessage>(0, dev, "QH,0").SetUserSetting("DoNotLog", "True");
        }

        private static IMessage FactoryHandShake(Parser dev, String[] data, ulong msgId, GPSPoint pos, IDataProvider dataProvider)
        {
            if (ParserUtils.IsInvalidDeviceId(dev.Id)) return null;

            var msg = new ConfigRequest(dev.Id, msgId);
            if (pos != null) msg.GeoPoint = pos;

            msg.AddStringToSend(Factory<String>(msgId, dev, "AH,0,{0:D4},{1}", dev.Id, DateTime.UtcNow.ToUnixTimeStamp()));

            //check Telephone
            if (data.Length > 4)
            {
                var phone = data[4].TrimStart("+".ToCharArray());
                Int64 number;
                if (!String.IsNullOrEmpty(phone) && Int64.TryParse(phone, out number) && number > 0)
                {
                    var oldnumber = dataProvider.GetDetalleDispositivo(dev.Id, "Telephone").As("null");
                    if (oldnumber != phone)
                    {
                        STrace.Debug(typeof(Parser).FullName, dev.Id, String.Format("Setting Telephone: {0}", phone));
                        dataProvider.SetDetalleDispositivo(dev.Id, "Telephone", phone, "string");
                    }
                }
            }

            //check config_revision
            if (data.Length > 3)
            {
                var revision = data[3];
                var oldrevision = dataProvider.GetDetalleDispositivo(dev.Id, "config_revision").As("");
                if (oldrevision != revision && !String.IsNullOrEmpty(revision))
                {
                    STrace.Debug(typeof(Parser).FullName, dev.Id, String.Format("Setting config_revision: {0}", revision));
                    //todo: update config
                    //DataProvider.SetDetalleDispositivo(Id, "config_revision", revision, "int");
                }
            }

            return msg;
        }

        private static UserMessage FactoryKeepAlive(Parser dev, IEnumerable<String> data, ulong msgId)
        {
            var extra = (data.SingleOrDefault(s => s.StartsWith("rt=")) != null) ? ",t=" + DateTime.UtcNow.ToUnixTimeStamp() : "";
            return Factory<UserMessage>(msgId, dev, "AK,0{0}", extra).SetUserSetting("user_message_code", "KEEPALIVE");
        }

        private static IMessage FactoryPositionOrEvent(Parser dev, String[] data, ulong msgId)
        {
            IMessage res;
            var extra = (data.LastOrDefault(s => s.StartsWith("rt")) != null) ? ",t=" + DateTime.UtcNow.ToUnixTimeStamp() : "";
            
            var date = DateTimeUtils.SafeParseFormat(data[2], "ddMMyyHHmmss");
            
            var pos = GPSPoint.Factory(
                date,
                Convert.ToSingle(data[3], CultureInfo.InvariantCulture),
                Convert.ToSingle(data[4], CultureInfo.InvariantCulture),
                Convert.ToSingle(data[5], CultureInfo.InvariantCulture),
                Convert.ToSingle(data[6], CultureInfo.InvariantCulture),
                0, // Altitude
                0, // Hdop
                GetProvider(data[7]), // SourceProvider
                Convert.ToSingle(data[8], CultureInfo.InvariantCulture)); // HorizontalAccuracy

            // si la lat es cero, no hay posicion.
            if (pos.Lat == 0.0) pos = null;            

            var ev = data.SingleOrDefault(s => s.StartsWith("e="));
            if (ev != null)
            {
                var codigo = (MessageIdentifier)Convert.ToInt32(ev.Substring(2));
                if (codigo == MessageIdentifier.DeviceTurnedOn)
                {
                    date = DateTime.UtcNow;
                }
                var xd = data.SingleOrDefault(s => s.StartsWith("x="));
                List<Int64> xdata = null;
                if (xd != null)
                {
                    xdata = new List<Int64> {Convert.ToInt32(xd.Substring(2))};
                    var xd2 = data.SingleOrDefault(s => s.StartsWith("x2="));
                    if (xd2 != null)
                    {
                        xdata.Add(Convert.ToInt32(xd2.Substring(3)));
                    }
                }
                res = codigo.FactoryEvent(dev.Id, msgId, pos, date, null, xdata);
            }
            else
            {
                res = pos.ToPosition(dev.Id, msgId);
            }

            return res.AddStringToSend(Factory<String>(msgId, dev, "AP,0{0}", extra));
        }

        private static GPSPoint.SourceProviders GetProvider(string provider) {
            if (String.IsNullOrEmpty(provider)) return GPSPoint.SourceProviders.Unespecified;
            if (provider.ToLower() == "network") return GPSPoint.SourceProviders.NetworkProvider;
            if (provider.ToLower() == "agps") return GPSPoint.SourceProviders.AGpsProvider;
            return GPSPoint.SourceProviders.Unknown;
        }

        private static IMessage FactoryMensaje(Parser dev, String[] data, ulong msgId, GPSPoint pos)
        {
            IMessage msg;
            var extra = (data.SingleOrDefault(s => s.StartsWith("rt")) != null) ? ",t=" + DateTime.UtcNow.ToUnixTimeStamp() : "";

            var t = DateTimeUtils.SafeParseFormat(data[7], "ddMMyyHHmmss");
            var tmc = (MessageIdentifier)Convert.ToInt16(data[8], CultureInfo.InvariantCulture);
            var body = data[9];

            if (tmc == MessageIdentifier.ReservedDoNotUse)
            {
                msg = new TextEvent(dev.Id, msgId, t)
                    {
						Text = HttpUtility.UrlDecode(body),
                        GeoPoint = pos,
                    };
            }
            else
            {
                msg = tmc.FactoryEvent(dev.Id, msgId, pos, t, null, null);
            }
            return msg.AddStringToSend(Factory<String>(msgId, dev, "AM,0,{0},{1}{2}", data[2], data[4], extra));
        }

        #endregion

        #region Private Members

        private static IEnumerable<string> GetEntradas(byte[] payload)
        {
            return Encoding.ASCII.GetString(payload).Split(new[] {"<"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s + "<").Select(k => GetEntrada(k));
        }

        private static String GetEntrada(string tmp)
        {
            try
            {
                var ini = tmp.IndexOf('>');
                var len = (tmp.IndexOf('<', ini) - ini) + 1;
                var msg = tmp.Substring(ini, len);
                if (!msg.Contains(";*")) return msg;
                var chkini = msg.IndexOf(";*", StringComparison.Ordinal) + 2;
                var chkOrig = Convert.ToInt32(msg.Substring(chkini, 2), 16);
                var chkCalc = GetCheckSum(msg);
                if (chkCalc != chkOrig)
                {
                    STrace.Debug(typeof(Parser).FullName, ParserUtils.GetDeviceIdTaip(msg), String.Format("Mensaje con error de checksum: {0}", msg));
                    return null;
                }
                return msg;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Parser).FullName, e, tmp);
                return null;
            }
        }

        private static String GetEntrada(byte[] payload)
        {
            return GetEntrada(Encoding.ASCII.GetString(payload));
        }

        private static int GetCheckSum(String message)
        {
            var lon = message.IndexOf(";*", StringComparison.Ordinal);
            if (lon == -1)
                lon = message.Length;
            else
                lon += 2;

            var chksum = 0;
            for (var i = 0; i < lon; i++)
                chksum ^= message[i];

            return chksum;
        }

        private abstract class Reporte
        {
            public const String HandShake = ">RH";
            public const String KeepAlive = ">KA";
            public const String Position = ">NP";
            public const String Mensaje = ">TM";
        }

        #endregion

        #region IFooteable
        public bool ReloadFirmware(ulong messageId)
        {
            return true;
        }

        public bool ReloadMessages(ulong messageId)
        {
            return true;
        }

        public bool ResetFMIOnGarmin(ulong messageId)
        {
            throw new NotImplementedException();
        }

        public Boolean? IsGarminConnected { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool ReloadConfiguration(ulong messageId)
        {
            return true;
        }

        public bool ContainsMessage(string line)
        {
            return line.Trim().StartsWith(">");
        }

        #endregion

        #region IRoutable Members

        public bool ReloadRoute(ulong messageId, Destination[] route, bool sort, int routeId)
        {
            return LoadRoute(messageId, route, sort, routeId);
        }


		public bool LoadRoute(ulong messageId, Destination[] route, bool sort, int routeId)
        {

            // Purgo los viajes
            var msg = Factory<String>((ulong)messageId, this, "SP,0,trip,start,{0},{1}", routeId, "x");
            SendMessages(msg, messageId, false);
			foreach (var destination in route)
            {
                SendMessages(new[]
                               {
                                   //>SP,0,trip,code,stringconcodigo,x;ID=123;MSG=ABC;*AB<
                                   Factory<String>(NextSequence, this, "SP,0,trip,code,{0},x", destination.Code),
                                   //>SP,0,trip,name,stringconcodigo,stringname;ID=123;MSG=ABC;*AB< 
                                   Factory<String>(NextSequence, this, "SP,0,trip,name,{0},{1}", destination.Code,
                                                   HttpUtility.UrlEncode(destination.Text)),
                                   //>SP,0,trip,description,stringconcodigo,stringdescription;ID=123;MSG=ABC;*AB<
                                   Factory<String>(NextSequence, this, "SP,0,trip,description,{0},{1}", destination.Code,
                                                   HttpUtility.UrlEncode(destination.Address)),
                                   //>SP,0,trip,latitude,stringconcodigo,-34.234556;ID=123;MSG=ABC;*AB<
                                   Factory<String>(NextSequence, this, "SP,0,trip,latitude,{0},{1}", destination.Code,
                                                   destination.Point.Lat.ToString(CultureInfo.InvariantCulture)),
                                   //>SP,0,trip,longitude,stringconcodigo,-57.765463;ID=123;MSG=ABC;*AB<
                                   Factory<String>(NextSequence, this, "SP,0,trip,longitude,{0},{1}", destination.Code,
                                                   destination.Point.Lon.ToString(CultureInfo.InvariantCulture))
                               }
                , messageId, false);
            }
            msg = Factory<String>((ulong)messageId, this, "SP,0,trip,end,{0},{1}", "x", "x");
            SendMessages(msg, messageId, false);
            return true;
        }

        public bool UnloadRoute(ulong messageId, Destination[] route, int routeId)
        {
            return true;
        }
             
		public bool UnloadRoute(ulong messageId, int routeId)
		{
            var msg = Factory<String>((ulong)messageId, this, "SP,0,trip,purge,{0},{1}", routeId, "x");
            SendMessages(msg, messageId, false);
			return true;
		}

        public bool UnloadStop(ulong messageId, Destination[] route)
        {
            return true;
        }
        #endregion
    }
}