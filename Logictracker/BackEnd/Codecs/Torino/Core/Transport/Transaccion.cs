#region Usings

using System;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transport;

#endregion

namespace Urbetrack.Comm.Core.Transaction
{
    public abstract class Transaccion
    {
        public enum Estados
        {
            INICIO,
            ENVIANDO,
            CONFIRMADO,
            CONFIRMANDO,
            RESPONDIENDO,
            TERMINADO,
            CANCELADO
        }

        protected Transaccion(Transporte t, TransactionUser ut)
        {
            CustomTV = 0;
            Transporte = t;
            TransactionUser = ut;
            Estado = Estados.INICIO;
        }

        #region Acciones de MRC y MRS
        public abstract void VencimientoTR(int timer_id);
        public abstract void VencimientoTV(int timer_id);
        public abstract void RecibePDU(PDU pdu);
        //public virtual void VencimientoSeqCache(int timer_id) { }
        #endregion

        #region Manipulacion de Timers
        //public static int seq_cache_timer_id;
       
      //  public void AbortaSeqCache()
       // {
       //     if (seq_cache_timer_id != 0) Scheduler.Scheduler.DelTimer(seq_cache_timer_id);
       // }

        public void LanzaTR()
        {
            //tr = null;
            if (Transporte == null)
            {
                throw new Exception("SA: no esta definida la capa de transporte.");
            }
            if (Transporte.ConnectionMode != Transporte.ConnectionModes.DatagramOriented) return;
            if (tr_activo == 0) tr_activo = Transporte.TimerTR;
            else if (tr_activo < Transporte.TimerTRM) tr_activo = tr_activo * 2;
            Scheduler.Scheduler.AddTimer(VencimientoTR, tr_activo);
        }

        public void LanzaTV()
        {
            if (Transporte == null)
            {
                throw new Exception("SA: no esta definida la capa de transporte.");
            }
            Scheduler.Scheduler.AddTimer(VencimientoTV, Transporte.TimerTV);
        }
        #endregion
        
        #region Propiedades Publicas

        public int CustomTV { get; set; }

        public bool Pasada { get; set; }

        public Estados Estado { get; set; }

        public byte Seq { get; set; }

        public short IdDispositivo { get; set; }

        public byte CLdeRespuesta { get; set; }

        public Transporte Transporte { get; private set; }

        public TransactionUser TransactionUser { get; private set; }

        #endregion

        #region Propiedades Privadas

        private int tr_activo;

        #endregion
    }
}