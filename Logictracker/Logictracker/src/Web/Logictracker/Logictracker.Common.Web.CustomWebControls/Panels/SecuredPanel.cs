using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Logictracker.Security;

namespace Logictracker.Web.CustomWebControls.Panels
{
    public class SecuredPanel: Panel
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

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Visible = WebSecurity.IsSecuredAllowed(SecureRefference);
        }
    }
}
