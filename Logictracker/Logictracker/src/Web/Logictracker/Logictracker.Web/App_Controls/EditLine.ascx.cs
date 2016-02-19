using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using Point = Logictracker.Web.Monitor.Geometries.Point;
using Logictracker.Services.Helpers;

namespace Logictracker.App_Controls
{
    public partial class App_Controls_EditLine : BaseUserControl
    {
        protected const int MinZoomLevel = 4;
        protected const string LayerRecorrido = "Recorrido";
        protected const string LayerMarkers = "Puntos";
        protected const string LayerViajeProgramado = "Viaje";
        public Logictracker.Web.Monitor.Monitor Mapa { get { return Monitor; } }

        public event EventHandler MapLoad;

        public VsProperty<List<PointF>> Points { get { return this.CreateVsProperty<List<PointF>>("Points", null); } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeMonitor();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void InitializeMonitor()
        {
            Monitor.DrawLine += MonitorDrawLine;
            Monitor.FeatureModified += MonitorFeatureModified;
            Monitor.EnableTimer = false;
            if (IsPostBack) return;

            Monitor.DrawLineMethod = EventMethods.PostBack;
            Monitor.ModFeatureMethod = EventMethods.PostBack;

            Monitor.ImgPath = Config.Monitor.GetMonitorImagesFolder(Page);
            Monitor.GoogleMapsScript = Config.Map.GoogleMapsKey;

            Monitor.AddLayers(LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), MinZoomLevel),
                              //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, MIN_ZOOM_LEVEL),
                              LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                              LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), MinZoomLevel),
                              LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), MinZoomLevel),
                              LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), MinZoomLevel),
                              LayerFactory.GetVector(LayerRecorrido, true, StyleFactory.GetHandlePoint()),
                              LayerFactory.GetMarkers(LayerMarkers, true),
                              LayerFactory.GetVector(LayerViajeProgramado, true));

            Monitor.AddControls(ControlFactory.GetLayerSwitcher(),
                                ControlFactory.GetNavigation(),
                                ControlFactory.GetPanZoomBar(),
                                ControlFactory.GetToolbar(false, false, false, true, false, true, false, false, LayerRecorrido));
            if (MapLoad != null) MapLoad(Monitor, EventArgs.Empty);
        }

        public void SetCenter(double lat, double lon)
        {
            Monitor.SetCenter(lat, lon, 10);
            Monitor.SetDefaultCenter(lat, lon);
        }

        void MonitorDrawLine(object sender, MonitorDrawLineEventArgs e)
        {
            SetLine(e.Points);
        }
        void MonitorFeatureModified(object sender, MonitorModifyFeatureEventArgs e)
        {
            SetLine(e.Points);
        }
        public void DrawViajeProgramado(int idViajeProgramado)
        {
            Monitor.ClearLayer(LayerViajeProgramado);
            if (idViajeProgramado > 0)
            {
                var viaje = DAOFactory.ViajeProgramadoDAO.FindById(idViajeProgramado);
                if (viaje != null)
                {
                    var count = viaje.Detalles.Count;
                    if (count > 1)
                    {
                        var primero = viaje.Detalles[0].PuntoEntrega;
                        var ultimo = viaje.Detalles[count - 1].PuntoEntrega;
                        var origen = new LatLon(primero.ReferenciaGeografica.Latitude, primero.ReferenciaGeografica.Longitude);
                        var destino = new LatLon(ultimo.ReferenciaGeografica.Latitude, ultimo.ReferenciaGeografica.Longitude);
                        var waypoints = new List<LatLon>();
                        for (int i = 1; i < count - 1; i++)
                        {
                            var punto = viaje.Detalles[i].PuntoEntrega;
                            var waypoint = new LatLon(punto.ReferenciaGeografica.Latitude, punto.ReferenciaGeografica.Longitude);
                            waypoints.Add(waypoint);
                        }

                        var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, waypoints.ToArray());
                        var posiciones = directions.Legs.SelectMany(l => l.Steps.SelectMany(s => s.Points));
                        var line = new Line("D:" + Color.Red.ToArgb(), StyleFactory.GetLineFromColor(Color.Red, 4, 0.5));
                        line.AddPoints(posiciones.Select(p => new Point("", p.Longitud, p.Latitud)));
                        Monitor.AddGeometries(LayerViajeProgramado, line);
                        Monitor.SetCenter(destino.Latitud, destino.Longitud);
                    }
                }
            }
        }
        public void Invertir()
        {
            var points = Points.Get();
            if (points == null || points.Count == 0) return;
            points.Reverse();
            SetLine(points);
        }
        public void Clear()
        {
            Points.Set(null);
            Monitor.ClearLayer(LayerRecorrido);
            Monitor.ClearLayer(LayerMarkers);
            Monitor.ClearLayer(LayerViajeProgramado);
        }
        public void SetLine(List<PointF> points)
        {
            var line = new Line("Recorrido", StyleFactory.GetLineFromColor(Color.Blue, 7, 0.7));
            foreach (var p in points) line.AddPoint(new Point("", p.X, p.Y));
            Monitor.AddGeometries(LayerRecorrido, line);
            Points.Set(points);
            AddMarkers();
        }
        public void AddMarkers()
        {
            var points = Points.Get();
            if (points == null || points.Count == 0)
            {
                Monitor.ClearLayer(LayerMarkers);
                return;
            }
            var inicio = points.First();
            var fin = points.Last();
            var markIni = new Marker("inicio", ResolveUrl("~/images/salida.png"), inicio.Y, inicio.X, DrawingFactory.GetSize(32, 32), DrawingFactory.GetOffset(-16, -32));
            var markFin = new Marker("fin", ResolveUrl("~/images/llegada.png"), fin.Y, fin.X, DrawingFactory.GetSize(32, 32), DrawingFactory.GetOffset(-16, -32));

            Monitor.AddMarkers(LayerMarkers, markIni, markFin);
        }

        public Unit Width
        {
            get { return Monitor.Width; }
            set { Monitor.Width = value; }
        }
        public Unit Height
        {
            get { return Monitor.Height; }
            set { Monitor.Height = value; }
        }
    }
}
