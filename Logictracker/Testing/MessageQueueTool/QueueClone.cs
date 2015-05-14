using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Urbetrack.Messaging;
using Urbetrack.Messaging.Batch;

namespace MessageQueueTool
{
    public partial class QueueClone : Form
    {
        public QueueClone()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            RefreshScreen(true);
        }

        void RefreshScreen(bool change)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "dummy";
            }
            var path = @".\private$\" + textBox1.Text;
            textBox2.Text = textBox1.Text + "_one";
            textBox3.Text = textBox1.Text + "_two";
            var msgs = MessageQueueMonitor.GetMessagesInQueue(path);
            if (msgs >= 0)
            {
                label5.Text = "OK (" + msgs + " mensajes)";
                label5.ForeColor = Color.Green;
                button1.Enabled = change && msgs > 0;
            } else
            {
                label5.Text = "(no existe)";
                label5.ForeColor = Color.Red;
                button1.Enabled = false;
            }
        }

        private void QueueClone_Load(object sender, EventArgs e)
        {
            textBox1.Text = "combustible";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private QueueBatchConsumer batchConsumer;

        private void button1_Click(object sender, EventArgs e)
        {
            var queue_clone = new QueueCloneDispatcher {QueueOne = textBox2.Text, QueueTwo = textBox3.Text};
            batchConsumer = new QueueBatchConsumer(textBox1.Text, queue_clone, "");
            batchConsumer.Start();
            timer1.Enabled = true;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           var path = @".\private$\" + textBox1.Text;
           var msgs = MessageQueueMonitor.GetMessagesInQueue(path);
           if (msgs < 1)
           {
               batchConsumer = null;
               timer1.Enabled = false;
               textBox1.Enabled = true;
               textBox2.Enabled = true;
               textBox3.Enabled = true;
               button2.Enabled = true;
               textBox1.Text = textBox1.Text;
               RefreshScreen(false);
               return;
           }
           RefreshScreen(false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var q = new BinaryMessageQueue {Name = textBox1.Text};
            for(var i = 0; i < 100; i++)
            {
                q.Push("HOLAMUNDO!","MENSAJE:" + i);
            }
            Thread.Sleep(1000);
            RefreshScreen(true);
        }
    }
}
