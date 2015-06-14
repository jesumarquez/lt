using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Logictracker.Security;
using Logictracker.Security.Exceptions;
using Logictracker.Types.BusinessObjects.Auditoria;
using Logictracker.Types.BusinessObjects.Organizacion;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Mobile
{
    public partial class Mobile_Default : BasePage
    {

        #region Protected Properties

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return null; } }

        /// <summary>
        /// Gets the custom culture selected by the user.
        /// </summary>
        protected string UserCulture
        {
            get { return ViewState["UserCulture"] != null ? ViewState["UserCulture"].ToString() : null; }
            set { ViewState["UserCulture"] = value; }
        }

        private int LoginUser
        {
            get { return (int)(ViewState["LoginUser"] ?? 0); }
            set { ViewState["LoginUser"] = value; }
        }

        #endregion

        #region Private Properies

        /// <summary>
        /// The loged in user.
        /// </summary>
        private Usuario _usuario;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Shows welcome page when accesing throw default2 or the login page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(UserCulture))
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureSelector.SelectedValue);

            cultureSelector.SelectedCountryChanged += cultureSelector_SelectedCountryChanged;

            SelectLoginStyle();

            if (IsPostBack) return;

            if (LoginUser > 0) ShowWellcome();
            else ShowLogin();
        }

        /// <summary>
        /// Established the newly selected culture as the culture to be used for the current session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cultureSelector_SelectedCountryChanged(object sender, EventArgs e)
        {
            UserCulture = cultureSelector.SelectedValue;

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureSelector.SelectedValue);
        }

        /// <summary>
        /// Validates the user name and password, and in case of succesfull login the profiles asigned to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUsuario.Text == String.Empty) throw new ApplicationException(CultureManager.GetError("NO_USERNAME"));
                if (txtPassword.Text == String.Empty) throw new ApplicationException(CultureManager.GetError("NO_PASSWORD"));

                _usuario = DAOFactory.UsuarioDAO.FindForLogin(txtUsuario.Text, txtPassword.Text);

                WebSecurity.ValidateLogin(_usuario);

                DAOFactory.LoginAuditDAO.SaveOrUpdate(new LoginAudit
                                                          {
                                                              Usuario = _usuario,
                                                              FechaInicio = DateTime.UtcNow,
                                                              FechaFin = null,
                                                              IP = Request.UserHostAddress
                                                          });

                var profiles = DAOFactory.PerfilDAO.FindPerfilesByUsuario(_usuario);

                if (profiles.Count == 0) throw new Exception(CultureManager.GetError("NO_ROLES"));

                if (profiles.Count == 1)
                {
                    var selectedProfile = profiles.First().Id;

                    IEnumerable<Logictracker.Types.SecurityObjects.Module> modules;
                    IEnumerable<Asegurable> securables;
                    IEnumerable<int> perfiles = DAOFactory.PerfilDAO.GetProfileAccess(_usuario, selectedProfile, out modules, out securables);

                    WebSecurity.Login(_usuario, perfiles, modules, securables);

                    ShowWellcome();
                }
                else
                {
                    LoginUser = _usuario.Id;
                    ShowPerfil();
                    BindPerfiles();
                }
            }
            catch (WrongUserPassException)
            {
                ShowError(new ApplicationException(CultureManager.GetError("WRONG_USER_PASS")));
            }
            catch (NoAccessException)
            {
                ShowError(new ApplicationException(CultureManager.GetError("NO_ACCESS")));
            }
            catch (UserDisabledException)
            {
                _usuario.Inhabilitado = true;
                _usuario.FechaExpiracion = null;
                DAOFactory.UsuarioDAO.SaveOrUpdate(_usuario);
                ShowError(new ApplicationException(CultureManager.GetError("USER_DISABLED")));
            }
            catch (RestrictedIpException)
            {
                ShowError(new ApplicationException("No puede acceder al sistema desde esta ubicacion"));
            }
            catch (Exception ex)
            {
                infoLabel1.Text = ex.Message;
            }
        }

        /// <summary>
        /// Cancels the login.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btCancel_Click(object sender, EventArgs e)
        {
            ShowLogin();
        }

        /// <summary>
        /// Selects the indicated profile for the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btSelPerfil_Click(object sender, EventArgs e)
        {

            try
            {
                if (LoginUser > 0)
                {
                    _usuario = DAOFactory.UsuarioDAO.FindById(LoginUser);

                    if (cbPerfiles.SelectedIndex == -1) throw new Exception(string.Format(CultureManager.GetError("NO_SELECTED"), CultureManager.GetEntity("ROLE")));

                    var selectedProfile = Convert.ToInt32(cbPerfiles.SelectedValue);

                    IEnumerable<Logictracker.Types.SecurityObjects.Module> modules;
                    IEnumerable<Asegurable> securables;
                    IEnumerable<int> perfiles = DAOFactory.PerfilDAO.GetProfileAccess(_usuario, selectedProfile, out modules, out securables);

                    WebSecurity.Login(_usuario, perfiles, modules, securables);

                    ShowWellcome();
                }
                else ShowLogin();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Select a special login style if the request cames from a specific subdomain.
        /// </summary>
        private void SelectLoginStyle()
        {
            var host = Request.Url.Host;

            if (host.StartsWith("www.") || host.IndexOf('.') < 0) return;

            var style = host.Split('.')[0];

            int ip;

            if (int.TryParse(style, out ip)) return;

            divBack.Attributes.Add("class", style);
        }

        /// <summary>
        /// Cleans the page up.
        /// </summary>
        private void CleanUp()
        {
            panPerfil.Visible = false;
            panLogin.Visible = false;
            txtUsuario.Text = null;
            txtPassword.Text = null;

            cbPerfiles.Items.Clear();
        }

        /// <summary>
        /// Show the login controls.
        /// </summary>
        private void ShowLogin()
        {
            CleanUp();

            panLogin.Visible = true;
        }

        /// <summary>
        /// Shows the profile selection controls.
        /// </summary>
        private void ShowPerfil()
        {
            CleanUp();

            panPerfil.Visible = true;
            form1.DefaultButton = "btSelPerfil";
            form1.DefaultFocus = "cbPerfiles";
        }

        /// <summary>
        /// Shows the welcome page.
        /// </summary>
        private void ShowWellcome()
        {
            var user = WebSecurity.AuthenticatedUser;

            if (!string.IsNullOrEmpty(UserCulture)) user.Culture = CultureInfo.GetCultureInfo(UserCulture);

            Response.Redirect(
                !string.IsNullOrEmpty(Request.Params["RedirectUrl"])
                    ? Server.UrlDecode(Request.Params["RedirectUrl"])
                    : String.Concat(ApplicationPath, "Mobile/Documentos/MDocumentoLista.aspx"), false);
        }

        /// <summary>
        /// Bind all user profiles.
        /// </summary>
        private void BindPerfiles()
        {
            cbPerfiles.DataValueField = "NO";
            cbPerfiles.DataSource = DAOFactory.PerfilDAO.FindPerfilesByUsuario(_usuario);
            cbPerfiles.DataTextField = "Descripcion";
            cbPerfiles.DataValueField = "Id";
            cbPerfiles.DataBind();
        }

        #endregion
    }
}