#region Usings

using System;
using Exception = Logictracker.ZeroMQ.Exception;

#endregion

namespace Logictracker.ZeroMQ
{
    public class Context : IDisposable
    {
        private IntPtr ptr;

        public Context(int io_threads)
        {
            ptr = ZmqMarshal.C.zmq_init(io_threads);
            if (ptr == IntPtr.Zero)
                throw new Exception();
        }

        ~Context()
        {
            Dispose(false);
        }

        public Socket Socket(int type)
        {
            var socket_ptr = ZmqMarshal.C.zmq_socket(ptr, type);
            if (ptr == IntPtr.Zero)
                throw new Exception();

            return new Socket(socket_ptr);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (ptr == IntPtr.Zero) return;
            var rc = ZmqMarshal.C.zmq_term(ptr);
            ptr = IntPtr.Zero;
            if (rc != 0)
                throw new Exception();
        }
    }
}