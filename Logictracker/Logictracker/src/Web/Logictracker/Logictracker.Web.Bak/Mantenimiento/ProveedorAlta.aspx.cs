using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class ProveedorAlta : SecuredAbmPage<Proveedor>
    {
        protected override string RedirectUrl { get { return "ProveedorLista.aspx"; } }
        protected override string VariableName { get { return "MAN_PROVEEDOR"; } }
        protected override string GetRefference() { return "MAN_PROVEEDOR"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbTipoProveedor.SetSelectedValue(EditObject.TipoProveedor != null ? EditObject.TipoProveedor.Id : cbTipoProveedor.AllValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
        }

        protected override void OnDelete() { DAOFactory.ProveedorDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Empresa = (cbEmpresa.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = (cbLinea.Selected > 0) ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.TipoProveedor = (cbTipoProveedor.Selected > 0) ? DAOFactory.TipoProveedorDAO.FindById(cbTipoProveedor.Selected) : null;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            
            DAOFactory.ProveedorDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbTipoProveedor.Selected, "PARENTI86");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            if (!DAOFactory.ProveedorDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");
        }
    }
}
