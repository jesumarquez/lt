#region Usings

using System.Runtime.InteropServices;

#endregion

namespace Logictracker.ZeroMQ
{
    public sealed class Exception : System.Exception
    {
        private readonly int errno;

        public int Errno
        {
            get { return errno; }
        }

        public Exception(string source)
            : base(Marshal.PtrToStringAnsi(ZmqMarshal.C.zmq_strerror(ZmqMarshal.C.zmq_errno())))
        {
            Source = source;
            errno = ZmqMarshal.C.zmq_errno();
        }

        public Exception()
            : base(Marshal.PtrToStringAnsi(ZmqMarshal.C.zmq_strerror(ZmqMarshal.C.zmq_errno())))
        {
            errno = ZmqMarshal.C.zmq_errno();
        }
    }
}