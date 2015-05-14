using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.Net;
using System.Windows.Forms;
using Urbetrack.Mobile.Comm.ClienteMobile;
using Urbetrack.Mobile.Comm.GEO;
using Urbetrack.Mobile.Comm.Queuing;
using Urbetrack.Mobile.Comm.Transport;
using Urbetrack.Mobile.GPS;
using Urbetrack.Mobile.Install;
using Urbetrack.Mobile.IPAQUtils;
using Urbetrack.Mobile.Toolkit;
using Win32;

namespace Urbetrack.Mobile.Gateway
{
    public class Program
    {
        private static readonly ClienteMobile mobile = new ClienteMobile();
        private static int restart_timer = 300 * 1000;
        private static int shutdown_timer = 300 * 1000;
        private static int connection_timer = 60 * 1000;
        
        private static int fix_interval = 60;
        private static bool running = true;
        private static Thread network;
        private static Thread tracker;
        private static readonly GPS.GPS gps = new GPS.GPS(30);
        private static readonly Base64MessageQueue comandos = new Base64MessageQueue();
        private static readonly Base64MessageQueue posicion = new Base64MessageQueue();
        private static readonly Base64MessageQueue status = new Base64MessageQueue();
        private static int active_ot;
        private static GPSPoint last_point;
        private static GPSPoint last_sent_point;
        private static GPSPoint destination_point;
        private static int destination_area_radio = 1500;
        private static int destination_radio = 100;

        public static int ThresholdGpsSignalWeak = 300;
        public static int GpsMinimunPDOP = 8;

        public static DateTime LastestGpsDateTransition { get; set; }

        public static bool GpsSignalWeak
        {
            get
            {
                if (LastestGpsDateTransition == DateTime.MinValue) return false;
                if (GpsSignalFixed) return false;
                var seconds = Math.Abs((LastestGpsDateTransition - DateTime.Now).TotalSeconds);
                return seconds > 300;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                T.Initialize("urbemobile");
                T.CurrentLevel = 0;
                Net.TCP.Stream.StreamTraceLevel = 0;
                var appDomain = AppDomain.CurrentDomain;
                appDomain.UnhandledException += appDomain_UnhandledException;
                if (Math.Abs(CoreDLL.PowerPolicyNotify(PPNMessage.PPN_UNATTENDEDMODE, -1)) == 1)
                {
                    T.ResetFileOnStartup = ConfigurationManager.AppSettings["hack_dont_reset_log"] == "active"
                                               ? false
                                               : true;
                    UnattendedMain(args);
                }/*
                else
                {
                    MessageBox.Show("MAIN: es imposible activar modo inatendido, urbemobile no va a ejecutarse.");
                }*/
                CoreDLL.PowerPolicyNotify(PPNMessage.PPN_UNATTENDEDMODE, 0);
            }
            catch (Exception e)
            {
				T.EXCEPTION(e,"main");
                T.DEAD_REPORT(e);
            }
        }

        private static void appDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating) T.DEAD_REPORT((Exception)e.ExceptionObject);
        }

        static void UnattendedMain(string[] args) 
        {
            if (args == null) throw new ArgumentNullException("args");
            Beep("start_app");

            //Locked = false;
            MSMQ.Install();

            //TRACE = true;
            RunApplication();
            return;
        }

        private static IPEndPoint ServerAddress;
        private static IPEndPoint HostAddress;

        public static IPEndPoint TargetAddres
        {
            get
            {
                //return SystemState.ConnectionsDesktopCount > 0 ? HostAddress : ServerAddress;
                return ServerAddress;
            }
        }

        static void RunApplication()
        {
            CoreDLL.SystemIdleTimerReset(); 

            //comandos.Name = "comandos";
            //comandos.Clear();
            //posicion.Name = "posicion";
            //status.Name = "status";
            mobile.StackFailure += ClientMobileStackFailure;
            mobile.destination = new Destination();
            try
            {
                ServerAddress = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ip_address"]) ?? IPAddress.Any, 2357);
                HostAddress = new IPEndPoint(IPAddress.Parse("169.254.2.2") ?? IPAddress.Any, 2357);

                // establezo la direccion UDP y UIQ (inter queue)
                //mobile.destination.UIQ = mobile.destination.UDP = SystemState.ActiveSyncStatus == ActiveSyncStatus.Synchronizing ? HostAddress : ServerAddress;
                mobile.destination.UIQ = mobile.destination.UDP = ServerAddress;

                restart_timer = Convert.ToInt32(ConfigurationManager.AppSettings["restart_timer"]);
                if (restart_timer == 0) restart_timer = 300 * 1000;
                shutdown_timer = Convert.ToInt32(ConfigurationManager.AppSettings["shutdown_timer"]);
                if (shutdown_timer == 0) shutdown_timer = 300 * 1000;
                connection_timer = Convert.ToInt32(ConfigurationManager.AppSettings["connection_timer"]);
                if (connection_timer == 0) connection_timer = 60 * 1000;
                GpsMinimunPDOP = Convert.ToInt32(ConfigurationManager.AppSettings["gps_min_pdop"]);
                if (GpsMinimunPDOP == 0) GpsMinimunPDOP = 8;
                fix_interval = Convert.ToInt32(ConfigurationManager.AppSettings["gps_fix_interval"]);
                if (fix_interval == 0) fix_interval = 60;
                destination_area_radio = Convert.ToInt32(ConfigurationManager.AppSettings["gps_area_radio"]);
                if (destination_area_radio == 0) destination_area_radio = 1500;
                //notschedule = (ConfigurationManager.AppSettings["hack_not_re_schedule"] == "active" ? true : false);
                var auto_gc = ConfigurationManager.AppSettings["hack_auto_gc"];
                
                /*if (!string.IsNullOrEmpty(auto_gc)) {
                    T.TRACE(String.Format("HACK: Activo GeoCerca Automatica de Pruebas {0}",auto_gc));
                    var dummy = new byte[2];
                    comandos.Push(auto_gc, dummy);
                }*/
            }
            catch (Exception)
            {
                T.ERROR("ERROR FATAL: La configuracion no es valida, revisar.");
                T.ERROR("CUIDADO!!! No se programa el restart, debe ejecutar la aplicacion manualmente.");
                return;
            }
            
#if false
            mobile.IMEI = iPaqUtil.GetDeviceSN();
#else
        	mobile.IMEI = "HTC-GUSTAVO";
#endif
            using (var script_hlp = File.Create(@"\Temp\pdaserial.txt"))
            {
                var buffer = Encoding.ASCII.GetBytes(mobile.IMEI);
                script_hlp.Write(buffer, 0, buffer.GetLength(0));
            }
            
            mobile.Password = "2357";
            mobile.Init(2357, 2358, 8192, "entrante", "saliente");
            T.INFO("COMM: Init convocado.");
            
            network = new Thread(NetworkProc);
            T.INFO("NETWORK: lanzando hilo.");
            network.Start();

            tracker = new Thread(TrackerProc);
            T.INFO("TRACKER: lanzando hilo.");
            tracker.Start();

            CoreDLL.SystemIdleTimerReset(); 

            IntPtr wavHandle = CoreDLL.SetPowerRequirement("WAV1:",
                                                           CEDEVICE_POWER_STATE.D0,
                                                           DevicePowerFlags.POWER_NAME | DevicePowerFlags.POWER_FORCE, IntPtr.Zero, 0);
            if (wavHandle == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception();
            }

            var gpsHandle = CoreDLL.SetPowerRequirement("GPD0:",
                                                        CEDEVICE_POWER_STATE.D0,
                                                        DevicePowerFlags.POWER_NAME | DevicePowerFlags.POWER_FORCE, IntPtr.Zero, 0);
            if (gpsHandle == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception();
            }

            CoreDLL.Beep(1);
            T.TRACE("Lanzando Urbetrack Ready");
            LaunchGatewayRunning();

            var upgrade_count = 0;
            while(running)
            {
                CoreDLL.SystemIdleTimerReset(); 
                if (upgrade_count == 0)
                {
                    T.TRACE("Lanzando Urbetrack Update");
                    LaunchGatewayUpgrade();
                    upgrade_count = 360;
                }
                upgrade_count--;
                for (var x = 0; x < 10; x++)
                {
                    Thread.Sleep(1000);
                    Application.DoEvents();
                }
                var comando = "";
                try
                {
					Thread.Sleep(1000);
                    /*comandos.Pop(ref comando);
                    if (comando.Length > 0)
                        ProcessCommand(comando);*/
                } catch (Exception e)
                {
                    T.EXCEPTION(e,"Command Processor");
                } 
            }

            if (wavHandle != IntPtr.Zero)
            {
                CoreDLL.ReleasePowerRequirement(wavHandle);
            }

            if (gpsHandle != IntPtr.Zero)
            {
                CoreDLL.ReleasePowerRequirement(gpsHandle);
            }

            T.INFO("URBEMOBILE: hora de ahorar energia...");
            mobile.Close();
            network.Join(10000);
            T.INFO("URBEMOBILE: network sincronizada.");
            //supervisor.Join(10000);
            //T.INFO("URBEMOBILE: supervisor sincornizado.");
            tracker.Join(10000);
            T.INFO("URBEMOBILE: tracker sincornizado.");

            T.TRACE("--- URBEMOBILE TERMINADO ---");

            Beep("end_app");
        }

        private static void ProcessCommand(string comando)
        {
            if (!running) return;
            var data = comando.Split(";".ToCharArray());
            if (data[0] == "CLOSE")
            {
                var closer = data.GetLength(0) == 3 ? data[2] : "UNKNOW";
                T.INFO("CORE: application exit required from " + closer);
                running = false;
                
                Application.Exit();
                return;
            }
            if (data[0] == "STATUS")
            {
                T.INFO("CORE: status requerido");
                var dd = new byte[2];
                //status.Push(network_status ? "Activo, En Linea" : "Activo, Sin Conexion", dd);
                return;
            }
            if (data.GetLength(0) >= 4)
            {
                
                if (data[0] == "GC")
                {
                    active_ot = Convert.ToInt32(data[1]);
                    var culture = new CultureInfo("en-US");
                    var lat = Convert.ToSingle(data[2], culture);
                    var lon = Convert.ToSingle(data[3], culture);
                    destination_point = new GPSPoint(DateTime.Now, lat, lon);
                    destination_radio = Convert.ToInt32(data[4]);
                    T.INFO("CORE: Configurando punto destino.");
                    T.INFO(destination_point.AsMessage(destination_radio));
                }
            } else
            {
                T.NOTICE("CORE: Comando desconocido. " + comando );
            }
                 
        }

        private static bool gpsSignalFixed;

        public static bool GpsSignalFixed
        {
            get { return gpsSignalFixed; }
            set
            {
                var newstate = value;
                if (newstate == gpsSignalFixed) return;
                gpsSignalFixed = newstate;
                LastestGpsDateTransition = DateTime.Now;
                if (newstate)
                {
                    Beep("gps_got_fix");
                    T.INFO("TRACKER: GPS OBTUBO FIX");
                }else
                {
                    Beep("gps_lost_fix");
                    T.NOTICE("TRACKER: GPS PERDIO EL FIX.");
                }
            }
        }

        private static IntPtr _gpsPowerRequirements = IntPtr.Zero;
        private static void TrackerProc()
        {
            T.INFO(String.Format("TRACKER: iniciando GPS ({0})", GpsDeviceName));

            CEDEVICE_POWER_STATE currentPowerState;
            CoreDLL.GetDevicePower(GpsDeviceName, DevicePowerFlags.POWER_NAME | DevicePowerFlags.POWER_FORCE, out currentPowerState);
            _gpsPowerRequirements = CoreDLL.SetPowerRequirement(GpsDeviceName, CEDEVICE_POWER_STATE.D0, DevicePowerFlags.POWER_NAME, IntPtr.Zero, 0);
            if (currentPowerState == CEDEVICE_POWER_STATE.D4)
            {
                T.INFO("TRACKER: el GPS esta apagado, activo guarda.");
                Thread.Sleep(500);
            }
            gps.Open();
            LastestGpsDateTransition = DateTime.Now;
            while (running)
            {
                Thread.Sleep(5000);
                var position = gps.GetPosition();
                if (position == null)
                {
                    GpsSignalFixed = false;
                    continue;
                }
                if (GPS.GPS.IsPositionValid(position, true, GpsMinimunPDOP))
                {
                    GpsSignalFixed = true;
                    ProcessPosition(position);
                } else
                {
                    GpsSignalFixed = false;
                }
            }
            gps.Close();
            Thread.Sleep(800);
            CoreDLL.ReleasePowerRequirement(_gpsPowerRequirements);
            T.INFO("TRACKER: terminado.");
        }

        private static void ProcessPosition(GPSPosition position)
        {
            //last_point = new GPSPoint(position.Time.ToUniversalTime(), (float)position.dblLatitude, (float)position.dblLongitude, Convert.ToInt32(position.flSpeed))
            last_point = new GPSPoint(position.Time, (float)position.dblLatitude, (float)position.dblLongitude, Convert.ToInt32(position.flSpeed))
                             {Course = position.flHeading};
            
            T.INFO(last_point.ToString());
            double delta_distance = -1;

            mobile.Fix(last_point);

            if (destination_point != null) {
                delta_distance = Distances.Rhumb(last_point, destination_point);
                T.TRACE(String.Format("TRACKER: delta_distance={0}",delta_distance));
                //Locked = delta_distance <= destination_area_radio;

                /*if (delta_distance <= destination_radio)
                {
                    T.INFO("TRACKER: Llegamos al destino. Informando.");
                    Beep("gps_in_destination");
                    posicion.Clear();
                    destination_point = null;
                    var dummy = new byte[2];
                    dummy[0] = 0;
                    posicion.Push(last_point.AsMessage((int)delta_distance), dummy);
                    mobile.ActiveOT(last_point, active_ot);
                }*/
            }

            if (last_sent_point == null || (last_point.Date - last_sent_point.Date).TotalSeconds >= fix_interval)
            {
                T.INFO("TRACKER: fix seleccionado para enviar.");
                last_sent_point = last_point;
                mobile.Fix(last_sent_point);
                posicion.Clear();
                var dummy = new byte[2];
                dummy[0] = 0;
                posicion.Push(last_sent_point.AsMessage((int) delta_distance), dummy);
            }
        }

        static void Beep(string sound)
        {
            if (ConfigurationManager.AppSettings["sound_enabled"] != "1") return;
            var filename = ConfigurationManager.AppSettings[string.Format("sound_{0}", sound)];
            if (filename == "") filename = @"\windows\Alarm1.wav";
            CoreDLL.PlaySound(filename, IntPtr.Zero,
                              CoreDLL.PlaySoundFlags.SND_FILENAME | CoreDLL.PlaySoundFlags.SND_ASYNC);
        }

        static void NetworkFailure(string text)
        {
            T.NOTICE(text);
            mobile.NetworkFailure();
        }

        private static void ClientMobileStackFailure()
        {
            T.NOTICE("NETWORK: STACK FAILURE!");
            CoreDLL.Beep(2);
            network_status = false;
            SyncTimedInternetDisconnect(60);
        }

        private static bool network_status;
        private const int connection_wd_reset = 12;
        static void NetworkProc()
        {
            while(running) {
                T.INFO("NETWORK: probando conectividad.");
                if (SyncTimedInternetConnect(connection_timer / 1000))
                {
                    network_status = true;
                    T.INFO("NETWORK: conexion comprobada, estamos en internet.");
                    //Beep("connected");
                    //var dock = SystemState.ConnectionsDesktopCount;
                    var dock = 0;
                    mobile.destination.UIQ = mobile.destination.UDP = dock > 0 ? HostAddress : ServerAddress;
                    T.TRACE(string.Format("NETWORK: server seleccionado {0}", dock > 0 ? HostAddress : ServerAddress));
                    mobile.NetworkReady();
                    var connetion_wd = connection_wd_reset; // s
                    while (network_status)
                    {
                        Thread.Sleep(10000);
                        if (connetion_wd-- <= 0)
                        {
                            if (!SyncTimedInternetTest(connection_timer / 1000))
                            {
                                NetworkFailure("NETWORK: test de conexion fallido.");
                                break;
                            }
                            connetion_wd = connection_wd_reset; 
                        }
                        /*if (dock != SystemState.ConnectionsDesktopCount)
                        {
                            NetworkFailure("NETWORK: cambio de condiciones del Dock.");
                            break;
                        }*/
                        if (running) continue;
                        T.TRACE("NETWORK: tarea terminada. (e3)");
                        return;
                    }
                }
                //Beep("not_connected");
                if (!running)
                {
                    T.TRACE("NETWORK: tarea terminada. (e1)");
                    return;
                }
                network_status = false;
                T.NOTICE("NETWORK: Conexion a la red fallida, TimeWait 10s ....");
                Thread.Sleep(20000);
            }
            T.TRACE("NETWORK: tarea terminada. (e2)");
        }
        
        static void SyncTimedInternetDisconnect(int timeout_seconds)
        {
//            var pname = ConfigurationManager.GetAppSettingsValue("net_disconnect", @"\Archivos de programa\Urbetrack Mobile\netdown.mscr");
            var pname = ConfigurationManager.GetAppSettingsValue("net_disconnect", @"\Program Files\Urbetrack Mobile\netdown.mscr");
            var p = System.Diagnostics.Process.Start(pname, "");
            if (p == null) return;
            p.WaitForExit(timeout_seconds * 1000);
            return;
        }

        static bool SyncTimedInternetTest(int timeout_seconds)
        {
            try
            {
                //var pname = ConfigurationManager.GetAppSettingsValue("net_test", @"\Archivos de programa\Urbetrack Mobile\nettest.mscr");
                //var upfilename = ConfigurationManager.GetAppSettingsValue("net_status_file", @"\Archivos de programa\Urbetrack Mobile\UP.TXT");
                var pname = ConfigurationManager.GetAppSettingsValue("net_test", @"\Program Files\Urbetrack Mobile\nettest.mscr");
                var upfilename = ConfigurationManager.GetAppSettingsValue("net_status_file", @"\Program Files\Urbetrack Mobile\UP.TXT");
                if (File.Exists(upfilename)) File.Delete(upfilename);
                var p = System.Diagnostics.Process.Start(pname, "");
                if (p == null) return false;
                p.WaitForExit(timeout_seconds * 1000);
                using (var file = File.OpenText(upfilename))
                {
                    var line = file.ReadLine();
                    return line.StartsWith("CR");
                }
            // ReSharper disable EmptyGeneralCatchClause
            }
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
            return false;
        }

        static bool SyncTimedInternetConnect(int timeout_seconds)
        {
            try
            {
//                var pname = ConfigurationManager.GetAppSettingsValue("net_connect", @"\Archivos de programa\Urbetrack Mobile\netup.mscr");
//                var upfilename = ConfigurationManager.GetAppSettingsValue("net_status_file",@"\Archivos de programa\Urbetrack Mobile\UP.TXT");
                var pname = ConfigurationManager.GetAppSettingsValue("net_connect", @"\Program Files\Urbetrack Mobile\netup.mscr");
                var upfilename = ConfigurationManager.GetAppSettingsValue("net_status_file",@"\Program Files\Urbetrack Mobile\UP.TXT");
                if (File.Exists(upfilename)) File.Delete(upfilename);
                var p = System.Diagnostics.Process.Start(pname, "");
                if (p == null) return false;
                p.WaitForExit(timeout_seconds * 1000);
                using (var file = File.OpenText(upfilename))
                {
                    var line = file.ReadLine();
                    return line.StartsWith("CR");
                }
// ReSharper disable EmptyGeneralCatchClause
            } catch {}
// ReSharper restore EmptyGeneralCatchClause
            return false;
        }

        static void LaunchGatewayUpgrade()
        {
            try
            {
//                var pname = ConfigurationManager.GetAppSettingsValue("upgrade_tool", @"\Archivos de programa\Urbetrack Mobile\upgrade.mscr");
                var pname = ConfigurationManager.GetAppSettingsValue("upgrade_tool", @"\Program Files\Urbetrack Mobile\upgrade.mscr");
                System.Diagnostics.Process.Start(pname, "");
// ReSharper disable EmptyGeneralCatchClause
            } catch {}
// ReSharper restore EmptyGeneralCatchClause
        }

        static void LaunchGatewayRunning()
        {
            try
            {
//                var pname = ConfigurationManager.GetAppSettingsValue("gateway_running", @"\Archivos de programa\Urbetrack Mobile\onrunning.mscr");
                var pname = ConfigurationManager.GetAppSettingsValue("gateway_running", @"\Program Files\Urbetrack Mobile\onrunning.mscr");
                System.Diagnostics.Process.Start(pname, "");
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
        }

        public static void RunAppAtTime(string applicationEvent, DateTime startTime)
        {
            long fileTimeUTC = startTime.ToFileTime();
            long fileTimeLocal = 0;
            var systemStartTime = new SystemTime();
            CoreDLL.FileTimeToLocalFileTime(ref fileTimeUTC, ref fileTimeLocal);
            CoreDLL.FileTimeToSystemTime(ref fileTimeLocal, systemStartTime);
            CoreDLL.CeRunAppAtTime(applicationEvent, systemStartTime);
        }

        public static void RunAppAtTime(string applicationEvent, TimeSpan timeDisplacement)
        {
            var targetTime = DateTime.Now + timeDisplacement;
            RunAppAtTime(applicationEvent, targetTime);
        }

        public static void RunAppAtTime(string applicationEvent, int seconds)
        {
            var targetTime = DateTime.Now.AddSeconds(seconds);
            RunAppAtTime(applicationEvent, targetTime);
        }

        const string GPS_DEVICE_NAME_PATH = @"DRIVERS\Builtin\GPSID";
        private static string _gpsDeviceName;
        public static string GpsDeviceName
        {
            get
            {
                if (_gpsDeviceName == null)
                {
                    var gpsInfoKey = Registry.LocalMachine.OpenSubKey(GPS_DEVICE_NAME_PATH);
                    if (gpsInfoKey != null)
                    {
                        try
                        {
                            _gpsDeviceName = String.Format("{0}{1}:", gpsInfoKey.GetValue("Prefix"), gpsInfoKey.GetValue("Index"));
                        }
                        catch (Exception e)
                        {
                            T.EXCEPTION(e, "GpsDeviceName");
                        } 
                    }
                }
                return _gpsDeviceName;
            }
        }
    }
}