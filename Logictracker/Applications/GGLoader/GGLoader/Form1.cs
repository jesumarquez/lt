using System;
using System.IO;
using System.Windows.Forms;

namespace GGLoader
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			textBox1.MaxLength = 10;
			ReloadDrives(sender, e);
		}

		private void ReloadDrives(object sender, EventArgs e)
		{
			cb_SDCards.Items.Clear();
			cb_SDCards.Items.AddRange(checkBox2.Checked ? SDCard.GetSDCards() : SDCard.GetAllDevices());
			if (cb_SDCards.Items.Count > 0) cb_SDCards.SelectedIndex = 0;
		}

		private void AbrirGeogrilla(object sender, EventArgs e)
		{
			openFileDialog1.ShowDialog();
		}

		private void CargarGeogrilla(object sender, EventArgs e)
		{
			var path = openFileDialog1.FileName;
			if (cb_SDCards.SelectedIndex < 0)
			{
				MessageBox.Show("Seleccione una unidad donde cargar la geogrilla");
				return;
			}
			if (String.IsNullOrEmpty(path) || !File.Exists(path))
			{
				MessageBox.Show("Seleccione un archivo de geogrilla");
				return;
			}

			panel1.Enabled = false;
			SDCard.LoadGeogrillaFile(path, (String)cb_SDCards.SelectedItem, progressBar1, label2, panel1, checkBox1.Checked, textBox1.Text, Convert.ToInt32(numericUpDown1.Value) * 512);
		}
	}
}
