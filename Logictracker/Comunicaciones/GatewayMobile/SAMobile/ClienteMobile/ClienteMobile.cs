using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using SAMobile.Queuing;
using Urbetrack.Mobile.Comm.GEO;
using Urbetrack.Mobile.Comm.Queuing;
using Urbetrack.Mobile.Comm.Transport;
using Urbetrack.Mobile.Comm.Transport.FileTransfer;
using Urbetrack.Mobile.Toolkit;
using Win32;

namespace Urbetrack.Mobile.Comm.ClienteMobile
{
    public class ClienteMobile
    {
        public enum EstadoCliente
        {
            NETWORK_FAILURE,
            TRY_ONLINE,
            ONLINE,
            TIME_WAIT
        }

        public delegate void StackFailureEvent();
        public event StackFailureEvent StackFailure;

        public Queues queues = new Queues();

        private EstadoCliente estado;
        private const int timewait_timeout = 5;
        private int timewait_left = timewait_timeout;

        public Timer TickTimer
        {
            get { return tickTimer; }
        }

        public EstadoCliente Estado
        {
            get { return estado; }
            set
            {
                var new_value = value;
                if (new_value == EstadoCliente.TIME_WAIT)
                {
                    T.TRACE(0,"COMM: Reseteando timer de estado time_wait a {0} segundos.", timewait_timeout);
                    timewait_left = timewait_timeout;
                }

                estado = new_value;

                if (estado == EstadoCliente.NETWORK_FAILURE) 
                {
                    T.TRACE(0,"COMM: indicando falla de stack");
                    if (StackFailure != null) StackFailure();
                }
            }
        }

        public Destination destination;
        public string IMEI;
        public string Password;
        public short TableVersion;
        public short IdDispositivo;
        public int LoginAttemps;

        private bool iniciado; // para saber si ya se llamo a init.s
        private IPEndPoint localGatewayEndPoint;
        private IPEndPoint localInterQEndPoint;
        private TransporteUDP transporteUDP;
        private FileServer fileServer;
        private readonly ClientMobileTU clientTU = new ClientMobileTU();
        private readonly Timer tickTimer;
        private readonly object tickLock = new object();
        private readonly Base64MessageQueue saliente = new Base64MessageQueue();
        private readonly Base64MessageQueue entrante = new Base64MessageQueue();
        
        public ClienteMobile()
        {
            estado = EstadoCliente.NETWORK_FAILURE;
            tickTimer = new Timer(Tick, null, 0, 5000);
            sending_files = false;
        }

        private GPSPoint queued_fix;
        private GPSPoint last_fix;
        public void Fix(GPSPoint point)
        {
            if (point == null)
            {
                //T.TRACE("FIX - punto nulo");
                return;
            }
            if (last_fix == null) last_fix = new GPSPoint(point.Date.AddSeconds(-120), 0, 0);
            if (Math.Abs((point.Date - last_fix.Date).TotalSeconds) < 60)
            {
                //T.TRACE("FIX - punto muy reciente " + Math.Abs((point.Date - last_fix.Date).TotalSeconds));
                return;
            }
            if (Estado != EstadoCliente.ONLINE) 
            {
                ///T.TRACE("FIX - punto encolado");
                queued_fix = point;
                return;
            }
            last_fix = point;
            queued_fix = null;
            var l = new List<GPSPoint> {point};
            //T.TRACE("FIX - punto reportado");
            clientTU.AutoReport(transporteUDP, destination, l, 0);
        }

        #region Programador
        internal void Tick(object sender)
        {
            //DateTime timeTick = DateTime.Now;
            lock(tickLock)
            {
                switch (Estado)
                {
                    case EstadoCliente.TIME_WAIT:
                        timewait_left--;
                        TickTIMEWAIT();
                        if (timewait_left == 0)
                        {
                            StartTryingLogin();
                        }
                        break;
                    case EstadoCliente.ONLINE:
                        TickONLINE();
                        break;
                } 
            }
        }

        private void TickTIMEWAIT()
        {
            if (sending_files) return;
            try
            {
               /* var label = "";
                var array = saliente.Peek(ref label);
                if (array == null) return;
                T.INFO("NET: hay mensajes salientes, forzamos reconexion.");
                // forzamos la reconexion.
                timewait_left = 0;*/
            }
            catch (Base64MessageQueue.EmptyMessageQueueException)
            {
                return;
            }    
        }

        internal void StartTryingLogin()
        {
            // si no hay transporte, esperamos.
            if (transporteUDP == null || destination == null) return;

            if (Estado == EstadoCliente.TIME_WAIT)
            {
                T.INFO("NETWORK: Intentando pasar a ONLINE...");

                Estado = EstadoCliente.TRY_ONLINE;
                clientTU.LoginSuccess += LoginSuccess;
                clientTU.LoginFail += LoginFail;
                clientTU.LoginRequest(transporteUDP, destination, IMEI, Password, TableVersion, "eyemobile_v1.0");
                LoginAttemps++;
                //saliente.RehabilitarEventos();
            } 
        }

        bool sending_files;
        private int keepalive_wd;
        private const int keepalive_wd_reset = 20;
        internal void TickONLINE()
        {
            if (keepalive_wd-- <= 0)
            {
                T.TRACE("COMM: Enviando KeepAlive.");
                clientTU.KeepAlive(transporteUDP, destination);
                keepalive_wd = keepalive_wd_reset;
            }
            // si no hay transporte, esperamos.
            if (transporteUDP == null || destination == null)
            {
                Estado = EstadoCliente.NETWORK_FAILURE;
                return;
            }
            if (sending_files) return;
            try
            {
                var label = "";
                /*var array = saliente.Peek(ref label);
                if (array == null) return;
                if (FileClient.SendMessage(destination.UIQ, IdDispositivo, array, label))
                {
                    saliente.Pop(ref label);
                }*/
            }
            catch (Base64MessageQueue.EmptyMessageQueueException)
            {
                return;
            }
            catch (Exception e)
            {
                T.EXCEPTION(e, "TickONLINE");
            }
        }

        #endregion

        #region Procedimientos de Login/Logout
        internal void LoginSuccess(object sender, short idDisp)
        {
            if (Estado == EstadoCliente.TRY_ONLINE)
            {
                CoreDLL.Beep(4);
                T.INFO("NETWORK: Estamos ONLINE...");
                Estado = EstadoCliente.ONLINE;
                clientTU.IdDispositivo = IdDispositivo = idDisp;
                if (queued_fix != null)
                {
                    T.TRACE("FIX - envaindo punto encolado");
                    Fix(queued_fix);
                }
            }
        }

        internal void LoginFail(object sender, byte reason)
        {
            if (Estado == EstadoCliente.TRY_ONLINE)
            {
                T.INFO("NETWORK: ERROR {0} fuerza volver a TIME_WAIT.", reason);
                Estado = EstadoCliente.NETWORK_FAILURE;
            }
            if (Estado == EstadoCliente.ONLINE)
            {
                Estado = EstadoCliente.NETWORK_FAILURE;
            }
        }
        #endregion

        public void Close()
        {
            if (!iniciado) return;
            fileServer.close();
            transporteUDP.CerrarTransporte();
            T.TRACE("COMM: cerrado.");
        }

        public void Init(int udp_port, int local_interq_port, int buffer_size, string cola_entrante, string cola_saliente)
        {
            if (iniciado) return;
            
            //entrante.Name = cola_entrante;
            //saliente.Name = cola_saliente;
            localGatewayEndPoint = new IPEndPoint(IPAddress.Any, udp_port);
            localInterQEndPoint = new IPEndPoint(IPAddress.Any, local_interq_port);
            if (buffer_size == 0) buffer_size = 8192;
            transporteUDP = new TransporteUDP
                                {
                                    TransactionUser = clientTU
                                };
            transporteUDP.AbrirTransporte(localGatewayEndPoint, buffer_size, "main_server");
            T.TRACE("COMM: transporte UDP iniciado.");
            
            //FileServer.MessageReceived += FileServer_MessageReceived;
            //fileServer = new FileServer(localInterQEndPoint);
            iniciado = true;
            T.TRACE("COMM: Cliente Windows Mobile iniciado.");
            clientTU.AutoReportFail += AutoReportFail;
            clientTU.GeneralFail += GeneralFail;
        }

        private void GeneralFail(object sender)
        {
			T.ERROR("Keepalive fallo! reinicio cliente torino."); 
            Estado = EstadoCliente.NETWORK_FAILURE;
        }

        public void NetworkReady()
        {
			T.NOTICE("NETWORK READY");
            Estado = EstadoCliente.TIME_WAIT;
        }

        public void NetworkFailure()
        {
			T.NOTICE("NETWORK FAILURE");
            Estado = EstadoCliente.NETWORK_FAILURE;
        }

        internal void AutoReportFail(object sender, int qid)
        {
        	T.ERROR("Autoreporte fallo! reinicio cliente torino.");
            Estado = EstadoCliente.NETWORK_FAILURE;
        }

        internal bool FileServer_MessageReceived(object sender, int idDispositivo, string queuename, Stream file, bool status)
        {
            try
            {
                if (!status)
                {
                    T.ERROR(String.Format("UIQ: ERROR al recibir Mensaje. devid={0} queue={1} size={2}",
                                          idDispositivo, queuename, file.Length));
                    return false;
                }
                T.INFO(String.Format("UIQ: Mensaje Recibido. devid={0} queue={1} size={2}", idDispositivo,
                                     queuename, file.Length));
                var s = file as MemoryStream;
                if (s == null) throw new Exception("el stream recibido del file server no es MemoryStream.");
                //entrante.Push(String.Format("{0}", idDispositivo), s.ToArray());
                return true;
            } catch  (Exception e)
            {
                T.EXCEPTION(e, "FileServer_MessageReceived");
                return false;
            }
        }

        public void ActiveOT(GPSPoint last_point, int active_ot)
        {
            var dummy = new byte[1];
            //saliente.Push(String.Format("{0};ESTADO;0;{1}", IdDispositivo,
             ///                           DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")), dummy);
        }
    }
}