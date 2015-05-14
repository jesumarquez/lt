#region Usings

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml;
using Logictracker.ZeroMQ;

#endregion

namespace ReceiveReplies
{
    class Receiver
    {
        private static int recibidos;
        private static Stopwatch rwatch;
        private static IChannel ch;
        
        static void Main()
        {
            ch = new PipelineChannel();
            var in_interface = "tcp://*:5678";         /// recibo mensajes
            // var out_address = "tcp://127.0.0.1:8765"; /// envio las respuestas
            var out_address = "tcp://10.8.0.38:8765"; /// envio las respuestas
            ch.Setup(out_address, in_interface, 12);
            ch.MessageReceived += ch_MessageReceived;
            ch.Start();
            Console.Out.WriteLine("Presione una CTRL+BREAK para salir!");

            while (true)
            {
                Console.ReadKey();
                Console.Out.WriteLine("Presione una CTRL+BREAK para salir!");
            }
        }

        private static bool ch_MessageReceived(byte[] data)
        {
            //Console.Out.WriteLine(">> MSG");
            if (recibidos == 0)
            {
                rwatch = new Stopwatch();
                rwatch.Start();
            }
            var inmsg = Encoding.ASCII.GetString(data);
            var doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\"?>" + inmsg);
            var root = doc.SelectSingleNode("/*");
            if (root == null)
            {
                Console.Out.WriteLine("> Mensaje no XML");
                return false;
            }

            if (root.Name == "token")
            {
                ch.Send(data);
            } else
            {
                var sync = Encoding.ASCII.GetBytes("<message-ack q='" + root.Attributes["s"] + "'/>");
                ch.Send(sync);    
            }

            if (recibidos++ == 10000)
            {
                rwatch.Stop();
                var elapsedTime = rwatch.ElapsedMilliseconds;
                Console.Out.WriteLine("Rendimiento: {0} mensajes/segundo.",
                                      ((double)recibidos * 1000) / elapsedTime);
                recibidos = 0;
            }
            return true;
        }
        static void MainOld(string[] args)
        {
            var in_interface = "tcp://*:5678";         /// recibo mensajes
            var out_address =   "tcp://127.0.0.1:8765"; /// envio las respuestas
            var transactrionCount = 20;
            
            //  Initialise 0MQ infrastructure
            var ctx = new Context(5);
            var inp_socket = ctx.Socket(ZmqMarshal.UPSTREAM);
            var out_socket = ctx.Socket(ZmqMarshal.DOWNSTREAM);
            inp_socket.Bind(in_interface);

            Console.Out.WriteLine("Presione un tecla para comenzar una prueba (de {0} mensajes):", transactrionCount);
            Console.In.Read();
            out_socket.Connect(out_address);

            var count = 0;
            var eagain_msg = 0;
            var eagain_total = 0;
            while (true)
            {
                byte[] msg;
                var eagain = 0;
                while(!inp_socket.Recv(out msg))
                {
                    if (eagain == 0)
                        eagain_msg++;
                    eagain++;
                    eagain_total++;
                    if (eagain % 10 == 0)
                    {
                        Console.Out.WriteLine("WARN: {0} intentos de lectura fueron postergados en el mismo mensaje.", eagain);
                    }
                    Thread.Sleep(1);
                }
                var inmsg = Encoding.ASCII.GetString(msg);
                var doc = new XmlDocument();
                doc.LoadXml("<?xml version=\"1.0\"?>" + inmsg);
                var message = doc.SelectSingleNode("/*");
                if (message == null) 
                {
                    Console.Out.WriteLine("> Mensaje no XML");
                    continue;
                }
                if (count++ % 100 == 0)
                {
                    Console.Out.WriteLine("> Reporte Receptor: Mensajes: total={0} reintentados={1}, Reintentos: {2}", count, eagain_msg, eagain_total);
                }
                // Send the synchronisation messages to 'send_requests' application.
                var sync = Encoding.ASCII.GetBytes("<message-ack s='" + message.Attributes["s"] + "'/>");
                out_socket.Send(sync);
                while (!inp_socket.Send(sync))
                {
                    if (eagain == 0)
                        eagain_msg++;
                    eagain++;
                    eagain_total++;
                    if (eagain % 10 == 0)
                    {
                        Console.Out.WriteLine("WARN: {0} intentos de lectura fueron postergados en el mismo mensaje.", eagain);
                    }
                    Thread.Sleep(1);
                }
            }
        }
    }
}
