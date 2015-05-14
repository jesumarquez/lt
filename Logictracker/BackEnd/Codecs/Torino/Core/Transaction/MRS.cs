#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transport;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Transaction
{
    class MRS : Transaccion
    {
        private readonly PDU pdu;
        private PDU rta;

        public MRS(PDU _pdu, Transporte t, TransactionUser ut)
            : base(t, ut)
        {
            pdu = _pdu;
            Seq = pdu.Seq;
            IdDispositivo = pdu.IdDispositivo;
        }

        public override string ToString()
        {
            return String.Format("MRS: seq={0} dev={1} trid={2} CH={3}", Seq, IdDispositivo, pdu.IdTransaccion, pdu.CH);
        }

        public void Start()
        {
            if (TransactionUser == null) STrace.Debug(GetType().FullName,"MRS: Usuario Transaccion es NULO.");
            if (pdu == null) STrace.Debug(GetType().FullName,"MRS: PDU es NULO.");
            if (Transporte == null) STrace.Debug(GetType().FullName,"MRS: Transporte es NULO.");
            
            //AbortaSeqCache();
            
            var replay_required = TransactionUser.NuevaSolicitud(pdu, this, Transporte);
            if (pdu.RequiereRespuesta)
            {
                rta = TransactionUser.RequieroRespuesta(pdu, this, Transporte);
                if (rta == null)
                {
                    Estado = Estados.TERMINADO;
                    Transporte.TransaccionCompleta(this, pdu);
                    return;
                }
                replay_required = true;
                rta.IdDispositivo = pdu.IdDispositivo;
                rta.Destino = pdu.Destino;
                rta.Seq = pdu.Seq;
            } 
            else
            {
                rta = new PDU
                          {
                              IdDispositivo = pdu.IdDispositivo,
                              Seq = pdu.Seq,
                              Options = pdu.Options,
                              Destino = pdu.Destino,
                              CH = ((byte) Codes.HighCommand.OKACK),
                              CL = CLdeRespuesta
                          };
            }
            if (!replay_required)
            {
                Estado = Estados.TERMINADO;
                Transporte.TransaccionCompleta(this, pdu);
                return;
            }
            rta.SourcePdu = pdu;
            if (pdu.CH == 0x10 && Transporte.UseSeqCache) Transporte.SeqCache = pdu.Seq;
            Transporte.Send(rta);
            LanzaTV();
            //Transporte.Send(rta);
            if (pdu.RequiereACK)
            {
                Estado = Estados.RESPONDIENDO;
                LanzaTR();
            } 
            else
            {
                if (Transporte.UseSeqCache)
                {
                    LanzaSeqCache(100);
                }
                Estado = Estados.CONFIRMANDO;
            }
        }

        public void LanzaSeqCache(int seq_cache_timeout)
        {
            Scheduler.Scheduler.AddTimer(VencimientoSeqCache, seq_cache_timeout);
        }

        public void VencimientoSeqCache(int timer_id)
        {
            if (pdu.CH != 0x10 || Transporte.SeqCache != pdu.Seq) return;
            Transporte.Send(rta);
        }

        public override void VencimientoTR(int timer_id)
        {
            if (Estado == Estados.RESPONDIENDO)
            {
                Transporte.Send(rta);
                LanzaTR();
            }
        }

        public override void VencimientoTV(int timer_id)
        {
            //Marshall.Debug("MSR: TV vencido.");
            switch (Estado)
            {
                case Estados.CONFIRMANDO:
                    Estado = Estados.TERMINADO;
                    Transporte.TransaccionCompleta(this, pdu);
                    break;
                case Estados.RESPONDIENDO:
                    Estado = Estados.TERMINADO;
                    TransactionUser.RespuestaNoEntregada(rta, this, Transporte);
                    Transporte.TransaccionCompleta(this, pdu);
                    break;
                default:
                    STrace.Debug(GetType().FullName, String.Format("MRS: Error en timer de vencimeinto, el estado es inconsistente state={0}", Estado));
                    Estado = Estados.TERMINADO;
                    Transporte.TransaccionCompleta(this, pdu);
                    break;
            }
        }

        public override void RecibePDU(PDU received_pdu)
        {
			STrace.Debug(GetType().FullName, String.Format("MRS: ReceivePDU: Estado={0} ReceivedPDU.CH={1} PDU.CH={2}", Estado, received_pdu.CH, pdu.CH));
            switch (Estado)
            {
                case Estados.CONFIRMANDO:
                    if (received_pdu.CH == pdu.CH)
                    {
                        if (pdu.CH == 0x10 && Transporte.UseSeqCache && Transporte.SeqCache != pdu.Seq) break;
                        Transporte.Send(rta);
                    }
                    break;
                case Estados.RESPONDIENDO:
                    if (received_pdu.CH != pdu.CH)
                    {
                        Estado = Estados.TERMINADO;
                        TransactionUser.RespuestaEntregada(received_pdu, this, Transporte);
                        Transporte.TransaccionCompleta(this, pdu);
                    }
                    break;
                default:
                    STrace.Debug(GetType().FullName, String.Format("MRS: Error en recibo PDU, el estado es inconsistente state={0}", Estado));
                    Estado = Estados.TERMINADO;
                    Transporte.TransaccionCompleta(this, pdu);
                    break;
            }
        }

        #region Propiedades Privadas
        
        #endregion
    }
}