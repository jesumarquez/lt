using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DAL.NHibernate;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;
using Logictracker.Security;
using System;
using System.IO;

namespace Logictracker.Admin
{
    public partial class LogiclinkAlta : SecuredAbmPage<LogicLinkFile>
    {
        protected override string RedirectUrl { get { return "Logiclink.aspx"; } }
        protected override string VariableName { get { return "LOGICLINK_ADMIN"; } }
        protected override string GetRefference() { return "LOGICLINK_ADMIN"; }
        protected override bool DuplicateButton { get { return false; } }
        protected override bool AddButton { get { return false; } }
        protected override bool DeleteButton { get { return false; } }
        protected override bool SaveButton { get { return false; } }

        private const string FileDirectory = "C:\\inetpub\\LogicLink\\FileResources\\";
        
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            
            var value = cbEstrategia.Items.FindByText(EditObject.Strategy).Value;
            cbEstrategia.SetSelectedValue(int.Parse(value));

            lblNombre.Text = EditObject.FilePath.Contains("@") ? EditObject.FilePath.Split('@')[1] : EditObject.FilePath;
            lblOrigen.Text = EditObject.FileSource;
            lblFechaAlta.Text = EditObject.DateAdded.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss");
            lblFechaProcesamiento.Text = EditObject.DateProcessed.HasValue ? EditObject.DateProcessed.Value.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            lblEstado.Text = LogicLinkFile.Estados.GetString((short)EditObject.Status);
            lblResultado.Text = EditObject.Result;

            panelTopLeft.Enabled = fileUpload.Visible = lblArchivo.Visible = btnGuardar.Visible = !EditMode;
            panelTopRight.Visible = EditMode;
        }

        protected void BtnGuardarOnClick(object sender, EventArgs e)
        {
            ValidateSave();
            OnSave();
            Response.Redirect(RedirectUrl);
        }

        protected override void OnSave()
        {
            if (!EditMode)
            {
                EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
                EditObject.Linea = (cbLinea.Selected > 0) ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                EditObject.Strategy = cbEstrategia.SelectedItem.Text;

                EditObject.ServerName = "DEDICADO";
                EditObject.FileSource = WebSecurity.AuthenticatedUser.Name;
                EditObject.DateAdded = DateTime.UtcNow;
                EditObject.Status = LogicLinkFile.Estados.Pendiente;

                if (fileUpload.HasFile)
                {
                    if (!Directory.Exists(FileDirectory))
                        Directory.CreateDirectory(FileDirectory);

                    var now = DateTime.UtcNow;
                    var filename = now.ToString("yyyyMMddHHmmss") + "@" + fileUpload.FileName;
                    var path = FileDirectory + filename;

                    fileUpload.SaveAs(path);
                    EditObject.FilePath = path;
                }

                DAOFactory.LogicLinkFileDAO.SaveOrUpdate(EditObject);
            }
        }
        protected override void OnDelete() { }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");

            if (!EditMode && !fileUpload.HasFile) ThrowMustEnter("ARCHIVO");
        }
    }
}
