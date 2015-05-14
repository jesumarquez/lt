using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Urbetrack.GatewayMovil
{
    public partial class Nomenclaturas : Form
    {
        public Nomenclaturas()
        {
            InitializeComponent();
        }

        private void Nomenclaturas_Load(object sender, EventArgs e)
        {
            webBrowser1.DocumentText = Recursos.nomencla.ToString();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void Nomenclaturas_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
