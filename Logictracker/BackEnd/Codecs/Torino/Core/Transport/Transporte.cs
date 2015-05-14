#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    [Serializable]
    public abstract class Transporte
    {
        private readonly Dictionary<string, Transaccion> trmap = new Dictionary<string,Transaccion>();
        private readonly object trmap_lock = new object();
        
        public abstract bool Send(PDU pdu);

        public abstract void Close(Destino dst);

        public virtual bool UseSeqCache { get { return false; } }

        public virtual int SeqCache { get; set; }

        public virtual bool DeviceGoesONNET { get { return false; } }

        public virtual int MTU { get { return 512; } }

        #region Mapa de Transacciones
        public Transaccion ObtenerTransaccion(string trId)
        {
            lock (trmap_lock)
            {
                return trmap.ContainsKey(trId) ? trmap[trId] : null;
            }
        }

        public void TransaccionCompleta(Transaccion t, PDU pdu)
        {
            lock (trmap_lock) {
                if (trmap.ContainsKey(pdu.IdTransaccion))
                {
                    if (pdu.Saliente)
                    {
                        pdu.Destino.LiberarSecuencia(t.TransactionUser, pdu.Seq);
                    }
                    trmap.Remove(pdu.IdTransaccion);
                }
            }
        }

        public void NuevaTransaccion(Transaccion t, PDU pdu)
        {
            lock (trmap_lock)
            {
                if (pdu.Saliente)
                {
                    t.Seq = pdu.Seq = pdu.Destino.TomarSecuencia(t.TransactionUser);
                }
                try
                {
                    trmap.Add(pdu.IdTransaccion, t);
                }
                catch (IndexOutOfRangeException e)
                {
                    STrace.Exception(GetType().FullName,e);
                    throw;
                }
            }
        }
        #endregion

        public int TimerTR { get; set; }

        public int TimerTV { get; set; }

        public int TimerTRM { get; set; }

        public enum ConnectionModes
        {
            ConnectionOriented,
            DatagramOriented,
            PassiveDatagramOriented
        } ;

        public ConnectionModes ConnectionMode { get; protected set; }

        public bool PassiveMode { get; protected set; }

        public bool HACK_DisableSocketRead { get; set; }

        public bool HACK_DisableSocketWrite { get; set; }

        public CODEC CODEC { get; set; }

        public TransactionUser TransactionUser { get; set; }

        public TransactionUser Managment { get; set; }

        protected Transporte()
        {
            TimerTR = 2000;
            TimerTV = 32000;
            TimerTRM = 8000;
        }
    }
}