#region Usings

using System;
using System.Linq;
using System.Windows.Forms;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Scheduler.Tasks.Mantenimiento;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Vehiculos;
using System.Collections.Generic;

#endregion

namespace HandlerTest
{
    public partial class Tester : Form
    {
        private readonly DAOFactory DaoFactory;
        
        public Tester()
        {
            InitializeComponent();
            SessionHelper.CreateSession();
            DaoFactory = new DAOFactory();            
        }

        private void Tester_Load(object sender, EventArgs e)
        {
            InitializeGenerador();
            mapControl1.Click += mapControl1_Click;
            mapControl1.Initialized += mapControl1_Initialized;
        }

        private void mapControl1_Initialized(object sender, EventArgs e)
        {
            InitializeMap();
            InitializeEntidades();
            InitializePosition();
            InitializeMessages();
        }

        private void Tester_FormClosing(object sender, FormClosingEventArgs e)
        {
            SessionHelper.CloseSession();
            DaoFactory.Dispose();            
            timerGen.Stop();
			Environment.Exit(0);
        }

        private void btDatamart_Click(object sender, EventArgs e)
        {
            var par = "";
            if(cbVehiculosDatamart.SelectedItems.Count > 0)
            {
                par += "Vehiculos=" + string.Join(",", cbVehiculosDatamart.SelectedItems.Cast<Coche>().Select(c => c.Id.ToString()).ToArray ()) + ";";
            }

            var desde = dtDesdeDatamart.Value.Date;
            var hasta = dthastaDatamart.Value.Date.AddDays(1);

            par += "Desde=" + desde.ToString() + ";";
            par += "Hasta=" + hasta.ToString() + ";";

            par += "Regenera=" + (chkDatamartRegenera.Checked ? "True" : "False") + ";";

            var datamart = new DatamartGeneration();
            datamart.SetParameters(par);
            datamart.Execute(null);
        }


        private void btCargarDistribucion_Click(object sender, EventArgs e)
        {
            CargarDistribucion();
        }

        public void CargarDistribucion()
        {
            if (Coche == null) return;
            var distribucion = DaoFactory.ViajeDistribucionDAO.FindEnCurso(Coche);
            if(distribucion == null)
            {
                lblEntregas.Text = "El vehiculo no tiene una distribución";
                cbEntregas.Items.Clear();
                btEntregaCompletada.Enabled = false;
                btEntregaCancelada.Enabled = false;
                return;
            }

            lblEntregas.Text = distribucion.Codigo;
            cbEntregas.DataSource = distribucion.Detalles.Where(x => x.PuntoEntrega != null && x.PuntoEntrega.Nomenclado).ToList();
            btEntregaCompletada.Enabled = true;
            btEntregaCancelada.Enabled = true;
        }

        private void btEntregaCompletada_Click(object sender, EventArgs e)
        {
            if(cbEntregas.SelectedItem == null) return;

            var item = cbEntregas.SelectedItem as EntregaDistribucion;
            var xdata = new List<Int64> {item.Id};
            var ev = MessageIdentifier.GarminStopStatusDone.FactoryEvent(MessageIdentifier.GenericMessage, Dispositivo.Id,
                                                                        0, null, DateTime.UtcNow, null, xdata);
            SendEvent(ev);
        }
        private void btEntregaCancelada_Click(object sender, EventArgs e)
        {
            if (cbEntregas.SelectedItem == null) return;

            var item = cbEntregas.SelectedItem as EntregaDistribucion;
            var xdata = new List<Int64> { item.Id };
            var ev = MessageIdentifier.GarminStopStatusDeleted.FactoryEvent(MessageIdentifier.GenericMessage, Dispositivo.Id,
                                                                        0, null, DateTime.UtcNow, null, xdata);
            SendEvent(ev);
        }
        private void SendEvent(IMessage message)
        {
            SuspendLayout();
            if (message == null)
            {
                MessageBox.Show("Mensaje null");
                return;
            }
            Watch.Start();
            Enqueue(message);

            ResumeLayout(true);
        }

        

        
    }
}
