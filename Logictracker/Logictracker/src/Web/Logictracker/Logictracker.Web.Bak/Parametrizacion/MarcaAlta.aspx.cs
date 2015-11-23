using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class MarcaAlta : SecuredAbmPage<Marca>
    {
        protected override string VariableName { get { return "PAR_MARCAS"; } }

        protected override string RedirectUrl { get { return "MarcaLista.aspx"; } }

        protected override string GetRefference() { return "PAR_MARCAS"; }

        protected override bool AddButton { get { return false; } }

        protected override bool DuplicateButton { get { return false; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtMarca.Text = EditObject.Descripcion;
        }

        protected override void OnDelete()
        {
            DAOFactory.MarcaDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Descripcion = txtMarca.Text;
            DAOFactory.MarcaDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEmpty((string) txtMarca.Text, (string) "PARENTI06");
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
        }
    }
}
