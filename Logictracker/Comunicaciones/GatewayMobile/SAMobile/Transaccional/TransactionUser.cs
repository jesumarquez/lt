using Urbetrack.Mobile.Comm.Messages;
using Urbetrack.Mobile.Comm.Transaccional;
using Urbetrack.Mobile.Comm.Transport;

namespace Urbetrack.Mobile.Comm.Transaccional
{
    public abstract class TransactionUser
    {
        // MRS cuando llega una nueva solicitud, siempre.
        public abstract bool NuevaSolicitud(PDU pdu, Transaction tr, AbstractTransport t);
        
        // MRS si la solicitud requiere RTA
        public abstract PDU RequieroRespuesta(PDU pdu, Transaction tr, AbstractTransport t);

        // MRS cuando recibe el ACK de un mensaje con RTA.
        public abstract void RespuestaEntregada(PDU pdu, Transaction tr, AbstractTransport t);

        // MRS cuando nunca recibio el ACK.
        public abstract void RespuestaNoEntregada(PDU pdu, Transaction tr, AbstractTransport t);
        
        // MRC cuando llega una RTA a una solicitud enviada.
        public abstract PDU RequieroACK(PDU pdu, Transaction tr, AbstractTransport t);
        
        // MRC cuando no obtiene ni RTA ni ACK.
        public abstract void SolicitudNoEnviada(PDU pdu, Transaction tr, AbstractTransport t);

        // MRC cuando obtiene un ACK de un msg sin RTA.
        public abstract void SolicitudEntregada(PDU response, PDU pdu, Transaction tr, AbstractTransport t);

        public abstract byte LimiteSuperiorSeq
        {
            get;
        }

        public abstract byte LimiteInferiorSeq
        {
            get;
        }
    }
}