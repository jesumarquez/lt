using Urbetrack.Mobile.Comm.Messages;
using Urbetrack.Mobile.Comm.Transaccional;
using Urbetrack.Mobile.Comm.Transport;

namespace Urbetrack.Mobile.Comm.Transaccional
{
    class MRS : Transaction
    {
        private readonly PDU pdu;
        private PDU rta;

        public MRS(PDU _pdu, AbstractTransport t, TransactionUser ut)
            : base(t, ut)
        {
            pdu = _pdu;
            Seq = pdu.Seq;
            DeviceId = pdu.DeviceId;
        }

        public void Start()
        {
            TransactionUser.NuevaSolicitud(pdu, this, Transport);
            if (pdu.RequiresAnswer)
            {
                rta = TransactionUser.RequieroRespuesta(pdu, this, Transport);
                rta.DeviceId = pdu.DeviceId;
                rta.Destination = pdu.Destination;
                rta.Seq = pdu.Seq;
            } 
            else
            {
                rta = new PDU
                          {
                              DeviceId = pdu.DeviceId,
                              Seq = pdu.Seq,
                              Options = pdu.Options,
                              Destination = pdu.Destination,
                              CH = ((byte) Decoder.ComandoH.OKACK),
                              CL = ResponseCL
                          };
            }
            Transport.Send(rta);
            SetupDisposeTimer();
            if (pdu.RequiresACK)
            {
                State = States.RESPONDIENDO;
                SetupRetryTimer();
            } 
            else
            {
                State = States.CONFIRMANDO;
            }
            
        }

        public override void RetryTimerExpired(object sender)
        {
            if (State == States.RESPONDIENDO)
            {
                Transport.Send(rta);
                SetupRetryTimer();
            }
        }

        public override void DisposeTimerExpired(object sender)
        {
            if (State == States.CONFIRMANDO)
            {
                State = States.TERMINADO;
                Transport.TransaccionCompleta(this, pdu);
            } else if (State == States.RESPONDIENDO)
            {
                State = States.TERMINADO;
                TransactionUser.RespuestaNoEntregada(rta, this, Transport);
                Transport.TransaccionCompleta(this, pdu);
            }
        }

        public override void ReceivedPDU(PDU p)
        {
            if (State == States.CONFIRMANDO)
            {
                Transport.Send(rta);
            } else if (State == States.RESPONDIENDO)
            {
                if (p.CH != pdu.CH)
                {
                    State = States.TERMINADO;
                    TransactionUser.RespuestaEntregada(p, this, Transport);
                    Transport.TransaccionCompleta(this, pdu);
                }
            }
        }

        #region Propiedades Privadas
        
        #endregion
    }
}