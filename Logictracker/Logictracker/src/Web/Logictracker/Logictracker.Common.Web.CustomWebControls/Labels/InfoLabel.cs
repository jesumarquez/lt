using System;
using System.ComponentModel;
using System.Web.UI;

namespace Logictracker.Web.CustomWebControls.Labels
{
    public enum InfoLabelMode{INFO, ERROR }

    [DefaultProperty("Text")]
    [Themeable(true)]
    [ToolboxData("<{0}:InfoLabel ID=\"InfoLabel1\" runat=\"server\"></{0}:InfoLabel>")]
    public class InfoLabel : UpdatePanel
    {
        #region Public Properties

        /// <summary>
        /// The associated message text.
        /// </summary>
        [Localizable(false),Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public string Text { get; set; }

        /// <summary>
        /// The error css class used to display the control content in error mode.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ErrorCssClass
        {
            get { return ((string)ViewState["ErrorCssClass"] ?? string.Empty); }
            set { ViewState["ErrorCssClass"] = value; }
        }

        /// <summary>
        /// The info css class used to display the control content in info mode.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string InfoCssClass
        {
            get { return ((string)ViewState["InfoCssClass"] ?? string.Empty); }
            set { ViewState["InfoCssClass"] = value; }
        }

        /// <summary>
        /// Defines the mode in wich the info label behaives.
        /// </summary>
        [Bindable(true)]
        [Category("Behaviour")]
        [DefaultValue("")]
        [Localizable(true)]
        public InfoLabelMode Mode
        {
            get { return (InfoLabelMode)(ViewState["Mode"] ?? InfoLabelMode.ERROR); }
            set { ViewState["Mode"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets the update mode to always and the render mode to inline.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateMode = UpdatePanelUpdateMode.Always;
            RenderMode = UpdatePanelRenderMode.Inline;
        }

        /// <summary>
        /// Renders the control with the givenn text and css class dependign on the label defined mode.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            var text = Text != null ? Text.Trim() : Text;

            if (!string.IsNullOrEmpty(text))
            {
                var css = Mode == InfoLabelMode.ERROR ? ErrorCssClass : InfoCssClass;

                var lit = new LiteralControl(string.Format("<div id='{0}_content' class='{1}'>{2}</div>", ClientID, css, Text));

                ContentTemplateContainer.Controls.Add(lit);
            }

            base.Render(writer);
        }

        #endregion
    }
}
