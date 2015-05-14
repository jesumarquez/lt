using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Web.CustomWebControls.Panels
{
    public class ResourcePanel : Panel
    {
        #region Public Properties

        /// <summary>
        /// The name of the resource set wich contains the resource to be displayed.
        /// </summary>
        public string ResourceName
        {
            get { return (string)ViewState["ResourceName"]; }
            set { ViewState["ResourceName"] = value; }
        }

        /// <summary>
        /// The resource name to be displayed.
        /// </summary>
        public string VariableName
        {
            get { return (string)ViewState["VariableName"]; }
            set { ViewState["VariableName"] = value; }
        }

        /// <summary>
        /// Sets the grouping text for the panel.
        /// </summary>
        public override string GroupingText
        {
            get { return ResourceName != null ? CultureManager.GetString(ResourceName, VariableName) : base.GroupingText; }
            set { base.GroupingText = value; }
        }
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
        #endregion
    }
}
