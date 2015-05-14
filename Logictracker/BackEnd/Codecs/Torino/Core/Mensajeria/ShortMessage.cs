#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Comm.Core.Codecs;

#endregion

namespace Urbetrack.Comm.Core.Mensajeria
{
    public class ShortMessage : PDU
    {
        ///<set brief="Enumeracion de Comandos de Mensajeria Corta">
        ///<remarks brief="Arquitectura basada en SMPP" ref="WIKI:SMPP">
        ///El sistema de mensajeria, esta basado en SMPPv1, las operaciones
        ///disponibles son un subconjuto de las especificadas en dicha norma.
        ///</remarks>
        public enum Commands {
            INVALID             = 0x00,     /// El comando no se establecion.
            SUBMIT              = 0x01,	    /// Enviar Mensaje
            DELIVER             = 0x02,		/// Hubo un cambio de Estado de mensaje enviado.
            REPLACE             = 0x03,		/// Remplazar Mensaje (en transito). 
            CANCEL              = 0x04,     /// Cancelar Mensaje (en transito).
            QUERY               = 0x05,		/// Consultar Estado de mensaje.
            
            SET_DRAFT           = 0xC0,     /// Establecer un mensaje borrador.
            CLEAR_DRAFTS        = 0xCD,     /// Establecer un mensaje borrador.            
            
        }
        /// </set>

        /// <set brief="Enumeracion de los posibles estados de entrega de un mensaje">
        /// <remarks brief="Arquitectura basada en SMPP" ref="WIKI:SMPP">
        /// El sistema de mensajeria, esta basado en SMPP, los estados de entrega
        /// son por lo tanto, los especificados en dicha norma, exceptuando los
        /// casos indicados.
        /// </remarks>
        public enum DeliveryStatus
        {
            ///<subset brief="Estados heredados de SMPP.">
            ENROUTE = 1, 			/// (value=1) The message is in enroute state.
            DELIVERED, 				/// (value=2) Message is delivered to destination
            EXPIRED,		        /// (value=3) Message validity period has expired.
            DELETED, 				/// (value=4) Message has been deleted.
            UNDELIVERABLE, 			/// (value=5) Message is undeliverable
            ACCEPTED, 				/// (value=6) Message is in accepted state 
                                    ///           (i.e. has been manually, read
                                    ///           on behalf of the subscriber by 
                                    ///           customer service)
            UNKNOWN,  			    /// (value=7) Message is in invalid state
            REJECTED, 			    /// (value=8) Message is in a rejected state
            ///</subset>

            ///<subset brief="Transiciones propietarias.">
            READ_RECEIPT =0x80,     /// (value=10)El mensaje fue leido.
            ///</subset>
        };
        //</set>
        
        public ShortMessage()
        {
            CH = (byte) Codes.HighCommand.ShortMessage;
            CL = 0xFF; // INDICA INVALIDO
            Saliente = true;
        }

        public DeliveryStatus Status { get; set; }

        public int Revision { get; set; }

        public short Code { get; set; }

        public byte Source { get; set; }

        public byte Destination { get; set; }

        public string Message { get; set; }

        public string ReplyFilter { get; set; }

        public int Session { get; set; }

        public override void FinalEncode(ref byte[] buffer, ref int pos)
        {
            switch (CL)
            {
                case 0x00: // SET/MODIFY 
                case 0x01: // DELETE
                case 0x02: // DELETE ALL
                case 0x03: // SET LOGISTIC STATE
                case 0x04: // SHOW DRAFT
                case 0x05: // SHOW CUSTOM
                case 0x06: // DELIVER
                case 0x07: // READ RECEIPT
                    UrbetrackCodec.EncodeInteger(ref buffer, ref pos, Revision);
                    UrbetrackCodec.EncodeByte(ref buffer, ref pos, Convert.ToByte(Code & 0xFF));
                    UrbetrackCodec.EncodeByte(ref buffer, ref pos, Convert.ToByte((Source == 'M' ? 0x1 : 0) & (Destination == 'M' ? 0x02 : 0)));
                    UrbetrackCodec.EncodeString(ref buffer, ref pos, Message);
                    UrbetrackCodec.EncodeString(ref buffer, ref pos, ReplyFilter);
                    UrbetrackCodec.EncodeInteger(ref buffer, ref pos, Session);
                    break;
            }
        }

        public override string Trace(string data)
        {
            return base.Trace("MESSAGE");
        }

        public List<string> TraceFull()
        {
            var lista = new List<string>();
            lista.Insert(0, base.Trace("MESSAGE"));
            lista.Insert(0, String.Format("\tAction: {0} Code: {1} Value: {2} Revision: {3}", CL, Code, Message, Revision));
            return lista;
        }
    }
}