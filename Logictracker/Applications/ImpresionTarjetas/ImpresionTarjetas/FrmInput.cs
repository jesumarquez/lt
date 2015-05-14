using System.Windows.Forms;

namespace Tarjetas
{
    public partial class FrmInput : Form
    {
        public string Nombre
        { 
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public FrmInput()
        {
            InitializeComponent();
        }

        private void btCancelar_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btAceptar_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
