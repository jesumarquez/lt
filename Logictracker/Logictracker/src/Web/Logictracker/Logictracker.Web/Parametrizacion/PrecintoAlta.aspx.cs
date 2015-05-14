using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PrecintoAlta : SecuredAbmPage<Precinto>
    {
        protected override string VariableName { get { return "PAR_PRECINTO"; } }

        protected override string RedirectUrl { get { return "PrecintoLista.aspx"; } }

        protected override string GetRefference() { return "PAR_PRECINTO"; }

        protected override bool AddButton { get { return false; } }

        protected override bool DuplicateButton { get { return false; } }

        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
        }

        protected override void OnDelete()
        {
            var dispositivos = DAOFactory.DispositivoDAO.GetByPrecinto(EditObject);

            if (dispositivos.Count == 0)
                DAOFactory.PrecintoDAO.Delete(EditObject);
            else
                ThrowError("PRECINTO_ASIGNADO", new[] { dispositivos[0].Codigo });
        }

        protected override void OnSave()
        {
            EditObject.Codigo = txtCodigo.Text.Trim();
            
            DAOFactory.PrecintoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            if (!DAOFactory.PrecintoDAO.IsCodeUnique(EditObject.Id, code))
                ThrowDuplicated("CODE");
        }
    }
}
