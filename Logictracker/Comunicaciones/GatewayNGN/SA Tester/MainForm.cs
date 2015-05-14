using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Windows.Forms;
using SA;
using SA.DeviceUtils;
using SA.Transport;

namespace SA_Tester
{
    public partial class MainForm : Form, IMarshalLogger
    {
        private TransporteUDP transporteUDP;
        private TransporteTCP transporteTCP;
        private TesterUT ut;
        private string firmwareBinaryFilename;

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Marshall.logger = this;            
        }

        void IMarshalLogger.log(string text, string cat)
        {
            if (cat != Marshall.DEBUGLOG) log.Items.Insert(0,text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            openFileDialog1.InitialDirectory = ConfigurationManager.AppSettings["firmware_path"];
            firmwareBinaryFilename = ConfigurationManager.AppSettings["fota_filename"];
            int tcp_timer_tr = Convert.ToInt32(ConfigurationManager.AppSettings["tcp_timer_tr"]);
            int tcp_timer_trm = Convert.ToInt32(ConfigurationManager.AppSettings["tcp_timer_trm"]);
            int tcp_timer_tv = Convert.ToInt32(ConfigurationManager.AppSettings["tcp_timer_tv"]);

            int udp_timer_tr = Convert.ToInt32(ConfigurationManager.AppSettings["udp_timer_tr"]);
            int udp_timer_trm = Convert.ToInt32(ConfigurationManager.AppSettings["udp_timer_trm"]);
            int udp_timer_tv = Convert.ToInt32(ConfigurationManager.AppSettings["udp_timer_tv"]);

            int queue_delay = Convert.ToInt32(ConfigurationManager.AppSettings["queue_delay"]);
            
            var listener = new IPEndPoint(IPAddress.Any, port);
            
            // aca queda corriendo en background
            transporteUDP = new TransporteUDP();
            ut = new TesterUT {FotaQueueDelay = queue_delay};
            Devices.i().RetrieveDevices += MainForm_UpdateDevices;

            if (udp_timer_tr != 0) transporteUDP.TimerTR = udp_timer_tr;
            if (udp_timer_trm != 0) transporteUDP.TimerTRM = udp_timer_trm;
            if (udp_timer_tv != 0) transporteUDP.TimerTV = udp_timer_tv;

            transporteUDP.UsuarioTransaccion = ut;
            transporteUDP.AbrirTransporte(listener, 8192, "main_server");
            SA.Colas.Colas.i().DefaultUserGroup = "Usuarios";
            transporteTCP = new TransporteTCP();
            if (tcp_timer_tr != 0) transporteTCP.TimerTR = tcp_timer_tr;
            if (tcp_timer_trm != 0) transporteTCP.TimerTRM = tcp_timer_trm;
            if (tcp_timer_tv != 0) transporteTCP.TimerTV = tcp_timer_tv;
            transporteTCP.UsuarioTransaccion = ut;
            transporteTCP.AbrirTransporte(listener, 8192, "main_server");

        }

        private Dictionary<int, Device> MainForm_UpdateDevices()
        {
            var devices = new Dictionary<int, Device>();

            var d = new Device(transporteUDP.UsuarioTransaccion)
                        {
                            Id = 64,
                            IMEI = "CONSOLE_EVECCHIO",
                            KeepaliveLapse = 500,
                            password = "2357",
                            type = Device.Type.CONSOLE_DEVICE
                        };
            devices.Add(d.Id, d);

            d = new Device(transporteUDP.UsuarioTransaccion)
                    {
                        Id = 12,
                        IMEI = "359587010829599",
                        KeepaliveLapse = 180,
                        password = "2357",
                        type = Device.Type.NGN_DEVICE
                    };
            devices.Add(d.Id, d);

            d = new Device(transporteUDP.UsuarioTransaccion)
                    {
                        Id = 13,
                        IMEI = "359586019869150",
                        KeepaliveLapse = 180,
                        password = "2357",
                        type = Device.Type.NGN_DEVICE
                    };
            devices.Add(d.Id, d);
            
            d = new Device(transporteUDP.UsuarioTransaccion)
                    {
                        Id = 14,
                        IMEI = "TWC65200PK",
                        KeepaliveLapse = 180,
                        password = "2357",
                        type = Device.Type.NGN_DEVICE
                    };
            devices.Add(d.Id, d);
            
            d = new Device(transporteUDP.UsuarioTransaccion)
                    {
                        Id = 16,
                        IMEI = "TWC7040774",
                        KeepaliveLapse = 180,
                        password = "2357",
                        type = Device.Type.NGN_DEVICE
                    };
            devices.Add(d.Id, d);

            d = new Device(transporteUDP.UsuarioTransaccion)
                    {
                        Id = 15,
                        IMEI = "359587010827122",
                        KeepaliveLapse = 180,
                        password = "2357",
                        type = Device.Type.NGN_DEVICE
                    };
            devices.Add(d.Id, d);

            return devices;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (Device d in Devices.i().List)
            {
                listBox1.Items.Insert(0, d);
            }
            listBox1.Refresh();
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var d = listBox1.SelectedItem as Device;
                if (d != null)
                {
                    Marshall.User("Dispositivo Seleccionado para Flashear: {0}", d.ToString());
                    flashIt.Tag = d.Id;
                    flashIt.Text = String.Format("Flashar Dispositivo id={0}", d.Id);
                    flashIt.Enabled = true;
                    return;
                }
            }
            flashIt.Text = "Debe seleccionar un Dispositivo";
            flashIt.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            using (var fw = File.Create(firmwareBinaryFilename))
            {
                using (var fs = openFileDialog1.OpenFile() as FileStream) 
                {
                    if (fs == null) {
                        MessageBox.Show("No se pudo abrir el archivo.");
                        return;
                    }
                    var b = new byte[512];
                    var count = fs.Read(b, 0, 512);
                    while (count > 0)
                    {
                        if (count < 512)
                        {
                            for (int i = count - 1; i < 512; i++)
                            {
                                b[i] = 0xFF;
                            }
                        }
                        fw.Write(b, 0, 512);
                        count = fs.Read(b, 0, 512);
                    }
                }
            }
        }

        private void flashIt_Click_1(object sender, EventArgs e)
        {
            var d = Devices.i().Find(Convert.ToInt16(flashIt.Tag));
            if (d != null)
            {
                ut.FlashOverTheAir(d.Id, firmwareBinaryFilename, Convert.ToInt32(nud_pagina.Value));
                flashIt.Text = String.Format("Flashar Dispositivo id={0}", (object[]) flashIt.Tag);
                return;
            }
            Marshall.User("No se encontro el dispositivo seleccionado para Flashear: {0}", flashIt.Tag.ToString());
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Marshall.Info("CORE: cerrando...");
            Marshall.Shutdown();
        }

    }
}