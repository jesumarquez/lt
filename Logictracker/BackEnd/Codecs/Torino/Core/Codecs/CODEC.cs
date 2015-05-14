#region Usings

using System;
using Urbetrack.Comm.Core.Mensajeria;

#endregion

namespace Urbetrack.Comm.Core.Codecs
{
    public class TypeAttribute : Attribute
    {
        public byte Code;
        public bool Simple;
        public string Description;

        public TypeAttribute(byte Code, string Description)
            : this(Code, Description, true)
        {
        }

        public TypeAttribute(byte Code, string Description, bool Simple)
        {
            this.Code = Code;
            this.Simple = Simple;
            this.Description = Description;
        }
    }
    public abstract class CODEC
    {
        public enum BasicDataTypes
        {
            [Type(0x01,"Byte/Char")] 
            Byte,
            [Type(0x02, "Short")]
            Short,
            [Type(0x03, "Integer")]
            Integer,
            [Type(0x04, "Long")]
            Long,
            [Type(0x05, "Unsigned Short")]
            UnsignedShort,
            [Type(0x06, "Unsigned Integer")]
            UnsignedInteger,
            [Type(0x07, "Unsigned Long")]
            UnsignedLong,
            [Type(0x08, "String")]
            String,
            [Type(0x09, "Byte Array")]
            ByteArray,
            [Type(0x0A, "DateTime (32b)")] // hasta segundos
            DateTime,
            [Type(0x0B, "DeltaDateTime8(8b)")]
            DeltaDateTime8,
            [Type(0x0C, "DeltaDateTime16(16b)")]
            DeltaDateTime16,
            [Type(0x0D, "TimeStamp (64b)")] // hasta nanosegundos
            TimeStamp,
            [Type(0x0E, "Float (32b)")] 
            Float,
            [Type(0x0F, "Double (64b)")]
            Double,
            [Type(0x10, "Float 12x4 (16b)")] // 12 bits parte entera, 4 decimal
            Float12x4,
            [Type(0x11, "Float 9x7 (16b)")]  // 9 bits parte entera, 7 decimal
            Float9x7
        }

        /* el decodificador propio del protocolo que implemente */
        public abstract PDU Decode(byte[] buffer, ref Codes.DecodeErrors outcode);

        /* el decodificador propio del protocolo que implemente */
        public abstract int Encode(ref byte[] buffer, ref int size, PDU pdu);

        /* el protocolo soporta el mensaje Login Required */
        public abstract bool SupportsLoginRequired { get; }

        /* el protocolo soporta el mensaje Login Required */
        public abstract bool DontReplayWhenOffline { get; }

        public abstract bool AllowOfflineMessages { get; }
    }
}