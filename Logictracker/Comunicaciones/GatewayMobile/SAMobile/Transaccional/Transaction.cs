using System;
using System.Threading;
using Urbetrack.Mobile.Comm.Messages;
using Urbetrack.Mobile.Comm.Transport;

namespace Urbetrack.Mobile.Comm.Transaccional
{
    public abstract class Transaction
    {
        public enum States
        {
            INICIO,
            ENVIANDO,
            CONFIRMADO,
            CONFIRMANDO,
            RESPONDIENDO,
            TERMINADO
        }

        protected Transaction(AbstractTransport t, TransactionUser ut)
        {
            Transport = t;
            TransactionUser = ut;
            State = States.INICIO;
        }

        #region Acciones de MRC y MRS
        public abstract void RetryTimerExpired(object sender);
        public abstract void DisposeTimerExpired(object sender);
        public abstract void ReceivedPDU(PDU pdu);
        #endregion

        #region Manipulacion de Timers
        public void SetupRetryTimer()
        {
            tr = null;
            if (Transport == null)
            {
                throw new Exception("SA: no esta definida la capa de transport.");
            }
            if (!Transport.RequiereRetransmicion) return;
            if (tr_activo == 0) tr_activo = Transport.TimerTR;
            else if (tr_activo < Transport.TimerTRM) tr_activo = tr_activo * 2;
            tr = new Timer(RetryTimerExpired, null, tr_activo, Timeout.Infinite);
        }

        public void SetupDisposeTimer()
        {
            tv = null;
            if (Transport == null)
            {
                throw new Exception("SA: no esta definida la capa de transport.");
            }
            tv = new Timer(DisposeTimerExpired, null, Transport.TimerTV, Timeout.Infinite);
        }
        #endregion

        #region Propiedades Publicas

        public States State { get; set; }

        public byte Seq { get; set; }

        public short DeviceId { get; set; }

        public byte ResponseCL { get; set; }

        public AbstractTransport Transport { get; protected set; }

        public TransactionUser TransactionUser { get; protected set; }

        #endregion

        #region Propiedades Privadas

        private Timer tr = null;
        private Timer tv = null;
        private int tr_activo = 0;

        #endregion

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}