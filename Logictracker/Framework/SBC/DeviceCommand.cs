#region Usings

using System;
using System.Diagnostics;
using System.Threading;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Model;

#endregion

namespace Urbetrack.SessionBorder
{
	public class DeviceCommand
    {
		#region Properties

		public readonly String TrackingExtraData;
		public readonly int NodeCode;
		public readonly CommanderReader CommanderReader;
		public IDispatcherLayer ResponseDispatcher { get; set; }
		public int SecondsTimeout { get; set; }
		public int trackingId { get; set; }
		public int gatewayMessageId { get; set; }
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

		public DeviceCommand(int trackingId, int gatewayMessageId, int NodeCode, IDispatcherLayer ResponseDispatcher, int SecondsTimeout, String TrackingExtraData, CommanderReader CommanderReader)
		{
			State = CommandStates.JustWaitingForBegin;
			this.TrackingExtraData = TrackingExtraData;
			this.NodeCode = NodeCode;
			this.CommanderReader = CommanderReader;
			this.trackingId = trackingId;
			this.gatewayMessageId = gatewayMessageId;
			this.SecondsTimeout = SecondsTimeout;
			this.ResponseDispatcher = ResponseDispatcher;

			Timer = new Timer(OnTimeout, this, Timeout.Infinite, Timeout.Infinite);
		}

		public void Begin()
		{
			if (State != CommandStates.JustWaitingForBegin)
			{
				STrace.Debug(typeof(DeviceCommand).FullName, String.Format("CMD: {0} {1} {2} Node[{3:D6}] No se inicia por estado invalido.", State, trackingId, SecondsTimeout, NodeCode));
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
					CommanderReader.DispatchNack(NodeCode, trackingId, TrackingExtraData, ResponseDispatcher);

				STrace.Debug(typeof(DeviceCommand).FullName, String.Format("CMD: {0} {1} {2} Node[{3:D6}] Cancel No se inicia por estado invalido.", State, trackingId, SecondsTimeout, NodeCode));
				return;
			}

			State = CommandStates.TryingCancel;
			var st = CommanderReader.DataProvider.Get(NodeCode) as ICanQueueOutgoingMessages;
			if (st != null)
			{
				st.CancelMessage(trackingId);
			}
			CommanderReader.DispatchNack(NodeCode, trackingId, TrackingExtraData, ResponseDispatcher);

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

		private static void OnTimeout(object state)
		{
			var cmd = state as DeviceCommand;

			Debug.Assert(cmd != null);

			var st = cmd.CommanderReader.DataProvider.Get(cmd.NodeCode) as ICanQueueOutgoingMessages;
			if (st != null)
			{
				st.CancelMessage(cmd.trackingId);
			}

			if (cmd.State == CommandStates.TryingCancel)
			{
				cmd.State = CommandStates.Timewait;
				return;
			}

			if (cmd.State != CommandStates.Trying)
			{
				STrace.Debug(typeof(DeviceCommand).FullName, cmd + "Comando ignora timeout");
				return;
			}

			cmd.Cancel();
		}

		#endregion
	}

}
