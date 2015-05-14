#region Usings

using System;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transport;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Transaction
{
    public class MRC : Transaccion
    {
        private PDU pdu;

        public MRC(PDU _pdu, Transporte t, TransactionUser ut)
            : base(t, ut)
        {
            pdu = _pdu;
            Seq = pdu.Seq;
            IdDispositivo = pdu.IdDispositivo;
            Estado = Estados.INICIO;
        }

        public override string ToString()
        {
            return String.Format("MRC: seq={0} dev={1} trid={2} CH={3}", Seq, IdDispositivo, pdu.IdTransaccion, pdu.CH);
        }

        public void Start()
        {
            Transporte.Send(pdu);
            Estado = Estados.ENVIANDO;
            LanzaTR();
            LanzaTV();
        }

        public override void VencimientoTR(int timer_id)
        {
            if (Estado != Estados.ENVIANDO) return;
            Transporte.Send(pdu);
            LanzaTR();
        }

        public override void VencimientoTV(int timer_id)
        {
            switch (Estado)
            {
                case Estados.ENVIANDO:
                    Estado = Estados.TERMINADO;
                    TransactionUser.SolicitudNoEnviada(pdu, this, Transporte);
                    Transporte.TransaccionCompleta(this, pdu);
                    break;
                case Estados.CONFIRMANDO:
                    Estado = Estados.TERMINADO;
                    Transporte.TransaccionCompleta(this, pdu);
                    break;
                case Estados.CANCELADO:
                    Estado = Estados.TERMINADO;
                    TransactionUser.SolicitudCancelada(pdu, this, Transporte);
                    Transporte.TransaccionCompleta(this, pdu);
                    break;
            }
        }

        public void Cancelar()
        {
            if (Estado == Estados.ENVIANDO)
                Estado = Estados.CANCELADO;
        }

        public override void RecibePDU(PDU response)
        {
            switch (Estado)
            {
                case Estados.ENVIANDO:
                    if (response.CH == (byte)Codes.HighCommand.OKACK)
                    {
                        Estado = Estados.TERMINADO;
                        TransactionUser.SolicitudEntregada(response, pdu, this, Transporte);
                        Transporte.TransaccionCompleta(this, pdu);
                        return;
                    }
                    if (response.RequiereACK)
                    {
                        Estado = Estados.CONFIRMANDO;
                        pdu = TransactionUser.RequieroACK(response, this, Transporte);
                        pdu.Destino = response.Destino;
                        Transporte.Send(pdu);
                    } 
                    else
                    {
                        Estado = Estados.TERMINADO;
                        TransactionUser.SolicitudEntregada(response, pdu, this, Transporte);
                        Transporte.TransaccionCompleta(this, pdu);
                    }
                    break;
                case Estados.CONFIRMANDO:
                    Transporte.Send(pdu);
                    break;
                case Estados.CANCELADO:
                    STrace.Debug(GetType().FullName, String.Format("MRC/DEVICE[id={0}]: Recibio una rta en estado CANCELADO, se ignora.", pdu.IdDispositivo));
                    break;
            }
        }
    }
}