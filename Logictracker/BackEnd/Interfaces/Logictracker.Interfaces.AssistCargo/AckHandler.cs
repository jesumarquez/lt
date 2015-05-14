using System;
using System.Web.Script.Serialization;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Dispatcher.Core;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

namespace Logictracker.Interfaces.AssistCargo
{
    [FrameworkElement(XName = "AssistCargoAckHandler", IsContainer = false)]
    public class AckHandler : BaseHandler<UserMessage>
	{
		#region Implementation of IMessageHandler<UserMessage>

	    protected override HandleResults OnHandleMessage(UserMessage message)
	    {
	        var a = new JavaScriptSerializer().Serialize(message.UserSettings);
			STrace.Debug(typeof(AckHandler).FullName, message.DeviceId, String.Format("AssistCargo. OnHandleMessage {0}: trackingId={1}", message.DeviceId, a));

			if (!message.HasUserSetting("user_message_code")) return HandleResults.Success;

            var code = message.UserSettings["user_message_code"];

			var trackingId = message.HasUserSetting("trackingId")
                                ? Convert.ToInt32(message.UserSettings["trackingId"])
                                : 0;

			var trackingExtraData = message.HasUserSetting("trackingExtraData")
                                        ? message.UserSettings["trackingExtraData"]
                                        : String.Empty;

            

            switch (code)
            {
                case "ACK": OnAck(trackingId, trackingExtraData, message); break;
                case "NACK": OnNack(trackingId, trackingExtraData, message); break;
            }
            return HandleResults.BreakSuccess;
	    }

	    #endregion

	    private static void OnAck(int trackingId, String trackingExtraData, UserMessage message)
        {
        	if (trackingExtraData != "AssistCargo") return;
			STrace.Debug(typeof(AckHandler).FullName, message.DeviceId, String.Format("AssistCargo. ComandoProcesado OK: trackingId={0}", trackingId));
        	var service = new Service();
        	service.ComandoProcesado(trackingId, true);
        }

	    private static void OnNack(int trackingId, String trackingExtraData, UserMessage message)
        {
        	if (trackingExtraData != "AssistCargo") return;
			STrace.Debug(typeof(AckHandler).FullName, message.DeviceId, String.Format("AssistCargo. ComandoProcesado FAIL: trackingId={0}", trackingId));
        	var service = new Service();
        	service.ComandoProcesado(trackingId, false);
        }
	}
}
