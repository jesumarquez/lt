#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Model;

#endregion

namespace Urbetrack.SessionBorder
{
    public static class DevicesCommandStatus
    {
		#region Properties

        private static readonly Dictionary<int, List<DeviceCommand>> DeviceCommands = new Dictionary<int, List<DeviceCommand>>();

		#endregion

		#region Public Methods

		public static List<DeviceCommand> GetDeviceList(int deviceId)
		{
			if (!DeviceCommands.ContainsKey(deviceId))
			{
				var list = new List<DeviceCommand>();
				try
				{
					DeviceCommands.Add(deviceId, list);
				}
				catch (Exception e)
				{
					STrace.Debug(typeof(DevicesCommandStatus).FullName, e.ToString());
				}
				return list;
			}
			return DeviceCommands[deviceId];
		}

		public static void CreateCommand(INode node, IDispatcherLayer disp, int trackingId, int gatewayMessageId, int timeout, Action action, String trackingExtraData, CommanderReader commanderReader)
		{
			var command = new DeviceCommand(trackingId, gatewayMessageId, node.DeviceId, disp, timeout, trackingExtraData, commanderReader);
			GetDeviceList(node.DeviceId).Add(command);
			try
			{
				action.Invoke();
				command.Begin();
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(DevicesCommandStatus).FullName, e);
				command.Rollback();
				GetDeviceList(node.DeviceId).Remove(command);
			}
		}

		public static void RemoveCommand(int deviceId, int trackingId)
		{
			var cmd = (from a in GetDeviceList(deviceId)
					   where a.trackingId == trackingId
					   select a).FirstOrDefault();
			if (cmd == null) return;
			cmd.Rollback();
			GetDeviceList(deviceId).Remove(cmd);
		}

		#endregion
    }
}
