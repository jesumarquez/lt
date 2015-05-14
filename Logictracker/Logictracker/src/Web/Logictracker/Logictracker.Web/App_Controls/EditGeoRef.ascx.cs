using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.HtmlControls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using Point = Logictracker.Web.Monitor.Geometries.Point;

namespace Logictracker.App_Controls
{
    public partial class EditGeoRef : BaseUserControl
    {
        #region const

        /// <summary>
        /// Nivel de zoom minimo del mapa
        /// </summary>
        protected const int MinZoomLevel = 4;

        /// <summary>
        /// Nivel de zoom para las busquedas
        /// </summary>
        protected const int SearchZoomLevel = 13;

        /// <summary>
        /// Nombre del layer de Puntos de interes
        /// </summary>
        protected static string LayerPoi { get { return CultureManager.GetLabel("LAYER_POI"); } }

        /// <summary>
        /// Nombre del layer de areas de los puntos de interes
        /// </summary>
        protected static string LayerAreasPoi { get { return CultureManager.GetLabel("LAYER_POI_AREA"); } }


        #endregion

        #region Private Properties

        /// <summary>
        /// Points for the current pologonal geocerca.
        /// </summary>
        private List<PointF> CurrentPolygon
        {
            get { return ViewState["Currentpolygon"] as List<PointF>; }
            set { ViewState["Currentpolygon"] = value; }
        }

        /// <summary>
        /// Radio for a current circular geocerca.
        /// </summary>
        private int Radio
        {
            get { return (int)(ViewState["CircleRadio"] ?? 0); }
            set
            {
                if (value == 0) ViewState["CircleRadio"] = null;
                else ViewState["CircleRadio"] = value;
            }
        }

        #endregion

        public void BorrarDireccion()
        {
            monitor.ClearLayer(LayerPoi);
            DireccionSearch1.SetDireccion(null);
        }
        public void BorrarGeocerca()
        {
            monitor.ClearLayer(LayerAreasPoi);
            Radio = 0;
            CurrentPolygon = null;
        }
        public Direccion Direccion
        {
            get { return DireccionSearch1.Selected; }
            set { DireccionSearch1.SetDireccion(value); }
        }
        public Poligono Poligono
        {
            get
            {
                if (CurrentPolygon == null) return null;
                var pol = new Poligono();
                pol.AddPoints(CurrentPolygon);
                pol.Radio = Radio;
                return pol;
            }
            set
            {
                CurrentPolygon = value != null ? value.ToPointFList() : null;
                Radio = value != null ? value.Radio : 0;
                AddGeometry(CurrentPolygon, Radio);
            }
        }
        public void ClearIcono()
        {
            IconPath = null;
            ViewState["IconWidth"] =
                ViewState["IconHeight"] =
                ViewState["IconOffsetX"] =
                ViewState["IconOffsetY"] = null;
            ChangeIcon();
        }

        public void SetIcono(int idIcono)
        {
            SetIcono(DAOFactory.IconoDAO.FindById(idIcono));
        }
        public void SetIcono(Icono icono)
        {
            SetIcono(Config.Directory.IconDir + icono.PathIcono, icono.Width, icono.Height, icono.OffsetX, icono.OffsetY);
        }

        public void SetIcono(string path, short width, short height, short offsetx, short offsety)
        {
            IconPath = path;
            IconWidth = width;
            IconHeight = height;
            IconOffsetX = offsetx;
            IconOffsetY = offsety;
            ChangeIcon();
        }
        protected string IconPath
        {
            get { return (string)(ViewState["IconPath"] ?? ResolveUrl("~/images/salida.gif")); }
            set { ViewState["IconPath"] = value; }
        }
        protected short IconWidth
        {
            get { return ViewState["IconWidth"] != null ? (short)ViewState["IconWidth"] : (short)20; }
            set { ViewState["IconWidth"] = value; }
        }
        protected short IconHeight
        {
            get { return ViewState["IconHeight"] != null ? (short)ViewState["IconHeight"] : (short)20; }
            set { ViewState["IconHeight"] = value; }
        }
        protected short IconOffsetX
        {
            get { return ViewState["IconOffsetX"] != null ? (short)ViewState["IconOffsetX"] : (short)-10; }
            set { ViewState["IconOffsetX"] = value; }
        }
        protected short IconOffsetY
        {
            get { return ViewState["IconOffsetY"] != null ? (short)ViewState["IconOffsetY"] : (short)-19; }
            set { ViewState["IconOffsetY"] = value; }
        }
        public Color Color
        {
            get { return (Color)(ViewState["Color"] ?? Color.Blue); }
            set { ViewState["Color"] = value; ChangeColor(); }
        }

        public void SetCenter(double latitud, double longitud)
        {
            monitor.SetDefaultCenter(latitud, longitud);
            monitor.SetCenter(latitud, longitud);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            monitor.Click += MonitorClick;
            monitor.DrawPolygon += MonitorDrawPolygon;
            monitor.DrawCircle += MonitorDrawCircle;

            if (!IsPostBack)
            {
                // Map Style
                var link = new HtmlGenericControl("link");
                link.Attributes.Add("rel", "stylesheet");
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("href", ResolveUrl("~/App_Styles/openlayers.css"));
                Page.Header.Controls.AddAt(0, link);

                CurrentPolygon = null;
                Radio = 0;
                monitor.EnableTimer = false;
                monitor.ClickMethod = EventMethods.PostBack;
                monitor.DrawPolygonMethod = EventMethods.PostBack;
                monitor.DrawCircleMethod = EventMethods.PostBack;
                monitor.ImgPath = Config.Monitor.GetMonitorImagesFolder(Page);
                monitor.GoogleMapsScript = Config.Map.GoogleMapsKey;
                monitor.DefaultMarkerIcon = "salida.png";
                monitor.AddLayers(LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), MinZoomLevel),
                                  //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, MIN_ZOOM_LEVEL),
                                  LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                                  LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), MinZoomLevel),
                                  LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), MinZoomLevel),
                                  LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), MinZoomLevel),
                                  LayerFactory.GetVector(LayerAreasPoi, true),
                                  LayerFactory.GetMarkers(LayerPoi, true));

                monitor.AddControls(ControlFactory.GetLayerSwitcher(),
                                    ControlFactory.GetNavigation(),
                                    ControlFactory.GetPanZoomBar(),
                                    ControlFactory.GetToolbar(true, true, false, false, false, false, false, false, LayerAreasPoi));

                SetCenter(-34.6, -58.6);
            }
        }

        protected void MonitorDrawCircle(object sender, MonitorDrawCircleEventArgs e)
        {
            var point = new PointF((float)e.Longitud, (float)e.Latitud);

            CurrentPolygon = new List<PointF> { point };
            Radio = e.Radio;

            AddCircle(point, e.Radio);
        }

        protected void MonitorDrawPolygon(object sender, MonitorDrawPolygonEventArgs e)
        {
            Radio = 0;
            CurrentPolygon = e.Points;

            AddPolygon(e.Points);
        }

        void MonitorClick(object sender, MonitorClickEventArgs e)
        {
            DireccionSearch1.FindByLatLon(e.Latitud, e.Longitud);
        }
        protected void DireccionSearch1DireccionSelected(object sender, EventArgs e)
        {
            AddMarker();
        }

        /// <summary>
        /// Adds a polygonal geocerca.
        /// </summary>
        /// <param name="points"></param>
        protected void AddPolygon(List<PointF> points)
        {
            monitor.ClearLayer(LayerAreasPoi);

            if (points == null || points.Count == 0) return;

            var poly = new Polygon("POLYGON", StyleFactory.GetPointFromColor(Color));

            for (var i = 0; i < points.Count; i++) poly.AddPoint(new Point(i.ToString("#0"), points[i].X, points[i].Y));

            monitor.AddGeometries(LayerAreasPoi, poly);

            if (Direccion == null)
            {
                var lat = points.Average(p => p.Y);
                var lon = points.Average(p => p.X);
                SetCenter(lat, lon);
            }
        }

        protected void AddGeometry(List<PointF> points, int radio)
        {
            if (radio <= 0) AddPolygon(points);
            else AddCircle(points[0], radio);

        }
        /// <summary>
        /// Adds a circle shaped geocerca.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radio"></param>
        protected void AddCircle(PointF point, int radio)
        {
            monitor.ClearLayer(LayerAreasPoi);

            if (Radio <= 0) return;

            var poly = new Point("POLYGON", point.X, point.Y, radio, StyleFactory.GetPointFromColor(Color));

            monitor.AddGeometries(LayerAreasPoi, poly);
        }

        private void ChangeIcon()
        {
            AddMarker();
        }
        private void ChangeColor()
        {
            if (CurrentPolygon == null || CurrentPolygon.Count == 0) return;
            if (Radio == 0) AddPolygon(CurrentPolygon);
            else AddCircle(CurrentPolygon[0], Radio);
        }

        private void AddMarker()
        {
            monitor.ClearLayer(LayerPoi);
            var dir = DireccionSearch1.Selected;
            if (dir == null) return;

            var marker = new Marker("1", IconPath, dir.Latitud, dir.Longitud,
                DrawingFactory.GetSize(IconWidth, IconHeight),
                DrawingFactory.GetOffset(IconOffsetX, IconOffsetY));
            monitor.AddMarkers(LayerPoi, marker);
            SetCenter(dir.Latitud, dir.Longitud);
            monitor.ZoomTo(11);
        }
    }
}