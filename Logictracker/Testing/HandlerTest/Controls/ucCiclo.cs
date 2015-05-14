using System;
using System.Linq;
using System.Windows.Forms;
using HandlerTest.Classes;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace HandlerTest.Controls
{
    public partial class ucCiclo : UserControl, ICiclo
    {
        public ITestApp TestApp { get { return ParentForm as ITestApp; } }
        public event EventHandler MostrarEnMapaChanged;
        public event EventHandler DistribucionChanged;

        public ViajeDistribucion Distribucion { get; set; }

        public bool MostrarEnMapa { get { return chkMostrarEnMapa.Checked; } }

        public ucCiclo()
        {
            InitializeComponent();
        }
        
        private void ucCiclo_Load(object sender, EventArgs e)
        {
            if(TestApp != null) TestApp.CocheChanged += TestApp_CocheChanged;
        }

        void TestApp_CocheChanged(object sender, EventArgs e)
        {
            SetCiclo(TestApp.Coche);
        }

        public void SetCiclo(Coche vehiculo)
        {
            if (vehiculo == null) return;
            Distribucion = TestApp.Data.GetDistribucion(vehiculo);
            if (Distribucion == null)
            {
                lblEntregas.Text = "El vehiculo no tiene una distribución";
                cbEntregas.Items.Clear();
                btEntregaCompletada.Enabled = false;
                btEntregaCancelada.Enabled = false;
                return;
            }

            lblEntregas.Text = Distribucion.Codigo;
            cbEntregas.DataSource = Distribucion.Detalles.Where(x => x.PuntoEntrega != null && x.PuntoEntrega.Nomenclado).ToList();
            btEntregaCompletada.Enabled = true;
            btEntregaCancelada.Enabled = true;

            if (DistribucionChanged != null) DistribucionChanged(this, EventArgs.Empty);
        }

        private void btEntregaCompletada_Click(object sender, EventArgs e)
        {
            //SendEvent(MessageIdentifier.StopStatusCompleted);
        }
        private void btEntregaCancelada_Click(object sender, EventArgs e)
        {
            //SendEvent(MessageIdentifier.StopStatusCancelled);
        }
        private void SendEvent(MessageIdentifier messageIdentifier)
        {
            if (cbEntregas.SelectedItem == null) return;

            var item = cbEntregas.SelectedItem as EntregaDistribucion;
            var evt = Sender.CreateGenericEvent(messageIdentifier, TestApp.Dispositivo, DateTime.UtcNow, new Int64[] { item.Id });

            Sender.Enqueue(TestApp.Config.Queue, evt);
        }

        private void chkMostrarEnMapa_CheckedChanged(object sender, EventArgs e)
        {
            if (MostrarEnMapaChanged != null) MostrarEnMapaChanged(this, EventArgs.Empty);
        }
    }
}
