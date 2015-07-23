using System.Windows.Forms;

namespace TraxEmu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Model = new EmuVM();
            InitializeComponent();
            tbIP.DataBindings.Add("Text", Model, "IP");
            npkPort.DataBindings.Add("Value", Model, "Port");
            tbTxtCmd.DataBindings.Add("Text",Model,"CmdToSend");
            npkId.DataBindings.Add("Text", Model, "DeviceId");
        }

        public EmuVM Model { get; set; }

        private void btnEnviar_Click(object sender, System.EventArgs e)
        {
            Model.Enviar();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
