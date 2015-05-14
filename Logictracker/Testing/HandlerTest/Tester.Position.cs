using System;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.Process.Geofences;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;
using System.Windows.Forms;

namespace HandlerTest
{
    public partial class Tester
    {
        private static ulong _messageId = 1;
	    private float Velocidad { get { return Convert.ToSingle(txtVelocidad.Value); } }

	    private LogUltimaPosicionVo GetLastPosition(Dispositivo dispositivo)
        {
            if (dispositivo == null) return null;
	        var coche = DaoFactory.CocheDAO.FindMobileByDevice(dispositivo.Id);
            return DaoFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);
        }

        private void InitializePosition()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendPosition();
        }
       
        private void SendPosition()
        {
            SuspendLayout();
            //var handler = new Positions();
            var message = GetPosition(Dispositivo, Velocidad, (float) Latitud, (float) Longitud);
            if(message == null)
            {
                MessageBox.Show("Mensaje null");
                return;
            }
            Watch.Start();
            Enqueue(message);
            //handler.HandleMessage(message);

            BindGeocercas();

            ResumeLayout(true);
        }

		private static Position GetPosition(Dispositivo dispositivo, float velocidad, float latitud, float longitud)
        {
            if (dispositivo == null) return null;
            var messgeId = GetNextMessageId();
            var date = DateTime.UtcNow;
            return (Position)new GPSPoint(date, latitud, longitud, velocidad, GPSPoint.SourceProviders.GpsProvider, 50).ToPosition(dispositivo.Id, messgeId);
        }

        
        private static ulong GetNextMessageId()
        {
            return _messageId++;
        }

        private void BindGeocercas()
        {
            cbDentro.Items.Clear();
            cbFuera.Items.Clear();
            /*
            var geocercas = GeocercaManager.GetGeocercas(Coche, DaoFactory);
            var estado = GeocercaManager.GetEstadoVehiculo(Coche, DaoFactory);
            var dentro = estado.GeocercasDentro.Select(x => x.Geocerca.Id);
            foreach (var geocerca in geocercas)
            {
                var inside = dentro.Contains(geocerca.Id);
                if (inside)
                {
                    cbDentro.Items.Add(geocerca.Descripcion);    
                }
                else
                {
                    cbFuera.Items.Add(geocerca.Descripcion);    
                }
            }*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var rg = DaoFactory.ReferenciaGeograficaDAO.GetList(new[] { Coche.Empresa != null ? Coche.Empresa.Id : -1 }, new[] { Coche.Linea != null ? Coche.Linea.Id : -1 }, new[] { -1 });
            var r = rg.FirstOrDefault();
            DaoFactory.ReferenciaGeograficaDAO.UpdateGeocercas(r);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            BindGeocercas();
        }


        protected void ShowEnviado()
        {
            lblEnviado.Visible = true;
            timerSent.Enabled = true;
            timerSent.Start();
        }

        private void timerSent_Tick(object sender, EventArgs e)
        {
            lblEnviado.Visible = false;
            timerSent.Stop();
        }
    }
}
