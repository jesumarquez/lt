using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1Gauge;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Web.Reportes.CicloLogistico
{
    public partial class KPIEstadoEntregas : SecuredBaseReportPage<KPIEstadoEntregas.KpiEstado>
    {
        protected override string GetRefference() { return "KPI_ESTADO_ENTREGAS"; }
        protected override string VariableName { get { return "KPI_ESTADO_ENTREGAS"; } }
        protected override InfoLabel LblInfo { get { return null; } }
        protected override bool CsvButton { get { return false; } }
        protected override bool ExcelButton { get { return false; } }
        protected override bool PrintButton { get { return false; } }
        protected override bool HideSearch { get { return true; } }

        protected override void ExportToCsv() { }
        protected override void ExportToExcel() { }
        protected override List<KpiEstado> GetResults() { return new List<KpiEstado>(); }

        public int ReportIndex 
        {
            get { return Session["ReportIndex"] != null ? Convert.ToInt32(Session["ReportIndex"]) : 0; }
            set { Session["ReportIndex"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            RegisterExtJsStyleSheet();
            gridTransportistas.Visible = false;
        }

        protected void FiltersSelectedIndexChanged(object sender, EventArgs e)
        {
            gridTransportistas.Visible = false;
        }

        protected void OnTick(object sender, EventArgs e)
        {
            ReportIndex++;
            CargarReporte();
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            ReportIndex = 0;
            CargarReporte();            
        }

        private void CargarReporte()
        {
            gridTransportistas.Visible = true;
            var desde = DateTime.Today.ToDataBaseDateTime();
            var hasta = desde.AddHours(24);
            CalcularEstadisticas(desde, hasta);
            ShowInfo(desde, hasta);
        }

        private void CalcularEstadisticas(DateTime desde, DateTime hasta)
        {
            var transportistas = DAOFactory.TransportistaDAO.GetList(new[] { ddlEmpresa.Selected }, new[] { ddlPlanta.Selected });

            var resultados = new List<KpiEstado>();

            var buscados = DAOFactory.ViajeDistribucionDAO.GetList(new[] { ddlEmpresa.Selected },
                                                                   new[] { ddlPlanta.Selected },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   new[] { -1 },
                                                                   desde,
                                                                   hasta)
                                                          .Where(v => v.Vehiculo != null && v.Vehiculo.Transportista != null);

            foreach (var transportista in transportistas)
            {
                var viajes = buscados.Where(v => v.Vehiculo.Transportista.Id == transportista.Id);

                if (!viajes.Any()) continue;

                var completados = viajes.Sum(v => v.EntregasCompletadosCount);
                var visitados = viajes.Sum(v => v.EntregasVisitadosCount);
                var realizados = completados + visitados;
                var total = viajes.Sum(v => v.EntregasTotalCount);
                var porc = total > 0 && realizados > 0 ? (double) realizados/(double) total*100 : 0.00;

                var result = new KpiEstado
                {
                    Transportista = transportista.Descripcion,
                    Rutas = viajes.Count(),
                    Entregas = total,
                    Realizados = realizados,
                    Porc = porc,
                    Completados = completados,
                    Visitados = visitados,
                    EnSitio = viajes.Sum(v => v.EntregasEnSitioCount),
                    EnZona = viajes.Sum(v => v.EntregasEnZonaCount),
                    NoCompletados = viajes.Sum(v => v.EntregasNoCompletadosCount),
                    NoVisitados = viajes.Sum(v => v.EntregasNoVisitadosCount),
                    Pendientes = viajes.Sum(v => v.EntregasPendientesCount)
                };

                resultados.Add(result);
            }

            gridTransportistas.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI07");
            gridTransportistas.Columns[1].HeaderText = CultureManager.GetLabel("RUTAS");
            gridTransportistas.Columns[2].HeaderText = CultureManager.GetLabel("ENTREGAS");
            gridTransportistas.Columns[3].HeaderText = CultureManager.GetLabel("REALIZADOS");
            gridTransportistas.Columns[4].HeaderText = CultureManager.GetLabel("PORCENTAJE");
            gridTransportistas.Columns[5].HeaderText = CultureManager.GetLabel("COMPLETADOS");
            gridTransportistas.Columns[6].HeaderText = CultureManager.GetLabel("VISITADOS");
            gridTransportistas.Columns[7].HeaderText = CultureManager.GetLabel("EN_SITIO");
            gridTransportistas.Columns[8].HeaderText = CultureManager.GetLabel("EN_ZONA");
            gridTransportistas.Columns[9].HeaderText = CultureManager.GetLabel("NO_COMPLETADOS");
            gridTransportistas.Columns[10].HeaderText = CultureManager.GetLabel("NO_VISITADOS");
            gridTransportistas.Columns[11].HeaderText = CultureManager.GetLabel("PENDIENTES");

            for (var i = 0; i < gridTransportistas.Columns.Count; i++)
            {
                gridTransportistas.Columns[i].HeaderStyle.Font.Bold = true;
                gridTransportistas.Columns[i].HeaderStyle.Font.Size = FontUnit.Larger;
                gridTransportistas.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }

            gridTransportistas.DataSource = resultados;
            gridTransportistas.DataBind();
        }

        protected void GridTransportistasOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var result = e.Row.DataItem as KpiEstado;
                if (result != null)
                {
                    var lbl = e.Row.FindControl("lblTransportista") as Label;
                    if (lbl != null) lbl.Text = result.Transportista;

                    lbl = e.Row.FindControl("lblRutas") as Label;
                    if (lbl != null) lbl.Text = result.Rutas.ToString("#0");

                    lbl = e.Row.FindControl("lblEntregas") as Label;
                    if (lbl != null) lbl.Text = result.Entregas.ToString("#0");

                    lbl = e.Row.FindControl("lblRealizados") as Label;
                    if (lbl != null) lbl.Text = result.Porc.ToString("#0") + " % (" + result.Realizados.ToString("#0") + ")";

                    var gauge = e.Row.FindControl("gaugeCompletados") as C1Gauge;
                    if (gauge != null)
                    {
                        gauge.Gauges[0].Maximum = 100;
                        gauge.Gauges[0].Value = result.Porc;
                    }

                    lbl = e.Row.FindControl("lblCompletados") as Label;
                    if (lbl != null) lbl.Text = result.Completados.ToString("#0");

                    lbl = e.Row.FindControl("lblVisitados") as Label;
                    if (lbl != null) lbl.Text = result.Visitados.ToString("#0");

                    lbl = e.Row.FindControl("lblEnSitio") as Label;
                    if (lbl != null) lbl.Text = result.EnSitio.ToString("#0");

                    lbl = e.Row.FindControl("lblEnZona") as Label;
                    if (lbl != null) lbl.Text = result.EnZona.ToString("#0");

                    lbl = e.Row.FindControl("lblNoCompletados") as Label;
                    if (lbl != null) lbl.Text = result.NoCompletados.ToString("#0");

                    lbl = e.Row.FindControl("lblNoVisitados") as Label;
                    if (lbl != null) lbl.Text = result.NoVisitados.ToString("#0");

                    lbl = e.Row.FindControl("lblPendientes") as Label;
                    if (lbl != null) lbl.Text = result.Pendientes.ToString("#0");
                }
            }
        }

        public void ShowInfo(DateTime desde, DateTime hasta)
        {
            ClearMimico();

            var distribuciones = DAOFactory.ViajeDistribucionDAO.GetList(ddlEmpresa.SelectedValues,
                                                                         ddlPlanta.SelectedValues,
                                                                         new[] { -1 }, // TRANSPORTISTAS
                                                                         new[] { -1 }, // DEPARTAMENTOS
                                                                         new[] { -1 }, // CC
                                                                         new[] { -1 }, // SUB CC
                                                                         new[] { -1 }, // COCHES
                                                                         desde,
                                                                         hasta)
                                                                .Where(v => v.Vehiculo != null)
                                                                .OrderBy(v => v.Vehiculo.Patente);

            var cantidadPorPagina = DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected).KpiCantidadPagina;
            var initialIndex = ReportIndex * cantidadPorPagina;
            var lastIndex = initialIndex + cantidadPorPagina;

            if (initialIndex >= distribuciones.Count())
            {
                ReportIndex = 0;
                initialIndex = ReportIndex * cantidadPorPagina;
                lastIndex = initialIndex + cantidadPorPagina;
            }            

            if (lastIndex > distribuciones.Count())
            {
                lastIndex = distribuciones.Count();
            }

            var ciclos = new List<Ciclo>();
            for (int i = initialIndex; i < lastIndex; i++)
            {
                var ciclo = new Ciclo(distribuciones.ElementAt(i), DAOFactory);
                ciclos.Add(ciclo);
            }

            var sh = new ScriptHelper(this);
            foreach (var ciclo in ciclos)
            {
                sh.RegisterStartupScript(string.Format("init_{0}_{1}", ciclo.Tipo, ciclo.Id), ciclo.Render(), true);
            }
        }

        private void ClearMimico()
        {
            var sh = new ScriptHelper(this);
            sh.RegisterStartupScript("clear", "clearMimicos();", true);
        }

        public class KpiEstado
        {
            public string Transportista { get; set; }
            public int Rutas { get; set; }
            public int Entregas { get; set; }
            public int Realizados { get; set; }
            public int Completados { get; set; }
            public int Visitados { get; set; }
            public int EnSitio { get; set; }
            public int EnZona { get; set; }
            public int NoCompletados { get; set; }
            public int NoVisitados { get; set; }
            public int Pendientes { get; set; }
            public double Porc { get; set; }
        }

        private class Ciclo
        {
            private const string Template = "{{ 'id': '{0}' , 'type': '{5}', 'interno': '{1}', 'style': '{2}', 'icon': '{3}', completed: {4} }}";
            public int Id { get; set; }
            public string Tipo { get; set; }
            public string Icono { get; set; }
            public string Interno { get; set; }
            public string LabelStyle { get; set; }
            public int Completed { get; set; }
            public List<Detalle> Detalles { get; set; }

            public Ciclo(ViajeDistribucion distribucion, DAOFactory daoFactory)
            {
                Id = distribucion.Id;
                Tipo = "Distribucion";
                Icono = Path.Combine(IconDir, distribucion.Vehiculo.TipoCoche.IconoDefault.PathIcono);
                Interno = distribucion.Vehiculo.Interno;
                LabelStyle = GetLabelStyle(distribucion.Vehiculo, daoFactory);
                Detalles = distribucion.Detalles.Where(d => d.PuntoEntrega != null).Select((d, i) => new Detalle(d) { Descripcion = (i + 1).ToString() }).ToList();
                Completed = GetCompleted(distribucion);
            }

            private int GetCompleted(ViajeDistribucion distribucion)
            {
                var detalles = distribucion.Detalles.Where(d => d.Linea == null);
                var cantDetalles = detalles.Count();
                var entregados = detalles.Count(detalle => detalle.Entrada.HasValue || detalle.Manual.HasValue || detalle.Salida.HasValue);

                return Convert.ToInt32(entregados * 100 / cantDetalles);
            }

            private string GetLabelStyle(Coche coche, DAOFactory daoFactory)
            {
                var upm = SharedPositions.GetLastPositions(new List<Coche> { coche }).FirstOrDefault();
                if (upm == null) return "ol_marker_labeled_red";

                string style;

                if (upm.FechaMensaje >= DateTime.UtcNow.AddMinutes(-5)) style = upm.Velocidad == 0 ? "ol_marker_labeled" : "ol_marker_labeled_green";
                else style = upm.FechaMensaje >= DateTime.UtcNow.AddHours(-48) ? "ol_marker_labeled_yellow" : "ol_marker_labeled_red";

                return style;
            }

            public string Render()
            {
                var m = string.Format(Template, Id, Interno, LabelStyle, Icono, Completed, Tipo);
                var st = string.Concat("[", string.Join(",", Detalles.Select(d => d.Render()).ToArray()), "]");

                return string.Format("addMimico('{0}', {1}, {2});", Id, m, st);
            }
        }

        private class Detalle
        {
            public int Id { get; set; }
            public string Descripcion { get; set; }
            public short Estado { get; set; }
            public string PopupText { get; set; }
            public int Duracion { get; set; }
            public int PorcentajeDelTotal { get; set; }
            public int PorcentajeParcial { get; set; }
            public bool Completado { get; set; }
            public DateTime Programado { get; set; }
            public DateTime? Automatico { get; set; }
            public double Latitud { get; set; }
            public double Longitud { get; set; }

            public Detalle(EntregaDistribucion entrega)
            {
                Id = entrega.Id;
                Descripcion = entrega.Orden.ToString();
                var evento = string.Empty;
                if (entrega.Estado == EntregaDistribucion.Estados.NoCompletado && entrega.EventosDistri.Any())
                {
                    var msj = entrega.EventosDistri.FirstOrDefault(d => d.LogMensaje.Mensaje.TipoMensaje.DeRechazo);
                    if (msj != null) evento = " - " + msj.LogMensaje.Mensaje.Descripcion;
                }

                PopupText = string.Format("<div>Cód. Cliente: <span>{6}</span></div><div>Cliente: <span>{5}</span></div><div>Estado: <span>{4}</span></div><div>Programado: <span>{0}</span><br/>Entrada: <span>{1}</span><br/>Salida: <span>{2}</span><br/>Manual: <span>{3}</span></div>",
                                  entrega.Programado.ToDisplayDateTime().ToString("HH:mm"),
                                  (entrega.Entrada.HasValue ? entrega.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  (entrega.Salida.HasValue ? entrega.Salida.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  (entrega.Manual.HasValue ? entrega.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : ""),
                                  CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado)) + evento,
                                  entrega.PuntoEntrega.Descripcion,
                                  entrega.PuntoEntrega.Codigo);

                Programado = entrega.Programado;
                Automatico = entrega.Entrada;
                Estado = entrega.Estado;
                Latitud = entrega.ReferenciaGeografica.Latitude;
                Longitud = entrega.ReferenciaGeografica.Longitude;
            }

            public string Render()
            {
                return string.Format("{{ 'id': '{0}', 'name': '{1}', 'estado': {2}, 'details': '{3}' }}",
                                Id,
                                Descripcion,
                                Estado,
                                PopupInfo().Replace("'", "\\'"));
            }
            private string PopupInfo()
            {
                return string.Format("<div class='popup_title'>{0}</div><div class='popup_detail'>{1}</div>", Descripcion, PopupText);
            }
            private string GetJsDate(DateTime? date)
            {
                return date.HasValue ? string.Concat("new Date", date.Value.ToDisplayDateTime().ToString("(yyyy,M,d,H,m)")) : "null";
            }
        }       
    }
}
