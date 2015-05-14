using System;
using System.Net;
using System.Text;

namespace Urbetrack.Mobile.Comm.Messages
{
    public class Decoder
    {
        #region Opciones CH
        public enum ComandoH
        {
            // Solicitudes
            Discovery = 0x00,
            LoginRequest = 0x01,
            Reboot = 0x02,
            Status = 0x03,
            KeepAlive = 0x04,
            // Mensajes AVL
            MsgPosicion = 0x10,
            MsgTeclado = 0x11,
            MsgTecladoTr = 0x12,
            MsgEvento = 0x12,
            // Administracion de Archivos
            FAT_Crear = 0x20,
            FAT_Pedir = 0x21,
            FAT_Datos = 0x22,
            FAT_Borra = 0x23,
            FAT_Ren = 0x24,
            FAT_Lista = 0x25,

            // Varios
            GR2_Cambio = 0x30,

            // Entre Servidores (llevan options 1)
            HEARTBEAT = 0x50,   //!< anuncio de vida del server.
            ACARREO = 0x51,   //!< reenvio de paquete de un dispositivo.

            // Flash Over The Air.
            FOTA_TCP = 0xF0,
            FOTA_START = 0xFA,
            FOTA_DATA = 0xFB,
            FOTA_END = 0xFC,
            FOTA = 0x7F,

            // Respuestas
            OKACK = 0x80,
            LoginAcepted = 0x81,
            NodoDisponible = 0x82,
            HardwareStatus = 0x83,

            // Administracion de Archivos
            FAT_Creado = 0xA0,
            FAT_DataOk = 0xA1,
            FAT_Salvado = 0xA2,
            FAT_Borrado = 0xA3,
            FAT_RenOk = 0xA4,
            FAT_PaginaDir = 0xA5,

            // Errores Generales
            BadOptions = 0xB0,
            LoginReject = 0xB1,

            // Errores Archivos
            FAT_Error = 0xC0,

            // Varios
            ErrorInterno = 0xDF,
            FOTA_ERROR = 0xFA
        }

        public enum DecodeErrors
        {
            NoError,
            ChecksumError,
            BadLength,
            BadOptions

        }
        #endregion

        #region Tipo: Byte
        public static byte DecodeByte(byte[] buffer, ref int pos)
        {
            return buffer[pos++];
        }

        public static void EncodeByte(ref byte[] buffer, ref int pos, byte data)
        {
            buffer[pos++] = data;
        }
        #endregion

        #region Tipo: Short
        public static short DecodeShort(byte[] buffer, ref int pos)
        {
            return (short)(buffer[pos++] << 8 | buffer[pos++]);
        }

        public static void EncodeShort(ref byte[] buffer, ref int pos, short data)
        {
            buffer[pos++] = Convert.ToByte(data >> 8);
            buffer[pos++] = Convert.ToByte(data & 0xFF);
        }
        #endregion

        #region Tipo: Integer
        public static int DecodeInteger(byte[] buffer, ref int pos)
        {
            return (buffer[pos++] << 24 | buffer[pos++] << 16 | buffer[pos++] << 8 | buffer[pos++]);
        }

        public static void EncodeInteger(ref byte[] buffer, ref int pos, int data)
        {
            buffer[pos++] = Convert.ToByte((data >> 24) & 0xFF);
            buffer[pos++] = Convert.ToByte((data >> 16) & 0xFF);
            buffer[pos++] = Convert.ToByte((data >> 8) & 0xFF);
            buffer[pos++] = Convert.ToByte(data & 0xFF);
        }
        #endregion

        #region Tipo: Float
        public static float DecodeFloat(byte[] buffer, ref int pos)
        {
            byte[] invertido = new byte[4];
            invertido[0] = buffer[pos + 3];
            invertido[1] = buffer[pos + 2];
            invertido[2] = buffer[pos + 1];
            invertido[3] = buffer[pos];
            pos += 4;
            return BitConverter.ToSingle(invertido, 0);
        }

        public static void EncodeFloat(ref byte[] buffer, ref int pos, float data)
        {
            byte[] invertido;
            invertido = BitConverter.GetBytes(data);
            buffer[pos + 3] = invertido[0];
            buffer[pos + 2] = invertido[1];
            buffer[pos + 1] = invertido[2];
            buffer[pos] = invertido[3];
            pos += 4;
        }
        #endregion

        #region Tipo: String
        public static string DecodeString(byte[] buffer, ref int pos)
        {
            int size = buffer[pos++];
            if (size == 0)
            {
                return "";
            }
            string salida = Encoding.ASCII.GetString(buffer, pos, size);
            pos += size;
            return salida;
        }

        public static void EncodeString(ref byte[] buffer, ref int pos, string data)
        {
            if (data == null)
            {
                buffer[pos++] = 0x00; // string vacio.
                return;
            }
            if (data.Length > 0xFF)
            {
                throw new Exception("DAC, imposible codificar un string de mas de 256bytes.");
            }
            buffer[pos++] = Convert.ToByte(data.Length);
            Array.Copy(Encoding.ASCII.GetBytes(data), 0, buffer, pos, data.Length);
            pos += data.Length;
        }
        #endregion

        #region Tipo: Bytes
        public static byte[] DecodeBytes(byte[] buffer, ref int pos, int size)
        {
            byte[] salida = new byte[size];
            Array.Copy(buffer, pos, salida, 0, size);
            pos += size;
            return salida;
        }

        public static void EncodeBytes(ref byte[] buffer, ref int pos, byte[] data)
        {
            //TODO
        }
        #endregion

        #region Tipo: IPEndPoint
        public static IPEndPoint DecodeIPEndPoint(byte[] buffer, ref int pos)
        {
            byte[] b = new byte[4];
            Array.Copy(buffer, pos, b, 0, 4);
            pos += 4;
            short port = DecodeShort(buffer, ref pos);
            return new IPEndPoint(new IPAddress(b), port);
        }

        public static void EncodeIPEndPoint(ref byte[] buffer, ref int pos, IPEndPoint ep)
        {
            byte[] b = ep.Address.GetAddressBytes();
            Array.Copy(b, 0, buffer, pos, 4);
            pos += 4;
            EncodeShort(ref buffer, ref pos, (short)ep.Port);
        }
        #endregion


        #region Tipo: Headers
        public static void DecodeHeaders(byte[] buffer, ref PDU pdu, ref int pos)
        {
            pdu.Incomming = true;
            pdu.DeviceId = DecodeShort(buffer, ref pos);
            pdu.Seq = DecodeByte(buffer, ref pos);
            pdu.CH = DecodeByte(buffer, ref pos);
            pdu.CL = DecodeByte(buffer, ref pos);
        }

        public static void EncodeHeaders(ref byte[] buffer, ref int pos, PDU pdu)
        {
            EncodeShort(ref buffer, ref pos, pdu.DeviceId);
            EncodeByte(ref buffer, ref pos, pdu.Seq);
            EncodeByte(ref buffer, ref pos, pdu.CH);
            EncodeByte(ref buffer, ref pos, pdu.CL);
        }
        #endregion

        #region Checksum
        public static byte Checksum8B(byte[] buffer, int limit)
        {
            return Convert.ToByte(Checksum8(buffer, limit) & 0xFF);
        }

        public static short Checksum8(byte[] buffer, int limit)
        {
            short sum = 0;
            foreach (byte b in buffer)
            {
                if (limit-- == 0) break;
                sum += b;
            }
            return sum;
        }
        #endregion

        #region Decodificador
        public static PDU Decode(byte[] buffer, ref DecodeErrors outcode)
        {
            /*int chkpos = buffer.GetLength(0) - 2;
            short chk = DecodeShort(buffer, ref chkpos);
            if (Checksum8(buffer, buffer.GetLength(0) - 2) != chk)
            {   // CRC ERROR
                crc_errors++;
                outcode = DecodeErrors.ChecksumError; // crcerror
                return null;
            }*/
            int pos = 0;
            short pdusize = DecodeShort(buffer, ref pos);
            if (pdusize != buffer.GetLength(0) - 2)
            {
                badlength_error++;
                outcode = DecodeErrors.BadLength;
                return null;
            }
            byte options = DecodeByte(buffer, ref pos);
            if (options > 0x01)
            {
                badoptions_error++;
                outcode = DecodeErrors.BadOptions;
                return null;
            }
            PDU headers = new PDU();
            DecodeHeaders(buffer, ref headers, ref pos);
            pos = 3;
            switch ((ComandoH)headers.CH)
            {
                case ComandoH.LoginRequest:
                    return new LoginRequest(buffer, pos);
                case ComandoH.LoginAcepted:
                    return new LoginAcepted(buffer, pos);
                case ComandoH.LoginReject:
                    return new LoginReject(buffer, pos);
                case ComandoH.MsgPosicion:
                    return new Posicion(buffer, pos);
                case ComandoH.MsgEvento:
                    return DecodeEventos(headers, buffer, pos);
                default:
                    return headers;
            }
        }

        internal static PDU DecodeEventos(PDU headers, byte[] buffer, int pos)
        {

            return null;
        }

        #endregion

        #region Codificador
        public static void Encode(ref byte[] buffer, ref int size, PDU pdu)
        {
            int start = 0;
            int payload = 3; // salteo
            EncodeHeaders(ref buffer, ref payload, pdu);
            pdu.FinalEncode(ref buffer, ref payload);
            short pdu_size = Convert.ToInt16(payload & 0xFFFF);
            // PDU Size.
            EncodeShort(ref buffer, ref start, pdu_size);
            // Options = 0x00
            EncodeByte(ref buffer, ref start, pdu.Options);
            short sum = Checksum8(buffer, pdu_size);
            EncodeShort(ref buffer, ref payload, sum);
            size = pdu_size + 2; // del sum
        }
        #endregion

        #region Contadores Estadisticos

        private static int badlength_error = 0;
        private static int badoptions_error = 0;

        #endregion
    }
}