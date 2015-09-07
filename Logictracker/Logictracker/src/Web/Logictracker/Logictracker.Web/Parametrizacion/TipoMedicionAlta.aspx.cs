using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class TipoMedicionAlta : SecuredAbmPage<TipoMedicion>
    {
        protected override string RedirectUrl { get { return "TipoMedicionLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TIPO_MEDICION"; } }
        protected override string GetRefference() { return "PAR_TIPO_MEDICION"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            chkControlaLimites.Checked = EditObject.ControlaLimites;
            cbUnidadMedida.SetSelectedValue(EditObject.UnidadMedida != null ? EditObject.UnidadMedida.Id : -1);
        }

        protected override void OnDelete()
        {
            DAOFactory.TipoMedicionDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.ControlaLimites = chkControlaLimites.Checked;
            EditObject.UnidadMedida = DAOFactory.UnidadMedidaDAO.FindById(cbUnidadMedida.Selected);

            DAOFactory.TipoMedicionDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {   
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            var tipo = DAOFactory.TipoMedicionDAO.FindByCode(code);
            ValidateDuplicated(tipo, "CODE");

            ValidateEntity(cbUnidadMedida.Selected, "PARENTI85");
        }
    }
}
