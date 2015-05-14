#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;

#endregion

namespace Logictracker.Layers.CommanderReader
{
    public static class DevicesCommandStatus
    {
		#region Properties

        private static readonly Dictionary<int, List<DeviceCommand>> DeviceCommands = new Dictionary<int, List<DeviceCommand>>();

		#endregion

		#region Public Methods

		public static List<DeviceCommand> GetDeviceList(int deviceId)
		{
            lock (DeviceCommands)
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
                        STrace.Log(typeof (DevicesCommandStatus).FullName, e, deviceId, LogTypes.Debug, null, e.Message);
                    }
                    return list;
                }
            }
		    return DeviceCommands[deviceId];
		}

		public static void CreateCommand(INode node, IDataTransportLayer disp, ulong messageId, int gatewayMessageId, int timeout, Action action, String trackingExtraData, CommanderReader commanderReader)
		{
			var command = new DeviceCommand(messageId, gatewayMessageId, node, disp, timeout, trackingExtraData);
			GetDeviceList(node.GetDeviceId()).Add(command);
			try
			{
				if (node == null) throw new ArgumentNullException("node");

				node.ExecuteOnGuard(action.Invoke, "CreateCommand", "");
				command.Begin();
			}
			catch
			{
				command.Rollback();
				GetDeviceList(node.Id).Remove(command);
				throw;
			}
		}

		public static void RemoveCommand(int deviceId, ulong trackingId)
		{
			var cmd = GetDeviceList(deviceId).FirstOrDefault(a => a.MessageId == trackingId);
			if (cmd == null) return;
			cmd.Rollback();
			GetDeviceList(deviceId).Remove(cmd);
		}

		#endregion
    }
}
