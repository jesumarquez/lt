#region Usings

using System;
using System.Runtime.InteropServices;

#endregion

namespace Logictracker.ZeroMQ
{

    public class ZmqMarshal
    {
        internal class C
        {
            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr zmq_init(int io_threads);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_term(IntPtr context);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_close(IntPtr socket);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_setsockopt(IntPtr socket, int option, IntPtr optval, int optvallen);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_setsockopt(IntPtr socket, int option, string optval, int optvallen);

            [DllImport("libzmq", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_bind(IntPtr socket, string addr);

            [DllImport("libzmq", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_connect(IntPtr socket, string addr);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_recv(IntPtr socket, IntPtr msg, int flags);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_send(IntPtr socket, IntPtr msg, int flags);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr zmq_socket(IntPtr context, int type);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_msg_close(IntPtr msg);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr zmq_msg_data(IntPtr msg);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_msg_init(IntPtr msg);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_msg_init_size(IntPtr msg, int size);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_msg_size(IntPtr msg);

            [DllImport("libzmq", CallingConvention = CallingConvention.Cdecl)]
            public static extern int zmq_errno();

            [DllImport("libzmq", CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr zmq_strerror(int errnum);
        }

        public const int HWM = 1;
        public const int SWAP = 3;
        public const int AFFINITY = 4;
        public const int IDENTITY = 5;
        public const int SUBSCRIBE = 6;
        public const int UNSUBSCRIBE = 7;
        public const int RATE = 8;
        public const int RECOVERY_IVL = 9;
        public const int MCAST_LOOP = 10;
        public const int SNDBUF = 11;
        public const int RCVBUF = 12;

        public const int PAIR = 0;
        public const int PUB = 1;
        public const int SUB = 2;
        public const int REQ = 3;
        public const int REP = 4;
        public const int XREQ = 5;
        public const int XREP = 6;
        public const int UPSTREAM = 7;
        public const int DOWNSTREAM = 8;

        public const int NOBLOCK = 1;

        public const int ZMQ_HAUSNUMERO = 156384712;
        public const int ETERM = (ZMQ_HAUSNUMERO + 53);


    }
}