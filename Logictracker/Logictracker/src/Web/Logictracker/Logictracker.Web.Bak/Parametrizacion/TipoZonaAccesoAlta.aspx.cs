using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoZonaAccesoAlta : SecuredAbmPage<TipoZonaAcceso>
    {
        protected override string RedirectUrl { get { return "TipoZonaAccesoLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPOS_ZONA_ACCESO"; } }
        protected override string GetRefference() { return "PAR_TIPOS_ZONA_ACCESO"; }
        
        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
        }

        protected override void OnDelete()
        {
            DAOFactory.TipoZonaAccesoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            
            DAOFactory.TipoZonaAccesoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            var code = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");
            
            if (!DAOFactory.TipoZonaAccesoDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");
        }
    }
}
