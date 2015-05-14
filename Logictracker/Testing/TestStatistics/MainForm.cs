#region Usings

using System;
using System.Windows.Forms;

#endregion

namespace TestStatistics
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var gt = new GaugeTest();
            gt.ShowDialog();
        }
    }
}
