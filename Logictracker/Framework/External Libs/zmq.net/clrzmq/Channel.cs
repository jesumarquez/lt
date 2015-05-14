#region Usings

using System;
using System.Threading;
using Exception = Logictracker.ZeroMQ.Exception;

#endregion

namespace Logictracker.ZeroMQ
{
    public delegate bool MessageReceivedHandler(byte[] data);
    public delegate byte[] RequestReceivedHandler(byte[] data);

    public interface IChannel
    {
        event MessageReceivedHandler MessageReceived;
        void Setup(string PeerUri, string SelfUri, int Threads);
        void Setup(string PeerUri, string SelfUri, Context context);
        bool Start();
        void Stop();
        bool Send(byte[] frame);
    }

    public class Channel : IChannel
    {
        public event MessageReceivedHandler MessageReceived;

        private Context Context;
        private string RemoteUri;
        private string LocalUri;
        private Thread recevier_thread;
        private bool Running;

        // sockets de canal saliente.
        private Socket Sender;              // envio paquetes...
        // sockets de canal entrante.
        private Socket Receiver;            // recibo paquetes...

        public void Setup(string PeerUri, string SelfUri, int Threads)
        {
            var context = new Context(Threads);
            Setup(PeerUri, SelfUri, context);
        }

        public void Setup(string PeerUri, string SelfUri, Context context)
        {
            Context = context;
            RemoteUri = PeerUri;
            LocalUri = SelfUri;
        }

        public bool Start()
        {
            if (Context == null)
            {
                return false;
            }
            /// canal de acks.
            Receiver = Context.Socket(ZmqMarshal.SUB);
            Receiver.Bind(LocalUri);

            Sender = Context.Socket(ZmqMarshal.PUB);
            Sender.Connect(RemoteUri);

            recevier_thread = new Thread(ReceiverMainLoop);
            Running = true;
            recevier_thread.Start();
            return true;
        }

        public void Stop()
        {
            Running = false;
        }

        public bool Send(byte[] frame)
        {
            return Sender.Send(frame);
        }

        private void ReceiverMainLoop()
        {
            try
            {
                while (Running)
                {
                    byte[] msg;
                    var eagain = 0;
                    while (!Receiver.Recv(out msg))
                    {
                        eagain++;
                        if (eagain%10 == 0)
                        {
                            Console.Out.WriteLine("WARN: {0} intentos de lectura fueron postergados en el mismo mensaje.", eagain);
                        }
                        Thread.Sleep(1);
                    }
                    if (MessageReceived != null)
                           MessageReceived(msg);
                }
            } catch (Exception e)
            {
                Console.Out.WriteLine(" ----- EXCEPTION --------- ");
                Console.Out.WriteLine(e);
            }
        }
    }
}