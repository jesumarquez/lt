using System;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tarjetas;

namespace TarjetasSAI
{
    public partial class Back : UserControl
    {
        private const double ScaleX = 16.18421052631579;
        private const double ScaleY = 17.24295774647887;

        public DataAccess Data { get; set; }
        private string pictureFile;
        private back fondo = new back();
        private bool changing = false;


        public Back()
        {
            InitializeComponent();

#if DLS
            button3.Enabled = groupBox1.Enabled = false;
#endif
        }

        private void Back_Load(object sender, EventArgs e)
        {
            if(Data != null) BindFondos();
        }

        private void BindFondos()
        {
            listBox1.DataSource = Data.GetAllBacks();
            if(listBox1.Items.Count == 0)
            {
                show(new back());
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changing) return;
            var item = listBox1.SelectedItem as back;
            show(item);
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            pictureFile = openFileDialog1.FileName;

            pictureBox1.BackgroundImage = Image.FromFile(pictureFile);
        }
        private void btGuardar_Click(object sender, EventArgs e)
        {
            var img = new byte[0];
            if (pictureFile != null)
                img = File.ReadAllBytes(pictureFile);
            fondo.nombre = textBox1.Text;
            fondo.print_apellido = chkApellido.Checked;
            fondo.print_documento = chkDocumento.Checked;
            fondo.print_foto = chkFoto.Checked;
            fondo.print_legajo = chkLegajo.Checked;
            fondo.print_nombre = chkNombre.Checked;
            fondo.print_upcode = chkUpcode.Checked;

            if (fondo.id == 0)
            {
                fondo.image = pictureFile != null ? new Binary(img) : null;

                if (fondo.image == null)
                {
                    MessageBox.Show("Debe seleccionar una imagen de fondo");
                    return;
                }

                Data.AddBack(fondo);
            }
            else if(pictureFile != null)
            {
                fondo.image = new Binary(img);
            }

            Data.SubmitChanges();
            pictureFile = null;
            BindFondos();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureFile = null;

            show(new back());
            //textBox1.Text = string.Empty;
            //pictureBox1.BackgroundImage = null;

            
        }
        private void show(back item)
        {
            try
            {
                if (item == null)
                {
                    textBox1.Text = string.Empty;
                    pictureBox1.BackgroundImage = null;
                    fondo = new back();
                    return;
                }
                textBox1.Text = item.nombre;
                chkApellido.Checked = item.print_apellido;
                chkDocumento.Checked = item.print_documento;
                chkFoto.Checked = item.print_foto;
                chkLegajo.Checked = item.print_legajo;
                chkNombre.Checked = item.print_nombre;
                chkUpcode.Checked = item.print_upcode;

                lblUbicarItem.Text = "(ninguno)";
                numLeft.Value = 0;
                numTop.Value = 0;

                lblPlaceLegajo.Top = Convert.ToInt32(item.legajo_top/ScaleY);
                lblPlaceLegajo.Left = Convert.ToInt32(item.legajo_left/ScaleX);

                lblPlaceDocumento.Top = Convert.ToInt32(item.documento_top / ScaleY);
                lblPlaceDocumento.Left = Convert.ToInt32(item.documento_left / ScaleX);

                lblPlaceNombre.Top = Convert.ToInt32(item.nombre_top / ScaleY);
                lblPlaceNombre.Left = Convert.ToInt32(item.nombre_left/ ScaleX);

                lblPlaceApellido.Top = Convert.ToInt32(item.apellido_top / ScaleY);
                lblPlaceApellido.Left = Convert.ToInt32(item.apellido_left / ScaleX);

                lblPlaceUpcode.Top = Convert.ToInt32(item.upcode_top / ScaleY);
                lblPlaceUpcode.Left = Convert.ToInt32(item.upcode_left/ ScaleX);

                lblPlaceFoto.Top = Convert.ToInt32(item.foto_top / ScaleY);
                lblPlaceFoto.Left = Convert.ToInt32(item.foto_left/ ScaleX);

                if (item.image != null)
                {
                    byte[] foto = item.image.ToArray();
                    if (foto.Length > 1)
                    {
                        var ms = new MemoryStream();
                        for (int i = 0; i < foto.Length; i++) ms.WriteByte(foto[i]);
                        pictureBox1.BackgroundImage = Image.FromStream(ms);
                        ms.Close();
                    }
                }
                else
                {
                    pictureBox1.BackgroundImage = null;
                }
                fondo = item;
            }
            catch
            {
                MessageBox.Show("Error obteniendo foto.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var item = listBox1.SelectedItem as back;
            if(item == null) return;

            var todos = Data.GetAllBacks();
            foreach (var f in todos)
            {
                f.active = false;
            }

            item.active = true;
            Data.SubmitChanges();
            BindFondos();
        }

        private void SelectPlaceControl(string nombre, int currentTop, int currentLeft)
        {
            lblUbicarItem.Text = "(ninguno)"; // para evitar quilombo con numPlace_ValueChanged()

            numTop.Value = currentTop;
            numLeft.Value = currentLeft;

            lblUbicarItem.Text = nombre;
        }
        private void numPlace_ValueChanged(object sender, EventArgs e)
        {
            if (changing) return;
            changing = true;
            var item = lblUbicarItem.Text;

            switch (item)
            {
                case "Legajo":
                    fondo.legajo_top = Convert.ToInt32(numTop.Value);
                    fondo.legajo_left = Convert.ToInt32(numLeft.Value);
                    lblPlaceLegajo.Top = Convert.ToInt32(fondo.legajo_top / ScaleY);
                    lblPlaceLegajo.Left = Convert.ToInt32(fondo.legajo_left / ScaleX);
                    break;
                case "Documento":
                    fondo.documento_top = Convert.ToInt32(numTop.Value);
                    fondo.documento_left = Convert.ToInt32(numLeft.Value);
                    lblPlaceDocumento.Top = Convert.ToInt32(fondo.documento_top / ScaleY);
                    lblPlaceDocumento.Left = Convert.ToInt32(fondo.documento_left / ScaleX);
                    break;
                case "Nombre":
                    fondo.nombre_top = Convert.ToInt32(numTop.Value);
                    fondo.nombre_left = Convert.ToInt32(numLeft.Value);
                    lblPlaceNombre.Top = Convert.ToInt32(fondo.nombre_top / ScaleY);
                    lblPlaceNombre.Left = Convert.ToInt32(fondo.nombre_left / ScaleX);
                    break;
                case "Apellido": 
                    fondo.apellido_top = Convert.ToInt32(numTop.Value);
                    fondo.apellido_left = Convert.ToInt32(numLeft.Value);
                    lblPlaceApellido.Top = Convert.ToInt32(fondo.apellido_top / ScaleY);
                    lblPlaceApellido.Left = Convert.ToInt32(fondo.apellido_left / ScaleX);
                    break;
                case "Upcode":
                    fondo.upcode_top = Convert.ToInt32(numTop.Value);
                    fondo.upcode_left= Convert.ToInt32(numLeft.Value);
                    lblPlaceUpcode.Top = Convert.ToInt32(fondo.upcode_top / ScaleY);
                    lblPlaceUpcode.Left = Convert.ToInt32(fondo.upcode_left / ScaleX);
                    break;
                case "Foto":
                    fondo.foto_top = Convert.ToInt32(numTop.Value);
                    fondo.foto_left= Convert.ToInt32(numLeft.Value);
                    lblPlaceFoto.Top = Convert.ToInt32(fondo.foto_top / ScaleY);
                    lblPlaceFoto.Left = Convert.ToInt32(fondo.foto_left / ScaleX);
                    break;
            }
            changing = false;
        }

        private void chkPlace_CheckedChanged(object sender, EventArgs e)
        {
            lblPlaceLegajo.Visible = chkLegajo.Checked;
            lblPlaceDocumento.Visible = chkDocumento.Checked;
            lblPlaceNombre.Visible = chkNombre.Checked;
            lblPlaceApellido.Visible = chkApellido.Checked;
            lblPlaceUpcode.Visible = chkUpcode.Checked;
            lblPlaceFoto.Visible = chkFoto.Checked;
        }

        private void lblPlaceLegajo_Click(object sender, EventArgs e)
        {
            SelectPlaceControl("Legajo", fondo.legajo_top, fondo.legajo_left);
        }
        private void lblPlaceDocumento_Click(object sender, EventArgs e)
        {
            SelectPlaceControl("Documento", fondo.documento_top, fondo.documento_left);
        }
        private void lblPlaceNombre_Click(object sender, EventArgs e)
        {
            SelectPlaceControl("Nombre", fondo.nombre_top, fondo.nombre_left);
        }
        private void lblPlaceApellido_Click(object sender, EventArgs e)
        {
            SelectPlaceControl("Apellido", fondo.apellido_top, fondo.apellido_left);
        }
        private void lblPlaceUpcode_Click(object sender, EventArgs e)
        {
            SelectPlaceControl("Upcode", fondo.upcode_top, fondo.upcode_left);
        }
        private void lblPlaceFoto_Click(object sender, EventArgs e)
        {
            SelectPlaceControl("Foto", fondo.foto_top, fondo.foto_left);
        }

        private void Back_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Up:
                    if(numTop.Value > numTop.Minimum) numTop.Value--;
                    break;
                case Keys.Down:
                    if (numTop.Value < numTop.Maximum) numTop.Value++;
                    break;
                case Keys.Left:
                    if (numLeft.Value > numLeft.Minimum) numLeft.Value--;
                    break;
                case Keys.Right:
                    if (numLeft.Value < numLeft.Maximum) numLeft.Value++;
                    break;
                case Keys.PageUp:
                    if (numTop.Value > numTop.Minimum + 10) numTop.Value -= 10;
                    else if (numTop.Value > numTop.Minimum) numTop.Value = numTop.Minimum;
                    break;
                case Keys.PageDown:
                    if (numTop.Value < numTop.Maximum - 10) numTop.Value += 10;
                    else if (numTop.Value < numTop.Maximum) numTop.Value = numTop.Maximum;
                    break;
                case Keys.Home:
                    if (numLeft.Value > numLeft.Minimum + 10) numLeft.Value -= 10;
                    else if (numLeft.Value > numLeft.Minimum) numLeft.Value = numLeft.Minimum;
                    break;
                case Keys.End:
                    if (numLeft.Value < numLeft.Maximum - 10) numLeft.Value += 10;
                    else if (numLeft.Value < numLeft.Maximum) numLeft.Value = numLeft.Maximum;
                    break;
            }
            numPlace_ValueChanged(sender, e);
        }
    }
}
