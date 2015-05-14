using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace Urbetrack.Postal.Forms
{
    public partial class Sincronizacion : Form
    {
        public Sincronizacion()
        {
            InitializeComponent();
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            var menu = new Menu();

            menu.Show();

            Close();
        }

        private void btSincronizar_Click(object sender, EventArgs e)
        {
            btCancelar.Enabled = false;

            var txts = new ArrayList() { "Sincronizando", "Sincronizando.", "Sincronizando.." };

            lblSincronizar.Visible = true;

            for (var i = 0; i < 5; i++)
            {   
                lblSincronizar.Text = (string)txts[i - 3 * ((int)(i / 3))];
                lblSincronizar.Refresh();

                progressBar1.Value = i*4;
                
                Thread.Sleep(200);
            }

            DummyServices.Init();
            var lista = new ListServicios();
            lista.Show();
            Close();
        }

        private void progressBar1_ParentChanged(object sender, EventArgs e)
        {

        }

        private void lblSincronizar_ParentChanged(object sender, EventArgs e)
        {

        }
    }
}