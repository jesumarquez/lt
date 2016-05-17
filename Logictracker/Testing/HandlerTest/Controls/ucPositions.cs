using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Compumap.Controls;
using Compumap.Controls.Layers;
using HandlerTest.Classes;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Compumap.Controls.Geometries;
using Compumap.Controls.BaseTypes;
using Logictracker.Types.ValueObject.Positions;
using Point = Compumap.Controls.Geometries.Point;
using Logictracker.DAL.Factories;
using Logictracker.DAL.DAO.BusinessObjects.Positions;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;

namespace HandlerTest.Controls
{
    public partial class ucPositions : UserControl, IPosition
    {
        public ITestApp TestApp { get { return ParentForm as ITestApp; } }

        public event EventHandler MapInitialized;
        public LogPosicionVo LastPosition { get { return TestApp.Data.GetLastPosition(TestApp.Coche); } }
        protected bool Initialized { get; set; }
        protected Random Random = new Random(DateTime.Now.Millisecond);
        protected Markers Layer;
        protected Vector LayerBase;
        protected Vector LayerGeocercas;

        protected double Latitud
        {
            get { double d; return double.TryParse(txtLatitud.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : 0; }
            set { txtLatitud.Text = value.ToString(CultureInfo.InvariantCulture); }
        }
        protected double Longitud
        {
            get { double d; return double.TryParse(txtLongitud.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : 0; }
            set { txtLongitud.Text = value.ToString(CultureInfo.InvariantCulture); }
        }

        protected int Velocidad
        {
            get
            {
                if (chkRandomSpeed.Checked) { txtVelocidad.Value = Random.Next(0, 100); }
                return Convert.ToInt32(txtVelocidad.Value);
            }
        }

        public ucPositions()
        {
            InitializeComponent();
        }

        private void ucPositions_Load(object sender, EventArgs e)
        {
            mapControl1.Click += mapControl1_Click;
            mapControl1.Initialized += mapControl1_Initialized;
            if(TestApp != null)
            {
                TestApp.BaseChanged += TestApp_BaseChanged;
                TestApp.CocheChanged += TestApp_CocheChanged;
                TestApp.Ciclo.MostrarEnMapaChanged += Ciclo_MostrarEnMapaChanged;
                TestApp.Ciclo.DistribucionChanged += Ciclo_DistribucionChanged;
            }
        }

        void Ciclo_DistribucionChanged(object sender, EventArgs e)
        {
            mapControl1.ClearFeatures(LayerGeocercas);
            var distribucion = TestApp.Ciclo.Distribucion;
            if(distribucion == null) { return; }
            foreach(var referencia in distribucion.Detalles.Where(x=>x.PuntoEntrega != null && x.PuntoEntrega.Nomenclado && x.PuntoEntrega.ReferenciaGeografica != null).Select(x=>x.PuntoEntrega.ReferenciaGeografica))
            {
                var polygon = GetPolygon(referencia.Id.ToString(), referencia, Color.Blue);
                mapControl1.AddGeometries(LayerGeocercas, polygon);
            }
        }

        void Ciclo_MostrarEnMapaChanged(object sender, EventArgs e)
        {
            if (!Initialized) return;
            mapControl1.SetLayerVisibility(LayerGeocercas, TestApp.Ciclo.MostrarEnMapa);
        }

        void TestApp_BaseChanged(object sender, EventArgs e)
        {
            mapControl1.ClearFeatures(LayerBase);
            if(TestApp.Linea == null || TestApp.Linea.ReferenciaGeografica == null || TestApp.Linea.ReferenciaGeografica.Poligono == null)
                return;

            var polygon = GetPolygon("base", TestApp.Linea.ReferenciaGeografica, Color.Red);
            mapControl1.AddGeometries(LayerBase, polygon);
        }

        void TestApp_CocheChanged(object sender, EventArgs e)
        {
            SetPosition();
        }

        private void mapControl1_Click(object sender, MapMouseEventArgs e)
        {
            SetPosition(e.Latitude, e.Longitude);
        }

        private void mapControl1_Initialized(object sender, EventArgs e)
        {
            Initialized = true;
            mapControl1.AddLayers(Layer = new Markers("Posicion"), LayerBase = new Vector("Base"), LayerGeocercas = new Vector("Entregas"));
            if (MapInitialized != null) MapInitialized(this, EventArgs.Empty);
        }

        private void txtLatitud_TextChanged(object sender, EventArgs e)
        {
            SetPosition(Latitud, Longitud);
        }
        private void chkRandomSpeed_CheckedChanged(object sender, EventArgs e)
        {
            txtVelocidad.Enabled = !chkRandomSpeed.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var position = Sender.CreatePosition(TestApp.Dispositivo, DateTime.UtcNow, Latitud, Longitud, Velocidad);
            Sender.Enqueue(TestApp.Config.Queue, position, TestApp.Config.QueueType);
        }

        private void lnkUltima_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetPosition();
        }

        public Geometry GetPolygon(string id, ReferenciaGeografica referencia, Color color)
        {
            if (referencia.Poligono.Radio == 0)
            {
                var polygon = new Polygon(id, Style.GetPolygon(color));
                foreach (Punto punto in referencia.Poligono.Puntos)
                {
                    polygon.AddPoint(new Point("", punto.Latitud, punto.Longitud));
                }
                return polygon;
            }
            else
            {
                var punto = referencia.Poligono.Puntos.Cast<Punto>().First();
                var polygon = Polygon.CreateRegularPolygon(id, Style.GetPolygon(color),
                                                           new LonLat(punto.Longitud, punto.Latitud),
                                                           referencia.Poligono.Radio, 50, 0);
                return polygon;
            }
        }

        public void SetPosition(double latitud, double longitud)
        {
            Latitud = latitud;
            Longitud = longitud;
            mapControl1.ClearFeatures(Layer);
            mapControl1.AddMarker(Layer, latitud, longitud);
            mapControl1.SetCenter(latitud, longitud, 10);
        }
        protected void SetPosition()
        {
            var pos = LastPosition;
            if (pos == null) { return; }
            SetPosition(pos.Latitud, pos.Longitud);
        }

        private void chkMostrarBase_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initialized) return;
            mapControl1.SetLayerVisibility(LayerBase, chkMostrarBase.Checked);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dao = DAOFactory.GetDao<LogPosicionDAO>();

            var posiciones = dao.GetPositionsBetweenDates(10288, new DateTime(2016, 01, 19, 15, 25, 0), new DateTime(2016, 01, 19, 15, 36, 0));
            var dispo = DAOFactory.GetDao<DispositivoDAO>().FindById(6314);

            foreach (var aux in posiciones)
            {
                var posicion = Sender.CreatePosition(dispo, aux.FechaMensaje, aux.Latitud, aux.Longitud, aux.Velocidad);
                Sender.Enqueue(TestApp.Config.Queue, posicion, TestApp.Config.QueueType);
            }

            
        }
    }
}
