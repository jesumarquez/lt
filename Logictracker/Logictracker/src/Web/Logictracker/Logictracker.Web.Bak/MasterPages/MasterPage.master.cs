#region Usings

using System;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.MasterPages
{
    public partial class MasterPage : BaseMasterPage
    {
        #region Protected Methods

        /// <summary>
        /// Sets the user name.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) lblUserName.Text = Usuario.Name;
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btLogout_Click(object sender, EventArgs e)
        {
            WebSecurity.Logout();

            Response.Redirect("~/",false);
        }

        /// <summary>
        /// Displays user profile information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btPerfil_Click(object sender, EventArgs e)
        {
            Session["id"] = Usuario.Id;

            Response.Redirect("~/perfil.aspx",false);
        }

        #endregion
    }
}
