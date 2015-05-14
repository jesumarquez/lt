#region Usings

using System;

#endregion

namespace Urbetrack.Compresion
{
    public class BitStream
    {
        private readonly byte[] buffer;
        private int cursor;
        private byte current_byte;
        private byte current_bit;
        public int total_bits;

        public BitStream(byte [] in_buffer)
        {
            buffer = in_buffer;
            cursor = 0;
            current_byte = buffer[cursor++];
            current_bit = 7;
            total_bits = 0;
        }

        public BitStream(int max_size)
        {
            buffer = new byte[max_size];
            cursor = 0;
            current_byte = 0;
            current_bit = 7;
            total_bits = 0;
        }

        public void Pad()
        {
            //Marshall.User("BITSTREAM: Padding of {0} bits.", current_bit);
            if (current_bit == 7) return; // no se requiere padding.
            for (int i = current_bit; i >= 0; --i) Push(false);
        }

        public void Push(int src, int bits)
        {
            for (var i = (bits-1); i >= 0; --i)
                Push(((src & (1 << i)) == (1 << i) ? true : false));
        }

        public void Push(byte src)
        {
            Push(src, 8);
        }

        public void Push(bool value)
        {
            //if (trace_in_padding) Marshall.User(" PUSH " + (value ? "1" : "0"));
            if (value)
            {
                current_byte |= (byte)(1 << current_bit);
            }
            current_bit--;
            total_bits++;
            if (current_bit != 0xFF) return;
            //if (trace_in_padding) Marshall.User(" PADDING FINISHED ");
            buffer[cursor++] = current_byte;
            current_byte = 0;
            current_bit = 7;
        }

        public byte[] GetBuffer()
        {
            var result = new byte[cursor];
            Array.Copy(buffer,result,cursor);
            return result;
        }

        public bool Pop()
        {
            var result = ((current_byte & (1 << current_bit)) == (1 << current_bit) ? true : false);
            //Marshall.User(" POP " + (result ? "1" : "0"));
            current_bit--;
            total_bits++;
            if (current_bit == 0xFF)
            {
                current_byte = buffer[cursor++];
                current_bit = 7;
            }
            return result;
        }

        public int Pop(int bits) {
            var chr = 0;
            for (var i = (bits -1); i >= 0; --i)
                if (Pop()) chr |= (1 << i);
            return chr;
        }

        public void Pop(ref byte result)
        {
            for (var i = 7; i >= 0; --i)
                if (Pop()) result |= (byte)(1 << i);
        }

        public void Rewind()
        {
            cursor = 0;
            current_byte = buffer[cursor++];
            if (cursor >= buffer.GetLength(0)) throw new Exception("BITSTREAM: buffer overrun.");
            current_bit = 7;
            total_bits = 0;
        }
    }
}