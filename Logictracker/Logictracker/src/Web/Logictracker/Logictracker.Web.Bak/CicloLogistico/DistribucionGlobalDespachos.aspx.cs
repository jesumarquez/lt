using System;
using System.Drawing;
using System.Linq;
using System.Web.UI.HtmlControls;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Helpers.ColorHelpers;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Control = System.Web.UI.Control;
using Point = Logictracker.Web.Monitor.Geometries.Point;
using NHibernate.Util;

namespace Logictracker.CicloLogistico
{
    public partial class DistribucionGlobalDespachos : OnLineSecuredPage
    {
        protected override string GetRefference() { return "DISTRIBUCION_GLOBAL_ENTREGAS"; }
        protected override InfoLabel LblInfo { get { return null; } }
        public static string LayerPuntos { get { return CultureManager.GetEntity("PARENTI44"); } }
        public static string LayerReferencias { get { return CultureManager.GetLabel("LAYER_POI"); } }
        
        protected const int MinZoomLevel = 4;

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
                monitorPuntos.AddLayers(LayerFactory.GetVector(LayerReferencias, true), LayerFactory.GetMarkers(LayerPuntos, true));
                monitorPuntos.ZoomTo(8);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadViewStateParameters();
        }

        private void LoadViewStateParameters()
        {
            dtFecha.SelectedDate = InitialDate;

            if (Request.QueryString["l"] != null && Request.QueryString["l"] == "true")
            {
                if (Empresa > 0) cbEmpresa.SetSelectedValue(Empresa);
                if (Linea > 0) cbLinea.SetSelectedValue(Linea);
                if (Transportista > 0) cbTransportista.SetSelectedValue(Transportista);
                if (Departamento > 0) cbDepartamento.SetSelectedValue(Departamento);
                if (CentroDeCosto > 0) cbCentroDeCostos.SetSelectedValue(CentroDeCosto);
                if (SubCentroDeCosto > 0) cbSubCentroDeCostos.SetSelectedValue(SubCentroDeCosto);
                if (Vehiculos.Count > 0) cbVehiculo.SetSelectedValues(Vehiculos.Select(v => v.ToString("#0")));
                
                SearchPositions();
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
            
            pnlResumen.Visible = false;

            SearchPositions();
        }

        private HtmlTable SetUpAndCreateTblResumen()
        {
            pnlResumen.ContentTemplateContainer.Controls.Clear();
            var tblResumen = new HtmlTable {Width = "100%", CellPadding = 2, CellSpacing = 2};

            pnlResumen.ContentTemplateContainer.Controls.Add(tblResumen);
            return tblResumen;
        }

        private static Label CreateLabel(String content)
        {
            return new Label { Text = content };
        }

        private static HtmlTableCell CreateCell(IEnumerable<Control> controls, int colspan)
        {
            var cell = new HtmlTableCell();
            if (colspan > 1)
                cell.ColSpan = colspan;

            foreach (var c in controls)
            {
                cell.Controls.Add(c);
            }
            return cell;
        }

        private static HtmlTableRow CreateRow(IEnumerable<HtmlTableCell> cells)
        {
            var tr = new HtmlTableRow();
            foreach(var c in cells)
            {
                tr.Cells.Add(c);
            }
            return tr;
        }

        private void SearchPositions()
        {
            monitorPuntos.ClearLayers();
            var distribuciones = DAOFactory.ViajeDistribucionDAO.GetList(new[] {Empresa},
                                                                         new[] {Linea},
                                                                         new[] {Transportista},
                                                                         new[] {Departamento},
                                                                         new[] {CentroDeCosto},
                                                                         new[] {SubCentroDeCosto},
                                                                         Vehiculos,
                                                                         InitialDate,
                                                                         FinalDate)
                                                                .Where(v => v.Vehiculo != null)
                                                                .OrderByDescending(v => v.Detalles.Count);

            lnkMonitorEstado.Visible = lnkMonitorEstado.Visible || distribuciones.Any();

            var entregas = 0;

            var tblResumen = SetUpAndCreateTblResumen();
            
            var rows2Add = new List<HtmlTableRow>();
            var geomList = new List<Geometry>();

            if (distribuciones.Any())
            {
                var colorGenerator = new ColorGenerator();

                var rownum = 1;
                foreach (var distribucion in distribuciones)
                {
                    entregas += distribucion.EntregasTotalCount;

                    var color = colorGenerator.GetNextColor();                    

                    var ttrr = GenTblResumenRows(rownum, distribucion, color);
                    rows2Add.Add(ttrr);

                    geomList.AddRange(GeneratePuntos(distribucion, color));
                    rownum++;
                }
                monitorPuntos.AddGeometries(LayerReferencias, geomList.ToArray());
                monitorPuntos.SetZoomOn(LayerReferencias);
            }

            var label1 = CreateLabel("Cantidad de Rutas: " + distribuciones.Count().ToString("#0"));
            label1.Attributes.CssStyle.Add("font-weight", "bold");
            var label2 = CreateLabel("Total de Entregas: " + entregas.ToString("#0"));
            label2.Attributes.CssStyle.Add("font-weight", "bold");
            var label3 = CreateLabel("&nbsp;");

            tblResumen.Rows.Add(CreateRow(new[] { CreateCell(new Control[] {label1}, 3) }));
            tblResumen.Rows.Add(CreateRow(new[] { CreateCell(new Control[] {label2}, 3) }));
            if (rows2Add.Any())
            tblResumen.Rows.Add(CreateRow(new[] { CreateCell(new Control[] {label3}, 3) }));

            foreach(var i in rows2Add)
            {
                tblResumen.Rows.Add(i);
            }
            
            pnlResumen.Update();
            pnlResumen.Visible = true;
        }

        protected void ViewMonitorCiclo(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.Attributes["id"] != string.Empty)
            OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlDistribucion(Convert.ToInt32(btn.Attributes["id"]))), "_blank");
        }

        protected HtmlTableRow GenTblResumenRows(int rownum, ViajeDistribucion distribucion, Color color)
        {
            var tr = new HtmlTableRow();

            var tdNumber = CreateCell(new Control[] { CreateLabel("#" + rownum)}, 1);

            var tdColor = new HtmlTableCell
                              {
                                  Width = "30px",
                                  InnerText = distribucion.EntregasTotalCount.ToString("#0")
                              };

            tdColor.Attributes.CssStyle.Add("background-color", ColorGenerator.HexConverter(color));
            tdColor.Attributes.CssStyle.Add("font-weight", "bold");
            tdColor.Attributes.CssStyle.Add("color", "#FFFFFF");
            tdColor.Attributes.CssStyle.Add("text-align", "center");

            var vehText = String.Empty;
            if (distribucion.Vehiculo != null)
            {
                if (!String.IsNullOrEmpty(distribucion.Vehiculo.Interno) || !String.IsNullOrEmpty(distribucion.Vehiculo.Patente))
                {

                    if (!String.IsNullOrEmpty(distribucion.Vehiculo.Interno))
                        vehText += distribucion.Vehiculo.Interno;
                    if (!String.IsNullOrEmpty(distribucion.Vehiculo.Patente))
                        vehText += (!String.IsNullOrEmpty(vehText) ? "/" : String.Empty) + distribucion.Vehiculo.Patente;
                    
                }
                if (!String.IsNullOrEmpty(vehText))
                    vehText = "(" + vehText + ")";
            }

            var label = CreateLabel(distribucion.Codigo + (!String.IsNullOrEmpty(vehText) ? BreakLine + vehText : String.Empty));

            var td = CreateCell(new Control[] { label }, 1);
            
            td.Controls.Add(label);
            td.Align = "center";

            tr.Cells.Add(tdNumber);
            tr.Cells.Add(tdColor);
            tr.Cells.Add(td);

            tr.Attributes.CssStyle.Add("border-bottom", "1px solid black");
            return tr;
        }

        protected List<Geometry> GeneratePuntos(ViajeDistribucion distribucion, Color color)
        {
            var geomList = new List<Geometry>();

            var entregas = distribucion.Detalles.Where(d => d.Linea == null);
            foreach (var entrega in entregas)
            {
                var punto = entrega.ReferenciaGeografica;
                if (punto != null)
                {
                    var radio = punto.Poligono != null && punto.Poligono.Radio > 0
                                    ? punto.Poligono.Radio : 100;
                    var geocerca = new Point("G:" + punto.Id, punto.Longitude, punto.Latitude, radio, StyleFactory.GetPointFromColor(color));                    
                    //var marker = MarkerFactory.CreateMarker("P:" + punto.Id, string.Empty, punto.Latitude, punto.Longitude, GetEntregaPopupContent(entrega.Id));                    

                    geomList.Add(geocerca);
                    //markerList.Add(marker);
                }
            }

            return geomList;
            /*            
            if (markerList.Any())
                monitorPuntos.AddMarkers(LayerPuntos, markerList.ToArray());
            if (geomList.Any())
                monitorPuntos.AddGeometries(LayerReferencias, geomList.ToArray());
            */
        }

        private static string GetEntregaPopupContent(int idEntrega)
        {
            return "javascript:getEntregaP('" + idEntrega + "')";
        }

        protected void LnkMonitorEstado(object sender, EventArgs e)
        {
            AddSessionParameters();

            OpenWin("MonitorEstadoEntregas.aspx?l=true", "Monitor Estado");
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