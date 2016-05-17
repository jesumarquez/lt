using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos
{
    public partial class PeriodoAlta : SecuredAbmPage<Periodo>
    {
        protected override string RedirectUrl { get { return "Periodos.aspx"; } }
        protected override string VariableName { get { return "COST_PERIODOS"; } }
        protected override string GetRefference() { return "PERIODO"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            txtDescripcion.Text = EditObject.Descripcion;
            dtDesde.SelectedDate = EditObject.FechaDesde;
            dtHasta.SelectedDate = EditObject.FechaHasta;

            if (EditObject.Estado != Periodo.Abierto)
                cbEmpresa.Enabled = txtDescripcion.Enabled = dtDesde.Enabled = dtHasta.Enabled = false;
        }

        protected override void OnDelete()
        {
            if (EditObject.Estado != Periodo.Abierto)
                ThrowError("PERIODO_NO_ABIERTO");

            DAOFactory.PeriodoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.FechaDesde = dtDesde.SelectedDate.Value;
            EditObject.FechaHasta = dtHasta.SelectedDate.Value;
            EditObject.Estado = Periodo.Abierto;
            DAOFactory.PeriodoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");
            ValidateEmpty(dtDesde.SelectedDate, "DESDE");
            ValidateEmpty(dtHasta.SelectedDate, "HASTA");
        }
    }
}
