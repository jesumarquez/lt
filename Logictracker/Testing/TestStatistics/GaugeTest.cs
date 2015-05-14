#region Usings

using System;
using System.Windows.Forms;
using Urbetrack.Statistics;

#endregion

namespace TestStatistics
{
    public partial class GaugeTest : Form
    {
        private readonly Gauge g = new Gauge();
        public GaugeTest()
        {
            InitializeComponent();
        }

        private void UpdateScreen()
        {
            if (label1.InvokeRequired)
            {
                label1.Invoke(new MethodInvoker(UpdateScreen));
            }

            label1.Text = String.Format("Today:{0} Last24Hs:{1} Partial:{2} Historic:{3}", g.TodayValue,
                                        g.Last24HoursValue, g.PartialValue, g.HistoricValue);
            
            label2.Text = g.ListLast24();
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            var r = new Random();
            g.Inc((ulong)r.Next(0, 100));
            UpdateScreen();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            g.HackedNow(dateTimePicker1.Value);
        }
    }
}
