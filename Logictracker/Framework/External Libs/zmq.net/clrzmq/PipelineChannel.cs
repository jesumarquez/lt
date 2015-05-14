#region Usings

using System.Threading;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.ZeroMQ
{
    public class PipelineChannel : IChannel
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
            // canal de acks.
            Receiver = Context.Socket(ZmqMarshal.UPSTREAM);
            Receiver.Bind(LocalUri);

            Sender = Context.Socket(ZmqMarshal.DOWNSTREAM);
            Sender.Connect(RemoteUri);

            recevier_thread = new Thread(ReceiverMainLoop);
            Running = true;
            recevier_thread.Start();
            return true;
        }

        public void Stop()
        {
            Context.Dispose();
            Running = false;
        }

        public bool Send(byte[] frame)
        {
            return Sender.Send(frame, ZmqMarshal.NOBLOCK);
        }

        private void ReceiverMainLoop()
        {
            try
            {
                while (Running)
                {
                    byte[] msg;
                    while (!Receiver.Recv(out msg, ZmqMarshal.NOBLOCK))
                    {
                        Thread.Sleep(250);
                    }
                    if (MessageReceived != null)
                           MessageReceived(msg);
                }
            } catch (Exception e)
            {
                if (e.Errno == ZmqMarshal.ETERM)
                {
                    STrace.Debug(GetType().FullName,"PipelineChannel Reader Terminated.");
                    return;
                }
                STrace.Exception(GetType().FullName,e);
            }
            catch (System.Exception e)
            {
                STrace.Exception(GetType().FullName,e);
            }
        }
    }
}