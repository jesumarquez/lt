using System;
using Logictracker.Messages.Saver;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;
using System.Windows.Forms;

namespace HandlerTest
{
    public partial class Tester
    {
        public Mensaje Mensaje { get { return cbMensaje.SelectedItem as Mensaje; } }

        private void InitializeMessages()
        {
            
        }
        private void BindMessages()
        {
            cbMensaje.DataSource = DaoFactory.MensajeDAO.FindByEmpresaYLineaAndUser(Empresa, Linea, null);
            cbMensaje.DisplayMember = "Descripcion";
        }
        private void SendMessage()
        {
            try
            {
                var saver = new MessageSaver(DaoFactory);
                var inicio = new GPSPoint(DateTime.UtcNow, (float) Latitud, (float) Longitud);
                saver.Save(Mensaje.Codigo, Coche, inicio.Date, inicio, txtTextoMensaje.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btSendMessage_Click(object sender, EventArgs e)
        {
            SendMessage();
        }
    }
}
