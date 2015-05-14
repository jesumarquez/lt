using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Forms;

namespace Urbetrack.Postal.Forms
{
    public partial class Camara : Form
    {
        public Byte[] Image { get; set; }

        public Boolean Acepto { get; set; }

        private void TomarFoto()
        {
            var cameraCapture = new CameraCaptureDialog();
            cameraCapture.Mode = CameraCaptureMode.Still;
            cameraCapture.Resolution = Configuration.Resolution;
            cameraCapture.Owner = this;
            var result = cameraCapture.ShowDialog();

            if (result != DialogResult.OK)
            {
                Acepto = false;
                Close();
                return;
            }
            else
            {
                Acepto = true;
                try
                {
                    using (var file = File.OpenRead(cameraCapture.FileName))
                    {

                        var bitmap = new Bitmap(file);

                        pictureBox1.Image = bitmap;

                        Image = new byte[file.Length];

                        file.Read(Image, 0, (int) file.Length);
                        file.Close();
                    }

                    File.Delete(cameraCapture.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Se produjo un error");
                }
            }
        }

        public Camara()
        {
            InitializeComponent();
        }

        private void Camara_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                // Soft Key 1
                // Not handled when menu is present.
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                // Soft Key 2
                // Not handled when menu is present.
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                Close();
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.D0))
            {
                // 0
            }

        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Acepto = true;

            Close();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            TomarFoto();
        }

        private void Camara_Load(object sender, EventArgs e)
        {
            TomarFoto();
        }
    }
}