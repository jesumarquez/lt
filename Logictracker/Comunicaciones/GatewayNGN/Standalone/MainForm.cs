using System;
using System.Windows.Forms;
using Marshall = Urbetrack.ServerCore.Marshall;
using SAMarshall = SA.Marshall;

namespace PumpControl.Testing
{
    public partial class MainForm : Form
    {
        private ServerHandler server;

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void MainForm_Load(object sender, EventArgs evt)
        {
            try
            {
                Marshall.instance.Start();
                server = Marshall.instance.getServiceByName("PumpControl") as ServerHandler;
                server.OnMessageRecived += new ServerHandler.OnMessageRecivedHandler(server_OnMessageRecived);
                label1.Text = String.Format(label1.Tag.ToString(), server.Hostname, server.Port);
            } catch (Exception e)
            {
                SAMarshall.Error(string.Format("exception {0}, texto={1}, en: {2}", e.GetType(), e, e.StackTrace));
                MessageBox.Show("La configuracion es erronea. se debe revisar");
            }
        }

        private void server_OnMessageRecived(object sender, string msg)
        {
            listBox1.Items.Add(msg);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            server.QueryStates();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            server.QueryLogSize('M');
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server.QueryLogSize('V');
        }

        private void button3_Click(object sender, EventArgs e)
        {
            server.QueryLogSize('T');
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Marshall.instance.Stop();
        }
    }
}