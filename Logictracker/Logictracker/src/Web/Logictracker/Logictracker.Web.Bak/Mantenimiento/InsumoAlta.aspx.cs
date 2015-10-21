using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Mantenimiento
{
    public partial class InsumoAlta : SecuredAbmPage<Insumo>
    {
        protected override string RedirectUrl { get { return "InsumoLista.aspx"; } }
        protected override string VariableName { get { return "MAN_INSUMOS"; } }
        protected override string GetRefference() { return "MAN_INSUMOS"; }

        protected override bool DuplicateButton { get { return true; } }
        protected override bool AddButton { get { return true; } }
        
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbTipoInsumo.SetSelectedValue(EditObject.TipoInsumo.Id);
            cbUnidadMedida.SetSelectedValue(EditObject.UnidadMedida != null ? EditObject.UnidadMedida.Id : -1);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            txtValorReferencia.Text = EditObject.ValorReferencia.ToString("#0.00");
        }

        protected override void OnDelete() { DAOFactory.InsumoDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Empresa = (cbEmpresa.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = (cbLinea.Selected > 0) ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.TipoInsumo = DAOFactory.TipoInsumoDAO.FindById(cbTipoInsumo.Selected);
            EditObject.UnidadMedida = DAOFactory.UnidadMedidaDAO.FindById(cbUnidadMedida.Selected);
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            double valorReferencia;
            double.TryParse(txtValorReferencia.Text, out valorReferencia);
            EditObject.ValorReferencia = valorReferencia;

            DAOFactory.InsumoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbTipoInsumo.Selected, "PARENTI60");
            ValidateEntity(cbUnidadMedida.Selected, "PARENTI85");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            if (!DAOFactory.InsumoDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, cbTipoInsumo.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");

            if (txtValorReferencia.Text != string.Empty)
                ValidateDouble(txtValorReferencia.Text, "VALOR_REFERENCIA");
        }
    }
}
