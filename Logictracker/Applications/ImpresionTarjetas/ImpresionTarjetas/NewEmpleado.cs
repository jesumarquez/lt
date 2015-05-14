using System;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tarjetas;

namespace TarjetasSAI
{
    public partial class NewEmpleado : Form
    {
        private DataAccess data;
        private string pictureFile;
        public NewEmpleado()
        {
            InitializeComponent();
        }

        private void btGetUpcode_Click(object sender, EventArgs e)
        {
            var upcode = data.GetNextUpcode();
            if(upcode != null)
            {
                txtUpcode.Text = upcode.code;    
            }
            else
            {
                MessageBox.Show("No hay upcodes disponibles");
                txtUpcode.Text = string.Empty;
            }
        }

        private void NewEmpleado_Load(object sender, EventArgs e)
        {
            if (data == null)
            {
                data = new DataAccess();
                LoadEmpresas();
            }
        }
        private void LoadEmpresas()
        {
            cbEmpresa.DataSource = data.GetAllEmpresas().ToList();
        }
        private void btCancelar_Click(object sender, EventArgs e)
        {
            pictureFile = null;
            Close();
        }

        private void btAceptar_Click(object sender, EventArgs e)
        {
            var img = new byte[0];
            if (pictureFile != null)
            {
                img = File.ReadAllBytes(pictureFile);
            }
            var empresa = cbEmpresa.SelectedItem as empresa;
            var emp = data.GetEmpleadoByLegajo(empresa.id, txtLegajo.Text);

            bool create = (emp == null);
            if (create) emp = new empleado { legajo = txtLegajo.Text};

            emp.empresa = empresa.id;
            emp.apellido = txtApellido.Text;
            emp.nombre = txtNombre.Text;
            emp.documento = txtDocumento.Text;
            emp.puesto = txtPuesto.Text;
            emp.foto = pictureFile != null ? new Binary(img) : null;
            emp.editado = DateTime.Now;

            var upcode = data.GetUpcodeByCode(txtUpcode.Text);

            if (upcode == null)
            {
                MessageBox.Show("Error en Upcode");
                return;
            }

            emp.upcode = upcode.code;
            emp.code = upcode.image;

            upcode.used = true;
            
            if(create) data.AddEmpleado(emp);
            data.SubmitChanges();
            pictureFile = null;
            Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            pictureFile = openFileDialog1.FileName;

            pictureBox1.ImageLocation = pictureFile;
        }

        public void ShowEmpleado(empleado empleado)
        {
            if (data == null)
            {
                data = new DataAccess();
                LoadEmpresas();
            }
            cbEmpresa.SelectedItem = cbEmpresa.Items.Cast<empresa>().FirstOrDefault(x => x.id == empleado.empresa);
            txtLegajo.Text = empleado.legajo;
            txtLegajo_TextChanged(this, EventArgs.Empty);
        }
        private void txtLegajo_TextChanged(object sender, EventArgs e)
        {
            var empresa = cbEmpresa.SelectedItem as empresa;
            var emp = data.GetEmpleadoByLegajo(empresa.id, txtLegajo.Text.Trim());
            if (emp == null)
            {
                ClearForm();
                return;
            }

            if (emp.foto != null)
            {
                try
                {
                    byte[] foto = emp.foto.ToArray();
                    if (foto.Length > 1)
                    {
                        var ms = new MemoryStream();
                        for (int i = 0; i < foto.Length; i++) ms.WriteByte(foto[i]);
                        pictureBox1.Image = Image.FromStream(ms);
                        ms.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("Error obteniendo foto.");
                }
            }
            else
            {
                pictureBox1.Image = null;
            }

            txtLegajo.Text = emp.legajo;

            txtApellido.Text = emp.apellido;
            txtNombre.Text = emp.nombre;
            txtDocumento.Text = emp.documento;
            txtPuesto.Text = emp.puesto;
            txtUpcode.Text = emp.upcode;
        }
        private void ClearForm()
        {
            txtApellido.Text = 
            txtNombre.Text = 
            txtDocumento.Text = 
            txtPuesto.Text = 
            txtUpcode.Text = string.Empty;
            pictureBox1.Image = null;
        }
    }
}
