using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messages.Saver;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Security;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Utils;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.DropDownLists;
using Logictracker.Web.CustomWebControls.DropDownLists.Distribucion;
using Logictracker.Web.CustomWebControls.Input;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.Monitor.Markers;
using Geocoder.Core.VO;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Point = Logictracker.Web.Monitor.Geometries.Point;
using Logictracker.DAL.NHibernate;
using ListItem = System.Web.UI.WebControls.ListItem;
using C1.Web.UI.Controls.C1GridView;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class ViajeDistribucionAlta : SecuredAbmPage<ViajeDistribucion>
    {
        protected override string VariableName { get { return "CLOG_DISTRIBUCION"; } }
        protected override string RedirectUrl { get { return "ViajeDistribucionLista.aspx"; } }
        protected override string GetRefference() { return "DISTRIBUCION"; }
        protected override bool DuplicateButton { get { return true; } }
        protected override bool MapButton { get { return EditMode && EditObject.InicioReal.HasValue; } }
        protected override bool EventButton { get { return EditMode && EditObject.InicioReal.HasValue; } }
        protected override bool SaveButton 
        {
            get
            {
                return !EditMode || EditObject.Estado == ViajeDistribucion.Estados.Pendiente;
            }
        }
        protected override bool RegenerateButton
        {
            get
            {
                return EditMode && EditObject.Estado != ViajeDistribucion.Estados.EnCurso &&
                       EditObject.Vehiculo != null && EditObject.Vehiculo.Dispositivo != null;
            }
        }
        
        private VsProperty<List<Entrega>> Entregas { get { return this.CreateVsProperty("Entregas", new List<Entrega>()); } }
        private VsProperty<List<DateTime>> Horarios { get { return this.CreateVsProperty("Horarios", new List<DateTime>()); } }
        private VsProperty<List<DateTime>> Hasta { get { return this.CreateVsProperty("Hasta", new List<DateTime>()); } }
        private static string LayerClientPoi { get { return CultureManager.GetLabel("LAYER_CLIENT_POI"); } }
        private static string LayerClientArea { get { return CultureManager.GetLabel("LAYER_CLIENT_AREA"); } }

        private DateTime Inicio
        {
            get { return dtFecha.SelectedDate.HasValue ? dtFecha.SelectedDate.Value.ToDataBaseDateTime() : DateTime.UtcNow; }
        }
        private IList<DireccionVO> Direcciones
        {
            get { return (IList<DireccionVO>)(ViewState["Direcciones"] ?? new List<DireccionVO>()); }
            set { ViewState["Direcciones"] = value; }
        }
        public Direccion Selected
        {
            get { return ViewState["Selected"] as Direccion; }
            private set { ViewState["Selected"] = value; }
        }

        private TipoServicioCiclo _tipoServicioDefault;
        private TipoServicioCiclo TipoServicioDefault
        {
            get
            {
                return _tipoServicioDefault ?? (_tipoServicioDefault = DAOFactory.TipoServicioCicloDAO.FindDefault(
                                                                        new[] {cbEmpresa.Selected > 0 ? cbEmpresa.Selected : cbEmpresa.AllValue},
                                                                        new[] {cbLinea.Selected > 0 ? cbLinea.Selected : cbLinea.AllValue}));
            }
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!IsPostBack) if (!SetCenterLinea()) EditLine1.SetCenter(-34.6134981326759, -58.4255323559046);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                // Map Style
                var link = new HtmlGenericControl("link");
                link.Attributes.Add("rel", "stylesheet");
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("href", ResolveUrl("~/App_Styles/openlayers.css"));
                Page.Header.Controls.AddAt(0, link);

                if (!EditMode)
                {
                    dtFecha.SetDate();
                    dtFechaProg.SetDate();
                    dtRegeneraDesde.SetDate();
                    dtRegeneraHasta.SetDate();
                    BindEntregas();
                }
                SetCancelState();
                AbmTabPanel3.Visible = !EditMode;
            }
        }

        #region Acciones (Bind, Save, Validate, ViewMap, Regenerate)

        protected override void OnDuplicate()
        {
            base.OnDuplicate();
        }

        protected override void Bind()
        {
            DAOFactory.Session.Refresh(EditObject);

            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbTransportista.SetSelectedValue(EditObject.Transportista != null ? EditObject.Transportista.Id : cbTransportista.AllValue);
            cbCentroDeCosto.SetSelectedValue(EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : cbCentroDeCosto.AllValue);
            cbSubCentroDeCosto.SetSelectedValue(EditObject.SubCentroDeCostos != null ? EditObject.SubCentroDeCostos.Id : cbSubCentroDeCosto.AllValue);
            cbTipoVehiculo.SetSelectedValue(EditObject.TipoCoche != null ? EditObject.TipoCoche.Id : cbTipoVehiculo.NoneValue);
            cbVehiculo.SetSelectedValue(EditObject.Vehiculo != null ? EditObject.Vehiculo.Id : cbVehiculo.AllValue);
            cbChofer.SetSelectedValue(EditObject.Empleado != null ? EditObject.Empleado.Id : cbChofer.AllValue);
            cbTipoCicloLogistico.SetSelectedValue(EditObject.TipoCicloLogistico != null ? EditObject.TipoCicloLogistico.Id : cbTipoCicloLogistico.AllValue);

            txtCodigo.Text = EditObject.Codigo.ToUpperInvariant();
            dtFecha.SelectedDate = EditObject.Inicio.ToDisplayDateTime();
            dtRegeneraDesde.SelectedDate = EditObject.InicioReal.HasValue
                                               ? EditObject.InicioReal.Value.ToDisplayDateTime()
                                               : EditObject.Inicio.ToDisplayDateTime();
            dtRegeneraHasta.SelectedDate = EditObject.Fin.ToDisplayDateTime();
            radTipo.SelectedValue = EditObject.Tipo.ToString(CultureInfo.InvariantCulture);
            txtDesvio.Text = EditObject.Desvio.ToString(CultureInfo.InvariantCulture);
            chkRegresaABase.Checked = EditObject.RegresoABase;
            chkProgramacionDinamica.Checked = EditObject.ProgramacionDinamica;
            txtComentario.Text = EditObject.Comentario;
            txtUmbral.Text = EditObject.Umbral.ToString("#0");
            lblFechaAlta.Text = EditObject.Alta.HasValue
                                     ? EditObject.Alta.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss")
                                     : string.Empty;
            lblInicioReal.Text = EditObject.InicioReal.HasValue
                                     ? EditObject.InicioReal.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss")
                                     : string.Empty;
            dtFechaFin.SelectedDate = EditObject.Fin.ToDisplayDateTime();
            
            var detalles = EditObject.Detalles.ToList();

            Horarios.Set(detalles.OrderBy(d=> d.Orden).Select(h => h.Programado).ToList());
            Hasta.Set(detalles.OrderBy(d => d.Orden).Select(h => h.ProgramadoHasta).ToList());
            var entregas = detalles.OrderBy(d => d.Orden).Select(e => new Entrega(e)).ToList();
            Entregas.Set(entregas);

            BindEntregas();
            BindEstadosCumplidos();

            var points = EditObject.Recorrido.Select(detalle => new PointF((float)detalle.Longitud, (float)detalle.Latitud)).ToList();
            EditLine1.SetLine(points);

            if (EditObject.EntregasTotalCountConBases > 0)
            {
                var inicio = EditObject.Detalles.First();
                EditLine1.Mapa.SetCenter(inicio.ReferenciaGeografica.Latitude, inicio.ReferenciaGeografica.Longitude, 10);
                EditLine1.Mapa.SetDefaultCenter(inicio.ReferenciaGeografica.Latitude, inicio.ReferenciaGeografica.Longitude);
            }
            else if (points.Count > 0) EditLine1.SetCenter(points[0].Y, points[0].X);
            else SetCenterLinea();

            OpcionesRecorridoVisible();

            SetEnableState();
        }

        private void SetCancelState()
        {
            if (EditObject.Estado == ViajeDistribucion.Estados.EnCurso)
            {
                btnAnular.Visible = lblMotivo.Visible = txtMotivo.Visible = true;
                txtMotivo.Enabled = true;
            }
            else if (EditObject.Estado == ViajeDistribucion.Estados.Anulado)
            {
                btnAnular.Visible = false;
                lblMotivo.Visible = txtMotivo.Visible = true;
                txtMotivo.Text = EditObject.Motivo;
                txtMotivo.Enabled = false;
            }
            else
            {
                btnAnular.Visible = lblMotivo.Visible = txtMotivo.Visible = false;
                lblFechaFin.Visible = dtFechaFin.Visible = EditMode && EditObject.Estado == ViajeDistribucion.Estados.Cerrado;
                dtFechaFin.Enabled = EditObject.InicioReal.HasValue && EditObject.InicioReal.Value > DateTime.Today;
            }
        }

        private void SetEnableState()
        {
            var pendiente = EditObject.Estado == ViajeDistribucion.Estados.Pendiente;
            var cerrado = EditObject.Estado == ViajeDistribucion.Estados.Cerrado;
            panelAbrirEntregaExistente.Visible = panelAbrirNuevaEntrega.Visible = pendiente;
            dtFecha.Enabled = pendiente; // Fecha
            //ReorderList1.Enabled = pendiente; // Entregas

            // Datos Generales
            radTipo.Enabled = pendiente;
            cbEmpresa.Enabled = pendiente;
            cbLinea.Enabled = pendiente;
            cbCentroDeCosto.Enabled = pendiente;
            cbSubCentroDeCosto.Enabled = pendiente;
            cbTransportista.Enabled = pendiente;
            cbVehiculo.Enabled = pendiente;
            cbChofer.Enabled = pendiente;
            cbTipoCicloLogistico.Enabled = pendiente;
           // txtCodigo.Enabled = pendiente;
            dtFecha.Enabled = pendiente;
            txtUmbral.Enabled = pendiente;
            chkRegresaABase.Enabled = pendiente;
            chkProgramacionDinamica.Enabled = pendiente;
            txtComentario.Enabled = pendiente;

            lblFechaFin.Visible = dtFechaFin.Visible = EditMode && cerrado;
            dtFechaFin.Enabled = EditObject.InicioReal.HasValue && EditObject.InicioReal.Value > DateTime.Today;
        }

        protected override void OnDelete()
        {
            if (EditObject.Estado == ViajeDistribucion.Estados.EnCurso)
            {
                var ciclo = new CicloLogisticoDistribucion(EditObject, DAOFactory, new MessageSaver(DAOFactory));
                IEvent evento = new CloseEvent(DateTime.UtcNow);
                ciclo.ProcessEvent(evento);
            }

            DAOFactory.ViajeDistribucionDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                    EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                    EditObject.Transportista = cbTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(cbTransportista.Selected) : null;
                    EditObject.CentroDeCostos = cbCentroDeCosto.Selected > 0 ? DAOFactory.CentroDeCostosDAO.FindById(cbCentroDeCosto.Selected) : null;
                    EditObject.SubCentroDeCostos = cbSubCentroDeCosto.Selected > 0
                        ? DAOFactory.SubCentroDeCostosDAO.FindById(cbSubCentroDeCosto.Selected)
                        : null;
                    EditObject.TipoCoche = cbTipoVehiculo.Selected > 0 ? DAOFactory.TipoCocheDAO.FindById(cbTipoVehiculo.Selected) : null;
                    EditObject.Vehiculo = cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected) : null;
                    EditObject.Empleado = cbChofer.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbChofer.Selected) : null;
                    EditObject.TipoCicloLogistico = cbTipoCicloLogistico.Selected > 0 ? DAOFactory.TipoCicloLogisticoDAO.FindById(cbTipoCicloLogistico.Selected) : null;

                    EditObject.Codigo = txtCodigo.Text.ToUpperInvariant();
                    EditObject.NumeroViaje = 0;
                    EditObject.Tipo = Convert.ToInt16(radTipo.SelectedValue);
                    EditObject.Desvio = Convert.ToInt32(txtDesvio.Text.Trim());
                    EditObject.RegresoABase = chkRegresaABase.Checked;
                    EditObject.ProgramacionDinamica = chkProgramacionDinamica.Checked;
                    EditObject.Comentario = txtComentario.Text;
                    EditObject.Umbral = !txtUmbral.Text.Trim().Equals(string.Empty) ? Convert.ToInt32(txtUmbral.Text.Trim()) : 0;
                    if (EditMode &&
                        EditObject.Estado == ViajeDistribucion.Estados.Cerrado &&
                        EditObject.InicioReal.HasValue &&
                        EditObject.InicioReal.Value > DateTime.Today)
                        EditObject.Fin = dtFechaFin.SelectedDate.Value.ToDataBaseDateTime();

                    var dicDetalles = EditObject.Detalles.ToDictionary(d => d.Id);
                    var entregas = Entregas.Get();
                    var horarios = Horarios.Get();
                    var hasta = Hasta.Get();
                    for (var i = 0; i < entregas.Count; i++)
                    {
                        entregas[i].Orden = i;
                        entregas[i].Programado = horarios[i];
                        entregas[i].ProgramadoHasta = hasta[i];
                    }

                    EditObject.Inicio = Inicio;
                    EditObject.Fin = entregas.Last().ProgramadoHasta;

                    EditObject.Detalles.Clear();
                    foreach (var entrega in entregas)
                    {
                        var tipoServicio = entrega.TipoServicio > 0 ? DAOFactory.TipoServicioCicloDAO.FindById(entrega.TipoServicio) : null;
                        var cliente = entrega.Cliente > 0 ? DAOFactory.ClienteDAO.FindById(entrega.Cliente) : null;
                        var puntoEntrega = entrega.PuntoEntrega > 0 ? DAOFactory.PuntoEntregaDAO.FindById(entrega.PuntoEntrega) : null;
                        var origen = entrega.Linea > 0 ? DAOFactory.LineaDAO.FindById(entrega.Linea) : null;
                        var det = new EntregaDistribucion
                                  {
                                      Cliente = cliente,
                                      Descripcion = entrega.Descripcion,
                                      Orden = entrega.Orden,
                                      Programado = entrega.Programado,
                                      ProgramadoHasta = entrega.ProgramadoHasta,
                                      PuntoEntrega = puntoEntrega,
                                      Linea = origen,
                                      Id = entrega.Id,
                                      TipoServicio = tipoServicio
                                  };
                        if (det.Id > 0)
                        {
                            if (dicDetalles.Count > 0 && dicDetalles.ContainsKey(entrega.Id))
                                det = dicDetalles[entrega.Id];
                            else
                            {
                                det.Id = 0;
                            }
                            det.Orden = entrega.Orden;
                            det.Programado = entrega.Programado;
                            det.ProgramadoHasta = entrega.ProgramadoHasta;
                            det.TipoServicio = tipoServicio;
                        }
                        det.Viaje = EditObject;
                        EditObject.Detalles.Add(det);
                    }

                    if (!EditMode)
                    {
                        EditObject.Estado = ViajeDistribucion.Estados.Pendiente;
                        EditObject.Alta = DateTime.UtcNow;
                    }

                    if (EditObject.Tipo == ViajeDistribucion.Tipos.RecorridoFijo)
                    {
                        var points = EditLine1.Points.Get();
                        EditObject.Recorrido.Clear();
                        RecorridoDistribucion last = null;
                        for (var i = 0; i < points.Count; i++)
                        {
                            var point = points[i];
                            var det = new RecorridoDistribucion {Latitud = point.Y, Longitud = point.X, Distribucion = EditObject, Orden = i};
                            det.Distancia = last == null ? 0 : Distancias.Loxodromica(last.Latitud, last.Longitud, det.Latitud, det.Longitud)/1000.0;
                            EditObject.Recorrido.Add(det);
                            last = det;
                        }
                    }

                    DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(EditObject);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        protected override void ValidateSave()
        {
            var codigo = ValidateEmpty(txtCodigo.Text, "CODE");
            var desvio = ValidateEmpty(txtDesvio.Text, "DESVIO");
            ValidateInt32(desvio, "DESVIO");

            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, codigo);
            ValidateDuplicated(byCode, "CODE");

            ValidateEmpty(dtFecha.SelectedDate, "FECHA");
            if (!txtUmbral.Text.Trim().Equals(string.Empty))
                ValidateInt32(txtUmbral.Text, "UMBRAL_SALIDA");

            var entregas = Entregas.Get();
            if (entregas.Count == 0) ThrowMustEnter("ENTREGA");

            var tipo = Convert.ToInt16(radTipo.SelectedValue);
            if (tipo == ViajeDistribucion.Tipos.RecorridoFijo)
            {
                var points = EditLine1.Points.Get();
                if (points == null || points.Count <= 1) ThrowMustEnter("RECORRIDO");
            }
        }

        protected override void ViewMap()
        {
            if (EditObject.EntregasTotalCountConBases == 0)
            {
                ShowInfo("No hay datos para mostrar");
                return;
            }

            OpenWin(ResolveUrl(UrlMaker.MonitorLogistico.GetUrlDistribucion(EditObject.Id)), "_blank");
        }

        protected override void ViewEvent()
        {
            if (EditObject.Vehiculo == null)
            {
                ShowInfo("La ruta no tiene un vehículo asignado");
                return;
            }

            var inicio = EditObject.InicioReal.Value;
            var fin = EditObject.Estado == ViajeDistribucion.Estados.Cerrado ? EditObject.Fin : DateTime.UtcNow;
            var mobile = EditObject.Vehiculo;

            Session.Add("EventsLocation", mobile.Empresa != null ? mobile.Empresa.Id : mobile.Linea != null ? mobile.Linea.Empresa.Id : -1);
            Session.Add("EventsCompany", mobile.Linea != null ? mobile.Linea.Id : -1);
            Session.Add("EventsMobileType", mobile.TipoCoche.Id);
            Session.Add("EventsMobile", mobile.Id);
            Session.Add("EventsFrom", inicio.AddMinutes(-1).ToDisplayDateTime());
            Session.Add("EventsTo", fin.AddMinutes(1).ToDisplayDateTime());

            OpenWin(ResolveUrl("~/Reportes/DatosOperativos/eventos.aspx"), "Reporte de Eventos");
        }

        protected override void Regenerate()
        {
            panelReprocesarTicket.Visible = true;
            mpePanel.CancelControlID = btRegeneraCancelar.ID;

            lblRuta.Text = EditObject.Codigo;
            lblVehiculoReprocesar.Text = EditObject.Vehiculo.Interno + " - " + EditObject.Vehiculo.Patente;
            dtRegeneraDesde.SelectedDate = EditObject.InicioReal.HasValue
                                               ? EditObject.InicioReal.Value.ToDisplayDateTime()
                                               : EditObject.Inicio.ToDisplayDateTime();
            dtRegeneraHasta.SelectedDate = EditObject.Fin.ToDisplayDateTime();

            mpePanel.Show();
        }

        protected void BtRegeneraAceptarClick(object sender, EventArgs e)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var desde = dtRegeneraDesde.SelectedDate.Value.ToDataBaseDateTime();
                    var hasta = dtRegeneraHasta.SelectedDate.Value.ToDataBaseDateTime();
                    var cerrar = !chkNoCerrar.Checked;

                    var ruta = DAOFactory.ViajeDistribucionDAO.FindById(EditObject.Id);

                    ruta.InicioReal = desde;
                    ruta.Estado = ViajeDistribucion.Estados.EnCurso;

                    foreach (var detalle in ruta.Detalles)
                    {
                        if (detalle.Estado != EntregaDistribucion.Estados.Completado && detalle.Estado != EntregaDistribucion.Estados.Cancelado)
                        {
                            detalle.Estado = EntregaDistribucion.Estados.Pendiente;
                            detalle.Entrada = null;
                            detalle.Manual = null;
                            detalle.Salida = null;

                            DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
                        }
                    }

                    DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(ruta);

                    var ciclo = new CicloLogisticoDistribucion(ruta, DAOFactory, null);
                    ciclo.Regenerar(desde, hasta);

                    if (cerrar)
                    {
                        ruta.Fin = hasta;
                        ruta.Estado = ViajeDistribucion.Estados.Cerrado;
                        DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(ruta);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            Response.Redirect(RedirectUrl);
        }

        #endregion

        private void BindEstadosCumplidos()
        {
            gridEstadosCumplidos.Columns[0].HeaderText = string.Empty;
            gridEstadosCumplidos.Columns[1].HeaderText = CultureManager.GetEntity("PARTICK08");
            gridEstadosCumplidos.Columns[2].HeaderText = CultureManager.GetLabel("INICIO");
            gridEstadosCumplidos.Columns[3].HeaderText = CultureManager.GetLabel("FIN");
            gridEstadosCumplidos.Columns[4].HeaderText = CultureManager.GetLabel("TOTAL");
            gridEstadosCumplidos.Columns[5].HeaderText = CultureManager.GetLabel("DEMORA");
            gridEstadosCumplidos.Columns[6].HeaderText = CultureManager.GetLabel("DESVIO");

            var dt = EditObject.EstadosCumplidos;
            gridEstadosCumplidos.DataSource = dt;
            gridEstadosCumplidos.DataBind();
        }
        private void BindEntregas()
        {
            CreateOrigen();
            var horarios = Horarios.Get();
            var hasta = Hasta.Get();
            var entregas = Entregas.Get();

            for (var i = 0; i < entregas.Count; i++)
            {
                entregas[i].Orden = i;
                entregas[i].Programado = horarios[i];
                entregas[i].ProgramadoHasta = hasta[i];
            }
            Entregas.Set(entregas);

            ReorderList1.DataSource = entregas;
            ReorderList1.DataBind();
            updEntregas.Update();

            cbFiltroEntregas.Items.Clear();
            foreach(var entrega in entregas)
            {
                cbFiltroEntregas.Items.Add(new ListItem(string.Format("{0}. {1}", entrega.Orden, entrega.Descripcion), entrega.Orden.ToString("#0"), true));    
            }
            updFiltroEntregas.Update();

            AddMarkers();
        }
        private void BindHorarios()
        {
            var horarios = Horarios.Get();

            for (int i = 0; i < ReorderList1.Items.Count; i++)
            {
                var item = ReorderList1.Items[i];
                var updDt = item.FindControl("updDt") as UpdatePanel;
                var dt = item.FindControl("dtHoraEstado") as DateTimePicker;
                if (dt != null) dt.SelectedDate = horarios[i].ToDisplayDateTime();
                if (updDt != null) updDt.Update();
            }
        }
        private void BindHasta()
        {
            var hasta = Hasta.Get();

            for (var i = 0; i < ReorderList1.Items.Count; i++)
            {
                var item = ReorderList1.Items[i];
                var updHasta = item.FindControl("updHasta") as UpdatePanel;
                var dt = item.FindControl("dtHasta") as DateTimePicker;
                if (dt != null) dt.SelectedDate = hasta[i].ToDisplayDateTime();
                if (updHasta != null) updHasta.Update();
            }
        }

        protected void GridEstadosCumplidosOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var result = e.Row.DataItem as EstadoDistribucion;
                if (result != null)
                {
                    var img = e.Row.FindControl("imgIcono") as System.Web.UI.WebControls.Image;
                    if (img != null) img.ImageUrl = "../../iconos/" + result.EstadoLogistico.Icono.PathIcono;

                    var lbl = e.Row.FindControl("lblEstadoLogistico") as Label;
                    if (lbl != null) lbl.Text = result.EstadoLogistico.Descripcion;

                    lbl = e.Row.FindControl("lblDesde") as Label;
                    if (lbl != null) lbl.Text = result.Inicio.HasValue ? result.Inicio.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;

                    lbl = e.Row.FindControl("lblHasta") as Label;
                    if (lbl != null) lbl.Text = result.Fin.HasValue ? result.Fin.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;

                    var total = result.Inicio.HasValue && result.Fin.HasValue ? result.Fin.Value.Subtract(result.Inicio.Value) : new TimeSpan();
                    lbl = e.Row.FindControl("lblTotal") as Label;
                    if (lbl != null) lbl.Text = total.ToString();

                    var demora = new TimeSpan(0, result.EstadoLogistico.Demora, 0);
                    lbl = e.Row.FindControl("lblDemora") as Label;
                    if (lbl != null) lbl.Text = demora.ToString();

                    var desvio = total.TotalSeconds > 0 ? total.Subtract(demora) : new TimeSpan();
                    lbl = e.Row.FindControl("lblDesvio") as Label;
                    if (lbl != null) lbl.Text = desvio.ToString();
                }
            }
        }

        protected void BtnAnularClick(object sender, EventArgs e)
        {
            if (txtMotivo.Text.Trim() == string.Empty)
            {
                LblInfo.Text = string.Format(CultureManager.GetError("MUST_ENTER_VALUE"), CultureManager.GetLabel("MOTIVO_ANULACION"));
            }
            else
            {
                if (EditObject.Estado == ViajeDistribucion.Estados.EnCurso)
                {
                    EditObject.Motivo = txtMotivo.Text;
                    EditObject.Estado = ViajeDistribucion.Estados.Anulado;
                    
                    var pendientes = EditObject.Detalles.Where(d => d.Estado == EntregaDistribucion.Estados.Pendiente);
                    foreach (var detalle in pendientes)
                    {
                        detalle.Estado = EntregaDistribucion.Estados.Cancelado;
                        DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
                    }
                    DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(EditObject);
                }

                AfterSave();
            }
        }

        protected void CbLineaSelectedIndexChanged(object sender, EventArgs e)
        {
            var points = EditLine1.Points.Get();
            var entregas = Entregas.Get();
            if ((points == null || points.Count == 0) && entregas.Count == 0) SetCenterLinea();
            if (chkRegresaABase.Checked)
            {
                DeleteDestino();
                CreateDestino();
            }
            BindEntregas();
        }

        protected void CbVehiculoSelectedIndexChanged(object sender, EventArgs e)
        {
            SelectChoferFromMovil();
        }

        protected void RadTipoElectedIndexChanged(object sender, EventArgs e)
        {
            OpcionesRecorridoVisible();
        }

        protected void CbFiltroEntregasSelectedIndexChanged(object sender, EventArgs e)
        {
            AddMarkers();
        }
        
        protected void ChkRegresaABaseCheckedChanged(object sender, EventArgs e)
        {
            if (chkRegresaABase.Checked) 
                CreateDestino();
            else 
                DeleteDestino();
        }

        protected void CbCentroDeCostoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCentroDeCosto.Selected > 0)
            {
                var cc = DAOFactory.CentroDeCostosDAO.FindById(cbCentroDeCosto.Selected);
                SetFechaInicio(cc);
            }
        }

        protected void CbSubCentroDeCostoSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSubCentroDeCosto.Selected > 0 && cbCentroDeCosto.Selected <= 0)
            {
                var scc = DAOFactory.SubCentroDeCostosDAO.FindById(cbSubCentroDeCosto.Selected);
                SetFechaInicio(scc.CentroDeCostos);
            }
        }

        protected void CbViajeProgramadoSelectedIndexChanged(object sender, EventArgs e)
        {
            var detalles = new List<EntregaProgramada>();
            if (cbViajeProgramado.Selected > 0)
            {
                var viajeProgramado = DAOFactory.ViajeProgramadoDAO.FindById(cbViajeProgramado.Selected);
                if (viajeProgramado != null)
                {
                    detalles = viajeProgramado.Detalles.ToList();
                    btnGuardar.Visible = true;
                }
                else
                {
                    btnGuardar.Visible = false;
                }
            }

            gridEntregas.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI44");
            gridEntregas.Columns[1].HeaderText = CultureManager.GetLabel("BULTOS");
            gridEntregas.Columns[2].HeaderText = CultureManager.GetLabel("PESO");
            gridEntregas.Columns[3].HeaderText = CultureManager.GetLabel("VOLUMEN");
            gridEntregas.Columns[4].HeaderText = CultureManager.GetLabel("VALOR");

            gridEntregas.DataSource = detalles;
            gridEntregas.DataBind();
        }

        protected void GridEntregasOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var entrega = e.Row.DataItem as EntregaProgramada;
                if (entrega != null)
                {
                    var lbl = e.Row.FindControl("lblPuntoEntrega") as Label;
                    if (lbl != null) lbl.Text = entrega.PuntoEntrega.Descripcion;

                    var txt = e.Row.FindControl("txtBultos") as TextBox;
                    if (txt != null) txt.Text = "0";

                    txt = e.Row.FindControl("txtPeso") as TextBox;
                    if (txt != null) txt.Text = "0.0";

                    txt = e.Row.FindControl("txtVolumen") as TextBox;
                    if (txt != null) txt.Text = "0.0";

                    txt = e.Row.FindControl("txtValor") as TextBox;
                    if (txt != null) txt.Text = "0.0";

                    var dt = e.Row.FindControl("dtDateProg") as DateTimePicker;
                    if (dt != null) dt.SelectedDate = dtFechaProg.SelectedDate;
                }
            }
        }

        protected void BtnGuardarOnClick(object sender, EventArgs e)
        {
            ValidateSaveProgramado();

            if (cbViajeProgramado.Selected > 0)
            {
                var viajeProg = DAOFactory.ViajeProgramadoDAO.FindById(cbViajeProgramado.Selected);
                if (viajeProg != null && viajeProg.Detalles.Any())
                {
                    var empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresaProg.Selected);
                    var vehiculo = cbVehiculoProg.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculoProg.Selected) : null;
                    var tipoVehiculo = cbTipoVehiculoProg.Selected > 0 ? DAOFactory.TipoCocheDAO.FindById(cbTipoVehiculoProg.Selected) : null;
                    var transportista = cbTransportistaProg.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(cbTransportistaProg.Selected) : null;
                    var tipoCiclo = cbTipoCicloProg.Selected > 0 ? DAOFactory.TipoCicloLogisticoDAO.FindById(cbTipoCicloProg.Selected) : null;
                    var codigo = txtCodigoProg.Text;
                    var fecha = dtFechaProg.SelectedDate.Value.ToDataBaseDateTime();

                    var viaje = new ViajeDistribucion
                    {
                        Alta = DateTime.UtcNow,
                        Codigo = codigo,
                        Empresa = empresa,
                        Estado = ViajeDistribucion.Estados.Pendiente,
                        Inicio = fecha,
                        Fin = fecha,
                        NumeroViaje = 1,
                        RegresoABase = false,
                        Tipo = ViajeDistribucion.Tipos.Desordenado,
                        Vehiculo = vehiculo,
                        TipoCoche = tipoVehiculo,
                        Transportista = transportista,
                        TipoCicloLogistico = tipoCiclo
                    };                    

                    for (var i = 0; i < viajeProg.Detalles.Count; i++)
                    {
                        var entregaProg = viajeProg.Detalles[i];

                        if (i > 0)
                        {
                            var origen = new LatLon(viajeProg.Detalles[i - 1].PuntoEntrega.ReferenciaGeografica.Latitude, viajeProg.Detalles[i - 1].PuntoEntrega.ReferenciaGeografica.Longitude);
                            var destino = new LatLon(viajeProg.Detalles[i].PuntoEntrega.ReferenciaGeografica.Latitude, viajeProg.Detalles[i].PuntoEntrega.ReferenciaGeografica.Longitude);
                            var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, null);

                            if (directions != null)
                            {
                                var duracion = directions.Duration;
                                fecha = fecha.Add(duracion);
                            }
                        }

                        var fila = gridEntregas.Rows[i];
                        var txtBultos = fila.FindControl("txtBultos") as TextBox;
                        var txtPeso = fila.FindControl("txtPeso") as TextBox;
                        var txtVolumen = fila.FindControl("txtVolumen") as TextBox;
                        var txtValor = fila.FindControl("txtValor") as TextBox;
                        
                        var bultos = 0;
                        var peso = 0.0;
                        var volumen = 0.0;
                        var valor = 0.0;

                        int.TryParse(txtBultos.Text.Replace(".",","), out bultos);
                        double.TryParse(txtPeso.Text.Replace(".", ","), out peso);
                        double.TryParse(txtVolumen.Text.Replace(".", ","), out volumen);
                        double.TryParse(txtValor.Text.Replace(".", ","), out valor);

                        var entrega = new EntregaDistribucion
                        {
                            Cliente = entregaProg.PuntoEntrega.Cliente,
                            PuntoEntrega = entregaProg.PuntoEntrega,
                            Descripcion = entregaProg.PuntoEntrega.Descripcion,
                            Estado = EntregaDistribucion.Estados.Pendiente,
                            Orden = viaje.Detalles.Count,
                            Programado = fecha,
                            ProgramadoHasta = fecha.AddHours(1),
                            Viaje = viaje,
                            Bultos = bultos,
                            Peso = peso,
                            Volumen = volumen,
                            Valor = valor                            
                        };

                        viaje.Detalles.Add(entrega);

                        viaje.Fin = fecha.AddHours(1);
                    }

                    DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
                }
            }

            Response.Redirect(RedirectUrl);
        }

        private void ValidateSaveProgramado()
        {
            var codigo = ValidateEmpty(txtCodigoProg.Text, "CODE");
            var byCode = DAOFactory.ViajeDistribucionDAO.FindByCodigo(cbEmpresaProg.Selected, -1, codigo);
            ValidateDuplicated(byCode, "CODE");
            ValidateEmpty(dtFechaProg.SelectedDate, "FECHA");
            ValidateEntity(cbViajeProgramado.Selected, "OPETICK13");
        }

        private void SetFechaInicio(CentroDeCostos cc)
        {
            if (cc.HorarioInicio.HasValue)
                dtFecha.SelectedDate = DateTime.Today.AddHours(cc.HorarioInicio.Value.Hour).AddMinutes(cc.HorarioInicio.Value.Minute);
        }

        private void SelectChoferFromMovil()
        {
            if (cbVehiculo.Selected <= 0) return;
            var movil = DAOFactory.CocheDAO.FindById(cbVehiculo.Selected);
            if (movil.Chofer == null) return;
            cbChofer.SetSelectedValue(movil.Chofer.Id);
        }

        private void OpcionesRecorridoVisible()
        {
            panelOpcionesRecorrido.Visible = radTipo.SelectedValue == ViajeDistribucion.Tipos.RecorridoFijo.ToString("#0");
        }

        #region Entregas
        
        protected void BtEntregaExistenteClick(object sender, EventArgs e)
        {
            panelEntregaExistente.Visible = !panelEntregaExistente.Visible;
            panelNuevaEntrega.Visible = false;
        }

        protected void BtNuevaEntregaClick(object sender, EventArgs e)
        {
            panelNuevaEntrega.Visible = !panelNuevaEntrega.Visible;
            panelEntregaExistente.Visible = false;
        }

        protected void CbPuntoEntregaSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPuntoEntrega.Selected <= 0) return;
            txtDescripcion.Text = cbPuntoEntrega.Text;
        }

        protected void BtDeleteCommand(object sender, CommandEventArgs e)
        {
            var deletedIndex = Convert.ToInt32(e.CommandArgument);

            var entregas = Entregas.Get();
            entregas.RemoveAt(deletedIndex);
            Entregas.Set(entregas);

            var horarios = Horarios.Get();
            horarios.RemoveAt(deletedIndex);
            Horarios.Set(horarios);

            var hasta = Hasta.Get();
            hasta.RemoveAt(deletedIndex);
            Hasta.Set(hasta);

            RecalcularHorariosPorDistancia(deletedIndex, deletedIndex);
            BindEntregas();
        }

        protected void BtAcceptEntregaExistenteClick(object sender, EventArgs e)
        {
            if (cbPuntoEntrega.Selected <= 0) return;
            var entregas = Entregas.Get();
            var punto = DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected);
            var end = DateTime.UtcNow.AddHours(12).ToDataBaseDateTime();
            if (punto.ReferenciaGeografica.Vigencia.Fin < end)
            {
                punto.ReferenciaGeografica.Vigencia.Fin = end;
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(punto.ReferenciaGeografica);
                STrace.Trace("QtreeReset", "DistribucionAlta 1");
                DAOFactory.PuntoEntregaDAO.SaveOrUpdate(punto);
            }
            var entrega = new Entrega
                              {
                                  Cliente = punto.Cliente.Id,
                                  Descripcion = txtDescripcion.Text,
                                  Programado = Inicio,
                                  ProgramadoHasta = Inicio,
                                  PuntoEntrega = punto.Id,
                                  ReferenciaGeografica = punto.ReferenciaGeografica.Id,
                                  PuntoEntregaDescripcion = punto.Descripcion,
                                  ClienteDescripcion = punto.Cliente.Descripcion,
                                  Icono = punto.ReferenciaGeografica.Icono != null ? punto.ReferenciaGeografica.Icono.PathIcono : string.Empty,
                                  TipoServicio = TipoServicioDefault != null ? TipoServicioDefault.Id : 0,
                                  Nomenclado = punto.Nomenclado
                              };

            var horarios = Horarios.Get();
            var hasta = Hasta.Get();
            var velocidadPromedio = GetVelocidad();
            DateTime hora;
            if (entregas.Count > 0)
            {
                var regresaABase = chkRegresaABase.Checked && entregas.Last().Linea > 0;
                var lastIndex = regresaABase ? entregas.Count-2 : entregas.Count - 1;
                var ultima = entregas[lastIndex];
                var lastPunto = ultima.PuntoEntrega > 0 
                    ? DAOFactory.PuntoEntregaDAO.FindById(ultima.PuntoEntrega).ReferenciaGeografica
                    : DAOFactory.LineaDAO.FindById(ultima.Linea).ReferenciaGeografica;
                var lastTipoServicio = ultima.TipoServicio > 0 ? DAOFactory.TipoServicioCicloDAO.FindById(ultima.TipoServicio) : null;
                var viaje = GetTiempoViaje(lastPunto, punto.ReferenciaGeografica, velocidadPromedio);
                var demora = lastTipoServicio != null ? lastTipoServicio.Demora : 0;

                hora = horarios[lastIndex].Add(viaje).AddMinutes(demora);
                if (regresaABase)
                {
                    horarios[lastIndex + 1] = horarios[lastIndex + 1].Add(viaje).AddMinutes(demora);
                    hasta[lastIndex + 1] = hasta[lastIndex + 1].Add(viaje).AddMinutes(demora);
                }
            }
            else hora = Inicio;

            if (chkRegresaABase.Checked && entregas.Count > 1 && entregas.Last().Linea > 0)
            {
                entregas.Insert(entregas.Count - 1, entrega);
                horarios.Insert(horarios.Count - 1, hora);
                hasta.Insert(hasta.Count - 1, hora);
            }
            else
            {
                entregas.Add(entrega);
                horarios.Add(hora);
                hasta.Add(hora);  
            }
            
            Entregas.Set(entregas);
            Horarios.Set(horarios);
            Hasta.Set(hasta);
            BindEntregas();

            var item = ReorderList1.Items.Last();
            var combo = item.FindControl("cbTipoServicio") as TipoServicioCicloDropDownList;
            if (TipoServicioDefault != null && combo != null) combo.SetSelectedValue(TipoServicioDefault.Id);

            panelEntregaExistente.Visible = true;
        }
        protected void BtCancelEntregaExistenteClick(object sender, EventArgs e)
        {
            panelEntregaExistente.Visible = false;
        }

        protected void BtnBuscarDireccionClick(object sender, EventArgs e)
        {
            var direccion = txtDireccion.Text.Trim();   
            if (!string.IsNullOrEmpty(direccion))
            {
                var results = GeocoderHelper.GetDireccionSmartSearch(direccion);
                SetResults(results);
            }
            else
            {
                Double lat;
                Double lon;

                var latitud = txtLatitud.Text.Trim().Replace('.',',');
                var longitud = txtLongitud.Text.Trim().Replace('.', ',');

                if (double.TryParse(latitud, out lat) && double.TryParse(longitud, out lon))
                {
                    var resultado = GeocoderHelper.GetEsquinaMasCercana(lat, lon);
                    SetResults(new List<DireccionVO>{resultado});
                }   
            }

        }
        private void SetResults(IList<DireccionVO> result)
        {
            lblDireccion.Visible = result.Count == 0;
            cbResults.Visible = result.Count != 0;
            btAceptar.Visible = result.Count != 0;

            if (result.Count == 0) lblDireccion.Text = string.Format("No se ha encontrado ninguna Dirección");
            else
            {
                Direcciones = result;
                cbResults.Items.Clear();

                foreach (var vo in result) cbResults.Items.Add(vo.Direccion);
            }
        }
        
        protected void BtAceptarClick(object sender, EventArgs e)
        {
            if (cbResults.SelectedIndex < 0) return;

            SetDireccion(DireccionFromVo(Direcciones[cbResults.SelectedIndex]));
            Direcciones = null;
        }
        protected void BtCancelarClick(object sender, EventArgs e)
        {
            Direcciones = null;
            panelNuevaEntrega.Visible = false;
        }

        protected void DtFechaDateChanged(object sender, EventArgs e)
        {
            var horarios = Horarios.Get();
            var hasta = Hasta.Get();
            var horarioInicial = horarios[0];
            var diferencia = dtFecha.SelectedDate.HasValue ? dtFecha.SelectedDate.Value.ToDataBaseDateTime().Subtract(horarioInicial) : new TimeSpan();

            for (var i = 0; i < horarios.Count; i++)
            {
                horarios[i] = horarios[i].Add(diferencia);
                hasta[i] = hasta[i].Add(diferencia);
            }
            Horarios.Set(horarios);
            Hasta.Set(hasta);

            BindEntregas();
        }

        private static Direccion DireccionFromVo(DireccionVO direccion)
        {
            var dir = new Direccion
            {
                Altura = direccion.Altura,
                Calle = direccion.Calle,
                Descripcion = direccion.Direccion,
                IdCalle = direccion.IdPoligonal,
                IdEntrecalle = (-1),
                IdEsquina = direccion.IdEsquina,
                IdMapa = ((short)direccion.IdMapaUrbano),
                Latitud = direccion.Latitud,
                Longitud = direccion.Longitud,
                Pais = "Argentina",
                Partido = direccion.Partido,
                Provincia = direccion.Provincia,
                Vigencia = new Vigencia()
            };
            return dir;
        }
        private void SetDireccion(Direccion direccion)
        {
            Selected = direccion;

            if (direccion != null)
            {
                AgregarNuevaEntrega();
                panelNuevaEntrega.Visible = false;
                cbResults.Items.Clear();
                Selected = null;
                //txtPuntoEntrega.Text = string.Empty;
                txtDescripcionNuevaEntrega.Text = string.Empty;
                txtDireccion.Text = string.Empty;
            }
            else
            {
                lblDireccion.Text = string.Format("No se ha seleccionado Dirección");
                lblDireccion.Visible = true;
                cbResults.Visible = false;
            }
        }
        
        private void AgregarNuevaEntrega()
        {
            var entregas = Entregas.Get();

            var cliente = DAOFactory.ClienteDAO.FindById(cbClienteNuevo.Selected);
            var descripcion = txtDescripcionNuevaEntrega.Text.Trim();
            var codigo = descripcion.Truncate(32);
            var direccion = Selected;

            var puntoEntrega = DAOFactory.PuntoEntregaDAO.FindByCode(cbEmpresa.SelectedValues,
                                                                     cbLinea.SelectedValues, 
                                                                     cbCliente.SelectedValues,
                                                                     codigo);

            if (puntoEntrega == null)
            {
                var georef = new ReferenciaGeografica
                                 {
                                     Codigo = codigo,
                                     Descripcion = descripcion,
                                     Empresa = cliente.Empresa,
                                     Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono,
                                     Linea = cliente.Linea,
                                     TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                                     Vigencia = new Vigencia {Inicio = DateTime.UtcNow, Fin = DateTime.UtcNow.AddHours(24)}
                                 };

                var poligono = new Poligono {Radio = 100, Vigencia = new Vigencia {Inicio = DateTime.UtcNow}};
                poligono.AddPoints(new[] {new PointF((float) direccion.Longitud, (float) direccion.Latitud)});

                georef.Historia.Add(new HistoriaGeoRef
                                        {
                                            ReferenciaGeografica = georef,
                                            Direccion = direccion,
                                            Poligono = poligono,
                                            Vigencia = new Vigencia {Inicio = DateTime.UtcNow}
                                        });
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(georef);
                STrace.Trace("QtreeReset", "DistribucionAlta 2");

                puntoEntrega = new PuntoEntrega
                                   {
                                       Cliente = cliente,
                                       Codigo = codigo,
                                       Descripcion = descripcion,
                                       Telefono = string.Empty,
                                       Baja = false,
                                       ReferenciaGeografica = georef,
                                       Nomenclado = true,
                                       DireccionNomenclada = string.Empty,
                                       Nombre = txtDescripcionNuevaEntrega.Text.Trim()
                                   };
            }
            else
            {
                if (puntoEntrega.ReferenciaGeografica.Latitude == direccion.Latitud
                 && puntoEntrega.ReferenciaGeografica.Longitude == direccion.Longitud)
                {
                    puntoEntrega.Descripcion = descripcion;
                }
                else
                {
                    var poligono = new Poligono { Radio = 100, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    poligono.AddPoints(new[] { new PointF((float)direccion.Longitud, (float)direccion.Latitud) });
                    puntoEntrega.ReferenciaGeografica.AddHistoria(direccion, poligono, DateTime.UtcNow);
                }
                
                puntoEntrega.ReferenciaGeografica.Vigencia.Fin = DateTime.UtcNow.AddHours(24);
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoEntrega.ReferenciaGeografica);
                STrace.Trace("QtreeReset", "DistribucionAlta 3");
            }

            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);

            var tipoServicio = DAOFactory.TipoServicioCicloDAO.FindDefault(cbEmpresa.SelectedValues, cbLinea.SelectedValues);

            var entrega = new Entrega
            {
                Cliente = puntoEntrega.Cliente.Id,
                Descripcion = txtDescripcionNuevaEntrega.Text,
                Programado = Inicio,
                ProgramadoHasta = Inicio,
                PuntoEntrega = puntoEntrega.Id,
                ReferenciaGeografica = puntoEntrega.ReferenciaGeografica.Id,
                PuntoEntregaDescripcion = puntoEntrega.Descripcion,
                ClienteDescripcion = puntoEntrega.Cliente.Descripcion,
                Icono = puntoEntrega.ReferenciaGeografica.Icono != null ? puntoEntrega.ReferenciaGeografica.Icono.PathIcono : string.Empty,
                TipoServicio = tipoServicio != null ? tipoServicio.Id : 0,
                Nomenclado = puntoEntrega.Nomenclado
            };

            var horarios = Horarios.Get();
            var hasta = Hasta.Get();
            var velocidadPromedio = GetVelocidad();
            DateTime hora;
            if (entregas.Count > 0)
            {
                var regresaABase = chkRegresaABase.Checked && entregas.Last().Linea > 0;
                var lastIndex = regresaABase ? entregas.Count - 2 : entregas.Count - 1;
                var ultima = entregas[lastIndex];
                var lastPunto = ultima.PuntoEntrega > 0
                    ? DAOFactory.PuntoEntregaDAO.FindById(ultima.PuntoEntrega).ReferenciaGeografica
                    : DAOFactory.LineaDAO.FindById(ultima.Linea).ReferenciaGeografica;
                var lastTipoServicio = ultima.TipoServicio > 0 ? DAOFactory.TipoServicioCicloDAO.FindById(ultima.TipoServicio) : null;
                var viaje = GetTiempoViaje(lastPunto, puntoEntrega.ReferenciaGeografica, velocidadPromedio);
                var demora = lastTipoServicio != null ? lastTipoServicio.Demora : 0;

                hora = horarios[lastIndex].Add(viaje).AddMinutes(demora);
                if (regresaABase)
                {
                    horarios[lastIndex + 1] = horarios[lastIndex + 1].Add(viaje).AddMinutes(demora);
                    hasta[lastIndex + 1] = hasta[lastIndex + 1].Add(viaje).AddMinutes(demora);
                }
            }
            else hora = Inicio;

            if (chkRegresaABase.Checked && entregas.Count > 1 && entregas.Last().Linea > 0)
            {
                entregas.Insert(entregas.Count - 1, entrega);
                horarios.Insert(horarios.Count - 1, hora);
                hasta.Insert(hasta.Count - 1, hora);
            }
            else
            {
                entregas.Add(entrega);
                horarios.Add(hora);
                hasta.Add(hora);
            }

            Entregas.Set(entregas);
            Horarios.Set(horarios);
            Hasta.Set(horarios);
            BindEntregas();

            var item = ReorderList1.Items.Last();
            var combo = item.FindControl("cbTipoServicio") as TipoServicioCicloDropDownList;
            if (TipoServicioDefault != null && combo != null) combo.SetSelectedValue(TipoServicioDefault.Id);

            panelEntregaExistente.Visible = true;
        }

        private bool _binding;
        protected void ReorderList1ItemDataBound(object sender, ReorderListItemEventArgs e)
        {
            var item = e.Item.DataItem as Entrega;

            var block = (item != null && item.Linea > 0) || (EditMode && EditObject.Estado != ViajeDistribucion.Estados.Pendiente);
            if (block)
            {
                var panelHandle = e.Item.GetControl("drag_handle") as HtmlGenericControl;
                if (panelHandle != null) panelHandle.Attributes.Add("onmousedown", "event.stopPropagation();");
                e.Item.GetControl("cbTipoServicio").Visible = false;
                e.Item.GetControl("btDelete").Visible = false;
            }

            var dt = e.Item.GetControl("dtHoraEstado") as DateTimePicker;
            var dtHasta = e.Item.GetControl("dtHasta") as DateTimePicker;
            if (dt == null || dtHasta == null) return;
            if (item != null)
            {
                dt.SelectedDate = item.Programado.ToDisplayDateTime();
                dtHasta.SelectedDate = item.ProgramadoHasta.ToDisplayDateTime();
                dt.Enabled = dtHasta.Enabled = EditObject.Estado == EntregaDistribucion.Estados.Pendiente;
            }

            var combo = e.Item.GetControl("cbTipoServicio") as TipoServicioCicloDropDownList;
            if (combo == null) return;
            var tipoServicio = item != null && item.TipoServicio > 0 ? item.TipoServicio : TipoServicioDefault != null ? TipoServicioDefault.Id : 0;
            _binding = true;
            combo.SetSelectedValue(tipoServicio);
            combo.Enabled = EditObject.Estado == EntregaDistribucion.Estados.Pendiente;
            _binding = false;

            var cbEstadoEntrega = e.Item.GetControl("cbEstadoEntrega") as EstadoEntregaDistribucionDropDownList;
            if (cbEstadoEntrega == null) return;
            cbEstadoEntrega.SetSelectedValue(item != null ? item.Estado : EntregaDistribucion.Estados.Pendiente);
            cbEstadoEntrega.Enabled = EditMode &&
                                      EditObject.Estado == ViajeDistribucion.Estados.Cerrado &&
                                      EditObject.InicioReal.HasValue &&
                                      EditObject.InicioReal.Value > DateTime.Today;
        }
        protected void ReorderList1ItemReorder(object sender, ReorderListItemReorderEventArgs e)
        {
            if (e.NewIndex == 0) return;
            var entregas = Entregas.Get();

            if (entregas[e.NewIndex].Linea > 0) return;

            var it = entregas[e.OldIndex];
            entregas.RemoveAt(e.OldIndex);
            entregas.Insert(e.NewIndex, it);
            Entregas.Set(entregas);

            var horarios = Horarios.Get();
            var ith = horarios[e.OldIndex];
            horarios.RemoveAt(e.OldIndex);
            horarios.Insert(e.NewIndex, ith);
            Horarios.Set(horarios);

            var hasta = Hasta.Get();
            var itHasta = hasta[e.OldIndex];
            hasta.RemoveAt(e.OldIndex);
            hasta.Insert(e.NewIndex, itHasta);
            Hasta.Set(hasta);

            RecalcularHorariosPorDistancia(e.OldIndex, e.NewIndex);
            BindEntregas();
        }
        protected void CbTipoServicioSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_binding) return;
            var entregas = Entregas.Get();
            var horarios = Horarios.Get();
            var index = ReorderList1.Items.Select((t, i) => new { index = i, combo = t.FindControl("cbTipoServicio") }).Where(t => t.combo == sender).Select(t => t.index).FirstOrDefault();
            var combo = (sender as TipoServicioCicloDropDownList);

            var newValue = combo != null && combo.SelectedIndex >= 0 ? combo.Selected : 0;
            if (index >= entregas.Count) return;
            var oldValue = entregas[index].TipoServicio;

            var oldTipo = oldValue > 0 ? DAOFactory.TipoServicioCicloDAO.FindById(oldValue) : null;
            var newTipo = newValue > 0 ? DAOFactory.TipoServicioCicloDAO.FindById(newValue) : null;

            var oldDemora = (oldTipo != null ? oldTipo.Demora : 0);
            var newDemora = (newTipo != null ? newTipo.Demora : 0);
            var demora = -oldDemora + newDemora;
            var anterior = index > 0 ? horarios[index - 1] : horarios[index];
            for (var i = index + 1; i < horarios.Count; i++)
            {
                var newHorario = horarios[i].AddMinutes(demora);
                if (newHorario < anterior)
                {
                    var diff = Convert.ToInt32(horarios[i].Subtract(newHorario).TotalMinutes);
                    demora += diff;
                    newHorario = horarios[i].AddMinutes(demora);
                }
                horarios[i] = newHorario;
            }
            entregas[index].TipoServicio = newValue;
            Entregas.Set(entregas);
            Horarios.Set(horarios);

            BindEntregas();
        }
        protected void DtHoraEstadoDateChanged(object sender, EventArgs e)
        {
            var txtIndex = ReorderList1.Items.Select((t, i) => new { index = i, textbox = t.FindControl("dtHoraEstado") }).Where(t => t.textbox == sender).Select(t => t.index).FirstOrDefault();
            CalcularHorarios(txtIndex);
            BindHorarios();
            BindHasta();
        }

        protected void DtHastaDateChanged(object sender, EventArgs e)
        {
            var txtIndex = ReorderList1.Items.Select((t, i) => new { index = i, textbox = t.FindControl("dtHasta") }).Where(t => t.textbox == sender).Select(t => t.index).FirstOrDefault();
            var hasta = Hasta.Get();
            var item = ReorderList1.Items[txtIndex];
            var dtHasta = item.FindControl("dtHasta") as DateTimePicker;
            var hora = dtHasta != null && dtHasta.SelectedDate.HasValue ? dtHasta.SelectedDate.Value.ToDataBaseDateTime() : hasta[txtIndex];
            hasta[txtIndex] = hora;
            Hasta.Set(hasta);

            var horarios = Horarios.Get();
            if (horarios[txtIndex] > hora)
            {
                horarios[txtIndex] = hora;
                Horarios.Set(horarios);
                BindHorarios();
            }
        }

        protected void CbEstadoEntregaOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var cbIndex = ReorderList1.Items.Select((t, i) => new { index = i, cb = t.FindControl("cbEstadoEntrega") }).Where(t => t.cb == sender).Select(t => t.index).FirstOrDefault();
            var item = ReorderList1.Items[cbIndex];
            var cbEstadoEntrega = item.FindControl("cbEstadoEntrega") as EstadoEntregaDistribucionDropDownList;
            var estado = Convert.ToInt32(cbEstadoEntrega.SelectedValue);

            var entregas = Entregas.Get();
            if (entregas[cbIndex].Estado != estado)
            {
                var entrega = DAOFactory.EntregaDistribucionDAO.FindById(entregas[cbIndex].Id);
                entrega.Estado = (short)estado;
                DAOFactory.EntregaDistribucionDAO.SaveOrUpdate(entrega);

                entregas[cbIndex].Estado = (short)estado;
                Entregas.Set(entregas);
                BindEntregas();
            }
        }

        private void CalcularHorarios(int changedIndex)
        {
            var horarios = Horarios.Get();
            var hasta = Hasta.Get();
            var prevDate = horarios.First();
            var deltas = new List<TimeSpan>(horarios.Count);
            foreach (var horario in horarios)
            {
                deltas.Add(horario.Subtract(prevDate));
                prevDate = horario;
            }

            prevDate = Inicio;
            for (var i = 0; i < ReorderList1.Items.Count; i++)
            {
                if (i == 0)
                {
                    var dt = ReorderList1.Items[i].FindControl("dtHoraEstado") as DateTimePicker;
                    prevDate = dt != null && dt.SelectedDate.HasValue? dt.SelectedDate.Value.ToDataBaseDateTime() : prevDate;
                }
            
                DateTime hora;
                if (i == changedIndex)
                {
                    var item = ReorderList1.Items[i];
                    var dt = item.FindControl("dtHoraEstado") as DateTimePicker;
                    hora = dt != null && dt.SelectedDate.HasValue? dt.SelectedDate.Value.ToDataBaseDateTime() : prevDate;
                }
                else if (i > changedIndex)
                    hora = prevDate.Add(deltas[i]);
                else
                    hora = horarios[i];
                
                horarios[i] = hora;
                if (hasta[i] < hora) hasta[i] = hora;
                prevDate = hora;
            }
            Horarios.Set(horarios);
            Hasta.Set(hasta);
        }

        private void RecalcularHorariosPorDistancia(int oldIndex, int newIndex)
        {
            var entregas = Entregas.Get();
            if (entregas.Count <= 1) return;
            var horarios = Horarios.Get();
            var hastas = Hasta.Get();

            var prevDate = Inicio;
            var prevTime = TimeSpan.Zero;
            var deltas = horarios.Select(d => (prevTime = d.Subtract(prevDate = prevDate.Add(prevTime)))).ToList();

            var primera = entregas.First();
            var prevPunto = primera.PuntoEntrega > 0
                                ? DAOFactory.PuntoEntregaDAO.FindById(primera.PuntoEntrega).ReferenciaGeografica
                                : DAOFactory.LineaDAO.FindById(primera.Linea).ReferenciaGeografica;
            prevDate = primera.Programado;
            var prevHasta = primera.ProgramadoHasta;

            var minIndex = Math.Min(oldIndex, newIndex);
            var velocidadPromedio = GetVelocidad();

            var newHigher = newIndex > oldIndex;
            var recalc = new[] {newIndex, newIndex + 1, newHigher ? oldIndex : oldIndex + 1};

            for (var i = 0; i < entregas.Count; i++)
            {
                var entrega = entregas[i];
                var punto = entrega.PuntoEntrega > 0
                                ? DAOFactory.PuntoEntregaDAO.FindById(entrega.PuntoEntrega).ReferenciaGeografica
                                : DAOFactory.LineaDAO.FindById(entrega.Linea).ReferenciaGeografica;
                var tipoServicio = entrega.TipoServicio > 0 ? DAOFactory.TipoServicioCicloDAO.FindById(entrega.TipoServicio) : null;
                var hora = horarios[i];
                var hasta = hastas[i];
                var demora = tipoServicio != null ? tipoServicio.Demora : 0;
                if (i >= minIndex)
                {
                    hora = prevDate.Add(recalc.Contains(i)
                                            ? GetTiempoViaje(prevPunto, punto, velocidadPromedio)
                                            : deltas[i]);
                    hasta = prevHasta.Add(recalc.Contains(i)
                                              ? GetTiempoViaje(prevPunto, punto, velocidadPromedio)
                                              : deltas[i]);
                }

                horarios[i] = hora;
                hastas[i] = hasta;
                prevPunto = punto;
                prevDate = hora.AddMinutes(demora);
            }
            Horarios.Set(horarios);
            Hasta.Set(hastas);
        }
        private static TimeSpan GetTiempoViaje(ReferenciaGeografica punto1, ReferenciaGeografica punto2, int velocidad)
        {
            var distancia = GeocoderHelper.CalcularDistacia(punto1.Latitude,
                                                             punto1.Longitude,
                                                             punto2.Latitude,
                                                             punto2.Longitude);
            return TimeSpan.FromHours(distancia / velocidad);
        }
        private int GetVelocidad()
        {
            var coche = cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected) : null;
            if (coche != null)
            {
                if (coche.VelocidadPromedio > 0) return coche.VelocidadPromedio;
                if (coche.TipoCoche.VelocidadPromedio > 0) return coche.TipoCoche.VelocidadPromedio;
            }
            var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            return empresa != null ? empresa.VelocidadPromedio : 20;
        }

        #endregion

        #region Recorrido

        protected void BtInvertirClick(object sender, EventArgs e)
        {
            EditLine1.Invertir();
        }
        protected void BtLimpiarClick(object sender, EventArgs e)
        {
            EditLine1.Clear();
        }
        protected void BtCopiarRecorridoClick(object sender, EventArgs e)
        {
            var recorrido = cbRecorrido.Selected > 0 ? DAOFactory.RecorridoDAO.FindById(cbRecorrido.Selected) : null;
            if (recorrido == null) return;

            txtDesvio.Text = recorrido.Desvio.ToString(CultureInfo.InvariantCulture);
            var points = recorrido.Detalles.Select(detalle => new PointF((float)detalle.Longitud, (float)detalle.Latitud)).ToList();
            EditLine1.SetLine(points);
        }

        #endregion

        #region Map

        protected void EditLine1OnMapLoad(object sender, EventArgs eventArgs)
        {
            EditLine1.Mapa.AddLayers(LayerFactory.GetVector(LayerClientArea, true), LayerFactory.GetMarkers(LayerClientPoi, true));
        }
        private void AddMarkers()
        {
            var entregas = Entregas.Get();
            EditLine1.Mapa.ClearLayer(LayerClientArea);
            EditLine1.Mapa.ClearLayer(LayerClientPoi);

            var mostrar = cbFiltroEntregas.SelectedValues;

            foreach(var entrega in entregas)
            {
                if (!mostrar.Contains(entrega.Orden.ToString("#0"))) continue;

                var dom = DAOFactory.ReferenciaGeograficaDAO.FindById(entrega.ReferenciaGeografica);
                if (dom.Poligono != null)
                {
                    var center = dom.Poligono.FirstPoint;
                    var col = StyleFactory.GetPointFromColor(dom.Color.Color);
                    var id = dom.Id + "_GEO";

                    if (dom.Poligono.Radio > 0)
                    {
                        EditLine1.Mapa.AddGeometries(LayerClientArea, new Point(id, center.X, center.Y, dom.Poligono.Radio, col));
                    }
                    else
                    {
                        var points = dom.Poligono.ToPointFList();
                        var poly = new Polygon(id, col);
                        for (var i = 0; i < points.Count; i++)
                            poly.AddPoint(new Point(id + "_" + i, points[i].X, points[i].Y));

                        EditLine1.Mapa.AddGeometries(LayerClientArea, poly);
                    }
                }

                if (dom.Direccion == null) continue;

                var icono = dom.Icono != null ? IconDir + dom.Icono.PathIcono : string.Empty;
                var marker = new LabeledMarker(dom.Id.ToString("#0"),
                                               icono,
                                               dom.Direccion.Latitud,
                                               dom.Direccion.Longitud,
                                               entrega.Orden.ToString("#0"),
                                               "ol_marker_labeled")
                                 {
                                     Size = DrawingFactory.GetSize(dom.Icono != null ? dom.Icono.Width : 32,
                                                                   dom.Icono != null ? dom.Icono.Height : 32),
                                     Offset = DrawingFactory.GetOffset(dom.Icono != null ? dom.Icono.OffsetX : 0,
                                                                       dom.Icono != null ? dom.Icono.OffsetY : 0)
                                 };

                EditLine1.Mapa.AddMarkers(LayerClientPoi, marker);

                if (mostrar.Count() == 1)
                {
                    EditLine1.Mapa.SetCenter(dom.Direccion.Latitud, dom.Direccion.Longitud);
                }
            }
        }
        protected bool SetCenterLinea()
        {
            if (cbLinea.Selected <= 0) return false;
            var l = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
            if (l.ReferenciaGeografica == null || (l.ReferenciaGeografica.Direccion == null && l.ReferenciaGeografica.Poligono == null)) return false;

            var lat = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Latitud : l.ReferenciaGeografica.Poligono.Centro.Y;
            var lon = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Longitud : l.ReferenciaGeografica.Poligono.Centro.X;

            EditLine1.SetCenter(lat, lon);
            return true;
        }

        #endregion
	    
        #region Salida y Regreso a Base
        private void CreateOrigen()
        {
            var entregas = Entregas.Get();
            var horarios = Horarios.Get();
            var hastas = Hasta.Get();
            if (!EditMode || (entregas.Any() && entregas.First().Linea <= 0))
            {
                var primera = entregas.FirstOrDefault();
                DateTime horario;
                DateTime hasta;
                if (primera != null && primera.Linea > 0)
                {
                    horario = horarios.First();
                    hasta = hastas.First();
                    entregas.RemoveAt(0);
                    horarios.RemoveAt(0);
                    hastas.RemoveAt(0);
                }
                else
                {
                    horario = dtFecha.SelectedDate.HasValue ? dtFecha.SelectedDate.Value : Inicio;
                    hasta = horario;
                }
                
                if (cbLinea.Selected > 0)
                {
                    var linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
                    var entrega = new Entrega(new EntregaDistribucion { Descripcion = linea.Descripcion, Linea = linea, Viaje = EditObject });

                    if (entregas.Count > 0)
                    {
                        entregas.Insert(0, entrega);
                        horarios.Insert(0, horario);
                        hastas.Insert(0, hasta);
                    }
                    else
                    {
                        entregas.Add(entrega);
                        horarios.Add(horario);
                        hastas.Add(hasta);
                    }
                }

                Horarios.Set(horarios);
                Hasta.Set(hastas);
                Entregas.Set(entregas);
            }
        }
        private void DeleteDestino()
        {
            var entregas = Entregas.Get();
            var horarios = Horarios.Get();
            var hasta = Hasta.Get();
            if (entregas.Count > 1 && entregas.Last().Linea > 0)
            {
                entregas.RemoveAt(entregas.Count - 1);
                horarios.RemoveAt(horarios.Count - 1);
                hasta.RemoveAt(hasta.Count - 1);
                Entregas.Set(entregas);
                Horarios.Set(horarios);
                Hasta.Set(hasta);
                BindEntregas();
            }
        }
        private void CreateDestino()
        {
            var entregas = Entregas.Get();
            var horarios = Horarios.Get();
            var hasta = Hasta.Get();

            if (entregas.Count == 1 || entregas.Last().Linea <= 0)
            {
                var linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
                var entrega = new Entrega(new EntregaDistribucion { Descripcion = linea.Descripcion, Linea = linea, Viaje = EditObject });

                var velocidadPromedio = GetVelocidad();
                DateTime hora;
                if (entregas.Count > 0)
                {
                    var ultima = entregas.Last();
                    var lastPunto = ultima.PuntoEntrega > 0
                                        ? DAOFactory.PuntoEntregaDAO.FindById(ultima.PuntoEntrega).ReferenciaGeografica
                                        : DAOFactory.LineaDAO.FindById(ultima.Linea).ReferenciaGeografica;
                    var lastTipoServicio = ultima.TipoServicio > 0
                                               ? DAOFactory.TipoServicioCicloDAO.FindById(ultima.TipoServicio)
                                               : null;
                    var viaje = GetTiempoViaje(lastPunto, linea.ReferenciaGeografica, velocidadPromedio);
                    var demora = lastTipoServicio != null ? lastTipoServicio.Demora : 0;
                    hora = horarios.Last().Add(viaje).AddMinutes(demora);
                }
                else hora = Inicio;

                entregas.Add(entrega);
                horarios.Add(hora);
                hasta.Add(hora);

                Entregas.Set(entregas);
                Horarios.Set(horarios);
                Hasta.Set(hasta);
                BindEntregas();
            }
        } 
        #endregion
    }

    [Serializable]
    public class Entrega
    {
        public int Id { get; set; }
        public DateTime Programado { get; set; }
        public DateTime ProgramadoHasta { get; set; }
        public int Orden { get; set; }
        public int PuntoEntrega { get; set; }
        public int Cliente { get; set; }
        public int Linea { get; set; }
        public int ReferenciaGeografica { get; set; }
        public string Descripcion { get; set; }
        public DateTime? Entrada { get; set; }
        public DateTime? Manual { get; set; }
        public DateTime? Salida { get; set; }
        public short Estado { get; set; }
        public string Icono { get; set; }
        public string PuntoEntregaDescripcion { get; set; }
        public string ClienteDescripcion { get; set; }
        public int TipoServicio { get; set; }
        public bool Nomenclado { get; set; }

        public Entrega() { }
        public Entrega(EntregaDistribucion entrega)
        {
            Id = entrega.Id;
            Programado = entrega.Programado;
            ProgramadoHasta = entrega.ProgramadoHasta;
            Orden = entrega.Orden;
            PuntoEntrega = entrega.PuntoEntrega != null ? entrega.PuntoEntrega.Id : -1;
            Cliente = entrega.Cliente != null ? entrega.Cliente.Id : -1;
            Linea = entrega.Linea != null ? entrega.Linea.Id : -1;
            Descripcion = entrega.Descripcion;
            var georef = entrega.ReferenciaGeografica;
            ReferenciaGeografica = georef.Id;
            Entrada = entrega.Entrada;
            Manual = entrega.Manual;
            Salida = entrega.Salida;
            Estado = entrega.Estado;
            Icono = georef.Icono != null ? georef.Icono.PathIcono : string.Empty;
            PuntoEntregaDescripcion = entrega.PuntoEntrega != null 
                ? entrega.PuntoEntrega.Descripcion 
                : entrega.Linea != null 
                    ? entrega.Linea.ReferenciaGeografica.Descripcion
                    : string.Empty;
            ClienteDescripcion = entrega.Cliente != null ? entrega.Cliente.Descripcion : string.Empty;
            TipoServicio = entrega.TipoServicio != null ? entrega.TipoServicio.Id : 0;
            Nomenclado = entrega.PuntoEntrega == null || entrega.PuntoEntrega.Nomenclado;
        }
    }
}
