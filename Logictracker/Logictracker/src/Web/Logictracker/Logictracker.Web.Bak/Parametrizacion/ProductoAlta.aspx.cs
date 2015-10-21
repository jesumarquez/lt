using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ProductoAlta : SecuredAbmPage<Producto>
    {
        protected override string VariableName { get { return "PAR_PRODUCTO"; } }
        protected override string RedirectUrl { get { return "ProductoLista.aspx"; } }
        protected override string GetRefference() { return "PAR_PRODUCTO"; }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbBocaDeCarga.SetSelectedValue(EditObject.BocaDeCarga != null ? EditObject.BocaDeCarga.Id : cbBocaDeCarga.AllValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            txtObservaciones.Text = EditObject.Observaciones;
            chkUsaPrefijo.Checked = EditObject.UsaPrefijo;
        }

        protected override void OnDelete() { DAOFactory.ProductoDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.BocaDeCarga = cbBocaDeCarga.Selected > 0 ? DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected) : null;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Observaciones = txtObservaciones.Text;

            if (WebSecurity.IsSecuredAllowed(Securables.Hormigon))
            {
                EditObject.UsaPrefijo = chkUsaPrefijo.Checked;
            }
            DAOFactory.ProductoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEmpty((string) txtDescripcion.Text, (string) "PARENTI06");
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            var producto = DAOFactory.ProductoDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, cbBocaDeCarga.Selected, code);
            ValidateDuplicated(producto, "CODE");
        }
    }
}
