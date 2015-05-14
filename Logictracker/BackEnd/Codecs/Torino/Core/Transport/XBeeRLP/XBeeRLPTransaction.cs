#region Usings

using System;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Toolkit;
using Urbetrack.Torino;

#endregion

namespace Urbetrack.Comm.Core.Transport.XBeeRLP
{
    public class XBeeRLPTransaction
    {
        public enum States
        {
            SENDING,
            COMPLETE,
            FINISHED
        }

        internal States State;

        internal byte[] buffer;
        private int tr_timer_id;
        private readonly int tv_timer_id;
        private readonly XBeeRadioLinkProtocol rlp;
        private readonly Device d;

        internal XBeeRLPTransaction(Device dev, byte[] data, XBeeRadioLinkProtocol _rlp)
        {
            State = States.SENDING;
            buffer = data;
            d = dev;
            rlp = _rlp;
            tr_timer_id = Scheduler.Scheduler.AddTimer(Timeout, 5000);
            tv_timer_id = Scheduler.Scheduler.AddTimer(Timeout, 60000);
        }

        internal void Remove()
        {
            State = States.COMPLETE;
            Scheduler.Scheduler.DelTimer(tr_timer_id);
        }

        public bool Answered()
        {
            return State != States.SENDING;
        }

        void Timeout(int timer_id)
        {
            if (timer_id == tr_timer_id && State == States.SENDING)
            {
                STrace.Debug(GetType().FullName, String.Format("XBEE RLP: Reenviando {0}.",
                        buffer[0] == (byte) XBeeRadioLinkProtocol.FrameType.ENABLE_LINK ? "ENABLE LINK" : "DISABLE LINK"));
                rlp.XBeeAPIPort.Send(buffer, d.Destino.XBee.Addr);
                tr_timer_id = Scheduler.Scheduler.AddTimer(Timeout, 5000);
            }

            if (timer_id != tv_timer_id) return;

            State = States.FINISHED;
            STrace.Debug(GetType().FullName,"XBEE RLP: Vencido.");
            if (tr_timer_id != 0) Scheduler.Scheduler.DelTimer(tr_timer_id);
            tr_timer_id = 0; // falso timer
            rlp.RemoveTransaction(d);
        }
    }
}