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

namespace Logictracker.App_Controls
{
    public partial class App_Controls_EditLine : BaseUserControl
    {
        protected const int MinZoomLevel = 4;
        protected const string LayerRecorrido = "Recorrido";
        protected const string LayerMarkers = "Puntos";
        public Monitor Mapa { get { return Monitor; } }

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
                              LayerFactory.GetMarkers(LayerMarkers, true));

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
