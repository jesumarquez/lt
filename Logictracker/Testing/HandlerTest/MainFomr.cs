using System;
using System.Windows.Forms;
using HandlerTest.Classes;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace HandlerTest
{
    public partial class MainFomr : Form, ITestApp
    {
        public IPosition Position { get { return ucPositions1; } }
        public ICiclo Ciclo { get { return ucCiclo1; }}
        public IConfig Config { get { return ucConfig1; } }
        public IMensajeria Mensajeria { get { return null; } }
        public Data Data { get; set; }
        public Empresa Empresa { get { return cbEmpresa.SelectedItem as Empresa; } }
        public Linea Linea { get { return cbLinea.SelectedItem as Linea; } }
        public Coche Coche { get { return cbVehiculo.SelectedItem as Coche; } }
        public Dispositivo Dispositivo { get { var coche = Coche; return coche == null ? null : coche.Dispositivo; } }
        public event EventHandler CocheChanged;
        public event EventHandler BaseChanged;

        public MainFomr()
        {
            InitializeComponent();
        }

        private void MainFomr_Load(object sender, EventArgs e)
        {
            Data = new Data();
            ucPositions1.MapInitialized += ucPositions1_MapInitialized;
        }

        void ucPositions1_MapInitialized(object sender, EventArgs e)
        {
            cbEmpresa.DataSource = Data.GetEmpresas();
        }

        private void cbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbLinea.DataSource = Data.GetLineas(Empresa);
        }

        private void cbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbVehiculo.DataSource = Data.GetVehiculos(Empresa, Linea);
            if (BaseChanged != null) BaseChanged(this, EventArgs.Empty);
        }

        private void cbVehiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CocheChanged != null) CocheChanged(this, EventArgs.Empty);
        }

        private void MainFomr_FormClosing(object sender, FormClosingEventArgs e)
        {
            Data.Dispose();
            Environment.Exit(0);
        }
    }
}
