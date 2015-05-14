using System;
using System.Drawing;
using System.Messaging;
using System.Net;
using System.Windows.Forms;
using Urbetrack.InterQ.Core.Transport;
using Urbetrack.Messaging;
using Urbetrack.Messaging.Batch;
using Urbetrack.Toolkit;

namespace MessageQueueTool
{
    public partial class QueueDocument : Form
    {
        private MessageQueueMonitor thesourcemon;
        private TcpInterQueueServer theserver;

        public QueueDocument()
        {
            InitializeComponent();
            var ep = new IPEndPoint(IPAddress.Any, 7543);
            theserver = new TcpInterQueueServer(ep, 6, false);
        }

        private void QueueDocument_Load(object sender, EventArgs e)
        {
            /*var datos = "La vida es una moneda, la la la !!!!!";
            var buff = GZip.SerializeAndCompress(datos);
            var denuevo = GZip.DecompressAndDeserialize(buff);
            var ms = new MemoryStream(buff);
            var otro = GZip.DecompressAndDeserialize(ms);*/

            txtQueueName.Text = (String) txtQueueName.Tag;
        }

        private void txtQueueName_TextChanged(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Start();
        }

        private void UpdateSource()
        {
            if (thesourcemon != null)
            {
                lblQueueState.Text = String.Format("{0} ({1})", thesourcemon.State.Description(),
                                                   thesourcemon.MessagesInQueue);
                lblQueueState.ForeColor = thesourcemon.State.Color();
                btnCreateQueue.Enabled = (thesourcemon.State == MessageQueueMonitor.States.NOT_FOUND);
                btnDeleteQueue.Enabled = (thesourcemon.State != MessageQueueMonitor.States.NOT_FOUND);
                btnPurgeQueue.Enabled = thesourcemon.MessagesInQueue > 0;
            } else
            {
                lblQueueState.Text = "DESCONOCIDO";
                lblQueueState.ForeColor = Color.Black;
                btnCreateQueue.Enabled = false;
                btnPurgeQueue.Enabled = false;
                btnDeleteQueue.Enabled = false;
            }
        }

        private void pageGeneral_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateSource();
        }

        private void btnCreateQueue_Click(object sender, EventArgs e)
        {
            var queuepath = txtQueueName.Text;
            if (MessageQueue.Exists(queuepath)) return;
            btnCreateQueue.Enabled = false;
            var queue = MessageQueue.Create(queuepath, trans.Checked);
            queue.SetPermissions(Config.GetUsersGroupName(), MessageQueueAccessRights.FullControl);
            queue.MessageReadPropertyFilter.SetAll();
            queue.Formatter = new BinaryMessageFormatter();
            queue.DefaultPropertiesToSend.Recoverable = true;
        }

        private void btnDeleteQueue_Click(object sender, EventArgs e)
        {
            var queuepath = txtQueueName.Text;
            if (!MessageQueue.Exists(queuepath)) {
                btnDeleteQueue.Enabled = false;
                return;
            }
            MessageQueue.Delete(queuepath);
        }

        private void btnPurgeQueue_Click(object sender, EventArgs e)
        {
            var queuepath = txtQueueName.Text;
            if (!MessageQueue.Exists(queuepath))
            {
                btnPurgeQueue.Enabled = false;
                return;
            }
            var queue = new MessageQueue(queuepath);
            queue.Purge();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop(); 
            if (thesourcemon != null)
            {
                thesourcemon.Stop();
                thesourcemon = null;
            }
            thesourcemon = new MessageQueueMonitor(txtQueueName.Text);
            thesourcemon.Start();
        }

        private void QueueDocument_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.Exit();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private QueueBatchConsumer batchConsumer;

        private void uiqp1Start_Click(object sender, EventArgs e)
        {
            var ep = new IPEndPoint(Dns.GetHostAddresses(uiqp1Address.Text)[0], Convert.ToInt32(uiqp1Port.Text));
            var dispatcher = new TcpInterQueueClient_V1_1(ep, uiqp1DstQueue.Text);
            batchConsumer = new QueueBatchConsumer(txtQueueName.Text, dispatcher, "");
            batchConsumer.Start();
            uiqp1Start.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var queuepath = txtQueueName.Text;
            if (!MessageQueue.Exists(queuepath)) return;
            Cursor = Cursors.WaitCursor;
            var mq = new Base64MessageQueue()
            {
                Nombre = queuepath
            };
            var datos = new byte[1024];
            new Random().NextBytes(datos);
            for (var i = 0; i < Convert.ToInt32(nudInject.Value); ++i)
            {
                mq.Push(String.Format("DATA: {0}", i), datos);
            }
            Cursor = Cursors.Default;
        }

        private QueueBatchConsumer uiqp2BatchConsumer;
        private void uiqp2Start_Click(object sender, EventArgs e)
        {
            if (uiqp2Start.Text == "Detener")
            {
                uiqp2BatchConsumer.Stop();
                uiqp2BatchConsumer = null;
                uiqp2Start.Text = "Iniciar";
            } else
            {
                var ep = new IPEndPoint(Dns.GetHostAddresses(uiqp2Address.Text)[0], Convert.ToInt32(uiqp2Port.Text));
                var dispatcher = new TcpInterQueueClient_V1_2(ep, uiqp2DstQueue.Text);
                uiqp2BatchConsumer = new QueueBatchConsumer(txtQueueName.Text, dispatcher, "");
                uiqp2BatchConsumer.Start();
                uiqp2Start.Text = "Detener";    
            }
        }

    }
}
