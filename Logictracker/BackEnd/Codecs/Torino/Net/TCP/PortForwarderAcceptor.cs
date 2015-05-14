using System;
using System.Net;
using Urbetrack.DatabaseTracer.Core;

namespace Urbetrack.Net.TCP
{
    internal class PortForwarderAcceptor : Acceptor
    {
        internal class PortForwarderClient : Client
        {
            internal PortForwarderAcceptor Source;
            internal IPEndPoint TargetAddress;


            public PortForwarderClient(IPEndPoint targetAddress, PortForwarderAcceptor acceptor)
            {
                TargetAddress = targetAddress;
                Source = acceptor;
            }

            public bool ConnectSync()
            {
                return Connect(TargetAddress, true);
            }

            public override void OnConnect()
            {
                TRACE("CLIENT ON CONNECT");
            }

            public override void OnRejected()
            {
                TRACE("CLIENT ON REJECTED");
                Source.Disconnect();
            }

            public override void OnTimedOut()
            {
                TRACE("CLIENT ON TIMEDOUT");
                Source.Disconnect();
            }

            public override void OnReceive(StreamBlock block)
            {
                TRACE("CLIENT ON RECEIVE");
                Source.EnqueueBlock(block);
            }

            public override void TRACE(string format, params object[] args)
            {
                //if (T.ActiveContext.CurrentLevel < Source.FileTransferTraceLevel) return;
                //var nf = "CLIENT[" + T.LOCALENDPOINT(os_socket) + "/" + T.REMOTEENDPOINT(os_socket) + "]:" + format;
                STrace.Debug(GetType().FullName, String.Format(format, args));
            }

            public override void OnDisconnect()
            {
                TRACE("CLIENT ON DISCONNECT");
                Source.Disconnect();
            }

            public override void OnInternalError()
            {
                TRACE("CLIENT ON INTERNAL ERROR");
                Source.Disconnect();
            }
        }

        private readonly IPEndPoint ListenAddress;
        private readonly IPEndPoint TargetAddress;
        public int FileTransferTraceLevel;
        private readonly PortForwarderClient Target;

        public PortForwarderAcceptor(IPEndPoint listenAddress, IPEndPoint targetAddress)
        {
            ListenAddress = listenAddress;
            TargetAddress = targetAddress;
            Target = new PortForwarderClient(TargetAddress, this);
        }

        public override void TRACE(String format, params object[] args)
        {
            //if (T.ActiveContext.CurrentLevel < FileTransferTraceLevel) return;
            //var nf = "SERVER[" + T.LOCALENDPOINT(os_socket) + "/" + T.REMOTEENDPOINT(os_socket) + "]:" + format;
            STrace.Debug(GetType().FullName, String.Format(format, args));
        }

        public override void OnConnection()
        {
            if (!Target.ConnectSync())
            {
                TRACE("CONEXION FALLIDA");
                Disconnect();
                return;
            }
            TRACE("CONNECTED TO [{0}]", TargetAddress);
        }

        public override void OnReceive(StreamBlock block)
        {
            TRACE("SERVER ON RECEIVE");
            Target.EnqueueBlock(block);
        }

        public override void OnDisconnect()
        {
            TRACE("SERVER ON DISCONNECT");
            Target.Disconnect();
        }

        public override void OnInternalError()
        {
            TRACE("SERVER ON INTERNAL ERROR");
            Target.Disconnect();
        }

        public override object Clone()
        {
            return new PortForwarderAcceptor(ListenAddress, TargetAddress)
                       {
                           FileTransferTraceLevel = FileTransferTraceLevel
                       };
        }
    }
}
