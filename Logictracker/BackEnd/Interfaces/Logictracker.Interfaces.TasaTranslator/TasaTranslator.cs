#region Usings

using System;
using System.Globalization;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Layers.MessageQueue;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Model.IAgent;

#endregion

namespace Logictracker.TasaTranslator
{
	[FrameworkElement(XName = "TasaTranslator", IsContainer = false)]
	public class TasaTranslator : FrameworkElement
		,IMessageHandler<Event>
		,IMessageHandler<Position>
		,IMessageHandler<ConfigRequest>
		,IMessageHandler<HardwareStatus>
		,IMessageHandler<UserMessage>
	{
		#region Attributes

		[ElementAttribute(XName = "MessageQueue", IsSmartProperty = true, IsRequired = true)]
		public IMessageQueue MessageQueue
		{
			get { return (IMessageQueue)GetValue("MessageQueue"); }
			set { SetValue("MessageQueue", value); }
		}
		
		#endregion

		#region IMessageHandler<Event>

		public HandleResults HandleMessage(Event message)
		{
			STrace.Debug(GetType().FullName, message.DeviceId, "HANDLE --> <Event>");

			var code = message.Code;
            if (code == Event.GenericMessage) code = (short) message.GetData();
            Enqueue(String.Format("MI,{0},{1},", message.DeviceId, code));
            return HandleResults.BreakSuccess;
		}

		#endregion

		#region IMessageHandler<Position>

		public HandleResults HandleMessage(Position message)
		{
			if (message == null || message.GeoPoints.Count < 1)
			{
				STrace.Error(GetType().FullName, message.GetDeviceId(), "Position: null or empty");
				return HandleResults.BreakSuccess;
			}

			const char code = 'Q'; // FueSolicitada? 'Q' : 'P';
			foreach (var p in message.GeoPoints)
			{
				STrace.Debug(GetType().FullName, message.DeviceId, "HANDLE --> <Position>");
				Enqueue(
					String.Format("{6},{0},{1},{2},{3},{4},{5},", 
						message.DeviceId, 
						p.Date.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture), //sacar el formato del config.
						p.Lat.ToString(CultureInfo.InvariantCulture), 
						p.Lon.ToString(CultureInfo.InvariantCulture), 
						p.Speed, 
						p.Course, 
						code
					));
			}
			return HandleResults.BreakSuccess;
		}

		#endregion

		#region IMessageHandler<ConfigRequest>

		public HandleResults HandleMessage(ConfigRequest message)
		{
			STrace.Debug(GetType().FullName, message.DeviceId, "HANDLE --> <ConfigRequest>");
			Enqueue(String.Format("LOG,{0},", message.DeviceId));
            return HandleResults.BreakSuccess;
		}

		#endregion

        #region IMessageHandler<HardwareStatus>

		public HandleResults HandleMessage(HardwareStatus message)
		{
			STrace.Debug(GetType().FullName, message.DeviceId, "HANDLE --> <HardwareStatus>");
			
			Enqueue(String.Format("HS,{0},{1},", message.DeviceId, message.Datos));
            return HandleResults.BreakSuccess;
		}

		#endregion

		#region IMessageHandler<UserMessage>

		public HandleResults HandleMessage(UserMessage message)
		{
			if (!message.HasUserSetting("user_message_code"))
            {
				STrace.Debug(GetType().FullName, message.DeviceId, "HANDLE --> <UserMessage> with no code");
				return HandleResults.Success;
            }

			var code = message.GetUserSetting("user_message_code");
            var trackingId = Convert.ToInt32(message.GetUserSetting("trackingId"));

			if (code == "ACK" || code == "NACK")
            {
				//limpio el UserMessage
				message.UserSettings.Remove("user_message_code");
				if (String.IsNullOrEmpty(message.GetUserSetting("trackingExtraData"))) message.UserSettings.Remove("trackingExtraData");

				//armo el String
				var s = String.Format("{0},{1},{2},", code, trackingId, message.DeviceId);
				s = message.UserSettings.Keys.Aggregate(s, (current, key) => String.Format("{0}{1}:{2},", current, key, message.UserSettings[key]));
				STrace.Debug(GetType().FullName, message.DeviceId, String.Format("HANDLE --> <UserMessage> msg: {0}", s));
				Enqueue(s);
                return HandleResults.BreakSuccess;
            }
 
			switch (code)
			{
				case "KEEPALIVE":
					Enqueue(String.Format("KA,{0},{1},{2},{3},{4},{5},", message.DeviceId,
						/*EstadoGPS ? "1" :*/ "0",
						/*ConBateriaPrincipal ? "1" :*/ "0",
						/*AudioOk ? "1" :*/ "0",
						/*EnPanico ? "1" :*/ "0",
						/*UltimoPanico*/ DateTime.MinValue.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
					break;
			} 
            return HandleResults.BreakSuccess;
		}

		#endregion

		#region Private Members

		private void Enqueue(String text)
        {
			MessageQueue.Send(text, text.Length > 100 ? text.Substring(0, 100) : text);
        }

		#endregion
    }

	[FrameworkElement(XName = "TasaTranslatorOnline", IsContainer = false)]
	public class TasaTranslatorOnline : FrameworkElement, IService
	{
		#region Attributes

		[ElementAttribute(XName = "MessageQueue", IsSmartProperty = true, IsRequired = true)]
		public IMessageQueue MessageQueue
		{
			get { return (IMessageQueue)GetValue("MessageQueue"); }
			set { SetValue("MessageQueue", value); }
		}

		#endregion

		#region IService

		public bool ServiceStart()
		{
			MessageQueue.SendStOnline();
			return true;
		}

		public bool ServiceStop()
		{
			return true;
		}

		#endregion
	}
}
