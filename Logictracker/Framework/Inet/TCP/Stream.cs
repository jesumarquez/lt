#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Utils;

#endregion

namespace Logictracker.InetLayer.TCP
{
    /// <summary>
    /// esta clase es la base de las clases ACCEPTOR y Client. ambas representan
    /// una conexion unica TCP. la primera es instanciada automaticamnete por el Listener
    /// lo otra es instanciada por el usuario.
    /// </summary>
    public abstract class Stream
    {
        protected Stream()
        {
            MaxBlockSize = 1024;
            _outgoingStream = new SyncQueue<StreamBlock>(64);
            _outgoingThread = new Thread(OutgoingStreamDispatcher);
            _outgoingThread.Start();
        }

        protected IPEndPoint SourceEndpoint;
        protected internal Socket OsSocket;
        private readonly SyncQueue<StreamBlock> _outgoingStream;
        private readonly Thread _outgoingThread;
        private bool _onceOnDisconnect;
        private bool _onceOnInternalError;
        private int MaxBlockSize { get; set; }
        private int TotalRxBytes { get; set; }
        private int TotalTxBytes { get; set; }
        private bool Broken { get; set; }
        protected abstract void OnReceive(StreamBlock block);
        protected abstract void OnDisconnect();

        private void OnceOnDisconnect()
        {
            Broken = true;
            if (_onceOnDisconnect) return;
            _onceOnDisconnect = true;
            OnDisconnect();
        }

        protected internal void OnceOnInternalError()
        {
            Broken = true;
            if (_onceOnInternalError) return;
            _onceOnInternalError = true;
            Disconnect();
        }

        public void StartReceiver()
        {
            var block = new StreamBlock
            {
                ContentType = StreamBlock.ContentTypes.StreamData,
                Data = new byte[MaxBlockSize],
                TotalBytes = MaxBlockSize
            };
            OsSocket.BeginReceive(block.Data, 0, block.TotalBytes, 0, ReadAsyncResult, block);
        }

        public void Send(byte[] buffer, int bufferSize, int deviceId, String address)
        {
            //STrace.Debug(GetType().FullName, deviceId, String.Format("Send size={0} ip={1} text={2}", buffer_size, address, StringUtils.MakeString(buffer)));
            _outgoingStream.Enqueue(new StreamBlock
            {
                ContentType = StreamBlock.ContentTypes.StreamData,
                Data = buffer,
                TotalBytes = bufferSize
            });
        }

        private void OutgoingStreamDispatcher()
        {
            while (!Broken)
            {
                var block = _outgoingStream.Dequeue(1000);
                if (block == null) continue;

                switch (block.ContentType)
                {
                    case StreamBlock.ContentTypes.StreamData:
                    case StreamBlock.ContentTypes.OobData:
                        //STrace.Debug(GetType().FullName, "DISPATCH {0} BLOCK OF {1} BYTES", block.ContentType == StreamBlock.ContentTypes.OOBData ? "OOB DATA" : "DATA", block.TotalBytes);
                        try
                        {
                            TotalTxBytes += OsSocket.Send(block.Data, 0, block.TotalBytes,
                                                           block.ContentType != StreamBlock.ContentTypes.OobData
                                                               ? SocketFlags.None
                                                               : SocketFlags.OutOfBand);
                        }
                        catch (ObjectDisposedException e)
                        {
                            STrace.Exception(GetType().FullName, e, "SEND, OBJECT DISPOSED");
                        }
                        catch (Exception e)
                        {
                            STrace.Exception(GetType().FullName, e, "SEND, ONCE ON INTERNAL ERROR");
                            OnceOnInternalError();
                        }
                        break;
                    case StreamBlock.ContentTypes.Disconnect:
                        try
                        {
                            STrace.Debug(GetType().FullName, "DISPATCH SHUTDOWN/CLOSE BLOCK");
                            OsSocket.Shutdown(SocketShutdown.Send);
                            OsSocket.Close();
                            Broken = true;
                            return;
                        }
                        catch (ObjectDisposedException) { }
                        break;
                }
            }
        }

        private void ReadAsyncResult(IAsyncResult ar)
        {
            try
            {
                var block = (StreamBlock)ar.AsyncState;
                block.TotalBytes = OsSocket.EndReceive(ar);
                if (block.TotalBytes > 0)
                {
                    block.ContentType = StreamBlock.ContentTypes.StreamData;
                    TotalRxBytes += block.TotalBytes;
                    OnReceive(block);
                    StartReceiver();
                }
                else
                {
                    STrace.Debug(GetType().FullName, String.Format("Se ha desconectado el dispositivo de {0}", SourceEndpoint));
                    OnceOnDisconnect();
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e, "READ ASYNC RESULT");
                OnceOnInternalError();
            }
        }

        protected void Disconnect()
        {
            STrace.Debug(GetType().FullName, "Error al recibir");
            Broken = true;
            if (!OsSocket.Connected) return;
            _outgoingStream.Enqueue(new StreamBlock
            {
                ContentType = StreamBlock.ContentTypes.Disconnect,
                Data = null,
                TotalBytes = 0
            });
        }
    }
}