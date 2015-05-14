#region Usings

using System;
using System.Globalization;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;

#endregion

namespace Logictracker.Unetel.v2.UnexLibs
{
	[FrameworkElement(XName = "DatabaseUpdateHook", IsContainer = false)]
	public class DatabaseUpdateHook : FrameworkElement, IMessageHook
	{
		[ElementAttribute(XName = "DataProvider", IsSmartProperty = true, IsRequired = true)]
		public IDataProvider DataProvider
		{
			get { return (IDataProvider)GetValue("DataProvider"); }
			set { SetValue("DataProvider", value); }
		}

		public void HookUpMessage(IMessage message, INode parser)
		{
			if (!(message is ConfigRequest)) return;
			var cfm = message as ConfigRequest;
			var config = 0;
			var messages = 0;
			var firmware = "undefined";

			if (cfm.IntegerParameters.ContainsKey(Indication.indicatedConfigurationRevision)) config = cfm.IntegerParameters[Indication.indicatedConfigurationRevision];
			if (cfm.IntegerParameters.ContainsKey(Indication.indicatedCannedMessagesTableRevision)) messages = cfm.IntegerParameters[Indication.indicatedCannedMessagesTableRevision];
			if (cfm.StringParameters.ContainsKey(Indication.indicatedFirmwareSignature)) firmware = cfm.StringParameters[Indication.indicatedFirmwareSignature];
			try 
			{
				STrace.Debug(GetType().FullName, message.DeviceId, String.Format("DBUPDATE: {0}/{1}/{2}/{3}", message.DeviceId, config, messages, firmware));
				UpdateNodeData(DataProvider, message.DeviceId, config, messages, firmware);
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e);
			}
		}

		public static void UpdateNodeData(IDataProvider dataProvider, int deviceId, int config, int messages, String firmware)
		{
			dataProvider.SetDetalleDispositivo(deviceId, "known_config_revision", config.ToString(CultureInfo.InvariantCulture), "int");
			dataProvider.SetDetalleDispositivo(deviceId, "known_messages_revision", messages.ToString(CultureInfo.InvariantCulture), "int");
			dataProvider.SetDetalleDispositivo(deviceId, "known_firmware_signature", firmware, "string");
		}
	}
}