#region Usings

using System;
using System.Net.Sockets;
using System.Text;
using Urbetrack.Mobile.Toolkit;

#endregion

namespace Urbetrack.Mobile.Net.TCP
{
    /// <summary>
    /// esta clase es la base de las clases ACCEPTOR y Client. ambas representan
    /// una conexion unica TCP. la primera es instanciada automaticamnete por el Listener
    /// lo otra es instanciada por el usuario.
    /// </summary>
    public abstract class Stream
    {
        protected internal Socket os_socket;
        private readonly SyncQueue<StreamBlock> outgoing_stream;
        public static int StreamTraceLevel { get; set; }

        protected Stream()
        {
            MaxBlockSize = 1024;
            outgoing_stream = new SyncQueue<StreamBlock>(64);
        }

        public int MaxBlockSize { get; set; }

        /// <returns>cantidad total de bytes leidos desde la conexion.</returns>
        public int TotalRXBytes { get; internal set; }

        /// <returns>cantidad total de bytes escritos desde la conexion.</returns>
        public int TotalTXBytes { get; internal set; }

        /// <summary>
        /// registra detalles de los acontecimientos dentro del acceptor.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        internal protected abstract void TRACE(string format, params object[] args);

        /// <summary>
        /// callback, este metodo es llamado cuando se reciben datos por el socket.
        /// </summary>
        public abstract void OnReceive(StreamBlock block);

        /// <summary>
        /// callback, este metodo es llamado por el framework cuando se desconecta
        /// el socket desde el otro extremo de la conexion.
        /// </summary>
        public abstract void OnDisconnect();

        private bool onceOnDisconnect;
        protected void OnceOnDisconnect()
        {
            if (onceOnDisconnect) return;
            onceOnDisconnect = true;
            TRACE("ON DISCONNECT");
            OnDisconnect();
        }

        /// <summary>
        /// Lanzado cuando se produce un excepcion internamente,
        /// si connected es false, la excepcion implica que la conexion
        /// fue terminada y en tal caso OnDisconnect no se lanza.
        /// </summary>
        public abstract void OnInternalError();

        private bool onceOnInternalError;
        protected internal void OnceOnInternalError()
        {
            if (onceOnInternalError) return;
            onceOnInternalError = true;
            TRACE("ON INTERNAL ERROR");
            OnInternalError();
        }

        /// <summary>
        /// envia datos al peer, se debe indicar la cantidad de bytes que es necesario
        /// enviar del buffer provisto.
        /// </summary>
        /// <param name="buffer">buffer que contiene los datos a ser enviados.</param>
        /// <param name="buffer_size">cantidad de bytes que se requiere enviar de dicho buffer.</param>
        public virtual void Send(byte[] buffer, int buffer_size)
        {
            EnqueueBlock(new StreamBlock
                             {
                                 ContentType = StreamBlock.ContentTypes.StreamData,
                                 Data = buffer,
                                 TotalBytes = buffer_size
                             });
        }

        public virtual void Send(string s)
        {
            Send(Encoding.ASCII.GetBytes(s.ToCharArray(), 0, s.Length), s.Length);
        }

        public virtual void EnqueueBlock(StreamBlock block)
        {
            TRACE("ENQUEUE BLOCK [{0}]", block.ContentType);
            if (outgoing_stream.Enqueue(block))
            {
                DispatchBlock(block);
            }
        }

        public void StartReceiver()
        {
            TRACE("START RECEIVER");
            var block = new StreamBlock
                            {
                                ContentType = StreamBlock.ContentTypes.StreamData,
                                Data = new byte[MaxBlockSize],
                                TotalBytes = MaxBlockSize
                            };
            os_socket.BeginReceive(block.Data, 0, block.TotalBytes, 0, ReadAsyncResult, block);
        }

        private void DispatchBlock(StreamBlock block)
        {
            TRACE("DISPATCH BLOCK");
            if (block == null)
            {
                block = outgoing_stream.Peek();
                if (block == null) return;
            }
            switch (block.ContentType)
            {
                case StreamBlock.ContentTypes.StreamData:
                    os_socket.BeginSend(block.Data, 0, block.TotalBytes, SocketFlags.None,
                                        SendAsyncResult, block);
                    break;
                    /*case StreamBlock.ContentTypes.OOBData:
                    os_socket.BeginSend(block.Data, 0, block.TotalBytes, SocketFlags.OutOfBand,
                                        new AsyncCallback(SendAsyncResult), os_socket);
                    break;*/
                case StreamBlock.ContentTypes.Disconnect:
                    try
                    {
                        os_socket.Shutdown(SocketShutdown.Send);
                        os_socket.Close();
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                    break;
            }
        }

        private void SendAsyncResult(IAsyncResult ar)
        {
            try {
                var sent = os_socket.EndSend(ar);
                outgoing_stream.Dequeue();
                TotalTXBytes += sent;
                TRACE("SEND ASYNC RESULT {0}/{1}", sent, TotalTXBytes);
                DispatchBlock(null);
                
            } catch (ObjectDisposedException e)
            {
                T.EXCEPTION(e, "SEND ASYNC RESULT, OBJECT DISPOSED");
            }
            catch (Exception e)
            {
                T.EXCEPTION(e, "SEND ASYNC RESULT, ONCE ON INTERNAL ERROR");
                OnceOnInternalError();
            }
        }

        private void ReadAsyncResult(IAsyncResult ar)
        {
            try
            {
                var block = (StreamBlock)ar.AsyncState;
                block.TotalBytes = os_socket.EndReceive(ar);
                if (block.TotalBytes > 0)
                {
                    block.ContentType = StreamBlock.ContentTypes.StreamData;
                    TotalRXBytes += block.TotalBytes;
                    TRACE("READ ASYNC RESULT {0}/{1}", block.TotalBytes, TotalRXBytes);
                    OnReceive(block);
                    StartReceiver();
                }
                else
                {
                    TRACE("READ ASYNC, ON DISCONNECT");
                    OnceOnDisconnect();
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception e)
            {
                T.EXCEPTION(e, "READ ASYNC RESULT");
                OnceOnInternalError();
            }
        }

        /// <summary>
        /// este metodo cierra ordenadamente la conexion. luego de ejecutado este metodo
        /// intentar ejecutar Send lanzara una excepcion.
        /// </summary>
        public void Disconnect()
        {
            if (!os_socket.Connected) return;
            EnqueueBlock(new StreamBlock
                             {
                                 ContentType = StreamBlock.ContentTypes.Disconnect,
                                 Data = null,
                                 TotalBytes = 0
                             });
        }
    }
}