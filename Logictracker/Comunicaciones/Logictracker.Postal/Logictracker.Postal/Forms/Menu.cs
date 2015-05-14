using System;
using System.Windows.Forms;

namespace Urbetrack.Postal.Forms
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            var servicios = new ListServicios();
            servicios.Show();
            servicios.BringToFront();
            Close();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            var servicios = new ListServicios();
            servicios.Show();
            servicios.BringToFront();
            Close();

        }

        private void bSincronizar_Click(object sender, EventArgs e)
        {
            var sincronizacion = new Sincronizacion();

            sincronizacion.Show();

            Close();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            var login = new Login();
            login.ShowDialog();
            Close();
        }

    }
}