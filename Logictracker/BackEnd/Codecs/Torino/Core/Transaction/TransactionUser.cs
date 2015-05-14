#region Usings

using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transport;

#endregion

namespace Urbetrack.Comm.Core.Transaction
{
    public abstract class TransactionUser
    {
        // MRS cuando llega una nueva solicitud, siempre.
        public abstract bool NuevaSolicitud(PDU pdu, Transaccion tr, Transporte t);
        
        // MRS si la solicitud requiere RTA
        public abstract PDU RequieroRespuesta(PDU pdu, Transaccion tr, Transporte t);

        // MRS cuando recibe el ACK de un mensaje con RTA.
        public abstract void RespuestaEntregada(PDU pdu, Transaccion tr, Transporte t);

        // MRS cuando nunca recibio el ACK.
        public abstract void RespuestaNoEntregada(PDU pdu, Transaccion tr, Transporte t);
        
        // MRC cuando llega una RTA a una solicitud enviada.
        public abstract PDU RequieroACK(PDU pdu, Transaccion tr, Transporte t);
        
        // MRC cuando no obtiene ni RTA ni ACK.
        public abstract void SolicitudNoEnviada(PDU pdu, Transaccion tr, Transporte t);

        // MRC cuando obtiene un ACK de un msg sin RTA.
        public abstract void SolicitudEntregada(PDU response, PDU pdu, Transaccion tr, Transporte t);

        // MRC cuando se cancela definitivamente una solicitud (previo llamado a MRC.Cancelar();
        // la cancelacion no se notifica al otro extremo es una mecanica para cortar
        // un pedido en curso y salir de forma prolija.
        public abstract void SolicitudCancelada(PDU pdu, Transaccion mrc, Transporte transporte);

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