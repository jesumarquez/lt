using System;
using System.Linq;
using Logictracker.CicloLogistico.Controls;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class AltaServicio : SecuredAbmPage<Servicio>
    {
        private const string SinPermisos = "El usuario actual no tiene permisos para editar este elemento.";

        protected override string VariableName { get { return "CLOG_SERVICIOS"; } }

        protected override string RedirectUrl { get { return "ServicioLista.aspx"; } }
        protected override string GetRefference() { return "SERVICIO"; }

        protected void cbEmpresa_InitialBindig(object sender, EventArgs e) { if (EditMode && EditObject.Owner.Empresa != null) cbEmpresa.SetSelectedValue(EditObject.Owner.Empresa.Id); }
        protected void cbLinea_InitialBindig(object sender, EventArgs e) { if (EditMode && EditObject.Owner.Linea != null) cbLinea.SetSelectedValue(EditObject.Owner.Linea.Id); }
        protected void cbCicloLogistico_InitialBinding(object sender, EventArgs e) { if (EditMode && EditObject.CicloLogistico != null) cbCicloLogistico.SetSelectedValue(EditObject.CicloLogistico.Id); }
        protected void cbVehiculo_InitialBinding(object sender, EventArgs e) { if (EditMode && EditObject.Vehiculo != null) cbVehiculo.SetSelectedValue(EditObject.Vehiculo.Id); }
        protected void cbChofer_InitialBinding(object sender, EventArgs e) { if (EditMode && EditObject.Chofer != null) cbChofer.SetSelectedValue(EditObject.Chofer.Id); }

        protected override void OnPreLoad(EventArgs e)
        {
            listaDetalles.LoadDetalles();
            base.OnPreLoad(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EditMode) dtInicio.SelectedDate = DateTime.Now;
        }

        protected void cbVehiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = cbVehiculo.Selected;
            if (id <= 0) return;
            var coche = DAOFactory.CocheDAO.FindById(id);
            if (coche.Chofer == null) return;
            cbChofer.SetSelectedValue(coche.Chofer.Id);
        }

        protected void cbCicloLogistico_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = cbCicloLogistico.Selected;

            if (id <= 0)
            {
                LoadDetalles(null);
                return;
            }

            var ciclo = DAOFactory.CicloLogisticoDAO.FindById(id);
            lblCustom1.Text = ciclo.Custom1;
            lblCustom2.Text = ciclo.Custom2;
            lblCustom3.Text = ciclo.Custom3;

            if (EditMode && !IsPostBack) return;
            LoadDetalles(ciclo);
        }

        protected override void Bind()
        {
            var empresa = EditObject.Owner != null ? EditObject.Owner.Empresa != null ? EditObject.Owner.Empresa.Id : -1 : -1;
            var linea = EditObject.Owner != null ? EditObject.Owner.Linea != null ? EditObject.Owner.Linea.Id : -1 : -1;
            if (!CanEditEmpresa(empresa) || !CanEditLinea(linea)) ShowInfo(SinPermisos);

            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            txtCustom1.Text = EditObject.Custom1;
            txtCustom2.Text = EditObject.Custom2;
            txtCustom3.Text = EditObject.Custom3;
            cbCicloLogistico.SetSelectedValue(EditObject.CicloLogistico.Id);
            cbVehiculo.SetSelectedValue(EditObject.Vehiculo != null ? EditObject.Vehiculo.Id : -1);
            cbChofer.SetSelectedValue(EditObject.Chofer != null ? EditObject.Chofer.Id : -1);

            dtInicio.SelectedDate = EditObject.FechaInicio.ToDisplayDateTime().Date;

            BindDataDetalles();
        }
        private void BindDataDetalles()
        {
            var detalles = EditObject.Detalles.OfType<DetalleServicio>().OrderBy(det => det.Orden).ToList();
            listaDetalles.SetCiclo(EditObject.CicloLogistico);
            listaDetalles.SetDetalles(detalles);
        }

        protected override void OnDelete()
        {
            EditObject.Estado = Servicio.EstadoCancelado;
            DAOFactory.ServicioDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnSave()
        {
            var empresa = EditObject.Owner != null ? EditObject.Owner.Empresa != null ? EditObject.Owner.Empresa.Id : -1 : -1;
            var linea = EditObject.Owner != null ? EditObject.Owner.Linea != null ? EditObject.Owner.Linea.Id : -1 : -1;
            if (!CanEditEmpresa(empresa) || !CanEditLinea(linea)) throw new ApplicationException(SinPermisos);

            EditObject.Owner = new Owner();
            if (cbLinea.Selected > 0)
            {
                EditObject.Owner.Linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
                EditObject.Owner.Empresa = EditObject.Owner.Linea.Empresa;
            }
            else if (cbEmpresa.Selected > 0)
            {
                EditObject.Owner.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            }

            EditObject.CicloLogistico = DAOFactory.CicloLogisticoDAO.FindById(cbCicloLogistico.Selected);
            EditObject.Vehiculo = cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected) : null;
            EditObject.Chofer = cbChofer.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbChofer.Selected) : null;

            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Custom1 = txtCustom1.Text.Trim();
            EditObject.Custom2 = txtCustom2.Text.Trim();
            EditObject.Custom3 = txtCustom3.Text.Trim();
            EditObject.Descripcion = txtDescripcion.Text.Trim();


            EditObject.FechaInicio = DateTime.MinValue;

            var date = dtInicio.SelectedDate.GetValueOrDefault().Date;
            var lastDate = date;

            var detalles = listaDetalles.Detalles;
            var current = EditObject.Detalles.OfType<DetalleServicio>().ToDictionary(detalle => detalle.Id, detalle => detalle);
            EditObject.Detalles.Clear();
            short orden = 0;
            foreach (var detalle in detalles)
            {
                if (detalle.Programada == DateTime.MinValue) throw new ApplicationException("Ingrese la hora para " + detalle.DetalleCiclo.Descripcion);

                date = date.Date.Add(detalle.Programada.TimeOfDay);
                if (EditObject.FechaInicio == DateTime.MinValue) EditObject.FechaInicio = date;
                else if (date < lastDate) date = date.AddDays(1);

                EditObject.FechaFin = date;

                if (current.ContainsKey(detalle.Id))
                {
                    var cur = current[detalle.Id];
                    cur.Servicio = EditObject;
                    cur.Mensaje = detalle.Mensaje;
                    cur.Minutos = detalle.Minutos;
                    cur.Obligatorio = detalle.Obligatorio;
                    cur.Orden = detalle.Orden;
                    cur.ReferenciaGeografica = detalle.ReferenciaGeografica;
                    cur.Tipo = detalle.Tipo;
                    cur.Programada = date;
                    cur.Orden = orden;
                    EditObject.Detalles.Add(cur);
                }
                else
                {
                    detalle.Id = 0;
                    detalle.Servicio = EditObject;
                    detalle.Programada = date;
                    detalle.Orden = orden;
                    EditObject.Detalles.Add(detalle);
                }

                lastDate = date;
                orden++;
            }

            if (!EditMode) EditObject.FechaAlta = DateTime.UtcNow;
            DAOFactory.ServicioDAO.SaveOrUpdate(EditObject);

        }

        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text.Trim())) ThrowMustEnter("DESCRIPCION");

            if (string.IsNullOrEmpty(txtCodigo.Text.Trim())) ThrowMustEnter("CODIGO");

            if (!dtInicio.SelectedDate.HasValue) ThrowMustEnter("FECHA");

            if (cbCicloLogistico.Selected == 0) ThrowMustEnter("CICLO_LOGISTICO");

            var ciclo = DAOFactory.ServicioDAO.GetByCodigo(cbEmpresa.Selected, cbLinea.Selected, txtCodigo.Text.Trim());

            if (ciclo == null) return;

            if (!EditMode || (EditMode && !EditObject.Id.Equals(ciclo.Id)))
                throw new ApplicationException("Ya existe un servicio con el mismo codigo");

        }

        private void LoadDetalles(Logictracker.Types.BusinessObjects.Tickets.CicloLogistico ciclo)
        {
            listaDetalles.SetCiclo(ciclo);
            if (!EditMode) listaDetalles.SetDate(DateTime.Now);
        }

        protected void listaDetalles_DateChange(object sender, EventArgs e)
        {
            var ctl = sender as ControlDetalleServicio;
            var list = listaDetalles.GetControlsEstado();
            ControlDetalleServicio prev = null;
            foreach (var detalle in list)
            {
                if (detalle == ctl) break;
                prev = detalle;
            }
            if (prev != null)
            {
                if (ctl != null)
                {
                    var date = ctl.GetFirstDate();

                    var prevDate = prev.GetFirstDate();
                    prev.Duracion = Convert.ToInt32(date.Subtract(prevDate).TotalMinutes);
                }
            }

            listaDetalles.SetDate(listaDetalles.GetFirstDate());
        }
    }
}