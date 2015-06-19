using System;
using System.Windows.Forms;
using HandlerTest.Classes;
using Logictracker.AVL.Messages;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;

namespace HandlerTest.Controls
{
    public partial class ucMensajeria : UserControl, IMensajeria
    {
        public ITestApp TestApp { get { return ParentForm as ITestApp; } }

        protected bool EnExceso { get; set; }
        protected LogPosicionVo InicioExceso { get; set; }

        public ucMensajeria()
        {
            InitializeComponent();
        }

        private void ucMensajeria_Load(object sender, EventArgs e)
        {
            if(TestApp != null) cbMensajes.DataSource = TestApp.Data.GetMensajes(TestApp.Coche.Empresa, TestApp.Coche.Linea);
        }

        private void btExceso_Click(object sender, EventArgs e)
        {
            var pos = TestApp.Position.LastPosition;
            if(pos == null)
            {
                MessageBox.Show("El vehiculo tiene que reportar una posición primero");
                return;
            }

            EnExceso = true;
            InicioExceso = pos;
            btExceso.Enabled = false;
            btTerminarExceso.Enabled = true;
            btCancelarExceso.Enabled = true;
        }

        private void btTerminarExceso_Click(object sender, EventArgs e)
        {
            var inicio = InicioExceso;
            var fin = TestApp.Position.LastPosition;
            var point1 = new GPSPoint(inicio.FechaMensaje, (float)inicio.Latitud, (float)inicio.Longitud, inicio.Velocidad, GPSPoint.SourceProviders.GpsProvider, 50);
            var point2 = new GPSPoint(fin.FechaMensaje, (float)fin.Latitud, (float)fin.Longitud, fin.Velocidad, GPSPoint.SourceProviders.GpsProvider, 50);
            var limit = Convert.ToInt32(txtExcesoPermitida.Value);

            var evt = new SpeedingTicket(TestApp.Dispositivo.Id, 0, point1, point2, Math.Max(inicio.Velocidad, fin.Velocidad), limit, null);

            Sender.Enqueue(TestApp.Config.Queue, evt, TestApp.Config.QueueType);
            btCancelarExceso_Click(sender, e);
        }

        private void btCancelarExceso_Click(object sender, EventArgs e)
        {
            EnExceso = false;
            InicioExceso = null;
            btExceso.Enabled = true;
            btTerminarExceso.Enabled = false;
            btCancelarExceso.Enabled = false;
        }

        private void btEnviarMensaje_Click(object sender, EventArgs e)
        {
            var mensaje = cbMensajes.SelectedItem as Mensaje;
            int codigo;
            if(!int.TryParse(mensaje.Codigo, out codigo)) return;
            var position = TestApp.Position.LastPosition;
            var now = DateTime.UtcNow;
            var point = new GPSPoint(now, (float)position.Latitud, (float)position.Longitud, position.Velocidad, GPSPoint.SourceProviders.GpsProvider, 50);
            var device = TestApp.Dispositivo;
            var mi = (MessageIdentifier) Enum.ToObject(typeof (MessageIdentifier), codigo);
            var evt = mi.FactoryEvent(device.Id, 0, point, now, null, new Int64[0]);
            Sender.Enqueue(TestApp.Config.Queue, evt, TestApp.Config.QueueType);
        }

        

        
    }
}
