using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Urbetrack.DatabaseTracer.Core;

namespace Urbetrack.Net.TCP
{
    public class Listener : IDisposable
    {
        public IPEndPoint ListenAddress { get; private set; }
        public Acceptor usr_acceptor { get; private set; }
        Socket os_socket;
        readonly int pendings;
        
        private string traceTip;
        public String TraceTip
        {
            get { return "TCPLISTENER[" + (String.IsNullOrEmpty(traceTip) ? "?" : traceTip) + "]:"; }
            set
            {
                traceTip = value;
            }
        }
        
        private void TRACE(string format, params object[] args)
        {
            //if (T.ActiveContext.CurrentLevel < 0) return;
            var nf = TraceTip + format;
            STrace.Debug(typeof(Listener).FullName, String.Format(nf, args));
        }

        internal class AcceptState
        {
            internal Socket os_socket;
            internal Acceptor usr_acceptor;
        }

        public Listener(String shortdesc, IPEndPoint listen_address, Acceptor acceptor)
        {
            traceTip = shortdesc;
            usr_acceptor = acceptor;
            ListenAddress = listen_address;
            os_socket = new Socket(AddressFamily.InterNetwork, 
                                   SocketType.Stream, ProtocolType.Tcp);
            pendings = 100;
            os_socket.Bind(ListenAddress);
            os_socket.Listen((pendings == 0 ? 1 : pendings));
            var instance_state = new AcceptState
                                    {
                                        os_socket = os_socket,
                                        usr_acceptor = usr_acceptor
                                    };
            os_socket.BeginAccept(AcceptAsyncResult, instance_state);
        }

        public void Close()
        {
            TRACE("LISTENER CLOSE");
            os_socket.Close();
            os_socket = null;
        }

        private static void AcceptAsyncResult(IAsyncResult ar)
        {
            try
            {
                // aceptamos la conexion 
                var instance_state = (AcceptState)ar.AsyncState;
                var os_socket = instance_state.os_socket;
                Socket usr_socket;
                try
                {
                    usr_socket = os_socket.EndAccept(ar);
                } catch (Exception e)
                {
                    STrace.Exception(typeof(Listener).FullName,e, "TCP.Listener.AcceptAsyncResult=>EndAccept");
                    return;
                } finally
                {
                    if (instance_state.usr_acceptor != null)
                    {
                        var new_instance_state = new AcceptState
                                                     {
                                                         os_socket = os_socket,
                                                         usr_acceptor = (Acceptor)instance_state.usr_acceptor.Clone()
                                                     };
                        os_socket.BeginAccept(AcceptAsyncResult, new_instance_state);
                    } else
                    {
                        STrace.Exception(typeof(Listener).FullName,new ApplicationException("El TCP.Listener perdio la referencia al aceptor EP:" + os_socket.LocalEndPoint));
                    }
                }

				Debug.Assert(instance_state.usr_acceptor != null);
                var instance_acceptor = (Acceptor)instance_state.usr_acceptor.Clone();
                instance_acceptor.os_socket = usr_socket;
                try
                {
                    instance_acceptor.OnConnection();
                    instance_acceptor.StartReceiver();
                    return;
                }
                catch (ObjectDisposedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(Listener).FullName,e, "TCP.Listener.AcceptAsyncResult=>acceptor.OnConnection");
                    instance_acceptor.OnceOnInternalError(); 
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception e)
            {
               STrace.Exception(typeof(Listener).FullName,e, "Listener.OnConnection");
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}