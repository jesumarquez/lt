using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ZonaAccesoAlta : SecuredAbmPage<ZonaAcceso>
    {
        protected override string RedirectUrl { get { return "ZonaAccesoLista.aspx"; } }
        protected override string VariableName { get { return "PAR_ZONAS_ACCESO"; } }
        protected override string GetRefference() { return "PAR_ZONAS_ACCESO"; }
        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbTipoZonaAcceso.SetSelectedValue(EditObject.TipoZonaAcceso != null ? EditObject.TipoZonaAcceso.Id : cbTipoZonaAcceso.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
        }

        protected override void OnDelete()
        {
            DAOFactory.ZonaAccesoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.TipoZonaAcceso = DAOFactory.TipoZonaAccesoDAO.FindById(cbTipoZonaAcceso.Selected);
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            
            DAOFactory.ZonaAccesoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbTipoZonaAcceso.Selected, "PARENTI91");
            var code = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");
            
            if (!DAOFactory.ZonaAccesoDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");
        }
    }
}
