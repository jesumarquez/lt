using System;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.GsTraq;
using Logictracker.Layers;
using Logictracker.Layers.MessageQueue;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Positions;
using System.Windows.Forms; 

namespace HandlerTest
{
    public partial class Tester
    {
    	readonly Timer _timerSend = new Timer();
        private readonly Random _random = new Random(DateTime.Now.Millisecond);
        private bool _running;
        private void InitializeGenerador()
        {
            BindVehiculosGen();
            _timerSend.Interval = 1000;
            _timerSend.Tick += TimerSendTick;
        }

        protected void BindVehiculosGen()
        {
            var list = DaoFactory.CocheDAO.GetList(new[] { -1 }, new[] { -1 })
                .Where(coche => coche.Dispositivo != null)
                .Select(coche => new Vehiculo(coche))
                .ToList();
            cbVehiculosGen.DataSource = list;
            cbVehiculosGen.DisplayMember = "Descripcion";
        }

        protected void BtGeneradorClick(object sender, EventArgs e)
        {
            if (_running)
            {
                timerGen.Stop();
                _running = false;
                btGenerador.Text = @"Iniciar";
            }
            else
            {
                timerGen.Interval = Convert.ToInt32(numFrecuenciaGen.Value * 1000);
                timerGen.Start();
                _running = true;
                btGenerador.Text = @"Detener";
            }
            numFrecuenciaGen.Enabled = !_running;
            cbVehiculosGen.Enabled = !_running;
            txtQueueName.Enabled = !_running;
        }
        private void BtSelectGenClick(object sender, EventArgs e)
        {
            var all = cbVehiculosGen.CheckedItems.Count != cbVehiculosGen.Items.Count;
            for (var i = 0; i < cbVehiculosGen.Items.Count; i++)
            {
                cbVehiculosGen.SetItemChecked(i, all);
            }
        }

        private void ChkVeloAutoGenCheckedChanged(object sender, EventArgs e)
        {
            numVeloGen.Enabled = !chkVeloAutoGen.Checked;
        }

        private void TimerGenTick(object sender, EventArgs e)
        {
            panSendGen.Visible = true;
            _timerSend.Start();

            GenerarPosicion();
        }
        private void Button6Click(object sender, EventArgs e)
        {
            GenerarPosicion();
        }

        void GenerarPosicion()
        {
        	foreach (var message in from Vehiculo item in cbVehiculosGen.CheckedItems
                                    select DaoFactory.CocheDAO.FindById(item.Id)
        	                        into coche
        	                        let lastPosition =
        	                        	GetLastPosition(coche.Dispositivo) ??
        	                        	new LogUltimaPosicionVo(new LogPosicion
        	                        	                        	{
        	                        	                        		Latitud = -34.6,
        	                        	                        		Longitud = -58.6,
        	                        	                        		Coche = coche,
        	                        	                        		Dispositivo = coche.Dispositivo
        	                        	                        	})
        	                        let velocidad =
        	                        	!chkVeloAutoGen.Checked
        	                        		? Convert.ToInt32(numVeloGen.Value)
        	                        		: (lastPosition.Velocidad > 0
        	                        		   	? _random.Next(90)
        	                        		   	: _random.Next(5) > 1 ? 0 : _random.Next(20) + 1) let latSign = _random.Next(1) == 1
        	                        let lonSign = _random.Next(1) == 1
        	                        let latitud =
        	                        	velocidad == 0
        	                        		? lastPosition.Latitud
        	                        		: lastPosition.Latitud + ((_random.NextDouble()*0.0006)*(latSign ? 1 : -1))
        	                        let longitud =
        	                        	velocidad == 0
        	                        		? lastPosition.Longitud
        	                        		: lastPosition.Longitud + ((_random.NextDouble()*0.0006)*(lonSign ? 1 : -1))
        	                        select GetPosition(coche.Dispositivo, velocidad, (float) latitud, (float) longitud))
        	{
				message.GeoPoints[0].Date = new DateTime(
					message.GeoPoints[0].Date.Year
					,message.GeoPoints[0].Date.Month
					,message.GeoPoints[0].Date.Day
					,message.GeoPoints[0].Date.Hour
					,message.GeoPoints[0].Date.Minute
					,message.GeoPoints[0].Date.Second
					,DateTimeKind.Utc
					);
        		SendPosition(message);
        	}
        }

        void SendPosition(Position message)
        {
        	BaseCodec node;
			if (radTDGte.Checked)
				node = new Logictracker.Trax.v1.Parser();
			else if (radTDUnetelv1.Checked)
				node = new Logictracker.Unetel.v1.Parser();
			else if (radTDUnetelv2.Checked)
				node = new Logictracker.Unetel.v2.Parser();
			else if (radTDFulmar.Checked)
				node = new Logictracker.FulMar.Parser();
			else if (radTDEnfora.Checked)
				node = new Logictracker.MiniMT.v1.Parser();
			else if (radTDGlobalSat.Checked)
				node = new Parser();
			else if (radDisp.Checked) //no eligio ningun tipo de dispositivo entonces se envia directo al dispatcher
			{
				Enqueue(message);
				return;
			}
			else throw new NotSupportedException(@"Por favor, elija una opción");

        	node.Id = message.DeviceId;
			/*var buff = node.Encode(message);
			var message2 = node.Decode(new Frame(buff));

			if (!message.GeoPoints[0].Equivalent(((Position)message2).GeoPoints[0]))
				MessageBox.Show(String.Format(@"Error en el parser Original:{0}; Convertida:{1}", message.GeoPoints[0], ((Position)message2).GeoPoints[0]));
			Enqueue(message2);
			//*/
		}


        void TimerSendTick(object sender, EventArgs e)
        {
            _timerSend.Stop();
            panSendGen.Visible = false;
        }

		private void Enqueue(object message)
        {
            try
            {
				var umq = new IMessageQueue
				{
					QueueName = txtQueueName.Text,
				};

				if (umq.LoadResources())
				{
					umq.Send(message);
				}
            }
            catch
            {
            }
        }
    }

	public class Vehiculo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Vehiculo(Coche coche)
        {
            Id = coche.Id;
            Descripcion = String.Format("[{0}] {1}.{2}",
                                        coche.Dispositivo.TipoDispositivo.Fabricante,
                                        coche.Empresa != null
                                            ? coche.Linea != null
                                                  ? coche.Empresa.RazonSocial + "." + coche.Linea.Descripcion
                                                  : coche.Empresa.RazonSocial
                                            : "Todos",
                                        coche.Interno);
        }
    }
}
