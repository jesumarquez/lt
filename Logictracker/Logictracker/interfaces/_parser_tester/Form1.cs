using System;
using System.Windows.Forms;

namespace command_data
{
    public partial class Form1 : Form
    {

        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TicketsWatcher tw = new TicketsWatcher();
            tw.Initialize();
        }
    }
}