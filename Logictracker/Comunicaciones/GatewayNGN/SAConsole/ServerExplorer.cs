using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using SA;
using SA.Console;
using SA.DeviceUtils;
using SA.Mensajeria;
using SA.Transport;
using Urbetrack.Toolkit;

namespace SAConsole
{
    public partial class ServerExplorer : Form
    {
        private readonly ServerConsole conexion = new ServerConsole();
        private readonly string cluster_name;

        public ServerExplorer(string clusterName)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            cluster_name = clusterName;
            UpdateTree = UpdateTreeImpl;
            conexion.Destino = new Destino();

            IPHostEntry he = null;
            string imei;
            try
            {
                while (he == null)
                {
                    he = Dns.GetHostEntry(Config.GetConfigurationString(cluster_name, "server_ip_address", "localhost"));
                    Marshall.Debug(String.Format("CONSOLE: Esperando que resuelva {0}", ConfigurationManager.AppSettings["ip_address"]));
                    Thread.Sleep(1000);
                }
                imei = ConfigurationManager.AppSettings[cluster_name + ".client_identifier"];
            }
            catch (Exception)
            {
                Marshall.Error("ERROR FATAL: La configuracion no es valida, revisar.");
                return;
            }
            conexion.Destino.UDP = new IPEndPoint(he.AddressList[0], 2357);
            conexion.IMEI = imei;
            conexion.Password = "2357";
            conexion.StateChange += conexion_StateChange;
            conexion.DeviceUpdate += conexion_DeviceUpdate;
            conexion.Init(7532, 8192);
        }

        private delegate void UpdateTreeHandler(Device d);
        private UpdateTreeHandler UpdateTree;

        private void UpdateTreeImpl(Device d)
        {
            var imei = d.GetParameter("device_identifier", "unknow");
            var text = String.Format("{0}", imei);
            if (devicesTree.Nodes.ContainsKey(imei))
            {
                devicesTree.Nodes[imei].Text = text;
            } else
            {
                devicesTree.Nodes.Add(imei, text);        
            }
            devicesTree.Nodes[imei].SelectedImageIndex =
                devicesTree.Nodes[imei].ImageIndex = 
                (d.State == Device.States.ONLINE ? 1 : 0);
            
        }
        private readonly Dictionary<int, Device> devices = new Dictionary<int, Device>();
        private readonly Dictionary<string, Device> devices_by_imei = new Dictionary<string, Device>();
        private void conexion_DeviceUpdate(object sender, Device dev)
        {
            var imei = dev.GetParameter("device_identifier", "unknow");
            if (devices.ContainsKey(dev.Id))
            {
                devices[dev.Id] = dev;
                devices_by_imei[imei] = dev;
                Invoke(UpdateTree,  new Object[] {dev});
            } else
            {
                devices.Add(dev.Id, dev);
                devices_by_imei.Add(imei,  dev);
                Invoke(UpdateTree, new Object[] { dev });
            }
        }

        private void conexion_StateChange(ServerConsole src, ServerConsole.EstadoConexion anterior, ServerConsole.EstadoConexion nuevo)
        {
            Text = String.Format("Explorador de Servidores: {0} ({1})", conexion.Destino.UDP, nuevo);
        }

        private void ServerExplorer_Load(object sender, EventArgs e)
        {
            conexion.NetworkReady();
        }

        private Device selected_device;
        private void devicesTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Marshall.User("Nodo Seleccionado: {0}{1}", e.Node.Name, "");
            selected_device = devices_by_imei[e.Node.Name];
            UpdatePanel(selected_device);
        }

        private void UpdatePanel(Device d)
        {
            lblModelo.Text = String.Format(lblModelo.Tag.ToString(), d.type);
            lblFirmware.Text = String.Format(lblFirmware.Tag.ToString(), d.GetParameter("know_firmware_signature", "unknow"));
            lblIMEI.Text = String.Format(lblIMEI.Tag.ToString(), d.GetParameter("device_identifier", "unknow"));
            lblNetwork.Text = String.Format(lblNetwork.Tag.ToString(), d.Destino);
            lblSystemResets.Text = String.Format(lblSystemResets.Tag.ToString(), d.GetParameter("system_resets", "unknow"));
            lblWatchDogResets.Text = String.Format(lblWatchDogResets.Tag.ToString(), d.GetParameter("watchdog_resets", "unknow"));
            lblNETWORK_UDP_ReceivedBytes.Text = String.Format(lblNETWORK_UDP_ReceivedBytes.Tag.ToString(),
                                                              d.GetParameter("net_udp_recv_bytes", "unknow"));
            lblNETWORK_UDP_SentBytes.Text = String.Format(lblNETWORK_UDP_SentBytes.Tag.ToString(),
                                                          d.GetParameter("net_udp_sent_bytes", "unknow"));
            lblNETWORK_UDP_ReceivedDgrams.Text = String.Format(lblNETWORK_UDP_ReceivedDgrams.Tag.ToString(),
                                                               d.GetParameter("net_udp_recv_dgams", "unknow"));
            lblNETWORK_UDP_SentDgrams.Text = String.Format(lblNETWORK_UDP_SentDgrams.Tag.ToString(),
                                                           d.GetParameter("net_udp_sent_dgams", "unknow"));
            
            switch(d.type)
            {
                case Device.Type.URB_v0_5:
                    gbContadores.Enabled = true;
                    btnUpFirm.Enabled = true;
                    break;
                default:
                    gbContadores.Enabled = false;
                    btnUpFirm.Enabled = false;
                    break;
            }

        }

        private void btnUpFirm_Click(object sender, EventArgs e)
        {
            if (selected_device == null)
            {
                btnUpFirm.Enabled = false;
                return;
            }
            var data = new byte[4];
            var pos = 0;
            Decoder.EncodeShort(ref data, ref pos, selected_device.Id);
            conexion.Command(conexion.transporteUDP, conexion.Destino, (byte)Command.Comandos.DeviceUpgradeFirmware, data);
        }

        private void selectFirm_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
                firmFile.Text = openFileDialog1.FileName;
        }

        private void add_Click(object sender, EventArgs e)
        {
           
        }

    }
}