#region Usings

using System;
using System.Net;
using System.Runtime.InteropServices;

#endregion

namespace Logictracker.ZeroMQ
{
    public class Socket : IDisposable
    {
        private IntPtr ptr;
        private IntPtr msg;
        private const int EAGAIN = 11;
        private const int ZMQ_MAX_VSM_SIZE = 30;
        private readonly int ZMQ_MSG_T_SIZE = IntPtr.Size + 2 + ZMQ_MAX_VSM_SIZE;

        //  Don't call this, call Context.CreateSocket
        public Socket(IntPtr ptr)
        {
            this.ptr = ptr;
            msg = Marshal.AllocHGlobal(ZMQ_MSG_T_SIZE);
        }

        ~Socket()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (msg != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(msg);
                msg = IntPtr.Zero;
            }

            if (ptr == IntPtr.Zero) return;
            var rc = ZmqMarshal.C.zmq_close(ptr);
            ptr = IntPtr.Zero;
            if (rc != 0)
                throw new Exception();
        }

        public void SetSockOpt(int option, string value)
        {
            if (ZmqMarshal.C.zmq_setsockopt(ptr, option, value, value.Length) != 0)
                throw new Exception();
        }

        public void Bind(string addr)
        {
            if (ZmqMarshal.C.zmq_bind(ptr, addr) != 0)
                throw new Exception();
        }

        public void Connect(string addr)
        {
            if (ZmqMarshal.C.zmq_connect(ptr, addr) != 0)
                throw new Exception();
        }

        public bool Recv(out byte[] message)
        {
            return Recv(out message, 0);
        }

        public bool Recv(out byte[] message, int flags)
        {
            if (ZmqMarshal.C.zmq_msg_init(msg) != 0)
                throw new Exception();
            var rc = ZmqMarshal.C.zmq_recv(ptr, msg, flags);
            if (rc == 0)
            {
                message = new byte[ZmqMarshal.C.zmq_msg_size(msg)];
                Marshal.Copy(ZmqMarshal.C.zmq_msg_data(msg), message, 0, message.Length);
                ZmqMarshal.C.zmq_msg_close(msg);
                return true;
            }
            if (ZmqMarshal.C.zmq_errno() == EAGAIN)
            {
                message = new byte[0];
                return false;
            }
            throw new Exception();
        }

        public bool Send(byte[] message)
        {
            return Send(message, 0);
        }

        public bool Send(byte[] message, int flags)
        {
            if (ZmqMarshal.C.zmq_msg_init_size(msg, message.Length) != 0)
                throw new Exception();
            Marshal.Copy(message, 0, ZmqMarshal.C.zmq_msg_data(msg), message.Length);
            var rc = ZmqMarshal.C.zmq_send(ptr, msg, flags);
            //  No need for zmq_msg_close here as the message is empty anyway.
            if (rc == 0)
                return true;
            if (ZmqMarshal.C.zmq_errno() == EAGAIN)
                return false;
            throw new Exception();
        }

        public static string ConnectUri(IPEndPoint ep)
        {
            return String.Format("tcp://{0}:{1}", ep.Address, ep.Port);
        }

        public static string BindUri(IPEndPoint ep)
        {
            return String.Format("tcp://*:{0}",ep.Port);
        }
    }
}