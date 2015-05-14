using System;
using System.Windows.Forms;
using Urbetrack.Postal.DataSources;
using System.Text;
using System.Security.Cryptography;

namespace Urbetrack.Postal.Forms
{
    public partial class Login : Form
    {
        SQLiteDataSet sq = new SQLiteDataSet();

        public Login()
        {
            InitializeComponent();
            if (sq.readTable("distribuidores")) 
            {
            if (sq.dataSet.Tables[0].Rows.Count == 1)
            {
                txtUser.Text = sq.dataSet.Tables[0].Rows[0]["usuario"].ToString();
                txtPass.Focus();
            }
                } 

        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (sq.read(String.Concat("SELECT * FROM distribuidores where usuario = '", txtUser.Text, "'"))) 
            {

            if (sq.dataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("Usuario incorrecto");
                txtUser.SelectAll(); 
                txtUser.Focus();                
            } else
            {
                if (sq.dataSet.Tables[0].Rows[0]["clave"].ToString() == GetMd5(txtPass.Text))
                {
                    txtPass.Text = "";
                    //cargar servicios

                    Rutas.Init();

                    var servicios = new ListServicios();
                    servicios.Show();
                    servicios.BringToFront();
                }
                else
                {
                    MessageBox.Show("Contraseña incorrecta");
                    txtPass.SelectAll();
                    txtPass.Focus();
                }
            }
            } else
            {
                MessageBox.Show("No se puede acceder a los datos");
            }
        }

        private void menuSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string GetMd5(string strSource)
        {

            var encoding = new ASCIIEncoding();



            var ary = encoding.GetBytes(strSource);



            return GetMd5(ary);

        }


        private  string GetMd5(byte[] ary)
        {

            var md5 = new MD5CryptoServiceProvider();



            var hash = md5.ComputeHash(ary);

            var hashValue = "";



            foreach (var e in hash)

                if (e <= 15) hashValue += "0" + e.ToString("X");

                else hashValue += e.ToString("X");



            return hashValue;

        }
    }
}