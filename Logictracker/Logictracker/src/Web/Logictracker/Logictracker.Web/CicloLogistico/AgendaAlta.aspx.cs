using System;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.Controls;

namespace Logictracker.Web.CicloLogistico
{
    public partial class AgendaAlta : SecuredAbmPage<AgendaVehicular>
    {
        protected override string VariableName { get { return "PAR_AGENDA"; } }
        protected override string RedirectUrl { get { return "AgendaLista.aspx"; } }
        protected override string GetRefference() { return "PAR_AGENDA"; }
        protected override bool DuplicateButton { get { return false; } }
        protected override bool DeleteButton { get { return false; } }
        protected override bool SaveButton { get { return false; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dtpDesde.SetDate();
                dtpHasta.SetDate();
                SetContextKey();

                if (EditMode)
                {
                    auEmpleado.Visible = false;
                    cbEmpleado.Visible = true;
                }
                else
                {
                    auEmpleado.Visible = true;
                    cbEmpleado.Visible = false;
                }
            }
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            cbLinea.SetSelectedValue(EditObject.Linea.Id);
            cbDepartamento.SetSelectedValue(EditObject.Departamento != null ? EditObject.Departamento.Id : cbDepartamento.AllValue);
            dtpDesde.SelectedDate = EditObject.FechaDesde.ToDisplayDateTime();
            dtpHasta.SelectedDate = EditObject.FechaHasta.ToDisplayDateTime();
            lblDtDesde.Text = EditObject.FechaDesde.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");
            lblDtHasta.Text = EditObject.FechaHasta.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");

            cbVehiculo.Items.Clear();
            cbVehiculo.Items.Add(new ListItem(EditObject.Vehiculo.Interno, EditObject.Vehiculo.Id.ToString("#0")));
            cbVehiculo.SelectedIndex = cbVehiculo.Items.IndexOf(cbVehiculo.Items.FindByValue(EditObject.Vehiculo.Id.ToString("#0")));
            cbEmpleado.SelectedIndex = cbEmpleado.Items.IndexOf(cbEmpleado.Items.FindByValue(EditObject.Empleado.Id.ToString("#0")));
            cbTurno.SetSelectedValue(EditObject.Turno != null ? EditObject.Turno.Id : cbTurno.AllValue);

            if (EditMode)
            {
                btnReservar.Visible = false;
                btnCancelar.Visible = true;
                cbEmpresa.Enabled = false;
                cbLinea.Enabled = false;
                cbDepartamento.Enabled = false;
                cbVehiculo.Enabled = false;
                cbEmpleado.Enabled = false;
                cbTurno.Enabled = false;
                dtpDesde.Visible = false;
                dtpHasta.Visible = false;
                lblDtDesde.Visible = true;
                lblDtHasta.Visible = true;
                btnConsultar.Enabled = false;
                btnCancelar.Visible = EditObject.Estado != AgendaVehicular.Estados.Cancelado;
                auEmpleado.Visible = false;
                cbEmpleado.Visible = true;
            }
            else
            {
                auEmpleado.Visible = true;
                cbEmpleado.Visible = false;
            }
        }

        protected override void OnDelete() { }

        protected void BtnReservarOnClick(object sender, EventArgs e)
        {
            try
            {
                ValidateSave();
                OnSave();
                AfterSave();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        protected void BtnCancelarOnClick(object sender, EventArgs e)
        {
            EditObject.Estado = AgendaVehicular.Estados.Cancelado;
            DAOFactory.AgendaVehicularDAO.SaveOrUpdate(EditObject);

            AfterSave();
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbLinea.Selected, "PARENTI02");
            ValidateEntity(int.Parse(cbVehiculo.SelectedValue), "PARENTI03");
            ValidateEntity(int.Parse(cbEmpleado.SelectedValue), "PARENTI09");

            if (dtpDesde.SelectedDate.Value < DateTime.UtcNow.ToDisplayDateTime()) ThrowInvalidValue("DESDE");
            if (dtpHasta.SelectedDate.Value <= dtpDesde.SelectedDate.Value) ThrowInvalidValue("HASTA");

            var documentos = DAOFactory.DocumentoDAO.FindVencidosForEmpleado(int.Parse(cbEmpleado.SelectedValue), dtpHasta.SelectedDate.Value);
            if (documentos.Any()) ThrowError("EMPLEADO_DOC_VENCIDO");
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
            EditObject.Departamento = cbDepartamento.Selected > 0 ? DAOFactory.DepartamentoDAO.FindById(cbDepartamento.Selected) : null;
            EditObject.Vehiculo = DAOFactory.CocheDAO.FindById(int.Parse(cbVehiculo.SelectedValue));
            EditObject.Empleado = DAOFactory.EmpleadoDAO.FindById(auEmpleado.Selected);
            EditObject.Turno = cbTurno.Selected > 0 ? DAOFactory.ShiftDAO.FindById(cbTurno.Selected) : null;

            EditObject.FechaDesde = dtpDesde.SelectedDate.Value.ToDataBaseDateTime();
            EditObject.FechaHasta = dtpHasta.SelectedDate.Value.ToDataBaseDateTime();

            DAOFactory.AgendaVehicularDAO.SaveOrUpdate(EditObject);
        }

        protected void BtnConsultarOnClick(object sender, EventArgs e)
        {
            var agendas = DAOFactory.AgendaVehicularDAO.GetList(cbEmpresa.SelectedValues,
                                                                cbLinea.SelectedValues,
                                                                new[] { -1 },
                                                                new[] { -1 },
                                                                dtpDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                                dtpHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                       .Where(a => a.Estado == AgendaVehicular.Estados.EnCurso || a.Estado == AgendaVehicular.Estados.Reservado);

            var asignados = agendas.Select(a => a.Vehiculo).Distinct();
            var vehiculos = DAOFactory.CocheDAO.GetList(cbEmpresa.SelectedValues, cbLinea.SelectedValues, new[]{-1}, new[]{-1}, new[]{-1});
            var noAsignados = vehiculos.Where(v => !asignados.Contains(v));

            var disponibles = noAsignados.Where(v => v.Departamento != null && v.Departamento.Id == cbDepartamento.Selected);
            if (!disponibles.Any())
                disponibles = noAsignados.Where(v => v.Departamento == null);
            
            cbVehiculo.Items.Clear();
            foreach (var vehiculo in disponibles) cbVehiculo.Items.Add(new ListItem(vehiculo.Interno, vehiculo.Id.ToString("#0")));
            cbVehiculo.Enabled = true;
        }

        protected void ConditionChanged(object sender, EventArgs e)
        {
            cbVehiculo.Items.Clear();
            cbVehiculo.Enabled = false;
        }
        protected void SetContextKey()
        {
            auEmpleado.ContextKey = AutoCompleteTextBox.CreateContextKey(new[] { cbEmpresa.Selected },
                                                                         new[] { cbLinea.Selected },
                                                                         new[] { -1 },
                                                                         new[] { -1 },
                                                                         new[] { -1 },
                                                                         new[] { cbDepartamento.Selected });
        }
    }
}
