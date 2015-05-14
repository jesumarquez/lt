using System;
using System.IO;

namespace Urbetrack.InterQ.Core.Protocol
{
    public class Chunk
    {
        public enum States
        {
            WAITING_HEADERS,
            RECEIVING,
            TERMINATED
        };

        public const int ReceiveBlockSize = 1024;
        public int Size { get; private set; }
        public int Received { get; private set; }
        public int Remaining { get { return Math.Max(0, Size - Received); } }
        
        private States state;
        public States State
        {
            get
            {
                if (state == States.RECEIVING && Remaining == 0) return States.TERMINATED;
                return state;
            }
            private set { state = value; }
        }

        public Version Version { get; private set; }
        private byte[] data_buffer;
        
        public Chunk(Version Version)
        {
            this.Version = Version;
            Size = 0;
            State = States.WAITING_HEADERS;
        }

        public int GetActiveBufferSize()
        {
            return State == States.WAITING_HEADERS ? 2 : Math.Min(256, Remaining);
        }

        public bool ReceiveData(byte[] buffer, int size)
        {
            if (State == States.WAITING_HEADERS)
            {
                Size = (short)(buffer[0] << 8 | buffer[1]);
                if (Size == 0) throw new Exception("Size Invalida para construir un CHUNK");
                data_buffer = new byte[Size];
                State = States.RECEIVING;
                return false;
            }
            if (size > ReceiveBlockSize) throw new Exception("CHUNK, recibio un bloque mayor a lo permitido.");
            Array.Copy(buffer, 0, data_buffer, Received, size);
            Received += size;
            return Remaining == 0;
        }

        public Stream GetStream()
        {
            if (State != States.TERMINATED) throw new Exception("CHUNK, el bloque no se termino de recibir.");
            var strm = new MemoryStream(data_buffer, 0, Size);
            strm.Seek(0, 0);
            return strm;
        }
    }
}
