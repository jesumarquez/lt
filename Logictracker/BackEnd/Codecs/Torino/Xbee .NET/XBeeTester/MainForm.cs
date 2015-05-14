#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using Urbetrack.Toolkit;
using XbeeCore;
using XBeeTester.Properties;

#endregion

namespace XBeeTester
{
    public partial class MainForm : Form
    {
        private XBeeAPIPort port;
        private readonly Queue<int> packet_queue = new Queue<int>();
        private readonly Semaphore queue_sema = new Semaphore(0, 1024);
        private int packet_per_second;
        private int packet_size;
        private float packet_per_loop;
        private float packet_per_loop_rest;
        private DateTime start_time;
        private readonly Thread consumidor;        
        private bool close_consumidor;

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            consumidor = new Thread(Consumidor);
            consumidor.Start();
            //stats.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true); 
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            port = new XBeeAPIPort
                       {
                           Rate = Convert.ToInt32(tbXbeePortRate.Text),
                           SerialPort = tbXbeePortCom.Text,
                           Id = 1
                       };
            port.HardwareStatus += port_HardwareStatus;
            port.PDURecibida +=port_PDURecibida;
            //lblHWStatus.Text = lblHWStatus.Tag.ToString();
            comboBox1.SelectedIndex = Settings.Default.XBeePortParity;
            comboBox2.SelectedIndex = Settings.Default.XBeePortStopBits;
            comboBox3.SelectedIndex = Settings.Default.XBeePortFlowCtrl;
            port.SerialConnectionTimeout = Convert.ToInt32(Settings.Default.XBeeConnectionTimeout);
            UpdatePacketizer();
        }

        private static void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.Save();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            close_consumidor = true;
            // señalizo el semaforo para que termine la thread del consumidor
            queue_sema.Release();
        }

        int last_active_second;
        int active_second_bytes;
        int last_active_minute;
        int active_minute_bytes;

        private void port_PDURecibida(XBeeAPIPort sender, XBeePDU pdu)
        {
            // loopback           
            if (pdu.Data[0] != 0xC4)
            {
                Stats._frames_received_errors++;
                return;
            }
            
            Stats._frames_received++;
            Stats._pong_frames_received++;
            Stats._0_test_in_bytes_recv += pdu.Data.GetLength(0);

            // ancho de banda entrante efectivo 
            var now = DateTime.Now;
            if (last_active_second != now.Second)
            {                
                Stats._0_efective_incomming_bps = active_second_bytes * 8;
                active_second_bytes = 0;
            }
            last_active_second = now.Second;
            active_second_bytes += pdu.Data.GetLength(0);

            if (last_active_minute != now.Minute)
            {
                Stats._0_1_minute_avg_incomming_bps = active_minute_bytes * 8 / 60;
                active_minute_bytes = 0;
            }
            last_active_minute = now.Minute;
            active_minute_bytes += pdu.Data.GetLength(0);

        }

        private void port_HardwareStatus(XBeeAPIPort sender, XBeePDU pdu)
        {
            var msg = "";
            if (pdu.HWStatus == 0)
            {
                msg = String.Format("{0} Hardware Reset",DateTime.Now);
            }
            if (pdu.HWStatus == 1)
            {
                msg = String.Format("{0} Watchdog Timer Reset", DateTime.Now);
            }
            if (pdu.HWStatus == 2)
            {
                msg = String.Format("{0} Associated", DateTime.Now);
            }
            if (pdu.HWStatus == 3)
            {
                msg = String.Format("{0} Disassociated", DateTime.Now);
            }
            if (pdu.HWStatus == 4)
            {
                msg = String.Format("{0} Sync lost", DateTime.Now);
            }
            if (pdu.HWStatus == 5)
            {
                msg = String.Format("{0} Coordinator realignment", DateTime.Now);
            }
            if (pdu.HWStatus == 6)
            {
                msg = String.Format("{0} Coordinator started", DateTime.Now);
            }
            if (pdu.HWStatus > 6)
            {
                msg = "Codigo de estado de HW desconocido.";
            }
            log.Items.Insert(0, msg);
        }

        private void tbXbeePortCom_TextChanged(object sender, EventArgs e)
        {
            port.SerialPort = tbXbeePortCom.Text;
            log.Items.Insert(0, String.Format("{0} Se ha seleccionado el puerto {1}", DateTime.Now, port.SerialPort));
        }

        private void tbXbeePortRate_TextChanged(object sender, EventArgs e)
        {
            if (tbXbeePortRate.Text == "") return;
            port.Rate = Convert.ToInt32(tbXbeePortRate.Text);
            log.Items.Insert(0, String.Format("{0} Se ha seleccionado velocidad el puerto {1}", DateTime.Now, port.Rate));
        }

        private void updater_Tick(object sender, EventArgs e)
        {
            if (port != null)
            {
                lblCTS.BackColor = (port.CTS ? Color.Green : Color.Red);
                lblDSR.BackColor = (port.DSR ? Color.Green : Color.Red);
                lblCD.BackColor = (port.CD ? Color.Green : Color.Red);
                cbDTR.Checked = port.DTR;
                cbRTS.Checked = port.RTS;

                switch (port.Estado)
                {
                    case XBeeAPIPort.Estados.MODULO_ENLINEA:
                        lblXbeeStat.Text = String.Format("MODEM {0}", port.MyXbeeAddress);
                        lblXbeeStat.ForeColor = Color.Green;
                        break;
                    case XBeeAPIPort.Estados.PUERTO_ABIERTO:
                        lblXbeeStat.Text = "PUERTO OK";
                        lblXbeeStat.ForeColor = Color.Yellow;
                        break;
                    default:
                        lblXbeeStat.Text = "PUERTO ERR";
                        lblXbeeStat.ForeColor = Color.Red;
                        break;
                }
            }
            else
            {
                lblXbeeStat.Text = "DESCONOCIDO";
                lblXbeeStat.ForeColor = Color.Red;
            }

            var textocola = lblQueue.Tag.ToString().Split("|".ToCharArray());
            lock (packet_queue)
            {
                lblQueue.Text = packet_queue.Count > 0 ? String.Format(textocola[1], packet_queue.Count) : textocola[0];
            }

            if (packetizer.Enabled)
            {
                var this_time = DateTime.Now;
                var diff = this_time - start_time;
                Stats._0_test_in_seconds = (int) diff.TotalSeconds;
            }

            var data = Stack.Update();            
            stats.BeginUpdate();
            foreach(var k in data.Keys)
            {
                //Console.WriteLine("procesando {0}", k);
                var li = stats.FindItemWithText(k);
                if (li == null)
                {
                    li = new ListViewItem(k, 0);
                    li.SubItems.Add(data[k]);
                    stats.Items.Add(li);
                } else
                {
                    li.SubItems[1].Text = data[k];
                }
            }
            data = Stats.Update();
            foreach (var k in data.Keys)
            {
                //Console.WriteLine("procesando {0}", k);
                var li = stats.FindItemWithText(k);
                if (li == null)
                {
                    li = new ListViewItem(k, 0);
                    li.SubItems.Add(data[k]);
                    stats.Items.Add(li);
                }
                else
                {
                    li.SubItems[1].Text = data[k];
                }
            }
            stats.EndUpdate();
        }

        private void UpdatePacketizer()
        {
            try
            {
                packet_per_second = Convert.ToInt32(tbPacketsPerSecond.Text);
                packet_size = Convert.ToInt32(tbBytesPerPacket.Text);
                var xbeebps = (packet_size * 8 * packet_per_second);
                grpSender.Text = String.Format(grpSender.Tag.ToString(), xbeebps, "bps (aprox.)");
// ReSharper disable PossibleLossOfFraction
                packet_per_loop = ((float)packet_per_second) / (1000 / packetizer.Interval);
// ReSharper restore PossibleLossOfFraction
            }
            catch (Exception)
            {
                grpSender.Text = String.Format(grpSender.Tag.ToString(), "(invalido)", "");
                packet_per_loop = 0;
                packet_per_second = 0;
                packet_size = 0;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var boton = btnStart.Tag.ToString().Split("|".ToCharArray());
            if (packetizer.Enabled)
            {
                btnStart.Text = boton[0];
                // desactivo el timer del generador de paquetes.
                packetizer.Enabled = false;
                UpdatePacketizer();
                lock (packet_queue) {
                    packet_queue.Clear();
                }
                var end_time = DateTime.Now;
                var diff = end_time - start_time;
                Stats._0_test_in_seconds = (int) diff.TotalSeconds;                
                //int esperados = ((int)diff.TotalSeconds) * packet_per_second;
                //Console.WriteLine("Esperados: {0} Reales: {1}", esperados, packet_id);
            }
            else
            {
                // activo el timer del generador de paquetes.
                btnStart.Text = boton[1];
                packet_id = 0;
                packetizer.Enabled = true;
                start_time = DateTime.Now;
                Stats._0_test_in_seconds = 0;
                Stats._0_test_in_bytes_recv = 0;
                Stats._0_test_in_bytes_sent = 0;
            }
        }

        private void tbPacketsPerSecond_TextChanged(object sender, EventArgs e)
        {
            UpdatePacketizer();
        }

        private void tbBytesPerPacket_TextChanged(object sender, EventArgs e)
        {
            UpdatePacketizer();
        }

        private int packet_id;
        private void packetizer_Tick(object sender, EventArgs e)
        {            
            var fpps = (packet_per_loop + packet_per_loop_rest);
            packet_per_loop_rest = fpps - ((int)fpps);
            for (var i = 0; i < (int)fpps; i ++) {
                lock (packet_queue)
                {
                    packet_queue.Enqueue(packet_id++);
                }
                try
                {
                    queue_sema.Release();
                    Stats._tx_queue_size++;
                } catch (SemaphoreFullException)
                {
                    
                }
            }
        }
                
        private void Consumidor()
        {
            while (true)
            {
                ushort remote_addr;
                queue_sema.WaitOne();
                Stats._tx_queue_size--;
                if (close_consumidor) return;
                try {
                    remote_addr = Convert.ToUInt16(tbRemoteAddress.Text);
                }
                catch (Exception)
                {
                    continue;
                }
                lock (packet_queue)
                {
                    if (packet_queue.Count > 0)
                    {
                        if (packet_size > 0)
                        {
                            var data = new byte[packet_size];
                            data[0] = 0xC4;
                            data[1] = 0x0B; // large loopback request.
                            for (int i = 2; i < packet_size; ++i) 
                            {
                                data[i] = 0xFF; 
                            }
                            try
                            {
                                if (!port.Send(data, remote_addr))
                                {
                                    Stats._send_errors++;
                                    continue;
                                }
                                Stats._frames_sent++;
                                Stats._ping_frames_sent++;
                                Stats._0_test_in_bytes_sent += packet_size;
                            }
                            catch (Exception)
                            {
                                Stats._send_errors++;
                            }
                        }
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            log.Items.Clear();
            Stack.Clear();
            Stats.Clear();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            port.DataBits = Convert.ToInt32(textBox1.Text);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            T.TRACE(1, "Paridad Seleciona Indice: {0}", comboBox1.SelectedIndex);
            Settings.Default.XBeePortParity = comboBox1.SelectedIndex;
            switch (comboBox1.SelectedIndex)
            {
                case 0: port.Parity = Parity.None; break;
                case 1: port.Parity = Parity.Odd; break;
                case 2: port.Parity = Parity.Even; break;
                case 3: port.Parity = Parity.Mark; break;
                case 4: port.Parity = Parity.Space; break;                
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            T.TRACE(1, "StopBits Seleciona Indice: {0}", comboBox2.SelectedIndex);
            Settings.Default.XBeePortStopBits = comboBox2.SelectedIndex;
            switch (comboBox2.SelectedIndex)
            {
                case 0: port.StopBits = StopBits.None; break;
                case 1: port.StopBits = StopBits.One; break;
                case 2: port.StopBits = StopBits.OnePointFive; break;
                case 3: port.StopBits = StopBits.Two; break;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            T.TRACE(1, "Control de Flujo Seleciona Indice: {0}", comboBox3.SelectedIndex);
            Settings.Default.XBeePortFlowCtrl = comboBox3.SelectedIndex;
            switch (comboBox3.SelectedIndex)
            {
                case 0: port.Handshake = Handshake.None; break;
                case 1: port.Handshake = Handshake.RequestToSend; break;
                case 2: port.Handshake = Handshake.XOnXOff; break;
            }
        }

        private void cbRTS_CheckedChanged(object sender, EventArgs e)
        {
            port.RTS = cbRTS.Checked;
        }

        private void cbDTR_CheckedChanged(object sender, EventArgs e)
        {
            port.DTR = cbDTR.Checked;
        }

        private void tbTimeout_TextChanged(object sender, EventArgs e)
        {
            port.SerialConnectionTimeout = Convert.ToInt32(tbTimeout.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            port.RemoteATCommand(Convert.ToUInt16(tbRemoteAddress.Text), textBox3.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ch = Convert.ToByte(textBox4.Text);
            port.SetChannel(ch);
        }

    }
}