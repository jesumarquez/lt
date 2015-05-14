#region Usings

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Urbetrack.Utils;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Net.TCP
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
        private readonly Thread outgoing_thread;

        protected Stream()
        {
            MaxBlockSize = 1024;
            outgoing_stream = new SyncQueue<StreamBlock>(64);
            outgoing_thread = new Thread(OutgoingStreamDispatcher);
            outgoing_thread.Start();
        }

        public int MaxBlockSize { get; set; }

        /// <returns>cantidad total de bytes leidos desde la conexion.</returns>
        public int TotalRXBytes { get; internal set; }

        /// <returns>cantidad total de bytes escritos desde la conexion.</returns>
        public int TotalTXBytes { get; internal set; }

        public virtual bool Broken { get; protected set; }

        /// <summary>
        /// registra detalles de los acontecimientos dentro del acceptor.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public abstract void TRACE(string format, params object[] args);

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
            Broken = true;
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
            Broken = true;
            if (onceOnInternalError) return;
            onceOnInternalError = true;
            TRACE("ON INTERNAL ERROR");
            OnInternalError();
        }

        protected bool receiver_running;
        public void StartReceiver()
        {
            TRACE("START RECEIVER");
            receiver_running = true;
            var block = new StreamBlock
            {
                ContentType = StreamBlock.ContentTypes.StreamData,
                Data = new byte[MaxBlockSize],
                TotalBytes = MaxBlockSize
            };
            os_socket.BeginReceive(block.Data, 0, block.TotalBytes, 0, ReadAsyncResult, block);
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
            TRACE("ENQUEUE BLOCK [{0}] OF {1} BYTES", block.ContentType, block.TotalBytes);
            outgoing_stream.Enqueue(block);
        }

        private void OutgoingStreamDispatcher()
        {
            while (!Broken)
            {
                var block = outgoing_stream.Dequeue(1000);
                if (block == null)
                {
                    //TRACE("DISPATCH CONFORT LOOP");
                    continue;
                }
                switch (block.ContentType)
                {
                    case StreamBlock.ContentTypes.StreamData:
                    case StreamBlock.ContentTypes.OOBData:
                        TRACE("DISPATCH {0} BLOCK OF {1} BYTES", block.ContentType != StreamBlock.ContentTypes.OOBData ? "OOB DATA" : "DATA", block.TotalBytes);
                        try
                        {
                            TotalTXBytes += os_socket.Send(block.Data, 0, block.TotalBytes,
                                                           block.ContentType != StreamBlock.ContentTypes.OOBData
                                                               ? SocketFlags.None
                                                               : SocketFlags.OutOfBand);
                        }
                        catch (ObjectDisposedException e)
                        {
                            STrace.Exception(GetType().FullName,e, "SEND, OBJECT DISPOSED");
                        }
                        catch (Exception e)
                        {
                            STrace.Exception(GetType().FullName,e, "SEND, ONCE ON INTERNAL ERROR");
                            OnceOnInternalError();
                        }
                        break;
                    case StreamBlock.ContentTypes.Disconnect: 
                        try
                        {
                            TRACE("DISPATCH SHUTDOWN/CLOSE BLOCK");
                            os_socket.Shutdown(SocketShutdown.Send);
                            os_socket.Close();
                            Broken = true;
                            return;
                        } catch (ObjectDisposedException)
                        {
                            
                        }
                        break;
                }
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
                STrace.Exception(GetType().FullName,e, "READ ASYNC RESULT");
                OnceOnInternalError();
            }
        }

        /// <summary>
        /// este metodo cierra ordenadamente la conexion. luego de ejecutado este metodo
        /// intentar ejecutar Send lanzara una excepcion.
        /// </summary>
        public virtual void Disconnect()
        {
            Broken = true;
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