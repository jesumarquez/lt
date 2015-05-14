using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.Security;

namespace Logictracker.Web.CustomWebControls.Panels
{
    public class AbmTabPanel : Panel
    {
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// The name of the data source resource.
        /// </summary>
        [Category("Custom Resources")]
        public string ResourceName
        {
            get { return ViewState["TitleResourceName"] != null ? ViewState["TitleResourceName"].ToString() : string.Empty; }
            set { ViewState["TitleResourceName"] = value; }
        }

        /// <summary>
        /// The name of the specific variable wanted form the resource manager.
        /// </summary>
        [Category("Custom Resources")]
        public string VariableName
        {
            get { return ViewState["TitleVariableName"] != null ? ViewState["TitleVariableName"].ToString() : string.Empty; }
            set { ViewState["TitleVariableName"] = value; }
        }

        public string Title
        {
            get { return ViewState["Title"] != null ? ViewState["Title"].ToString() : string.Empty; }
            set { ViewState["Title"] = value; }
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
            base.Visible = WebSecurity.IsSecuredAllowed(SecureRefference);
            ScriptManager.RegisterStartupScript(Page, typeof(string), ClientID + "_" + DateTime.UtcNow.Ticks, string.Format("tabvisible('{0}',{1});", ClientID, Show ? "true" : "false"), true);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var title = !string.IsNullOrEmpty(ResourceName)
                            ? CultureManager.GetString(ResourceName, VariableName)
                            : Title;

            if (Selected != null) writer.AddAttribute("dopb", "true");
            writer.AddAttribute("command", ClientID);
            writer.AddAttribute("title", title);
            writer.RenderBeginTag("div");
            base.Render(writer);
            writer.RenderEndTag();
        }

        public void InvokeSelected()
        {
            if (Selected != null) Selected(this, EventArgs.Empty);
        }

        public bool Show
        {
            get { return base.Visible && (bool) (ViewState["IsVisible"] ?? true); }
            set { ViewState["IsVisible"] = value; }
        }
    }
}
