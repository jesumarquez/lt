#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using Urbetrack.Mobile.Toolkit;

#endregion

namespace Urbetrack.Mobile.Net.TCP
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
            os_socket = new Socket(AddressFamily.InterNetwork,
                                   SocketType.Stream, ProtocolType.Tcp);
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
                if (e.ErrorCode == 10061)
                {
                    TRACE("Connetion Refused");
                    return false;
                }
                if (e.ErrorCode == 10060)
                {
                    TRACE("Connetion Timeout");
                    return false;
                }
                T.EXCEPTION(e, "CLIENT CONNECT (Socket Exception)");
                return false;
            }
            catch (Exception e)
            {
                T.EXCEPTION(e, "CLIENT CONNECT");
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
                    T.EXCEPTION(e, "CONNECT ASYNC RESULT/START RECEIVER");
                    OnceOnInternalError();
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10061)
                {
                    TRACE("Connetion Refused");
                    OnRejected();
                }
                if (e.ErrorCode == 10060)
                {
                    TRACE("Connetion Timeout");
                    OnRejected();
                }
                T.EXCEPTION(e, "CLIENT CONNECT (Socket Exception)");
                OnceOnInternalError();
            }
            catch (Exception e)
            {
                T.EXCEPTION(e, "CONNECT ASYNC RESULT/REJECT");
                OnceOnInternalError();
            }
        }
    }
}