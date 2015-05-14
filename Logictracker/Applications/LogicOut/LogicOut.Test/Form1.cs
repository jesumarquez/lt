using System;
using System.Windows.Forms;
using LogicOut.Core;

namespace LogicOut.Test
{
    public partial class Form1 : Form
    {
        protected OutService Service;

        public Form1()
        {
            InitializeComponent();
            Service = new OutService();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Service.Process();
        }
    }
}
