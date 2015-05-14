using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.CustomWebControls.Helpers;

namespace Logictracker.Web.CustomWebControls.Buttons
{
    [ToolboxData("<{0}:ResourceLinkButton ID=\"ResourceLinkButton1\" runat=\"server\"></{0}:ResourceLinkButton>")]
    public class ResourceLinkButton : LinkButton
    {
        #region Protected Properties

        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public string ResourceName
        {
            get { return ViewState["ResourceName"] != null ? ViewState["ResourceName"].ToString() : string.Empty; }
            set { ViewState["ResourceName"] = value; }
        }

        /// <summary>
        /// The name of the specific variable wanted form the resource manager.
        /// </summary>
        [Category("Custom Resources")]
        public string VariableName
        {
            get { return ViewState["VariableName"] != null ? ViewState["VariableName"].ToString() : string.Empty; }
            set { ViewState["VariableName"] = value; }
        }

        /// <summary>
        /// ID of the target ListControl to manage multi select/unselect.
        /// </summary>
        public string ListControlTargetID
        {
            get { return ViewState["ControlID"] != null ? ViewState["ControlID"].ToString() : string.Empty; }
            set { ViewState["ControlID"] = value; }
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
        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the resource value and sets it as the label text.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            Text = CultureManager.GetString(ResourceName, VariableName);
            Visible = WebSecurity.IsSecuredAllowed(SecureRefference);

            if (!String.IsNullOrEmpty(ListControlTargetID)) RegisterScript();
        }

        /// <summary>
        /// Finds the control asociated to the givenn id.
        /// </summary>
        /// <param name="parent">The id of a parent control.</param>
        /// <param name="controls">A list of controls.</param>
        /// <returns>The parent control or null.</returns>
        private static Control FindParent(string parent, ControlCollection controls)
        {
            if (controls == null) return null;

            foreach (Control control in controls)
            {
                if (!string.IsNullOrEmpty(control.ID) && control.ID.Equals(parent)) return control;

                var cnt = FindParent(parent, control.Controls);

                if (cnt != null) return cnt;
            }

            return null;
        }

        protected void RegisterScript()
        {
            var ctrl = FindParent(ListControlTargetID, Page.Controls) as ListControl;
            var scriptHelper = new ScriptHelper(this);

            const string script = @"function selectCombo(c){{ 
                var cb = $get(c); 
                var st = cb.selectedIndex < 0;

                var opts = cb.getElementsByTagName('option');
                for(var i = 0; i < opts.length; i++) opts[i].selected = st;
            }}";

            scriptHelper.RegisterStartupScript("selectCombo", script);

            OnClientClick = string.Format("selectCombo('{0}');{1};return false;", ctrl.ClientID,
                ctrl.AutoPostBack ? Page.ClientScript.GetPostBackEventReference(ctrl, string.Empty) : "");
        }

        #endregion
    }
}
