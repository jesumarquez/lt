#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.MsmqMessaging.Interfaces;
using Logictracker.MsmqMessaging.Opaque;
using Logictracker.Utils;

#endregion

namespace Logictracker.MsmqMessaging.Batch
{
    public class QueueBatchConsumer : Task
    {
        #region Result enum

        public enum Result
        {
            SourceEmpty,
            SourceBackupFailure,
            DestinationLocked,
            DestinationFailure,
            Dispatched,
            Deferred
        } ;

        #endregion

        private const int confort_sleep_time = 1000;

        private readonly ISyncQueueDispatcher dst;
        private readonly OpaqueMessageQueue srcq;

        public QueueBatchConsumer(string source, ISyncQueueDispatcher disp, String readLabelReformatExpression)
            : base(String.Format("QueueBatchConsumer[{0}]",source))
        {
            srcq = new OpaqueMessageQueue(readLabelReformatExpression) {Name = source};
            dst = disp;
        }

        public int DispatchedMessages { get; protected set; }

        public static Result Dispatch(OpaqueMessageQueue Source, ISyncQueueDispatcher Destination)
        {
            var msg = Source.Peek();
            if (msg == null) return Result.SourceEmpty;
            if (!Destination.CanPush) return Result.DestinationLocked;
            if (!Destination.BeginPush(msg)) return Result.DestinationFailure;
            if (Destination.WaitCompleted(msg))
            {
                Source.Pop(msg.Id, false);
                return Result.Dispatched;
            }
            return Result.DestinationFailure;
        }

        internal class DispatchState
        {
            internal long LookupId;
            internal OpaqueMessageQueue Source;
        }

        public static Result Dispatch(OpaqueMessageQueue Source, IAsyncQueueDispatcher Destination)
        {
            var msg = Source.WindowPeek(0);
            if (msg == null) return Result.SourceEmpty;
            if (!Destination.CanPush) return Result.DestinationLocked;
            var state = new DispatchState {LookupId = msg.Id, Source = Source};
            if (!Destination.BeginPush(msg, AsyncDispatched, state)) return Result.DestinationFailure;
            return Result.Deferred;
        }

        private static void AsyncDispatched(IAsyncResult ar)
        {
            var state = (DispatchState) ar.AsyncState;
            if (ar.IsCompleted) state.Source.Pop(state.LookupId, true);
        }

        protected override int DoWork(ulong ticks)
        {
            while (true)
            {
                try
                {
                    switch(Dispatch(srcq, dst))
                    {
                        case Result.Deferred:
                            continue;
                        case Result.Dispatched:
                            DispatchedMessages++;
                            continue;
                        default:
                            return confort_sleep_time;
                    }

                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName, e, "QueueBatchConsumer.DoWork");
                    return confort_sleep_time;
                }
            }
        }
    }
}