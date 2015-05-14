using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HandlerTest.Classes;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.ValueObject.Positions;

namespace HandlerTest.Controls
{
    public partial class ucGenerador : UserControl
    {
        public ITestApp TestApp { get { return ParentForm as ITestApp; } }
        private readonly Random Random = new Random(DateTime.Now.Millisecond);
        private bool running;

        public ucGenerador()
        {
            InitializeComponent();
            timer1.Interval = 1000;
        }

        private void ucGenerador_Load(object sender, EventArgs e)
        {
            if (TestApp != null)
            {
                var source = new BindingSource();
                var vehiculos = TestApp.Data.GetVehiculos(-1, -1).Select(x => new Classes.Vehiculo(x));
                source.DataSource = new SortableBindingList<Classes.Vehiculo>(vehiculos); ;
                dataGridView1.DataSource = source;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var text = textBox1.Text;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                currencyManager1.SuspendBinding();

                var match = string.IsNullOrEmpty(text) || row.Cells.Cast<DataGridViewCell>().Where(x => x.Value != null).Any(x => x.Value.ToString().ToLower().Contains(text.ToLower()));
                row.Visible = match;

                currencyManager1.ResumeBinding();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView1.Rows[e.RowIndex] as DataGridViewRow;
            var vehiculo = row.DataBoundItem as Classes.Vehiculo;
            vehiculo.Selected = !vehiculo.Selected;
            dataGridView1.Refresh();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            var check = dataGridView1.Rows.Cast<DataGridViewRow>()
                .Where(x => x.Visible)
                .Any(x => (x.DataBoundItem as Classes.Vehiculo).Selected);
            
            chkAll.Checked = !check;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.Visible) continue;
                var vehiculo = row.DataBoundItem as Classes.Vehiculo;
                vehiculo.Selected = !check;
            }
            dataGridView1.Refresh();
        }

        private void btEnviar_Click(object sender, EventArgs e)
        {
            GenerarPosiciones();
        }

        private void btIniciar_Click(object sender, EventArgs e)
        {
            timer1.Interval = Convert.ToInt32(numFrecuenciaGen.Value * 1000);
            timer1.Start();

            numFrecuenciaGen.Enabled = false;
            btEnviar.Enabled = false;
            btIniciar.Enabled = false;
            btDetener.Enabled = true;
        }

        private void btDetener_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            numFrecuenciaGen.Enabled = true;
            btEnviar.Enabled = true;
            btIniciar.Enabled = true;
            btDetener.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (running) return;
            try
            {
                running = true;
                GenerarPosiciones();
            }
            catch
            {
                throw;
            }
            finally
            {
                running = false;
            }
        }

        void GenerarPosiciones()
        {
            var selected = dataGridView1.Rows.Cast<DataGridViewRow>()
                .Select(x => x.DataBoundItem as Classes.Vehiculo)
                .Where(x => x.Selected);

            foreach (var vehiculo in selected)
            {
                GenerarPosicion(vehiculo);
            }
        }

        void GenerarPosicion(Classes.Vehiculo vehiculo)
        {
            const double radius = 0.0006;
            var coche = TestApp.Data.DaoFactory.CocheDAO.FindById(vehiculo.Id);
            var lastPosition = TestApp.Data.GetLastPosition(coche)
                               ?? new LogUltimaPosicionVo(new LogPosicion { Latitud = -34.6, Longitud = -58.6, Coche = coche, Dispositivo = coche.Dispositivo });
            var velocidad = Math.Max(lastPosition.Velocidad + (Random.Next(20)-10), 0);
            var dLatitud = velocidad == 0 ? lastPosition.Latitud
                                         : lastPosition.Latitud + ((Random.NextDouble() * radius * 2) - radius);
            var dLongitud = velocidad == 0 ? lastPosition.Longitud
                                          : lastPosition.Longitud + ((Random.NextDouble() * radius * 2) - radius);
            var latitud = lastPosition.Latitud + dLatitud;
            var longitud = lastPosition.Longitud + dLongitud;

            var position = Sender.CreatePosition(coche.Dispositivo, DateTime.UtcNow, latitud, longitud, velocidad);
            Sender.Enqueue(TestApp.Config.Queue, position);
        }
    }
}
