using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionCentroCostoAlta : SecuredAbmPage<CentroDeCostos>
    {
        protected override string VariableName { get { return "CENTROS_COSTOS"; } }
        protected override string RedirectUrl { get { return "CentroCostoLista.aspx"; } }
        protected override string GetRefference() { return "CENTRO_COSTOS"; }

        protected override void OnDelete() { DAOFactory.CentroDeCostosDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = ddlEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected) : EditObject.Linea != null ? EditObject.Linea.Empresa : null;
            EditObject.Departamento = cbDepartamento.Selected > 0 ? DAOFactory.DepartamentoDAO.FindById(cbDepartamento.Selected) : null;

            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Codigo = txtCodigo.Text.Trim();

            EditObject.Empleado = cbEmpleado.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbEmpleado.Selected) : null;

            EditObject.GeneraDespachos = chkGeneraEntregas.Checked;
            EditObject.InicioAutomatico = chkInicioAutomatico.Checked;
            EditObject.HorarioInicio = dtHorario.SelectedDate.HasValue
                                           ? dtHorario.SelectedDate.Value
                                           : (DateTime?) null;

            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            DAOFactory.CentroDeCostosDAO.SaveOrUpdate(EditObject);

            if (EditMode || !user.PorCentroCostos) { return; }

            user.AddCentro(DAOFactory.CentroDeCostosDAO.FindById(EditObject.Id));

            DAOFactory.UsuarioDAO.SaveOrUpdate(user, false);
        }
        
        protected override void Bind()
        {
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            chkGeneraEntregas.Checked = EditObject.GeneraDespachos;
            chkInicioAutomatico.Checked = EditObject.InicioAutomatico;
            dtHorario.SelectedDate = EditObject.HorarioInicio.HasValue
                                         ? EditObject.HorarioInicio.Value
                                         : (DateTime?) null;
        }

        protected void DdlEmpresaPreBind(object sender, EventArgs e) { if (EditMode) ddlEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : ddlEmpresa.AllValue; }

        protected void CbLineaPreBind(object sender, EventArgs e) { if (EditMode) cbLinea.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue; }

        protected void CbDeptoPreBind(object sender, EventArgs e) { if (EditMode) cbDepartamento.EditValue = EditObject.Departamento != null ? EditObject.Departamento.Id : cbDepartamento.AllValue; }

        protected void CbEmpleadoPreBind(object sender, EventArgs e) { if (EditMode) cbEmpleado.EditValue = EditObject.Empleado != null ? EditObject.Empleado.Id : cbEmpleado.NoneValue; }

        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text.Trim())) ThrowMustEnter("DESCRIPCION");
            if (string.IsNullOrEmpty(txtCodigo.Text.Trim())) ThrowMustEnter("CODE");

            //Codigo Unico
            var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            var empresa = linea != null ? linea.Empresa : ddlEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected) : null;
            if (!DAOFactory.CentroDeCostosDAO.IsCodeUnique(txtCodigo.Text.Trim(), empresa, linea, EditObject.Id)) 
                ThrowDuplicated("CODE");
        }
    }
}
