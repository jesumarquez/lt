using System;
using System.Text;
using Urbetrack.DatabaseTracer.Core;

namespace XbeeCore
{
    /// <summary>
    /// Representa un mensaje enviado o recibido por un puerto XBEE.
    /// </summary>
    public class XBeePDU
    {
        #region Valores Constantes
        public const ushort BROADCAST = ushort.MaxValue;
        public const ushort MAXBUFFERSIZE = 256;
        #endregion

        #region Constructores
        public XBeePDU()
        {
            Address = 0;
            IdAPI = 0;
            Sequence = 0;
            HWStatus = 0xFF;
            Sequence = 0;
            Address = 0;
            IdAPI = 0;
            Data = null;
        }

        public XBeePDU(ushort _address, ushort _api, string _data)
            : this(_address, _api, Encoding.ASCII.GetBytes(_data), 0x00) { }

        public XBeePDU(ushort _address, ushort _api, string _data, byte _seq)
            : this(_address, _api, Encoding.ASCII.GetBytes(_data), _seq) { }

        public XBeePDU(ushort _address, ushort _api, byte[] _data)
            : this(_address, _api, _data, 0x00) {}

        public XBeePDU(ushort _address, ushort _api, byte[] _data, byte _seq)
        {
            Data = _data;
            Address = _address;
            IdAPI = _api;
            Sequence = _seq;
            HWStatus = 0xFF;
        }
        #endregion

        #region Propiedades Publicas

        public bool Ack { get; private set; }

        public string StringData
        {
            get { return Encoding.ASCII.GetString(Data); }
            set { Data = Encoding.ASCII.GetBytes(value); }
        }

        public byte[] Data { get; set; }

        public ushort IdAPI { get; set; }

        public ushort Address { get; set; }

        public byte Sequence { get; set; }

        public byte HWStatus { get; set; }

        #endregion

        #region Decoder/Encoder
        public byte[] ToByteArray()
        {
            if (IdAPI == 0x01)
            {
                return ToByteArray_API01();
            }
            if (IdAPI == 0x08)
            {
                return ToByteArray_API08();
            }
            if (IdAPI == 0x17)
            {
                return ToByteArray_API17();
            }
            throw new Exception(String.Format("XBEEAPI: API Id del mensaje, desconocido apiid={0}", IdAPI));
        }

        private byte[] ToByteArray_API17()
        {
            // primero los headers.
            var slength = Convert.ToInt16(Data.Length + 13);
            var ix = 0;
            // este es el tamanio maximo del buffer con todos los caracteres escapeados.-
            var buffer = new byte[2 + slength];
            // Byte 1 - Start Delimiter
            // 0x7E ----> se pone en SEND 

            // Bytes 2,3 - Length (BigEndian) (ESCAPADO)
            buffer[ix++] = 0; // los paquetes nunca tienen mas de 100 bytes.
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(slength & 0xFF));
            // FrameData
            // byte 4 - API IDENTIFIER
            buffer[ix++] = 0x17; // Remote Comand Request
            // byte 5 - sequence number (0x01 a 0xFF)
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(Sequence));
            // byte 6-13 - 64bit adddress (a 0)
            ix += 8;
            // byte 14-15
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(Address >> 8));
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(Address & 0xFF));
            // byte 16 - 0x02 para grabar los cambios
            buffer[ix] = 0x02;
            if (Data[0] == 'B' && Data[1] == 'D')
            {
                Data[2] = (byte)(Data[2] - 0x30);
            }
            if (Data[0] == 'C' && Data[1] == 'H')
            {
                if (Data[2] == 'C') Data[2] = 12;
                if (Data[2] == 'D') Data[2] = 13;
                if (Data[2] == 'E') Data[2] = 14;
                if (Data[2] == 'F') Data[2] = 15;
            }
            Array.Copy(Data, 0, buffer, 16, Data.Length);
            return buffer;
        }

        // TX REQUEST, 16bit Address
        private byte[] ToByteArray_API01()
        {
            // primero los headers.
            var _length = Convert.ToInt16(Data.Length + 5);
            var ix = 0;
            // este es el tamanio maximo del buffer con todos los caracteres escapeados.-
            var buffer = new byte[_length];
            // FrameData
            // byte 4 - API IDENTIFIER
            buffer[ix++] = 0x01; // Transmit TX Request at 16 bits
            // byte 5 - sequence number (0x01 a 0xFF)
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(Sequence));
            // byte 6,7 - pdu source or destination address
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(Address >> 8));
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(Address & 0xFF));
            // byte 8 - 0x00 
            buffer[ix++] = 0x00;
            Array.Copy(Data, 0, buffer, ix, Data.Length);
            return buffer;
        }

        // Modem Status Request
        private byte[] ToByteArray_API08()
        {
            // primero los headers.
            var _length = Convert.ToInt16(Data.Length + 2);
            if (_length < 4) return null;
            var ix = 0;
            // este es el tamanio maximo del buffer con todos los caracteres escapeados.-
            var buffer = new byte[2 + _length];
            // Byte 1 - Start Delimiter
            // 0x7E ----> se pone en SEND 
            // Bytes 2,3 - Length (BigEndian) (ESCAPADO)
            buffer[ix++] = 0; // los paquetes nunca tienen mas de 100 bytes.
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(_length & 0xFF));
            // FrameData
            // byte 4 - API IDENTIFIER
            buffer[ix++] = 0x08; // AT COMMAND REQUEST
            // byte 5 - sequence number (0x01 a 0xFF)
            #if ENABLE_XBEE_TR_LAYER
            buffer[ix++] = Convert.ToByte(Convert.ToInt16(Sequence));            
            #else
            buffer[ix++] = 0xA1;
            #endif
            
            // byte 6,7 - AT Command en si
            Array.Copy(Data, 0, buffer, ix, Data.Length);
            return buffer;
        }

        public static byte[] Verify(byte[] source)
        {
            // El protocolo API de Xbee requiere esta secuencia
            // de escapamiento de ciertos caracteres utilizados para
            // el control interno de la propia API.
            // calculo cuantos tendre que escapar y el tamanio del nuevo buffer.
            var size = 0;
            if (source.GetLength(0) >= 3)
            {
                size = source[2]; // tamanio del buffer.
            }
            var ix = 0;
            
            var dest = new byte[size + 4];
            // ahora escapeo la secuencia
            for(var i = 0; i < (size + 4);++i)
            {
               dest[ix++] = source[i];
            }
            // coputo del checksum
            var checksum = 0;
            ix = 0;
            foreach (var b in dest)
            {
                if (ix++ > 2 && ix < dest.GetLength(0))
                    checksum += b;
            }
            if (0xFF - (checksum & 0xFF) != dest[ix-1])
            {
                Stack._checksum_errors++;
                return null;
            }
            return dest;
        }

        public enum PushStatus
        {
            CONTINUE,
            DISPATCH,
            RESET
        };

        public PushStatus PushReadedByte(byte b)
        {
            if (buffer_pos > MAXBUFFERSIZE) return PushStatus.RESET;
            read_buffer[buffer_pos++] = b;
            if (buffer_pos == 3) length = read_buffer[2];         
            if (buffer_pos == length + 4)
                return PushStatus.DISPATCH;
            return PushStatus.CONTINUE;
        }

        public bool Decode()
        {
            var buffer = Verify(read_buffer);
            if (buffer == null) return false;
            
            var ApiId = buffer[3];
            if (ApiId == 0x89)
            { // Es un ACK.
                return DecodeACK(buffer);
            }
            if (ApiId == 0x81)
            {
                return DecodeReceived(buffer);
            }
            if (ApiId == 0x88)
            {
                return DecodeATResponse(buffer);
            }
            if (ApiId == 0x8A)
            {
                return DecodeModemStatus(buffer);
            }
            if (ApiId == 0x83)
            {
                IdAPI = 0x83;
                return true;
            }
            if (ApiId == 0x97)
            {
                IdAPI = 0x97;
                return true;
            }
            STrace.Debug(GetType().FullName, String.Format("XBEEAPI: decodificando mensaje desconocido. apiid={0}", ApiId));
            return false;
        }

        private bool DecodeModemStatus(byte[] buffer)
        {
            length = buffer[2];
            IdAPI = 0x8A;
            HWStatus = buffer[4];
            return true;
        }

        private bool DecodeATResponse(byte[] buffer)
        {
            length = buffer[2];
            IdAPI = 0x88;
            Sequence = buffer[4];
            Address = buffer[7];
            Data = new byte[buffer.GetLength(0) - 7];
            Array.Copy(buffer, 5, Data, 0, 2);
            Array.Copy(buffer, 8, Data, 2, buffer.GetLength(0) - 9);
            return true;
        }

        private bool DecodeACK(byte[] buffer)
        {
            length = buffer[2];
            IdAPI = 0x89;
            Sequence = buffer[4];
            Ack = (buffer[5] == 0x00);
            return true;
        }

        private bool DecodeReceived(byte[] buffer)
        {
            length = buffer[2];
            IdAPI = 0x81;
            Address = (ushort)(buffer[4] << 8 | buffer[5]);
            Data = new byte[length - 5];
            Array.Copy(buffer, 8, Data, 0, length - 5);
            return true;
        }
        #endregion

        #region Variables privadas 
        internal byte length;
        private readonly byte[] read_buffer = new byte[MAXBUFFERSIZE]; // mas que suficiente.
        private int buffer_pos;
        #endregion
    }
}
