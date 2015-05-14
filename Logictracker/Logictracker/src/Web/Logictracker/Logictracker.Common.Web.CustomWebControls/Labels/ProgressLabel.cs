using System;
using System.ComponentModel;
using System.Web.UI;
using Logictracker.Culture;

namespace Logictracker.Web.CustomWebControls.Labels
{
    /// <summary>
    /// Progress label custom control.
    /// </summary>
    [DefaultProperty("Text")]
    [Themeable(true)]
    [ToolboxData("<{0}:ProgressLabel ID=\"ProgressLabel1\" runat=\"server\"></{0}:ProgressLabel>")]
    public class ProgressLabel : UpdateProgress
    {
        #region Public Properties

        /// <summary>
        /// Associated progress text.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get { return ((String)ViewState["Text"] ?? CultureManager.GetSystemMessage("WAIT")); }
            set { ViewState["Text"] = value; }
        }

        /// <summary>
        /// CssClass for rendering and display control.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string CssClass
        {
            get { return ((String)ViewState["CssClass"] ?? String.Empty); }
            set { ViewState["CssClass"] = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Adds a custom tamplate as the proggress template.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ProgressTemplate = new CustomTemplate();
        }

        /// <summary>
        /// Adds a custom template as the progress template when the proggres label is a child control.
        /// </summary>
        protected override void CreateChildControls()
        {
            if(ProgressTemplate == null) ProgressTemplate = new CustomTemplate();

            base.CreateChildControls();
        }

        /// <summary>
        /// Renders the control using the givenn text and css class.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            Controls.Add(new LiteralControl(string.Format("<div class='{0}'>{1}</div>", CssClass, Text)));

            base.OnPreRender(e);
        }

        #endregion
    }

    /// <summary>
    /// Custom ITemplate implementation.
    /// </summary>
    internal class CustomTemplate : ITemplate
    {
        #region ITemplate Members

        /// <summary>
        /// Instanciates a custom template within the givenn container.
        /// </summary>
        /// <param name="container"></param>
        public void InstantiateIn(Control container) { }

        #endregion
    }
}