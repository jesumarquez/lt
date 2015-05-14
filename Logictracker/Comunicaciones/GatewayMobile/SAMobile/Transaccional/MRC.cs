using Urbetrack.Mobile.Comm.Messages;
using Urbetrack.Mobile.Comm.Transaccional;
using Urbetrack.Mobile.Comm.Transport;

namespace Urbetrack.Mobile.Comm.Transaccional
{
    class MRC : Transaction
    {
        internal PDU pdu;

        public MRC(PDU _pdu, AbstractTransport t, TransactionUser ut)
            : base(t, ut)
        {
            pdu = _pdu;
            Seq = pdu.Seq;
            DeviceId = pdu.DeviceId;
            State = States.INICIO;
        }

        public void Start()
        {
            Transport.Send(pdu);
            State = States.ENVIANDO;
            SetupRetryTimer();
            SetupDisposeTimer();
        }

        public override void RetryTimerExpired(object sender)
        {
            if (State == States.ENVIANDO)
            {
                Transport.Send(pdu);
                SetupRetryTimer();
            }
        }

        public override void DisposeTimerExpired(object sender)
        {
            if (State == States.ENVIANDO)
            {
                State = States.TERMINADO;
                TransactionUser.SolicitudNoEnviada(pdu, this, Transport);
                Transport.TransaccionCompleta(this, pdu);
            } else if (State == States.CONFIRMANDO)
            {
                State = States.TERMINADO;
                // no se informa TU
                Transport.TransaccionCompleta(this, pdu);
            }
        }

        public override void ReceivedPDU(PDU response)
        {
            if (State == States.ENVIANDO)
            {
                if (response.RequiresACK)
                {
                    State = States.CONFIRMANDO;
                    pdu = TransactionUser.RequieroACK(response, this, Transport);
                    pdu.Destination = response.Destination;
                    Transport.Send(pdu);
                } 
                else
                {
                    State = States.TERMINADO;
                    TransactionUser.SolicitudEntregada(response, pdu, this, Transport);
                    Transport.TransaccionCompleta(this, pdu);
                }
            } 
            else if (State == States.CONFIRMANDO)
            {
                // se reenvia el ack, pues llego la rta nuevamente.
                Transport.Send(pdu);
            }
        }
    }
}