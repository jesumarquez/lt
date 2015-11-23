using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoEntidadAlta : SecuredAbmPage<TipoEntidad>
    {
        protected override string RedirectUrl { get { return "TipoEntidadLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_ENTIDAD"; } }
        protected override string GetRefference() { return "PAR_TIPO_ENTIDAD"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
        }

        protected override void OnDelete()
        {
            DAOFactory.TipoEntidadDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            DAOFactory.TipoEntidadDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {   
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            ValidateEntity(cbEmpresa.Selected, "PARENTI01");

            if (!DAOFactory.TipoEntidadDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");
        }
    }
}
