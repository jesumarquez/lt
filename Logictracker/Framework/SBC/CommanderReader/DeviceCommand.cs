#region Usings

using System;
using System.Diagnostics;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Model;

#endregion

namespace Logictracker.Layers.CommanderReader
{
	public class DeviceCommand
    {
		#region Properties

		public readonly String TrackingExtraData;
		public readonly INode Node;
		public IDataTransportLayer ResponseDispatcher { get; set; }
		public int SecondsTimeout { get; set; }
		public ulong MessageId { get; set; }
		public int GatewayMessageId { get; set; }
		public Timer Timer { get; private set; }
		private CommandStates State;
		
		private enum CommandStates
		{
			JustWaitingForBegin,
			Trying,
			TryingCancel,
			Rollback,
			Timewait
		}

		#endregion

		#region Public Methods

		public DeviceCommand(ulong messageId, int gatewayMessageId, INode device, IDataTransportLayer responseDispatcher, int SecondsTimeout, String TrackingExtraData)
		{
			State = CommandStates.JustWaitingForBegin;
			this.TrackingExtraData = TrackingExtraData;
			Node = device;
			this.MessageId = messageId;
			this.GatewayMessageId = gatewayMessageId;
			this.SecondsTimeout = SecondsTimeout;
			this.ResponseDispatcher = responseDispatcher;

			Timer = new Timer(OnTimeout, this, Timeout.Infinite, Timeout.Infinite);
		}

		public void Begin()
		{
			if (State != CommandStates.JustWaitingForBegin)
			{
				STrace.Debug(typeof(DeviceCommand).FullName, Node.Id, String.Format("CMD: State={0} MessageId={1} SecondsTimeout={2} No se inicia por estado invalido.", State, MessageId, SecondsTimeout));
				return;
			}
			State = CommandStates.Trying;
			Timer.Change(SecondsTimeout * 1000, Timeout.Infinite);
		}

		public void Cancel()
		{
			if (State != CommandStates.Trying)
			{
				if (State != CommandStates.Rollback)
					CommanderReader.DispatchNack(Node, MessageId, TrackingExtraData, ResponseDispatcher);

				STrace.Debug(typeof(DeviceCommand).FullName, Node.Id, String.Format("CMD: State={0} MessageId={1} SecondsTimeout={2} Cancel No se inicia por estado invalido.", State, MessageId, SecondsTimeout));
				return;
			}

			State = CommandStates.TryingCancel;
			CommanderReader.DispatchNack(Node, MessageId, TrackingExtraData, ResponseDispatcher);

			if (Timer == null) return;
			Timer.Change(10000, Timeout.Infinite);
		}

		public void Rollback()
		{
			State = CommandStates.Rollback;
			if (Timer == null) return;
			Timer.Dispose();
			Timer = null;
		}
		
		#endregion

		#region Private Methods

		private static void OnTimeout(Object state)
		{
			var cmd = state as DeviceCommand;

			Debug.Assert(cmd != null);

			if (cmd.State == CommandStates.TryingCancel)
			{
				cmd.State = CommandStates.Timewait;
				return;
			}

			if (cmd.State != CommandStates.Trying)
			{
				STrace.Debug(typeof(DeviceCommand).FullName, cmd.Node.Id, String.Format("{0} - Comando ignora timeout", cmd));
				return;
			}

			cmd.Cancel();
		}

		#endregion
	}

}
