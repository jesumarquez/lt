using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class DepositoAlta : SecuredAbmPage<Deposito>
    {
        protected override string RedirectUrl { get { return "DepositoLista.aspx"; } }
        protected override string VariableName { get { return "MAN_DEPOSITOS"; } }
        protected override string GetRefference() { return "MAN_DEPOSITOS"; }

        protected override bool DuplicateButton { get { return true; } }
        protected override bool AddButton { get { return true; } }
        
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
        }

        protected override void OnDelete() { DAOFactory.DepositoDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Empresa = (cbEmpresa.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = (cbLinea.Selected > 0) ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;

            DAOFactory.DepositoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            if (!DAOFactory.DepositoDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");
        }
    }
}
