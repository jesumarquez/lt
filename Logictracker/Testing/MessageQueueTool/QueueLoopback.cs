using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Urbetrack.Messaging;
using Urbetrack.Messaging.Batch;
using Process=Urbetrack.Toolkit.Process;

namespace MessageQueueTool
{
    public partial class QueueLoopback : Form
    {
        public QueueLoopback()
        {
            InitializeComponent();
        }

        private QueueBatchConsumer batchConsumer;

        private void button1_Click(object sender, EventArgs e)
        {
            var dispatcher = new QueueSimpleDispatcher { Destination = textBox2.Text };
            batchConsumer = new QueueBatchConsumer(textBox1.Text, dispatcher, "");
            batchConsumer.Start();
            button1.Enabled = false;
            timer1.Enabled = true;
        }

        private void QueueLink_Load(object sender, EventArgs e)
        {
            textBox1.Text = "agc_gtw2disp";
            textBox2.Text = "zzdev_twc709037f";
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (batchConsumer == null)
            {
                Text = "Queue Loopback";
                return;
            }
            Text = String.Format("Queue Loopback: {0} Inyectados: {1}", batchConsumer.DispatchedMessages, injectados);
        }

        private void QueueLoopback_FormClosed(object sender, FormClosedEventArgs e)
        {
            batchConsumer.Stop();
            Application.Exit();
        }

        private static void SetLoopbackSignature(byte[] data)
        {
            byte seq = 0;
            for (var i = 0; i < data.GetLength(0); ++i)
            {
                if (seq >= 64) seq = 0;
                data[i] = seq++;
            }
            Debug.Assert(VerifyLoopbackSignature(data));
        }

        private static bool VerifyLoopbackSignature(byte[] data)
        {
            byte seq = 0;
            for (var i = 0; i < data.GetLength(0); ++i)
            {
                if (seq >= 64) seq = 0;
                if (data[i] != seq++) return false;
            }
            return true;
        }

        private int injectados;
        private void button3_Click(object sender, EventArgs e)
        {
            var datos = new byte[1024];
            SetLoopbackSignature(datos);
            var mq = new Base64MessageQueue()
                         {
                             Nombre = textBox2.Text
                         };
            mq.Push(String.Format("MQTOOL: {0}", injectados++),datos);
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
