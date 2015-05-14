using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Web.CustomWebControls.CheckBoxs
{
    [ToolboxData("<{0}:ResourceCheckBox ID=\"ResourceCheckBox1\" runat=\"server\"></{0}:ResourceCheckBox>")]
    public class ResourceCheckBox :CheckBox
    {
        #region Public Properties

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
        }

        #endregion
    }
}