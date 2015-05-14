#region Usings

using System.Threading;

#endregion

namespace XbeeCore
{
    internal class XBeeTxTransaction
    {
        readonly XBeePDU pdu;
        readonly XBeeTrLayer layer;
        Timer lost_timer;
        private bool finished;
        public byte id;
        
        internal XBeeTxTransaction(XBeeTrLayer _layer, XBeePDU _pdu, int timeout)        
        {
            Stack._created_transactions++;
            layer = _layer;
            pdu = _pdu;
            id = pdu.Sequence;
            lost_timer = new Timer(TimedOut, null, timeout, Timeout.Infinite);
        }

        public XBeePDU Pdu
        {
            get { return pdu; }
        }

        public void End()
        {
            Finish();
            Stack._completed_transactions++;
        }

        public void Cancel()
        {
            Finish();
            Stack._nacks++;
        }

        private void Finish()
        {
            lock (this)
            {
                if (!finished)
                {
                    if (lost_timer == null) return;                    
                    lost_timer.Change(Timeout.Infinite, Timeout.Infinite);
                    lost_timer = null;
                    finished = true;
                }
            }
        }

        private void TimedOut(object sender)        
        {
            if (!finished)
            {
                lock (this)
                {
                    Stack._timedout_transactions++;
                    layer.Remove(id);
                    finished = true;
                    lost_timer = null;
                }                
            }
        }

    }
}
