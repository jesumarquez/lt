using System;
using System.Configuration;
using System.Diagnostics;
using System.Messaging;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using etao.marshall;

namespace avl2cmd
{
    partial class AVL2CMD : ServiceBase, ILogger
    {
        // pattern singleton
        public static AVL2CMD _instance = null;

        cmd_acceptor _cmd_a;
        MessageQueue input = new MessageQueue(ConfigurationManager.AppSettings["in_queue"]);
        EventLog el;
        Thread t;
        Semaphore allDone = new Semaphore(0,1);
        bool request_in_progress;
        
        public AVL2CMD()
        {
            InitializeComponent();
            request_in_progress = false;
            _instance = this;
            if (!EventLog.SourceExists("AVL2CMD"))
            {
                EventLog.CreateEventSource("AVL2CMD", "Application");
            }
            el = new EventLog();
            el.Source = "AVL2CMD";
            // Write an informational entry to the event log.    
            cmd_acceptor.SETLOG(this);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 6677);
            tcp_listener<cmd_acceptor> server = new tcp_listener<cmd_acceptor>(ep, 0);
            input.Formatter = new BinaryMessageFormatter();
            t = new Thread(new ThreadStart(ThreadProc));
            t.Start();
            el.WriteEntry("software startup.");
        }

        public void ack(bool acked)
        {
            if (request_in_progress)
            {
                if (!acked)
                {
                    log("Se ha recibido un NACK, pausa 1s y reintentando...");
                    Thread.Sleep(1000);
                }
                else
                {
                    log("Se ha recibido un ACK! un estado ha cambiado!");
                    input.Receive(); // volamos el msg de la cola 
                }
                // incremento el contador.
                allDone.Release();
                log("Se libero el semaforo!");
            }
            else
            {
                log("Se ha recibido algo y se ignora!");
            }
        }

        public static void ThreadProc()
        {
            _instance.el.WriteEntry("Inicia bucle de la message queue!");
            while (true)
            {
                try
                {
                    Message request = _instance.input.Peek();
                    // mecanismo de salida del thread.
                    if (request.Label.StartsWith("close")) return;
                    _instance.el.WriteEntry(String.Format("se recibio un mensaje MQ {0}", request.Label));
                    // Label="1121,9,123412341234"
                    // 1 => camion
                    // 2 => nuevo estado
                    // 3 => hora desde epoch
                    if (_instance._cmd_a == null)
                    {
                        _instance.el.WriteEntry("NO SE PUEDE PROCESAR MSG, NO ESTAMOS CONECTADOS!");
                        Thread.Sleep(1000);
                        continue;
                    }
                    string[] fields = request.Label.Split(',');
                    if (fields.Length != 3)
                    {
                        _instance.el.WriteEntry("NO SE PUDO PROCESAR POR ERROR DE FTO: LABEL='" + request.Label + "'");
                        _instance.input.Receive(); // finalmente lo desencolo.
                        continue;
                    }
                    _instance.el.WriteEntry("SE ENVIA SOLICITUD: LABEL='" + request.Label + "'");
                    _instance.request_in_progress = true;
                    DateTime original = new DateTime(Convert.ToInt64(fields[2]));
                    int segundos_afuera = (int) DateTime.Now.Subtract(original).TotalSeconds;
                    _instance._cmd_a.send(CMDPDU.encode_estado(Convert.ToInt32(fields[0]),Convert.ToInt32(fields[1]),segundos_afuera == 0 ? 0 : (segundos_afuera/60)+1));
                    // este semaforo senializa al recebir el ACK                    
                    _instance.log("Se esperara el semaforo!");
                    _instance.allDone.WaitOne();
                    _instance.log("Request Terminada!");
                    _instance.request_in_progress = false;
                }
                catch (Exception ex)
                {
                    _instance.el.WriteEntry("Error al procesar un mensaje de la cola entrante.");
                    _instance.el.WriteEntry("OS Error:" + ex.Message + "\n" + ex.StackTrace);
                    Thread.Sleep(1000);
                }
            }
        }

        public void log(string s)
        {
            el.WriteEntry(s);
        }

        public void take(cmd_acceptor a)
        {
            log("se ha seteado un command acceptor");
            _cmd_a = a;
            if (request_in_progress)
            {
                log("Se ha reseteado una conexion, reintentando...");
                // incremento el contador.
                allDone.Release();
                log("Se libero el semaforo!");
            }
        }

        protected override void OnStart(string[] args)
        {
            log("Service On Start Launched");
        }

        protected override void OnStop()
        {
            log("Service On Stop Launched");
        }
    }
}
