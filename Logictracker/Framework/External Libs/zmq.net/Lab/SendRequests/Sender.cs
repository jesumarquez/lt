#region Usings

using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml;
using Logictracker.ZeroMQ;

#endregion

namespace SendRequests
{
    class Sender
    {
        private static int recibidos;
        private static int enviados;
        private static int ventana;
        private static double latencia;
        private static int thro_each;
        private static int rwatch_counter;
        private static Stopwatch rwatch;

        static void Main(string[] args)
        {
            IChannel ch = new PipelineChannel();
            thro_each = 10000;
            var in_interface = "tcp://*:8765";        /// recibo las respuestas
            //var out_address = "tcp://127.0.0.1:5678"; /// envio los pedidos
            var out_address = "tcp://10.8.0.1:5678"; /// envio los pedidos
            ch.Setup(out_address, in_interface, 12);
            ch.MessageReceived += ch_MessageReceived;
            ch.Start();

            var send_seq = 0;
            
            Console.Out.WriteLine("Presione una CTRL+BREAK para salir!");

            while (true)
            {
                var xmlmsg = "";
                /*if (ventana > 100)
                {
                    Thread.Sleep(50);
                }
                if (ventana > 2000)
                {
                    while(ventana > 1500) Thread.Sleep(500);
                    continue;
                }*/
                if ((enviados++)%1000 == 0)
                {
                    xmlmsg = "<token t='" + DateTime.Now.Ticks + "'/>";
                } else
                {
                    xmlmsg = "<message label='jelo guor' q='" + (send_seq++) + "'/>";
                    ventana++;
                }
                var msg = Encoding.ASCII.GetBytes(xmlmsg);
                if (!ch.Send(msg))
                {
                    Thread.Sleep(100);
                    continue;
                }                
            }
        }

        
        private static bool ch_MessageReceived(byte[] data)
        {
            //Console.Out.WriteLine(">> ACK");
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
                var ticks = root.Attributes["t"];
                var sent = new DateTime(Convert.ToInt64(ticks.Value));
                var lapse = DateTime.Now - sent;
                latencia = lapse.TotalMilliseconds;
                Console.Out.WriteLine("Latencia: {0}ms enviados={1} recibidos={2}", latencia, enviados, recibidos);
            } else
            {
                recibidos++;
                ventana--;
            }
            if (rwatch_counter == 0)
            {
                rwatch = new Stopwatch();
                rwatch.Start();
            }
            if ((rwatch_counter++) == thro_each)
            {
                rwatch.Stop();
                var elapsedTime = rwatch.ElapsedMilliseconds;
                Console.Out.WriteLine("Rendimiento: {0} mensajes/segundo.",
                                      (((double)rwatch_counter * 1000) / elapsedTime).ToString("0.00"));
                rwatch_counter = 0;
            }
            return true;
        }
    }
}
