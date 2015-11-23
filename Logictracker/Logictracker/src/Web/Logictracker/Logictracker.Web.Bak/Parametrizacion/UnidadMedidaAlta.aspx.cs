using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class UnidadMedidaAlta : SecuredAbmPage<UnidadMedida>
    {
        protected override string RedirectUrl { get { return "UnidadMedidaLista.aspx"; } }
        protected override string VariableName { get { return "PAR_UNIDAD_MEDIDA"; } }
        protected override string GetRefference() { return "PAR_UNIDAD_MEDIDA"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            txtSimbolo.Text = EditObject.Simbolo;
        }

        protected override void OnDelete()
        {
            DAOFactory.UnidadMedidaDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Simbolo = txtSimbolo.Text;

            DAOFactory.UnidadMedidaDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {   
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            ValidateEmpty((string) txtSimbolo.Text, (string) "SIMBOLO");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            var tipo = DAOFactory.UnidadMedidaDAO.FindByCode(code);
            ValidateDuplicated(tipo, "CODE");
        }
    }
}
