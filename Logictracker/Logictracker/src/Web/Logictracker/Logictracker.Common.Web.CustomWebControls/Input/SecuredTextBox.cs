using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Logictracker.Security;

namespace Logictracker.Web.CustomWebControls.Input
{
    public class SecuredTextBox:TextBox
    {
        /// <summary>
        /// Secures the control
        /// </summary>
        [Category("Custom Resources")]
        public string SecureRefference
        {
            get { return ViewState["SecureRefference"] != null ? ViewState["SecureRefference"].ToString() : string.Empty; }
            set { ViewState["SecureRefference"] = value; }
        }
        /// <summary>
        /// Gets the resource value and sets it as the label text.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Visible = WebSecurity.IsSecuredAllowed(SecureRefference);
        }
    }
}
