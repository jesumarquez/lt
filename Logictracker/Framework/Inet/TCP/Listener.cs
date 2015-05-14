#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.InetLayer.TCP
{
    public class Listener : IDisposable
    {
        private IPEndPoint ListenAddress { get; set; }
        private Acceptor UsrAcceptor { get; set; }
        Socket _osSocket;
        readonly int _pendings;

        private string _traceTip;
        public String TraceTip
        {
            get { return "TCPLISTENER[" + (String.IsNullOrEmpty(_traceTip) ? "?" : _traceTip) + "]:"; }
            set { _traceTip = value; }
        }

        private class AcceptState
        {
            internal Socket OsSocket;
            internal Acceptor usr_acceptor;
        }

        public Listener(String shortdesc, IPEndPoint listenAddress, Acceptor acceptor)
        {
            _traceTip = shortdesc;
            UsrAcceptor = acceptor;
            ListenAddress = listenAddress;
            _osSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _osSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            _pendings = 100;
            _osSocket.Bind(ListenAddress);
            _osSocket.Listen((_pendings == 0 ? 1 : _pendings));
            var instanceState = new AcceptState
            {
                OsSocket = _osSocket,
                usr_acceptor = UsrAcceptor
            };
            _osSocket.BeginAccept(AcceptAsyncResult, instanceState);
        }

        public void Close()
        {
            _osSocket.Close();
            _osSocket = null;
        }

        private static void AcceptAsyncResult(IAsyncResult ar)
        {
            try
            {
                // aceptamos la conexion 
                var instanceState = (AcceptState)ar.AsyncState;
                var osSocket = instanceState.OsSocket;
                Socket usrSocket;
                try
                {
                    usrSocket = osSocket.EndAccept(ar);
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(Listener).FullName, e, "TCP.Listener.AcceptAsyncResult=>EndAccept");
                    return;
                }
                finally
                {
                    if (instanceState.usr_acceptor != null)
                    {
                        var newInstanceState = new AcceptState
                        {
                            OsSocket = osSocket,
                            usr_acceptor = (Acceptor)instanceState.usr_acceptor.Clone()
                        };
                        osSocket.BeginAccept(AcceptAsyncResult, newInstanceState);
                    }
                    else
                    {
                        STrace.Exception(typeof(Listener).FullName, new ApplicationException("El TCP.Listener perdio la referencia al aceptor EP:" + osSocket.LocalEndPoint));
                    }
                }

                if (instanceState.usr_acceptor != null)
                {
                    var instanceAcceptor = (Acceptor)instanceState.usr_acceptor.Clone();
                    instanceAcceptor.OsSocket = usrSocket;
                    try
                    {
                        instanceAcceptor.OnConnection();
                        instanceAcceptor.StartReceiver();
                    }
                    catch (ObjectDisposedException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        STrace.Exception(typeof(Listener).FullName, e, "TCP.Listener.AcceptAsyncResult=>acceptor.OnConnection");
                        instanceAcceptor.OnceOnInternalError();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Listener).FullName, e, "Listener.OnConnection");
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}