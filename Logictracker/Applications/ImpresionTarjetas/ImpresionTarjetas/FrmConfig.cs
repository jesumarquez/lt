using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tarjetas.Properties;
using System.Data.SqlClient;

namespace Tarjetas
{
    public partial class FrmConfig : Form
    {
        private DataAccess data;

        public FrmConfig(DataAccess data)
        {
            InitializeComponent();
            this.data = data;
            back1.Data = data;
#if DLS
            cbEmpresa.Enabled = btNuevo.Enabled = btModificar.Enabled = btEliminar.Enabled = 
                btUploadUpcode.Enabled = false;
#endif
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {
            LoadEmpresas();
            LoadUpcodes();
        }
        private void LoadEmpresas()
        {
            cbEmpresa.DataSource = data.GetAllEmpresas().ToList();
        }

        private void btNuevo_Click(object sender, EventArgs e)
        {
            using(var input = new FrmInput())
            {
                input.Nombre = string.Empty;
                if (input.ShowDialog() != DialogResult.OK) return;
                var name = input.Nombre.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("El nombre de la empresa no puede estar vacío.");
                    return;
                }
                var empresa = data.GetAllEmpresas().FirstOrDefault(x => x.nombre == name);
                if(empresa != null)
                {
                    MessageBox.Show("La empresa '" + name + "' ya existe.");
                    return;
                }
                empresa = new empresa {nombre = name};
                data.AddEmpresa(empresa);
                data.SubmitChanges();
                LoadEmpresas();
            }
        }

        private void btModificar_Click(object sender, EventArgs e)
        {
            var empresa = cbEmpresa.SelectedItem as empresa;
            if (empresa == null)
            {
                MessageBox.Show("Debe seleccionar una empresa.");
                return;
            }
            using (var input = new FrmInput())
            {
                input.Nombre = empresa.nombre;
                if (input.ShowDialog() != DialogResult.OK) return;
                var name = input.Nombre.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("El nombre de la empresa no puede estar vacío.");
                    return;
                }
                empresa.nombre = name;
                data.SubmitChanges();
                LoadEmpresas();
            }
        }

        private void btEliminar_Click(object sender, EventArgs e)
        {
            var empresa = cbEmpresa.SelectedItem as empresa;
            if (empresa == null)
            {
                MessageBox.Show("Debe seleccionar una empresa.");
                return;
            }
            if (MessageBox.Show("¿Está seguro?", "Eliminar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            data.DeleteEmpresa(empresa);
            data.SubmitChanges();
            LoadEmpresas();
        }

        private void btDownloadDb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (saveDb.ShowDialog() != DialogResult.OK) return;
                var newfilename = saveDb.FileName;
                var filename = Settings.Default.tarjetasConnectionString.Split(';').FirstOrDefault(x=>x.ToLower().StartsWith("data source")).Split('=')[1].Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
                File.Copy(filename, newfilename);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (openFileDb.ShowDialog() != DialogResult.OK) return;
                var newfilename = openFileDb.FileName;
                var filename = Settings.Default.tarjetasConnectionString.Split(';').FirstOrDefault(x => x.ToLower().StartsWith("data source")).Split('=')[1].Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
                if (MessageBox.Show("Se perderán todos los datos de la base actual. ¿Desea continuar?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                File.Copy(newfilename, filename, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadUpcodes()
        {
            var upcodes = data.GetAllUpcodes();
            var lista = upcodes.Select(x => x.code + (x.used ? " (usado)" : string.Empty)).ToList();
            cbUpcode.DataSource = lista;
            var total = upcodes.Count();
            var libres = upcodes.Count(x => !x.used);
            lblCoutUpcode.Text = string.Format("Total: {0}     Libres: {1}", total, libres);
        }

        private void btUploadUpcode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (openFileUpcode.ShowDialog() != DialogResult.OK) return;
            btUploadUpcode.Visible = false;

            foreach (var filename in openFileUpcode.FileNames)
            {
                UploadUpcode(filename);
            }
            data.SubmitChanges();
            LoadUpcodes();
            btUploadUpcode.Visible = true;
        }
        public void UploadUpcode(string filename)
        {
            int start = filename.LastIndexOf('\\') + 1;
            int end = filename.LastIndexOf('.');
            var code = filename.Substring(start, end - start);

            var upc = data.GetUpcodeByCode(code);
            bool existscode = upc != null;
            if (existscode && MessageBox.Show("El codigo " + code + " ya existe. \nDesea remplazarlo?", "Aviso", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            Bitmap bmpCrop = null;
            using (var im = Image.FromFile(filename) as Bitmap)
            {
                if (im == null) return;
                if (im.Width == 300 && im.Height == 300)
                    bmpCrop = im.Clone(new Rectangle(40, 0, 218, 256), PixelFormat.Undefined);
            }
            if (bmpCrop != null)
                bmpCrop.Save(filename, ImageFormat.Png);

            byte[] img = File.ReadAllBytes(filename);

            if (!existscode) upc = new upcode { code = code };
            upc.image = new Binary(img);
            if (!existscode) data.AddUpcode(upc);
        }
    }
}
