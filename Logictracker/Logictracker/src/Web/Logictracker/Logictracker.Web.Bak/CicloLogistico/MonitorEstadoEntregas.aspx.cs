using System;
using System.Linq;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Markers;
using System.Collections.Generic;
using NHibernate.Util;

namespace Logictracker.CicloLogistico
{
    public partial class MonitorEstadoEntregas : OnLineSecuredPage
    {
        protected override string GetRefference() { return "MONITOR_ESTADO_ENTREGAS"; }
        protected override InfoLabel LblInfo { get { return null; } }
        public static string LayerPuntos { get { return CultureManager.GetLabel("LAYER_POI"); } }

        private int Empresa
        {
            get
            {
                if (ViewState["Empresa"] == null)
                {
                    ViewState["Empresa"] = Session["Empresa"];
                    Session["Empresa"] = null;
                }
                return (ViewState["Empresa"] != null) ? Convert.ToInt32(ViewState["Empresa"]) : 0;
            }
            set { ViewState["Empresa"] = value; }
        }
        private int Linea
        {
            get
            {
                if (ViewState["Linea"] == null)
                {
                    ViewState["Linea"] = Session["Linea"];
                    Session["Linea"] = null;
                }
                return (ViewState["Linea"] != null) ? Convert.ToInt32(ViewState["Linea"]) : 0;
            }
            set { ViewState["Linea"] = value; }
        }
        private int Departamento
        {
            get
            {
                if (ViewState["Departamento"] == null)
                {
                    ViewState["Departamento"] = Session["Departamento"];
                    Session["Departamento"] = null;
                }
                return (ViewState["Departamento"] != null) ? Convert.ToInt32(ViewState["Departamento"]) : 0;
            }
            set { ViewState["Departamento"] = value; }
        }
        private int CentroDeCosto
        {
            get
            {
                if (ViewState["CentroDeCosto"] == null)
                {
                    ViewState["CentroDeCosto"] = Session["CentroDeCosto"];
                    Session["CentroDeCosto"] = null;
                }
                return (ViewState["CentroDeCosto"] != null) ? Convert.ToInt32(ViewState["CentroDeCosto"]) : 0;
            }
            set { ViewState["CentroDeCosto"] = value; }
        }
        private int SubCentroDeCosto
        {
            get
            {
                if (ViewState["SubCentroDeCosto"] == null)
                {
                    ViewState["SubCentroDeCosto"] = Session["SubCentroDeCosto"];
                    Session["SubCentroDeCosto"] = null;
                }
                return (ViewState["SubCentroDeCosto"] != null) ? Convert.ToInt32(ViewState["SubCentroDeCosto"]) : 0;
            }
            set { ViewState["SubCentroDeCosto"] = value; }
        }
        private int Transportista
        {
            get
            {
                if (ViewState["Transportista"] == null)
                {
                    ViewState["Transportista"] = Session["Transportista"];
                    Session["Transportista"] = null;
                }
                return (ViewState["Transportista"] != null) ? Convert.ToInt32(ViewState["Transportista"]) : 0;
            }
            set { ViewState["Transportista"] = value; }
        }
        private List<int> Vehiculos
        {
            get
            {
                if (ViewState["Vehiculos"] == null)
                {
                    ViewState["Vehiculos"] = Session["Vehiculos"];
                    Session["Vehiculos"] = null;
                }
                return (ViewState["Vehiculos"] != null) ? (List<int>)ViewState["Vehiculos"] : new List<int>();
            }
            set { ViewState["Vehiculos"] = value; }
        }
        private DateTime InitialDate
        {
            get
            {
                if (ViewState["InitialDate"] == null)
                {
                    ViewState["InitialDate"] = Session["InitialDate"];
                    Session["InitialDate"] = null;
                }
                return (ViewState["InitialDate"] != null) ? Convert.ToDateTime(ViewState["InitialDate"]) : DateTime.Today.ToDataBaseDateTime();
            }
            set { ViewState["InitialDate"] = value; }
        }
        private DateTime FinalDate
        {
            get
            {
                if (ViewState["FinalDate"] == null)
                {
                    ViewState["FinalDate"] = Session["FinalDate"];
                    Session["FinalDate"] = null;
                }
                return (ViewState["FinalDate"] != null) ? Convert.ToDateTime(ViewState["FinalDate"]) : DateTime.Today.AddDays(1).ToDataBaseDateTime();
            }
            set { ViewState["FinalDate"] = value; }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            SetExpiration();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                var googleMapsEnabled = true;
                var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
                if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
                {
                    var empresa = usuario.Empresas.First() as Empresa;
                    if (empresa != null)
                        googleMapsEnabled = empresa.GoogleMapsEnabled;
                }

                RegisterExtJsStyleSheet();
                monitorPuntos.Initialize(googleMapsEnabled);
                monitorPuntos.AddLayers(LayerFactory.GetMarkers(LayerPuntos, true));
                monitorPuntos.ZoomTo(8);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && (Request.QueryString["l"] == null || Request.QueryString["l"] != "true"))
            {
                gaugeCompletados.Gauges[0].Value = 0;
                lblCompletados.Text = "0 % (0)";
                lblComp.Text = lblVisitados.Text = lblNoCompletados.Text = 
                lblSinVisitar.Text = lblEnSitio.Text = lblPendientes.Text = 
                lblEnZona.Text = lblTotal.Text = lblRutas.Text = "0";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadViewStateParameters();
        }

        private void LoadViewStateParameters()
        {
            dtFecha.SelectedDate = InitialDate;
            cbEstado.SelectedIndex = -1;

            if (Request.QueryString["l"] != null && Request.QueryString["l"] == "true")
            {
                if (Empresa > 0) cbEmpresa.SetSelectedValue(Empresa);
                if (Linea > 0) cbLinea.SetSelectedValue(Linea);
                if (Transportista > 0) cbTransportista.SetSelectedValue(Transportista);
                if (Departamento > 0) cbDepartamento.SetSelectedValue(Departamento);
                if (CentroDeCosto > 0) cbCentroDeCostos.SetSelectedValue(CentroDeCosto);
                if (SubCentroDeCosto > 0) cbSubCentroDeCostos.SetSelectedValue(SubCentroDeCosto);
                if (Vehiculos.Count > 0) cbVehiculo.SetSelectedValues(Vehiculos.Select(v => v.ToString("#0")));
            
                BtnSearchClick(null, null);
            }
        }

        protected void CbLineaSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLinea.Selected > 0)
            {
                var linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
                if (linea.ReferenciaGeografica != null)
                    monitorPuntos.SetCenter(linea.ReferenciaGeografica.Latitude, linea.ReferenciaGeografica.Longitude);
            }
        }
            
        protected void BtnSearchClick(object sender, EventArgs e)
        {
            if (!dtFecha.SelectedDate.HasValue) return;

            InitialDate = SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value);
            FinalDate = InitialDate.AddDays(1);
            Empresa = cbEmpresa.Selected;
            Linea = cbLinea.Selected;
            Transportista = cbTransportista.Selected;
            Departamento = cbDepartamento.Selected;
            CentroDeCosto = cbCentroDeCostos.Selected;
            SubCentroDeCosto = cbSubCentroDeCostos.Selected;
            Vehiculos = cbVehiculo.SelectedValues;

            SearchPositions();
        }

        private void SearchPositions()
        {
            var distribuciones = GetDistribuciones();
            ShowPuntos(distribuciones);
            lnkDistribucionGlobal.Visible = distribuciones.Count > 0;
        }

        private List<ViajeDistribucion> GetDistribuciones()
        {
            var estadosRutas = cbEstadoRuta.SelectedValues;
            if (cbEstadoRuta.SelectedIndex == -1) estadosRutas.Add(-1);

            return DAOFactory.ViajeDistribucionDAO.GetList(new[] {Empresa},
                                                           new[] {Linea},
                                                           new[] {Transportista},
                                                           new[] {Departamento},
                                                           new[] {CentroDeCosto},
                                                           new[] {SubCentroDeCosto},
                                                           Vehiculos,
                                                           estadosRutas,
                                                           InitialDate,
                                                           FinalDate);
        }

        protected void ShowPuntos(List<ViajeDistribucion> distribuciones)
        {
            monitorPuntos.ClearLayers();
            
            var entregas = new List<EntregaDistribucion>();

            foreach (var distribucion in distribuciones)
            {
                entregas.AddRange(distribucion.Detalles.Where(d => d.Linea == null));
            }

            var completados = entregas.Count(e => e.Estado == EntregaDistribucion.Estados.Completado);
            var visitados = entregas.Count(e => e.Estado == EntregaDistribucion.Estados.Visitado);
            var noCompletados = entregas.Count(e => e.Estado == EntregaDistribucion.Estados.NoCompletado);
            var sinVisitar = entregas.Count(e => e.Estado == EntregaDistribucion.Estados.SinVisitar);
            var enSitio = entregas.Count(e => e.Estado == EntregaDistribucion.Estados.EnSitio);
            var pendientes = entregas.Count(e => e.Estado == EntregaDistribucion.Estados.Pendiente);
            var enZona = entregas.Count(e => e.Estado == EntregaDistribucion.Estados.EnZona);

            var estados = GetEstados();
            var filtradas = entregas.Where(e => estados.Count == 0 || estados.Contains(e.Estado));

            var markers = new List<Marker>();
            var lat = 0.0;
            var lon = 0.0;
            foreach (var entrega in filtradas)
            {
                var punto = entrega.ReferenciaGeografica;
                if (punto != null)
                {
                    string url;
                    var style = string.Empty;
                    var text = string.Empty;
                    lat = punto.Latitude;
                    lon = punto.Longitude;

                    switch (entrega.Estado)
                    {
                        case EntregaDistribucion.Estados.Completado:
                            url = ResolveUrl("~/images/Green-Ball-icon.png");
                            style = "ol_marker_labeled_green";
                            text = entrega.Manual.HasValue 
                                        ? entrega.Orden + " (" + entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") + ")"
                                        : entrega.Orden.ToString("#0");
                            break;
                        case EntregaDistribucion.Estados.Visitado:
                            url = ResolveUrl("~/images/Yellow-Ball-icon.png");
                            style = "ol_marker_labeled_yellow";
                            text = entrega.Entrada.HasValue
                                        ? entrega.Orden + " (" + entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") + ")"
                                        : entrega.Orden.ToString("#0");
                            break;
                        case EntregaDistribucion.Estados.NoCompletado:
                            url = ResolveUrl("~/images/Red-Ball-icon.png");
                            style = "ol_marker_labeled_red";
                            text = entrega.Manual.HasValue 
                                        ? entrega.Orden + " (" + entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") + ")"
                                        : entrega.Orden.ToString("#0");
                            break;
                        case EntregaDistribucion.Estados.EnSitio:
                            url = ResolveUrl("~/images/Blue-Ball-icon.png");
                            style = "ol_marker_labeled_blue";
                            text = entrega.Entrada.HasValue
                                        ? entrega.Orden + " (" + entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") + ")"
                                        : entrega.Orden.ToString("#0");
                            break;
                        case EntregaDistribucion.Estados.EnZona:
                            url = ResolveUrl("~/images/Grey-Ball-icon.png");
                            style = "ol_marker_labeled";
                            text = entrega.Entrada.HasValue
                                        ? entrega.Orden + " (" + entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") + ")"
                                        : entrega.Orden.ToString("#0");
                            break;
                        default:
                            url = ResolveUrl("~/images/Orange-Ball-icon.png");
                            break;
                    }

                    if (style != string.Empty)
                    {
                        var lmarker = MarkerFactory.CreateLabeledMarker("P:" + punto.Id, url, punto.Latitude, punto.Longitude, text, style, GetEntregaPopupContent(entrega.Id));
                        markers.Add(lmarker);                        
                    }
                    else
                    {
                        var marker = MarkerFactory.CreateMarker("P:" + punto.Id, url, punto.Latitude, punto.Longitude, GetEntregaPopupContent(entrega.Id));
                        markers.Add(marker);
                    }
                }

                if (lat != 0.0 && lon != 0.0) monitorPuntos.SetCenter(lat, lon);
            }

            monitorPuntos.AddMarkers(LayerPuntos, markers.ToArray());

            var porc = 0.0;
            var finalizados = completados + visitados + enSitio + enZona;
            if (entregas.Count > 0 && finalizados > 0)
                porc = (float)finalizados / (float)entregas.Count* 100;
                
            gaugeCompletados.Gauges[0].Value = porc;
            lblCompletados.Text = porc.ToString("#0") + " % (" + finalizados + ")";
            lblComp.Text = completados.ToString("#0");
            lblVisitados.Text = visitados.ToString("#0");
            lblNoCompletados.Text = noCompletados.ToString("#0");
            lblSinVisitar.Text = sinVisitar.ToString("#0");
            lblEnSitio.Text = enSitio.ToString("#0");
            lblPendientes.Text = pendientes.ToString("#0");
            lblEnZona.Text = enZona.ToString("#0");
            lblTotal.Text = entregas.Count.ToString("#0");
            lblRutas.Text = distribuciones.Count.ToString("#0");
        }

        private void SetExpiration()
        {
            gaugeCompletados.AbsoluteExpiration = DateTime.Today.AddDays(-1);
            gaugeCompletados.SlidingExpiration = new TimeSpan(0, 0, 1);
        }

        private List<int> GetEstados()
        {
            return cbEstado.SelectedIndex != -1 ? cbEstado.SelectedValues : new List<int>();
        }

        private static string GetEntregaPopupContent(int idEntrega)
        {
            return "javascript:getEntregaP('" + idEntrega + "')";
        }

        protected void LnkDistribucionGlobal(object sender, EventArgs e)
        {
            AddSessionParameters();

            OpenWin("DistribucionGlobalDespachos.aspx?l=true", "Distribución Global");
        }

        private void AddSessionParameters()
        {
            Session.Add("Empresa", Empresa);
            Session.Add("Linea", Linea);
            Session.Add("Departamento", Departamento);
            Session.Add("CentroDeCosto", CentroDeCosto);
            Session.Add("SubCentroDeCosto", SubCentroDeCosto);
            Session.Add("Transportista", Transportista);
            Session.Add("Vehiculos", Vehiculos);
            Session.Add("InitialDate", InitialDate);
            Session.Add("FinalDate", FinalDate);
        }
    }
}