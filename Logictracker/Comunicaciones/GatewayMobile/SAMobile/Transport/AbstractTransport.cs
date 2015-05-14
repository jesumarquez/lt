using System;
using System.Collections.Generic;
using Urbetrack.Mobile.Comm.Messages;
using Urbetrack.Mobile.Comm.Transaccional;

namespace Urbetrack.Mobile.Comm.Transport
{
    public abstract class AbstractTransport
    {
        private readonly Dictionary<string, Transaction> trmap = new Dictionary<string,Transaction>();

        public abstract bool Send(PDU pdu);

        #region Mapa de Transacciones
        public Transaction ObtenerTransaccion(string trId)
        {
            if (trmap.ContainsKey(trId))
            {
                return trmap[trId];
            }
            return null;
        }

        public void TransaccionCompleta(Transaction t, PDU pdu)
        {
            if (trmap.ContainsKey(pdu.TransactionId))
            {
                if (pdu.Outgoing)
                {
                    pdu.Destination.LiberarSecuencia(t.TransactionUser, pdu.Seq);
                }
                trmap.Remove(pdu.TransactionId);
            }
        }

        public void NuevaTransaccion(Transaction t, PDU pdu)
        {
            if (pdu.Outgoing)
            {
                pdu.Seq = pdu.Destination.TomarSecuencia(t.TransactionUser);
            }
            trmap.Add(pdu.TransactionId, t);
        }

        public void Clear()
        {
            foreach(var t in trmap.Values)
            {
                if (t.GetType() == typeof(MRC))
                {
                    var mrc = t as MRC;
                    if (mrc == null) throw new Exception("C# no se pone de acuerdo con los tipos (MRC)");
                    t.TransactionUser.SolicitudNoEnviada(mrc.pdu, t, this);
                    mrc.pdu.Destination.LiberarSecuencia(t.TransactionUser, mrc.pdu.Seq);
                }
            }
            trmap.Clear();
        }
        #endregion

        public int TimerTR { get; set; }

        public int TimerTV { get; set; }

        public int TimerTRM { get; set; }

        public bool RequiereRetransmicion { get; set; }

        public bool HACK_DisableSocketRead { get; set; }

        public bool HACK_DisableSocketWrite { get; set; }

        public TransactionUser TransactionUser { get; set; }

        public TransactionUser Managment { get; set; }

        #region Propiedades Privadas

        #endregion

        protected AbstractTransport()
        {
            TimerTRM = 4000;
            TimerTV = 12500;
            TimerTR = 1000;
        }
    }
}