#region Usings

using System;
using System.Windows.Forms;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.InterQ.Server
{
    public partial class InterQStandalone : Form
    {
        public InterQStandalone()
        {
            T.DefaultContext.OutputRedirect += DefaultContext_OutputRedirect;
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
            numericUpDown1.Value = T.DefaultContext.CurrentLevel;
            T.INFO("--- INTER-QUEUES STANDALONE INICIADO --");
            Remoting.DumpAllInfoAboutRegisteredRemotingTypes();
        }

        private delegate void DefaultContext_OutputRedirectHandler(string cat, string line);

        void DefaultContext_OutputRedirect(string cat, string line)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new DefaultContext_OutputRedirectHandler(DefaultContext_OutputRedirect), cat, line);
            } else
            {
                AppendLog(cat, line);
            }
        }

        void AppendLog(string cat, string line)
        {
            // si no es la primer linea, avanzo una para evitar que tome el formato de la seleccion.
            if (rtbLog.Text.Length != 0)
                rtbLog.AppendText("\r\n");
            // configuro el formato
            switch (cat)
            {
                case "INFO   ":
                    rtbLog.SelectionBackColor = System.Drawing.Color.Green;
                    rtbLog.SelectionColor = System.Drawing.Color.White;
                    break;
                case "NOTICE ":
                    rtbLog.SelectionBackColor = System.Drawing.Color.Blue;
                    rtbLog.SelectionColor = System.Drawing.Color.White;
                    break;
                case "WARNING":
                    rtbLog.SelectionBackColor = System.Drawing.Color.Yellow;
                    rtbLog.SelectionColor = System.Drawing.Color.Black;
                    break;
                case "ERROR  ":
                    rtbLog.SelectionBackColor = System.Drawing.Color.Red;
                    rtbLog.SelectionColor = System.Drawing.Color.Yellow;
                    break;
                case "SWITCH ":
                    rtbLog.SelectionBackColor = System.Drawing.Color.Black;
                    rtbLog.SelectionColor = System.Drawing.Color.Purple;
                    break;
                case "MARK   ":
                    rtbLog.SelectionBackColor = System.Drawing.Color.Cyan;
                    rtbLog.SelectionColor = System.Drawing.Color.Black;
                    break;
                default:
                    rtbLog.SelectionBackColor = System.Drawing.Color.Black;
                    rtbLog.SelectionColor = System.Drawing.Color.Gray;
                    break;
            }
            // inserto el texto.
            rtbLog.AppendText(line);
            
            if (!cbAutoscroll.Checked) return;

            rtbLog.ScrollToCaret();
            rtbLog.SelectionStart = rtbLog.Text.Length;
        }

        private void SpineStandalone_FormClosing(object sender, FormClosingEventArgs e)
        {
            InterQServer.Stop();
            Process.Exit(Process.ExitCodes.Voluntary, "el usuario cerro la ventana Standalone.");
        }

        private int mark_cnt;
        private void button1_Click(object sender, EventArgs e)
        {
            T.MARK(" ------------ INTER-QUEUES STANDALONE MARK {0} ------------ ", mark_cnt++);
            Remoting.DumpAllInfoAboutRegisteredRemotingTypes();
        }

        private void rtbLog_SelectionChanged(object sender, EventArgs e)
        {
            rtbLog.Copy();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rtbLog.Text = "";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            T.DefaultContext.CurrentLevel = Convert.ToInt32(numericUpDown1.Value);
            T.TRACE(0, "Cambiando nivel del trazador a: {0}", Convert.ToInt32(numericUpDown1.Value));
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            T.DefaultContext.CurrentLevel = 0;
            T.TRACE(0, "Reseteando nivel del trazador a: 0");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            rtbLog.ScrollToCaret();
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.Focus();   
        }

        private void InterQStandalone_Load(object sender, EventArgs e)
        {
            InterQServer.Start();
        }
    }
}