using System;
using Logictracker.Configuration;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Markers;
using Logictracker.Culture;
using Logictracker.Types.ReportObjects;
using NHibernate.Util;

namespace Logictracker.Monitor.MonitorUltimaPosicion
{
    public partial class Monitor_UltimaPosicion : ApplicationSecuredPage
    {
        #region Private Properties

        /// <summary>
        /// Urls constant paths.
        /// </summary>
        private static string ClarinMapsUrl { get { return Config.Map.CompumapTiles; } }
        private string ImgUrl { get { return Config.Monitor.GetMonitorImagesFolder(this); } }
        private static readonly string CurrentPositionImgUrl = string.Concat(ApplicationPath, "OpenLayers/img/marker-blue.png");

        /// <summary>
        /// Contant map zoom levels configuration.
        /// </summary>
        private const int InitialZoomLevel = 10;
        private const int MinZoomLevel = 6;

        /// <summary>
        /// Events constant values.
        /// </summary>
        private const string PopupEvent = "mousedown";

        /// <summary>
        /// Layers constant names.
        /// </summary>
        private string _hybrid;
        private string _compumap;
        private string _physical;
        private string _posicion;
        private string _satellite;
        private string _street;

        /// <summary>
        /// The selected mobile.
        /// </summary>
        private int Mobile
        {
            get
            {
                if (ViewState["Mobile"] == null)
                {
                    ViewState["Mobile"] = Session["Mobile"];
                    Session["Mobile"] = null;
                }

                return (ViewState["Mobile"] != null) ? Convert.ToInt32(ViewState["Mobile"]) : 0;
            }
            set { ViewState["Mobile"] = value; }
        }

        /// <summary>
        /// The selected device.
        /// </summary>
        private int Device
        {
            get
            {
                if (ViewState["Device"] == null)
                {
                    ViewState["Device"] = Session["Device"];
                    Session["Device"] = null;
                }

                return (ViewState["Device"] != null) ? Convert.ToInt32(ViewState["Device"]) : 0;
            }
            set { ViewState["Device"] = value; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return null; } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Searchs route positions filtered by the selected search criteria.
        /// </summary>
        private void SearchPosition()
        {
            MobilePosition lastPosition = null;
            if(Mobile > 0)
            {
                lastPosition = ReportFactory.MobilePositionDAO.GetMobileLastPosition(DAOFactory.CocheDAO.FindById(Mobile));
            }
            else if(Device > 0)
            {
                var coche = DAOFactory.CocheDAO.FindMobileByDevice(Device);
                if(coche != null) lastPosition = ReportFactory.MobilePositionDAO.GetMobileLastPosition(coche);
            }

            if (lastPosition == null) return;

            DisplayPosition(lastPosition);
        }

        /// <summary>
        /// Load layes labels.
        /// </summary>
        private void LoadLayersLabels()
        {
            _hybrid = CultureManager.GetLabel("LAYER_GHIBRIDO");
            _satellite = CultureManager.GetLabel("LAYER_GSAT");
            _street = CultureManager.GetLabel("LAYER_GSTREET");
            _physical = CultureManager.GetLabel("LAYER_GFISICO");
            _compumap = CultureManager.GetLabel("LAYER_COMPUMAP");
            _posicion = CultureManager.GetLabel("POSICION");
        }

        /// <summary>
        /// Displays the last position reported by the givenn mobile or device.
        /// </summary>
        /// <param name="lastPosition"></param>
        private void DisplayPosition(MobilePosition lastPosition)
        {
            var id = lastPosition.IdPosicion.ToString();
            var latitude = lastPosition.Latitud;
            var longitude = lastPosition.Longitud;

            var popup = string.Format("javascript:gPP('{0}','{1}','{2}','{3}','{4}','{5}')", lastPosition.EsquinaCercana, lastPosition.Interno, lastPosition.Responsable,
                lastPosition.Dispositivo, string.Concat(lastPosition.Fecha.Value.ToShortDateString(), "", lastPosition.Fecha.Value.TimeOfDay.ToString()), lastPosition.Velocidad);

            var marker = new Marker(id, CurrentPositionImgUrl, latitude, longitude, popup, DrawingFactory.GetSize(21, 25), DrawingFactory.GetOffset(-10.5, -25));

            Monitor.AddMarkers(_posicion, marker);

            Monitor.SetCenter(latitude, longitude);

            Monitor.SetDefaultCenter(latitude, longitude);

            Monitor.TriggerEvent(id, _posicion, PopupEvent);
        }

        /// <summary>
        /// Initial map set up.
        /// </summary>
        private void InitializeMap()
        {
            Monitor.ImgPath = ImgUrl;
            Monitor.EnableTimer = false;
            Monitor.MultiplePopUps = true;
            Monitor.GoogleMapsScript = GoogleMapsKey;

            var googleMapsEnabled = true;
            var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
            if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
            {
                var empresa = usuario.Empresas.First() as Empresa;
                if (empresa != null)
                    googleMapsEnabled = empresa.GoogleMapsEnabled;
            }

            if (googleMapsEnabled)
            {
                Monitor.AddLayers(LayerFactory.GetGoogleHybrid(_hybrid, MinZoomLevel),
                                    LayerFactory.GetGoogleSatellite(_satellite, MinZoomLevel),
                                    LayerFactory.GetGoogleStreet(_street, MinZoomLevel),
                                    LayerFactory.GetGooglePhysical(_physical, MinZoomLevel),
                /*                  LayerFactory.GetCompumap(_compumap, ClarinMapsUrl, MinZoomLevel), LayerFactory.GetMarkers(_posicion, true), */
                                    LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")));
            }
            else
            {
                Monitor.AddLayers(LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")));
            }

            Monitor.AddControls(ControlFactory.GetLayerSwitcher(), ControlFactory.GetNavigation(), ControlFactory.GetPanZoomBar());

            Monitor.ZoomTo(InitialZoomLevel);
        }

        /// <summary>
        /// Sets up initial filter values according to how the page was called.
        /// </summary>
        private void SetInitialFilterValues()
        {
            var mobile = Request.QueryString["Mobile"];
            var device = Request.QueryString["Device"];

            if (!string.IsNullOrEmpty(mobile)) Mobile = Convert.ToInt32(mobile);
            if (!string.IsNullOrEmpty(device)) Device = Convert.ToInt32(device);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets initial filter values.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreLoad(EventArgs e)
        {
            SetInitialFilterValues();

            base.OnPreLoad(e);
        }

        /// <summary>
        /// Performs initial search.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            LoadLayersLabels();

            base.OnLoad(e);

            var displayData = Mobile > 0 || Device > 0;

            if (displayData)
            {
                InitializeMap();

                SearchPosition();
            }

            tblNoData.Visible = !displayData;
        }

        /// <summary>
        /// Gets the module refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "REP_TOMAS"; }

        #endregion
    }
}
