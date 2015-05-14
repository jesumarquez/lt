using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Security;
using Logictracker.Types.SecurityObjects;
using Logictracker.Web.Helpers;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Web.BaseClasses.BasePages
{
    /// <summary>
    /// Base page that implements all the necessary security steps for the session handling.
    /// </summary>
    public abstract class SessionSecuredPage : BasePage
    {
        #region Protected Properties

        /// <summary>
        /// Redirect url for re-login.
        /// </summary>
        protected virtual string NotLoguedUrl { get { return string.Format("{0}Default.aspx?RedirectUrl={1}", ApplicationPath, Server.UrlEncode(Request.Url.PathAndQuery)); } }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the logged in user.
        /// </summary>
        protected static UserSessionData Usuario { get { return WebSecurity.AuthenticatedUser; } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the user privileges for accessing the page.
        /// </summary>
        private void ApplySecurity() { if (!WebSecurity.Authenticated) OnSessionLoss(); }

        /// <summary>
        /// Gets all the inner exception messages.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private static string GetInnerExceptionText(Exception error)
        {
            var message = string.Empty;

            while (error.InnerException != null)
            {
                message = string.Concat(message, error.InnerException.Message, "<br />");

                error = error.InnerException;
            }

            return message;
        }

        /// <summary>
        /// Gets the exception stack trace.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private static string GetStackTrace(Exception error) { return error.StackTrace != null ? error.StackTrace.Replace("\n", "<br />") : " - "; }

        /// <summary>
        /// Traces into database the detected error.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="lastUrl"></param>
        private void LogError(Exception e, string lastUrl)
        {
            if (e is ApplicationException) return;

            var context = GetCustomErrorContext() ?? new Dictionary<string, string>();

			context.Add("user.id", Usuario.Id.ToString(CultureInfo.InvariantCulture));

			foreach (var id in Usuario.IdPerfiles) context.Add("user.profiles", id.ToString(CultureInfo.InvariantCulture));

			STrace.Log(lastUrl, e, 0, LogTypes.Error, context, e.Message);
        }

        /// <summary>
        /// Sends a mail notifying the detected error.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="lastUrl"></param>
        private static void SendMail(Exception e, string lastUrl)
        {
            var mailSender = new MailSender(Config.Mailing.ErrorMailingConfig);

            mailSender.SendMail(GetErrorMessage(e, lastUrl));
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the current custom context for the detected error.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<String, String> GetCustomErrorContext() { return null; }

        /// <summary>
        /// Displays all exception messages.
        /// </summary>
        /// <param name="ex"></param>
        protected override void DisplayError(Exception ex)
        {
            NotifyError(ex, Request.Url.AbsolutePath);

            base.DisplayError(ex);
        }

        /// <summary>
        /// Method thar redirects the user in case of a session loss.
        /// </summary>
        protected virtual void OnSessionLoss() { Response.Redirect(NotLoguedUrl, true); }

        /// <summary>
        /// Applies security, culture and theming to the page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Usuario == null) return;

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = Usuario.Culture;

            ThemeManager.ApplyLogo(this, Usuario.Logo);
        }

        protected override IEnumerable<GaCustomVar> GetGaCustomVars
        {
            get
            {
                return new[]{ new GaCustomVar(1,"Usuario", Usuario != null ? Usuario.Name : string.Empty, GaCustomVar.Scopes.SessionLevel) };
            }
        }
        /// <summary>
        /// Applyies user defined themes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            ApplySecurity();
        
            if (Usuario != null) ThemeManager.ApplyTheme(this, Usuario.Theme);
        }

        /// <summary>
        /// Sends the email with the exception, user, and last visited page, according to the config file.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="lastUrl"></param>
        protected Boolean NotifyError(Exception e, string lastUrl)
        {
            try
            {
                LogError(e, lastUrl);

                if(e.GetType() != typeof(ApplicationException)) SendMail(e, lastUrl);

                return true;
            }
            catch
            {
            	return false;
            }
        }

        /// <summary>
        /// Gets the email body.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="lastPage"></param>
        /// <returns></returns>
        protected static string GetErrorMessage(Exception e, String lastPage)
        {
            var empresa = string.Empty;
            try
            {
                var dao = new DAOFactory();
                var usuario = dao.UsuarioDAO.FindById(Usuario.Id);
                if (usuario != null && usuario.NombreUsuario == Usuario.Name)
                {
                    var empresasHabilitas = usuario.Empresas.OfType<Empresa>();
                    if (empresasHabilitas.Count() == 1) 
                        empresa = " (" + empresasHabilitas.First().RazonSocial + ")";
                    else if (empresasHabilitas.Count() > 1)
                        empresa = " (" + empresasHabilitas.First().RazonSocial + " y " + (empresasHabilitas.Count() - 1) + " empresas más)";
                }
            }
            catch (Exception) { }
            
            return string.Concat(
				"User: ", Usuario.Name + empresa, "<br />",
				"Error on Page: ", lastPage, "<br /><br />",
				"Message: ", e.Message, "<br /><br />", 
				"Strack Trace: ", GetStackTrace(e), "<br /><br />",
				"Inner Exception: ", GetInnerExceptionText(e));
        }

        #endregion
    }
}