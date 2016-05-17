using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionDepartamentoAlta : SecuredAbmPage<Departamento>
    {
        protected override string VariableName { get { return "PAR_DEPARTAMENTO"; } }
        protected override string RedirectUrl { get { return "DepartamentoLista.aspx"; } }
        protected override string GetRefference() { return "PAR_DEPARTAMENTO"; }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id: cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbEmpleado.SetSelectedValue(EditObject.Empleado != null ? EditObject.Empleado.Id : cbEmpleado.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
        }

        protected override void ValidateSave()
        {
            ValidateEmpty((string) txtCodigo.Text, (string) "CODE");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empleado = cbEmpleado.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbEmpleado.Selected) : null;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;

            DAOFactory.DepartamentoDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnDelete()
        {
            DAOFactory.DepartamentoDAO.Delete(EditObject);
        }
    }
}
