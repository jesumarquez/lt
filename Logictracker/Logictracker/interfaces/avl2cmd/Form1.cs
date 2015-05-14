using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using etao.marshall;

namespace avl2cmd
{
    public partial class Form1 : Form, ILogger
    {
        string strlog;
        cmd_acceptor acc;
        public Form1()
        {
            InitializeComponent();
            bindComboValorXCamion();
            acc = null;
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 6677);
            tcp_listener<cmd_acceptor> server = new tcp_listener<cmd_acceptor>(ep, 0);
        }

        public void log(string s)
        {
            strlog = s + "\r\n" + strlog;
        }

        public void take(cmd_acceptor a)
        {
            acc = a;
        }

        public void ack(bool acked)
        {
            Form1.ActiveForm.Text = String.Format("ACK RECIBIO = {0}",acked);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private  void bindComboValorXCamion()
        {
            List<KeyValuePair<int, string>> a = new List<KeyValuePair<int, string>>();
            a.Add(new KeyValuePair<int,String>(0,"“LOAD” Status"));
            a.Add(new KeyValuePair<int,String>(1,"“TO JOB” Status"));
            a.Add(new KeyValuePair<int,String>(2,"“ON JOB” Status"));
            a.Add(new KeyValuePair<int,String>(3,"“POUR” Status"));
            a.Add(new KeyValuePair<int,String>(4,"“WASH” Status"));
            a.Add(new KeyValuePair<int,String>(5,"“TO PLANT” Status"));
            a.Add(new KeyValuePair<int,String>(6,"“AT PLANT” Status"));
            a.Add(new KeyValuePair<int,String>(7,"“IN SERVICE” Status"));
            a.Add(new KeyValuePair<int, String>(8, "“REQ TO TALK” Status"));
            estado.DataSource = a;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmd_acceptor.SETLOG(this);
        }

        private void bACK_Click(object sender, EventArgs e)
        {
            if (acc != null)
            {
                acc.send(CMDPDU.encode_ack());
                log("ACK Enviado!");
            }
        }

        private void recvText_DoubleClick(object sender, EventArgs e)
        {
            recvText.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (recvText.Text != strlog) recvText.Text = strlog;
        }

        private void connLbl_Click(object sender, EventArgs e)
        {

        }

        private void bNACK_Click(object sender, EventArgs e)
        {
            if (acc != null)
            {
                acc.send(CMDPDU.encode_nack());
                log("NACK Enviado!");
            }
        }

        private void bEnvia_Click(object sender, EventArgs e)
        {
            if (acc != null)
            {
                acc.send(CMDPDU.encode_estado(Convert.ToInt32(camionID.Value), Convert.ToInt32(estado.SelectedValue) , Convert.ToInt32(latencia.Value)));
                log("Estado Enviado!");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}