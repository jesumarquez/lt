#region Usings

using System;
using System.Net;
using System.Windows.Forms;
using Urbetrack.InterQ.Core.Transport;
using Urbetrack.Messaging;
using Urbetrack.Messaging.Batch;
using Urbetrack.Messaging.Opaque;

#endregion

namespace MessageQueueTool
{
    public partial class MainForm : Form
    {
        private readonly BinaryMessageQueue saliente = new BinaryMessageQueue();
        private readonly BinaryMessageQueue entrante = new BinaryMessageQueue();
        private TcpInterQueueServer server;
        public MainForm()
        {
            InitializeComponent();
            //saliente.Name = "inicio_prueba";
            //entrante.Name = "fin_prueba";
            server = new TcpInterQueueServer(new IPEndPoint(IPAddress.Any,1234),0,true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
 }

        private void button2_Click(object sender, EventArgs evt)
        {
            var mq = new BinaryMessageQueue();
            mq.Name = comboBox1.Text;
            var label = "";
            try
            {
                var m = mq.Peek();
                MessageBox.Show((string)m.Body, m.Label);
            } catch (Exception e)
            {
                MessageBox.Show(e.Message, "EXCEPCION");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var omq = new OpaqueMessageQueue(textBox2.Text)
                          {
                              Name = comboBox1.Text
                          };
            var om = omq.Peek();
            if (om == null)
            {
                MessageBox.Show("No hay datos!");
                return;
            }
            MessageBox.Show(String.Format("Label: {0} Length: {1} OpaqueBody: {2}", om.Label, om.Length,
                                          Convert.ToBase64String(om.OpaqueBody)));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*var cli = new TcpInterQueueClient_V1_1(new IPEndPoint(IPAddress.Loopback, 1234), comboBox2.Text);
            var disp = new QueueBatchConsumer(comboBox1.Text, cli, textBox2.Text);
            var r = QueueBatchConsumer.Dispatch();
            MessageBox.Show("Resultado " + r.ToString());*/
        }

        private void button5_Click(object sender, EventArgs e)
        {
            saliente.Name = comboBox1.Text;
            var obj = new byte[2];
            saliente.Push(comboBox3.Text, obj);
            ///MessageBox.Show(comboBox3.Text);
        }
    }


}
