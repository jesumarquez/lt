using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Culture;

namespace Logictracker.Web.CustomWebControls.ToolBar
{
    /// <summary>
    /// Custom titlebar control.
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TitleBar ID=\"TitleBar1\" runat=\"server\"></{0}:TitleBar>")]
    public class TitleBar : Panel
    {
        #region Public Properties

        /// <summary>
        /// Associated titlebar text for displaying as the title of the page.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get { return ((String)ViewState["Text"] ?? String.Empty); }
            set { ViewState["Text"] = value; }
        }

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
        /// Css class to be applyied to the title text.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TitleCssClass
        {
            get { return ((String)ViewState["TitleCssClass"] ?? String.Empty); }
            set { ViewState["TitleCssClass"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Overrides control rendering tasks.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer) { RenderContents(writer); }

        /// <summary>
        /// Render all associated controls.
        /// </summary>
        /// <param name="output"></param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            var text = string.IsNullOrEmpty(Text.Trim()) ? CultureManager.GetString(ResourceName, VariableName) : Text;

            output.Write(string.Format("<div class='{0}'>", CssClass));
            output.Write("<table style='width: 100%;'><tr><td style='vertical-align: middle;'>");
            output.Write(string.Format("<span class='{0}'>{1}</span>", TitleCssClass, text));
            output.Write("</td><td style='text-align: right;vertical-align: top;'>");

            foreach (Control control in Controls) control.RenderControl(output);

            output.Write("</td>");
            output.Write("</tr></table></div>");
        }

        #endregion
    }
}