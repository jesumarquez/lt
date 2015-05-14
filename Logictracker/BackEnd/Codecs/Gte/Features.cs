using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Logictracker.Cache;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Model;

namespace Logictracker.Trax.v1
{
	public partial class Parser : IPowerBoot, IShortMessage, IRoutable, IPicture, ITemperature, IWorkflow
	{
        public Boolean? IsGarminConnected
        {
            get { return _isGarminConnected; }
            set
            {
                string md = DataProvider.GetDetalleDispositivo(Id, "GTE_MESSAGING_DEVICE").As(String.Empty);

                var key = "device_" + Id + "_GarminConnected";


                // deletes from cache if null value comes
                if (value == null)
                {
                    LogicCache.Delete(key);
                }

                if (md != MessagingDevice.Garmin)
                {
                    LogicCache.Store(key, "GARMINCONNECTED_NOTCONFIGURED");
                }
                else
                {
                    LogicCache.Store(key,
                                    (value == null
                                         ? "GARMINCONNECTED_UNKNOWN"
                                         : value.Value ? "GARMINCONNECTED_TRUE" : "GARMINCONNECTED_FALSE"));
                }

                STrace.Debug(typeof(Mensaje).FullName, Id, string.Format("Setting isGarminTurnedOn: {0}", value));

                if (value != null && value.Value && md == MessagingDevice.Garmin && (IsGarminConnected == null || !IsGarminConnected.Value) && DateTime.UtcNow.Subtract(_garminSetupLastTimeSent) > TimeSpan.FromHours(6))
                {
                    GarminSetup(this);
                    STrace.Debug(typeof(IFoteable).FullName, Id, "Sending GarminSetup");
                    _garminSetupLastTimeSent = DateTime.UtcNow;
                }

                _isGarminConnected = value;
            }
        }        

		#region IPowerBoot

		public bool Reboot(ulong messageId)
		{
			SendMessages(Mensaje.Factory("SSR55AA"), messageId, null);
			return true;
		}

		#endregion

        #region IShortMessage

        public bool SetCannedMessage(ulong messageId, int codeMessageId, String message, int revision)
		{
			var md = GetMessagingDevice();
			switch (md)
			{
				case MessagingDevice.Garmin:
			        var cmd = GarminFmi.EncodeSetCanned(codeMessageId, message, FmiPacketId.ScSetCannedMessage).ToTraxFM(this);
                    SendMessages(new [] { cmd }, md);
					break;
				case MessagingDevice.Mpx01:
					break;
			}
			return true;
		}

		public bool SetCannedResponse(ulong messageId, int codeResponseId, String response, int revision)
		{
			var md = GetMessagingDevice();
			switch (md)
			{
				case MessagingDevice.Garmin:
			        var cmd = GarminFmi.EncodeSetCanned(codeResponseId, response, FmiPacketId.ScSetCannedResponse).ToTraxFM(this);
                    SendMessages(new[] { cmd }, md);
					break;
				case MessagingDevice.Mpx01:
					break;
			}
			return true;
		}


		public bool SubmitCannedMessage(ulong messageId, int codeMessageId, int[] replies)
		{

			//var messageM = daoFactory.MensajeDAO.GetCannedMessagesTable(DeviceId, 0).Where(m => m.Codigo == codeMessageId.ToString()).SingleOrDefault();
			var messageM = DataProvider.GetCannedMessagesTable(Id, 0).SingleOrDefault(m => m.Codigo == codeMessageId.ToString(CultureInfo.InvariantCulture));
			if (messageM == null)
			{
				STrace.Error(typeof(Parser).FullName, Id, String.Format("Se intento enviar un mensaje predefinido no existente con codigo={0}", codeMessageId));
				return false;
			}
			var message = messageM.Texto;

			var md = GetMessagingDevice();		    
			switch (md)
			{
                case MessagingDevice.Garmin:
			        {
			            //envio sin id por que si el id se repite el envio da error y no hay canned messages hacia garmin, solo hacia server, que el usuario se encargue de borrar los mensajes
                        /*(UInt32)codeMessageId*/
			            var cmd = GarminFmi.EncodeOpenTextMessage(0, message, true).ToTraxFM(this);
                        SendMessages(new[] { cmd }, md);
			            break;
			        }
                case MessagingDevice.Mpx01:
			        {
			            var cmd = EncodeMpx01Msg(message, messageId, this);
			            SendMessages(cmd, messageId, md);
			            break;
			        }
			    case MessagingDevice.Sms:
			        {
                        var cmd = EncodeSmsMsg(message, messageId, this);
                        SendMessages(cmd, messageId, md);
			        }					
					break;
			}            
			return true;
		}

        public bool SubmitTextMessage(ulong messageId, String textMessage, int[] replies)
        {
            return SubmitTextMessage(messageId, 0, textMessage, replies.Select(b=> (UInt32) b).ToArray());
        }

        public bool SubmitTextMessage(ulong messageId, uint textMessageId, String textMessage, uint[] replies)
        {
            return SubmitTextMessage(messageId, textMessageId, textMessage, replies, -1);
        }

		public bool SubmitTextMessage(ulong messageId, uint textMessageId, String textMessage, uint[] replies, int ackEvent)
		{
            STrace.Debug(typeof(Parser).FullName, Id, "SubmitTextMessage: messageId=" + messageId.ToString() + ",textMessageId=" + textMessageId.ToString() + ",textMessage=" + textMessage + ",replies=" + (replies==null?"null":String.Join(",",replies.Select(b=>b.ToString()).ToArray())));
			var md = GetMessagingDevice();
			switch (md)
			{
                case MessagingDevice.Garmin:
			        {
			            var cnf = new StringBuilder();
//			            cnf.Append(GarminFmi.EncodeDeleteTextmessage(textMessageId).ToTraxFM(this, false));
                        if (replies != null && replies.Length > 0)
                            cnf.Append(GarminFmi.EncodeCannedResponseList(textMessageId, replies).ToTraxFM(this, false));
                        else
                            cnf.Append(GarminFmi.EncodeDeleteTextmessage(textMessageId).ToTraxFM(this, false));

                        cnf.Append(GarminFmi.EncodeOpenTextMessage(textMessageId, textMessage, true).ToTraxFM(this, false));

                        if (ackEvent != -1)
                        {
                            cnf.Append(Fota.VirtualMessageFactory((MessageIdentifier) ackEvent, NextSequence));
                        }

			            var cmd = cnf.ToString();
			            SendMessages(cmd, messageId, md);
			            break;
			        }
			    case MessagingDevice.Mpx01:
			        {
			            var cmd = EncodeMpx01Msg(textMessage, messageId, this);
                        SendMessages(cmd, messageId, md);
			            break;
			        }
                case MessagingDevice.Sms:
			        {
			            var cmd = EncodeSmsMsg(textMessage, messageId, this);
                        SendMessages(cmd, messageId, md);
			            break;
			        }
			}
			return true;
		}

		public bool DeleteCannedMessage(ulong messageId, int code, int revision)
		{
            var config = new StringBuilder();

			var md = GetMessagingDevice();
		    var cmd = "";
            if (code == 0)
            {                
                var messagesParameters = DataProvider.GetCannedMessagesTable(Id, 0).OrderBy(m => m.Codigo);
                foreach (var mensaje in messagesParameters)
                {
                    switch (md)
                    {
                        case MessagingDevice.Garmin:
                            config.Append(GarminFmi.EncodeDeleteCanned(Convert.ToInt32(mensaje.Codigo), FmiPacketId.ScDeleteCannedMessage).ToTraxFM(this, false));
                            break;
                        case MessagingDevice.Mpx01:
                            break;
                    }
                }
                config.Append(Environment.NewLine);
                //                config.AppendFormat("{0}{1}", GarminFmi.EncodeDataDeletionFor(DataDeletionProtocolId.DeleteAllCannedMessagesOnTheClient));                
            }
            else
                switch (md)
                {
                    case MessagingDevice.Garmin:
                        config.Append(GarminFmi.EncodeDeleteCanned(code, FmiPacketId.ScDeleteCannedMessage).ToTraxFM(this, true));
                        break;
                    case MessagingDevice.Mpx01:
                        break;
                }

            cmd = config.ToString();
            SendMessages(cmd, messageId, md);
			return true;
		}

		internal static void EnsureMessagingDeviceIsGarmin(Parser dev)
		{
			//controlo que este bien seteado el Detalle GTE_MESSAGING_DEVICE
			//var md_g = dev.GetMessagingDevice();
			//if (md_g != MessagingDevice.Garmin) dev.DataProvider.SetDetalleDispositivo(dev.Id, "GTE_MESSAGING_DEVICE", MessagingDevice.Garmin, "string");

		}

		#endregion

		#region IRoutable

		public bool LoadRoute(ulong messageId, Destination[] route, bool sort, int routeId)
		{
		    string cmd = null;
			var md = GetMessagingDevice();
			switch (md)
			{
				case MessagingDevice.Garmin:
					var config = new StringBuilder();
			        config.AppendFormat("{0}{1}", BaseDeviceCommand.createFrom(">SSG161<", this, null).ToString(false), Environment.NewLine);
			        config.Append(GarminFmi.EncodeDataDeletionFor(DataDeletionProtocolId.DeleteAllStopsOnTheClient).ToTraxFM(this, false));
                    foreach (var p in route)
                    {                        
                        var stopStr = GarminFmi.EncodeA603StopProtocol(p.Point, p.Code, p.Text).ToTraxFM(this, false);
                        STrace.Debug(typeof(GarminFmi).FullName, Id, "LoadRoute: Stop ID=" + Convert.ToString(p.Code) + " - " + stopStr);
                        config.Append(stopStr);
                    }
			        config.Append(Environment.NewLine);
					config.Append(Fota.VirtualMessageFactory(MessageIdentifier.LoadRouteSuccess, NextSequence));
			        cmd = config.ToString();
					break;
				case MessagingDevice.Mpx01:
					break;
			}
            SendMessages(cmd, messageId, md);
			return true;
		}

        public bool ReloadRoute(ulong messageId, Destination[] route, bool sort, int routeId)
        {
            string cmd = null;
            var md = GetMessagingDevice();
            switch (md)
            {
                case MessagingDevice.Garmin:
                    var config = new StringBuilder();
                    config.AppendFormat("{0}{1}", BaseDeviceCommand.createFrom(">SSG161<", this, null).ToString(false), Environment.NewLine);
                    //config.Append(GarminFmi.EncodeDataDeletionFor(DataDeletionProtocolId.DeleteAllStopsOnTheClient).ToTraxFM(this, false));
                    foreach (var p in route)
                    {
                        var stopStr = GarminFmi.EncodeA603StopProtocol(p.Point, p.Code, p.Text).ToTraxFM(this, false);
                        //STrace.Debug(typeof(GarminFmi).FullName, Id, "ReloadRoute: Stop ID=" + Convert.ToString(p.Code) + " - " + stopStr);
                        config.Append(stopStr);
                    }
                    config.Append(Environment.NewLine);
                    config.Append(Fota.VirtualMessageFactory(MessageIdentifier.ReloadRouteSuccess, NextSequence));
                    cmd = config.ToString();
                    break;
                case MessagingDevice.Mpx01:
                    break;
            }
            SendMessages(cmd, messageId, md);
            return true;
        }

        
        public bool UnloadStop(ulong messageId, Destination[] route)
        {
            string cmd = null;
            var md = GetMessagingDevice();
            switch (md)
            {
                case MessagingDevice.Garmin:
                    var config = new StringBuilder();
                    foreach (var p in route)
                    {
                        config.Append(GarminFmi.EncodeDeleteTextmessage((UInt32)p.Code).ToTraxFM(this, false));

                        var stopStr =
                            GarminFmi.EncodeA603StopStatusProtocol(p.Code, StopStatusValue.DeleteStop).ToTraxFM(this,
                                                                                                                false);
                        STrace.Debug(typeof(GarminFmi).FullName, Id, "UnloadStop: Stop ID=" + Convert.ToString(p.Code) + " - " + stopStr);
                        config.Append(stopStr);
                    }
                    config.Append(Environment.NewLine);
                    cmd = config.ToString();
                    break;
                case MessagingDevice.Mpx01:
                    break;
            }
            SendMessages(cmd, messageId, md);
            return true;
        }

        public bool UnloadRoute(ulong messageId, int routeId)
        {
            return true;
        }

        public bool UnloadRoute(ulong messageId, Destination[] route, int routeId)
        {
            string cmd = null;
            var md = GetMessagingDevice();
            switch (md)
            {
                case MessagingDevice.Garmin:
                    var config = new StringBuilder();
                    config.AppendFormat("{0}{1}", BaseDeviceCommand.createFrom(">SSG160<", this, null).ToString(false), Environment.NewLine);
                    config.Append(GarminFmi.EncodeDataDeletionFor(DataDeletionProtocolId.DeleteAllStopsOnTheClient).ToTraxFM(this, false));
                    foreach (var p in route)
                    {
                        config.Append(GarminFmi.EncodeDeleteTextmessage((UInt32) p.Code).ToTraxFM(this, false));
                    }
                    config.Append(Environment.NewLine);                    
                    config.Append(Fota.VirtualMessageFactory(MessageIdentifier.UnloadRouteSuccess, 0));
                    cmd = config.ToString();
                    break;
                case MessagingDevice.Mpx01:
                    break;
            }

            SendMessages(cmd, messageId, md);
            return true;
		}

		#endregion

		#region IPicture

		public bool RetrievePictures(ulong messageId, string from, string to)
		{
			STrace.Debug(GetType().FullName, Id, String.Format("RetrievePictures: MessageId:{0}; from:{1}; to:{2}", messageId, from, to));
			SendMessages(Mensaje.Factory((ulong)messageId, this, "QSDI,P,I{0},E{1}", from, to), messageId, null);
			return true;
		}

		#endregion

		#region ITemperature

		public bool ReportTemperature(ulong messageId, int timerInterval)
		{
			var config = new StringBuilder();
			config.AppendLine(Mensaje.Factory(TemperatureActivateReports, 1));
			config.AppendLine(Mensaje.Factory("STD07{0:D6}000000", timerInterval));
			SendMessages(config.ToString(), messageId, null);
			return true;
		}

		public bool ReportTemperatureStop(ulong messageId)
		{
			var config = new StringBuilder();
			config.AppendLine(Mensaje.Factory(TemperatureActivateReports, 0));
			SendMessages(config.ToString(), messageId, null);
			return true;
		}

		private const String TemperatureActivateReports = "SSG01{0:D1}";

		#endregion

		#region IWorkflow

		public bool SetWorkflowState(ulong messageId, int state, WorkflowMessage[] messages)
		{
			var dev = GetMessagingDevice();
			if (dev != MessagingDevice.Mpx01) return false;
			var s = new StringBuilder(">SMTD**<" + Environment.NewLine); //Borra la cola de estados logisticos del dispositivo
			foreach (var m in messages)
			{
				var texto = m.Text;
				if (texto.Length > 120)
				{
					STrace.Trace(GetType().FullName, Id, String.Format("Estado logistico '{0}' se trunca el texto por ser demasiado largo: {1}", m.Code, texto));
					texto = texto.Substring(0, 120);
				}
				
				s.AppendLine(String.Format(">SMT N|{0:D5}|{1}<{2}", m.Code, texto, Environment.NewLine));
			}

			SendMessages(s.ToString(), messageId, null);
			return true;
		}

		#endregion
	}
}