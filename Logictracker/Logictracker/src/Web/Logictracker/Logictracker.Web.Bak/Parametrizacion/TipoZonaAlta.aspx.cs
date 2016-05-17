using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoZonaAlta : SecuredAbmPage<TipoZona>
    {
        protected override string RedirectUrl { get { return "TipoZonaLista.aspx"; } }
        protected override string VariableName { get { return GetRefference(); } }
        protected override string GetRefference() { return "PAR_TIPO_ZONA"; }
        
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
        }

        protected override void OnDelete()
        {
            DAOFactory.TipoZonaDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            
            DAOFactory.TipoZonaDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            var code = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            var byCode = DAOFactory.TipoZonaDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, code);
            ValidateDuplicated(byCode, "CODE");
        }
    }
}
