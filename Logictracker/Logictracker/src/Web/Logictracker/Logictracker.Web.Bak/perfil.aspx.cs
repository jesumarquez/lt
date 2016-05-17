#region Usings

using System;
using System.Globalization;
using System.Threading;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;
using Logictracker.Web.Helpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker
{
    public partial class _perfil : SecuredAbmPage<Usuario>
    {
        #region Protected Properties

        /// <summary>
        /// C1 Tool bar control.
        /// </summary>
        protected override ToolBar ToolBar { get { return null; } }

        /// <summary>
        /// List page associated to the abm.
        /// </summary>
        protected override string RedirectUrl { get { return "Home.aspx"; } }

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        protected override string VariableName { get { return ""; } }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        protected override string PageTitle { get { return string.Format("{0} - {1}", ApplicationTitle, CultureManager.GetUser("PROFILE_TITLE")); } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Binds initial values.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!EditMode) Response.Redirect(RedirectUrl,false);

            if (IsPostBack) return;

            BindThemes();

            if (EditObject!= null && EditObject.Tipo <= 2 && EditObject.InhabilitadoCambiarPass) txtClave.Enabled = txtConfirmacion.Enabled = false;
        }

        /// <summary>
        /// Saves the edited user.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Entidad = AltaEntidad.GetEntidad(false);

            if (txtClave.Visible) DAOFactory.UsuarioDAO.SaveOrUpdate(EditObject);
            else DAOFactory.UsuarioDAO.UpdatePersonalData(EditObject);
        }

        /// <summary>
        /// Deletes the user being edited.
        /// </summary>
        protected override void OnDelete() { }

        /// <summary>
        /// Binds all properties of the user being edited.
        /// </summary>
        protected override void Bind()
        {
            AltaEntidad.SetEntidad(EditObject.Entidad);

            lblUsuario.Text = EditObject.NombreUsuario;

            var liuh = ddlUsoHorario.Items.FindByValue(EditObject.TimeZoneId);
            if (liuh != null) liuh.Selected = true;
            BindThemes();
            var lith = cbTheme.Items.FindByValue(EditObject.Theme);
            if (lith != null) lith.Selected = true;

            cultureSelector.SelectedValue = EditObject.Culture;
        }

        /// <summary>
        /// Validates the current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (txtClave.Visible && (txtClave.Text == string.Empty || txtConfirmacion.Text == string.Empty
                                     || (!txtClave.Text.Equals(txtConfirmacion.Text))))
                throw new Exception(CultureManager.GetError("PASSWORDS_DONT_MATCH"));
        }

        protected void btGuardar_Click(object sender, EventArgs e)
        {
            Save();
            MultiView1.ActiveViewIndex = 0;
        }

        protected void btCancelar_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 0; }

        protected void lnkDatos_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 1; }

        protected void lnkContraseña_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 2; }

        protected void lnkTheme_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 3; }

        protected void lnkConfiguracionRegional_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 4; }

        protected void btAceptarPass_Click(object sender, EventArgs e)
        {
            ValidateSave();
            EditObject.Clave = txtClave.Text;
            DAOFactory.UsuarioDAO.SaveOrUpdate(EditObject, txtClave.Text.Equals(string.Empty));
            //DAOFactory.UsuarioDAO.SaveOrUpdate(EditObject, DAOFactory.UsuarioDAO.FindById(Usuario.Id));
            MultiView1.ActiveViewIndex = 0;
        }

        protected void btCancelarPass_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 0; }
        protected void btCancelarTheme_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 0; }

        protected void btAceptarTheme_Click(object sender, EventArgs e)
        {
            EditObject.Theme = cbTheme.SelectedValue;
            DAOFactory.UsuarioDAO.SaveOrUpdate(EditObject, true);

            Usuario.Theme = cbTheme.SelectedValue;
            Session.Add("id", EditObject.Id);
            Response.Redirect("perfil.aspx",false);
            //MultiView1.ActiveViewIndex = 0;
        }

        protected override void ApplyModuleSecurity() { }

        protected override string GetRefference() { return string.Empty; }

        #endregion

        protected void BindThemes()
        {
            if (cbTheme.Items.Count > 0) return;
            cbTheme.DataSource = ThemeManager.GetThemes();
            cbTheme.DataBind();
        }

        protected void rbCancelConfRegional_Click(object sender, EventArgs e) { MultiView1.ActiveViewIndex = 0; }

        protected void rbAceptConfRegional_Click(object sender, EventArgs e)
        {
            if (cbPredeterminados.Checked)
            {
                EditObject.TimeZoneId = ddlUsoHorario.SelectedValue;
                EditObject.Culture = cultureSelector.SelectedValue;
            }

            DAOFactory.UsuarioDAO.SaveOrUpdate(EditObject, true);

            Usuario.GmtModifier = TimeZoneInfo.FindSystemTimeZoneById(ddlUsoHorario.SelectedValue).BaseUtcOffset.TotalHours;
            Usuario.Culture = CultureInfo.GetCultureInfo(cultureSelector.SelectedValue);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = Usuario.Culture;

            MultiView1.ActiveViewIndex = 0;
        }
    }
}
