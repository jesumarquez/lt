using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Urbetrack.Mobile.Comm.Queuing;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.MessageQueueTool
{
    public partial class QueueLoopback : Form
    {
        private Thread worker;
        private readonly Base64MessageQueue entrante;
        private readonly Base64MessageQueue saliente;

        public QueueLoopback()
        {
            InitializeComponent();
            saliente = new Base64MessageQueue();
            entrante = new Base64MessageQueue();
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


        private void button1_Click(object sender, EventArgs e)
        {
            worker = new Thread(TheWork);
            worker.Start();
            button1.Enabled = false;
        }

        private int data_ok;
        private int data_err;

        private void TheWork()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var label = "";
                        var data = entrante.SafePop(ref label);
                        if (data == null) continue;
                        if (VerifyLoopbackSignature(data)) data_ok++; else data_err++;
                        saliente.Push(label, data);
                        message_counter++;
                        UpdateStats();
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        T.EXCEPTION(e, "MainLoop");
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                T.EXCEPTION(e, "MainLoop");
                T.INFO("TheWork abortado!!!");
            }
        }

        private int message_counter;
        private delegate void UpdateStatsDelegate();
        private void UpdateStats()
        {
            if (label2.InvokeRequired)
            {
                label2.Invoke(new UpdateStatsDelegate(UpdateStats));
                return;
            }
            label2.Text = string.Format("Msg: {0} Valid:{1} Err:{2}", message_counter, data_ok, data_err);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "entrante";
            textBox2.Text = "saliente";

        }

        private void label2_ParentChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text)) return;
            entrante.Name = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text)) return;
            saliente.Name = textBox2.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            worker.Abort();
            worker.Join(2000);
            Application.Exit();
        }

        private void QueueLoopback_Closed(object sender, EventArgs e)
        {
            worker.Abort();
            worker.Join(2000);
            Application.Exit();
        }
    }
}