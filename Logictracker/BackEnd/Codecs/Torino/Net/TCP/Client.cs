using System;
using System.Net;
using System.Net.Sockets;
using Urbetrack.DatabaseTracer.Core;

namespace Urbetrack.Net.TCP
{
    /// <summary>
    /// Client representa una conexion saliente TCP.
    /// </summary>
    public abstract class Client : Stream
    {
        /// <summary>
        /// constructor completo. este constructor permite definir el tamanio del buffer 
        /// de recepcion.
        /// </summary>
        protected Client()
        {
            os_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public override bool Broken
        {
            get
            {
                if (os_socket == null) return false;
                if (!receiver_running) return false;
                return os_socket.Connected ? false : true;
            }
            protected set
            {
                base.Broken = value;
            }
        }
        
        /// <summary>
        /// callback, este metodo es llamado por el framework cuando se completa la 
        /// conexion asincronica.
        /// </summary>
        public abstract void OnConnect();

        /// <summary>
        /// callback, este metodo es llamado por el framework cuando se completa con falla la 
        /// conexion asincronica.
        /// </summary>
        public abstract void OnRejected();

        /// <summary>
        /// callback, este metodo es llamado por el framework cuando se completa con falla la 
        /// conexion asincronica.
        /// </summary>
        public abstract void OnTimedOut();

        /// <summary>
        /// este metodo inicial la conexion asincronica al destino indicado.
        /// </summary>
        /// <param name="ep">endpoint a donde realizar la conexion.</param>
        /// <param name="sync">indica si la conexion debe hacerse sincronica o asicnronicamente.</param>
        public bool Connect(IPEndPoint ep, bool sync)
        {
            try
            {
                if (sync) {
                    os_socket.Connect(ep);
                    if (os_socket.Connected) {
                        StartReceiver();
                        return true;
                    }
                    return false;
                }
                os_socket.BeginConnect(ep, ConnectAsyncResult, os_socket);
                return true;
            }
            catch (SocketException e)
            {
                Broken = true;
                if (e.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    TRACE("Connetion Refused");
                    return false;
                }
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    TRACE("Connetion Timedout");
                    return false;
                }
                STrace.Exception(GetType().FullName,e, "CLIENT CONNECT (Socket Exception)");
                return false;
            }
            catch (Exception e)
            {
                Broken = true;
                STrace.Exception(GetType().FullName,e, "CLIENT CONNECT");
                return false;
            }            
        }

        /// <summary>
        /// callback interno, este metodo es utilizado internamente para recibir 
        /// el evento de conexion establecida.
        /// </summary>
        /// <param name="ar">parametro de conexion asincronica provisto por .NET FW</param>
        private void ConnectAsyncResult(IAsyncResult ar)
        {
            try
            {                
                var client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                OnConnect();
                try
                {
                    StartReceiver();
                } catch (Exception e)
                {
                    STrace.Exception(GetType().FullName,e, "CONNECT ASYNC RESULT/START RECEIVER");
                    OnceOnInternalError();
                }
            }
            catch (SocketException e)
            {
                Broken = true;
                if (e.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    TRACE("Connetion Refused");
                    OnRejected();
                    return;
                }
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    TRACE("Connetion Timeout");
                    OnTimedOut();
                    return;
                }
                STrace.Exception(GetType().FullName,e, "CLIENT CONNECT (Socket Exception)");
                OnceOnInternalError();
            }
            catch (Exception e)
            {
                Broken = true;
                STrace.Exception(GetType().FullName,e, "CONNECT ASYNC RESULT/REJECT");
                OnceOnInternalError();
            }
        }
    }
}