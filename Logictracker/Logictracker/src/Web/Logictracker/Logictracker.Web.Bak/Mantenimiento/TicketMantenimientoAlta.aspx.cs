using C1.Web.UI.Controls.C1GridView;
using System;
using System.Globalization;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class TicketMantenimientoAlta : SecuredAbmPage<TicketMantenimiento>
    {
        protected override string RedirectUrl { get { return "TicketMantenimientoLista.aspx"; } }
        protected override string VariableName { get { return "MANT_TICKET"; } }
        protected override string GetRefference() { return "MANT_TICKET"; }
        protected override bool DeleteButton { get { return false; } }
        protected override bool DuplicateButton { get { return false; } }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Request.QueryString["v"] != null)
            {
                int id;
                if (int.TryParse(Request.QueryString["v"], out id) && id > 0)
                {
                    cbVehiculo.SetSelectedValue(id);
                    var vehiculo = DAOFactory.CocheDAO.FindById(id);
                    if (vehiculo != null)
                        cbEmpresa.SetSelectedValue(vehiculo.Empresa.Id);
                }
            }
        }

        protected override void Bind()
        {
            cbTaller.SetSelectedValue(EditObject.Taller != null ? EditObject.Taller.Id : cbTaller.NoneValue);
            dtInicio.SelectedDate = EditObject.FechaSolicitud.ToDisplayDateTime();
            txtCodigo.Text = EditObject.Codigo;
            cbNivel.SetSelectedValue(EditObject.NivelComplejidad);
            cbEstado.SetSelectedValue(EditObject.Estado);
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbVehiculo.SetSelectedValue(EditObject.Vehiculo != null ? EditObject.Vehiculo.Id : cbVehiculo.NoneValue);
            cbTipoVehiculo.SetSelectedValue(EditObject.Vehiculo != null ? EditObject.Vehiculo.TipoCoche.Id : cbTipoVehiculo.AllValue);
            cbEmpleado.SetSelectedValue(EditObject.Empleado != null ? EditObject.Empleado.Id : cbEmpleado.NoneValue);

            dtTurno.SelectedDate = EditObject.FechaTurno.HasValue ? EditObject.FechaTurno.Value.ToDisplayDateTime() : (DateTime?) null;
            dtRecepcion.SelectedDate = EditObject.FechaRecepcion.HasValue ? EditObject.FechaRecepcion.Value.ToDisplayDateTime() : (DateTime?) null;
            dtVerificacion.SelectedDate = EditObject.FechaVerificacion.HasValue ? EditObject.FechaVerificacion.Value.ToDisplayDateTime() : (DateTime?) null;
            dtTrabajoTerminado.SelectedDate = EditObject.FechaTrabajoTerminado.HasValue ? EditObject.FechaTrabajoTerminado.Value.ToDisplayDateTime() : (DateTime?) null;
            dtEntrega.SelectedDate = EditObject.FechaEntrega.HasValue ? EditObject.FechaEntrega.Value.ToDisplayDateTime() : (DateTime?) null;
            dtTrabajoAceptado.SelectedDate = EditObject.FechaTrabajoAceptado.HasValue ? EditObject.FechaTrabajoAceptado.Value.ToDisplayDateTime() : (DateTime?) null;

            txtPresupuesto.Text = EditObject.Presupuesto;
            txtMonto.Text = EditObject.Monto.ToString(CultureInfo.InvariantCulture);
            dtPresupuesto.SelectedDate = EditObject.FechaPresupuestada.HasValue ? EditObject.FechaPresupuestada.Value.ToDisplayDateTime() : (DateTime?) null;
            cbEstadoPresupuesto.SetSelectedValue(EditObject.EstadoPresupuesto);
            txtPrimerPresupuesto.Text = EditObject.PrimerPresupuesto;
            dtPrimerPresupuesto.SelectedDate = EditObject.FechaPresupuestoOriginal.HasValue ? EditObject.FechaPresupuestoOriginal.Value.ToDisplayDateTime() : (DateTime?) null;
            dtRecotizacion.SelectedDate = EditObject.FechaRecotizacion.HasValue ? EditObject.FechaRecotizacion.Value.ToDisplayDateTime() : (DateTime?) null;
            dtAprobacion.SelectedDate = EditObject.FechaAprobacion.HasValue ? EditObject.FechaAprobacion.Value.ToDisplayDateTime() : (DateTime?) null;
            txtEntrada.Text = EditObject.Entrada.HasValue ? EditObject.Entrada.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            txtSalida.Text = EditObject.Salida.HasValue ? EditObject.Salida.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;

            BindHistoria();
        }

        protected override void OnSave()
        {
            EditObject.Taller = cbTaller.Selected > 0 ? DAOFactory.TallerDAO.FindById(cbTaller.Selected) : null;
            if (dtInicio.SelectedDate.HasValue) EditObject.FechaSolicitud = SecurityExtensions.ToDataBaseDateTime(dtInicio.SelectedDate.Value);
            EditObject.Codigo = txtCodigo.Text;
            EditObject.NivelComplejidad = (short)cbNivel.Selected;
            EditObject.Estado = (short) cbEstado.Selected;
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Vehiculo = cbVehiculo.Selected > 0 ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected) : null;
            EditObject.Empleado = cbEmpleado.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbEmpleado.Selected) : null;

            EditObject.FechaTurno = dtTurno.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtTurno.SelectedDate.Value) : (DateTime?) null;
            EditObject.FechaRecepcion = dtRecepcion.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtRecepcion.SelectedDate.Value) : (DateTime?)null;
            EditObject.FechaVerificacion = dtVerificacion.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtVerificacion.SelectedDate.Value) : (DateTime?)null;
            EditObject.FechaTrabajoTerminado = dtTrabajoTerminado.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtTrabajoTerminado.SelectedDate.Value) : (DateTime?)null;
            EditObject.FechaEntrega = dtEntrega.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtEntrega.SelectedDate.Value) : (DateTime?)null;
            EditObject.FechaTrabajoAceptado = dtTrabajoAceptado.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtTrabajoAceptado.SelectedDate.Value) : (DateTime?)null;

            EditObject.Presupuesto = txtPresupuesto.Text;
            double monto;
            double.TryParse(txtMonto.Text, out monto);
            EditObject.Monto = monto;
            EditObject.FechaPresupuestada = dtPresupuesto.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtPresupuesto.SelectedDate.Value) : (DateTime?)null;
            EditObject.EstadoPresupuesto = (short)cbEstadoPresupuesto.Selected;
            EditObject.PrimerPresupuesto = txtPrimerPresupuesto.Text;
            EditObject.FechaPresupuestoOriginal = dtPrimerPresupuesto.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtPrimerPresupuesto.SelectedDate.Value) : (DateTime?)null;
            EditObject.FechaRecotizacion = dtRecotizacion.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtRecotizacion.SelectedDate.Value) : (DateTime?)null;
            EditObject.FechaAprobacion = dtAprobacion.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtAprobacion.SelectedDate.Value) : (DateTime?)null;
            EditObject.Descripcion = txtDescripcion.Text;

            var historia = new HistoriaTicketMantenimiento
                               {
                                   Codigo = EditObject.Codigo,
                                   Descripcion = EditObject.Descripcion,
                                   Empleado = EditObject.Empleado,
                                   Empresa = EditObject.Empresa,
                                   Estado = EditObject.Estado,
                                   EstadoPresupuesto = EditObject.EstadoPresupuesto,
                                   Fecha = DateTime.UtcNow,
                                   FechaAprobacion = EditObject.FechaAprobacion,
                                   FechaEntrega = EditObject.FechaEntrega,
                                   FechaPresupuestada = EditObject.FechaPresupuestada,
                                   FechaPresupuestoOriginal = EditObject.FechaPresupuestoOriginal,
                                   FechaRecepcion = EditObject.FechaRecepcion,
                                   FechaRecotizacion = EditObject.FechaRecotizacion,
                                   FechaSolicitud = EditObject.FechaSolicitud,
                                   FechaTrabajoAceptado = EditObject.FechaTrabajoAceptado,
                                   FechaTrabajoTerminado = EditObject.FechaTrabajoTerminado,
                                   FechaTurno = EditObject.FechaTurno,
                                   FechaVerificacion = EditObject.FechaVerificacion,
                                   Monto = EditObject.Monto,
                                   NivelComplejidad = EditObject.NivelComplejidad,
                                   Presupuesto = EditObject.Presupuesto,
                                   PrimerPresupuesto = EditObject.PrimerPresupuesto,
                                   Taller = EditObject.Taller,
                                   TicketMantenimiento = EditObject,
                                   Usuario = DAOFactory.UsuarioDAO.FindById(Usuario.Id),
                                   Vehiculo = EditObject.Vehiculo
                               };

            EditObject.Historia.Add(historia);

            DAOFactory.TicketMantenimientoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            base.ValidateSave();

            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbVehiculo.Selected, "PARENTI03");
            ValidateEntity(cbTaller.Selected, "PARENTI35");
            if (cbEstado.Selected < 0) ThrowMustEnter("TICKET_MANT_ESTADO");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            ValidateEmpty((DateTime?) dtInicio.SelectedDate, (string) "FECHA");
            if (dtInicio.SelectedDate.HasValue && dtInicio.SelectedDate.Value > DateTime.UtcNow.ToDisplayDateTime()) 
                ThrowError("INCIDENCE_CANNOT_START_IN_FUTURE");

            var isUnique = DAOFactory.TicketMantenimientoDAO.IsUnique(EditObject.Id, cbTaller.Selected, cbVehiculo.Selected, (short)cbEstado.Selected);
            if (!isUnique) ThrowError("EXISTE_TICKET_ABIERTO");
        }

        protected override void OnDelete() { }

        private void BindHistoria()
        {
            gridEstados.DataSource = EditObject.Historia;
            gridEstados.DataBind();
            gridPresupuesto.DataSource = EditObject.Historia;
            gridPresupuesto.DataBind();
        }

        protected void GridEstadosItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var detail = e.Row.DataItem as HistoriaTicketMantenimiento;

            if (detail == null) return;

            e.Row.BackColor = TicketMantenimiento.EstadosTicket.GetColor(detail.Estado);
            e.Row.Cells[0].Text = detail.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            e.Row.Cells[1].Text = detail.Usuario.NombreUsuario;
            e.Row.Cells[2].Text = CultureManager.GetLabel(TicketMantenimiento.EstadosTicket.GetLabelVariableName(detail.Estado));
            e.Row.Cells[3].Text = CultureManager.GetLabel(TicketMantenimiento.NivelesComplejidad.GetLabelVariableName(detail.NivelComplejidad));
            e.Row.Cells[4].Text = detail.FechaTurno.HasValue ? detail.FechaTurno.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[5].Text = detail.FechaRecepcion.HasValue ? detail.FechaRecepcion.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[6].Text = detail.FechaVerificacion.HasValue ? detail.FechaVerificacion.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[7].Text = detail.FechaTrabajoTerminado.HasValue ? detail.FechaTrabajoTerminado.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[8].Text = detail.FechaEntrega.HasValue ? detail.FechaEntrega.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[9].Text = detail.FechaTrabajoAceptado.HasValue ? detail.FechaTrabajoAceptado.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[10].Text = detail.Descripcion;
        }

        protected void GridPresupuestoItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var detail = e.Row.DataItem as HistoriaTicketMantenimiento;

            if (detail == null) return;

            e.Row.BackColor = TicketMantenimiento.EstadosPresupuesto.GetColor(detail.EstadoPresupuesto);
            e.Row.Cells[0].Text = detail.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            e.Row.Cells[1].Text = detail.Usuario.NombreUsuario;
            e.Row.Cells[2].Text = detail.Presupuesto;
            e.Row.Cells[3].Text = detail.Monto.ToString("#0.00");
            e.Row.Cells[4].Text = detail.FechaPresupuestada.HasValue ? detail.FechaPresupuestada.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[5].Text = CultureManager.GetLabel(TicketMantenimiento.EstadosPresupuesto.GetLabelVariableName(detail.EstadoPresupuesto));
            e.Row.Cells[6].Text = detail.PrimerPresupuesto;
            e.Row.Cells[7].Text = detail.FechaPresupuestoOriginal.HasValue ? detail.FechaPresupuestoOriginal.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[8].Text = detail.FechaRecotizacion.HasValue ? detail.FechaRecotizacion.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
            e.Row.Cells[9].Text = detail.FechaAprobacion.HasValue ? detail.FechaAprobacion.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : string.Empty;
        }
    }
}
