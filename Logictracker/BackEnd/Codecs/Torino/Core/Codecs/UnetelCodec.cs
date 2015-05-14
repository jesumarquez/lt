#region Usings

using System;
using System.Text;
using Urbetrack.Comm.Core.Codecs.Unetel;
using Urbetrack.Comm.Core.Mensajeria;
using Posicion=Urbetrack.Comm.Core.Codecs.Unetel.Posicion;

#endregion

namespace Urbetrack.Comm.Core.Codecs
{
    public class UnetelCodec : CODEC
    {
        public override PDU Decode(byte[] buffer, ref Codes.DecodeErrors outcode)
        {
            var high_command = Convert.ToChar(buffer[0]);
            outcode = Codes.DecodeErrors.NoError;
            switch (high_command)
            {
                case 'Q':
                    return FactoryPosicion(buffer);
                case 'H':
                    return FactoryHandSake(buffer);
                case 'E':
                    return FactoryEntrante(buffer);
                case 'R': {
                    var partes = Encoding.ASCII.GetString(buffer).Split(";".ToCharArray());
                    return new PDU
                               {
                                   CH = (byte) Codes.HighCommand.OKACK,
                                   CL = 0,
                                   Seq = Convert.ToByte(partes[1]),
                                   IdDispositivo = Convert.ToInt16(partes[2])
                               };
                }
                default:
                    outcode = Codes.DecodeErrors.UnknownMessage;
                    return null;
            }
        }

        public override int Encode(ref byte[] buffer, ref int size, PDU pdu)
        {
            string dgram;
            switch((Codes.HighCommand)pdu.CH)
            {
                case Codes.HighCommand.LoginAceptado:
                    {
                        var lar = pdu as LoginAceptado;
                        if (lar == null) return 0;
                        dgram = String.Format("H9;{0:D5}", lar.IdAsignado);
                        break;
                    }
                case Codes.HighCommand.OKACK:
                    {
                        if (pdu.SourcePdu == null) return 0;
                        switch ((Codes.HighCommand)pdu.SourcePdu.CH)
                        {
                            case Codes.HighCommand.MsgEvento:
                                dgram = String.Format("RE{2};{0:D3};{1:D5}", pdu.Seq, pdu.IdDispositivo, ((Evento) pdu.SourcePdu).SubTipoAuxiliar);
                                break;
                            case Codes.HighCommand.MsgPosicion:
                                dgram = String.Format("RQ2;{0:D3};{1:D5}", pdu.Seq, pdu.IdDispositivo);
                                break;
                            default:
                                return 0;
                        }
                        break;
                    }
                case Codes.HighCommand.SetParameter:
                    {
                        var par = pdu as SetParameter;
                        if (par == null) return 0;
                        dgram = String.Format("CP;{0:D3};{1:D5};{2};{3};{4:D5}", pdu.Seq, pdu.IdDispositivo, par.Parameter, par.Value, par.Revision);
                        break;
                    }
                case Codes.HighCommand.ShortMessage:
                    {
                        var msg = pdu as ShortMessage;
                        if (msg == null) return 0;
                        switch (msg.CL)
                        {
                            case 0x00: // configura mensaje
                                dgram = String.Format("CM;{0:D3};{1:D5};{2:D2};{3:D1};{4};{5:D5}", pdu.Seq, pdu.IdDispositivo, (msg.Source == 0 ? 0 : 1), msg.Code, msg.Message, msg.Revision);
                                break;
                            case 0x01: // borra mensaje
                                dgram = String.Format("DM;{0:D3};{1:D5};{2:D2}", pdu.Seq, pdu.IdDispositivo, msg.Code);
                                break;
                            case 0x02: // borra todos los mensajes
                                dgram = String.Format("DM;{0:D3};{1:D5};00", pdu.Seq, pdu.IdDispositivo);
                                break;
                            case 0x03: // setea el ciclo logistico
                                dgram = String.Format("CL;{0:D3};{1:D5};{2:D2}", pdu.Seq, pdu.IdDispositivo, msg.Code);
                                break;
                            case 0x04: // muestra un mensaje predefinido
                                dgram = String.Format("DD;{0:D3};{1:D5};{2:D2}", pdu.Seq, pdu.IdDispositivo, msg.Code);
                                break;
                            case 0x05: // muestra un mensaje personalizado
                                dgram = String.Format("DD;{0:D3};{1:D5};99;{2}", pdu.Seq, pdu.IdDispositivo, msg.Message);
                                break;
                            default:
                                return 0;
                        }
                        break;
                    }
                case Codes.HighCommand.Reboot:
                    {
                        dgram = String.Format("RS;{0:D3};{1:D5}", pdu.Seq, pdu.IdDispositivo);
                        break;
                    }
                default:
                    return 0;
            }
            var bytes = Encoding.ASCII.GetBytes(dgram);
            Array.Copy(bytes, buffer, bytes.GetLength(0));
            size += bytes.GetLength(0);
            return size;
        }

        public override bool SupportsLoginRequired
        {
            get { return false; }
        }

        public override bool DontReplayWhenOffline
        {
            get { return true; }
        }

        public override bool AllowOfflineMessages
        {
            get { return false; }
        }

        private static PDU FactoryPosicion(byte[] ins)
        {
            switch (Convert.ToChar(ins[1]))
            {
                    /*case 'P':
                    return new Posicion(ins);*/
                case '2':
                    return Posicion.Parse(ins);
                    /*case 'X':
                    return new PosicionLcy(ins);*/
            }
            throw new ApplicationException(
                String.Format("UNETEL/CODEC/DECODER/POSICION: No se como contruir el MSG [{0}{1} ",
                              ins[0], ins[1]
                    ));
        }

        private static PDU FactoryHandSake(byte[] ins)
        {
            switch (Convert.ToChar(ins[1]))
            {
                    /*case 'S':
                    return new HardwareStatus(ins);*/
                case '0':
                    return HandShake0.Parse(ins);
            }
            throw new ApplicationException(
                String.Format("No se como contruir el MSG [{0}{1} ",
                              ins[0], ins[1]
                    ));
        }
        
        private static PDU FactoryEntrante(byte[] buffer)
        {
            return Entrante.Parse(buffer);
        }
    }
}