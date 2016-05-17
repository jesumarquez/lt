using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DAL.NHibernate;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Admin
{
    public partial class ProgramacionReporteAlta : SecuredAbmPage<ProgramacionReporte>
    {
        protected override string RedirectUrl { get { return "ProgramacionReporteLista.aspx"; } }
        protected override string VariableName { get { return "ADMIN_PROGRAMACION_REPORTE"; } }
        protected override string GetRefference() { return "ADMIN_PROGRAMACION_REPORTE"; }

        protected override bool DuplicateButton { get { return false; } }
        protected override bool AddButton { get { return false; } }
        
        protected override void Bind()
        {
            cbPeriodicidad.Items.Clear();
            cbPeriodicidad.Items.Insert(0, new ListItem(CultureManager.GetLabel("DIARIO"), "D"));
            cbPeriodicidad.Items.Insert(1, new ListItem(CultureManager.GetLabel("SEMANAL"), "S"));
            cbPeriodicidad.Items.Insert(2, new ListItem(CultureManager.GetLabel("MENSUAL"), "M"));

            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtReporte.Text = CultureManager.GetLabel(EditObject.Report);
            txtReportName.Text = EditObject.ReportName;
            txtReportDescription.Text = EditObject.Description;
            cbPeriodicidad.SelectedValue = EditObject.Periodicity.ToString();
            txtMail.Text = EditObject.Mail;
            chkActivo.Checked = EditObject.Active;
            rbutExcel.Checked = EditObject.Format == ProgramacionReporte.FormatoReporte.Excel;
            rbutHtml.Checked = EditObject.Format == ProgramacionReporte.FormatoReporte.Html;
        }

        protected override void OnDelete()
        {
            EditObject.Active = false;            
            DAOFactory.ProgramacionReporteDAO.Delete(EditObject); // SaveOrUpdate(EditObject);
        }

        protected override void OnSave()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                EditObject.Empresa = (cbEmpresa.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                EditObject.Linea = (cbLinea.Selected > 0) ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                EditObject.Periodicity = cbPeriodicidad.SelectedValue[0];
                EditObject.Mail = txtMail.Text;
                EditObject.Active = chkActivo.Checked;
                EditObject.Format = rbutExcel.Checked ? ProgramacionReporte.FormatoReporte.Excel : ProgramacionReporte.FormatoReporte.Html;

                DAOFactory.ProgramacionReporteDAO.SaveOrUpdate(EditObject);

                transaction.Commit();
            }
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEmpty(txtMail.Text, "MAIL");
        }
    }
}
